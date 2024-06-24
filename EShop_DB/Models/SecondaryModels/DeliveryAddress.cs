using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using EShop_DB.Models.MainModels;

namespace EShop_DB.Models.SecondaryModels;

public class DeliveryAddress
{
    [Key]
    public Guid DeliveryAddressId { get; set; }

    public string City { get; set; } = "";
    public string Street { get; set; } = "";
    public string House { get; set; } = "";
    public string? Apartment { get; set; } = null;
    public string? Floor { get; set; } = null;

    #region Constructors
        
    public DeliveryAddress()
    { }

    public DeliveryAddress(
        string city, 
        string street, 
        string house, 
        string? apartment = null, 
        string? floor = null,
        Guid? userId = null,
        Guid? orderId = null
    )
    {
        City = city;
        Street = street;
        House = house;
        Apartment = apartment;
        Floor = floor;

        if (userId is null && orderId is null)
        {
            throw new ArgumentException("You must pass the Order or User");
        }

        if (userId is not null && orderId is not null)
        {
            throw new ArgumentException("You cant pass Order and User at the same time");
        }

        UserId = userId;
        OrderId = orderId;
    }
        
    #endregion
        
    #region Relationships

    //User
    public Guid? UserId { get; set; }
    [ForeignKey(nameof(UserId))] 
    public User? User { get; set; }

    //Order
    public Guid? OrderId { get; set; }
    [ForeignKey(nameof(OrderId))] 
    public Order? Order { get; set; }
    
    #endregion
}