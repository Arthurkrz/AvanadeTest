using Microsoft.AspNetCore.Mvc;
using Stock.API.Core.Contracts.Service;
using Stock.API.Core.Entities;
using Stock.API.Core.Enum;
using Stock.API.Web;
using Stock.API.Web.DTOs;

namespace StockAPI.Controllers;

[ApiController]
[Route("[controller]")]
public class ProductController : ControllerBase
{
    private readonly IProductService _productService;

    public ProductController(IProductService productService)
    {
        _productService = productService;
    }

    [HttpPost]
    public IActionResult Create([FromBody] ProductDTO productDTO)
    {
        // ADD 400, 401 AND 403 ERROR HANDLING

        if (string.IsNullOrWhiteSpace(productDTO.Name) || 
            string.IsNullOrWhiteSpace(productDTO.Description) || 
            productDTO.Price == 0 || productDTO.AmountInStock == 0)
            return BadRequest("Incorrect format.");

        var product = new Product(productDTO.Name, productDTO.Description, 
                                 (decimal)productDTO.Price!, 
                                 (int)productDTO.AmountInStock!);

        var creationResult = _productService.Create(product);

        if (!creationResult.Success) 
            return HTTPErrorMapper.Map(creationResult.ErrorType ?? ErrorType.Undefined, 
                                       creationResult.Errors ?? new List<string>());
        
        return Ok(creationResult.Value);
    }

    [HttpPut("{id:guid}")]
    public IActionResult Update(Guid id, [FromBody] int sellAmount)
    {
        // UNAUTHORIZED AND FORBIDDEN ERROR HANDLING

        if (id == Guid.Empty || sellAmount == 0 || sellAmount < 0)
            return BadRequest("Incorrect format.");

        var updateResult = _productService.UpdateStock(id, sellAmount);

        if (!updateResult.Success) return
                HTTPErrorMapper.Map(updateResult.ErrorType ?? ErrorType.Undefined, 
                                    updateResult.Errors ?? new List<string>());

        return Ok(updateResult.Value);
    }

    [HttpGet]
    public IActionResult GetAll() => Ok(_productService.GetAll());
}
