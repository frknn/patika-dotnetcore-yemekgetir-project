using System;
using System.Linq;
using YemekGetir.DBOperations;
using YemekGetir.Entities;
using YemekGetir.TokenOperations;
using YemekGetir.TokenOperations.Models;
using Microsoft.Extensions.Configuration;

namespace YemekGetir.Application.RestaurantOperations.Commands.RefreshToken
{
  public class RefreshTokenCommand
  {
    public string RefreshToken { get; set; }
    private readonly IYemekGetirDbContext _dbContext;
    private readonly IConfiguration _configuration;

    public RefreshTokenCommand(IYemekGetirDbContext dbContext, IConfiguration configuration)
    {
      _dbContext = dbContext;
      _configuration = configuration;
    }

    public Token Handle()
    {
      Restaurant restaurant = _dbContext.Restaurants.FirstOrDefault(restaurant => restaurant.RefreshToken == RefreshToken && restaurant.RefreshTokenExpireDate > DateTime.Now);
      if (restaurant is not null)
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
        throw new InvalidOperationException("Geçerli bir Refresh Token bulunamadı.");
      }
    }
  }
}
