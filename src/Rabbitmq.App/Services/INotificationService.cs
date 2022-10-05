namespace Rabbitmq.App.Services;

public interface INotificationService
{
    Task Notify(MessageInputModel message);
}