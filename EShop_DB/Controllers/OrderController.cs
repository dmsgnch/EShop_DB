using EShop_DB.Common.Constants;
using EShop_DB.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using SharedLibrary.Models.DbModels.MainModels;
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
    private readonly string _entity = "Order";

    public OrderController(ApplicationDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    [HttpPost, Route(ApiRoutesDb.OrderActions.CreatePath)]
    public IActionResult CreateCart([FromRoute] Guid userId)
    {
        if (_dbContext.Orders.Any(o =>
                o.UserId.Equals(userId) && o.ProcessingStage.Equals(OrderProcessingStage.Cart)))
        {
            return BadRequest(new LambdaResponse("This user already have a cart"));
        }

        Order cart = new Order()
        {
            OrderId = new Guid(),
            ProcessingStage = OrderProcessingStage.Cart,
            UserId = userId,
        };

        _dbContext.Orders.Add(cart);
        _dbContext.SaveChanges();

        return Ok();
    }

    private void ValidateOrder(Order order)
    {
        //TODO: Add validation after order event logic implementation

        //return BadRequest(ErrorMessages.Product.AlreadyExistsNameSeller);
    }

    [HttpDelete, Route(ApiRoutesDb.UniversalActions.DeleteControllerPath)]
    public IActionResult DeleteOrder([FromRoute] Guid id)
    {
        var result = _dbContext.Orders.FirstOrDefault(o => o.OrderId.Equals(id));

        if (result is null)
        {
            return BadRequest(new LambdaResponse(ErrorMessages.UniversalMessages.NotFoundWithId(_entity, id)));
        }

        _dbContext.Orders.Remove(result);
        _dbContext.SaveChanges();

        return Ok();
    }

    [HttpPut, Route(ApiRoutesDb.UniversalActions.UpdatePath)]
    public IActionResult UpdateOrder([FromBody] Order order)
    {
        var result = _dbContext.Orders.FirstOrDefault(o => o.OrderId.Equals(order.OrderId));

        if (result is null)
        {
            return BadRequest(new LambdaResponse(ErrorMessages.UniversalMessages.NotFoundWithId(_entity, order.OrderId)));
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

    [HttpGet, Route(ApiRoutesDb.UniversalActions.GetByIdControllerPath)]
    public IActionResult GetOrderById([FromRoute] Guid id)
    {
        var result = GetAllOrdersFunc().ResponseObject?.FirstOrDefault(o => o.OrderId.Equals(id));

        if (result is null)
        {
            return BadRequest(new LambdaResponse(ErrorMessages.UniversalMessages.NotFoundWithId(_entity, id)));
        }

        return Ok(new LambdaResponse<Order>(responseObject: result));
    }

    [HttpGet, Route(ApiRoutesDb.UniversalActions.GetAllPath)]
    public IActionResult GetAllOrders()
    {
        return Ok(GetAllOrdersFunc());
    }

    [HttpPost, Route(ApiRoutesDb.OrderActions.AddOrderItemPath)]
    public IActionResult AddOrderItem(ProductCartRequest request)
    {
        List<Order>? result = GetAllOrdersFunc().ResponseObject;
        
        if(result is null) throw new Exception("There are no any order");

        Order? cart = result.FirstOrDefault(r => r.OrderId.Equals(request.CartId));

        if(cart is null) throw new Exception("There are no any cart");

        _dbContext.OrderItems.Add(new OrderItem()
        {
            Quantity = 1,
            ProductId = request.ProductId,
            OrderId = request.CartId ?? throw new Exception("Cart id is null"),
        });

        _dbContext.SaveChanges();
        
        return Ok(result);
    }

    private LambdaResponse<List<Order>> GetAllOrdersFunc()
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

        return new LambdaResponse<List<Order>>(responseObject: result);
    }
}