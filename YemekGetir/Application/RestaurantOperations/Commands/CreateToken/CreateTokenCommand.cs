using System;
using System.Linq;
using YemekGetir.DBOperations;
using YemekGetir.Entities;
using YemekGetir.TokenOperations;
using YemekGetir.TokenOperations.Models;
using Microsoft.Extensions.Configuration;

namespace YemekGetir.Application.RestaurantOperations.Commands.CreateToken
{
  public class CreateTokenCommand
  {
    public RestaurantLoginModel Model { get; set; }
    private readonly IYemekGetirDbContext _dbContext;
    private readonly IConfiguration _configuration;

    public CreateTokenCommand(IYemekGetirDbContext dbContext, IConfiguration configuration)
    {
      _dbContext = dbContext;
      _configuration = configuration;
    }

    public Token Handle()
    {
      Restaurant restaurant = _dbContext.Restaurants.FirstOrDefault(restaurant => restaurant.Email == Model.Email);

      if (restaurant is not null && BCrypt.Net.BCrypt.Verify(Model.Password, restaurant.Password))
      {
        TokenHandler handler = new TokenHandler(_configuration);
        Token token = handler.CreateAccessToken(restaurant);
        restaurant.RefreshToken = token.RefreshToken;
        restaurant.RefreshTokenExpireDate = token.ExpirationDate.AddMinutes(5);

        _dbContext.SaveChanges();
        return token;
      }
      else
      {
        throw new InvalidOperationException("Kulanıcı adı veya şifre yanlış.");
      }
    }

  }

  public class RestaurantLoginModel
  {
    public string Email { get; set; }
    public string Password { get; set; }
  }
}