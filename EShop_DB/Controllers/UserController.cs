using EShop_DB.Common.Constants;
using EShop_DB.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using SharedLibrary.Models.DbModels.MainModels;
using SharedLibrary.Models.DtoModels.MainModels;
using SharedLibrary.Responses;
using SharedLibrary.Routes;

namespace EShop_DB.Controllers;

[ApiController, Route(ApiRoutesDb.Controllers.UserContr)]
public class UserController : ControllerBase
{
    private readonly ApplicationDbContext _dbContext;
    private readonly string _entity = "User";

    public UserController(ApplicationDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    [HttpPost, Route(ApiRoutesDb.UniversalActions.CreatePath)]
    public IActionResult AddUser([FromBody] User user)
    {
        if (_dbContext.Users.Any(u => u.Email.Equals(user.Email)))
        {
            var lambda = new LambdaResponse(ErrorMessages.UniversalMessages.AlreadyExistsEmail(_entity));
            return BadRequest(lambda);
        }

        if (!user.UserId.Equals(Guid.Empty))
        {
            if (_dbContext.Users.Any(u => u.UserId.Equals(user.UserId)))
            {
                return BadRequest(
                    new LambdaResponse<User>(errorInfo: ErrorMessages.UniversalMessages.AlreadyExistsId(_entity, user.UserId)));
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
        
        AddAllUserIncludeModels(user);

        return Ok(new LambdaResponse<User>(responseObject: user, info: SuccessMessages.UserMessages.Created));
    }

    [HttpDelete, Route(ApiRoutesDb.UniversalActions.DeleteControllerPath)]
    public IActionResult DeleteUser([FromRoute] Guid id)
    {
        var result = _dbContext.Users.FirstOrDefault(u => u.UserId.Equals(id));

        if (result is null)
        {
            return BadRequest(new LambdaResponse(errorInfo: ErrorMessages.UniversalMessages.NotFoundWithId(_entity, id)));
        }

        _dbContext.Users.Remove(result);
        _dbContext.SaveChanges();

        return Ok(new LambdaResponse(info: SuccessMessages.UserMessages.Deleted));
    }

    [HttpPut, Route(ApiRoutesDb.UniversalActions.UpdatePath)]
    public IActionResult UpdateUser([FromBody] User user)
    {
        var result = _dbContext.Users.FirstOrDefault(u => u.UserId.Equals(user.UserId));

        if (result is null)
        {
            return BadRequest(
                new LambdaResponse<User>(errorInfo: ErrorMessages.UniversalMessages.NotFoundWithId(_entity, user.UserId)));
        }

        result.UserId = user.UserId;

        result.Name = user.Name;
        result.LastName = user.LastName;
        result.Patronymic = user.Patronymic;

        result.Email = user.Email;
        result.PhoneNumber = user.PhoneNumber;

        if (!String.IsNullOrEmpty(user.PasswordHash))
        {
            result.PasswordHash = user.PasswordHash;
            result.Salt = user.Salt;
        }

        result.RoleId = user.RoleId;

        if (user.SellerId is not null)
        {
            result.SellerId = user.SellerId;
        }

        _dbContext.Users.Update(result);
        _dbContext.SaveChanges();

        AddAllUserIncludeModels(user);

        return Ok(new LambdaResponse<User>(responseObject: user, info: SuccessMessages.UserMessages.Updated));
    }

    [HttpGet, Route(ApiRoutesDb.UniversalActions.GetByIdControllerPath)]
    public IActionResult GetUserById([FromRoute] Guid id)
    {
        User? user = _dbContext.Users.FirstOrDefault(u => u.UserId.Equals(id));

        if (user is null)
        {
            return BadRequest(new LambdaResponse(ErrorMessages.UniversalMessages.NotFoundWithId(_entity, id)));
        }

        AddAllUserIncludeModels(user);

        return Ok(new LambdaResponse<User>(responseObject: user));
    }

    [HttpGet, Route(ApiRoutesDb.UniversalActions.GetAllPath)]
    public IActionResult GetAllUsers()
    {
        List<User> result = _dbContext.Users.ToList();

        foreach (var user in result)
        {
            AddAllUserIncludeModels(user);
        }

        return Ok(new LambdaResponse<List<User>>(responseObject: result));
    }

    private void AddAllUserIncludeModels(User user)
    {
        AddRoleIfNull(user);
        AddSellerIfNull(user);
        AddOrdersIfNull(user);
    }

    private void AddRoleIfNull(User user)
    {
        if (user.Role is null)
        {
            var role = _dbContext.Roles.FirstOrDefault(r => r.RoleId.Equals(user.RoleId))
                       ?? throw new Exception($"Role with id: {user.RoleId} is not exist!");

            role.Users = null;
            user.Role = role;
        }
    }

    private void AddSellerIfNull(User user)
    {
        if (user.SellerId is not null && user.Seller is null)
        {
            var seller = _dbContext.Sellers.FirstOrDefault(r => r.SellerId.Equals(user.SellerId))
                         ?? throw new Exception($"Seller with id: {user.SellerId} is not exist!");

            seller.Users = null;
            user.Seller = seller;
        }
    }
    
    private void AddOrdersIfNull(User user)
    {
        if (user.Orders is null || user.Orders.Count.Equals(0))
        {
            var orders = _dbContext.Orders.Where(r => r.UserId.Equals(user.UserId)).ToList();

            if (orders is not null && orders.Count > 0)
            {
                foreach (var order in orders)
                {
                    order.User = null;
                }
            }

            user.Orders = orders;
        }
    }
}