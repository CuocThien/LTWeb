using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using demo.Model;
using demo.Controllers;
using System.Web.UI;
using MailKit.Net.Smtp;
using PagedList;
using MimeKit;
using MailKit;
using MimeKit.Text;
using MailKit.Security;
using System.Data.Entity.Migrations;
using Facebook;
using System.Configuration;
using System.Web.Script.Serialization;

namespace demo.Controllers
{
    public class ShopController : Controller
    {

        private Uri RedirectUri
        {
            get
            {
                var uriBuilder = new UriBuilder(Request.Url);
                uriBuilder.Query = null;
                uriBuilder.Fragment = null;
                uriBuilder.Path = Url.Action("FacebookCallback");
                return uriBuilder.Uri;
            }
        }
        // GET: Shop



        public shopEntities _db = new shopEntities();
        private const string CartSession = "CartSession";
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
        
        //Xu ly gio hang
        public ActionResult Cart(string ProductID, string quantity)

        {
            int i = 0;
            var p = _db.Orders.ToList();
            User user = Session["User"] as User;
            foreach (var item in p)
            {
                if (i < int.Parse(item.ID_Order))
                {
                    i = int.Parse(item.ID_Order);
                }
            }
            i++;
           // Session[CartSession] = null;
            var ID = _db.Orders.Where(o => o.ID_Customer == user.Username && o.status == false).SingleOrDefault();
            List< OrderDetail> pro = new List<OrderDetail>();
            if (!(ID is null))
            {
                pro = _db.OrderDetails.Where(x => x.ID_Order == ID.ID_Order).ToList();
                Session[CartSession] = pro;

            }
            int Quantity = Convert.ToInt32(quantity);
            var product = _db.Products.Find(ProductID);
            var cart = Session[CartSession];
           
            //Kiem tra gio hang co san pham hay khong
            if (Quantity!=0 && !(ID is null))
            {
               
                var id = _db.OrderDetails.Where(x => x.ID_Order == ID.ID_Order && x.ID_Product==ProductID).SingleOrDefault();
              
                //Kiem tra gio hang co san pham nay hay chua
                var list = (List<OrderDetail>)cart;
                if (list.Exists(x => x.Product.ID == ProductID))
                {
                    foreach (var item in list)
                    {
                        if (item.Product.ID == ProductID)
                        {
                            item.Quantity += Quantity;
                            id.Quantity = item.Quantity;
                            id.Price = id.Quantity * item.Product.Price;
                            _db.OrderDetails.AddOrUpdate(id);
                        }
                    }
                    
                    _db.SaveChanges();

                }
                //ad san pham moi neu san pham chua co
                else
                {
                    OrderDetail orderDetail = new OrderDetail();
                    var order = new OrderDetail();
                    order.Product = product;
                    order.Quantity = Quantity;
                   // list.Add(order);
                    orderDetail.ID_Order = ID.ID_Order;
                    orderDetail.ID_Product = order.Product.ID;
                    orderDetail.Quantity = order.Quantity;
                    orderDetail.Price = order.Quantity * order.Product.Price;
                    _db.OrderDetails.Add(orderDetail);
                    _db.SaveChanges();
                    list.Add(orderDetail);
                }

                Session[CartSession] = list;

            }
            //Chua co san pham nao trong gio hang
            else if(Quantity!=0)
            {
                Order orders = new Order();
                var order = new OrderDetail();
                OrderDetail orderDetail = new OrderDetail();
                order.Product = product;
                order.Quantity = Quantity;
                var list = new List<OrderDetail>();
                //list.Add(order);

                //Luu xuong bang order
                orders.ID_Order = i.ToString();
                orders.ID_Customer = user.Username.ToString();
                orders.status = false;
                _db.Orders.Add(orders);
                //Luu xuong bang orderdetail
                orderDetail.ID_Order = orders.ID_Order;
                orderDetail.ID_Product = order.Product.ID;
                orderDetail.Quantity = order.Quantity;
                orderDetail.Price = order.Quantity * order.Product.Price;
                _db.OrderDetails.Add(orderDetail);
                _db.SaveChanges();
                list.Add(orderDetail);
                Session[CartSession] = list;


            }

            //Hien len view
          
            if(Quantity==0)
            {
                Session[CartSession] = pro;
            }
            cart = Session[CartSession];
            var l = new List<OrderDetail>();
                l = (List<OrderDetail>)cart;
            return View(l);
            //return RedirectToAction("ViewCart");
        }
        public JsonResult Delete(long id)
        {
            string Id = id.ToString();
            User user = Session["User"] as User;
            var ID = _db.Orders.Where(o => o.ID_Customer == user.Username && o.status == false).SingleOrDefault();
            var pro = _db.OrderDetails.Where(x => x.ID_Order == ID.ID_Order && x.ID_Product == Id).SingleOrDefault();
            _db.OrderDetails.Remove(pro);
            _db.SaveChanges();
            var sessionCart = _db.OrderDetails.Where(x => x.ID_Order == ID.ID_Order).ToList();
            Session[CartSession] = sessionCart;
            return Json(new
            {
                status = true
            });
        }
        public JsonResult DeleteAll()
        {
            User user = Session["User"] as User;
            var ID = _db.Orders.Where(o => o.ID_Customer == user.Username && o.status == false).SingleOrDefault();
            var pro = _db.OrderDetails.Where(x => x.ID_Order == ID.ID_Order).ToList();
            foreach (var item in pro)
            {
                _db.OrderDetails.Remove(item);
            }
            _db.SaveChanges();
            Session[CartSession] = null;
            return Json(new
            {
                status = true
            });
        }

