using RabbitMqMessages.App.Models;

namespace RabbitMqMessages.App.Services;

public interface INotificationService
{
    void NotifyUser(MessageInputModel messageInputModel);
}