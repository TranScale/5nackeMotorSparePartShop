using ProjectApplication.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Data.Entity;

namespace ProjectApplication.Controllers
{
    public class FeedbackManagerController : Controller
    {
        private ShopDbContext db = new ShopDbContext();

        // Danh sách feedback chưa duyệt
        public ActionResult Index()
        {
            var feedbacks = db.Feedbacks
                              .Include(f => f.Product)
                              .OrderByDescending(f => f.CreatedDate)
                              .Select(f => new FeedbackViewModel
                              {
                                  FeedbackId = f.FeedbackId,
                                  ProductName = f.Product.ProductName,
                                  CustomerName = f.CustomerName,
                                  Rating = f.Rating,
                                  Comment = f.Comment,
                                  CreatedDate = f.CreatedDate,
                                  IsApproved = f.IsApproved
                              })
                              .ToList();

            return View(feedbacks);
        }


        // Duyệt feedback
        public ActionResult Approve(int id)
        {
            var fb = db.Feedbacks.Find(id);
            if (fb != null)
            {
                fb.IsApproved = true;
                db.SaveChanges();
            }
            return RedirectToAction("Index");
        }

        // Xóa feedback
        public ActionResult Delete(int id)
        {
            var fb = db.Feedbacks.Find(id);
            if (fb != null)
            {
                db.Feedbacks.Remove(fb);
                db.SaveChanges();
            }
            return RedirectToAction("Index");
        }
    }

}