using EShop_DB.Common.Constants;
using EShop_DB.Data;
using EShop_DB.Models.MainModels;
using EShop_DB.Common.Constants.Routes;
using Microsoft.AspNetCore.Mvc;

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

    [HttpPost, Route(ApiRoutes.Universal.Create)]
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
    
    [HttpDelete, Route(ApiRoutes.Universal.Delete)]
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
    
    [HttpPut, Route(ApiRoutes.Universal.Update)]
    public IActionResult UpdateUser([FromBody]User user)
    {
        var result = _dbContext.Users.FirstOrDefault(u => u.UserId.Equals(user.UserId));
        
        if (result is null)
        {
            return BadRequest(ErrorMessages.Universal.NotFoundWithId(_entity, user.UserId));
        }
        
        result.UserId = user.UserId;
        result.Name = user.Name;
        result.Email = user.Email;
        result.Password = user.Password;

        _dbContext.Users.Update(result);
        _dbContext.SaveChanges();

        return Ok();
    }
    
    [HttpGet, Route(ApiRoutes.Universal.GetById)]
    public IActionResult GetUserById([FromRoute]Guid id)
    {
        var result = _dbContext.Users.FirstOrDefault(u => u.UserId.Equals(id));
        
        if (result is null)
        {
            return BadRequest(ErrorMessages.Universal.NotFoundWithId(_entity, id));
        }

        return Ok(result);
    }
    
    [HttpGet, Route(ApiRoutes.Universal.GetAll)]
    public IActionResult GetAllUsers()
    {
        List<User> result = _dbContext.Users.ToList();

        return Ok(result);
    }
}