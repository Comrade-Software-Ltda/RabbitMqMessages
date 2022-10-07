namespace Rabbitmq.App.Services;

public interface IHttpClientService
{
    Task<ApiResponseModel> CallApiAsync(MessageInputModel message, string baseAddressUri);
}