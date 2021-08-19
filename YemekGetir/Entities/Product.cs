using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace YemekGetir.Entities
{
  public class Product
  {
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; }
    public string Name { get; set; }
    public int Price { get; set; }
    public Guid RestaurantId { get; set; }
    public Restaurant Restaurant { get; set; }
  }
}