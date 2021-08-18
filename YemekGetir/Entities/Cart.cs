using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace MovieStore.Entities
{
  public class Cart
  {
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; }
    public string Name { get; set; }
    public int UserId { get; set; }
    public List<LineItem> LineItems { get; set; }
    public int TotalPrice { get; set; }
  }
}