using ECommerce.PaymentApi.Entities;
using ECommerce.PaymentApi.IServices;
using ECommerce.PaymentApi.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ECommerce.PaymentApi.Controllers;

[ApiController]
[Authorize]
[Route("api/[controller]")]
public class PaymentsController : ControllerBase
{
    private readonly IPaymentService _paymentService;

    public PaymentsController(IPaymentService paymentService)
    {
        _paymentService = paymentService;
    }

    [HttpPost("process")]
    public async Task<ActionResult<PaymentResponse>> ProcessPayment(PaymentRequest request)
    {
        var result = await _paymentService.ProcessPaymentAsync(request);

        if (result.Status == "Failed")
            return BadRequest(result);

        return Ok(result);
    }

    [HttpGet("{paymentReference}")]
    public async Task<ActionResult<PaymentRecord>> GetPayment(string paymentReference)
    {
        var payment = await _paymentService.GetPaymentAsync(paymentReference);

        if (payment == null)
            return NotFound(new { message = $"Payment {paymentReference} not found." });

        return Ok(payment);
    }

    [HttpGet("user/{userId}")]
    public async Task<ActionResult<IEnumerable<PaymentRecord>>> GetByUser(string userId)
    {
        var payments = await _paymentService.GetPaymentsByUserAsync(userId);
        return Ok(payments);
    }

    [HttpPatch("{paymentReference}/order")]
    public async Task<IActionResult> LinkOrder(string paymentReference, LinkOrderRequest request)
    {
        var linked = await _paymentService.LinkOrderAsync(paymentReference, request.OrderId);

        if (!linked)
            return NotFound();

        return Ok(new { paymentReference, request.OrderId });
    }
}
