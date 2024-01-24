using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;

namespace EShop_DB.Models;

public class Product
{
    [Key]
    public Guid ProductId { get; set; }

    public string ImageURL { get; set; }

    public string Name { get; set; }
    public string Description { get; set; }
    public double PricePerUnit { get; set; }
    public double WeightInGrams { get; set; }
    public double WholesalePricePerUnit { get; set; }
    public int InStock { get; set; }
    public double WholesaleQuantity { get; set; }
}