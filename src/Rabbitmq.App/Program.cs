using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;

namespace Rabbitmq.App;

public class Program
{
    public static void Main(string[] args)
    {
        try
        {
            Console.WriteLine("[INFO] # # # # # # # # # # # # # # # # # # # # # # # # # # # # # # # # # # # # # # # #");
            var hostBuilder = CreateHostBuilder(args).Build();
            Console.WriteLine("[INFO] Starting up application...");
            hostBuilder.Run();
        }
        catch (Exception ex)
        {
            Console.WriteLine("[ERROR] Application start-up failed: " + ex.Message);
        }
        finally
        {
            Console.WriteLine("[INFO] ...Ending application.");
            Console.WriteLine("[INFO] # # # # # # # # # # # # # # # # # # # # # # # # # # # # # # # # # # # # # # # #");
        }
    }

    public static IHostBuilder CreateHostBuilder(string[] args) =>
        Host.CreateDefaultBuilder(args)
            .ConfigureWebHostDefaults(webBuilder =>
            {
                webBuilder.UseStartup<Startup>();
            });
}