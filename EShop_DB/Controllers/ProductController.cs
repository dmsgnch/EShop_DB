using EShop_BL.Models.MainModels;
using EShop_DB.Common.Constants;
using EShop_DB.Data;
using Microsoft.AspNetCore.Mvc;
using SharedLibrary.Routes;

namespace EShop_DB.Controllers;

[ApiController, Route("product")]
public class ProductController : ControllerBase
{
    private readonly ApplicationDbContext _dbContext;
    private readonly string _entity = "Product";

    public ProductController(ApplicationDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    [HttpPost, Route(ApiRoutesDb.Universal.Create)]
    public IActionResult AddProduct([FromBody] Product product)
    {
        if (_dbContext.Products.Any(p => p.Name.Equals(product.Name) && p.SellerId.Equals(product.SellerId)))
        {
            return BadRequest(ErrorMessages.Product.AlreadyExistsNameSeller);
        }

        if (!product.ProductId.Equals(Guid.Empty))
        {
            if (_dbContext.Products.Any(p => p.ProductId.Equals(product.ProductId)))
            {
                return BadRequest(ErrorMessages.Universal.AlreadyExistsId(_entity, product.ProductId));
            }
        }
        else
        {
            product.ProductId = Guid.NewGuid();
        }

        _dbContext.Products.Add(product);
        _dbContext.SaveChanges();

        return Ok();
    }

    [HttpDelete, Route(ApiRoutesDb.Universal.Delete)]
    public IActionResult DeleteProduct([FromRoute] Guid id)
    {
        var result = _dbContext.Products.FirstOrDefault(p => p.ProductId.Equals(id));

        if (result is null)
        {
            return BadRequest(ErrorMessages.Universal.NotFoundWithId(_entity, id));
        }

        _dbContext.Products.Remove(result);
        _dbContext.SaveChanges();

        return Ok();
    }

    [HttpPut, Route(ApiRoutesDb.Universal.Update)]
    public IActionResult UpdateProduct([FromBody] Product product)
    {
        var result = _dbContext.Products.FirstOrDefault(p => p.ProductId.Equals(product.ProductId));

        if (result is null)
        {
            return BadRequest(ErrorMessages.Universal.NotFoundWithId(_entity, product.ProductId));
        }

        result.ProductId = product.ProductId;
        result.ImageUrl = product.ImageUrl;
        result.Name = product.Name;
        result.Description = product.Description;
        result.PricePerUnit = product.PricePerUnit;
        result.WeightInGrams = product.WeightInGrams;
        result.InStock = product.InStock;

        result.SellerId = product.SellerId;
        result.Seller = product.Seller;

        result.OrderItems = product.OrderItems;

        _dbContext.Products.Update(result);
        _dbContext.SaveChanges();

        return Ok();
    }

    [HttpGet, Route(ApiRoutesDb.Universal.GetById)]
    public IActionResult GetProductById([FromRoute] Guid id)
    {
        var result = _dbContext.Products.FirstOrDefault(p => p.ProductId.Equals(id));

        if (result is null)
        {
            return BadRequest(ErrorMessages.Universal.NotFoundWithId(_entity, id));
        }

        return Ok(result);
    }

    [HttpGet, Route(ApiRoutesDb.Universal.GetAll)]
    public IActionResult GetAllProducts()
    {
        List<Product> result = _dbContext.Products.ToList();
        
        return Ok(result);
    }
}