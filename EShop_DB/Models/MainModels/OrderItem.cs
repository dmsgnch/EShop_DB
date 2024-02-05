using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EShop_DB.Models.MainModels;

public class OrderItem
{
    [Key]
    public Guid OrderItemId { get; set; }

    public uint Quantity { get; set; }

    public decimal SummaryItemPrice
    {
        get
        {
            decimal pricePerUnit = (decimal)(Product?.PricePerUnit ??
                                              throw new InvalidOperationException());

            return pricePerUnit * Quantity;
        }
    }
    
    #region Constructors

    public OrderItem()
    { }

    public OrderItem(Order order, Product product, uint quantity = 1)
    {
        OrderId = order.OrderId;
        Order = order;
        
        OrderItemId = Guid.NewGuid();

        ProductId = product.ProductId;
        Product = product;

        Quantity = quantity;
    }
    
    #endregion

    #region Relationships

    //Order
    public Guid OrderId { get; set; }
    [ForeignKey(nameof(OrderId))]
    public Order? Order { get; set; }

    //Product
    public Guid ProductId { get; set; }
    [ForeignKey(nameof(ProductId))]
    public Product? Product { get; set; }
    
    #endregion
}