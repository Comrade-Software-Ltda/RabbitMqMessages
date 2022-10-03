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
        _serviceProvider = serviceProvider;
        InitRabbitMqFactory();
    }

    private void InitRabbitMqFactory()
    {
        _configuration = new RabbitMqConfiguration();
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
        NewConnection();
    }

    public void PostMessage(MessageInputModel message)
    {
        KeepConnectionIntegrity();
        var stringfiedMessage = JsonConvert.SerializeObject(message);
        var bytesMessage = Encoding.UTF8.GetBytes(stringfiedMessage);
        _channel.BasicPublish(
            exchange: _configuration.Exchange,
            routingKey: _configuration.Queue,
            basicProperties: null,
            body: bytesMessage);
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
                NotifyUser(message);
            }
            _channel.BasicAck(eventArgs.DeliveryTag, false);
        };
        _channel.BasicConsume(_configuration.Queue, false, _consumer);
    }

    private void NotifyUser(MessageInputModel message)
    {
        var scope = _serviceProvider.CreateScope();
        var notificationService = scope.ServiceProvider.GetRequiredService<INotificationService>();
        notificationService.NotifyUser(message);
    }

    private void KeepConnectionIntegrity()
    {
        if (_connection.IsOpen)
        {
            if (_channel.IsOpen) return;
            _channel.Dispose();
            NewChannel();
        }
        else
        {
            _connection.Dispose();
            NewConnection();
        }
    }

    private void NewConnection()
    {
        _connection = _factory.CreateConnection();
        NewChannel();
    }

    private void NewChannel()
    {
        _channel = _connection.CreateModel();
        NewQueue();
        NewConsumer();
    }

    private void NewQueue()
    {
        _channel.QueueDeclare(
            queue: _configuration.Queue,
            durable: false,
            exclusive: false,
            autoDelete: false,
            arguments: null
        );
        _channel.QueueBind(
            queue: _configuration.Queue,
            exchange: _configuration.Exchange,
            routingKey: _configuration.Queue,
            arguments: null
        );
    }

    private void NewConsumer()
    {
        _consumer = new EventingBasicConsumer(_channel);
    }
}