namespace EShop_DB.Models.Enums;

public enum OrderProcessingStage
{
    Cart = 0,   
    OrderConfirmation,
    Delivery,               
    PickupReady,           
    OrderCompleted,
}