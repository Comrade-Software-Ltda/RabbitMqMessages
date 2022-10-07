using System.IO;
using System.Text.Json;
using System.Text.Json.Serialization;
using Newtonsoft.Json;
using JsonSerializer = System.Text.Json.JsonSerializer;

namespace Rabbitmq.App.Utils;

public static class JsonObjectUtil
{
    private static readonly JsonSerializerOptions Options = InitJsonSerializerOptions();

    private static JsonSerializerOptions InitJsonSerializerOptions()
    {
        var options = new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        };
        options.Converters.Add(new JsonStringEnumConverter());
        return options;
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

    public static string ReturnJsonPropertyValue(string propertyName, string inputJsonString)
    {
        try
        {
            var reader = new JsonTextReader(new StringReader(inputJsonString));
            while (reader.Read())
            {
                if (JsonToken.PropertyName.Equals(reader.TokenType) && propertyName.Equals(reader.Value))
                {
                    reader.Read();
                    return reader.Value.ToString();
                }
            }
            return "";
        }
        catch (Exception ex)
        {
            Console.WriteLine("[ERROR] Error while trying to return json property value: " + ex.Message);
            return "";
        }
    }
}