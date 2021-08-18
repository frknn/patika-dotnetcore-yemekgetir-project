using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace YemekGetir.Entities
{
  public class Order
  {
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; }
    public int UserId { get; set; }
    public User User { get; set; }
    public int RestaurantId { get; set; }
    public Restaurant Restaurant { get; set; }
    public int ShippingAddressId { get; set; }
    public Address ShippingAddress { get; set; }
    public List<LineItem> LineItems { get; set; }
    public int TotalPrice { get; set; }
    public int StatusId { get; set; } = 1;
  }
}