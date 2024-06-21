using EShop_DB.Common.Constants;
using EShop_DB.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SharedLibrary.Models.DbModels.MainModels;
using SharedLibrary.Models.DtoModels.MainModels;
using SharedLibrary.Responses;
using SharedLibrary.Routes;

namespace EShop_DB.Controllers;

[ApiController, Route(ApiRoutesDb.Controllers.ProductContr)]
public class ProductController : ControllerBase
{
    private readonly ApplicationDbContext _dbContext;
    private readonly string _entity = "Product";

    public ProductController(ApplicationDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    [HttpPost, Route(ApiRoutesDb.UniversalActions.CreatePath)]
    public IActionResult AddProduct([FromBody] Product product)
    {
        if (_dbContext.Products.Any(p => p.Name.Equals(product.Name) && p.SellerId.Equals(product.SellerId)))
        {
            return BadRequest(new LambdaResponse(ErrorMessages.ProductMessages.AlreadyExistsNameSeller));
        }

        if (!product.ProductId.Equals(Guid.Empty))
        {
            if (_dbContext.Products.Any(p => p.ProductId.Equals(product.ProductId)))
            {
                return BadRequest(
                    new LambdaResponse<Product>(
                        errorInfo: ErrorMessages.UniversalMessages.AlreadyExistsId(_entity, product.ProductId)));
            }
        }
        else
        {
            product.ProductId = Guid.NewGuid();
        }

        _dbContext.Products.Add(product);
        _dbContext.SaveChanges();

        return Ok(new LambdaResponse<Product>(responseObject: product, info: SuccessMessages.ProductMessages.Created));
    }

    [HttpDelete, Route(ApiRoutesDb.UniversalActions.DeleteControllerPath)]
    public IActionResult DeleteProduct([FromRoute] Guid id)
    {
        var result = _dbContext.Products
            .Include(p => p.OrderItems)
            .FirstOrDefault(p => p.ProductId.Equals(id));

        if (result is null)
        {
            return BadRequest(new LambdaResponse(errorInfo: ErrorMessages.UniversalMessages.NotFoundWithId(_entity, id)));
        }

        if (result.OrderItems != null && result.OrderItems.Any())
        {
            _dbContext.OrderItems.RemoveRange(result.OrderItems);
        }

        _dbContext.SaveChanges();

        _dbContext.Products.Remove(result);
        _dbContext.SaveChanges();

        return Ok(new LambdaResponse(info: SuccessMessages.ProductMessages.Deleted));
    }

    [HttpPut, Route(ApiRoutesDb.UniversalActions.UpdatePath)]
    public IActionResult UpdateProduct([FromBody] Product product)
    {
        var result = _dbContext.Products.FirstOrDefault(p => p.ProductId.Equals(product.ProductId));

        if (result is null)
        {
            return BadRequest(
                new LambdaResponse<Product>(
                    errorInfo: ErrorMessages.UniversalMessages.NotFoundWithId(_entity, product.ProductId)));
        }

        result.ProductId = product.ProductId;

        result.ImageUrl = product.ImageUrl;
        result.Name = product.Name;
        result.Description = product.Description;
        result.PricePerUnit = product.PricePerUnit;
        result.WeightInGrams = product.WeightInGrams;
        if (!product.InStock.Equals(-1))
        {
            result.InStock = product.InStock;
        }

        result.SellerId = product.SellerId;

        _dbContext.Products.Update(result);
        _dbContext.SaveChanges();

        AddSellerIfNull(product);

        return Ok(new LambdaResponse<Product>(responseObject: product, info: SuccessMessages.ProductMessages.Updated));
    }

    [HttpGet, Route(ApiRoutesDb.UniversalActions.GetByIdControllerPath)]
    public IActionResult GetProductById([FromRoute] Guid id)
    {
        var product = _dbContext.Products.FirstOrDefault(p => p.ProductId.Equals(id));

        if (product is null)
        {
            return BadRequest(new LambdaResponse(ErrorMessages.UniversalMessages.NotFoundWithId(_entity, id)));
        }

        AddSellerIfNull(product);

        return Ok(new LambdaResponse<Product>(responseObject: product));
    }

    [HttpGet, Route(ApiRoutesDb.UniversalActions.GetAllPath)]
    public IActionResult GetAllProducts()
    {
        List<Product> products = _dbContext.Products.ToList();

        foreach (var product in products)
        {
            AddSellerIfNull(product);
        }

        return Ok(new LambdaResponse<List<Product>>(responseObject: products));
    }

    private void AddSellerIfNull(Product product)
    {
        if (!product.SellerId.Equals(Guid.Empty) && product.Seller is null)
        {
            var seller = _dbContext.Sellers.FirstOrDefault(r => r.SellerId.Equals(product.SellerId))
                         ?? throw new Exception($"Seller with id: {product.SellerId} is not exist!");

            seller.Products = null;
            product.Seller = seller;
        }
    }
}