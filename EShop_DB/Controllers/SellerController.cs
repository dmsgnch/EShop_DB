using EShop_DB.Common.Constants;
using EShop_DB.Common.Extensions;
using EShop_DB.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using EShop_DB.Models.MainModels;
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

    [HttpPost, Route(ApiRoutesDb.UniversalActions.CreateAction)]
    public IActionResult AddSeller([FromBody] SellerDTO sellerDto)
    {
        var seller = sellerDto.ToSeller();
        
        if (_dbContext.Sellers.Any(s => s.EmailAddress.Equals(seller.EmailAddress)
                                        || s.CompanyName.Equals(seller.CompanyName)))
        {
            return BadRequest(new UniversalResponse<SellerDTO>(errorInfo: ErrorMessages.SellerMessages.AlreadyExistsEmailOrName));
        }

        if (!seller.SellerId.Equals(Guid.Empty))
        {
            if (_dbContext.Sellers.Any(s => s.SellerId.Equals(seller.SellerId)))
            {
                return BadRequest(
                    new UniversalResponse<SellerDTO>(
                        errorInfo: ErrorMessages.UniversalMessages.AlreadyExistsId(_entity, seller.SellerId)));
            }
        }
        else
        {
            seller.SellerId = Guid.NewGuid();
        }

        _dbContext.Sellers.Add(seller);
        _dbContext.SaveChanges();

        return Ok(new UniversalResponse<SellerDTO>(responseObject: seller.ToSellerDto(), info: SuccessMessages.UniversalResponse.Created(_entity)));
    }

    [HttpDelete, Route(ApiRoutesDb.UniversalActions.DeleteAction)]
    public IActionResult DeleteSeller([FromBody] Guid id)
    {
        //TODO: Must be rewrite
        var seller = _dbContext.Sellers
            .Include(s => s.Products)
            .Include(s => s.Users)
            .FirstOrDefault(s => s.SellerId.Equals(id));

        if (seller is null)
        {
            return BadRequest(new UniversalResponse(errorInfo: ErrorMessages.UniversalMessages.NotFoundWithId(_entity, id)));
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

        return Ok(new UniversalResponse(info: SuccessMessages.UniversalResponse.Deleted(_entity)));
    }

    [HttpPut, Route(ApiRoutesDb.UniversalActions.UpdateAction)]
    public IActionResult UpdateSeller([FromBody] SellerDTO sellerDto)
    {
        var seller = sellerDto.ToSeller();
        
        var result = _dbContext.Sellers.FirstOrDefault(s => s.SellerId.Equals(seller.SellerId));

        if (result is null)
        {
            return BadRequest(
                new UniversalResponse<SellerDTO>(ErrorMessages.UniversalMessages.NotFoundWithId(_entity, seller.SellerId)));
        }

        result.CompanyName = seller.CompanyName;
        result.EmailAddress = seller.EmailAddress;
        result.ContactNumber = seller.ContactNumber;

        result.ImageUrl = seller.ImageUrl;
        result.CompanyDescription = seller.CompanyDescription;
        result.AdditionNumber = seller.AdditionNumber;

        _dbContext.Sellers.Update(result);
        _dbContext.SaveChanges();

        return Ok(new UniversalResponse<SellerDTO>(responseObject: result.ToSellerDto(), info: SuccessMessages.UniversalResponse.Updated(_entity)));
    }

    [HttpGet, Route(ApiRoutesDb.UniversalActions.GetByIdAction)]
    public IActionResult GetSellerById([FromBody] Guid id)
    {
        var result = _dbContext.Sellers.FirstOrDefault(s => s.SellerId.Equals(id));

        if (result is null)
        {
            return BadRequest(
                new UniversalResponse<SellerDTO>(errorInfo: ErrorMessages.UniversalMessages.NotFoundWithId(_entity, id)));
        }

        return Ok(new UniversalResponse<SellerDTO>(responseObject: result.ToSellerDto()));
    }

    [HttpGet, Route(ApiRoutesDb.UniversalActions.GetAllAction)]
    public IActionResult GetAllSellers()
    {
        List<Seller> result = _dbContext.Sellers.ToList();

        return Ok(new UniversalResponse<List<SellerDTO>>(responseObject: result.Select(u => u.ToSellerDto()).ToList()));
    }
}