using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MyServer.Classes;
using MyServer.DB;

namespace MyServer.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class userController : ControllerBase
    {
        [HttpGet("users/{username}")]
        public IActionResult GetUserByUsername(string username)
        {
            var response = DBCommands.getUser(username);

            return Ok(response);
        }
        
        [HttpPost("seccions")]
        public IActionResult Login([FromForm] string username, [FromForm] string password)
        {
            // Здесь вам нужно реализовать логику проверки учетных данных
            // Например, проверьте, существует ли пользователь с указанным именем и паролем
            User answerFromDB = DBCommands.IsValidUser(username, password);

            if (answerFromDB != null)
            {
                // Возвращаем успешный ответ с HTTP-статусом 200 и сообщением
                return Ok(answerFromDB);
            }
            else
            {
                // Возвращаем ответ с HTTP-статусом 401 и сообщением об ошибке
                return Unauthorized("Invalid username/password provided");
            }
        }

        [HttpPost("users")]
        public IActionResult Register([FromForm] string username, [FromForm] string password)
        {
            int answerFromDB = DBCommands.registerNewUSer(username, password);
            return Ok(answerFromDB);
        }
        // Пример логики проверки учетных данных

        
    }
    
}
