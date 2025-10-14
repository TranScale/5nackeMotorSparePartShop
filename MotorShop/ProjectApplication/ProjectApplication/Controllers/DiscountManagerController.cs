using ProjectApplication.Models;
using ProjectApplication.Service;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.Validation;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;

namespace ProjectApplication.Controllers
{
    public class DiscountManagerController : Controller
    {
        private ShopDbContext db = new ShopDbContext();

        // GET: DiscountManager
        public ActionResult Index(string discountType)
        {
            var discounts = DiscountManagerService.SearchDiscountTypeList(discountType);
            var viewModel = DiscountViewService.GetListDiscount(discounts);
            return View(viewModel);
        }

        // GET: DiscountManager/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Discount discount = db.Discounts.Find(id);
            var viewModel = DiscountViewService.GetViewDetail(discount);
            return View(viewModel);
        }

        // GET: DiscountManager/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: DiscountManager/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,Name,DateStart,DateEnd,DiscountType,DiscountValue,DiscountValueType,IsActive,CouponCode,PromotionDescription,DiscountCondition")] DiscountViewDetails viewModel)
        {
            if (ModelState.IsValid)
            {
                if(viewModel.DiscountType == "Coupon")
                {
                    var coupon = DiscountManagerService.GetCoupon(viewModel);
                    db.Coupons.Add(coupon);
                }
                else if (viewModel.DiscountType == "Promotion")
                {
                    var promotion = DiscountManagerService.GetPromotion(viewModel);
                    db.Promotions.Add(promotion);
                }
            }


                db.SaveChanges();
           


            return RedirectToAction("Index");
        }

        // GET: DiscountManager/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Discount discount = db.Discounts.Find(id);
            var viewModel = DiscountViewService.GetViewDetail(discount);
            return View(viewModel);
        }

        // POST: DiscountManager/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,Name,DateStart,DateEnd,DiscountType,DiscountValue,DiscountValueType")]DiscountViewDetails viewModel)
        {
            if (ModelState.IsValid)
            {
                var discount = db.Discounts.Find(viewModel.Id);
                if(discount is Coupon coupon)
                {
                    DiscountManagerService.UpdateCouponFromViewModel(coupon, viewModel);
                }
                else if(discount is Promotion promotion)
                {
                    DiscountManagerService.UpdatePromotionFromViewModel(promotion,viewModel);
                }
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(viewModel);
        }

        // GET: DiscountManager/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Discount discount = db.Discounts.Find(id);
            if (discount == null)
            {
                return HttpNotFound();
            }
            return View(discount);
        }

        // POST: DiscountManager/Delete/5
        [HttpPost, ActionName("DeleteConfirmed")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Discount discount = db.Discounts.Find(id);
            if (discount is Promotion promotion)
            {
                promotion = db.Promotions.Find(id);
                db.Promotions.Remove(promotion);
            }
            if (discount is Coupon coupon)
            {
                coupon = db.Coupons.Find(id);
                db.Coupons.Remove(coupon);
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
