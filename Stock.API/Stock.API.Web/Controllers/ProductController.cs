using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using Stock.API.Core.Common;
using Stock.API.Core.Contracts.Service;
using Stock.API.Core.Entities;
using Stock.API.Web.DTOs;

namespace StockAPI.Controllers;

[ApiController]
[Route("stock")]
public class ProductController : ControllerBase
{
    private readonly IProductService _productService;
    private readonly IValidator<ProductDTO> _productDTOValidator;

    public ProductController(IProductService productService, IValidator<ProductDTO> productDTOValidator)
    {
        _productService = productService;
        _productDTOValidator = productDTOValidator;
    }

    [HttpPost("create")]
    public async Task<IActionResult> Create([FromBody] ProductDTO productDTO)
    {
        var validationResult = _productDTOValidator.Validate(productDTO);

        if (!validationResult.IsValid) return BadRequest(validationResult.Errors);

        var product = new Product(productDTO.Name!, productDTO.Description!, 
                                 (decimal)productDTO.Price!, 
                                 (int)productDTO.AmountInStock!);

        var createdProduct = await _productService.CreateAsync(product);
        
        return Ok(createdProduct);
    }

    [HttpPut("update/{productCode:int}")]
    public async Task<IActionResult> Update(int productCode, [FromBody] ProductDTO productDTO)
    {
        if (productCode <= 0) return BadRequest(ErrorMessages.INCORRECTFORMAT);

        var validationResult = _productDTOValidator.Validate(productDTO);

        if (!validationResult.IsValid) return BadRequest(validationResult.Errors);

        var product = new Product(
            productDTO.Name!, 
            productDTO.Description!, 
            (decimal)productDTO.Price!, 
            (int)productDTO.AmountInStock!
        );

        var updatedProduct = await _productService.UpdateProductAsync(productCode, product);

        return Ok(updatedProduct);
    }

    [HttpDelete("delete/{productCode:int}")]
    public async Task<IActionResult> Delete(int productCode)
    {
        if (productCode <= 0) return BadRequest(ErrorMessages.INCORRECTFORMAT);

        var deletedProduct = await _productService.DeleteProductAsync(productCode);

        return Ok(new { deletedProduct.ID, deletedProduct.Name });
    }

    [HttpGet("all")]
    public async Task<IActionResult> GetAll()
    {
        var products = await _productService.GetAllAsync();

        if (!products.Any()) return BadRequest(ErrorMessages.NOPRODUCTSFOUND);

        return Ok(products);
    }

    [HttpGet("product/{productCode:int}")]
    public async Task<IActionResult> GetByCode(int productCode)
    {
        if (productCode <= 0) return BadRequest(ErrorMessages.INCORRECTFORMAT);

        var product = await _productService.GetByCodeAsync(productCode);

        if (product is null) return NotFound(ErrorMessages.PRODUCTNOTFOUND);

        return Ok(product);
    }

    [HttpGet("exists/{productCode}")]
    public async Task<bool> Exists(int productCode) => 
        await _productService.IsExistingByCodeAsync(productCode);
}
