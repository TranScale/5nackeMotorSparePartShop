using ProjectApplication.Models;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web.Mvc;
using System;

public class HomeController : Controller
{
    private ShopDbContext db = new ShopDbContext();

    private List<CartItemModel> GetCart()
    {
        var cart = Session["Cart"] as List<CartItemModel>;
        if (cart == null)
        {
            cart = new List<CartItemModel>();
            Session["Cart"] = cart;
        }
        return cart;
    }

    //---------------------------------------------------------
    // ACTION: Thêm vào giỏ hàng (ĐÃ SỬA THEO YÊU CẦU MỚI)
    //---------------------------------------------------------
    [HttpPost]
    [ValidateAntiForgeryToken]
    public ActionResult AddToCart(int productId, int quantity = 1)
    {
        var product = db.Products.Find(productId);
        if (product == null)
        {
            // Trả về Not Found nếu không tìm thấy sản phẩm
            TempData["ErrorMessage"] = "Sản phẩm không tồn tại.";
            return RedirectToAction("Index");
        }

        var cart = GetCart();
        var cartItem = cart.FirstOrDefault(i => i.ProductId == productId);

        // Xác định tên sản phẩm để dùng trong thông báo
        string productName = (product is Vehicle vehicle) ? vehicle.vehicleName : (product is SparePart part) ? part.spareName : "Sản phẩm";

        if (cartItem != null)
        {
            cartItem.Quantity += quantity;
        }
        else
        {
            cart.Add(new CartItemModel
            {
                ProductId = product.ProductId,
                ProductName = productName,
                Price = product.price,
                Quantity = quantity
            });
        }

        // 1. Lưu thông báo thành công vào TempData
        TempData["SuccessMessage"] = $"Đã thêm thành công '{productName}' vào giỏ hàng!";

        // 2. Chuyển hướng về trang chủ/danh sách sản phẩm
        return RedirectToAction("Index");
    }

    //---------------------------------------------------------
    // ACTION: Tóm tắt giỏ hàng (Dùng cho Partial View trong _Layout)
    //---------------------------------------------------------
    public ActionResult CartSummary()
    {
        var cart = GetCart();
        int count = cart.Count;
        ViewBag.CartCount = count;
        return PartialView("_CartSummary"); // Đã sửa tên View thành _CartSummary
    }

    //---------------------------------------------------------
    // GET: Home/Index - Hiển thị danh sách sản phẩm
    //---------------------------------------------------------
    public ActionResult Index(string searchString, string productType)
    {
        var products = db.Products.ToList();
        var productList = products.Select(p =>
        {
            if (p is Vehicle vehicle)
            {
                return new ProductViewModel
                {
                    Id = vehicle.ProductId,
                    Name = vehicle.vehicleName,
                    Price = vehicle.price,
                    Engine = vehicle.Displacement,
                    ProductType = "Vehicle",
                    description = vehicle.description
                };
            }
            else if (p is SparePart part)
            {
                return new ProductViewModel
                {
                    Id = part.ProductId,
                    Name = part.spareName,
                    Price = part.price,
                    ProductType = "SparePart",
                    description = part.spareDescription,
                    CompatibleModel = part.SuitableVehicles // Giả định CompatibleModel trong VM là SuitableVehicles trong Model
                };
            }
            return null;
        }).Where(vm => vm != null).ToList();

        if (!string.IsNullOrEmpty(productType) && productType != "All")
        {
            productList = productList.Where(p => p.ProductType == productType).ToList();
        }

        if (!string.IsNullOrEmpty(searchString))
        {
            productList = productList.Where(p => p.Name.ToLower().Contains(searchString.ToLower())).ToList();
        }

        ViewBag.ProductType = new SelectList(new List<SelectListItem>
        {
            new SelectListItem { Text = "Tất cả", Value = "All" },
            new SelectListItem { Text = "Xe máy", Value = "Vehicle" },
            new SelectListItem { Text = "Phụ tùng", Value = "SparePart" }
        }, "Value", "Text", productType);

        return View(productList);
    }

    //---------------------------------------------------------
    // GET: Home/Details/5 - Chi tiết sản phẩm
    //---------------------------------------------------------
    public ActionResult Details(int? id) // Thay đổi tham số sang int? để xử lý null
    {
        if (id == null)
        {
            return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
        }

        var product = db.Products.Find(id);
        if (product == null)
        {
            return HttpNotFound(); // không tìm thấy sản phẩm
        }

        ProductViewModel vm;

        if (product is Vehicle vehicle)
        {
            vm = new ProductViewModel
            {
                Id = vehicle.ProductId,
                Name = vehicle.vehicleName,
                Price = vehicle.price,
                number = vehicle.number,
                Engine = vehicle.Displacement,
                fuelCapacity = vehicle.fuelCapacity,
                weight = vehicle.weight,
                Color = vehicle.Color,
                description = vehicle.description,
                ProductType = "Vehicle"
            };
        }
        else if (product is SparePart part)
        {
            vm = new ProductViewModel
            {
                Id = part.ProductId,
                Name = part.spareName,
                Price = part.price,
                number = part.number,
                description = part.spareDescription,
                CompatibleModel = part.SuitableVehicles,
                ProductType = "SparePart"
            };
        }
        else
        {
            return HttpNotFound();
        }

        return View(vm);
    }

    //---------------------------------------------------------
    // GET: Home/Cart - Trang giỏ hàng
    //---------------------------------------------------------
    public ActionResult Cart()
    {
        var cart = GetCart();
        return View(cart);
    }
    public ActionResult RemoveFromCart(int productId)
    {
        var cart = GetCart();

        var itemToRemove = cart.FirstOrDefault(i => i.ProductId == productId);

        if (itemToRemove != null)
        {
            // Xóa sản phẩm khỏi danh sách
            cart.Remove(itemToRemove);
            TempData["SuccessMessage"] = $"Đã xóa thành công '{itemToRemove.ProductName}' khỏi giỏ hàng.";
        }
        else
        {
            TempData["ErrorMessage"] = "Không tìm thấy sản phẩm này trong giỏ hàng.";
        }

        // Chuyển hướng người dùng trở lại trang Giỏ hàng
        return RedirectToAction("Cart");
    }

    //---------------------------------------------------------
    // GET: Home/Checkout - Form thanh toán
    //---------------------------------------------------------
    public ActionResult Checkout()
    {
        var cart = GetCart();
        if (cart.Count == 0)
        {
            return RedirectToAction("Cart");
        }
        return View(new OrderModel());
    }

    //---------------------------------------------------------
    // POST: Home/Checkout - Xử lý đặt hàng
    //---------------------------------------------------------
    [HttpPost]
    [ValidateAntiForgeryToken]
    public ActionResult Checkout(OrderModel model)
    {
        var cart = GetCart();
        if (cart.Count == 0)
        {
            ModelState.AddModelError("", "Giỏ hàng của bạn đang trống.");
            return View(model);
        }

        if (ModelState.IsValid)
        {
            // Logic lưu đơn hàng vào DB
            // ...
            Session["Cart"] = null;
            return RedirectToAction("OrderComplete");
        }
        return View(model);
    }

    //---------------------------------------------------------
    // Action OrderComplete (Cần thiết cho Checkout)
    //---------------------------------------------------------
    public ActionResult OrderComplete()
    {
        return View();
    }

    //---------------------------------------------------------
    // Phương thức dọn dẹp
    //---------------------------------------------------------
    protected override void Dispose(bool disposing)
    {
        if (disposing)
        {
            db.Dispose();
        }
        base.Dispose(disposing);
    }
}