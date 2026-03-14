using ECommerce.OrderApi.Data;
using ECommerce.OrderApi.Entities;
using ECommerce.OrderApi.IServices;
using ECommerce.OrderApi.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ECommerce.OrderApi.Controllers;

[ApiController]
[Authorize]
[Route("api/[controller]")]
public class OrdersController : ControllerBase
{
    private readonly IOrderService _service;

    public OrdersController(IOrderService service)
    {
        _service = service;
    }

    [HttpPost]
    public async Task<ActionResult<OrderResponse>> CreateOrder(CreateOrderRequest request)
    {
        var result = await _service.CreateOrderAsync(request);

        return CreatedAtAction(nameof(GetById),
            new { orderId = result.OrderId },
            result);
    }

    [HttpGet("{orderId}")]
    public async Task<ActionResult<OrderResponse>> GetById(string orderId)
    {
        var order = await _service.GetOrderByIdAsync(orderId);

        if (order == null)
            return NotFound(new { message = $"Order {orderId} not found." });

        return Ok(order);
    }

    [HttpGet("user/{userId}")]
    public async Task<ActionResult<IEnumerable<OrderResponse>>> GetByUser(string userId)
    {
        var orders = await _service.GetOrdersByUserAsync(userId);

        return Ok(orders);
    }

    [HttpPatch("{orderId}/status")]
    public async Task<IActionResult> UpdateStatus(string orderId, UpdateStatusRequest request)
    {
        var updated = await _service.UpdateOrderStatusAsync(orderId, request.Status);

        if (!updated)
            return NotFound();

        return Ok(new { orderId, request.Status });
    }
}
