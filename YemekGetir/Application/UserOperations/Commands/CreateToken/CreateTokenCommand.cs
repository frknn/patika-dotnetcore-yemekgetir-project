using System;
using System.Linq;
using YemekGetir.DBOperations;
using YemekGetir.Entities;
using YemekGetir.TokenOperations;
using YemekGetir.TokenOperations.Models;
using Microsoft.Extensions.Configuration;

namespace YemekGetir.Application.UserOperations.Commands.CreateToken
{
  public class CreateTokenCommand
  {
    public LoginModel Model { get; set; }
    private readonly IYemekGetirDbContext _dbContext;
    private readonly IConfiguration _configuration;

    public CreateTokenCommand(IYemekGetirDbContext dbContext, IConfiguration configuration)
    {
      _dbContext = dbContext;
      _configuration = configuration;
    }

    public Token Handle()
    {
      User user = _dbContext.Users.FirstOrDefault(user => user.Email == Model.Email);

      if (user is not null && BCrypt.Net.BCrypt.Verify(Model.Password, user.Password))
      {
        TokenHandler handler = new TokenHandler(_configuration);
        Token token = handler.CreateAccessToken(user);
        user.RefreshToken = token.RefreshToken;
        user.RefreshTokenExpireDate = token.ExpirationDate.AddMinutes(5);

        _dbContext.SaveChanges();
        return token;
      }
      else
      {
        throw new InvalidOperationException("Kulanıcı adı veya şifre yanlış.");
      }
    }

  }

  public class LoginModel
  {
    public string Email { get; set; }
    public string Password { get; set; }
  }
}