using System.ComponentModel.DataAnnotations;
using SharedLibrary.Models.Enums;

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
}