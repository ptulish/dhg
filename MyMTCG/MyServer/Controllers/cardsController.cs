using System.Text.Json;
using Microsoft.AspNetCore.Mvc;
using MyServer.Classes;
using MyServer.DB;

namespace MyServer.Controllers;


[Route("[controller]")]
[ApiController]
public class cardsController : ControllerBase
{
    [HttpGet("cards/{user}")]
    public IActionResult getCardsFromUser(int user)
    {
        List<Card> answerFromDB = DBCommands.getCardsFromUser(user);
        string json = JsonSerializer.Serialize(answerFromDB);

        return Ok(json);
    }
}