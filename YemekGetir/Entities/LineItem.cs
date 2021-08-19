using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace YemekGetir.Entities
{
  public class LineItem
  {
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; }
    public int ProductId { get; set; }
    public Product Product { get; set; }
    public string Name { get; set; }
    public int Quantity { get; set; }
    public int Price { get; set; }
    public int TotalPrice { get { return Quantity * Price; } }
  }
}