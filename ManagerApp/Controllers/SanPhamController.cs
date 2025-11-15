using ManagerApp.Models;
using ManagerApp.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ManagerApp.Controllers
{
    public class SanPhamController : Controller
    {
        private readonly SanPhamRepository _sanphamRepo;

        public SanPhamController()
        {
            _sanphamRepo = new SanPhamRepository(new QL_PHONGGYM());
        }
        public ActionResult Index()
        {
            return View();
        }
    }
}