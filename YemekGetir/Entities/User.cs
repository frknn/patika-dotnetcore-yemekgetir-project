using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace YemekGetir.Entities
{
  public class User : ITokenHolder
  {
    // [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public Guid Id { get; set; }
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public Address Address { get; set; }
    public string PhoneNumber { get; set; }
    public Cart Cart { get; set; }
    public List<Order> Orders { get; set; }
    public string Email { get; set; }
    public string Password { get; set; }
    public string RefreshToken { get; set; }
    public DateTime? RefreshTokenExpireDate { get; set; }

  }
}