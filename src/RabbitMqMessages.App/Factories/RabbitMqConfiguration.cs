namespace RabbitMqMessages.App.Factories;

public class RabbitMqConfiguration
{
    public RabbitMqConfiguration()
    {
        Host = "localhost";
        Port = 5672;
        Exchange = "amq.fanout";
        Queue = "main";
        VirtualHost = "/";
        UserName = "admin";
        Password = "pass123";
    }
    public string Host { get; }
    public int Port { get; }
    public string Exchange { get; }
    public string Queue { get; }
    public string VirtualHost { get; }
    public string UserName { get; }
    public string Password { get; }
}