namespace ECommerce.MVC.Models;

// ── Products / Inventory ────────────────────────────
public class ProductViewModel
{
    public int     Id          { get; set; }
    public string  Name        { get; set; } = string.Empty;
    public string  Category    { get; set; } = string.Empty;
    public decimal Price       { get; set; }
    public int     Stock       { get; set; }
    public string  Description { get; set; } = string.Empty;
    public string  ImageUrl    { get; set; } = string.Empty;
    public double  Rating      { get; set; }
}

// ── Cart ────────────────────────────────────────────
public class CartItem
{
    public int     ProductId   { get; set; }
    public string  ProductName { get; set; } = string.Empty;
    public decimal UnitPrice   { get; set; }
    public int     Quantity    { get; set; }
    public string  ImageUrl    { get; set; } = string.Empty;
    public decimal TotalPrice  => UnitPrice * Quantity;
}

public class CartViewModel
{
    public List<CartItem> Items { get; set; } = new();
    public decimal SubTotal    => Items.Sum(i => i.TotalPrice);
    public decimal Tax         => Math.Round(SubTotal * 0.08m, 2);
    public decimal Total       => SubTotal + Tax;
    public int ItemCount       => Items.Sum(i => i.Quantity);
}

// ── Checkout / Payment ──────────────────────────────
public class CheckoutViewModel
{
    public CartViewModel Cart      { get; set; } = new();
    public string FullName         { get; set; } = string.Empty;
    public string Address          { get; set; } = string.Empty;
    public string City             { get; set; } = string.Empty;
    public string ZipCode          { get; set; } = string.Empty;
    public string Country          { get; set; } = string.Empty;
    public string CardNumber       { get; set; } = string.Empty;
    public string CardHolder       { get; set; } = string.Empty;
    public string ExpiryDate       { get; set; } = string.Empty;
    public string Cvv              { get; set; } = string.Empty;
    public string PaymentMethod    { get; set; } = "Credit Card";
}

// ── Order Confirmation ──────────────────────────────
public class OrderConfirmationViewModel
{
    public string  OrderId           { get; set; } = string.Empty;
    public string  PaymentReference  { get; set; } = string.Empty;
    public string  Status            { get; set; } = string.Empty;
    public decimal TotalAmount       { get; set; }
    public DateTime PlacedAt         { get; set; }
    public DateTime EstimatedDelivery{ get; set; }
    public string ShippingAddress    { get; set; } = string.Empty;
    public List<OrderItemViewModel> Items { get; set; } = new();
}

public class OrderItemViewModel
{
    public string  ProductName { get; set; } = string.Empty;
    public int     Quantity    { get; set; }
    public decimal UnitPrice   { get; set; }
    public decimal TotalPrice  { get; set; }
}
