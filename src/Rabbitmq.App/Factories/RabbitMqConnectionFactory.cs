using System.Text;
using Microsoft.Extensions.DependencyInjection;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using Rabbitmq.App.Services;

namespace Rabbitmq.App.Factories;

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
        Console.WriteLine("[INFO] # # # # # # # # # # # # # # # # # # # # # # # # # # # # # # # # # # # # # # # #");
        _serviceProvider = serviceProvider;
        InitRabbitMqFactory();
        Console.WriteLine("[INFO] # # # # # # # # # # # # # # # # # # # # # # # # # # # # # # # # # # # # # # # #");
    }

    private void InitRabbitMqFactory()
    {
        try
        {
            Console.WriteLine("[INFO] Initializing RabbitMq factory...");
            _configuration = new RabbitMqConfiguration();
            Console.WriteLine("[DEBUG] RabbitMq configurations:\n" + JsonObjectUtil.Serialize(_configuration));
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
            Console.WriteLine("[INFO] ...RabbitMq factory done.");
            NewConnection();
        }
        catch (Exception ex)
        {
            Console.WriteLine("[ERROR] Error while initializing RabbitMq factory:" + ex.Message);
        }
    }

    public void PostMessage(MessageInputModel message)
    {
        var stringfiedMessage = JsonObjectUtil.Serialize(message);
        var bytesMessage = Encoding.UTF8.GetBytes(stringfiedMessage);
        KeepConnectionIntegrity();
        Console.WriteLine("[INFO] Publishing in exchange: " + _configuration.Exchange + " -> (routingKey) " + _configuration.Queue);
        _channel.BasicPublish(
            exchange: _configuration.Exchange,
            routingKey: _configuration.Queue,
            basicProperties: null,
            body: bytesMessage);
    }

    public void ConsumeMessages()
    {
        KeepConnectionIntegrity();
        _consumer.Received += async (sender, eventArgs) =>
        {
            Console.WriteLine("[DEBUG] Sender:\n" + JsonObjectUtil.Serialize(sender));
            var contentArray = eventArgs.Body.ToArray();
            var contentString = Encoding.UTF8.GetString(contentArray);
            var message = JsonObjectUtil.Deserialize<MessageInputModel>(contentString);
            await Notify(message);
            _channel.BasicAck(eventArgs.DeliveryTag, false);
        };
        Console.WriteLine("[INFO] Consuming from queue: " + _configuration.Queue);
        _channel.BasicConsume(_configuration.Queue, false, _consumer);
    }

    private async Task Notify(MessageInputModel message)
    {
        var scope = _serviceProvider.CreateScope();
        var notificationService = scope.ServiceProvider.GetRequiredService<INotificationService>();
        await notificationService.Notify(message);
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
            Console.WriteLine("[DEBUG] RabbitMq connection:\n" + JsonObjectUtil.Serialize(_connection));
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
            Console.WriteLine("[DEBUG] RabbitMq channel:\n" + JsonObjectUtil.Serialize(_channel));
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
            Console.WriteLine("[DEBUG] RabbitMq consumer:\n" + JsonObjectUtil.Serialize(_consumer));
            Console.WriteLine("[INFO] ...New RabbitMq consumer done.");
        }
        catch (Exception ex)
        {
            Console.WriteLine("[ERROR] Error while creating new RabbitMq consumer:" + ex.Message);
        }
    }
}