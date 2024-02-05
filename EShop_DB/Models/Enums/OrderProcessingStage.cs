namespace EShop_DB.Models.Enums;

public enum OrderProcessingStage
{
    Cart,   
    OrderConfirmation,
    Delivery,               
    PickupReady,           
    OrderCompleted,
}