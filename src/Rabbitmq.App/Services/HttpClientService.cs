using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using Newtonsoft.Json.Linq;

namespace Rabbitmq.App.Services;

public class HttpClientService : IHttpClientService
{
    private HttpClient _client;
    public Uri BaseAddressUri { get; set; }

    public HttpClientService()
    {
        BaseAddressUri = new Uri("https://localhost:44304/api/v1/");
    }

    private void InitHttpClientService()
    {
        Console.WriteLine("[INFO] Initializing http client...");
        _client = new HttpClient
        {
            BaseAddress = BaseAddressUri
        };
        _client.DefaultRequestHeaders.Accept.Clear();
        _client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
        Console.WriteLine("[DEBUG] Http client:\n" + JsonObjectUtil.Serialize(_client));
        Console.WriteLine("[INFO] ...Http client done.");
    }

    public async Task<ApiResponseModel> CallApiAsync(MessageInputModel message)
    {
        Console.WriteLine("[INFO] Call api async method. Message:\n" + JsonObjectUtil.Serialize(message));
        var errorMessage = "";
        ApiResponseModel result;
        try
        {
            InitHttpClientService();
            switch (message.MethodName)
            {
                case "get-all":
                    result = await GetAllAsync(message);
                    break;
                case "get-by-id":
                    result = await GetByIdAsync(message);
                    break;
                case "create":
                    result = await PostAsync(message);
                    break;
                case "edit":
                    result = await UpdateAsync(message);
                    break;
                case "delete":
                    result = await DeleteAsync(message);
                    break;
                default:
                    errorMessage = "[ERROR] Unavailable informed method: " + message.MethodName;
                    result = new ApiResponseModel(HttpStatusCode.BadRequest, errorMessage);
                    break;
            }
        }
        catch (Exception ex)
        {
            errorMessage = "[ERROR] Error in " + message.MethodName + " async method:" + ex.Message;
            result = new ApiResponseModel(HttpStatusCode.InternalServerError, errorMessage);
        }
        Console.WriteLine(errorMessage);
        return result;
    }

    private async Task<ApiResponseModel> GetAllAsync(MessageInputModel message)
    {
        Console.WriteLine("[INFO] Get all async method.");
        var response = await _client.GetAsync(message.GetRequestUri());
        return await BuildResponseContent(response);
    }

    private async Task<ApiResponseModel> GetByIdAsync(MessageInputModel message)
    {
        Console.WriteLine("[INFO] Get by id async method.");
        var response = await _client.GetAsync(message.GetRequestUriWithId());
        return await BuildResponseContent(response);
    }

    private async Task<ApiResponseModel> PostAsync(MessageInputModel message)
    {
        Console.WriteLine("[INFO] Post async method.");
        var response = await _client.PostAsJsonAsync(message.GetRequestUri(), JObject.Parse(message.Params));
        return await BuildResponseContent(response);
    }

    private async Task<ApiResponseModel> UpdateAsync(MessageInputModel message)
    {
        Console.WriteLine("[INFO] Update async method.");
        var response = await _client.PutAsJsonAsync(message.GetRequestUri(), JObject.Parse(message.Params));
        return await BuildResponseContent(response);
    }

    private async Task<ApiResponseModel> DeleteAsync(MessageInputModel message)
    {
        Console.WriteLine("[INFO] Delete async method.");
        var response = await _client.DeleteAsync(message.GetRequestUriWithId());
        return await BuildResponseContent(response);
    }

    private static async Task<ApiResponseModel> BuildResponseContent(HttpResponseMessage response)
    {
        Console.WriteLine("[INFO] Build response content method.");
        Console.WriteLine("[DEBUG] Response:\n" + JsonObjectUtil.Serialize(response));
        var content = await response.Content.ReadAsStringAsync();
        Console.WriteLine("[DEBUG] Content:\n"  + JsonObjectUtil.Serialize(content));
        return new ApiResponseModel(response.StatusCode, content);
    }
}