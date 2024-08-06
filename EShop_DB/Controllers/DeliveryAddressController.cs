using EShop_DB.Common.Constants;
using EShop_DB.Common.Extensions;
using EShop_DB.Components;
using EShop_DB.Data;
using Microsoft.AspNetCore.Mvc;
using EShop_DB.Models.SecondaryModels;
using SharedLibrary.Models.DtoModels.SecondaryModels;
using SharedLibrary.Responses;
using SharedLibrary.Routes;

namespace EShop_DB.Controllers;

[ApiController, Route(ApiRoutesDb.Controllers.DeliveryAddressContr)]
public class DeliveryAddressController : ControllerBase
{
    private readonly ApplicationDbContext _dbContext;
    private readonly string _entity;

    public DeliveryAddressController(ApplicationDbContext dbContext)
    {
        _dbContext = dbContext;
        
        _entity = this.GetFormatedControllerName();
    }

    [HttpPost, Route(ApiRoutesDb.UniversalActions.CreateAction)]
    public IActionResult AddDeliveryAddress([FromBody] DeliveryAddressDTO deliveryAddressDto)
    {
        var deliveryAddress = deliveryAddressDto.ToDeliveryAddress();

        if (!deliveryAddress.DeliveryAddressId.Equals(Guid.Empty))
        {
            if (_dbContext.DeliveryAddresses.Any(da => da.DeliveryAddressId.Equals(deliveryAddress.DeliveryAddressId)))
            {
                return BadRequest(
                    new UniversalResponse(
                        ErrorMessages.UniversalMessages.AlreadyExistsId(_entity, deliveryAddress.DeliveryAddressId))
                );
            }
        }
        else
        {
            deliveryAddress.DeliveryAddressId = Guid.NewGuid();
        }

        _dbContext.DeliveryAddresses.Add(deliveryAddress);
        _dbContext.SaveChanges();

        return Ok(new UniversalResponse<DeliveryAddressDTO>(responseObject: deliveryAddress.ToDeliveryAddressDto(), info: SuccessMessages.UniversalResponse.Created(_entity)));
    }

    [HttpDelete, Route(ApiRoutesDb.UniversalActions.DeleteAction)]
    public IActionResult DeleteDeliveryAddress([FromBody] Guid id)
    {
        var result = _dbContext.DeliveryAddresses.FirstOrDefault(da => da.DeliveryAddressId.Equals(id));

        if (result is null)
        {
            return BadRequest(new UniversalResponse(ErrorMessages.UniversalMessages.NotFoundWithId(_entity, id)));
        }

        _dbContext.DeliveryAddresses.Remove(result);
        _dbContext.SaveChanges();

        return Ok(new UniversalResponse(info: SuccessMessages.UniversalResponse.Deleted(_entity)));
    }

    [HttpPut, Route(ApiRoutesDb.UniversalActions.UpdateAction)]
    public IActionResult UpdateDeliveryAddress([FromBody] DeliveryAddressDTO deliveryAddressDto)
    {
        var deliveryAddress = deliveryAddressDto.ToDeliveryAddress();
        
        var result =
            _dbContext.DeliveryAddresses.FirstOrDefault(da =>
                da.DeliveryAddressId.Equals(deliveryAddress.DeliveryAddressId));

        if (result is null)
        {
            return BadRequest(
                new UniversalResponse(ErrorMessages.UniversalMessages.NotFoundWithId(_entity, deliveryAddress.DeliveryAddressId)));
        }

        result.DeliveryAddressId = deliveryAddress.DeliveryAddressId;
        result.City = deliveryAddress.City;
        result.Street = deliveryAddress.Street;
        result.House = deliveryAddress.House;
        result.Apartment = deliveryAddress.Apartment;
        result.Floor = deliveryAddress.Floor;

        result.UserId = deliveryAddress.UserId;
        result.User = deliveryAddress.User;

        result.OrderId = deliveryAddress.OrderId;
        result.Order = deliveryAddress.Order;

        _dbContext.DeliveryAddresses.Update(result);
        _dbContext.SaveChanges();

        return Ok(new UniversalResponse<DeliveryAddressDTO>(responseObject: deliveryAddress.ToDeliveryAddressDto(), info: SuccessMessages.UniversalResponse.Updated(_entity)));
    }

    [HttpGet, Route(ApiRoutesDb.UniversalActions.GetByIdAction)]
    public IActionResult GetDeliveryAddressById([FromBody] Guid id)
    {
        var result = _dbContext.DeliveryAddresses.FirstOrDefault(da => da.DeliveryAddressId.Equals(id));

        if (result is null)
        {
            return BadRequest(new UniversalResponse(ErrorMessages.UniversalMessages.NotFoundWithId(_entity, id)));
        }

        return Ok(new UniversalResponse<DeliveryAddressDTO>(responseObject: result.ToDeliveryAddressDto()));
    }

    [HttpGet, Route(ApiRoutesDb.UniversalActions.GetAllAction)]
    public IActionResult GetAllDeliveryAddresses()
    {
        List<DeliveryAddress> result = _dbContext.DeliveryAddresses.ToList();

        return Ok(new UniversalResponse<List<DeliveryAddressDTO>>(responseObject: result.Select(u => u.ToDeliveryAddressDto()).ToList()));
    }
}