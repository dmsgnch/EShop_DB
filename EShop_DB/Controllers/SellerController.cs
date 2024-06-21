using EShop_DB.Common.Constants;
using EShop_DB.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SharedLibrary.Models.DbModels.MainModels;
using SharedLibrary.Models.DtoModels.MainModels;
using SharedLibrary.Responses;
using SharedLibrary.Routes;

namespace EShop_DB.Controllers;

[ApiController, Route(ApiRoutesDb.Controllers.SellerContr)]
public class SellerController : ControllerBase
{
    private readonly ApplicationDbContext _dbContext;
    private readonly string _entity = "Seller";

    public SellerController(ApplicationDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    [HttpPost, Route(ApiRoutesDb.UniversalActions.CreatePath)]
    public IActionResult AddSeller([FromBody] Seller seller)
    {
        if (_dbContext.Sellers.Any(s => s.EmailAddress.Equals(seller.EmailAddress)
                                        || s.CompanyName.Equals(seller.CompanyName)))
        {
            return BadRequest(new LambdaResponse<Seller>(errorInfo: ErrorMessages.SellerMessages.AlreadyExistsEmailOrName));
        }

        if (!seller.SellerId.Equals(Guid.Empty))
        {
            if (_dbContext.Sellers.Any(s => s.SellerId.Equals(seller.SellerId)))
            {
                return BadRequest(
                    new LambdaResponse<Seller>(
                        errorInfo: ErrorMessages.UniversalMessages.AlreadyExistsId(_entity, seller.SellerId)));
            }
        }
        else
        {
            seller.SellerId = Guid.NewGuid();
        }

        _dbContext.Sellers.Add(seller);
        _dbContext.SaveChanges();

        return Ok(new LambdaResponse<Seller>(responseObject: seller, info: SuccessMessages.SellerMessages.Created));
    }

    [HttpDelete, Route(ApiRoutesDb.UniversalActions.DeleteControllerPath)]
    public IActionResult DeleteSeller([FromRoute] Guid id)
    {
        var seller = _dbContext.Sellers
            .Include(s => s.Products)
            .Include(s => s.Users)
            .FirstOrDefault(s => s.SellerId.Equals(id));

        if (seller is null)
        {
            return BadRequest(new LambdaResponse(errorInfo: ErrorMessages.UniversalMessages.NotFoundWithId(_entity, id)));
        }

        if (seller.Products != null && seller.Products.Any())
        {
            _dbContext.Products.RemoveRange(seller.Products);
        }
        
        if (seller.Users != null && seller.Users.Any())
        {
            _dbContext.Users.RemoveRange(seller.Users);
        }
        
        _dbContext.SaveChanges();

        _dbContext.Sellers.Remove(seller);
        
        _dbContext.SaveChanges();

        return Ok(new LambdaResponse(info: SuccessMessages.SellerMessages.Deleted));
    }

    [HttpPut, Route(ApiRoutesDb.UniversalActions.UpdatePath)]
    public IActionResult UpdateSeller([FromBody] Seller seller)
    {
        var result = _dbContext.Sellers.FirstOrDefault(s => s.SellerId.Equals(seller.SellerId));

        if (result is null)
        {
            return BadRequest(
                new LambdaResponse<Seller>(ErrorMessages.UniversalMessages.NotFoundWithId(_entity, seller.SellerId)));
        }

        result.SellerId = seller.SellerId;

        result.EmailAddress = seller.EmailAddress;
        result.CompanyName = seller.CompanyName;

        result.ImageUrl = seller.ImageUrl;
        result.ContactNumber = seller.ContactNumber;
        result.CompanyDescription = seller.CompanyDescription;
        result.AdditionNumber = seller.AdditionNumber;

        _dbContext.Sellers.Update(result);
        _dbContext.SaveChanges();

        return Ok(new LambdaResponse<Seller>(responseObject: result, info: SuccessMessages.SellerMessages.Updated));
    }

    [HttpGet, Route(ApiRoutesDb.UniversalActions.GetByIdControllerPath)]
    public IActionResult GetSellerById([FromRoute] Guid id)
    {
        var result = _dbContext.Sellers.FirstOrDefault(s => s.SellerId.Equals(id));

        if (result is null)
        {
            return BadRequest(
                new LambdaResponse<Seller>(errorInfo: ErrorMessages.UniversalMessages.NotFoundWithId(_entity, id)));
        }

        return Ok(new LambdaResponse<Seller>(responseObject: result));
    }

    [HttpGet, Route(ApiRoutesDb.UniversalActions.GetAllPath)]
    public IActionResult GetAllSellers()
    {
        List<Seller> result = _dbContext.Sellers.ToList();

        return Ok(new LambdaResponse<List<Seller>>(responseObject: result));
    }
}