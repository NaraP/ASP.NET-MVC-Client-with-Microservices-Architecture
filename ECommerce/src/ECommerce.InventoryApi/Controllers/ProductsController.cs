using ECommerce.InventoryApi.Dtos;
using ECommerce.InventoryApi.IServices;
using ECommerce.InventoryApi.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ECommerce.InventoryApi.Controllers;

[ApiController]
[Authorize]
[Route("api/[controller]")]
public class ProductsController : ControllerBase
{
    private readonly IProductService _service;

    public ProductsController(IProductService service)
    {
        _service = service;
    }

    [HttpGet]
    public async Task<ActionResult<PagedResult<ProductDto>>> GetAll(
        [FromQuery] string? category = null,
        [FromQuery] string? search = null,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 20)
    {
        var result = await _service.GetAllAsync(category, search, page, pageSize);
        return Ok(result);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<ProductDto>> GetById(int id)
    {
        var product = await _service.GetByIdAsync(id);

        if (product == null)
            return NotFound();

        return Ok(product);
    }

    [HttpGet("categories")]
    public async Task<ActionResult<IEnumerable<string>>> GetCategories()
    {
        return Ok(await _service.GetCategoriesAsync());
    }

    [HttpPost]
    public async Task<ActionResult<ProductDto>> Create(UpsertProductRequest req)
    {
        var product = await _service.CreateAsync(req);
        return CreatedAtAction(nameof(GetById), new { id = product.Id }, product);
    }

    [HttpPut("{id}")]
    public async Task<ActionResult<ProductDto>> Update(int id, UpsertProductRequest req)
    {
        var product = await _service.UpdateAsync(id, req);

        if (product == null)
            return NotFound();

        return Ok(product);
    }

    [HttpPatch("{id}/stock")]
    public async Task<IActionResult> UpdateStock(int id, UpdateStockRequest req)
    {
        var success = await _service.UpdateStockAsync(id, req);

        if (!success)
            return BadRequest("Insufficient stock or product not found");

        return Ok();
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        var success = await _service.DeleteAsync(id);

        if (!success)
            return NotFound();

        return NoContent();
    }
}
