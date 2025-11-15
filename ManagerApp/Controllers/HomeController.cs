using ManagerApp.Repositories;
using ManagerApp.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ManagerApp.Helpers;

namespace ManagerApp.Controllers
{    
    public class HomeController : Controller
    {
        public UserService userService;
        public SanPhamRepository connStr;
        public HomeController()
        {   
            userService = new UserService("OracleDbContext");
        }

        public ActionResult SanPham()
        {
            connStr = new SanPhamRepository(Session["connectionString"] as string);
            var list = connStr.GetAll();
            return View(list);
        }

        [HttpPost]
        public JsonResult CheckSessionAlive()
        {
            var session = Session;

            if (session["user"] == null)
            {
                return Json(new { alive = false });
            }

            string username = session["user"].ToString();

            bool alive = userService.CheckOracleSession(username); 

            if (!alive)
            {
                session.Clear();
                session.Abandon();
            }

            return Json(new { alive = alive });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Logout()
        {
            string username = Session["user"]?.ToString();
            bool result = userService.Logout(username);

            Session.Clear();
            Session.Abandon();

            if (result)
                return RedirectToAction("Login");

            TempData["Error"] = "Lỗi đăng xuất!";
            return RedirectToAction("Index");
        }

        public ActionResult Login()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Login(FormCollection form)
        {
            string username = form["TenDangNhap"];
            string password = form["MatKhau"];

            bool result = userService.Login(username, password);

            if (result)
            {
                Session["user"] = username;
                string hashedPassword = MaHoa.MaHoaNhan(MaHoa.MaHoaNhan(password, 7), 7);
                Session["password"] = hashedPassword;
                Session["loginDate"] = DateTime.Now.ToString("dd/MM/yyyy");
                Session["connectionString"] = userService.ConnectionStringUser;

                return RedirectToAction("Index");
            }
            else
            {
                TempData["Error"] = "Sai tài khoản hoặc mật khẩu!";
                return RedirectToAction("Login");
            }
        }


        [CheckOracleSession]
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }
    }
}