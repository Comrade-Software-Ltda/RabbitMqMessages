using System.Text.Json;
using System.Text.Json.Serialization;

namespace Rabbitmq.App.Utils;

public class JsonObjectUtil
{
    private static readonly JsonSerializerOptions Options;

    static JsonObjectUtil()
    {
        Options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
        Options.Converters.Add(new JsonStringEnumConverter());
    }

    public static string Serialize(object inputJsonObject)
    {
        try
        {
            var obj = JsonSerializer.Serialize(inputJsonObject, Options);
            return obj;
        }
        catch (Exception ex)
        {
            Console.WriteLine("[ERROR] Error while trying to serialize object: " + ex.Message);
            return "";
        }
    }

    public static TEntity Deserialize<TEntity>(string inputJsonString)
    {
        try
        {
            var obj = JsonSerializer.Deserialize<TEntity>(inputJsonString, Options);
            return obj;
        }
        catch (Exception ex)
        {
            Console.WriteLine("[ERROR] Error while trying to deserialize object: " + ex.Message);
            return default;
        }
    }
}