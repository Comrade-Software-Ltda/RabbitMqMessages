using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using Rabbitmq.App.Factories;

namespace Rabbitmq.App.Controllers;

[Route("api/[controller]")]
[ApiController]
public class MessagesController : ControllerBase
{
    private readonly IRabbitMqConnectionFactory _factory;

    public MessagesController(IRabbitMqConnectionFactory factory)
    {
        _factory = factory;
    }

    [HttpPost("post")]
    [ProducesResponseType(StatusCodes.Status202Accepted)]
    [ProducesDefaultResponseType]
    public IActionResult PostMessage([FromBody] MessageInputModel message)
    {
        try
        {
            Console.WriteLine("[INFO] Post received message:\n" + JsonObjectUtil.Serialize(message));
            _factory.PostMessage(message);
            Console.WriteLine("[INFO] ...Post message done.");
            return Accepted();
        }
        catch (Exception ex)
        {
            Console.WriteLine("[ERROR] Error while posting message: " + ex.Message);
            return StatusCode(StatusCodes.Status500InternalServerError, ex);
        }
    }
}