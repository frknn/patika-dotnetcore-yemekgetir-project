using System.ComponentModel.DataAnnotations.Schema;

namespace MovieStore.Entities
{
  public class Product
  {
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; }
    public string Name { get; set; }
    public int Price { get; set; }
    public int RestaurantId { get; set; }
  }
}