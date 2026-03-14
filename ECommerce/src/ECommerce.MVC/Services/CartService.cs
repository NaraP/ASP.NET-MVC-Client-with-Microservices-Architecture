using ECommerce.MVC.Models;
using System.Text.Json;

namespace ECommerce.MVC.Services;

public class CartService
{
    private const string CartKey = "ShoppingCart";

    public static CartViewModel GetCart(ISession session)
    {
        var json = session.GetString(CartKey);
        if (string.IsNullOrEmpty(json)) return new CartViewModel();
        return JsonSerializer.Deserialize<CartViewModel>(json) ?? new CartViewModel();
    }

    public static void SaveCart(ISession session, CartViewModel cart)
    {
        session.SetString(CartKey, JsonSerializer.Serialize(cart));
    }

    public static void AddItem(ISession session, CartItem item)
    {
        var cart = GetCart(session);
        var existing = cart.Items.FirstOrDefault(i => i.ProductId == item.ProductId);

        if (existing is not null)
            existing.Quantity += item.Quantity;
        else
            cart.Items.Add(item);

        SaveCart(session, cart);
    }

    public static void RemoveItem(ISession session, int productId)
    {
        var cart = GetCart(session);
        cart.Items.RemoveAll(i => i.ProductId == productId);
        SaveCart(session, cart);
    }

    public static void UpdateQuantity(ISession session, int productId, int quantity)
    {
        var cart = GetCart(session);
        var item = cart.Items.FirstOrDefault(i => i.ProductId == productId);

        if (item is not null)
        {
            if (quantity <= 0)
                cart.Items.Remove(item);
            else
                item.Quantity = quantity;
        }

        SaveCart(session, cart);
    }

    public static void ClearCart(ISession session) =>
        session.Remove(CartKey);
}
