using ProjectApplication.Models;
using System;
using System.Web;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
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
    // ---------------- LOAD FROM JSON (Đã sửa lỗi) ----------------
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
    // 2. Lấy Quận/Huyện dựa trên Tỉnh/Thành (parentId)
    [HttpGet]
    public ActionResult GetDistricts(string parentId)
    {
        var data = LoadGeoData();
        var districts = new List<GeoModel>();

        IDictionary<string, object> dictionaryData = data as IDictionary<string, object>;

        if (dictionaryData != null && dictionaryData.ContainsKey(parentId))
        {
            if (dictionaryData[parentId] is IDictionary<string, object> province)
            {
                if (province.ContainsKey("districts") && province["districts"] is IDictionary<string, object> districtsData)
                {
                    foreach (var item in districtsData)
                    {
                        if (item.Value is IDictionary<string, object> districtDetail)
                        {
                            districts.Add(new GeoModel { Id = item.Key, Name = (string)districtDetail["name"] });
                        }
                    }
                }
            }
        }

        return Json(districts.OrderBy(d => d.Name).ToList(), JsonRequestBehavior.AllowGet);
    }

    // 3. Lấy Phường/Xã dựa trên Quận/Huyện (parentId)
    [HttpGet]
    public ActionResult GetWards(string parentId)
    {
        var data = LoadGeoData();
        var wards = new List<GeoModel>();

        if (data is IDictionary<string, object> dictionaryData)
        {
            // Phải duyệt qua tất cả tỉnh để tìm quận/huyện tương ứng
            foreach (var provinceEntry in dictionaryData.Values)
            {
                if (provinceEntry is IDictionary<string, object> province && province.ContainsKey("districts"))
                {
                    if (province["districts"] is IDictionary<string, object> districts)
                    {
                        if (districts.ContainsKey(parentId))
                        {
                            if (districts[parentId] is IDictionary<string, object> district)
                            {
                                if (district.ContainsKey("wards") && district["wards"] is IDictionary<string, object> wardsData)
                                {
                                    // Wards là Dictionary<string, string>, key=Mã, value=Tên
                                    foreach (var item in wardsData)
                                    {
                                        wards.Add(new GeoModel { Id = item.Key, Name = (string)item.Value });
                                    }
                                    return Json(wards.OrderBy(w => w.Name).ToList(), JsonRequestBehavior.AllowGet); // Đã tìm thấy, thoát ngay
                                }
                            }
                        }
                    }
                }
            }
        }
        return Json(wards, JsonRequestBehavior.AllowGet);
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
        return View(new OrderModel());
    }
    //----------------------------------------------------------------
    // ---------------- POST: Home/Checkout - Xử lý ----------------
    //----------------------------------------------------------------
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