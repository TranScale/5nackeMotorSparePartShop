using ProjectApplication.Models;
using ProjectApplication.Data;
using System;
using System.Web;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Web.Hosting;
using System.Web.Mvc;
using System.Globalization;
using System.Web.Script.Serialization;


//----------------------------------------------------------------
// ---------------- Model Địa lý qua JSON ----------------
//----------------------------------------------------------------
public class GeoModel
{
    public string Id { get; set; }
    public string Name { get; set; }
}
public class HomeController : Controller
{
    private static dynamic _geoDataCache;

    //----------------------------------------------------------------
    // ---------------- LOAD FROM JSON ----------------
    //----------------------------------------------------------------
    private dynamic LoadGeoData()
    {
        if (_geoDataCache == null)
        {
            try
            {
                string path = System.Web.HttpContext.Current.Server.MapPath("~/Data/vietnam_data.json");

                if (string.IsNullOrEmpty(path))
                {
                    throw new InvalidOperationException("Server.MapPath không thể xác định đường dẫn thực.");
                }

                if (System.IO.File.Exists(path))
                {
                    string jsonString = System.IO.File.ReadAllText(path, System.Text.Encoding.UTF8);
                    var serializer = new JavaScriptSerializer();
                    _geoDataCache = serializer.Deserialize<dynamic>(jsonString);
                }
                else
                {
                    throw new FileNotFoundException("Không tìm thấy file vietnam_data.json tại: " + path);
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Lỗi tải/phân tích dữ liệu địa lý: {ex.Message}");
                _geoDataCache = new Dictionary<string, object>();
            }
        }
        return _geoDataCache;
    }

    // --- CÁC ACTION AJAX CHO ĐỊA CHỈ  ---

    // 1. Lấy tất cả Tỉnh/Thành
    [HttpGet]
    public ActionResult GetProvinces()
    {
        var data = LoadGeoData();
        var provinces = new List<GeoModel>();

        IDictionary<string, object> dictionaryData = data as IDictionary<string, object>;
        if (dictionaryData == null || dictionaryData.Count == 0)
        {
            return Json(new List<GeoModel> {
            new GeoModel { Id = "Error", Name = "LỖI: Controller không tải được dữ liệu (Kiểm tra Log Debug)" }
        }, JsonRequestBehavior.AllowGet);
        }

        foreach (var item in dictionaryData)
        {
            if (item.Value is IDictionary<string, object> provinceDetail)
            {
                provinces.Add(new GeoModel { Id = item.Key, Name = (string)provinceDetail["name"] });
            }
        }

        return Json(provinces.OrderBy(p => p.Name).ToList(), JsonRequestBehavior.AllowGet);
    }

    private ShopDbContext db = new ShopDbContext();

    //----------------------------------------------------------------
    // ---------------- GET CART ----------------
    //----------------------------------------------------------------
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
    //----------------------------------------------------------------
    // ---------------- ACTION: ADD TO CART ----------------
    //----------------------------------------------------------------
    [HttpPost]
    [ValidateAntiForgeryToken]
    public ActionResult AddToCart(int productId, int quantity = 1, bool buyNow = false)
    {
        var product = db.Products.Find(productId);
        if (product == null)
        {
            TempData["ErrorMessage"] = "Sản phẩm không tồn tại.";
            return RedirectToAction("Index");
        }

        var cart = GetCart();
        var cartItem = cart.FirstOrDefault(i => i.ProductId == productId);

        string productName = (product is Vehicle vehicle) ? vehicle.vehicleName : (product is SparePart part) ? part.spareName : "Sản phẩm";

        if (cartItem != null)
        {
            cartItem.Quantity += quantity;
            TempData["SuccessMessage"] = $"Đã thêm {quantity} sản phẩm '{productName}' vào giỏ. Tổng số lượng: {cartItem.Quantity}.";
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
            TempData["SuccessMessage"] = $"Đã thêm sản phẩm '{productName}' vào giỏ hàng.";
        }

        if (buyNow)
        {
            // MUA NGAY
            return RedirectToAction("Cart");
        }
        else
        {
            // THÊM VÀO GIỎ
            if (Request.UrlReferrer != null)
            {
                return Redirect(Request.UrlReferrer.ToString());
            }
            return RedirectToAction("Index");
        }
    }
    //----------------------------------------------------------------
    // ---------------- ACTION: CART SUMMARY ----------------
    //----------------------------------------------------------------
    public ActionResult CartSummary()
    {
        var cart = GetCart();
        int count = cart.Count;
        ViewBag.CartCount = count;
        return PartialView("_CartSummary");
    }
    //----------------------------------------------------------------
    // ---------------- GET: Home/Index ----------------
    //----------------------------------------------------------------
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
                    CompatibleModel = part.SuitableVehicles
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
    //----------------------------------------------------------------
    // ---------------- GET: Home/Details/? ----------------
    //----------------------------------------------------------------
    public ActionResult Details(int? id)
    {
        if (id == null)
        {
            return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
        }

        var product = db.Products.Find(id);
        if (product == null)
        {
            return HttpNotFound();
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
    //----------------------------------------------------------------
    // ---------------- GET: Home/Cart ----------------
    //----------------------------------------------------------------
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
            cart.Remove(itemToRemove);
            TempData["SuccessMessage"] = $"Đã xóa thành công '{itemToRemove.ProductName}' khỏi giỏ hàng.";
        }
        else
        {
            TempData["ErrorMessage"] = "Không tìm thấy sản phẩm này trong giỏ hàng.";
        }

        return RedirectToAction("Cart");
    }
    //----------------------------------------------------------------
    // ---------------- GET: Home/Checkout - Đặt hàng ----------------
    //----------------------------------------------------------------
    public ActionResult Checkout()
    {
        var cart = GetCart();
        if (cart.Count == 0)
        {
            return RedirectToAction("Cart");
        }
        return View(new Order());
    }
    //----------------------------------------------------------------
    // ---------------- POST: Home/Checkout - Xử lý ----------------
    //----------------------------------------------------------------
    [HttpPost]
    [ValidateAntiForgeryToken]
    public ActionResult Checkout(Order model)
    {
        var cart = GetCart();
        if (cart.Count == 0)
        {
            ModelState.AddModelError("", "Giỏ hàng của bạn đang trống.");
            return View(model);
        }

        if (ModelState.IsValid)
        {
            Session["Cart"] = null;
            return RedirectToAction("OrderComplete");
        }
        return View(model);
    }
    //----------------------------------------------------------------
    // ---------------- ACTION ORDER COMPLETE ----------------
    //----------------------------------------------------------------
    public ActionResult OrderComplete()
    {
        return View();
    }
    [HttpPost]
    [ValidateAntiForgeryToken]
    public ActionResult UpdateCartItem(int productId, int newQuantity)
    {
        if (newQuantity <= 0)
        {
            return RedirectToAction("RemoveFromCart", new { productId = productId });
        }

        var cart = GetCart();
        var cartItem = cart.FirstOrDefault(i => i.ProductId == productId);

        if (cartItem != null)
        {
            cartItem.Quantity = newQuantity;

            TempData["SuccessMessage"] = $"Đã cập nhật số lượng sản phẩm '{cartItem.ProductName}' thành {newQuantity}.";
        }
        else
        {
            TempData["ErrorMessage"] = "Sản phẩm không còn trong giỏ hàng.";
        }

        return RedirectToAction("Cart");
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public ActionResult PlaceOrder(string customerName, string phone, string province, string district, string ward, string addressDetail, string notes)
    {
        // 1. Lấy giỏ hàng hiện tại
        List<CartItemModel> cart = Session["Cart"] as List<CartItemModel>;

        if (cart == null || cart.Count == 0)
        {
            TempData["ErrorMessage"] = "Giỏ hàng trống. Vui lòng thêm sản phẩm trước khi đặt hàng.";
            return RedirectToAction("Cart");
        }

        // 2. Tính tổng tiền
        decimal cartTotal = cart.Sum(item => item.SubTotal);

        using (var db = new ShopDbContext()) // Khởi tạo DbContext
        {
            // 3. TẠO ĐƠN HÀNG (Order)
            var newOrder = new Order
            {
                CustomerName = customerName,
                Phone = phone,
                Province = province,
                District = district,
                Ward = ward,
                AddressDetail = addressDetail,
                Notes = notes,
                OrderDate = DateTime.Now,
                TotalAmount = cartTotal,
                Status = "Pending" // Trạng thái ban đầu
            };

            db.Orders.Add(newOrder);
            db.SaveChanges(); // Lưu Order trước để lấy OrderId

            // 4. TẠO CHI TIẾT ĐƠN HÀNG (OrderDetails)
            foreach (var item in cart)
            {
                var detail = new OrderDetail
                {
                    OrderId = newOrder.OrderId, // Sử dụng OrderId vừa được tạo
                    ProductId = item.ProductId,
                    ProductName = item.ProductName,
                    Quantity = item.Quantity,
                    Price = item.Price
                };
                db.OrderDetails.Add(detail);
            }

            db.SaveChanges(); // Lưu tất cả OrderDetails

            // 5. Xóa Giỏ hàng và Trả về thông báo
            Session["Cart"] = null; // Xóa giỏ hàng sau khi đặt hàng thành công
        }

        // Thiết lập TempData để hiển thị thông báo thành công trên trang Cart
        TempData["SuccessMessage"] = "Đơn hàng của bạn đã được đặt thành công! Chúng tôi sẽ liên hệ lại sớm.";

        return RedirectToAction("Index"); // Chuyển hướng về trang giỏ hàng (hiện tại sẽ hiển thị giỏ hàng trống và thông báo)
    }

    // Phương thức dọn dẹp

    protected override void Dispose(bool disposing)
    {
        if (disposing)
        {
            db.Dispose();
        }
        base.Dispose(disposing);
    }
}