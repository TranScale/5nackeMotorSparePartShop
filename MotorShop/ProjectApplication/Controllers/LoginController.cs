using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Web.Mvc;
using ProjectApplication.Models;

namespace ProjectApplication.Controllers
{
    public class LoginController : Controller
    {
        private ShopDbContext db = new ShopDbContext();

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
                return RedirectToAction("Index", "ProductManager");
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
