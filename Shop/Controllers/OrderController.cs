using Microsoft.AspNetCore.Mvc;
using Shop.Models;

public class OrderController : Controller
{
    [HttpGet]
    public IActionResult Checkout()
    {
        return View();
    }

    [HttpPost]
    public IActionResult Checkout(Order order)
    {
        var cart = HttpContext.Session.GetObject<List<CartItem>>("cart") ?? new List<CartItem>();
        if (!cart.Any()) return RedirectToAction("Index", "Cart");

        // Tính tổng tiền
        order.TotalAmount = cart.Sum(i => i.Price * i.Quantity);
        order.OrderDate = DateTime.Now;
        order.OrderDetails = cart.Select(item => new OrderDetail
        {
            ProductId = item.ProductId,
            ProductName = item.ProductName,
            Price = item.Price,
            Quantity = item.Quantity
        }).ToList();

        // Lưu vào database (nếu có DbContext thì thêm SaveChanges)
        // dbContext.Orders.Add(order);
        // dbContext.SaveChanges();

        // Xóa giỏ hàng
        HttpContext.Session.Remove("cart");

        TempData["Success"] = "Đặt hàng thành công!";
        return RedirectToAction("Success");
    }

    public IActionResult Success()
    {
        return View();
    }
}
