namespace Rabbitmq.App.Services;

public interface INotificationService
{
    void Notify(MessageInputModel message);
}