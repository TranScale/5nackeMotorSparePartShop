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
    public class OrderManagerController : Controller
    {
        private ShopDbContext db = new ShopDbContext();

        // GET: OrderManager
        public ActionResult Index()
        {
            var orders = db.Orders.ToList();
            var viewModel = OrderViewService.GetListIndexView(orders);
            return View(viewModel);
        }
        // 🚚 Xác nhận đã giao hàng (Processing → Delivered)
        [HttpPost]
        public ActionResult MarkAsDelivered(int id)
        {
            var order = db.Orders.Find(id);
            if (order == null)
                return HttpNotFound();

            if (order.Status == "Processing")
            {
                order.Status = "Delivered";
                db.SaveChanges();
            }

            return RedirectToAction("Index");
        }

        [HttpPost]
        public ActionResult MarkAsProcessing(int id)
        {
            var order = db.Orders.Find(id);

            if(order == null)
                return HttpNotFound();
            if(order.Status == "Pending")
            {
                order.Status = "Processing";
                db.SaveChanges();
            }
            return RedirectToAction("Index");
        }

        // GET: OrderManager/Details/5
        public ActionResult Details(int? id)
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

        // GET: OrderManager/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: OrderManager/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
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
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
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
