using EShop_DB.Common.Constants;
using EShop_DB.Common.Extensions;
using EShop_DB.Data;
using Microsoft.AspNetCore.Mvc;
using EShop_DB.Models.SecondaryModels;
using SharedLibrary.Models.DtoModels.SecondaryModels;
using SharedLibrary.Responses;
using SharedLibrary.Routes;

namespace EShop_DB.Controllers;

[ApiController, Route(ApiRoutesDb.Controllers.OrderEventContr)]
public class OrderEventController : ControllerBase
{
    private readonly ApplicationDbContext _dbContext;
    private readonly string _entity;

    public OrderEventController(ApplicationDbContext dbContext)
    {
        _dbContext = dbContext;
        
        _entity = this.GetFormatedControllerName();
    }

    [HttpPost, Route(ApiRoutesDb.UniversalActions.CreateAction)]
    public IActionResult AddOrderEvent([FromBody] OrderEventDTO orderEventDto)
    {
        var orderEvent = orderEventDto.ToOrderEvent();
        
        ValidateOrderEvent(orderEvent);

        if (!orderEvent.OrderEventId.Equals(Guid.Empty))
        {
            if (_dbContext.OrderEvents.Any(oe => oe.OrderEventId.Equals(orderEvent.OrderEventId)))
            {
                return BadRequest(
                    new UniversalResponse(ErrorMessages.UniversalMessages.AlreadyExistsId(_entity, orderEvent.OrderEventId)));
            }
        }
        else
        {
            orderEvent.OrderEventId = Guid.NewGuid();
        }

        _dbContext.OrderEvents.Add(orderEvent);
        _dbContext.SaveChanges();

        return Ok(new UniversalResponse<OrderEventDTO>(responseObject: orderEvent.ToOrderEventDto(), info: SuccessMessages.UniversalResponse.Created(_entity)));
    }

    private void ValidateOrderEvent(OrderEvent orderEvent)
    {
        //TODO: Add validation after order event logic implementation

        //return BadRequest(ErrorMessages.Product.AlreadyExistsNameSeller);
    }

    [HttpDelete, Route(ApiRoutesDb.UniversalActions.DeleteAction)]
    public IActionResult DeleteOrderEvent([FromBody] Guid id)
    {
        var result = _dbContext.OrderEvents.FirstOrDefault(oe => oe.OrderEventId.Equals(id));

        if (result is null)
        {
            return BadRequest(new UniversalResponse(ErrorMessages.UniversalMessages.NotFoundWithId(_entity, id)));
        }

        _dbContext.OrderEvents.Remove(result);
        _dbContext.SaveChanges();

        return Ok(new UniversalResponse(info: SuccessMessages.UniversalResponse.Deleted(_entity)));
    }

    [HttpPut, Route(ApiRoutesDb.UniversalActions.UpdateAction)]
    public IActionResult UpdateOrderEvent([FromBody] OrderEventDTO orderEventDto)
    {
        var orderEvent = orderEventDto.ToOrderEvent();
        
        var result = _dbContext.OrderEvents.FirstOrDefault(oe => oe.OrderEventId.Equals(orderEvent.OrderEventId));

        if (result is null)
        {
            return BadRequest(new UniversalResponse(ErrorMessages.UniversalMessages.NotFoundWithId(_entity, orderEvent.OrderEventId)));
        }

        result.OrderEventId = orderEvent.OrderEventId;
        result.EventTime = orderEvent.EventTime;
        result.Stage = orderEvent.Stage;

        result.OrderId = orderEvent.OrderId;
        result.Order = orderEvent.Order;


        _dbContext.OrderEvents.Update(result);
        _dbContext.SaveChanges();

        return Ok(new UniversalResponse<OrderEventDTO>(responseObject: orderEvent.ToOrderEventDto(), info: SuccessMessages.UniversalResponse.Updated(_entity)));
    }

    [HttpGet, Route(ApiRoutesDb.UniversalActions.GetByIdAction)]
    public IActionResult GetOrderEventById([FromBody] Guid id)
    {
        var result = _dbContext.OrderEvents.FirstOrDefault(oe => oe.OrderEventId.Equals(id));

        if (result is null)
        {
            return BadRequest(new UniversalResponse(ErrorMessages.UniversalMessages.NotFoundWithId(_entity, id)));
        }

        return Ok(new UniversalResponse<OrderEventDTO>(responseObject: result.ToOrderEventDto()));
    }

    [HttpGet, Route(ApiRoutesDb.UniversalActions.GetAllAction)]
    public IActionResult GetAllOrderEvents()
    {
        List<OrderEvent> result = _dbContext.OrderEvents.ToList();

        return Ok(new UniversalResponse<List<OrderEventDTO>>(responseObject: result.Select(u => u.ToOrderEventDto()).ToList()));
    }
}