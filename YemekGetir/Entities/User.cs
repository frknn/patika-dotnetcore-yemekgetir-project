using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MovieStore.Entities
{
  public class User : ITokenHolder
  {
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public Guid Id { get; set; }
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public int AddressId { get; set; }
    public Address Address { get; set; }
    public string PhoneNumber { get; set; }
    public int Wallet { get; set; } = 0;
    public string Email { get; set; }
    public string Password { get; set; }
    public string RefreshToken { get; set; }
    public DateTime? RefreshTokenExpireDate { get; set; }

  }
}