using EShop_DB.Data;
using EShop_DB.Models;
using Microsoft.AspNetCore.Mvc;

namespace EShop_DB.Controllers;

[Route("user")]
[ApiController]
public class UserController : Controller
{
    private readonly ApplicationContext _context;

    public UserController(ApplicationContext context)
    {
        _context = context;
    }

    [Route("add")]
    [HttpPost]
    public IActionResult AddUser([FromBody]User user)
    {
        if (_context.Users.Any(u => u.Email.Equals(user.Email)))
        {
            return BadRequest("User with the same email is already exist");
        }
        
        //Check, that guid is exist
        if (user.UserId.Equals(Guid.Empty)) user.UserId = Guid.NewGuid();

        _context.Users.Add(user);
        _context.SaveChanges();

        return Ok();
    }
    
    [Route("delete")]
    [HttpDelete]
    public IActionResult DeleteUser([FromRoute]Guid id)
    {
        var result = _context.Users.FirstOrDefault(u => u.UserId.Equals(id));
        
        if (result is null)
        {
            return BadRequest("User was not found");
        }

        _context.Users.Remove(result);
        _context.SaveChanges();

        return Ok();
    }
    
    [Route("update")]
    [HttpPut]
    public IActionResult UpdateUser([FromBody]User user)
    {
        var result = _context.Users.FirstOrDefault(u => u.UserId.Equals(user.UserId));
        
        if (result is null)
        {
            return BadRequest("User was not found");
        }
        
        result.UserId = user.UserId;
        result.Name = user.Name;
        result.Email = user.Email;
        result.Password = user.Password;
        result.AccountType = user.AccountType;

        _context.Users.Update(result);
        _context.SaveChanges();

        return Ok();
    }
    
    [Route("getById")]
    [HttpGet]
    public IActionResult GetUserById([FromRoute]Guid id)
    {
        var result = _context.Users.FirstOrDefault(u => u.UserId.Equals(id));
        
        if (result is null)
        {
            return BadRequest("User was not found");
        }

        return Ok(result);
    }
    
    [Route("getAll")]
    [HttpGet]
    public IActionResult GetAllUsers()
    {
        List<User> result = _context.Users.ToList();

        return Ok(result);
    }
}