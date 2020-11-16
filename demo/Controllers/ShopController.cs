using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using demo.Model;
using demo.Controllers;
using System.Web.UI;

namespace demo.Controllers
{
    public class ShopController : Controller
    {
        // GET: Shop
        


        public shopEntities _db = new shopEntities();
        [HttpGet]

        public ActionResult Index()
        {
            return View();
        }
        public ActionResult Login()

        {
            return View();
        }
        public ActionResult Register()

        {
            return View();
        }
        public ActionResult AddProduct()

        {
            return View();
        }
        [HttpPost]
        public ActionResult Login(User user)

        {
            //Kiểm tra User có tồn tại trong database hay không
            if (user.Username == null || _db.Users.Find(user.Username.Trim()) == null)
                return View();
            else
            {
                //Nếu có thì tiến hành kiểm tra ở Form và ở database có trùng khớp hay không
                var u = _db.Users.Find(user.Username.Trim());
                if (user.Username.Trim() == u.Username.Trim() && user.Password.Trim() == u.Password.Trim())
                {
                    ViewBag.Message = "Thành Công";
                    @ViewBag.user = u.Name.Trim();
                    @ViewBag.isSuccess = "1";
                    //if(u.isAdmin==true)
                    //{
                    //    ViewBag.isAdmin = "1";
                    //}    
                    return View("Home");
                }
                else
                    ViewBag.Message = "Thất Bại";
            }

            return View();
        }
        [HttpPost]
        public ActionResult Register(User user)
        {

            return View();
        }
        public ActionResult Home()

        {
            return View();
        }
        public ActionResult Validate(string username, string password)

        {
            if(String.IsNullOrEmpty(username))
            {
                ModelState.AddModelError("User name", "Không được để trống");
            }
            return View("Login");
        }
    }
}