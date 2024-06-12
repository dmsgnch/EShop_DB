using EShop_DB.Common.Constants;
using EShop_DB.Data;
using Microsoft.AspNetCore.Mvc;
using SharedLibrary.Models.MainModels;
using SharedLibrary.Routes;

namespace EShop_DB.Controllers;

[ApiController, Route("seller")]
public class SellerController : ControllerBase
{
    private readonly ApplicationDbContext _dbContext;
    private readonly string _entity = "Seller";

    public SellerController(ApplicationDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    [HttpPost, Route(ApiRoutesDb.Universal.Create)]
    public IActionResult AddSeller([FromBody] Seller seller)
    {
        if (_dbContext.Sellers.Any(s => s.EmailAddress.Equals(seller.EmailAddress)
                                        || s.CompanyName.Equals(seller.CompanyName)))
        {
            return BadRequest(ErrorMessages.Seller.AlreadyExistsEmailOrName);
        }

        if (!seller.SellerId.Equals(Guid.Empty))
        {
            if (_dbContext.Sellers.Any(s => s.SellerId.Equals(seller.SellerId)))
            {
                return BadRequest(ErrorMessages.Universal.AlreadyExistsId(_entity, seller.SellerId));
            }
        }
        else
        {
            seller.SellerId = Guid.NewGuid();
        }

        _dbContext.Sellers.Add(seller);
        _dbContext.SaveChanges();

        return Ok();
    }

    [HttpDelete, Route(ApiRoutesDb.Universal.Delete)]
    public IActionResult DeleteSeller([FromRoute] Guid id)
    {
        var result = _dbContext.Sellers.FirstOrDefault(s => s.SellerId.Equals(id));

        if (result is null)
        {
            return BadRequest(ErrorMessages.Universal.NotFoundWithId(_entity, id));
        }

        _dbContext.Sellers.Remove(result);
        _dbContext.SaveChanges();

        return Ok();
    }

    [HttpPut, Route(ApiRoutesDb.Universal.Update)]
    public IActionResult UpdateSeller([FromBody] Seller seller)
    {
        var result = _dbContext.Sellers.FirstOrDefault(s => s.SellerId.Equals(seller.SellerId));

        if (result is null)
        {
            return BadRequest(ErrorMessages.Universal.NotFoundWithId(_entity, seller.SellerId));
        }

        result.SellerId = seller.SellerId;
        result.EmailAddress = seller.EmailAddress;
        result.CompanyName = seller.CompanyName;
        result.Products = seller.Products;
        result.ImageUrl = seller.ImageUrl;
        result.ContactNumber = seller.ContactNumber;
        result.CompanyDescription = seller.CompanyDescription;
        result.AdditionNumber = seller.AdditionNumber;

        _dbContext.Sellers.Update(result);
        _dbContext.SaveChanges();

        return Ok();
    }

    [HttpGet, Route(ApiRoutesDb.Universal.GetById)]
    public IActionResult GetSellerById([FromRoute] Guid id)
    {
        var result = _dbContext.Sellers.FirstOrDefault(s => s.SellerId.Equals(id));

        if (result is null)
        {
            return BadRequest(ErrorMessages.Universal.NotFoundWithId(_entity, id));
        }

        return Ok(result);
    }

    [HttpGet, Route(ApiRoutesDb.Universal.GetAll)]
    public IActionResult GetAllSellers()
    {
        List<Seller> result = _dbContext.Sellers.ToList();

        return Ok(result);
    }
}