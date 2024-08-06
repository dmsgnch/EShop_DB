using EShop_DB.Common.Constants;
using EShop_DB.Common.Extensions;
using EShop_DB.Components;
using EShop_DB.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using EShop_DB.Models.MainModels;
using EShop_DB.Models.SecondaryModels;
using SharedLibrary.Models.Enums;
using SharedLibrary.Models.DtoModels.MainModels;
using SharedLibrary.Requests;
using SharedLibrary.Responses;
using SharedLibrary.Routes;

namespace EShop_DB.Controllers;

[ApiController, Route(ApiRoutesDb.Controllers.OrderContr)]
public class OrderController : ControllerBase
{
    private readonly ApplicationDbContext _dbContext;
    private readonly string _entityCart = "Cart";
    private readonly string _entity;

    public OrderController(ApplicationDbContext dbContext)
    {
        _dbContext = dbContext;
        
        _entity = this.GetControllerName();
    }

    [HttpPost, Route(ApiRoutesDb.OrderActions.CreateCartAction)]
    public IActionResult CreateCart([FromBody] Guid userId)
    {
        // if (_dbContext.Orders.Include(o => o.OrderEvents).ToList().Any(o =>
        //         o.UserId.Equals(userId) && o.ProcessingStage.Equals(OrderProcessingStage.Cart)))
        // {
        //     return BadRequest(new UniversalResponse(ErrorMessages.OrderMessages.UserAlreadyHaveCart));
        // }

        Order cart = new Order(userId);

        _dbContext.Orders.Add(cart);
        _dbContext.SaveChanges();

        _dbContext.OrderEvents.Add(new OrderEvent(cart.OrderId));
        _dbContext.SaveChanges();

        var order = _dbContext.Orders.Include(o => o.OrderEvents).First(o => o.OrderId.Equals(cart.OrderId));
        if (order.OrderEvents is not null && order.OrderEvents.Count > 0)
        {
            foreach (var orderEvent in order.OrderEvents)
            {
                orderEvent.Order = null;
            }
        }

        var orderDto = order.ToOrderDto();
        return Ok(new UniversalResponse<OrderDTO>(responseObject: orderDto,
            info: SuccessMessages.UniversalResponse.Created(_entityCart)));
    }

    [HttpPost, Route(ApiRoutesDb.OrderActions.CreateOrderAction)]
    public IActionResult CreateOrderAsync([FromBody] Guid cartId)
    {
        _dbContext.OrderEvents.Add(new OrderEvent(cartId, OrderProcessingStage.Processing));
        _dbContext.SaveChanges();

        return Ok(new UniversalResponse(info: SuccessMessages.UniversalResponse.Created(_entity)));
    }

    [HttpDelete, Route(ApiRoutesDb.UniversalActions.DeleteAction)]
    public IActionResult DeleteOrder([FromBody] Guid id)
    {
        var result = _dbContext.Orders.FirstOrDefault(o => o.OrderId.Equals(id));

        if (result is null)
        {
            return BadRequest(new UniversalResponse(ErrorMessages.UniversalMessages.NotFoundWithId(_entity, id)));
        }

        _dbContext.Orders.Remove(result);
        _dbContext.SaveChanges();

        return Ok(new UniversalResponse(info: SuccessMessages.UniversalResponse.Deleted(_entity)));
    }

    [HttpPut, Route(ApiRoutesDb.UniversalActions.UpdateAction)]
    public IActionResult UpdateOrder([FromBody] OrderDTO orderDto)
    {
        var order = orderDto.ToOrder();

        var result = _dbContext.Orders.FirstOrDefault(o => o.OrderId.Equals(order.OrderId));

        if (result is null)
        {
            return BadRequest(
                new UniversalResponse(ErrorMessages.UniversalMessages.NotFoundWithId(_entity, order.OrderId)));
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

        return Ok(new UniversalResponse<OrderDTO>(responseObject: order.ToOrderDto(),
            info: SuccessMessages.UniversalResponse.Updated(_entity)));
    }

    [HttpGet, Route(ApiRoutesDb.UniversalActions.GetByIdAction)]
    public IActionResult GetOrderById([FromBody] Guid id)
    {
        var result = GetAllOrdersFunc().ResponseObject?.FirstOrDefault(o => o.OrderId.Equals(id));

        if (result is null)
        {
            return BadRequest(new UniversalResponse(ErrorMessages.UniversalMessages.NotFoundWithId(_entity, id)));
        }

        return Ok(new UniversalResponse<OrderDTO>(responseObject: result.ToOrderDto()));
    }

    [HttpGet, Route(ApiRoutesDb.UniversalActions.GetAllAction)]
    public IActionResult GetAllOrders()
    {
        var orders = GetAllOrdersFunc();
        return Ok(new UniversalResponse<List<OrderDTO>>(
            responseObject: orders.ResponseObject.Select(u => u.ToOrderDto()).ToList()));
    }

    private UniversalResponse<List<Order>> GetAllOrdersFunc()
    {
        List<Order> result = _dbContext.Orders
            .Include(o => o.OrderEvents)
            .Include(o => o.OrderItems).ThenInclude(oi => oi.Product)
            .ToList();

        if (result is not null && result.Count > 0)
        {
            foreach (var order in result)
            {
                if (order.OrderEvents is not null && order.OrderEvents.Count > 0)
                {
                    foreach (var orderEvent in order.OrderEvents)
                    {
                        orderEvent.Order = null;
                    }
                }

                if (order.OrderItems is not null && order.OrderItems.Count > 0)
                {
                    foreach (var orderItem in order.OrderItems)
                    {
                        orderItem.Product.OrderItems = null;
                        orderItem.Order = null;
                    }
                }
            }
        }

        return new UniversalResponse<List<Order>>(responseObject: result);
    }
}