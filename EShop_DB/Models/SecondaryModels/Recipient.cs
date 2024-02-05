using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using EShop_DB.Models.MainModels;

namespace EShop_DB.Models.SecondaryModels;

public class Recipient
{
    [Key] public Guid RecipientId { get; set; }

    public string Name { get; set; } = "";
    public string LastName { get; set; } = "";
    public string? Patronymic { get; set; } = null;

    public string PhoneNumber { get; set; } = "";

    #region Constructors

    public Recipient()
    { }

    public Recipient(
        string name, 
        string lastName, 
        string phoneNumber, 
        string? patronymic = null,
        User? user = null,
        Order? order = null)
    {
        Name = name;
        LastName = lastName;
        Patronymic = patronymic;
        
        PhoneNumber = phoneNumber;
        
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