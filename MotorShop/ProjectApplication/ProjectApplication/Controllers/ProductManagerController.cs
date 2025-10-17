using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using ProjectApplication.Models;
using ProjectApplication.Service;

namespace ProjectApplication.Controllers
{
    public class ProductManagerController : Controller
    {
        private ShopDbContext db = new ShopDbContext();

        // GET: ProductManager
        public ActionResult Index(string productType)
        {
            var products = ProductManagerService.SearchProductType(productType);
            var viewModel = ProductViewService.GetListIndex(products);
            return View(viewModel);
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
        public ActionResult Create([Bind(Include = "ProductId,BrandId,ProductName,ProductType,ProductPrice,ProductQuantity,ProductDescription,Engine,VehicleType,FuelCapacity,Color")] ProductViewDetail viewModel)
        {
            int productId; // Giữ ProductId sau khi thêm

            if (viewModel.ProductType == "Vehicle")
            {
                var vehicle = ProductManagerService.GetVehicle(viewModel);
                vehicle.Quantity = 0;
                db.Vehicles.Add(vehicle);
                db.SaveChanges(); // ⚡ Lưu trước để có ProductId
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

            // ✅ 2. Tạo Order "Admin nhập hàng"
            var order = OrderManageService.AdminOrder(viewModel);

            db.Orders.Add(order);
            db.SaveChanges();

            // ✅ 3. Tạo OrderDetail liên kết đúng ProductId
            var orderDetail = new OrderDetail();
            orderDetail = OrderManageService.GetOrderDetail(viewModel, order.OrderId, productId);

            db.OrderDetails.Add(orderDetail);
            db.SaveChanges();

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
        public ActionResult Edit([Bind(Include = "ProductId,BrandId,ProductName,ProductType,Price,Quantity,ProductDescription")] ProductViewDetail viewModel)
        {
            if (ModelState.IsValid)
            {
                var product = db.Products.Find(viewModel.ProductId);
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
