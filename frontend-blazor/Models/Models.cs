using System.Text.Json.Serialization;

namespace NeonGadgetStore.Web.Models;

public class Product
{
    [JsonPropertyName("id")]
    public string Id { get; set; } = string.Empty;

    [JsonPropertyName("name")]
    public string Name { get; set; } = string.Empty;

    [JsonPropertyName("slug")]
    public string Slug { get; set; } = string.Empty;

    [JsonPropertyName("description")]
    public string? Description { get; set; }

    [JsonPropertyName("price")]
    public decimal Price { get; set; }

    [JsonPropertyName("original_price")]
    public decimal? OriginalPrice { get; set; }

    [JsonPropertyName("images")]
    public List<string> Images { get; set; } = new();

    // Image property - can be set directly or computed from Images
    private string? _image;
    public string Image 
    { 
        get => _image ?? Images.FirstOrDefault() ?? "https://via.placeholder.com/400";
        set => _image = value;
    }

    [JsonPropertyName("category_id")]
    public string? CategoryId { get; set; }

    // Category property - can be set directly or use CategoryId
    private string? _category;
    public string Category 
    { 
        get => _category ?? CategoryId ?? "";
        set => _category = value;
    }

    [JsonPropertyName("stock_quantity")]
    public int StockQuantity { get; set; }

    // InStock property - can be set directly or computed from StockQuantity
    private bool? _inStock;
    public bool InStock 
    { 
        get => _inStock ?? StockQuantity > 0;
        set => _inStock = value;
    }

    [JsonPropertyName("is_featured")]
    public bool IsFeatured { get; set; }

    // Featured property - alias for IsFeatured, can be set directly
    private bool? _featured;
    public bool Featured 
    { 
        get => _featured ?? IsFeatured;
        set { _featured = value; IsFeatured = value; }
    }

    [JsonPropertyName("is_active")]
    public bool IsActive { get; set; } = true;

    // These are computed/displayed values not from backend
    public double Rating { get; set; } = 4.5;
    public int Reviews { get; set; } = 0;
    public List<string> Features { get; set; } = new();
}

public class Category
{
    [JsonPropertyName("id")]
    public string Id { get; set; } = string.Empty;

    [JsonPropertyName("name")]
    public string Name { get; set; } = string.Empty;

    [JsonPropertyName("slug")]
    public string Slug { get; set; } = string.Empty;

    [JsonPropertyName("description")]
    public string? Description { get; set; }

    [JsonPropertyName("icon")]
    public string? Icon { get; set; }

    [JsonPropertyName("image_url")]
    public string? ImageUrl { get; set; }

    [JsonPropertyName("is_active")]
    public bool IsActive { get; set; } = true;

    [JsonPropertyName("sort_order")]
    public int SortOrder { get; set; }
}

public class CartItem
{
    public Product Product { get; set; } = null!;
    public int Quantity { get; set; }
}

public class User
{
    [JsonPropertyName("id")]
    public string Id { get; set; } = string.Empty;

    [JsonPropertyName("email")]
    public string? Email { get; set; }
}

public class Profile
{
    [JsonPropertyName("id")]
    public string Id { get; set; } = string.Empty;

    [JsonPropertyName("user_id")]
    public string UserId { get; set; } = string.Empty;

    [JsonPropertyName("email")]
    public string Email { get; set; } = string.Empty;

    [JsonPropertyName("username")]
    public string Username { get; set; } = string.Empty;

    [JsonPropertyName("full_name")]
    public string? FullName { get; set; }

    [JsonPropertyName("avatar_url")]
    public string? AvatarUrl { get; set; }

    [JsonPropertyName("phone")]
    public string? Phone { get; set; }

    [JsonPropertyName("address")]
    public string? Address { get; set; }
}

public class Address
{
    [JsonPropertyName("full_name")]
    public string FullName { get; set; } = string.Empty;

    [JsonPropertyName("phone")]
    public string Phone { get; set; } = string.Empty;

    [JsonPropertyName("address_line1")]
    public string AddressLine1 { get; set; } = string.Empty;

    [JsonPropertyName("address_line2")]
    public string? AddressLine2 { get; set; }

    [JsonPropertyName("city")]
    public string City { get; set; } = string.Empty;

    [JsonPropertyName("state")]
    public string State { get; set; } = string.Empty;

    [JsonPropertyName("postal_code")]
    public string PostalCode { get; set; } = string.Empty;

    [JsonPropertyName("country")]
    public string Country { get; set; } = "US";

    // Street - can be set directly or computed from AddressLine1
    private string? _street;
    public string Street 
    { 
        get => _street ?? AddressLine1 + (string.IsNullOrEmpty(AddressLine2) ? "" : ", " + AddressLine2);
        set { _street = value; AddressLine1 = value; }
    }

    // ZipCode - alias for PostalCode
    private string? _zipCode;
    public string ZipCode 
    { 
        get => _zipCode ?? PostalCode;
        set { _zipCode = value; PostalCode = value; }
    }
}

