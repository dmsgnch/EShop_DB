using EShop_DB.Common.Constants;
using EShop_DB.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using SharedLibrary.Models.MainModels;
using SharedLibrary.Responses;
using SharedLibrary.Routes;

namespace EShop_DB.Controllers;

[ApiController, Route(ApiRoutesDb.Controllers.User)]
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
            return BadRequest(new LambdaResponse(ErrorMessages.Universal.AlreadyExistsEmail(_entity)));
        }
        
        if (!user.UserId.Equals(Guid.Empty))
        {
            if (_dbContext.Users.Any(u => u.UserId.Equals(user.UserId)))
            {
                return BadRequest(new LambdaResponse(ErrorMessages.Universal.AlreadyExistsId(_entity, user.UserId)));
            }
        }
        else
        {
            user.UserId = Guid.NewGuid();
        }

        if (!(user.RoleId > 0))
        {
            user.RoleId = 1;
        }

        _dbContext.Users.Add(user);
        _dbContext.SaveChanges();

        return Ok();
    }
    
    [HttpDelete, Route(ApiRoutesDb.Universal.DeleteController)]
    public IActionResult DeleteUser([FromRoute]Guid id)
    {
        var result = _dbContext.Users.FirstOrDefault(u => u.UserId.Equals(id));
        
        if (result is null)
        {
            return BadRequest(new LambdaResponse(ErrorMessages.Universal.NotFoundWithId(_entity, id)));
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
            return BadRequest(new LambdaResponse(ErrorMessages.Universal.NotFoundWithId(_entity, user.UserId)));
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
    
    [HttpGet, Route(ApiRoutesDb.Universal.GetByIdController)]
    public IActionResult GetUserById([FromRoute]Guid id)
    {
        try
        {
            User? result = _dbContext.Users.Include(u => u.Role).FirstOrDefault(u => u.UserId.Equals(id));

            if (result is null)
            {
                return BadRequest(new LambdaResponse(ErrorMessages.Universal.NotFoundWithId(_entity, id)));
            }
            
            result.Role.Users = null;

            var lambda = new LambdaResponse<User>(responseObject: result);

            var jsonResult = JsonConvert.SerializeObject(lambda);
            
            return Ok(jsonResult);
        }
        catch (Exception ex)
        {
            // Логируем ошибку, чтобы можно было отследить детали
            Console.WriteLine($"An error occurred while getting user by ID: {id} with error: {ex.Message}");

            // Возвращаем общее сообщение об ошибке
            return StatusCode(500, new LambdaResponse("An unexpected error occurred. Please try again later."));
        }
    }
    
    [HttpGet, Route(ApiRoutesDb.Universal.GetAll)]
    public IActionResult GetAllUsers()
    {
        List<User> result = _dbContext.Users.Include(u => u.Role).ToList();

        foreach (var user in result)
        {
            user.Role.Users = null;
        }
        
        return Ok(JsonConvert.SerializeObject(result));
    }
}