        public JsonResult Update(string cartModel)
        {
            var jsonCart = new JavaScriptSerializer().Deserialize<List<OrderDetail>>(cartModel);
            var sessionCart = (List<OrderDetail>)Session[CartSession];
            User user = Session["User"] as User;
            var ID = _db.Orders.Where(o => o.ID_Customer == user.Username && o.status == false).SingleOrDefault();
            foreach (var item in sessionCart)
            {
                var jsonItem = jsonCart.SingleOrDefault(x => x.Product.ID == item.Product.ID);
                if (jsonItem != null)
                {
                    item.Quantity = jsonItem.Quantity;
                    var pro = _db.OrderDetails.Where(x => x.ID_Order == ID.ID_Order && x.ID_Product==item.Product.ID).SingleOrDefault();
                    pro.Quantity = item.Quantity;
                    _db.OrderDetails.AddOrUpdate(pro);
                    _db.SaveChanges();
                }
            }
            Session[CartSession] = sessionCart;
            return Json(new
            {
                status = true
            });
        }
        [HttpGet]
        public ActionResult Payment()
        {
            User user = Session["User"] as User;
            var ID = _db.Orders.Where(o => o.ID_Customer == user.Username && o.status == false).SingleOrDefault();
            var cart = Session[CartSession];
            var list = new List<OrderDetail>();
            if (cart != null)
            {
                list = (List<OrderDetail>)cart;
            }
            return View(list);
        }

        [HttpPost]
        public ActionResult Payment(FormCollection frm)
        {

            User user = Session["User"] as User;
            var ID = _db.Orders.Where(o => o.ID_Customer == user.Username && o.status == false).SingleOrDefault();
           // var order = new Order();
            ID.Date_Create = DateTime.Now;
            ID.shipAddress = frm["address"];
            ID.shipMobile = frm["mobile"];
            ID.shipName = frm["shipName"];
            ID.status = true;
            _db.Orders.AddOrUpdate(ID);
            _db.SaveChanges();
            return RedirectToAction("Cart", "Shop");
            //var cart = (List<OrderDetail>)Session[CartSession];
            //return RedirectToAction("Success", "Shop");
            //return Redirect("/Shop/Success");
        }

        [HttpGet]
      
    [ChildActionOnly]
        public PartialViewResult HeaderCart()
        {
            User user = Session["User"] as User;
            var ID = _db.Orders.Where(o => o.ID_Customer == user.Username && o.status == false).SingleOrDefault();
            if (!(ID is null))
            {
                var pro = _db.OrderDetails.Where(x => x.ID_Order == ID.ID_Order).ToList();
                Session[CartSession] = pro;
            }
            var cart = Session[CartSession];
            // var cart = Session[CartSession];
            //var cart = pro;
            var list = new List<OrderDetail>();
            if (cart != null)
            {
                list = (List<OrderDetail>)cart;
            }

            return PartialView(list);
        }

        //Dang ky, dang nhap, dang xuat
        [HttpPost]
        public ActionResult Login(User user)

        {
            Session["Is Login"] = 0;
            Session["User"] = null;
            //Kiểm tra User có tồn tại trong database hay không
            if (user.Username == null || _db.Users.Find(user.Username.Trim()) == null)
                return Content("false");
            else
            {
                //Nếu có thì tiến hành kiểm tra ở Form và ở database có trùng khớp hay không
                var u = _db.Users.Find(user.Username.Trim());
                if (user.Username.Trim() == u.Username.Trim() && user.Password.Trim() == u.Password.Trim())
                {
                    Session["Is Login"] = 1;
                    Session["User"] = u;
                    return RedirectToAction("Home", "Shop");
                }
            }

            return Content("false");
        }

        [HttpPost]
        public ActionResult Logout()
        {
            Session["User"] = null;
            Session["Is Login"] = 0;
            return View("HomeGuest");
        }

        [HttpPost]
        public ActionResult Register(FormCollection user)
        {
            var u = _db.Users.Find(user["Username"]);
            if (u != null)
                return Content("userexist");
            var x = user["Password"];
            var y = user["Confirm"];
            if (Check.CheckUser(user) == true && u == null)
            {
                if (user["Password"].Equals(user["Confirm"]))
                {
                    u = Check.convertFtoU(user);
                    _db.Users.Add(u);
                    _db.SaveChanges();
                    return View("Login");
                }
                else
                    return Content("NotEqual");
            }
            else
            {
                return Content("false");
            }
        }

