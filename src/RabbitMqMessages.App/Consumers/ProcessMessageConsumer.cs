using Microsoft.Extensions.Hosting;
using System.Threading;
using System.Threading.Tasks;
using RabbitMqMessages.App.Factories;

namespace RabbitMqMessages.App.Consumers;

public class ProcessMessageConsumer : BackgroundService
{
    private readonly IRabbitMqConnectionFactory _factory;
    
    public ProcessMessageConsumer(IRabbitMqConnectionFactory factory)
    {
        _factory = factory;
    }

    protected override Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _factory.ConsumeMessages();
        return Task.CompletedTask;
    }
}