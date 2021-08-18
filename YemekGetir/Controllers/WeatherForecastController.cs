using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using YemekGetir.DBOperations;

namespace YemekGetir.Controllers
{
  [ApiController]
  [Route("[controller]")]
  public class WeatherForecastController : ControllerBase
  {
    private static readonly string[] Summaries = new[]
    {
            "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
        };

    private readonly ILogger<WeatherForecastController> _logger;
    private readonly IYemekGetirDbContext _context;

    public WeatherForecastController(ILogger<WeatherForecastController> logger, IYemekGetirDbContext context)
    {
      _logger = logger;
      _context = context;
    }

    [HttpGet]
    public IActionResult Get()
    {
      // var rng = new Random();
      // return Enumerable.Range(1, 5).Select(index => new WeatherForecast
      // {
      //     Date = DateTime.Now.AddDays(index),
      //     TemperatureC = rng.Next(-20, 55),
      //     Summary = Summaries[rng.Next(Summaries.Length)]
      // })
      // .ToArray();

      var result = _context.Users.OrderBy(user => user.Id);
      return Ok(result);
    }
  }
}
