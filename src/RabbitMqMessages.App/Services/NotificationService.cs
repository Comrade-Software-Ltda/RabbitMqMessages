using System;
using System.Text.Json;
using RabbitMqMessages.App.Models;

namespace RabbitMqMessages.App.Services;

public class NotificationService : INotificationService
{
    public void Notify(MessageInputModel message)
    {
        Console.WriteLine("[INFO] New message:\n" + JsonSerializer.Serialize(message));
    }
}