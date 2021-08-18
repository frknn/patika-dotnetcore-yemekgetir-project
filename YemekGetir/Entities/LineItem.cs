using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace MovieStore.Entities
{
  public class LineItem
  {
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; }
    public int ProductId { get; set; }
    public int CartId { get; set; }
    public string Name { get; set; }
    public int Quantity { get; set; }
    public int IndividualPrice { get; set; }
    public int TotalPrice { get; set; }
  }
}