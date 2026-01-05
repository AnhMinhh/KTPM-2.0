using Blazored.LocalStorage;
using NeonGadgetStore.Web.Models;

namespace NeonGadgetStore.Web.Services;

public class CartService
{
    private readonly ILocalStorageService _localStorage;
    private const string CartKey = "shopping-cart";
    
    private List<CartItem> _items = new();
    public IReadOnlyList<CartItem> Items => _items.AsReadOnly();
    
    public int TotalItems => _items.Sum(x => x.Quantity);
    public decimal TotalPrice => _items.Sum(x => x.Product.Price * x.Quantity);
    
    public bool IsCartOpen { get; set; }

    public event Action? OnChange;

    public CartService(ILocalStorageService localStorage)
    {
        _localStorage = localStorage;
    }

    public async Task InitializeAsync()
    {
        try
        {
            var items = await _localStorage.GetItemAsync<List<CartItem>>(CartKey);
            _items = items ?? new List<CartItem>();
        }
        catch
        {
            _items = new List<CartItem>();
        }
        NotifyStateChanged();
    }

    public async Task AddToCartAsync(Product product)
    {
        var existingItem = _items.FirstOrDefault(x => x.Product.Id == product.Id);
        if (existingItem != null)
        {
            existingItem.Quantity++;
        }
        else
        {
            _items.Add(new CartItem { Product = product, Quantity = 1 });
        }
        
        IsCartOpen = true;
        await SaveAsync();
        NotifyStateChanged();
    }

    public async Task RemoveFromCartAsync(string productId)
    {
        var item = _items.FirstOrDefault(x => x.Product.Id == productId);
        if (item != null)
        {
            _items.Remove(item);
            await SaveAsync();
            NotifyStateChanged();
        }
    }

    public async Task UpdateQuantityAsync(string productId, int quantity)
    {
        if (quantity <= 0)
        {
            await RemoveFromCartAsync(productId);
            return;
        }

        var item = _items.FirstOrDefault(x => x.Product.Id == productId);
        if (item != null)
        {
            item.Quantity = quantity;
            await SaveAsync();
            NotifyStateChanged();
        }
    }

    public async Task ClearCartAsync()
    {
        _items.Clear();
        await _localStorage.RemoveItemAsync(CartKey);
        NotifyStateChanged();
    }

    public void SetCartOpen(bool open)
    {
        IsCartOpen = open;
        NotifyStateChanged();
    }

    private async Task SaveAsync()
    {
        await _localStorage.SetItemAsync(CartKey, _items);
    }

    private void NotifyStateChanged() => OnChange?.Invoke();
}
