using ProjectApplication.Data;
using ProjectApplication.Models;
using ProjectApplication.Service;
using System;
using System.Collections.Generic;
using System.Drawing.Printing;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Hosting;
using System.Web.Mvc;
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
        decimal priceToUse = product.Price;
        bool itemHasDiscount = false;
        string productName = product.ProductName;

        // ✅ 1. Kiểm tra Promotion đang active
        var activePromotion = db.Promotions
            .FirstOrDefault(p => p.isActive &&
                                 today >= p.DateStart &&
                                 today <= p.DateEnd &&
                                 (p.Condition == product.ProductType || p.Condition == "All"));

        // ✅ 2. Nếu có Promotion, tính lại giá
        if (activePromotion != null)
        {
            itemHasDiscount = true;

            if (activePromotion.DiscountValueType == DiscountValueType.Percent)
            {
                priceToUse = product.Price - (product.Price * activePromotion.DiscountValue / 100);
            }
            else // trực tiếp trừ tiền
            {
                priceToUse = product.Price - activePromotion.DiscountValue;
            }

            if (priceToUse < 0) priceToUse = 0;
        }

        // ✅ 3. Lấy giỏ hàng hiện tại từ Session
        var cart = GetCart();
        var cartItem = cart.FirstOrDefault(i => i.ProductId == productId);

        // ✅ 4. Cập nhật hoặc thêm mới sản phẩm trong giỏ
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
                Price = priceToUse,
                Quantity = quantity
            });
            TempData["SuccessMessage"] = $"Đã thêm sản phẩm '{productName}' vào giỏ hàng.";
        }

        // ✅ 5. Lưu giỏ hàng vào Session
        Session["Cart"] = cart;

        // ✅ 6. Ghi cờ giảm giá (nếu cần dùng cho view Cart)
        Session["HasDiscount"] = itemHasDiscount;

        // ✅ 7. Điều hướng
        if (buyNow)
        {
            return RedirectToAction("Cart");
        }
        else
        {
            if (Request.UrlReferrer != null)
                return Redirect(Request.UrlReferrer.ToString());
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
    public ActionResult Index(string searchString, string productType, string sortBy, string priceRange)
    {
        var today = DateTime.Now.Date;

        // Lấy tất cả Promotion đang active
        var activePromotions = db.Promotions
            .Where(p => p.isActive &&
                        today >= p.DateStart &&
                        today <= p.DateEnd)
            .ToList();

        // ⭐ LẤY SẢN PHẨM ĐÃ LỌC KẾT HỢP TỪ SERVICE ⭐
        var products = ProductManagerService.SearchProduct(productType, searchString);
        if (!string.IsNullOrEmpty(priceRange))
        {
            // Dưới 20tr
            if (priceRange == "30M")
            {
                products = products.Where(p => p.Price < 30000000).ToList();
            }
            // Từ 30tr đến 40tr
            else if (priceRange == "40M")
            {
                products = products.Where(p => p.Price >= 30000000 && p.Price <= 40000000).ToList();
            }
            // >40: Trên 40tr
            else if (priceRange == "40M_UP")
            {
                products = products.Where(p => p.Price > 40000000).ToList();
            }
        }

        // Tính giá sau giảm (Ánh xạ sang ViewModel)
        var productList = products.Select(p =>
        {
            decimal finalPrice = p.Price;
            bool hasDiscount = false;
            decimal? originalPrice = null;
            string typeProduct = p.ProductType;

            var applicablePromotions = activePromotions
                .Where(promo => promo.Condition == typeProduct || promo.Condition == "All")
                .ToList();

            if (applicablePromotions.Any())
            {
                hasDiscount = true;
                originalPrice = p.Price;

                var bestPromotion = applicablePromotions
                    .OrderByDescending(promo => promo.DiscountValue)
                    .First();

                if (bestPromotion.DiscountValueType == DiscountValueType.Percent)
                {
                    finalPrice = p.Price - (p.Price * bestPromotion.DiscountValue / 100);
                }
                else
                {
                    finalPrice = p.Price - bestPromotion.DiscountValue;
                }

                if (finalPrice < 0) finalPrice = 0;
            }

            return new ProductViewIndex
            {
                ImagePath = p.ImagePath,
                ProductId = p.ProductId,
                ProductName = p.ProductName,
                ProductPrice = finalPrice, // Giá đã tính khuyến mãi
                ProductQuantity = p.Quantity,
                OriginalPrice = originalPrice,
                HasDiscount = hasDiscount
            };
        }).ToList();

        // LỌC THEO GIÁ (SORTING)
        switch (sortBy)
        {
            case "price_desc":
                // Sắp xếp từ cao đến thấp
                productList = productList.OrderByDescending(p => p.ProductPrice).ToList();
                break;
            case "price_asc":
                // Sắp xếp từ thấp đến cao
                productList = productList.OrderBy(p => p.ProductPrice).ToList();
                break;
            default:
                break;
        }

        // Dropdown lọc loại sản phẩm
        ViewBag.ProductType = new SelectList(new List<SelectListItem>
    {
        new SelectListItem { Text = "Tất cả", Value = "All" },
        new SelectListItem { Text = "Xe máy", Value = "Vehicle" },
        new SelectListItem { Text = "Phụ tùng", Value = "SparePart" }
    }, "Value", "Text", productType);

        // Lưu chuỗi tìm kiếm và trạng thái sắp xếp
        ViewBag.SearchString = searchString;
        ViewBag.CurrentSort = sortBy;

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

        ProductViewDetail vm = ProductViewService.GetDetail(product);

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
        TempData["SuccessMessage"] = "🎉 Đơn hàng của bạn đã được đặt thành công! Cảm ơn bạn đã mua sắm tại cửa hàng.";

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
                db.SaveChanges();
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
            var coupon = db.Coupons.FirstOrDefault(c => c.CouponCode == code && c.isActive);

            if (coupon == null)
            {
                // Xóa giá trị giảm giá cũ nếu mã không hợp lệ
                Session[DiscountSessionKey] = 0m;
                return Json(new { success = false, message = "Mã giảm giá không hợp lệ hoặc đã hết hạn.", discount = 0m });
            }

            decimal discountValue = coupon.DiscountValue;
            if (discountValue >= cartTotal)
            {
                // Tránh giảm giá vượt quá tổng tiền
                discountValue = cartTotal;
            }
            if (coupon.DiscountValueType == DiscountValueType.Percent)
            {
                discountValue = (cartTotal * coupon.DiscountValue) / 100m;
                cartTotal -= discountValue;
            }
            else
            {
                cartTotal = cartTotal - coupon.DiscountValue;
            }


            // 3. Lưu giá trị giảm giá vào Session
            Session[DiscountSessionKey] = discountValue;

            // 4. Trả về JSON cho JavaScript xử lý
            return Json(new
            {
                success = true,
                message = $"Áp dụng mã thành công! Giảm: {string.Format("{0:N0}", discountValue)} VNĐ",
                discount = discountValue,
                newTotal = cartTotal
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