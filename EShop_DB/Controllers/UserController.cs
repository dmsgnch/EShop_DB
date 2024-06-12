using EShop_DB.Common.Constants;
using EShop_DB.Data;
using Microsoft.AspNetCore.Mvc;
using SharedLibrary.Models.MainModels;
using SharedLibrary.Routes;

namespace EShop_DB.Controllers;

[ApiController, Route("user")]
public class UserController : ControllerBase
{
    private readonly ApplicationDbContext _dbContext;
    private readonly string _entity = "User";

    public UserController(ApplicationDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    [HttpPost, Route(ApiRoutesDb.Universal.Create)]
    public IActionResult AddUser([FromBody]User user)
    {
        if (_dbContext.Users.Any(u => u.Email.Equals(user.Email)))
        {
            return BadRequest(ErrorMessages.Universal.AlreadyExistsEmail(_entity));
        }
        
        if (!user.UserId.Equals(Guid.Empty))
        {
            if (_dbContext.Users.Any(u => u.UserId.Equals(user.UserId)))
            {
                return BadRequest(ErrorMessages.Universal.AlreadyExistsId(_entity, user.UserId));
            }
        }
        else
        {
            user.UserId = Guid.NewGuid();
        }

        _dbContext.Users.Add(user);
        _dbContext.SaveChanges();

        return Ok();
    }
    
    [HttpDelete, Route(ApiRoutesDb.Universal.Delete)]
    public IActionResult DeleteUser([FromRoute]Guid id)
    {
        var result = _dbContext.Users.FirstOrDefault(u => u.UserId.Equals(id));
        
        if (result is null)
        {
            return BadRequest(ErrorMessages.Universal.NotFoundWithId(_entity, id));
        }

        _dbContext.Users.Remove(result);
        _dbContext.SaveChanges();

        return Ok();
    }
    
    [HttpPut, Route(ApiRoutesDb.Universal.Update)]
    public IActionResult UpdateUser([FromBody]User user)
    {
        var result = _dbContext.Users.FirstOrDefault(u => u.UserId.Equals(user.UserId));
        
        if (result is null)
        {
            return BadRequest(ErrorMessages.Universal.NotFoundWithId(_entity, user.UserId));
        }
        
        result.UserId = user.UserId;
        
        result.Name = user.Name;
        result.LastName = user.LastName;
        result.Patronymic = user.Patronymic;
        
        result.Email = user.Email;
        result.PhoneNumber = user.PhoneNumber;
        
        result.PasswordHash = user.PasswordHash;
        result.Salt = user.Salt;

        result.RoleId = user.RoleId;
        result.Role = user.Role;
        result.Orders = user.Orders;
        result.Recipient = user.Recipient;
        result.DeliveryAddress = user.DeliveryAddress;

        _dbContext.Users.Update(result);
        _dbContext.SaveChanges();

        return Ok();
    }
    
    [HttpGet, Route(ApiRoutesDb.Universal.GetById)]
    public IActionResult GetUserById([FromRoute]Guid id)
    {
        var result = _dbContext.Users.FirstOrDefault(u => u.UserId.Equals(id));
        
        if (result is null)
        {
            return BadRequest(ErrorMessages.Universal.NotFoundWithId(_entity, id));
        }

        return Ok(result);
    }
    
    [HttpGet, Route(ApiRoutesDb.Universal.GetAll)]
    public IActionResult GetAllUsers()
    {
        List<User> result = _dbContext.Users.ToList();

        return Ok(result);
    }
}