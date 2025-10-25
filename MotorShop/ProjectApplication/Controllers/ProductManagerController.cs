using ProjectApplication.Models;
using ProjectApplication.Service;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;

namespace ProjectApplication.Controllers
{
    [AdminAuthorize]
    public class ProductManagerController : Controller
    {
        private ShopDbContext db = new ShopDbContext();

        // GET: ProductManager
        public ActionResult Index(string productType, int page = 1, int pageSize = 10)
        {
            var products = ProductManagerService.SearchProductType(productType);
            var viewModel = ProductViewService.GetListIndex(products);

            // Phân trang thủ công
            int totalItems = viewModel.Count();
            var pagedData = viewModel
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToList();

            // Gửi dữ liệu cần thiết sang View
            ViewBag.CurrentPage = page;
            ViewBag.TotalPages = (int)Math.Ceiling((double)totalItems / pageSize);
            ViewBag.ProductType = productType;

            return View(pagedData);
        }


        // GET: ProductManager/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Product product = db.Products.Find(id);
            var viewModel = ProductViewService.GetDetail(product);
            return View(viewModel);
        }

        // GET: ProductManager/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: ProductManager/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "ImageFile,ImagePath,ProductId,BrandId,ProductName,ProductType,ProductPrice,ProductQuantity,ProductDescription,Engine,VehicleType,FuelCapacity,Color")] ProductViewDetail viewModel)
        {
            // ✅ Kiểm tra validation phía server
            if (!ModelState.IsValid)
            {
                // Trả lại form + lỗi
                return View(viewModel);
            }

            // ⚙️ Lưu ảnh
            viewModel.ImagePath = ProductManagerService.saveProductsImage(viewModel.ImageFile);

            int productId;

            // ⚙️ Thêm sản phẩm tùy loại
            if (viewModel.ProductType == "Vehicle")
            {
                var vehicle = ProductManagerService.GetVehicle(viewModel);
                vehicle.Quantity = 0;
                db.Vehicles.Add(vehicle);
                db.SaveChanges();
                productId = vehicle.ProductId;
            }
            else if (viewModel.ProductType == "SparePart")
            {
                var sparePart = ProductManagerService.GetSparePart(viewModel);
                sparePart.Quantity = 0;
                db.SpareParts.Add(sparePart);
                db.SaveChanges();
                productId = sparePart.ProductId;
            }
            else
            {
                var product = ProductManagerService.GetProduct(viewModel);
                product.Quantity = 0;
                db.Products.Add(product);
                db.SaveChanges();
                productId = product.ProductId;
            }

            // ✅ Tạo đơn nhập hàng
            var order = OrderManageService.AdminOrder(viewModel);
            db.Orders.Add(order);
            db.SaveChanges();

            // ✅ Tạo chi tiết đơn hàng
            var orderDetail = OrderManageService.GetOrderDetail(viewModel, order.OrderId, productId);
            db.OrderDetails.Add(orderDetail);
            db.SaveChanges();

            TempData["SuccessMessage"] = "Đã thêm sản phẩm thành công!";
            return RedirectToAction("Index");
        }



        //Cập nhật thêm sản phẩm.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult AddItem(int productId, int quantity)
        {
            try
            {
                ProductManagerService.AddItem(productId, quantity, db);
                TempData["SuccessMessage"] = $"Đã thêm {quantity} sản phẩm và tạo đơn hàng thành công!";
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = $"Lỗi: {ex.Message}";
            }

            return RedirectToAction("Details", new { id = productId });
        }


        // GET: ProductManager/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var product = db.Products.Find(id);
            var viewModel = ProductViewService.GetDetail(product);
            return View(viewModel);
        }

        // POST: ProductManager/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(ProductViewDetail viewModel, HttpPostedFileBase ImageFile)
        {
            if (ModelState.IsValid)
            {
                var product = db.Products.Find(viewModel.ProductId);

                // Xử lý file mới
                if (ImageFile != null && ImageFile.ContentLength > 0)
                {
                    // Lưu file và lấy đường dẫn
                    string imagePath = ProductManagerService.saveProductsImage(ImageFile);
                    viewModel.ImagePath = imagePath;
                }

                if (product is Vehicle vehicle)
                {
                    ProductManagerService.UpdateVehicle(viewModel, vehicle);
                }
                else if (product is SparePart part)
                {
                    ProductManagerService.UpdateSparePart(viewModel, part);
                }

                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(viewModel);
        }


        // GET: ProductManager/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Product product = db.Products.Find(id);
            if (product == null)
            {
                return HttpNotFound();
            }
            return View(product);
        }

        // POST: ProductManager/Delete/5
        [HttpPost, ActionName("DeleteConfirmed")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Product product = db.Products.Find(id);
            if(product is Vehicle vehicle)
            {
                vehicle = db.Vehicles.Find(id);
                db.Vehicles.Remove(vehicle);
            }
            if (product is SparePart spare)
            {
                spare = db.SpareParts.Find(id);
                db.SpareParts.Remove(spare);
            }
            db.SaveChanges();
            return RedirectToAction("Index");
        }



        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
