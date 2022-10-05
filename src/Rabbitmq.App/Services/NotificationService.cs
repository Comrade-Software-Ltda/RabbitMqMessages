namespace Rabbitmq.App.Services;

public class NotificationService : INotificationService
{
    private readonly IHttpClientService _httpClientService;

    public NotificationService(IHttpClientService httpClientService)
    {
        _httpClientService = httpClientService;
    }

    public async Task Notify(MessageInputModel message)
    {
        var response = await _httpClientService.CallApiAsync(message);
        Console.WriteLine("[INFO] Message response:\n" + JsonObjectUtil.Serialize(response));
    }
}