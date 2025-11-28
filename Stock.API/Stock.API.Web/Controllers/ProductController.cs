using FluentValidation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Stock.API.Core.Common;
using Stock.API.Core.Contracts.Service;
using Stock.API.Core.Entities;
using Stock.API.Web.DTOs;

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

    [Authorize(Roles = "Admin")]
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

    [Authorize(Roles = "Admin")]
    [HttpDelete("delete/{productCode:int}")]
    public async Task<IActionResult> Delete(int productCode)
    {
        if (productCode <= 0) return BadRequest(ErrorMessages.INCORRECTFORMAT);

        var deletedProduct = await _productService.DeleteProductAsync(productCode);

        return Ok(new { deletedProduct.ID, deletedProduct.Name });
    }

    [Authorize(Roles = "Admin,Buyer")]
    [HttpGet("all")]
    public async Task<IActionResult> GetAll()
    {
        var products = await _productService.GetAllAsync();

        if (!products.Any()) return BadRequest(ErrorMessages.NOPRODUCTSFOUND);

        return Ok(products);
    }

    [Authorize(Roles = "Admin,Buyer")]
    [HttpGet("product/{productCode:int}")]
    public async Task<IActionResult> GetByCode(int productCode)
    {
        if (productCode <= 0) return BadRequest(ErrorMessages.INCORRECTFORMAT);

        var product = await _productService.GetByCodeAsync(productCode);

        if (product is null) return NotFound(ErrorMessages.PRODUCTNOTFOUND);

        return Ok(product);
    }

    [Authorize(Roles = "SalesAPI")]
    [HttpGet("exists/{productCode}")]
    public async Task<bool> Exists(int productCode) => 
        await _productService.IsExistingByCodeAsync(productCode);
}
