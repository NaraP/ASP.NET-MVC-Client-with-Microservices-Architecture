using ECommerce.MVC.IServices;
using ECommerce.MVC.Models;
using ECommerce.MVC.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ECommerce.MVC.Controllers;

[Authorize]
public class CartController(IInventoryService inventory) : Controller
{
    private readonly IInventoryService _inventory = inventory;

    public IActionResult Index()
    {
        var cart = CartService.GetCart(HttpContext.Session);
        return View(cart);
    }

    [HttpPost]
    public async Task<IActionResult> Add(int productId, int quantity = 1)
    {
        var product = await _inventory.GetProductAsync(productId);
        if (product is null) return NotFound();

        CartService.AddItem(HttpContext.Session, new CartItem
        {
            ProductId   = product.Id,
            ProductName = product.Name,
            UnitPrice   = product.Price,
            Quantity    = quantity,
            ImageUrl    = product.ImageUrl
        });

        TempData["Success"] = $"{product.Name} added to cart.";
        return RedirectToAction("Index", "Home");
    }

    [HttpPost]
    public IActionResult Remove(int productId)
    {
        CartService.RemoveItem(HttpContext.Session, productId);
        return RedirectToAction("Index");
    }

    [HttpPost]
    public IActionResult UpdateQuantity(int productId, int quantity)
    {
        CartService.UpdateQuantity(HttpContext.Session, productId, quantity);
        return RedirectToAction("Index");
    }

    [HttpGet]
    public IActionResult GetCount()
    {
        var cart = CartService.GetCart(HttpContext.Session);
        return Json(new { count = cart.ItemCount });
    }
}
