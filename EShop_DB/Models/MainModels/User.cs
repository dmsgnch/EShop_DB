using System.ComponentModel.DataAnnotations;
using EShop_DB.Models.SecondaryModels;

namespace EShop_DB.Models.MainModels;

public class User
{
    [Key]
    public Guid UserId { get; set; } = Guid.NewGuid();

    public string Name { get; set; }
    public string LastName { get; set; }
    public string? Patronymic { get; set; }
    
    public string PasswordHash { get; set; }
    public string Salt { get; set; }
    
    public string Email { get; set; }
    public string PhoneNumber { get; set; }

    #region Constructors
    
    public User() 
    {}

    public User(
        string name, 
        string lastName, 
        string passwordHash,
        string salt,
        string phoneNumber, 
        string email,
        string? patronymic = null,
        int roleId = 1)
    {
       
        Name = name;
        LastName = lastName;
        Patronymic = patronymic;

        PasswordHash = passwordHash;
        Salt = salt;
        
        Email = email;
        PhoneNumber = phoneNumber;

        RoleId = roleId;
    }
    
    #endregion

    #region Relationships

    //Role
    public int RoleId { get; set; } = 1;
    public Role? Role { get; set; }
    
    //Seller
    public Guid? SellerId { get; set; }
    public Seller? Seller { get; set; }
    
    //Order
    public List<Order>? Orders { get; set; }

    //Recipient
    public Recipient? Recipient { get; set; }

    //DeliveryAddress
    public DeliveryAddress? DeliveryAddress { get; set; }

    #endregion
}