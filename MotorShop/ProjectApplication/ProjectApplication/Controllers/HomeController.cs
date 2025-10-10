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

    // Lấy tất cả Tỉnh/Thành
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
    private const string DiscountedItemSessionKey = "HasDiscountedItem"; // Khai báo cờ này ở cấp Class

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

        var today = DateTime.Now.Date;
        decimal priceToUse = 0m;
        bool itemHasDiscount = false;

        // --- LOGIC XỬ LÝ GIÁ VÀ TÊN SẢN PHẨM ---
        string productName = "Sản phẩm";

        if (product is Vehicle vehicle)
        {
            productName = vehicle.vehicleName;
            priceToUse = vehicle.price;

            // 1. Kiểm tra khuyến mãi cho Vehicle
            if (vehicle.DiscountedPrice.HasValue &&
                vehicle.DiscountStartDate.HasValue &&
                vehicle.DiscountEndDate.HasValue &&
                today >= vehicle.DiscountStartDate.Value.Date &&
                today <= vehicle.DiscountEndDate.Value.Date)
            {
                priceToUse = vehicle.DiscountedPrice.Value;
                itemHasDiscount = (vehicle.DiscountedPrice.Value < vehicle.price);
            }
        }
        else if (product is SparePart part)
        {
            productName = part.spareName;
            priceToUse = part.price;

            // 2. Kiểm tra khuyến mãi cho SparePart
            if (part.DiscountedPrice.HasValue &&
                part.DiscountStartDate.HasValue &&
                part.DiscountEndDate.HasValue &&
                today >= part.DiscountStartDate.Value.Date &&
                today <= part.DiscountEndDate.Value.Date)
            {
                priceToUse = part.DiscountedPrice.Value;
                itemHasDiscount = (part.DiscountedPrice.Value < part.price);
            }
        }
        else
        {
            // Xử lý các loại sản phẩm khác nếu có
            priceToUse = product.price;
        }
        // --- KẾT THÚC LOGIC XỬ LÝ GIÁ VÀ TÊN SẢN PHẨM ---

        var cart = GetCart();
        var cartItem = cart.FirstOrDefault(i => i.ProductId == productId);

        // Đặt cờ Session nếu sản phẩm hiện tại có giảm giá
        // Nếu giỏ hàng đã có sản phẩm giảm giá, giữ nguyên cờ là true
        if (itemHasDiscount)
        {
            Session[DiscountedItemSessionKey] = true;
        }

        if (cartItem != null)
        {
            // Cập nhật số lượng (giữ nguyên giá cũ trong giỏ hàng)
            cartItem.Quantity += quantity;
            TempData["SuccessMessage"] = $"Đã thêm {quantity} sản phẩm '{productName}' vào giỏ. Tổng số lượng: {cartItem.Quantity}.";
        }
        else
        {
            cart.Add(new CartItemModel
            {
                ProductId = product.ProductId,
                ProductName = productName,
                // LƯU GIÁ ĐÃ GIẢM HOẶC GIÁ GỐC VÀO GIỎ HÀNG
                Price = priceToUse,
                Quantity = quantity
            });
            TempData["SuccessMessage"] = $"Đã thêm sản phẩm '{productName}' vào giỏ hàng.";
        }

        // Cập nhật Session
        Session["Cart"] = cart;


        if (buyNow)
        {
            return RedirectToAction("Cart");
        }
        else
        {
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
        var today = DateTime.Now.Date;

        var productList = products.Select(p =>
        {
            // Khởi tạo một biến tạm để lưu giá đã giảm (nếu có)
            decimal? currentDiscountedPrice = null;

            // Xử lý logic giảm giá chung cho tất cả các sản phẩm
            if (p.DiscountedPrice.HasValue &&
                p.DiscountStartDate.HasValue &&
                p.DiscountEndDate.HasValue &&
                today >= p.DiscountStartDate.Value.Date &&
                today <= p.DiscountEndDate.Value.Date)
            {
                currentDiscountedPrice = p.DiscountedPrice.Value;
            }

            if (p is Vehicle vehicle)
            {
                return new ProductViewModel
                {
                    Id = vehicle.ProductId,
                    Name = vehicle.vehicleName,
                    // SỬ DỤNG GIÁ ĐÃ GIẢM (nếu có), nếu không có thì dùng giá gốc
                    Price = currentDiscountedPrice ?? vehicle.price,
                    // Cần thêm thuộc tính này vào ProductViewModel để hiển thị giá gốc (tùy chọn)
                    // OriginalPrice = vehicle.price, 
                    // Có giảm giá hay không?
                    HasDiscount = currentDiscountedPrice.HasValue,

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
                    // SỬ DỤNG GIÁ ĐÃ GIẢM (nếu có), nếu không có thì dùng giá gốc
                    Price = currentDiscountedPrice ?? part.price,
                    // OriginalPrice = part.price,
                    // Có giảm giá hay không?
                    HasDiscount = currentDiscountedPrice.HasValue,

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
    //----------------------------------------------------------------
    // ---------------- ACTION PLACE ORDER ----------------
    //----------------------------------------------------------------
    private const string DiscountSessionKey = "DiscountValue"; // Đảm bảo bạn đã khai báo biến này ở cấp Class

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

        // 2. Tính tổng tiền ban đầu & Giảm giá
        decimal cartTotal = cart.Sum(item => item.SubTotal);
        decimal discountValue = 0m;
        if (Session[DiscountSessionKey] != null)
        {
            discountValue = (decimal)Session[DiscountSessionKey];
        }

        decimal finalTotal = cartTotal - discountValue;
        if (finalTotal < 0) finalTotal = 0m;

        using (var db = new ShopDbContext())
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
                TotalAmount = finalTotal, // LƯU TỔNG TIỀN ĐÃ TRỪ GIẢM GIÁ
                Status = "Pending"
            };

            db.Orders.Add(newOrder);
            db.SaveChanges(); // Lưu Order trước để lấy OrderId

            // 4. TẠO CHI TIẾT ĐƠN HÀNG (OrderDetails) VÀ TRỪ KHO HÀNG
            foreach (var item in cart)
            {
                // TÌM SẢN PHẨM GỐC TỪ DB ĐỂ TRỪ KHO
                var product = db.Products.Find(item.ProductId);

                // TẠO CHI TIẾT ĐƠN HÀNG (giữ nguyên)
                var detail = new OrderDetail
                {
                    OrderId = newOrder.OrderId,
                    ProductId = item.ProductId,
                    ProductName = item.ProductName,
                    Quantity = item.Quantity,
                    Price = item.Price
                };
                db.OrderDetails.Add(detail);

                // ---------------- LOGIC TRỪ KHO HÀNG ----------------
                if (product != null)
                {
                    // Sử dụng 'dynamic' để truy cập thuộc tính mà không cần biết kiểu (CÁCH DỄ NHẤT)
                    dynamic productStock = product;

                    // Kiểm tra xem đối tượng có thuộc tính Quantity hay không và trừ kho
                    // Lưu ý: Nếu bạn sử dụng Quantity, hãy đảm bảo nó là số nguyên (int)
                    if (product is Vehicle || product is SparePart)
                    {
                        // Lỗi: Nếu thuộc tính Quantity đã được gán giá trị ở dòng trên
                        productStock.Quantity -= item.Quantity;

                        // Đảm bảo số lượng tồn kho không bị âm sau khi trừ
                        if (productStock.Quantity < 0)
                        {
                            productStock.Quantity = 0;
                        }
                    }

                    // Nếu bạn không thích dùng dynamic, bạn cần dùng if/else if như sau:
                    /*
                    if (product is Vehicle vehicle)
                    {
                        vehicle.Quantity -= item.Quantity;
                        if (vehicle.Quantity < 0) vehicle.Quantity = 0;
                    }
                    else if (product is SparePart part)
                    {
                        part.Quantity -= item.Quantity;
                        if (part.Quantity < 0) part.Quantity = 0;
                    }
                    */
                }
            }
            db.SaveChanges();

            // 5. Xóa Giỏ hàng và Xóa Mã giảm giá sau khi đặt hàng thành công
            Session["Cart"] = null;
            Session[DiscountSessionKey] = null;
        }
        return RedirectToAction("Index");
    }
    //----------------------------------------------------------------
    // ---------------- ACTION APLLY COUPON ----------------
    //----------------------------------------------------------------

    [HttpPost]
    public ActionResult ApplyCoupon(string code)
    {
        // 1. Lấy tổng tiền giỏ hàng hiện tại (Bạn cần một cách để tính tổng tiền ở đây)
        List<CartItemModel> cart = Session["Cart"] as List<CartItemModel>;
        if (cart == null || cart.Count == 0)
        {
            // Trả về lỗi nếu giỏ hàng trống
            return Json(new { success = false, message = "Giỏ hàng trống. Không thể áp dụng mã.", discount = 0m });
        }
        decimal cartTotal = cart.Sum(item => item.SubTotal);

        // 2. Tìm mã giảm giá trong DB
        using (var db = new ShopDbContext())
        {
            var coupon = db.Coupons.FirstOrDefault(c => c.Code == code && c.IsActive);

            if (coupon == null)
            {
                // Xóa giá trị giảm giá cũ nếu mã không hợp lệ
                Session[DiscountSessionKey] = 0m;
                return Json(new { success = false, message = "Mã giảm giá không hợp lệ hoặc đã hết hạn.", discount = 0m });
            }

            decimal discountValue = coupon.Value;

            if (discountValue >= cartTotal)
            {
                // Tránh giảm giá vượt quá tổng tiền
                discountValue = cartTotal;
            }

            // 3. Lưu giá trị giảm giá vào Session
            Session[DiscountSessionKey] = discountValue;

            // 4. Trả về JSON cho JavaScript xử lý
            return Json(new
            {
                success = true,
                message = $"Áp dụng mã thành công! Giảm: {string.Format("{0:N0}", discountValue)} VNĐ",
                discount = discountValue
            });
        }
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