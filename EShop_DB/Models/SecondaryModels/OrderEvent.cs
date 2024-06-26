using System.ComponentModel.DataAnnotations;
using EShop_DB.Models.MainModels;
using SharedLibrary.Models.Enums;
using SharedLibrary.Models.DtoModels.MainModels;

namespace EShop_DB.Models.SecondaryModels;

public class OrderEvent
{
    [Key]
    public Guid OrderEventId { get; set; } = Guid.NewGuid();
    
    public DateTime EventTime { get; set; }
    
    public OrderProcessingStage Stage { get; set; }
    
    #region Constructors
    
    public OrderEvent() 
    {}

    public OrderEvent(Guid orderId, OrderProcessingStage newStage = OrderProcessingStage.Cart)
    {
        OrderId = orderId;

        EventTime = DateTime.Now;
        Stage = newStage;
    }
    
    #endregion

    #region Relationships

    public Guid OrderId { get; set; }
    public Order? Order { get; set; }

    #endregion
    
}