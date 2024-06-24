using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using EShop_DB.Models.SecondaryModels;
using SharedLibrary.Models.Enums;

namespace EShop_DB.Models.MainModels;

public class Order
{
    [Display(Name = "Order id")] 
    public Guid OrderId { get; set; } = Guid.NewGuid();

    [Display(Name = "Stage")] 
    public OrderProcessingStage ProcessingStage { get; set; } 
    public Guid? AnonymousToken { get; set; }

    #region Constructors

    public Order()
    { }

    public Order(Guid? userId = null, Guid? anonymousToken = null)
    {
        if (userId is not null && anonymousToken is not null ||
            userId is null && anonymousToken is null)
        {
            throw new Exception("Order must have only user or anonymous token!");
        }

        if (userId is not null)
        {
            UserId = userId;
        }
        else
        {
            AnonymousToken = anonymousToken;
        }
        
        //OrderEvents.Add(new OrderEvent(OrderId));
    }

    #endregion

    #region Relationships

    //User
    public Guid? UserId { get; set; }
    [ForeignKey(nameof(UserId))] public User? User { get; set; }
    
    //OrderEvent
    public List<OrderEvent>? OrderEvents { get; set; }

    //CartItem
    public List<OrderItem>? OrderItems { get; set; }

    //Recipient
    public Recipient? Recipient { get; set; }

    //DeliveryAddress
    public DeliveryAddress? DeliveryAddress { get; set; }

    #endregion
}