public class Order
{
    [JsonPropertyName("id")]
    public string Id { get; set; } = string.Empty;

    [JsonPropertyName("user_id")]
    public string? UserId { get; set; }

    [JsonPropertyName("order_number")]
    public string OrderNumber { get; set; } = string.Empty;

    [JsonPropertyName("subtotal")]
    public decimal Subtotal { get; set; }

    [JsonPropertyName("shipping_cost")]
    public decimal ShippingCost { get; set; }

    [JsonPropertyName("tax_amount")]
    public decimal TaxAmount { get; set; }

    [JsonPropertyName("total_amount")]
    public decimal TotalAmount { get; set; }

    // Total - can be set directly or computed from TotalAmount
    private decimal? _total;
    public decimal Total 
    { 
        get => _total ?? TotalAmount;
        set { _total = value; TotalAmount = value; }
    }

    [JsonPropertyName("payment_method")]
    public string PaymentMethod { get; set; } = "cod";

    [JsonPropertyName("payment_status")]
    public string PaymentStatus { get; set; } = "pending";

    [JsonPropertyName("status")]
    public string Status { get; set; } = "pending";

    [JsonPropertyName("shipping_address_json")]
    public string? ShippingAddressJson { get; set; }

    public Address? ShippingAddress { get; set; }

    [JsonPropertyName("created_at")]
    public DateTime CreatedAt { get; set; }

    [JsonPropertyName("updated_at")]
    public DateTime UpdatedAt { get; set; }

    [JsonPropertyName("shipped_at")]
    public DateTime? ShippedAt { get; set; }

    [JsonPropertyName("delivered_at")]
    public DateTime? DeliveredAt { get; set; }

    [JsonPropertyName("tracking_number")]
    public string? TrackingNumber { get; set; }

    [JsonPropertyName("items")]
    public List<OrderItem> Items { get; set; } = new();
}

public class OrderItem
{
    [JsonPropertyName("id")]
    public string Id { get; set; } = string.Empty;

    [JsonPropertyName("order_id")]
    public string OrderId { get; set; } = string.Empty;

    [JsonPropertyName("product_id")]
    public string ProductId { get; set; } = string.Empty;

    [JsonPropertyName("product_name")]
    public string ProductName { get; set; } = string.Empty;

    [JsonPropertyName("product_image")]
    public string? ProductImage { get; set; }

    [JsonPropertyName("price")]
    public decimal Price { get; set; }

    [JsonPropertyName("quantity")]
    public int Quantity { get; set; }

    // For backward compatibility with CartItem display
    public Product Product => new Product
    {
        Id = ProductId,
        Name = ProductName,
        Price = Price,
        Images = new List<string> { ProductImage ?? "" }
    };
}

public class UserWithProfile
{
    public string Id { get; set; } = string.Empty;
    public string? Email { get; set; }
    public Profile? Profile { get; set; }
    public List<string> Roles { get; set; } = new();
}

public class AuthResponse
{
    [JsonPropertyName("token")]
    public string Token { get; set; } = string.Empty;

    [JsonPropertyName("user")]
    public User User { get; set; } = null!;

    [JsonPropertyName("profile")]
    public Profile? Profile { get; set; }

    [JsonPropertyName("roles")]
    public List<string> Roles { get; set; } = new();
}

public class MeResponse
{
    [JsonPropertyName("user")]
    public User User { get; set; } = null!;

    [JsonPropertyName("profile")]
    public Profile? Profile { get; set; }

    [JsonPropertyName("roles")]
    public List<string> Roles { get; set; } = new();
}

public class WishlistEntry
{
    [JsonPropertyName("id")]
    public string Id { get; set; } = string.Empty;

    [JsonPropertyName("product_id")]
    public string ProductId { get; set; } = string.Empty;

    [JsonPropertyName("product")]
    public Product? Product { get; set; }
}

public class ViewedEntry
{
    [JsonPropertyName("id")]
    public string Id { get; set; } = string.Empty;

    [JsonPropertyName("product_id")]
    public string ProductId { get; set; } = string.Empty;

    [JsonPropertyName("product")]
    public Product? Product { get; set; }

    [JsonPropertyName("viewed_at")]
    public DateTime ViewedAt { get; set; }
}

// Request DTOs for API calls
public class CreateOrderRequest
{
    [JsonPropertyName("payment_method")]
    public string PaymentMethod { get; set; } = "card";

    [JsonPropertyName("shipping_address")]
    public Address ShippingAddress { get; set; } = new();

    [JsonPropertyName("items")]
    public List<CartItemRequest> Items { get; set; } = new();
}

public class CartItemRequest
{
    [JsonPropertyName("product_id")]
    public string ProductId { get; set; } = string.Empty;

    [JsonPropertyName("quantity")]
    public int Quantity { get; set; }
}
