namespace Rabbitmq.App.Factories;

public class RabbitMqConfiguration
{
    public string Host { get; set; }
    public int Port { get; set; }
    public string Exchange { get; set; }
    public string Queue { get; set; }
    public string VirtualHost { get; set; }
    public TimeSpan RequestedHeartbeat { get; set; }
    public string UserName { get; set; }
    public string Password { get; set; }
    public bool EnableSsl { get; set; }
}