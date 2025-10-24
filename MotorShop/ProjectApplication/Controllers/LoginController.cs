using ProjectApplication.Models;
using System;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Web.Mvc;

namespace ProjectApplication.Controllers
{
    public class LoginController : Controller
    {
        private ShopDbContext db = new ShopDbContext();

        public ActionResult Index()
        {
            var orders = db.Orders.ToList();

            // ✅ Tổng doanh thu: chỉ tính đơn KHÔNG phải Admin và đã giao (Delivered)
            decimal totalRevenue = orders
                .Where(o =>
                    o.CustomerName != null &&
                    !o.CustomerName.Equals("Admin", StringComparison.OrdinalIgnoreCase) &&
                    o.Status == "Delivered" &&
                    string.Equals(o.Status, "Delivered", StringComparison.OrdinalIgnoreCase)
                )
                .Sum(o => o.TotalAmount);

            // ✅ Tổng chi phí: các đơn của Admin (Admin nhập hàng)
            decimal totalCost = orders
                .Where(o =>
                    o.CustomerName != null &&
                    o.Status == "Delivered" &&
                    o.CustomerName.Equals("Admin", StringComparison.OrdinalIgnoreCase)
                )
                .Sum(o => o.TotalAmount);

            // ✅ Lợi nhuận = Doanh thu - Chi phí
            decimal profit = totalRevenue - totalCost;

            // ✅ Gửi sang View (giữ số gốc, format bên View cho đẹp)
            ViewBag.TotalRevenue = totalRevenue;
            ViewBag.TotalCost = totalCost;
            ViewBag.Profit = profit;

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
