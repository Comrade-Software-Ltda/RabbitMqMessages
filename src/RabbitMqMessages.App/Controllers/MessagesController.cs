using System;
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
            _factory.PostMessage(message);
            return Accepted();
        }
        catch (Exception e)
        {
            return StatusCode(StatusCodes.Status500InternalServerError, e);
        }
    }
}