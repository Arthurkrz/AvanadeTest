using FluentValidation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Stock.API.Core.Contracts.Service;
using Stock.API.Core.Entities;
using Stock.API.Core.Enum;
using Stock.API.Web.DTOs;
using Stock.API.Web.Utilities;

namespace StockAPI.Controllers;

[Authorize]
[ApiController]
[Route("api/[controller]")]
public class ProductController : ControllerBase
{
    private readonly IProductService _productService;
    private readonly IValidator<ProductDTO> _productDTOValidator;

    public ProductController(IProductService productService, IValidator<ProductDTO> productDTOValidator)
    {
        _productService = productService;
        _productDTOValidator = productDTOValidator;
    }

    [Authorize(Roles = "Admin")]
    [HttpPost]
    public IActionResult Create([FromBody] ProductDTO productDTO)
    {
        var validationResult = _productDTOValidator.Validate(productDTO);

        if (!validationResult.IsValid) return BadRequest(validationResult.Errors);

        var product = new Product(productDTO.Name!, productDTO.Description!, 
                                 (decimal)productDTO.Price!, 
                                 (int)productDTO.AmountInStock!);

        var creationResult = _productService.Create(product);

        if (!creationResult.Success) 
            return HTTPErrorMapper.Map(creationResult.ErrorType ?? ErrorType.Undefined, 
                                       creationResult.Errors ?? new List<string>());
        
        return Ok(creationResult.Value);
    }

    [Authorize(Roles = "Admin,SellsAPI")]
    [HttpPut("{id:guid}")]
    public IActionResult Sell(Guid id, [FromBody] int sellAmount)
    {
        if (id == Guid.Empty || sellAmount == 0 || sellAmount < 0)
            return BadRequest("Incorrect format.");

        var updateResult = _productService.UpdateStock(id, sellAmount);

        if (!updateResult.Success) return
                HTTPErrorMapper.Map(updateResult.ErrorType ?? ErrorType.Undefined, 
                                    updateResult.Errors ?? new List<string>());

        return Ok(updateResult.Value);
    }

    [Authorize(Roles = "Admin")]
    [HttpPut("update/{id:guid}")]
    public IActionResult Update(Guid id, [FromBody] ProductDTO productDTO)
    {
        if (id == Guid.Empty) return BadRequest("Invalid ID.");

        var validationResult = _productDTOValidator.Validate(productDTO);

        if (!validationResult.IsValid) return BadRequest(validationResult.Errors);

        var product = new Product(
            productDTO.Name!, 
            productDTO.Description!, 
            (decimal)productDTO.Price!, 
            (int)productDTO.AmountInStock!
        );

        var updateResult = _productService.UpdateProduct(id, product);

        if (!updateResult.Success) return
                HTTPErrorMapper.Map(updateResult.ErrorType ?? ErrorType.Undefined,
                                    updateResult.Errors ?? new List<string>());

        return Ok(updateResult.Value);
    }

    [Authorize(Roles = "Admin")]
    [HttpDelete("{id:guid}")]
    public IActionResult Delete(Guid id)
    {
        if (id == Guid.Empty) return BadRequest("Incorrect format.");

        var deleteResult = _productService.DeleteProduct(id);

        if (!deleteResult.Success) return 
                HTTPErrorMapper.Map(deleteResult.ErrorType ?? ErrorType.Undefined,
                                    deleteResult.Errors ?? new List<string>());

        return Ok(deleteResult.Value);
    }

    [Authorize(Roles = "Admin,SellsAPI")]
    [HttpGet]
    public IActionResult GetAll() => Ok(_productService.GetAll());
}
