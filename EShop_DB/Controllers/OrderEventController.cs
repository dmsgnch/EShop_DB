using EShop_DB.Common.Constants;
using EShop_DB.Data;
using Microsoft.AspNetCore.Mvc;
using SharedLibrary.Models.DbModels.SecondaryModels;
using SharedLibrary.Responses;
using SharedLibrary.Routes;

namespace EShop_DB.Controllers;

[ApiController, Route(ApiRoutesDb.Controllers.OrderEventContr)]
public class OrderEventController : ControllerBase
{
    private readonly ApplicationDbContext _dbContext;
    private readonly string _entity = "Order event";

    public OrderEventController(ApplicationDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    [HttpPost, Route(ApiRoutesDb.UniversalActions.CreatePath)]
    public IActionResult AddOrderEvent([FromBody] OrderEvent orderEvent)
    {
        ValidateOrderEvent(orderEvent);

        if (!orderEvent.OrderEventId.Equals(Guid.Empty))
        {
            if (_dbContext.OrderEvents.Any(oe => oe.OrderEventId.Equals(orderEvent.OrderEventId)))
            {
                return BadRequest(
                    new LambdaResponse(ErrorMessages.UniversalMessages.AlreadyExistsId(_entity, orderEvent.OrderEventId)));
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

    [HttpDelete, Route(ApiRoutesDb.UniversalActions.DeleteControllerPath)]
    public IActionResult DeleteOrderEvent([FromRoute] Guid id)
    {
        var result = _dbContext.OrderEvents.FirstOrDefault(oe => oe.OrderEventId.Equals(id));

        if (result is null)
        {
            return BadRequest(new LambdaResponse(ErrorMessages.UniversalMessages.NotFoundWithId(_entity, id)));
        }

        _dbContext.OrderEvents.Remove(result);
        _dbContext.SaveChanges();

        return Ok();
    }

    [HttpPut, Route(ApiRoutesDb.UniversalActions.UpdatePath)]
    public IActionResult UpdateOrderEvent([FromBody] OrderEvent orderEvent)
    {
        var result = _dbContext.OrderEvents.FirstOrDefault(oe => oe.OrderEventId.Equals(orderEvent.OrderEventId));

        if (result is null)
        {
            return BadRequest(new LambdaResponse(ErrorMessages.UniversalMessages.NotFoundWithId(_entity, orderEvent.OrderEventId)));
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

    [HttpGet, Route(ApiRoutesDb.UniversalActions.GetByIdControllerPath)]
    public IActionResult GetOrderEventById([FromRoute] Guid id)
    {
        var result = _dbContext.OrderEvents.FirstOrDefault(oe => oe.OrderEventId.Equals(id));

        if (result is null)
        {
            return BadRequest(new LambdaResponse(ErrorMessages.UniversalMessages.NotFoundWithId(_entity, id)));
        }

        return Ok(result);
    }

    [HttpGet, Route(ApiRoutesDb.UniversalActions.GetAllPath)]
    public IActionResult GetAllOrderEvents()
    {
        List<OrderEvent> result = _dbContext.OrderEvents.ToList();

        return Ok(result);
    }
}