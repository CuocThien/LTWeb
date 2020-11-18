using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using demo.Model;
using demo.Controllers;
using System.Web.UI;
using MailKit.Net.Smtp;
//using System.Net.Mail;
using MimeKit;
using MailKit;
using MimeKit.Text;
using MailKit.Security;

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
        public ActionResult ResetPassword()
        {

            return View();
        }
        public ActionResult ForgotPassword()
        {

            return View();
        }
        public ActionResult ForgotPasswordconfirmation()
        {

            return View();
        }
        [HttpPost]
        public ActionResult Login(User user)

        {
            //Kiểm tra User có tồn tại trong database hay không
            if (user.Username == null || _db.Users.Find(user.Username.Trim()) == null)
                return Content("false");
            else
            {
                //Nếu có thì tiến hành kiểm tra ở Form và ở database có trùng khớp hay không
                var u = _db.Users.Find(user.Username.Trim());
                if (user.Username.Trim() == u.Username.Trim() && user.Password.Trim() == u.Password.Trim())
                {
                    @ViewBag.user = u.Name.Trim();
                    @ViewBag.isSuccess = "1";
                    if(u.isAdmin==true)
                    {
                        @ViewBag.isAdmin = "1";
                    }    
                    return View("Home");
                }
            }

            return Content("false");
        }
        [HttpPost]
        public ActionResult Register(User user)
        {
            var u = _db.Users.Find(user.Username);
            if (u != null)
                return Content("userexist");
            if (Check.CheckUser(user)==true && u==null)
            {     
                _db.Users.Add(user);
                _db.SaveChanges();
                return View("Login");
            }
            
            else
            {
                return Content("false");
            }
        }
        public ActionResult Home()

        {
            return View();
        }
        [HttpPost]
        public ActionResult ResetPassword(User user)
        {//Kiểm tra User có tồn tại trong database hay không
            if (user.Username == null || _db.Users.Find(user.Username.Trim()) == null)
                return Content("false");
            else
            {
                //Nếu có thì tiến hành kiểm tra ở Form và ở database có trùng khớp hay không
                var u = _db.Users.Find(user.Username.Trim());
                if (user.Username.Trim() == u.Username.Trim() && user.Password.Trim() == u.Password.Trim())
                {
                    @ViewBag.user = u.Name.Trim();
                    @ViewBag.isSuccess = "1";
                    if (u.isAdmin == true)
                    {
                        @ViewBag.isAdmin = "1";
                    }
                    return View("Home");
                }
            }
            return View("");
        }
        [HttpPost]
        public ActionResult ForgotPassword(User user)
        {
            if (user.Email != null)
            {
                var x = _db.Users.Where(u => u.Email.Contains(u.Email));
                var MailTo = user.Email.Trim();
                //var u = _db.Users.Find(f["email"]);
                if (x == null)
                    return Content("false");
                else
                {
                    var message = new MimeMessage();
                    message.From.Add(new MailboxAddress("Thien-Vy's Watch Store", "watchstorethienvys@gmail.com"));
                    message.To.Add(new MailboxAddress("Custom", MailTo));
                    message.Subject = "Password recovery";
                    message.Body = new TextPart("plain")
                    {
                        Text = "Link: https://localhost:44305/Shop/ResetPassword"
                    };
                    using (var client = new SmtpClient())
                    {
                        client.Connect("smtp.gmail.com", 587, false);
                        client.Authenticate("watchstorethienvys@gmail.com", "thienvy123");
                        client.Send(message);
                        client.Disconnect(true);
                    }
                    return View("ForgotPasswordconfirmation");
                }
            }
            else
            {
                
                
                return Content("false");
            }
            //return View();
        }

    }
}