using EShop_DB.Common.Constants;
using EShop_DB.Common.Extensions;
using EShop_DB.Components;
using EShop_DB.Data;
using Microsoft.AspNetCore.Mvc;
using EShop_DB.Models.MainModels;
using SharedLibrary.Models.DtoModels.MainModels;
using SharedLibrary.Responses;
using SharedLibrary.Routes;

namespace EShop_DB.Controllers;

[ApiController, Route(ApiRoutesDb.Controllers.OrderItemContr)]
public class OrderItemController : ControllerBase
{
    private readonly ApplicationDbContext _dbContext;
    private readonly string _entity;

    public OrderItemController(ApplicationDbContext dbContext)
    {
        _dbContext = dbContext;
        
        _entity = this.GetFormatedControllerName();
    }

    [HttpPost, Route(ApiRoutesDb.UniversalActions.CreateAction)]
    public IActionResult AddOrderItem([FromBody] OrderItemDTO orderItemDto)
    {
        var orderItem = orderItemDto.ToOrderItem();

        if (!orderItem.OrderItemId.Equals(Guid.Empty))
        {
            if (_dbContext.OrderItems.Any(oi => oi.OrderItemId.Equals(orderItem.OrderItemId)))
            {
                return BadRequest(new UniversalResponse(ErrorMessages.UniversalMessages.AlreadyExistsId(_entity, orderItem.OrderItemId)));
            }
        }
        else
        {
            orderItem.OrderItemId = Guid.NewGuid();
        }

        var existOrderItem = _dbContext.OrderItems.FirstOrDefault(oi =>
            oi.OrderId.Equals(orderItem.OrderId) && oi.ProductId.Equals(orderItem.ProductId));

        if (existOrderItem is not null)
        {
            existOrderItem.Quantity++;
            _dbContext.SaveChanges();
        }
        else
        {
            _dbContext.OrderItems.Add(orderItem);
            _dbContext.SaveChanges();
        }
        
        return Ok(new UniversalResponse<OrderItemDTO>(responseObject: orderItem.ToOrderItemDto(), info: SuccessMessages.UniversalResponse.Created(_entity)));
    }

    [HttpDelete, Route(ApiRoutesDb.UniversalActions.DeleteAction)]
    public IActionResult DeleteOrderItem([FromBody] Guid id)
    {
        var result = _dbContext.OrderItems.FirstOrDefault(oi => oi.OrderItemId.Equals(id));

        if (result is null)
        {
            return BadRequest(new UniversalResponse(ErrorMessages.UniversalMessages.NotFoundWithId(_entity, id)));
        }

        _dbContext.OrderItems.Remove(result);
        _dbContext.SaveChanges();

        return Ok(new UniversalResponse(info: SuccessMessages.UniversalResponse.Deleted(_entity)));
    }

    [HttpPut, Route(ApiRoutesDb.UniversalActions.UpdateAction)]
    public IActionResult UpdateOrderItem([FromBody] OrderItemDTO orderItemDto)
    {
        var orderItem = orderItemDto.ToOrderItem();
        
        var result = _dbContext.OrderItems.FirstOrDefault(oi => oi.OrderItemId.Equals(orderItem.OrderItemId));

        if (result is null)
        {
            return BadRequest(new UniversalResponse(ErrorMessages.UniversalMessages.NotFoundWithId(_entity, orderItem.OrderItemId)));
        }

        result.OrderItemId = orderItem.OrderItemId;
        result.Quantity = orderItem.Quantity;

        result.OrderId = orderItem.OrderId;
        result.Order = orderItem.Order;

        result.ProductId = orderItem.ProductId;
        result.Product = orderItem.Product;


        _dbContext.OrderItems.Update(result);
        _dbContext.SaveChanges();

        return Ok(new UniversalResponse<OrderItemDTO>(responseObject: orderItem.ToOrderItemDto(), info: SuccessMessages.UniversalResponse.Updated(_entity)));
    }

    [HttpGet, Route(ApiRoutesDb.UniversalActions.GetByIdAction)]
    public IActionResult GetOrderItemById([FromBody] Guid id)
    {
        var result = _dbContext.OrderItems.FirstOrDefault(oi => oi.OrderItemId.Equals(id));

        if (result is null)
        {
            return BadRequest(new UniversalResponse(ErrorMessages.UniversalMessages.NotFoundWithId(_entity, id)));
        }

        return Ok(new UniversalResponse<OrderItemDTO>(responseObject: result.ToOrderItemDto()));
    }

    [HttpGet, Route(ApiRoutesDb.UniversalActions.GetAllAction)]
    public IActionResult GetAllOrderItems()
    {
        List<OrderItem> result = _dbContext.OrderItems.ToList();

        return Ok(new UniversalResponse<List<OrderItemDTO>>(responseObject: result.Select(u => u.ToOrderItemDto()).ToList()));
    }
}