namespace Rabbitmq.App.Services;

public class NotificationService : INotificationService
{
    public void Notify(MessageInputModel message)
    {
        Console.WriteLine("[INFO] New message:\n" + JsonObjectUtil.Serialize(message));
    }
}