using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using ProjectApplication.Models;

namespace ProjectApplication.Controllers
{
    public class DiscountManagerController : Controller
    {
        private ShopDbContext db = new ShopDbContext();

        // GET: DiscountManager
        public ActionResult Index(string discountType = "All")
        {
            var discounts = db.Basediscounts.ToList();
            foreach (var d in discounts)
            {
                bool currentStatus = DateTime.Now >= d.dateStart && DateTime.Now <= d.dateEnd;
                if (d.isActive != currentStatus)
                {
                    d.isActive = currentStatus;
                }
            }
            db.SaveChanges(); // ✅ Cập nhật DB luôn


            if (discountType == "Coupon")
            {
                discounts = discounts.Where(d => d.discountType == DiscountType.Coupon).ToList();
            }
            else if (discountType == "Promotion")
            {
                discounts = discounts.Where(d => d.discountType == DiscountType.Promotion).ToList();
            }
            // ✳️ Chuyển List<Basediscount> → List<DiscountViewModel>
            var discountViewModels = discounts
                .Select(d => new DiscountViewModel
                {
                    Id = d.BasediscountId,
                    Name = d.BasediscountName,
                    discountValueType = d.discountValueType,
                    discountType = d.discountType,
                    discountValue = d.discountValue,
                    dateStart = d.dateStart,
                    dateEnd = d.dateEnd,
                    isActive = d.isActive
                })
                .ToList();

            return View(discountViewModels);
        }


        // GET: DiscountManager/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            var discount = db.Basediscounts.Find(id);
            if (discount == null)
            {
                return HttpNotFound();
            }

            DiscountViewModel discountView = new DiscountViewModel
            {
                Id = discount.BasediscountId,
                Name = discount.BasediscountName,
                dateStart = discount.dateStart,
                dateEnd = discount.dateEnd,
                discountType = discount.discountType,
                discountValue = discount.discountValue,
                discountValueType = discount.discountValueType,
                isActive = discount.dateStart <= DateTime.Now && discount.dateEnd >= DateTime.Now
            };

            // Nếu là Coupon
            if (discount is Coupon coupon)
            {
                coupon = db.Coupons.FirstOrDefault(c => c.BasediscountId == id);
                discountView.couponCode = coupon.couponCode;
            }
            // Nếu là Promotion
            else if (discount is Promotion promotion)
            {
                discountView.promotionDescription = promotion.promotionDescription;
            }

            return View(discountView);
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
        public ActionResult Create([Bind(Include = "Id,Name,dateStart,dateEnd,discountType,discountValue,discountValueType,couponCode,promotionDescription,isActive")] DiscountViewModel discountViewModel)
        {
            bool isActive = DateTime.Now >= discountViewModel.dateStart && DateTime.Now <= discountViewModel.dateEnd;

            if (ModelState.IsValid)
            {
                // ✅ Kiểm tra loại discount
                if (discountViewModel.discountType == DiscountType.Coupon)
                {
                    // Tạo đối tượng con là Coupon
                    var coupon = new Coupon
                    {
                        BasediscountName = discountViewModel.Name,
                        dateStart = discountViewModel.dateStart,
                        dateEnd = discountViewModel.dateEnd,
                        discountType = discountViewModel.discountType,
                        discountValue = discountViewModel.discountValue,
                        discountValueType = discountViewModel.discountValueType,
                        couponCode = discountViewModel.couponCode,
                        isActive = isActive
                    };

                    db.Coupons.Add(coupon);
                }
                else if (discountViewModel.discountType == DiscountType.Promotion)
                {
                    // Tạo đối tượng con là Promotion
                    var promotion = new Promotion
                    {
                        BasediscountName = discountViewModel.Name,
                        dateStart = discountViewModel.dateStart,
                        dateEnd = discountViewModel.dateEnd,
                        discountType = discountViewModel.discountType,
                        discountValue = discountViewModel.discountValue,
                        discountValueType = discountViewModel.discountValueType,
                        promotionDescription = discountViewModel.promotionDescription,
                        isActive = isActive
                    };

                    db.Promotions.Add(promotion);
                }

                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(discountViewModel);
        }



        // GET: DiscountManager/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            DiscountViewModel discountViewModel = db.DiscountViewModels.Find(id);
            if (discountViewModel == null)
            {
                return HttpNotFound();
            }
            return View(discountViewModel);
        }

        // POST: DiscountManager/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,Name,dateStart,dateEnd,discountType,discountValue,couponCode,promotionDescription")] DiscountViewModel discountViewModel)
        {
            if (ModelState.IsValid)
            {
                db.Entry(discountViewModel).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(discountViewModel);
        }

        public ActionResult Delete(int id)
        {
            var discount = db.Basediscounts.Find(id);
            DiscountViewModel viewModel;
            if (discount == null)
            {
                return HttpNotFound();
            }
            viewModel = new DiscountViewModel
            {
                Id = discount.BasediscountId,
                Name = discount.BasediscountName,
                dateStart = discount.dateStart,
                dateEnd = discount.dateEnd,
                discountType = discount.discountType,
                discountValue = discount.discountValue,
                discountValueType = discount.discountValueType,
                isActive = discount.dateStart <= DateTime.Now && discount.dateEnd >= DateTime.Now
            };

            if(discount is Coupon coupon)
            {
                viewModel.couponCode = coupon.couponCode;
            }
            if(discount is Promotion promotion)
            {
                viewModel.promotionDescription = promotion.promotionDescription;
            }



            return View(viewModel);
        }


        // POST: DiscountManager/Delete/5
        [HttpPost, ActionName("DeleteConfirmed")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Basediscount basediscount = db.Basediscounts.Find(id);
            db.Basediscounts.Remove(basediscount);

            if(basediscount is Coupon coupon)
            {
                db.Coupons.Remove(coupon);
            }
            if(basediscount is Promotion promotion)
            {
                db.Promotions.Remove(promotion);
                
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
