using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using EShop_DB.Models.Enums;
using EShop_DB.Models.SecondaryModels;

namespace EShop_DB.Models.MainModels;

public class Order
{
    [Display(Name = "Order id")] public Guid OrderId { get; set; } = Guid.NewGuid();

    [Display(Name = "Stage")]
    public OrderProcessingStage OrderProcessingStage
    {
        get
        {
            return OrderEvents[0]?.Stage ?? throw new Exception("");
        }
    } 

    public Guid? AnonymousToken { get; set; }

    public decimal SummaryPrice
    {
        get
        {
            if (OrderItems is null) throw new ArgumentNullException(nameof(OrderItems));

            return OrderItems.Sum(ci => ci.SummaryItemPrice);
        }
    }

    #region Constructors

    public Order()
    {
    }

    public Order(User? user = null, Guid? anonymousToken = null)
    {
        if (user is not null && anonymousToken is not null ||
            user is null && anonymousToken is null)
        {
            throw new Exception("Order must have only user or anonymous token!");
        }

        if (user is not null)
        {
            if (!user.Role!.RoleTag.Equals(RoleTag.Customer))
            {
                throw new Exception("Only the customer can place an order!");
            }
            
            User = user;
            UserId = user.UserId;
        }
        else
        {
            AnonymousToken = anonymousToken;
        }
        
        OrderEvents.Add(new OrderEvent(this));
    }

    #endregion

    #region Relationships

    //User
    public Guid? UserId { get; set; }
    [ForeignKey(nameof(UserId))] public User? User { get; set; }
    
    //OrderEvent
    public List<OrderEvent> OrderEvents { get; set; } = new();

    //CartItem
    public List<OrderItem>? OrderItems { get; set; } = new();

    //Recipient
    public Recipient? Recipient { get; set; } = null;

    //DeliveryAddress
    public DeliveryAddress? DeliveryAddress { get; set; } = null;

    #endregion
}