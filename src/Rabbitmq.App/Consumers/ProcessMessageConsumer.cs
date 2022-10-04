using System;
using Microsoft.Extensions.Hosting;
using System.Threading;
using System.Threading.Tasks;
using Rabbitmq.App.Factories;

namespace Rabbitmq.App.Consumers;

public class ProcessMessageConsumer : BackgroundService
{
    private readonly IRabbitMqConnectionFactory _factory;

    public ProcessMessageConsumer(IRabbitMqConnectionFactory factory)
    {
        _factory = factory;
    }

    protected override Task ExecuteAsync(CancellationToken stoppingToken)
    {
        try
        {
            Console.WriteLine("[INFO] Initializing new message consumption...");
            _factory.ConsumeMessages();
            Console.WriteLine("[INFO] ...New message consumption done.");
        }
        catch (Exception ex)
        {
            Console.WriteLine("[ERROR] Error while consuming message: " + ex.Message);
        }
        return Task.CompletedTask;
    }
}