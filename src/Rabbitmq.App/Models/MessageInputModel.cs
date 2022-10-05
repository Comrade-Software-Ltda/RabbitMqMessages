namespace Rabbitmq.App.Models;

public class MessageInputModel
{
    public MessageInputModel()
    {
        Id = -1;
        ModelName = "";
        MethodName = "";
        Params = "";
        CreatedAt = DateTime.Now;
    }
    public int Id { get; set; }
    public string ModelName { get; set; }
    public string MethodName { get; set; }
    public string Params { get; set; }
    public DateTime CreatedAt { get; }

    public string GetRequestUri()
    {
        return ModelName + "/" + MethodName;
    }

    public string GetRequestUriWithId()
    {
        var result = GetRequestUri();
        var id = JsonObjectUtil.ReturnJsonPropertyValue("id", Params);
        return string.IsNullOrWhiteSpace(id) ? result : result + "/" + id;
    }
}