        //Xu ly san pham
        public ActionResult _Product(int? page)
        {
            int pagesize = 4;
            int pageNumber = (page ?? 1);
            var result = _db.Products.OrderBy(id => id.ID);
            return PartialView(result.ToPagedList(pageNumber, pagesize));
        }
        [HttpPost]
        public ActionResult AddProduct(FormCollection pro)

        {
            //var u = Check.convertFtoUPro(pro);
            Product product = new Product();
            if (Check.CheckProduct(pro) == true)
            {
                int i = 1;
                var p = _db.Products.ToList();
                foreach (var item in p)
                {
                    if (i < int.Parse(item.ID))
                    {
                        i = int.Parse(item.ID);
                    }
                }
                i++;
                product.ID = i.ToString();
                product.Brand = pro["Brand"];
                product.Country = pro["Country"];
                product.DateCreate = DateTime.Parse(pro["DateCreate"]);
                product.Description = pro["Description"];
                product.Image = pro["Image"];
                var x = product.Image.Length;
                product.Name = pro["Name"];
                product.Price = float.Parse(pro["Price"]);
                product.Style = pro["Style"];
                product.Warranty = int.Parse(pro["Warranty"]);
                _db.Products.Add(product);
                _db.SaveChanges();
                return View("AddProduct");
            }
            else
            {
                return Content("false");
            }
        }

        //Trang chu
        [HttpGet]
        [AuthorizeController]
        public ActionResult Home(int? page)
        {
            int pagesize = 4;
            int pageNumber = (page ?? 1);
            var result = _db.Products.OrderBy(id => id.ID);
            return View(result.ToPagedList(pageNumber,pagesize));
        }
        [HttpGet]
        public ActionResult HomeGuest(int? page)
        {
            int pagesize = 4;
            int pageNumber = (page ?? 1);
            var result = _db.Products.OrderBy(id => id.ID);
            return View(result.ToPagedList(pageNumber, pagesize));
        }

        [HttpPost]

        //Forgot and Reset Pass
        public ActionResult ResetPassword(FormCollection user)
        {//Kiểm tra User có tồn tại trong database hay không
            if (user["Username"] == null || _db.Users.Find(user["Username"].Trim()) == null)
                return Content("false");
            else
            {
                //Nếu có thì tiến hành kiểm tra ở Form và ở database có trùng khớp hay không
                var u = _db.Users.Find(user["Username"].Trim());
                if (user["Password"].Equals(user["Confirm"]))
                {
                    u.Password = user["Password"].Trim();
                    _db.Users.AddOrUpdate(u);
                    _db.SaveChanges();
                    return View("Login");
                }
                else
                {
                    return Content("NotEqual");
                }
            }
            //return View("");
        }
        [HttpPost]
        public ActionResult ForgotPassword(User user)
        {
            if (user.Email != null && ModelState.IsValid)
            {
                //var x = _db.Users.Where(u => u.Email.Equals(user.Email));
                var x = (from u in _db.Users where u.Email == user.Email select u).ToList();
                var MailTo = user.Email.Trim();
                //var u = _db.Users.Find(f["email"]);
                if (x.Count==1)
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
                    return View();
                }
                else
                {

                    return Content("false");
                }
            }
            else
            {
                
                
                return Content("false");
            }
            //return View();
        }

        //Dang nhap bang FB
        public ActionResult LoginFacebook()
        {
            var fb = new FacebookClient();
            var loginUrl = fb.GetLoginUrl(new
            {
                client_id = ConfigurationManager.AppSettings["FbAppId"],
                client_secret = ConfigurationManager.AppSettings["FbAppSecret"],
                redirect_uri = RedirectUri.AbsoluteUri,
                response_type = "code",
                scope = "email",
            });
            return Redirect(loginUrl.AbsoluteUri);
        }
        public ActionResult FacebookCallback(string code)
        {
            var fb = new FacebookClient();
            dynamic result = fb.Post("oauth/access_token", new
            {
                client_id = ConfigurationManager.AppSettings["FbAppId"],
                client_secret = ConfigurationManager.AppSettings["FbAppSecret"],
                redirect_uri = RedirectUri.AbsoluteUri,
                code = code,
            });
            var accessToken = result.access_token;
            if (!string.IsNullOrEmpty(accessToken))
            {
                fb.AccessToken = accessToken;

                dynamic me = fb.Get("me?fields=name,email,birthday,gender");
                string email = me.email;
                string name = me.name;
                DateTime birthday = Convert.ToDateTime(me.birthday);
                string gender = me.gender;

                var user = new User();
                user.Email = email;
                user.Username = email;
                user.Name = name;
                user.Birthday = birthday;
                //user. = gender;

                var resultInsert = new ShopController().InsertForFacebook(user);
                if(resultInsert!=null)
                {
                    Session["Is Login"] = 1;
                    Session["User"] = user;
                }    
                
            }
            return RedirectToAction("Home", "Shop");
        }
        public string InsertForFacebook(User model)
        {
            var user = _db.Users.SingleOrDefault(n => n.Username == model.Username);
            if (user == null)
            {
                _db.Users.Add(model);
                _db.SaveChanges();
                return model.Username;
            }
            else
            {
                return user.Username;
            }
        }
        
        
       
    }
}