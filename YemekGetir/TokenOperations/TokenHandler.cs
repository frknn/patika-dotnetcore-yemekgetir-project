using System;
using System.IdentityModel.Tokens.Jwt;
using System.Text;
using YemekGetir.TokenOperations.Models;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.Security.Claims;
using YemekGetir.Entities;

namespace YemekGetir.TokenOperations
{
  public class TokenHandler
  {
    public IConfiguration Configuration { get; set; }
    public TokenHandler(IConfiguration configuration)
    {
      Configuration = configuration;
    }

    public Token CreateAccessToken(ITokenHolder tokenHolder)
    {
      Token tokenModel = new Token();
      SymmetricSecurityKey key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(Configuration["Token:SecurityKey"]));

      SigningCredentials signingCredentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
      tokenModel.ExpirationDate = DateTime.Now.AddMinutes(15);

      JwtSecurityToken securityToken = new JwtSecurityToken(
        issuer: Configuration["Token:Issuer"],
        audience: Configuration["Token:Audience"],
        expires: tokenModel.ExpirationDate,
        notBefore: DateTime.Now,
        signingCredentials: signingCredentials,
        claims: new[] { new Claim("tokenHolderId", tokenHolder.Id.ToString()) }
      );

      JwtSecurityTokenHandler tokenHandler = new JwtSecurityTokenHandler();

      // Token burada yaratılıyor.
      tokenModel.AccessToken = tokenHandler.WriteToken(securityToken);
      tokenModel.RefreshToken = CreateRefreshToken();

      return tokenModel;
    }

    public string CreateRefreshToken()
    {
      return Guid.NewGuid().ToString();
    }
  }
}