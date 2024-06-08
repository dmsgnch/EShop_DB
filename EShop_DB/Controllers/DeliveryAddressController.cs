using EShop_BL.Models.SecondaryModels;
using EShop_DB.Common.Constants;
using EShop_DB.Data;
using Microsoft.AspNetCore.Mvc;
using SharedLibrary.Routes;

namespace EShop_DB.Controllers;

[ApiController, Route("deliveryAddress")]
public class DeliveryAddressController : ControllerBase
{
    private readonly ApplicationDbContext _dbContext;
    private readonly string _entity = "Delivery address";

    public DeliveryAddressController(ApplicationDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    [HttpPost, Route(ApiRoutesDb.Universal.Create)]
    public IActionResult AddDeliveryAddress([FromBody]DeliveryAddress deliveryAddress)
    {
        ValidateDeliveryAddress(deliveryAddress);
        
        if (!deliveryAddress.DeliveryAddressId.Equals(Guid.Empty))
        {
            if (_dbContext.DeliveryAddresses.Any(da => da.DeliveryAddressId.Equals(deliveryAddress.DeliveryAddressId)))
            {
                return BadRequest(ErrorMessages.Universal.AlreadyExistsId(_entity, deliveryAddress.DeliveryAddressId));
            }
        }
        else
        {
            deliveryAddress.DeliveryAddressId = Guid.NewGuid();
        }

        _dbContext.DeliveryAddresses.Add(deliveryAddress);
        _dbContext.SaveChanges();

        return Ok();
    }
    
    private void ValidateDeliveryAddress(DeliveryAddress deliveryAddress)
    {
        //TODO: Add validation after order event logic implementation

        //return BadRequest(ErrorMessages.Product.AlreadyExistsNameSeller);
    }
    
    [HttpDelete, Route(ApiRoutesDb.Universal.Delete)]
    public IActionResult DeleteDeliveryAddress([FromRoute]Guid id)
    {
        var result = _dbContext.DeliveryAddresses.FirstOrDefault(da => da.DeliveryAddressId.Equals(id));
        
        if (result is null)
        {
            return BadRequest(ErrorMessages.Universal.NotFoundWithId(_entity, id));
        }

        _dbContext.DeliveryAddresses.Remove(result);
        _dbContext.SaveChanges();

        return Ok();
    }
    
    [HttpPut, Route(ApiRoutesDb.Universal.Update)]
    public IActionResult UpdateDeliveryAddress([FromBody]DeliveryAddress deliveryAddress)
    {
        var result = _dbContext.DeliveryAddresses.FirstOrDefault(da => da.DeliveryAddressId.Equals(deliveryAddress.DeliveryAddressId));
        
        if (result is null)
        {
            return BadRequest(ErrorMessages.Universal.NotFoundWithId(_entity, deliveryAddress.DeliveryAddressId));
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

        return Ok();
    }
    
    [HttpGet, Route(ApiRoutesDb.Universal.GetById)]
    public IActionResult GetDeliveryAddressById([FromRoute]Guid id)
    {
        var result = _dbContext.DeliveryAddresses.FirstOrDefault(da => da.DeliveryAddressId.Equals(id));
        
        if (result is null)
        {
            return BadRequest(ErrorMessages.Universal.NotFoundWithId(_entity, id));
        }

        return Ok(result);
    }
    
    [HttpGet, Route(ApiRoutesDb.Universal.GetAll)]
    public IActionResult GetAllDeliveryAddresses()
    {
        List<DeliveryAddress> result = _dbContext.DeliveryAddresses.ToList();

        return Ok(result);
    }
}