using EShop_DB.Common.Constants;
using EShop_DB.Data;
using EShop_DB.Models.Enums;
using EShop_DB.Models.MainModels;
using EShop_DB.Models.SecondaryModels;
using EShop_DB.Common.Constants.Routes;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace EShop_DB.Controllers;

[ApiController, Route("orderEvent")]
public class OrderEventController : ControllerBase
{
    private readonly ApplicationDbContext _dbContext;
    private readonly string _entity = "Order event";

    public OrderEventController(ApplicationDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    [HttpPost, Route(ApiRoutes.Universal.Create)]
    public IActionResult AddOrderEvent([FromBody] OrderEvent orderEvent)
    {
        ValidateOrderEvent(orderEvent);

        if (!orderEvent.OrderEventId.Equals(Guid.Empty))
        {
            if (_dbContext.OrderEvents.Any(oe => oe.OrderEventId.Equals(orderEvent.OrderEventId)))
            {
                return BadRequest(ErrorMessages.Universal.AlreadyExistsId(_entity, orderEvent.OrderEventId));
            }
        }
        else
        {
            orderEvent.OrderEventId = Guid.NewGuid();
        }

        _dbContext.OrderEvents.Add(orderEvent);
        _dbContext.SaveChanges();

        return Ok();
    }

    private void ValidateOrderEvent(OrderEvent orderEvent)
    {
        //TODO: Add validation after order event logic implementation

        //return BadRequest(ErrorMessages.Product.AlreadyExistsNameSeller);
    }

    [HttpDelete, Route(ApiRoutes.Universal.Delete)]
    public IActionResult DeleteOrderEvent([FromRoute] Guid id)
    {
        var result = _dbContext.OrderEvents.FirstOrDefault(oe => oe.OrderEventId.Equals(id));

        if (result is null)
        {
            return BadRequest(ErrorMessages.Universal.NotFoundWithId(_entity, id));
        }

        _dbContext.OrderEvents.Remove(result);
        _dbContext.SaveChanges();

        return Ok();
    }

    [HttpPut, Route(ApiRoutes.Universal.Update)]
    public IActionResult UpdateOrderEvent([FromBody] OrderEvent orderEvent)
    {
        var result = _dbContext.OrderEvents.FirstOrDefault(oe => oe.OrderEventId.Equals(orderEvent.OrderEventId));

        if (result is null)
        {
            return BadRequest(ErrorMessages.Universal.NotFoundWithId(_entity, orderEvent.OrderEventId));
        }

        result.OrderEventId = orderEvent.OrderEventId;
        result.EventTime = orderEvent.EventTime;
        result.Stage = orderEvent.Stage;
        
        result.OrderId = orderEvent.OrderId;
        result.Order = orderEvent.Order;


        _dbContext.OrderEvents.Update(result);
        _dbContext.SaveChanges();

        return Ok();
    }

    [HttpGet, Route(ApiRoutes.Universal.GetById)]
    public IActionResult GetOrderEventById([FromRoute] Guid id)
    {
        var result = _dbContext.OrderEvents.FirstOrDefault(oe => oe.OrderEventId.Equals(id));

        if (result is null)
        {
            return BadRequest(ErrorMessages.Universal.NotFoundWithId(_entity, id));
        }

        return Ok(result);
    }

    [HttpGet, Route(ApiRoutes.Universal.GetAll)]
    public IActionResult GetAllOrderEvents()
    {
        List<OrderEvent> result = _dbContext.OrderEvents.ToList();

        return Ok(result);
    }
}