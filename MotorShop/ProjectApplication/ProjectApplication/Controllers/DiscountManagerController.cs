using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.Migrations;
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
            DiscountViewModel discountView = GetViewModel(discount);

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
            DiscountViewModel viewModel;
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var discount = db.Basediscounts.Find(id);
            if(discount is Coupon coupon)
            {
                viewModel = GetViewModel(coupon);
            }
            else if(discount is Promotion promotion)
            {
                viewModel = GetViewModel(promotion);
            }
            else
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);

            return View(viewModel);
        }

        // POST: DiscountManager/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(DiscountViewModel viewModel)
        {
            if (ModelState.IsValid)
            {
                Basediscount discount = db.Basediscounts.Find(viewModel.Id);

                if (discount == null)
                    return HttpNotFound();

                if (discount is Coupon && viewModel.discountType == DiscountType.Coupon || discount is Promotion && viewModel.discountType == DiscountType.Promotion)
                {
                    discount.BasediscountName = viewModel.Name;
                    discount.dateStart = viewModel.dateStart;
                    discount.dateEnd = viewModel.dateEnd;
                    discount.discountValue = viewModel.discountValue;

                    if (discount is Coupon coupon)
                        coupon.couponCode = viewModel.couponCode;
                    else if (discount is Promotion promotion)
                        promotion.promotionDescription = viewModel.promotionDescription;
                }
                else
                {
                    // Loại đổi → xóa entity cũ
                    if (discount is Coupon oldCoupon)
                        db.Coupons.Remove(oldCoupon);
                    else if (discount is Promotion oldPromotion)
                        db.Promotions.Remove(oldPromotion);

                    // Tạo entity mới
                    Basediscount newDiscount;
                    if (viewModel.discountType == DiscountType.Coupon)
                    {
                        newDiscount = new Coupon
                        {
                            BasediscountName = viewModel.Name,
                            dateStart = viewModel.dateStart,
                            dateEnd = viewModel.dateEnd,
                            discountValue = viewModel.discountValue,
                            couponCode = viewModel.couponCode
                        };
                    }
                    else // Promotion
                    {
                        newDiscount = new Promotion
                        {
                            BasediscountName = viewModel.Name,
                            dateStart = viewModel.dateStart,
                            dateEnd = viewModel.dateEnd,
                            discountValue = viewModel.discountValue,
                            promotionDescription = viewModel.promotionDescription
                        };
                    }

                    db.Basediscounts.Add(newDiscount);
                }

                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(viewModel);
        }


        public ActionResult Delete(int id)
        {
            var discount = db.Basediscounts.Find(id);
            DiscountViewModel viewModel;
            if (discount == null)
            {
                return HttpNotFound();
            }
            viewModel = GetViewModel(discount);



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

        public DiscountViewModel GetViewModel(Basediscount discount)
        {
            DiscountViewModel viewModel = new DiscountViewModel()
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

            if (discount is Coupon coupon)
            {
                viewModel.couponCode = coupon.couponCode;
            }
            if (discount is Promotion promotion)
            {
                viewModel.promotionDescription = promotion.promotionDescription;
            }

            return viewModel;
        }
    }
}
