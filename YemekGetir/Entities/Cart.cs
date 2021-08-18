using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;

namespace YemekGetir.Entities
{
  public class Cart
  {
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; }

    // [ForeignKey("User")]
    public Guid UserId { get; set; }
    public User User { get; set; }
    public List<LineItem> LineItems { get; set; } = new List<LineItem> { new LineItem { Price = 5, Quantity = 3 } };
    public int TotalPrice
    {
      get
      {
        return LineItems.Sum(item => item.TotalPrice);
      }
    }
  }
}