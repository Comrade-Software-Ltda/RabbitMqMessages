using System;
using System.Text.Json;
using Rabbitmq.App.Models;

namespace Rabbitmq.App.Services;

public class NotificationService : INotificationService
{
    public void Notify(MessageInputModel message)
    {
        Console.WriteLine("[INFO] New message:\n" + JsonSerializer.Serialize(message));
    }
}