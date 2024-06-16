using EShop_DB.Common.Constants;
using EShop_DB.Data;
using Microsoft.AspNetCore.Mvc;
using SharedLibrary.Models.MainModels;
using SharedLibrary.Responses;
using SharedLibrary.Routes;

namespace EShop_DB.Controllers;

[ApiController, Route(ApiRoutesDb.Controllers.OrderItem)]
public class OrderItemController : ControllerBase
{
    private readonly ApplicationDbContext _dbContext;
    private readonly string _entity = "Order item";

    public OrderItemController(ApplicationDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    [HttpPost, Route(ApiRoutesDb.Universal.Create)]
    public IActionResult AddOrderItem([FromBody] OrderItem orderItem)
    {
        //No restrictions on entities

        if (!orderItem.OrderItemId.Equals(Guid.Empty))
        {
            if (_dbContext.OrderItems.Any(oi => oi.OrderItemId.Equals(orderItem.OrderItemId)))
            {
                return BadRequest(new LambdaResponse(ErrorMessages.Universal.AlreadyExistsId(_entity, orderItem.OrderItemId)));
            }
        }
        else
        {
            orderItem.OrderItemId = Guid.NewGuid();
        }

        _dbContext.OrderItems.Add(orderItem);
        _dbContext.SaveChanges();

        return Ok();
    }

    [HttpDelete, Route(ApiRoutesDb.Universal.DeleteController)]
    public IActionResult DeleteOrderItem([FromRoute] Guid id)
    {
        var result = _dbContext.OrderItems.FirstOrDefault(oi => oi.OrderItemId.Equals(id));

        if (result is null)
        {
            return BadRequest(new LambdaResponse(ErrorMessages.Universal.NotFoundWithId(_entity, id)));
        }

        _dbContext.OrderItems.Remove(result);
        _dbContext.SaveChanges();

        return Ok();
    }

    [HttpPut, Route(ApiRoutesDb.Universal.Update)]
    public IActionResult UpdateOrderItem([FromBody] OrderItem orderItem)
    {
        var result = _dbContext.OrderItems.FirstOrDefault(oi => oi.OrderItemId.Equals(orderItem.OrderItemId));

        if (result is null)
        {
            return BadRequest(new LambdaResponse(ErrorMessages.Universal.NotFoundWithId(_entity, orderItem.OrderItemId)));
        }

        result.OrderItemId = orderItem.OrderItemId;
        result.Quantity = orderItem.Quantity;

        result.OrderId = orderItem.OrderId;
        result.Order = orderItem.Order;

        result.ProductId = orderItem.ProductId;
        result.Product = orderItem.Product;


        _dbContext.OrderItems.Update(result);
        _dbContext.SaveChanges();

        return Ok();
    }

    [HttpGet, Route(ApiRoutesDb.Universal.GetByIdController)]
    public IActionResult GetOrderItemById([FromRoute] Guid id)
    {
        var result = _dbContext.OrderItems.FirstOrDefault(oi => oi.OrderItemId.Equals(id));

        if (result is null)
        {
            return BadRequest(new LambdaResponse(ErrorMessages.Universal.NotFoundWithId(_entity, id)));
        }

        return Ok(result);
    }

    [HttpGet, Route(ApiRoutesDb.Universal.GetAll)]
    public IActionResult GetAllOrderItems()
    {
        List<OrderItem> result = _dbContext.OrderItems.ToList();

        return Ok(result);
    }
}