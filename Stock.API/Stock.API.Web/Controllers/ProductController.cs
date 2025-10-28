using FluentValidation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
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
    [HttpPost]
    public IActionResult Create([FromBody] ProductDTO productDTO)
    {
        var validationResult = _productDTOValidator.Validate(productDTO);

        if (!validationResult.IsValid) return BadRequest(validationResult.Errors);

        var product = new Product(productDTO.Name!, productDTO.Description!, 
                                 (decimal)productDTO.Price!, 
                                 (int)productDTO.AmountInStock!);

        var createdProduct = _productService.Create(product);
        
        return Ok(new { createdProduct.ID, createdProduct.Name, createdProduct.Price, createdProduct.AmountInStock });
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

        var updatedProduct = _productService.UpdateProduct(id, product);

        return Ok(updatedProduct);
    }

    [Authorize(Roles = "Admin")]
    [HttpDelete("{id:guid}")]
    public IActionResult Delete(Guid id)
    {
        if (id == Guid.Empty) return BadRequest("Incorrect format.");

        var deletedProduct = _productService.DeleteProduct(id);

        return Ok(new { deletedProduct.ID, deletedProduct.Name });
    }

    [Authorize(Roles = "Admin,SellsAPI")]
    [HttpGet]
    public IActionResult GetAll() => Ok(_productService.GetAll());
}
