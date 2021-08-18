using System;

namespace MovieStore.Entities
{
  public interface ITokenHolder
  {
    public Guid Id { get; set; }
    public string RefreshToken { get; set; }
    public DateTime? RefreshTokenExpireDate { get; set; }
    public string Email { get; set; }
    public string Password { get; set; }
    public Address Address { get; set; }
  }
}