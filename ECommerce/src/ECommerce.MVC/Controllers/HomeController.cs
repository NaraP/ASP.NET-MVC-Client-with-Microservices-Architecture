using ECommerce.MVC.IServices;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ECommerce.MVC.Controllers;

[Authorize]
public class HomeController : Controller
{
    private readonly IInventoryService _inventory;

    public HomeController(IInventoryService inventory)
    {
        _inventory = inventory;
    }

    public async Task<IActionResult> Index(string? category = null, string? search = null)
    {
        var products   = await _inventory.GetProductsAsync(category, search);
        var categories = await _inventory.GetCategoriesAsync();

        ViewBag.Categories       = categories;
        ViewBag.SelectedCategory = category;
        ViewBag.Search           = search;

        return View(products);
    }

    public async Task<IActionResult> Product(int id)
    {
        var product = await _inventory.GetProductAsync(id);
        if (product is null) return NotFound();
        return View(product);
    }
}
