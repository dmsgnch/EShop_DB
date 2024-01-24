using EShop_DB.Data;
using EShop_DB.Models;
using Microsoft.AspNetCore.Mvc;

namespace EShop_DB.Controllers;

[Route("product")]
[ApiController]
public class ProductController : Controller
{
    private readonly ApplicationContext _context;

    public ProductController(ApplicationContext context)
    {
        _context = context;
    }

    [Route("add")]
    [HttpPost]
    public IActionResult AddProduct([FromBody] Product product)
    {
        if (_context.Products.Any(p => p.Name.Equals(product.Name)))
        {
            return BadRequest("Product with the same name is already exist");
        }

        //Check, that guid is exist
        if (product.ProductId.Equals(Guid.Empty)) product.ProductId = Guid.NewGuid();

        _context.Products.Add(product);
        _context.SaveChanges();

        return Ok();
    }

    [Route("delete/{id:Guid}")]
    [HttpDelete]
    public IActionResult DeleteProduct([FromRoute] Guid id)
    {
        var result =
            _context.Products.FirstOrDefault(p =>
                p.ProductId.ToString().ToLower().Equals(id.ToString().ToLower()));

        if (result is null)
        {
            return BadRequest("Product was not found");
        }

        _context.Products.Remove(result);
        _context.SaveChanges();

        return Ok();
    }

    [Route("update")]
    [HttpPut]
    public IActionResult UpdateProduct([FromBody] Product product)
    {
        var result = _context.Products.FirstOrDefault(p => p.ProductId.Equals(product.ProductId));

        if (result is null)
        {
            return BadRequest("Product was not found");
        }

        result.ProductId = product.ProductId;
        result.ImageURL = product.ImageURL;
        result.Name = product.Name;
        result.Description = product.Description;
        result.PricePerUnit = product.PricePerUnit;
        result.WholesalePricePerUnit = product.WholesalePricePerUnit;
        result.WholesaleQuantity = product.WholesaleQuantity;
        result.InStock = product.InStock;
        result.WeightInGrams = product.WeightInGrams;

        _context.Products.Update(result);
        _context.SaveChanges();

        return Ok();
    }

    [Route("getById")]
    [HttpGet]
    public IActionResult GetProductById([FromRoute] Guid id)
    {
        var result = _context.Products.FirstOrDefault(p => p.ProductId.Equals(id));

        if (result is null)
        {
            return BadRequest("Product was not found");
        }

        return Ok(result);
    }

    [Route("getAll")]
    [HttpGet]
    public IActionResult GetAllProducts()
    {
        List<Product> foundProduct = _context.Products.ToList();

        return Ok(foundProduct);
    }
}