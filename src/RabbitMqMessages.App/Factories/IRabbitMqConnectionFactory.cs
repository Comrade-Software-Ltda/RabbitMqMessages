using RabbitMqMessages.App.Models;

namespace RabbitMqMessages.App.Factories;

public interface IRabbitMqConnectionFactory
{
    void PostMessage(MessageInputModel message);
    void ConsumeMessages();
}