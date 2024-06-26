using EShop_DB.Common.Constants;
using EShop_DB.Common.Extensions;
using EShop_DB.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using EShop_DB.Models.MainModels;
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

    [HttpPost, Route(ApiRoutesDb.UniversalActions.CreateAction)]
    public IActionResult AddUser([FromBody] UserDTO userDto)
    {
        var user = userDto.ToUser();
        
        if (_dbContext.Users.Any(u => u.Email.Equals(user.Email)))
        {
            var lambda = new UniversalResponse(errorInfo: ErrorMessages.UniversalMessages.AlreadyExistsEmail(_entity));
            return BadRequest(lambda);
        }

        if (!user.UserId.Equals(Guid.Empty))
        {
            if (_dbContext.Users.Any(u => u.UserId.Equals(user.UserId)))
            {
                return BadRequest(
                    new UniversalResponse<UserDTO>(errorInfo: ErrorMessages.UniversalMessages.AlreadyExistsId(_entity, user.UserId)));
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

        return Ok(new UniversalResponse<UserDTO>(responseObject: user.ToUserDto(), info: SuccessMessages.UniversalResponse.Created(_entity)));
    }

    [HttpDelete, Route(ApiRoutesDb.UniversalActions.DeleteAction)]
    public IActionResult DeleteUser([FromBody] Guid id)
    {
        var result = _dbContext.Users.AsNoTracking().FirstOrDefault(u => u.UserId.Equals(id));

        if (result is null)
        {
            return BadRequest(new UniversalResponse(errorInfo: ErrorMessages.UniversalMessages.NotFoundWithId(_entity, id)));
        }

        _dbContext.Users.Remove(result);
        _dbContext.SaveChanges();

        return Ok(new UniversalResponse(info: SuccessMessages.UniversalResponse.Deleted(_entity)));
    }

    [HttpPut, Route(ApiRoutesDb.UniversalActions.UpdateAction)]
    public IActionResult UpdateUser([FromBody] UserDTO userDto)
    {
        var user = userDto.ToUser();
        
        var result = _dbContext.Users.AsNoTracking().FirstOrDefault(u => u.UserId.Equals(user.UserId));

        if (result is null)
        {
            return BadRequest(
                new UniversalResponse<UserDTO>(errorInfo: ErrorMessages.UniversalMessages.NotFoundWithId(_entity, user.UserId)));
        }

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

        return Ok(new UniversalResponse<UserDTO>(responseObject: user.ToUserDto(), info: SuccessMessages.UniversalResponse.Updated(_entity)));
    }

    [HttpGet, Route(ApiRoutesDb.UniversalActions.GetByIdAction)]
    public IActionResult GetUserById([FromBody] Guid id)
    {
        User? user = _dbContext.Users.AsNoTracking().FirstOrDefault(u => u.UserId.Equals(id));

        if (user is null)
        {
            return BadRequest(new UniversalResponse(ErrorMessages.UniversalMessages.NotFoundWithId(_entity, id)));
        }

        AddAllUserIncludeModels(user);

        var userDto = user.ToUserDto();

        return Ok(new UniversalResponse<UserDTO>(responseObject: userDto));
    }

    [HttpGet, Route(ApiRoutesDb.UniversalActions.GetAllAction)]
    public IActionResult GetAllUsers()
    {
        List<User> result = _dbContext.Users.ToList();

        foreach (var user in result)
        {
            AddAllUserIncludeModels(user);
        }

        return Ok(new UniversalResponse<List<UserDTO>>(responseObject: result.Select(u => u.ToUserDto()).ToList()));
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
            var role = _dbContext.Roles.AsNoTracking().FirstOrDefault(r => r.RoleId.Equals(user.RoleId))
                       ?? throw new Exception($"Role with id: {user.RoleId} is not exist!");

            role.Users = null;
            user.Role = role;
        }
    }

    private void AddSellerIfNull(User user)
    {
        if (user.SellerId is not null && user.Seller is null)
        {
            var seller = _dbContext.Sellers.AsNoTracking().FirstOrDefault(r => r.SellerId.Equals(user.SellerId))
                         ?? throw new Exception($"Seller with id: {user.SellerId} is not exist!");

            seller.Users = null;
            user.Seller = seller;
        }
    }
    
    private void AddOrdersIfNull(User user)
    {
        if (user.Orders is null || user.Orders.Count.Equals(0))
        {
            var orders = _dbContext.Orders.AsNoTracking().Include(o => o.OrderEvents).Where(r => r.UserId.Equals(user.UserId)).ToList();

            if (orders is not null && orders.Count > 0)
            {
                foreach (var order in orders)
                {
                    order.User = null;
                }
                
                foreach (var order in orders)
                {
                    if (order.OrderEvents is not null && order.OrderEvents.Count > 0)
                    {
                        foreach (var orderEvent in order.OrderEvents)
                        {
                            orderEvent.Order = null;
                        }
                    }
                }
            }

            user.Orders = orders;
        }
    }
}