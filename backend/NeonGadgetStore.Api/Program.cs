using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using NeonGadgetStore.Api.Data;
using NeonGadgetStore.Api.Models;
using NeonGadgetStore.Api.Services;
using System.Text;
using System.Text.Json;

var builder = WebApplication.CreateBuilder(args);

builder.Services
    .AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.PropertyNamingPolicy = JsonNamingPolicy.SnakeCaseLower;
        options.JsonSerializerOptions.DictionaryKeyPolicy = JsonNamingPolicy.SnakeCaseLower;
        options.JsonSerializerOptions.PropertyNameCaseInsensitive = true;
    });

builder.Services.AddDbContext<AppDbContext>(options =>
{
    var cs = builder.Configuration.GetConnectionString("Default") ?? "Server=MINGDING;Database=NeonGadgetStore;Trusted_Connection=true;TrustServerCertificate=true;";
    options.UseSqlServer(cs);
});

builder.Services
    .AddIdentity<ApplicationUser, IdentityRole>(options =>
    {
        options.User.RequireUniqueEmail = true;
    })
    .AddEntityFrameworkStores<AppDbContext>()
    .AddDefaultTokenProviders();

builder.Services.AddScoped<JwtTokenService>();

builder.Services
    .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        var secret = builder.Configuration["JWT_SECRET"];
        if (string.IsNullOrWhiteSpace(secret))
        {
            throw new InvalidOperationException("JWT_SECRET is not configured");
        }

        var issuer = builder.Configuration["JWT_ISSUER"] ?? "NeonGadgetStore";
        var audience = builder.Configuration["JWT_AUDIENCE"] ?? "NeonGadgetStore";

        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateIssuerSigningKey = true,
            ValidateLifetime = true,
            ValidIssuer = issuer,
            ValidAudience = audience,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secret))
        };
    });

builder.Services.AddAuthorization();

builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(policy =>
    {
        policy
            .AllowAnyOrigin()
            .AllowAnyHeader()
            .AllowAnyMethod();
    });
});

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    db.Database.EnsureCreated();

    // Seed Categories
    if (!db.Categories.Any())
    {
        var categories = new List<Category>
        {
            new Category { Id = "cat-laptops", Name = "Laptops", Slug = "laptops", Description = "Máy tính xách tay cao cấp", Icon = "💻", SortOrder = 1 },
            new Category { Id = "cat-smartphones", Name = "Smartphones", Slug = "smartphones", Description = "Điện thoại thông minh", Icon = "📱", SortOrder = 2 },
            new Category { Id = "cat-tablets", Name = "Tablets", Slug = "tablets", Description = "Máy tính bảng", Icon = "📲", SortOrder = 3 },
            new Category { Id = "cat-headphones", Name = "Headphones", Slug = "headphones", Description = "Tai nghe cao cấp", Icon = "🎧", SortOrder = 4 },
            new Category { Id = "cat-smartwatches", Name = "Smartwatches", Slug = "smartwatches", Description = "Đồng hồ thông minh", Icon = "⌚", SortOrder = 5 },
            new Category { Id = "cat-cameras", Name = "Cameras", Slug = "cameras", Description = "Máy ảnh và phụ kiện", Icon = "📷", SortOrder = 6 },
            new Category { Id = "cat-accessories", Name = "Accessories", Slug = "accessories", Description = "Phụ kiện điện tử", Icon = "🔌", SortOrder = 7 },
        };
        db.Categories.AddRange(categories);
        db.SaveChanges();
    }

    // Seed Products
    if (!db.Products.Any())
    {
        var products = new List<Product>
        {
            // Laptops
            new Product { Name = "MacBook Pro 16\" M3 Max", Slug = "macbook-pro-16-m3-max", Price = 2499, OriginalPrice = 2799, CategoryId = "cat-laptops", StockQuantity = 25, IsFeatured = true, Description = "Laptop cao cấp với chip M3 Max, màn hình Liquid Retina XDR 16 inch, RAM 36GB, SSD 512GB", Images = new List<string> { "https://images.unsplash.com/photo-1517336714731-489689fd1ca8?w=500" } },
            new Product { Name = "Dell XPS 15", Slug = "dell-xps-15", Price = 1799, OriginalPrice = 1999, CategoryId = "cat-laptops", StockQuantity = 30, IsFeatured = true, Description = "Laptop Windows cao cấp với Intel Core i9, màn hình OLED 15.6 inch 4K, RAM 32GB, SSD 1TB", Images = new List<string> { "https://images.unsplash.com/photo-1593642632559-0c6d3fc62b89?w=500" } },
            new Product { Name = "ASUS ROG Zephyrus G16", Slug = "asus-rog-zephyrus-g16", Price = 2199, OriginalPrice = 2399, CategoryId = "cat-laptops", StockQuantity = 20, IsFeatured = false, Description = "Laptop gaming với RTX 4090, Intel Core i9-14900H, màn hình 16 inch 240Hz", Images = new List<string> { "https://images.unsplash.com/photo-1603302576837-37561b2e2302?w=500" } },
            new Product { Name = "Lenovo ThinkPad X1 Carbon", Slug = "lenovo-thinkpad-x1-carbon", Price = 1599, OriginalPrice = 1799, CategoryId = "cat-laptops", StockQuantity = 35, IsFeatured = false, Description = "Laptop doanh nhân siêu nhẹ, Intel Core i7, màn hình 14 inch 2.8K OLED", Images = new List<string> { "https://images.unsplash.com/photo-1588872657578-7efd1f1555ed?w=500" } },

            // Smartphones
            new Product { Name = "iPhone 15 Pro Max", Slug = "iphone-15-pro-max", Price = 1199, OriginalPrice = 1299, CategoryId = "cat-smartphones", StockQuantity = 50, IsFeatured = true, Description = "iPhone cao cấp nhất với chip A17 Pro, camera 48MP, titanium design, USB-C", Images = new List<string> { "https://images.unsplash.com/photo-1695048133142-1a20484d2569?w=500" } },
            new Product { Name = "Samsung Galaxy S24 Ultra", Slug = "samsung-galaxy-s24-ultra", Price = 1299, OriginalPrice = 1399, CategoryId = "cat-smartphones", StockQuantity = 45, IsFeatured = true, Description = "Flagship Android với S Pen, camera 200MP, màn hình Dynamic AMOLED 6.8 inch", Images = new List<string> { "https://images.unsplash.com/photo-1610945415295-d9bbf067e59c?w=500" } },
            new Product { Name = "Google Pixel 8 Pro", Slug = "google-pixel-8-pro", Price = 999, OriginalPrice = 1099, CategoryId = "cat-smartphones", StockQuantity = 40, IsFeatured = false, Description = "Điện thoại AI với chip Tensor G3, camera xuất sắc, 7 năm cập nhật", Images = new List<string> { "https://images.unsplash.com/photo-1598327105666-5b89351aff97?w=500" } },
            new Product { Name = "OnePlus 12", Slug = "oneplus-12", Price = 799, OriginalPrice = 899, CategoryId = "cat-smartphones", StockQuantity = 55, IsFeatured = false, Description = "Flagship killer với Snapdragon 8 Gen 3, sạc nhanh 100W, camera Hasselblad", Images = new List<string> { "https://images.unsplash.com/photo-1591337676887-a217a6970a8a?w=500" } },

            // Tablets
            new Product { Name = "iPad Pro 12.9\" M2", Slug = "ipad-pro-12-9-m2", Price = 1099, OriginalPrice = 1199, CategoryId = "cat-tablets", StockQuantity = 30, IsFeatured = true, Description = "Tablet mạnh nhất với chip M2, màn hình Liquid Retina XDR, hỗ trợ Apple Pencil 2", Images = new List<string> { "https://images.unsplash.com/photo-1544244015-0df4b3ffc6b0?w=500" } },
            new Product { Name = "Samsung Galaxy Tab S9 Ultra", Slug = "samsung-galaxy-tab-s9-ultra", Price = 1199, OriginalPrice = 1299, CategoryId = "cat-tablets", StockQuantity = 25, IsFeatured = false, Description = "Tablet Android cao cấp với màn hình AMOLED 14.6 inch, S Pen đi kèm", Images = new List<string> { "https://images.unsplash.com/photo-1561154464-82e9adf32764?w=500" } },

            // Headphones
            new Product { Name = "Sony WH-1000XM5", Slug = "sony-wh-1000xm5", Price = 349, OriginalPrice = 399, CategoryId = "cat-headphones", StockQuantity = 60, IsFeatured = true, Description = "Tai nghe chống ồn tốt nhất thế giới, pin 30 giờ, âm thanh Hi-Res", Images = new List<string> { "https://images.unsplash.com/photo-1618366712010-f4ae9c647dcb?w=500" } },
            new Product { Name = "Apple AirPods Pro 2", Slug = "apple-airpods-pro-2", Price = 249, OriginalPrice = 279, CategoryId = "cat-headphones", StockQuantity = 80, IsFeatured = true, Description = "Tai nghe true wireless với chống ồn chủ động, Spatial Audio, USB-C", Images = new List<string> { "https://images.unsplash.com/photo-1606220588913-b3aacb4d2f46?w=500" } },
            new Product { Name = "Bose QuietComfort Ultra", Slug = "bose-quietcomfort-ultra", Price = 429, OriginalPrice = 479, CategoryId = "cat-headphones", StockQuantity = 40, IsFeatured = false, Description = "Tai nghe over-ear với Immersive Audio, chống ồn hàng đầu", Images = new List<string> { "https://images.unsplash.com/photo-1546435770-a3e426bf472b?w=500" } },

            // Smartwatches
            new Product { Name = "Apple Watch Ultra 2", Slug = "apple-watch-ultra-2", Price = 799, OriginalPrice = 849, CategoryId = "cat-smartwatches", StockQuantity = 35, IsFeatured = true, Description = "Smartwatch cao cấp nhất của Apple, titanium, GPS dual-frequency, pin 36 giờ", Images = new List<string> { "https://images.unsplash.com/photo-1434493789847-2f02dc6ca35d?w=500" } },
            new Product { Name = "Samsung Galaxy Watch 6 Classic", Slug = "samsung-galaxy-watch-6-classic", Price = 399, OriginalPrice = 449, CategoryId = "cat-smartwatches", StockQuantity = 45, IsFeatured = false, Description = "Smartwatch Android với bezel xoay, theo dõi sức khỏe toàn diện", Images = new List<string> { "https://images.unsplash.com/photo-1579586337278-3befd40fd17a?w=500" } },

            // Cameras
            new Product { Name = "Sony A7 IV", Slug = "sony-a7-iv", Price = 2499, OriginalPrice = 2699, CategoryId = "cat-cameras", StockQuantity = 15, IsFeatured = true, Description = "Máy ảnh full-frame 33MP, quay video 4K 60fps, autofocus AI", Images = new List<string> { "https://images.unsplash.com/photo-1516035069371-29a1b244cc32?w=500" } },
            new Product { Name = "Canon EOS R6 Mark II", Slug = "canon-eos-r6-mark-ii", Price = 2299, OriginalPrice = 2499, CategoryId = "cat-cameras", StockQuantity = 18, IsFeatured = false, Description = "Máy ảnh mirrorless 24.2MP, quay 4K 60fps, chống rung 8 stop", Images = new List<string> { "https://images.unsplash.com/photo-1502920917128-1aa500764cbd?w=500" } },

            // Accessories
            new Product { Name = "Anker 737 Power Bank", Slug = "anker-737-power-bank", Price = 149, OriginalPrice = 179, CategoryId = "cat-accessories", StockQuantity = 100, IsFeatured = false, Description = "Sạc dự phòng 24000mAh, công suất 140W, sạc laptop được", Images = new List<string> { "https://images.unsplash.com/photo-1609091839311-d5365f9ff1c5?w=500" } },
            new Product { Name = "Logitech MX Master 3S", Slug = "logitech-mx-master-3s", Price = 99, OriginalPrice = 119, CategoryId = "cat-accessories", StockQuantity = 70, IsFeatured = false, Description = "Chuột không dây cao cấp, MagSpeed scroll, kết nối 3 thiết bị", Images = new List<string> { "https://images.unsplash.com/photo-1527864550417-7fd91fc51a46?w=500" } },
            new Product { Name = "Samsung T7 Shield 2TB", Slug = "samsung-t7-shield-2tb", Price = 189, OriginalPrice = 219, CategoryId = "cat-accessories", StockQuantity = 55, IsFeatured = false, Description = "Ổ cứng di động SSD chống sốc, tốc độ 1050MB/s", Images = new List<string> { "https://images.unsplash.com/photo-1597848212624-a19eb35e2651?w=500" } },
        };
        db.Products.AddRange(products);
        db.SaveChanges();
    }
}

app.UseForwardedHeaders(new ForwardedHeadersOptions
{
    ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto
});

// Enable CORS for frontend at port 5021
app.UseCors();

app.MapGet("/api/health", () => Results.Ok(new { status = "ok" }));

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
