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

    [HttpGet("restaurants")]
    public IActionResult GetRestaurants()
    {
      var result = _context.Restaurants.OrderBy(restaurant => restaurant.Name);
      return Ok(result);
    }
    [HttpGet("users")]
    public IActionResult GetUsers()
    {
      var result = _context.Users.Include(user => user.Cart).ThenInclude(cart => cart.LineItems).OrderBy(user => user.FirstName);
      return Ok(result);
    }
    [HttpGet("carts")]
    public IActionResult GetCarts()
    {
      var result = _context.Carts.Include(cart => cart.LineItems).OrderBy(cart => cart.Id);
      return Ok(result);
    }
  }
}
