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
        User? user = null,
        Order? order = null
    )
    {
        City = city;
        Street = street;
        House = house;
        Apartment = apartment;
        Floor = floor;

        if (order is null && user is null)
        {
            throw new ArgumentException("You must pass the Order or User");
        }

        if (order is not null && user is not null)
        {
            throw new ArgumentException("You cant pass Order and User at the same time");
        }

        UserId = user?.UserId;
        User = user;

        OrderId = order?.OrderId;
        Order = order;
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