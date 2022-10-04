using System;
using System.Text.Json;
using RabbitMqMessages.App.Models;
using Microsoft.AspNetCore.Mvc;
using RabbitMqMessages.App.Factories;
using Microsoft.AspNetCore.Http;

namespace RabbitMqMessages.App.Controllers;

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
            Console.WriteLine("[INFO] Post received message:\n" + JsonSerializer.Serialize(message));
            _factory.PostMessage(message);
            Console.WriteLine("[INFO] ...Post received message done.");
            return Accepted();
        }
        catch (Exception ex)
        {
            Console.WriteLine("[ERROR] Error while posting message: " + ex.Message);
            return StatusCode(StatusCodes.Status500InternalServerError, ex);
        }
    }
}