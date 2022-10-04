using System;

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
}