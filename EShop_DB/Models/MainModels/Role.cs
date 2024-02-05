using System.ComponentModel.DataAnnotations;
using EShop_DB.Models.Enums;
using Microsoft.EntityFrameworkCore;

namespace EShop_DB.Models.MainModels;

public class Role
{
    [Key] public int RoleId { get; set; }
    
    public RoleTag RoleTag { get; set; }
    
    #region Constructors
    
    public Role() 
    {}

    public Role(RoleTag roleTag)
    {
        RoleId = (int)roleTag;
        
        RoleTag = roleTag;
    }
    
    #endregion
    
    #region Relationships
    
    //User
    public ICollection<User> Users { get; set; } = new List<User>();
    
    #endregion

    // /// <summary>
    // /// AdvManager can grant advanced rights to another manager
    // /// </summary>
    // /// <exception cref="InvalidOperationException"></exception>
    // public void GrantAdvancedRights()
    // {
    //     if (RoleTag == RoleTag.Customer)
    //     {
    //         throw new InvalidOperationException(
    //             "Customer can`t get the extended rights of the manager, only the manager can do that!");
    //     }
    //     
    //     
    // }
}