using System;
using System.Linq;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using YemekGetir.DBOperations;
using YemekGetir.Entities;

namespace YemekGetir.Application.UserOperations.Commands.DeleteUser
{
  public class DeleteUserCommand
  {
    public string Id { get; set; }
    private readonly IYemekGetirDbContext _dbContext;
    private readonly IHttpContextAccessor _httpContextAccessor;

    public DeleteUserCommand(IYemekGetirDbContext dbContext, IHttpContextAccessor httpContextAccessor)
    {
      _dbContext = dbContext;
      _httpContextAccessor = httpContextAccessor;
    }

    public void Handle()
    {
      User user = _dbContext.Users.SingleOrDefault(user => Convert.ToString(user.Id) == Id);
      if (user is null)
      {
        throw new InvalidOperationException("Kullanıcı bulunamadı.");
      }

      string requestOwnerId = _httpContextAccessor.HttpContext.User.Claims.SingleOrDefault(claim => claim.Type == "tokenHolderId").Value;
      if (requestOwnerId != Convert.ToString(user.Id))
      {
        throw new InvalidOperationException("Yalnızca kendi hesabınızı silebilirsiniz.");
      }

      _dbContext.Users.Remove(user);
      _dbContext.SaveChanges();
    }
  }
}