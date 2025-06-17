using Microsoft.AspNetCore.Mvc;
using Shop.Models;
using Shop.Repositories;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

public class CartController : Controller
{
    private readonly IProductRepository _productRepo;
    private const string CartSessionKey = "cart"; // 🔑 Khai báo hằng số session key

    public CartController(IProductRepository productRepo)
    {
        _productRepo = productRepo;
    }

    private List<CartItem> GetCart()
    {
        var cart = HttpContext.Session.GetObject<List<CartItem>>(CartSessionKey);
        if (cart == null)
        {
            cart = new List<CartItem>();
            HttpContext.Session.SetObject(CartSessionKey, cart);
        }
        return cart;
    }

    [HttpGet]
    public IActionResult Index()
    {
        var cart = GetCart();
        return View(cart);
    }

    [HttpPost]
    public async Task<IActionResult> AddToCartAsync(int productId)
    {
        var product = await _productRepo.GetByIdAsync(productId);

        if (product == null) return NotFound();

        var cart = GetCart();
        var item = cart.FirstOrDefault(p => p.ProductId == productId);

        if (item == null)
        {
            cart.Add(new CartItem
            {
                ProductId = product.Id,
                ProductName = product.Name,
                Price = product.Price,
                Quantity = 1,
                ImageUrl = product.ImageUrl
            });
        }
        else
        {
            item.Quantity++;
        }

        HttpContext.Session.SetObject(CartSessionKey, cart);
        return RedirectToAction("Index");
    }

    [HttpPost]
    public IActionResult UpdateQuantity(int productId, string actionType)
    {
        var cart = HttpContext.Session.GetObject<List<CartItem>>(CartSessionKey) ?? new List<CartItem>();
        var item = cart.FirstOrDefault(x => x.ProductId == productId);

        if (item != null)
        {
            if (actionType == "increase")
            {
                item.Quantity++;
            }
            else if (actionType == "decrease" && item.Quantity > 1)
            {
                item.Quantity--;
            }
        }

        HttpContext.Session.SetObject(CartSessionKey, cart);
        return RedirectToAction("Index");
    }

    public IActionResult Remove(int productId)
    {
        var cart = GetCart();
        var item = cart.FirstOrDefault(p => p.ProductId == productId);
        if (item != null)
        {
            cart.Remove(item);
            HttpContext.Session.SetObject(CartSessionKey, cart);
        }
        return RedirectToAction("Index");
    }

    public IActionResult Clear()
    {
        HttpContext.Session.Remove(CartSessionKey);
        return RedirectToAction("Index");
    }
}
