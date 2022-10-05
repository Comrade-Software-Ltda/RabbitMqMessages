namespace Rabbitmq.App.Models;

public class MessageOutputModel
{
    public MessageOutputModel()
    {
        Id = -1;
        Content = "";
        CreatedAt = DateTime.Now;
    }

    public MessageOutputModel(string content)
    {
        Id = -1;
        Content = content;
        CreatedAt = DateTime.Now;
    }

    public int Id { get; set; }
    public string Content { get; set; }
    public DateTime CreatedAt { get; }
}