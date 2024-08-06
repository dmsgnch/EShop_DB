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

[ApiController, Route(ApiRoutesDb.Controllers.ProductContr)]
public class ProductController : ControllerBase
{
    private readonly ApplicationDbContext _dbContext;
    private readonly string _entity;

    public ProductController(ApplicationDbContext dbContext)
    {
        _dbContext = dbContext;
        
        _entity = this.GetControllerName();
    }

    [HttpPost, Route(ApiRoutesDb.UniversalActions.CreateAction)]
    public IActionResult AddProduct([FromBody] ProductDTO productDto)
    {
        var product = productDto.ToProduct();
        
        if (_dbContext.Products.Any(p => p.Name.Equals(product.Name) && p.SellerId.Equals(product.SellerId)))
        {
            return BadRequest(new UniversalResponse(ErrorMessages.ProductMessages.AlreadyExistsNameSeller));
        }

        if (!product.ProductId.Equals(Guid.Empty))
        {
            if (_dbContext.Products.Any(p => p.ProductId.Equals(product.ProductId)))
            {
                return BadRequest(
                    new UniversalResponse<Product>(
                        errorInfo: ErrorMessages.UniversalMessages.AlreadyExistsId(_entity, product.ProductId)));
            }
        }
        else
        {
            product.ProductId = Guid.NewGuid();
        }

        _dbContext.Products.Add(product);
        _dbContext.SaveChanges();

        return Ok(new UniversalResponse<ProductDTO>(responseObject: product.ToProductDto(), info: SuccessMessages.UniversalResponse.Created(_entity)));
    }

    [HttpDelete, Route(ApiRoutesDb.UniversalActions.DeleteAction)]
    public IActionResult DeleteProduct([FromBody] Guid id)
    {
        var result = _dbContext.Products
            .Include(p => p.OrderItems)
            .FirstOrDefault(p => p.ProductId.Equals(id));

        if (result is null)
        {
            return BadRequest(new UniversalResponse(errorInfo: ErrorMessages.UniversalMessages.NotFoundWithId(_entity, id)));
        }

        if (result.OrderItems != null && result.OrderItems.Any())
        {
            _dbContext.OrderItems.RemoveRange(result.OrderItems);
        }

        _dbContext.SaveChanges();

        _dbContext.Products.Remove(result);
        _dbContext.SaveChanges();

        return Ok(new UniversalResponse(info: SuccessMessages.UniversalResponse.Deleted(_entity)));
    }

    [HttpPut, Route(ApiRoutesDb.UniversalActions.UpdateAction)]
    public IActionResult UpdateProduct([FromBody] ProductDTO productDto)
    {
        var product = productDto.ToProduct();
        
        var result = _dbContext.Products.FirstOrDefault(p => p.ProductId.Equals(product.ProductId));

        if (result is null)
        {
            return BadRequest(
                new UniversalResponse<ProductDTO>(
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

        return Ok(new UniversalResponse<ProductDTO>(responseObject: product.ToProductDto(), info: SuccessMessages.UniversalResponse.Updated(_entity)));
    }

    [HttpGet, Route(ApiRoutesDb.UniversalActions.GetByIdAction)]
    public IActionResult GetProductById([FromBody] Guid id)
    {
        var product = _dbContext.Products.FirstOrDefault(p => p.ProductId.Equals(id));

        if (product is null)
        {
            return BadRequest(new UniversalResponse(ErrorMessages.UniversalMessages.NotFoundWithId(_entity, id)));
        }

        AddSellerIfNull(product);

        return Ok(new UniversalResponse<Product>(responseObject: product));
    }

    [HttpGet, Route(ApiRoutesDb.UniversalActions.GetAllAction)]
    public IActionResult GetAllProducts()
    {
        List<Product> products = _dbContext.Products.ToList();

        foreach (var product in products)
        {
            AddSellerIfNull(product);
        }

        return Ok(new UniversalResponse<List<ProductDTO>>(responseObject: products.Select(p => p.ToProductDto()).ToList()));
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