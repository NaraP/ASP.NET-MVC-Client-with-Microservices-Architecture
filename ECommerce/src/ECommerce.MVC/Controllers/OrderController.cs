using ECommerce.MVC.IServices;
using ECommerce.MVC.Models;
using ECommerce.MVC.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace ECommerce.MVC.Controllers;

[Authorize]
public class OrderController : Controller
{
    private readonly IPaymentService _payment;
    private readonly IOrderService   _order;

    public OrderController(IPaymentService payment, IOrderService order)
    {
        _payment = payment;
        _order   = order;
    }

    [HttpGet]
    public IActionResult Checkout()
    {
        var cart = CartService.GetCart(HttpContext.Session);
        if (!cart.Items.Any()) return RedirectToAction("Index", "Cart");

        var userName = User.FindFirstValue(ClaimTypes.Name) ?? string.Empty;

        return View(new CheckoutViewModel
        {
            Cart       = cart,
            FullName   = userName,
            Country    = "United States",
            CardHolder = userName
        });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> PlaceOrder(CheckoutViewModel model)
    {
        var cart = CartService.GetCart(HttpContext.Session);
        model.Cart = cart;

        if (!cart.Items.Any())
            return RedirectToAction("Index", "Cart");

        // Remove card validation from ModelState for display fields
        ModelState.Remove("Cart");

        if (!ModelState.IsValid) return View("Checkout", model);

        var userId    = User.FindFirstValue(ClaimTypes.NameIdentifier) ?? "UNKNOWN";
        var userEmail = User.FindFirstValue(ClaimTypes.Email) ?? string.Empty;

        // 1️⃣ Process Payment
        var paymentResult = await _payment.ProcessPaymentAsync(new PaymentApiRequest
        {
            UserId      = userId,
            Amount      = cart.Total,
            Currency    = "USD",
            CardNumber  = model.CardNumber.Replace(" ", ""),
            CardHolder  = model.CardHolder,
            ExpiryDate  = model.ExpiryDate,
            Cvv         = model.Cvv,
            PaymentMethod = model.PaymentMethod
        });

        if (paymentResult is null || paymentResult.Status != "Success")
        {
            TempData["PaymentError"] = paymentResult?.Message ?? "Payment failed. Please try again.";
            return View("Checkout", model);
        }

        // 2️⃣ Create Order
        var shippingAddress = $"{model.FullName}, {model.Address}, {model.City}, {model.ZipCode}, {model.Country}";

        var orderResult = await _order.CreateOrderAsync(new CreateOrderApiRequest
        {
            UserId           = userId,
            UserEmail        = userEmail,
            TotalAmount      = cart.Total,
            ShippingAddress  = shippingAddress,
            PaymentReference = paymentResult.PaymentReference,
            Items            = cart.Items.Select(i => new OrderItemApi
            {
                ProductId   = i.ProductId,
                ProductName = i.ProductName,
                Quantity    = i.Quantity,
                UnitPrice   = i.UnitPrice,
                TotalPrice  = i.TotalPrice
            }).ToList()
        });

        if (orderResult is null)
        {
            TempData["PaymentError"] = "Order creation failed. Please contact support with payment reference: " + paymentResult.PaymentReference;
            return View("Checkout", model);
        }

        // 3️⃣ Clear cart & redirect to confirmation
        CartService.ClearCart(HttpContext.Session);

        return RedirectToAction("Confirmation", new
        {
            orderId          = orderResult.OrderId,
            paymentReference = paymentResult.PaymentReference
        });
    }

    [HttpGet]
    public IActionResult Confirmation(string orderId, string paymentReference)
    {
        if (string.IsNullOrEmpty(orderId))
            return RedirectToAction("Index", "Home");

        // In production you'd fetch from Order API; here we use TempData or reconstruct
        var vm = new OrderConfirmationViewModel
        {
            OrderId           = orderId,
            PaymentReference  = paymentReference,
            Status            = "Confirmed",
            PlacedAt          = DateTime.Now,
            EstimatedDelivery = DateTime.Now.AddDays(5)
        };

        return View(vm);
    }
}
