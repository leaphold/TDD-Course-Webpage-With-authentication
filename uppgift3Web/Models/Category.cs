using System.Collections.Generic;
namespace uppgift3Web.Models
{
  public class Category
  {
    public int Id { get; set; }
    public string Name { get; set; }

    //the list below defines the relationship between the two tables
    public List<Product>? Products { get; set; }
  }
}
