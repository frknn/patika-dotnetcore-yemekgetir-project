using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace MovieStore.Entities
{
  public class Address
  {
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; }
    public string Country { get; set; }
    public string District { get; set; }
    public string City { get; set; }
    public string Line1 { get; set; }
    public string Line2 { get; set; }

  }
}