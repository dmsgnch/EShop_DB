using EShop_DB.Common.Constants;
using EShop_DB.Data;
using EShop_DB.Models;
using EShop_DB.Models.MainModels;
using EShop_DB.Common.Constants.Routes;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace EShop_DB.Controllers;

[ApiController, Route("order")]
public class OrderController : ControllerBase
{
    private readonly ApplicationDbContext _dbContext;
    private readonly string _entity = "Order";

    public OrderController(ApplicationDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    [HttpPost, Route(ApiRoutes.Universal.Create)]
    public IActionResult AddOrder([FromBody]Order order)
    {
        ValidateOrder(order);
        
        if (!order.OrderId.Equals(Guid.Empty))
        {
            if (_dbContext.Orders.Any(o => o.OrderId.Equals(order.OrderId)))
            {
                return BadRequest(ErrorMessages.Universal.AlreadyExistsId(_entity, order.OrderId));
            }
        }
        else
        {
            order.OrderId = Guid.NewGuid();
        }

        _dbContext.Orders.Add(order);
        _dbContext.SaveChanges();

        return Ok();
    }
    
    private void ValidateOrder(Order order)
    {
        //TODO: Add validation after order event logic implementation

        //return BadRequest(ErrorMessages.Product.AlreadyExistsNameSeller);
    }
    
    [HttpDelete, Route(ApiRoutes.Universal.Delete)]
    public IActionResult DeleteOrder([FromRoute]Guid id)
    {
        var result = _dbContext.Orders.FirstOrDefault(o => o.OrderId.Equals(id));
        
        if (result is null)
        {
            return BadRequest(ErrorMessages.Universal.NotFoundWithId(_entity, id));
        }

        _dbContext.Orders.Remove(result);
        _dbContext.SaveChanges();

        return Ok();
    }
    
    [HttpPut, Route(ApiRoutes.Universal.Update)]
    public IActionResult UpdateOrder([FromBody]Order order)
    {
        var result = _dbContext.Orders.FirstOrDefault(o => o.OrderId.Equals(order.OrderId));
        
        if (result is null)
        {
            return BadRequest(ErrorMessages.Universal.NotFoundWithId(_entity, order.OrderId));
        }
        
        result.OrderId = order.OrderId;
        result.AnonymousToken = order.AnonymousToken;
        result.UserId = order.UserId;
        result.User = order.User;
        result.OrderEvents = order.OrderEvents;
        result.OrderItems = order.OrderItems;
        result.DeliveryAddress = order.DeliveryAddress;

        _dbContext.Orders.Update(result);
        _dbContext.SaveChanges();

        return Ok();
    }
    
    [HttpGet, Route(ApiRoutes.Universal.GetById)]
    public IActionResult GetOrderById([FromRoute]Guid id)
    {
        var result = _dbContext.Orders.FirstOrDefault(o => o.OrderId.Equals(id));
        
        if (result is null)
        {
            return BadRequest(ErrorMessages.Universal.NotFoundWithId(_entity, id));
        }

        return Ok(result);
    }
    
    [HttpGet, Route(ApiRoutes.Universal.GetAll)]
    public IActionResult GetAllOrders()
    {
        List<Order> result = _dbContext.Orders.ToList();

        return Ok(result);
    }
}