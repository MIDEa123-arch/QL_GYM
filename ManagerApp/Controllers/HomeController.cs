using ManagerApp.Helpers;
using ManagerApp.Repositories;
using ManagerApp.ViewModel;
using Oracle.ManagedDataAccess.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

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

            if (session["LoginID"] == null)
            {
                return Json(new { alive = false });
            }

            string loginID = session["LoginID"].ToString();

            bool alive = userService.CheckOracleSession(loginID);

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
            bool result = false;

            try
            {
                result = userService.Login(username, password);
            }
            catch(OracleException ex)
            {
                if (ex.Number == 28000)
                {
                    TempData["Error"] = "Tài khoản của bạn đã bị khóa!";
                }
                else if (ex.Number == 1017)
                {
                    TempData["Error"] = "Sai tài khoản hoặc mật khẩu!";
                }
                else
                {
                    TempData["Error"] = "Lỗi Oracle: " + ex.Message;
                }
                result = false;
            }
            
            if (result)
            {
                string loginID = Guid.NewGuid().ToString();
                userService.RegisterLogin(username, loginID, 2);

                Session["LoginID"] = loginID;
                Session["user"] = username;
                string hashedPassword = MaHoa.MaHoaNhan(MaHoa.MaHoaNhan(password, 7), 7);
                Session["password"] = hashedPassword;
                Session["loginDate"] = DateTime.Now.ToString("dd/MM/yyyy");
                Session["connectionString"] = userService.ConnectionStringUser;

                return RedirectToAction("Index");
            }
            else
            {
                return RedirectToAction("Login");
            }
        }

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