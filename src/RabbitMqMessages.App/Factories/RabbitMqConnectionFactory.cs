using System;
using System.Text;
using RabbitMqMessages.App.Models;
using RabbitMqMessages.App.Services;
using Microsoft.Extensions.DependencyInjection;
using RabbitMQ.Client;
using Newtonsoft.Json;
using RabbitMQ.Client.Events;

namespace RabbitMqMessages.App.Factories;

public class RabbitMqConnectionFactory : IRabbitMqConnectionFactory
{
    private readonly IServiceProvider _serviceProvider;
    private RabbitMqConfiguration _configuration;
    private ConnectionFactory _factory;
    private IConnection _connection;
    private IModel _channel;
    private EventingBasicConsumer _consumer;

    public RabbitMqConnectionFactory(IServiceProvider serviceProvider)
    {
        Console.WriteLine("[INFO] # # # # # # # # # # # # # # # # # # # #");
        _serviceProvider = serviceProvider;
        InitRabbitMqFactory();
        Console.WriteLine("[INFO] # # # # # # # # # # # # # # # # # # # #");
    }

    private void InitRabbitMqFactory()
    {
        try
        {
            Console.WriteLine("[INFO] Initializing RabbitMq factory...");
            _configuration = new RabbitMqConfiguration();
            Console.WriteLine("[INFO] RabbitMq configurations:\n" + System.Text.Json.JsonSerializer.Serialize(_configuration));
            _factory = new ConnectionFactory
            {
                HostName = _configuration.Host,
                Port = _configuration.Port,
                VirtualHost = _configuration.VirtualHost,
                UserName = _configuration.UserName,
                Password = _configuration.Password,
                RequestedHeartbeat = new TimeSpan(60),
                Ssl =
                {
                    ServerName = _configuration.Host,
                    Enabled = false
                }
            };
            //Console.WriteLine("[INFO] RabbitMq connection factory:\n" + System.Text.Json.JsonSerializer.Serialize(_factory));
            NewConnection();
        }
        catch (Exception ex)
        {
            Console.WriteLine("[ERROR] Error while initializing RabbitMq factory:" + ex.Message);
        }
    }

    public void PostMessage(MessageInputModel message)
    {
        var stringfiedMessage = JsonConvert.SerializeObject(message);
        var bytesMessage = Encoding.UTF8.GetBytes(stringfiedMessage);
        if (bytesMessage.Length > 0)
        {
            KeepConnectionIntegrity();
            Console.WriteLine("[INFO] Publishing in exchange: " + _configuration.Exchange + " -> (routingKey) " + _configuration.Queue);
            _channel.BasicPublish(
                exchange: _configuration.Exchange,
                routingKey: _configuration.Queue,
                basicProperties: null,
                body: bytesMessage);
        }
        else
        {
            Console.WriteLine("[WARN] Received message empty.");
        }
    }

    public void ConsumeMessages()
    {
        KeepConnectionIntegrity();
        _consumer.Received += (sender, eventArgs) =>
        {
            var contentArray = eventArgs.Body.ToArray();
            var contentString = Encoding.UTF8.GetString(contentArray);
            var message = JsonConvert.DeserializeObject<MessageInputModel>(contentString);
            if (message != null)
            {
                Notify(message);
            }
            else
            {
                Console.WriteLine("[WARN] Consumed message content null.");
            }
            _channel.BasicAck(eventArgs.DeliveryTag, false);
        };
        Console.WriteLine("[INFO] Consuming from queue: " + _configuration.Queue);
        _channel.BasicConsume(_configuration.Queue, false, _consumer);
    }

    private void Notify(MessageInputModel message)
    {
        var scope = _serviceProvider.CreateScope();
        var notificationService = scope.ServiceProvider.GetRequiredService<INotificationService>();
        notificationService.Notify(message);
    }

    private void KeepConnectionIntegrity()
    {
        if (_connection.IsOpen)
        {
            if (_channel.IsOpen)
            {
                return;
            }
            _channel.Dispose();
            Console.WriteLine("[INFO] RabbitMq channel closed, creating a new one...");
            NewChannel();
        }
        else
        {
            _connection.Dispose();
            Console.WriteLine("[INFO] RabbitMq connection closed, creating a new one...");
            NewConnection();
        }
    }

    private void NewConnection()
    {
        try
        {
            Console.WriteLine("[INFO] Creating new RabbitMq connection...");
            _connection = _factory.CreateConnection();
            Console.WriteLine("[INFO] RabbitMq connection:\n" + System.Text.Json.JsonSerializer.Serialize(_connection));
            Console.WriteLine("[INFO] ...New RabbitMq connection done.");
            NewChannel();
        }
        catch (Exception ex)
        {
            Console.WriteLine("[ERROR] Error while creating new RabbitMq connection:" + ex.Message);
        }
    }

    private void NewChannel()
    {
        try
        {
            Console.WriteLine("[INFO] Creating new RabbitMq channel...");
            _channel = _connection.CreateModel();
            Console.WriteLine("[INFO] RabbitMq channel:\n" + System.Text.Json.JsonSerializer.Serialize(_channel));
            Console.WriteLine("[INFO] ...New RabbitMq channel done.");
            NewQueue();
            NewConsumer();
        }
        catch (Exception ex)
        {
            Console.WriteLine("[ERROR] Error while creating new RabbitMq channel:" + ex.Message);
        }
    }

    private void NewQueue()
    {
        try
        {
            Console.WriteLine("[INFO] Creating new RabbitMq queue: " + _configuration.Queue + "...");
            _channel.QueueDeclare(
                queue: _configuration.Queue,
                durable: false,
                exclusive: false,
                autoDelete: false,
                arguments: null
            );
            Console.WriteLine("[INFO] ...New RabbitMq queue done.");
            NewQueueBind();
        }
        catch (Exception ex)
        {
            Console.WriteLine("[ERROR] Error while creating new RabbitMq queue:" + ex.Message);
        }
    }

    private void NewQueueBind()
    {
        try
        {
            Console.WriteLine("[INFO] Creating new RabbitMq queue bind: " + _configuration.Queue + " <-> " + _configuration.Exchange + "...");
            _channel.QueueBind(
                queue: _configuration.Queue,
                exchange: _configuration.Exchange,
                routingKey: _configuration.Queue,
                arguments: null
            );
            Console.WriteLine("[INFO] ...New RabbitMq queue bind done.");
        }
        catch (Exception ex)
        {
            Console.WriteLine("[ERROR] Error while creating new RabbitMq queue bind:" + ex.Message);
        }
    }

    private void NewConsumer()
    {
        try
        {
            Console.WriteLine("[INFO] Creating new RabbitMq consumer...");
            _consumer = new EventingBasicConsumer(_channel);
            Console.WriteLine("[INFO] RabbitMq consumer:\n" + System.Text.Json.JsonSerializer.Serialize(_consumer));
            Console.WriteLine("[INFO] ...New RabbitMq consumer done.");
        }
        catch (Exception ex)
        {
            Console.WriteLine("[ERROR] Error while creating new RabbitMq consumer:" + ex.Message);
        }
    }
}