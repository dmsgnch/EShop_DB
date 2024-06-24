using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EShop_DB.Models.MainModels;

public class OrderItem
{
    [Key]
    public Guid OrderItemId { get; set; } = Guid.NewGuid();

    public uint Quantity { get; set; }
    
    #region Constructors

    public OrderItem()
    { }

    public OrderItem(Guid orderId, Guid productId, uint quantity = 1)
    {
        OrderId = orderId;
        ProductId = productId;

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