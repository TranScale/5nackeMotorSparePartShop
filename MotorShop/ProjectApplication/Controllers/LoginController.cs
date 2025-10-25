using ProjectApplication.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Web.Mvc;

namespace ProjectApplication.Controllers
{
    public class LoginController : Controller
    {
        private ShopDbContext db = new ShopDbContext();

        [AdminAuthorize]
        public ActionResult Index(string startDate, string endDate)
        {
            // Lấy toàn bộ đơn hàng
            var orders = db.Orders.ToList();

            // Chuyển đổi ngày sang DateTime, an toàn với TryParse
            DateTime start;
            bool hasStart = DateTime.TryParse(startDate, out start);

            DateTime end;
            bool hasEnd = DateTime.TryParse(endDate, out end);

            // Lọc đơn hàng theo ngày
            var filteredOrders = orders
                .Where(o => (!hasStart || o.OrderDate >= start) &&
                            (!hasEnd || o.OrderDate <= end))
                .ToList();

            // Tính tổng doanh thu (khách hàng không phải Admin & đã giao)
            decimal totalRevenue = filteredOrders
                .Where(o => !string.IsNullOrEmpty(o.CustomerName) &&
                            !o.CustomerName.Equals("Admin", StringComparison.OrdinalIgnoreCase) &&
                            o.Status?.Equals("Delivered", StringComparison.OrdinalIgnoreCase) == true)
                .Sum(o => o.TotalAmount);

            // Tính tổng chi phí (Admin & đã giao)
            decimal totalCost = filteredOrders
                .Where(o => !string.IsNullOrEmpty(o.CustomerName) &&
                            o.CustomerName.Equals("Admin", StringComparison.OrdinalIgnoreCase) &&
                            o.Status?.Equals("Delivered", StringComparison.OrdinalIgnoreCase) == true)
                .Sum(o => o.TotalAmount);

            // Lợi nhuận
            decimal profit = totalRevenue + totalCost;

            // Gửi dữ liệu ra View
            ViewBag.TotalRevenue = totalRevenue;
            ViewBag.TotalCost = totalCost;
            ViewBag.Profit = profit;
            ViewBag.StartDate = startDate;
            ViewBag.EndDate = endDate;

            return View();
        }




        public ActionResult Login()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Login(string username, string password)
        {
            string hash = ComputeSha256Hash(password);
            var admin = db.Admins.FirstOrDefault(a => a.AdminName == username && a.PasswordHash == hash);

            if (admin != null)
            {
                Session["AdminId"] = admin.AdminId;
                Session["AdminName"] = admin.AdminName;
                return RedirectToAction("Index", "Login");
            }

            ViewBag.Error = "Sai username hoặc password";
            return View();
        }

        public ActionResult Logout()
        {
            Session.Clear();
            return RedirectToAction("Login");
        }

        private string ComputeSha256Hash(string rawData)
        {
            using (SHA256 sha256Hash = SHA256.Create())
            {
                byte[] bytes = sha256Hash.ComputeHash(Encoding.UTF8.GetBytes(rawData));
                StringBuilder builder = new StringBuilder();
                foreach (var b in bytes)
                    builder.Append(b.ToString("x2"));
                return builder.ToString();
            }
        }
    }
}
