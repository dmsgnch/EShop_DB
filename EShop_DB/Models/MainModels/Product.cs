using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Diagnostics.CodeAnalysis;

namespace EShop_DB.Models.MainModels;

public class Product
{
    [Key] public Guid ProductId { get; set; }

    [Display(Name = "Company picture")] public string? ImageUrl { get; set; }

    public string Name { get; set; }
    public string Description { get; set; }
    [Display(Name = "Price")] public decimal PricePerUnit { get; set; }
    [Display(Name = "Weight")] public double WeightInGrams { get; set; }
    [Display(Name = "In stock")] public int InStock { get; set; }
    
    #region Constructors

    public Product()
    { }

    public Product(
        string name, 
        string description, 
        decimal pricePerUnit, 
        int weightInGrams, 
        Seller seller,
        string? imageUrl = null)
    {
        Name = name;
        Description = description;
        PricePerUnit = pricePerUnit;
        WeightInGrams = weightInGrams;

        ImageUrl = imageUrl ?? "";
        
        SellerId = seller.SellerId;
        Seller = seller;
    }
    
    #endregion

    #region Relationships
    
    //Seller
    public Guid SellerId { get; set; }
    [ForeignKey(nameof(SellerId))]
    public Seller? Seller { get; set; }
    
    //CartItem
    public List<OrderItem>? OrderItems { get; set; } = new();

    #endregion
}