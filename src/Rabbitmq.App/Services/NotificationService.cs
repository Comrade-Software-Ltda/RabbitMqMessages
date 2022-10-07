using Microsoft.Extensions.Configuration;

namespace Rabbitmq.App.Services;

public class NotificationService : INotificationService
{
    private readonly IConfiguration _configuration;
    private readonly IHttpClientService _httpClientService;

    public NotificationService(IConfiguration configuration, IHttpClientService httpClientService)
    {
        _configuration = configuration;
        _httpClientService = httpClientService;
    }

    public async Task Notify(MessageInputModel message)
    {
        var baseAddressUri = _configuration.GetValue<string>("EnvironmentVariables:COMRADE_BACKEND_PROJECT_URI");
        Console.WriteLine("[INFO] Backend uri: " + baseAddressUri);
        var response = await _httpClientService.CallApiAsync(message, baseAddressUri);
        Console.WriteLine("[INFO] Message response:\n" + JsonObjectUtil.Serialize(response));
    }
}