using ProjectApplication.Models;
using ProjectApplication.Service;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using System.Web.UI;

namespace ProjectApplication.Controllers
{
    [AdminAuthorize]
    public class OrderManagerController : Controller
    {
        private ShopDbContext db = new ShopDbContext();

        // GET: OrderManager
        public ActionResult Index(DateTime? startDate, DateTime? endDate, int page = 1, int pageSize = 10)
        {
            // Bắt đầu query dữ liệu
            var query = db.Orders.AsQueryable();

            // 🔍 Lọc theo thời gian
            if (startDate.HasValue)
            {
                query = query.Where(o => o.OrderDate >= startDate.Value);
            }
            if (endDate.HasValue)
            {
                // Lấy đến hết ngày cuối cùng (23:59:59)
                var end = endDate.Value.AddDays(1).AddTicks(-1);
                query = query.Where(o => o.OrderDate <= end);
            }

            // Sắp xếp mới nhất trước
            query = query.OrderByDescending(o => o.OrderDate);

            // Tổng số đơn hàng
            int totalOrders = query.Count();

            // Tính tổng số trang
            int totalPages = (int)Math.Ceiling((double)totalOrders / pageSize);

            // Giới hạn dữ liệu hiển thị theo trang
            var orders = query.Skip((page - 1) * pageSize).Take(pageSize).ToList();

            // Convert sang ViewModel
            var viewModel = OrderViewService.GetListIndexView(orders);

            // Truyền dữ liệu sang View
            ViewBag.Page = page;
            ViewBag.TotalPages = totalPages;
            ViewBag.StartDate = startDate?.ToString("yyyy-MM-dd");
            ViewBag.EndDate = endDate?.ToString("yyyy-MM-dd");

            return View(viewModel);
        }


        // 🚚 Xác nhận đã giao hàng (Processing → Delivered)
        [HttpPost]
        public ActionResult MarkAsDelivered(int id, int page = 1)
        {
            var order = db.Orders.Find(id);
            if (order == null)
                return HttpNotFound();

            if (order.Status == "Processing")
            {
                order.Status = "Delivered";
                ProductManagerService.Restock(id, db);
                db.SaveChanges();
            }

            return RedirectToAction("Index", new { page });
        }
        // 🚚 Xác nhận đơn hàng (Pending → Processing)
        [HttpPost]
        public ActionResult MarkAsProcessing(int id, int page = 1)
        {
            var order = db.Orders.Find(id);

            if(order == null)
                return HttpNotFound();
            if(order.Status == "Pending")
            {
                order.Status = "Processing";
                db.SaveChanges();
            }
            return RedirectToAction("Index", new { page });
        }


        [HttpPost]
        public ActionResult CancelOrder(int id, int page = 1)
        {
            // ✅ Phải Include để load đầy đủ OrderDetails
            var order = db.Orders
                        .Include(o => o.OrderDetails)
                        .FirstOrDefault(o => o.OrderId == id);

            if (order == null)
                return HttpNotFound();

            if (order.Status == "Pending")
            {
                foreach (var detail in order.OrderDetails)
                {
                    var product = db.Products.Find(detail.ProductId);
                    if (product != null)
                    {
                        product.Quantity += detail.Quantity;
                        db.Entry(product).State = EntityState.Modified;
                    }
                }
            }

            order.Status = "Cancelled";
            db.Entry(order).State = EntityState.Modified;
            db.SaveChanges();

            TempData["Message"] = $"Đơn hàng #{order.OrderId} đã được hủy và hàng đã cộng lại kho.";
            return RedirectToAction("Index", new { page });
        }



    // GET: OrderManager/Details/5
    public ActionResult Details(int? id)
        {
            if (id == null)
                return HttpNotFound();

            var order = db.Orders.Include("OrderDetails").FirstOrDefault(o => o.OrderId == id);

            if(order == null) 
                return HttpNotFound();

            var viewModel = OrderViewService.GetDetails(order);

            return View(viewModel);
        }

        // GET: OrderManager/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: OrderManager/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "OrderId,CustomerName,Phone,Province,District,Ward,AddressDetail,Notes,OrderDate,TotalAmount,Status")] Order order)
        {
            if (ModelState.IsValid)
            {
                db.Orders.Add(order);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(order);
        }

        // GET: OrderManager/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Order order = db.Orders.Find(id);
            if (order == null)
            {
                return HttpNotFound();
            }
            return View(order);
        }

        // POST: OrderManager/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "OrderId,CustomerName,Phone,Province,District,Ward,AddressDetail,Notes,OrderDate,TotalAmount,Status")] Order order)
        {
            if (ModelState.IsValid)
            {
                db.Entry(order).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(order);
        }

        // GET: OrderManager/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Order order = db.Orders.Find(id);
            if (order == null)
            {
                return HttpNotFound();
            }
            return View(order);
        }

        // POST: OrderManager/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Order order = db.Orders.Find(id);
            db.Orders.Remove(order);
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
