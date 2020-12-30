﻿using demo.Model;
using Facebook;
using MailKit.Net.Smtp;
using MimeKit;
using MongoDB.Bson;
using MongoDB.Driver;
using OfficeOpenXml;
using PagedList;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.Entity.Migrations;
using System.Data.OleDb;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Web.Mvc;
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

        public ActionResult Login1(string controller, string action, string id)

        {
            return View();
        }
        public ActionResult Register()

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
            var ID = _db.Orders.Where(o => o.ID_Customer == user.Username && o.status == "Cart").SingleOrDefault();
            List<OrderDetail> pro = new List<OrderDetail>();
            if (!(ID is null))
            {
                pro = _db.OrderDetails.Where(x => x.ID_Order == ID.ID_Order).ToList();
                Session[CartSession] = pro;

            }
            int Quantity = Convert.ToInt32(quantity);
            var product = _db.Products.Find(ProductID);
            var cart = Session[CartSession];

            //Kiem tra gio hang co san pham hay khong
            if (Quantity != 0 && !(ID is null))
            {

                var id = _db.OrderDetails.Where(x => x.ID_Order == ID.ID_Order && x.ID_Product == ProductID).SingleOrDefault();

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
            else if (Quantity != 0)
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
                orders.status = "Cart";
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

            if (Quantity == 0)
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
            var ID = _db.Orders.Where(o => o.ID_Customer == user.Username && o.status == "Cart").SingleOrDefault();
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
            var ID = _db.Orders.Where(o => o.ID_Customer == user.Username && o.status == "Cart").SingleOrDefault();
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
            var ID = _db.Orders.Where(o => o.ID_Customer == user.Username && o.status == "Cart").SingleOrDefault();
            foreach (var item in sessionCart)
            {
                var jsonItem = jsonCart.SingleOrDefault(x => x.Product.ID == item.Product.ID);
                if (jsonItem != null)
                {
                    item.Quantity = jsonItem.Quantity;
                    var pro = _db.OrderDetails.Where(x => x.ID_Order == ID.ID_Order && x.ID_Product == item.Product.ID).SingleOrDefault();
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
            //var ID = _db.Orders.Where(o => o.ID_Customer == user.Username && o.status == "Cart").SingleOrDefault();
            var cart = Session[CartSession];
            var list = new List<OrderDetail>();
            if (cart != null)
            {
                list = (List<OrderDetail>)cart;
            }
            return View(list);
        }

        public ActionResult CartStatus(string status)
        {
            var ID = _db.Orders.ToList();
            User user = Session["User"] as User;
            if (user.isAdmin == true)
            {
                ID = _db.Orders.Where(x => x.status == status).ToList();
            }
            else
            {
                // Session[CartSession] = null;
                ID = _db.Orders.Where(o => o.ID_Customer == user.Username && o.status == status).ToList();
            }
            List<OrderDetail> pro = new List<OrderDetail>();
            if (!(ID is null))
            {
                foreach (var item in ID)
                {
                    var Pro = _db.OrderDetails.Where(x => x.ID_Order == item.ID_Order).ToList();
                    pro.AddRange(Pro);

                }
                Session[CartSession] = pro;

            }
            //Hien len view
            var cart = Session[CartSession];
            var l = new List<OrderDetail>();
            l = (List<OrderDetail>)cart;
            ViewBag.Message = status;
            return View(l);
        }

        //hiển thị sản phẩm trong giỏ hàng

        [HttpPost]
        public ActionResult Payment(FormCollection frm)
        {

            User user = Session["User"] as User;
            var ID = _db.Orders.Where(o => o.ID_Customer == user.Username && o.status == "Cart").SingleOrDefault();
            // var order = new Order();
            ID.Date_Create = DateTime.Now;
            ID.shipAddress = frm["address"].Trim();
            ID.shipMobile = frm["mobile"].Trim();
            ID.shipName = frm["shipName"].Trim();
            ID.status = "Wait";
            _db.Orders.AddOrUpdate(ID);
            _db.SaveChanges();
            return RedirectToAction("Cart", "Shop");
        }

        public JsonResult DeleteOrder(long id)
        {
            string Id = id.ToString();
            var dh = _db.Orders.ToList();
            User user = Session["User"] as User;
            var ID = _db.Orders.Where(o => o.ID_Order == Id).SingleOrDefault();
            ID.status = "Cancel";
            ID.Date_Cancel = DateTime.Now;
            _db.Orders.AddOrUpdate(ID);
            _db.SaveChanges();
            if (user.isAdmin == true)
            {
                dh = _db.Orders.Where(o => o.status == "Wait").ToList();
            }
            else
            {
                dh = _db.Orders.Where(o => o.ID_Customer == user.Username && o.status == "Wait").ToList();
            }
            List<OrderDetail> idorder = new List<OrderDetail>();
            if (!(ID is null))
            {
                foreach (var item in dh)
                {
                    var Pro = _db.OrderDetails.Where(x => x.ID_Order == item.ID_Order).ToList();
                    idorder.AddRange(Pro);

                }
                Session[CartSession] = idorder;
            }
            return Json(new
            {
                status = true
            });
        }

        public JsonResult Confirm(long id)
        {
            string Id = id.ToString();
            var dh = _db.Orders.ToList();
            User user = Session["User"] as User;
            var ID = _db.Orders.Where(o => o.ID_Order == Id).SingleOrDefault();
            ID.status = "Delivery";
            ID.Date_delivery = DateTime.Now;
            var pros = _db.OrderDetails.Where(p => p.ID_Order == ID.ID_Order).ToList();
            foreach (var pro in pros)
            {
                var tmp = _db.Products.Where(x => x.ID == pro.ID_Product).SingleOrDefault();
                var count = tmp.Quantity;
                count = count - pro.Quantity;
                tmp.Quantity = count;
            }
            _db.Orders.AddOrUpdate(ID);
            _db.SaveChanges();
            if (user.isAdmin == true)
            {
                dh = _db.Orders.Where(o => o.status == "Wait").ToList();
            }
            List<OrderDetail> idorder = new List<OrderDetail>();
            if (!(ID is null))
            {
                foreach (var item in dh)
                {
                    var Pro = _db.OrderDetails.Where(x => x.ID_Order == item.ID_Order).ToList();
                    idorder.AddRange(Pro);

                }
                Session[CartSession] = idorder;
            }
            return Json(new
            {
                status = true
            });
        }

        public JsonResult Received(long id)
        {
            string Id = id.ToString();
            User user = Session["User"] as User;
            var ID = _db.Orders.Where(o => o.ID_Order == Id).SingleOrDefault();
            ID.status = "Finish";
            ID.Date_Recived = DateTime.Now;
            var pros = _db.OrderDetails.Where(p => p.ID_Order == ID.ID_Order).ToList();
            foreach (var pro in pros)
            {
                var tmp = _db.Products.Where(x => x.ID == pro.ID_Product).SingleOrDefault();
                var count = int.Parse(tmp.TopHot);
                count = count + int.Parse(pro.Quantity.ToString());
                tmp.TopHot = count.ToString();
            }
            _db.Orders.AddOrUpdate(ID);
            _db.SaveChanges();
            var dh = _db.Orders.Where(o => o.status == "Delivery").ToList();
            List<OrderDetail> idorder = new List<OrderDetail>();
            if (!(ID is null))
            {
                foreach (var item in dh)
                {
                    var Pro = _db.OrderDetails.Where(x => x.ID_Order == item.ID_Order).ToList();
                    idorder.AddRange(Pro);

                }
                Session[CartSession] = idorder;
            }
            return Json(new
            {
                status = true
            });
        }

        public JsonResult Addcart(string ProductID)
        {
            int quantity = 1;
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
            var ID = _db.Orders.Where(o => o.ID_Customer == user.Username && o.status == "Cart").SingleOrDefault();
            List<OrderDetail> pro = new List<OrderDetail>();
            if (!(ID is null))
            {
                pro = _db.OrderDetails.Where(x => x.ID_Order == ID.ID_Order).ToList();
                Session[CartSession] = pro;

            }
            int Quantity = Convert.ToInt32(quantity);
            var product = _db.Products.Find(ProductID);
            var cart = Session[CartSession];

            //Kiem tra gio hang co san pham hay khong
            if (Quantity != 0 && !(ID is null))
            {

                var id = _db.OrderDetails.Where(x => x.ID_Order == ID.ID_Order && x.ID_Product == ProductID).SingleOrDefault();

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
            else if (Quantity != 0)
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
                orders.status = "Cart";
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
            return Json(new
            {
                status = true
            });
        }
        public ActionResult Detail(string id)
        {
            string Id = id.ToString();
            User user = Session["User"] as User;
            var ID = _db.Orders.Where(o => o.ID_Order == Id).SingleOrDefault();
            var pro = _db.OrderDetails.Where(x => x.ID_Order == ID.ID_Order).ToList();
            var l = new List<OrderDetail>();
            l = (List<OrderDetail>)pro;
            return View(l);
        }

        //Doi dia chi giao hang

        [HttpGet]
        public ActionResult EditAddress()
        {
            var list = new List<DeliveryAddress>();
            User user = Session["User"] as User;
            var ID = _db.DeliveryAddresses.Where(x => x.UserName == user.Username).ToList();
            list = (List<DeliveryAddress>)ID;
            return View(list);
        }
        [HttpPost]
        public ActionResult EditAddress(FormCollection frm)
        {
            var name = frm["Name"];
            var phonenum = frm["Phonenum"];
            var province = frm["Province"];
            var district = frm["District"];
            var ward = frm["Ward"];
            var street = frm["Street"];
            shopEntities db = new shopEntities();
            var address = street + ", " + ward + ", " + district + ", " + province;
            var deliveryAddress = new DeliveryAddress();
            deliveryAddress.FullName = name;
            deliveryAddress.Phone = phonenum;
            deliveryAddress.Address = address;
            var user = Session["User"] as User;
            deliveryAddress.UserName = user.Username;
            deliveryAddress.isDefault = false;
            deliveryAddress.User = user;
            db.DeliveryAddresses.Add(deliveryAddress);

            db.SaveChanges();
            return View();
        }

        [HttpGet]

        [ChildActionOnly]
        public PartialViewResult HeaderCart()
        {
            User user = Session["User"] as User;
            if (user.isAdmin == false)
            {
                var ID = _db.Orders.Where(o => o.ID_Customer == user.Username && o.status == "Cart").SingleOrDefault();
                if (!(ID is null))
                {
                    var pro = _db.OrderDetails.Where(x => x.ID_Order == ID.ID_Order).ToList();
                    Session[CartSession] = pro;
                }
                else
                {
                    Session[CartSession] = null;
                }
            }
            else
            {
                var ID = _db.Orders.Where(x => x.status == "Wait").ToList();
                List<OrderDetail> pro = new List<OrderDetail>();
                if (!(ID is null))
                {
                    foreach (var item in ID)
                    {
                        var Pro = _db.OrderDetails.Where(x => x.ID_Order == item.ID_Order).ToList();
                        pro.AddRange(Pro);

                    }
                    Session[CartSession] = pro;

                }
                else
                {
                    Session[CartSession] = null;
                }
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
                    //return Redirect(Request.UrlReferrer.ToString());
                    //return Request.UrlReferrer;
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
            DeliveryAddress address = new DeliveryAddress();
            if (u != null)
                return Content("userexist");
            var x = user["Password"];
            var y = user["Confirm"];
            if (Check.CheckUser(user) == true && u == null)
            {
                if (user["Password"].Equals(user["Confirm"]))
                {
                    u = Check.convertFtoU(user);
                    address.UserName = u.Username;
                    address.Phone = u.Phone;
                    address.Address = u.Address;
                    _db.Users.Add(u);
                    _db.DeliveryAddresses.Add(address);
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
        public ActionResult _Product(int? page, string brand, string id)
        {
            if (id == "home")
            {
                int pagesize = 8;
                int pageNumber = (page ?? 1);
                var result = _db.Products.OrderBy(x => x.ID);
                return PartialView(result.ToPagedList(pageNumber, pagesize));
            }
            else if (id == "brand")
            {
                int pagesize = 8;
                int pageNumber = (page ?? 1);
                var result = _db.Products.Where(x => x.Brand == brand).OrderBy(ID => ID.ID);
                return PartialView(result.ToPagedList(pageNumber, pagesize));
            }
            else if (id == "style")
            {
                int pagesize = 8;
                int pageNumber = (page ?? 1);
                var result = _db.Products.Where(x => x.Style == brand).OrderBy(ID => ID.ID);
                return PartialView(result.ToPagedList(pageNumber, pagesize));
            }
            return View();
        }

        [ValidateInput(false)]
        [HttpPost]
        public ActionResult AddProduct(FormCollection pro)

        {
            //var u = Check.convertFtoUPro(pro);
            Product product = new Product();
            if (Check.CheckProduct(pro) == true)
            {
                int i = 0;
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
                byte[] image = Encoding.ASCII.GetBytes(pro["Image"]);
                product.Image = image;
                var x = product.Image.Length;
                product.Name = pro["Name"];
                product.Price = float.Parse(pro["Price"]);
                product.Style = pro["Style"];
                product.Warranty = int.Parse(pro["Warranty"]);
                product.Quantity = int.Parse(pro["Quantity"]);
                product.TopHot = "0";
                _db.Products.Add(product);
                _db.SaveChanges();
                return RedirectToAction("ListProduct", "Shop");
            }
            else
            {
                return Content("false");
            }
        }

        [HttpGet]
        public ActionResult AddProduct()
        {
            var list = new List<Product>();
            var pro = _db.Products.ToList();
            list = (List<Product>)pro;

            return View(list);
        }

        [ValidateInput(false)]
        [HttpPost]
        public ActionResult EditProduct(FormCollection pro)

        {
            //var u = Check.convertFtoUPro(pro);
            Product product = new Product();
            if (Check.CheckProduct(pro) == true)
            {
                var p = _db.Products.ToList();
                product.ID = p.Where(y => y.Name == pro["Name"]).SingleOrDefault().ID;
                product.Brand = pro["Brand"];
                product.Country = pro["Country"];
                product.DateCreate = DateTime.Parse(pro["DateCreate"]);
                product.Description = pro["Description"];
                byte[] image = Encoding.ASCII.GetBytes(pro["Image"]);
                product.Image = image;
                var x = product.Image.Length;
                product.Name = pro["Name"];
                product.Price = float.Parse(pro["Price"]);
                product.Style = pro["Style"];
                product.Warranty = int.Parse(pro["Warranty"]);
                product.Quantity = int.Parse(pro["Quantity"]);
                product.TopHot = "0";
                _db.Products.AddOrUpdate(product);
                _db.SaveChanges();
                return RedirectToAction("ListProduct", "Shop");
            }
            else
            {
                return Content("false");
            }
        }


        [HttpGet]
        public ActionResult EditProduct(string id)
        {
            var list = new List<Product>();
            var pro = _db.Products.Where(x => x.ID == id).ToList();
            list = (List<Product>)pro;

            return View(list);
        }

        public JsonResult DeletePro(long id)
        {
            string Id = id.ToString();
            var pro = _db.Products.Where(x => x.ID == Id).SingleOrDefault();
            _db.Products.Remove(pro);
            _db.SaveChanges();
            return Json(new
            {
                status = true
            });
        }

        public ActionResult ListProduct(int? page)
        {
            int pagesize = 8;
            int pageNumber = (page ?? 1);
            var result = _db.Products.OrderBy(id => id.ID);
            return View(result.ToPagedList(pageNumber, pagesize));
        }
        public ActionResult _ListProduct(int? page)
        {
            int pagesize = 8;
            int pageNumber = (page ?? 1);
            var result = _db.Products.OrderBy(id => id.ID);
            return View(result.ToPagedList(pageNumber, pagesize));
        }
        //Trang chu
        public ActionResult HeaderMenu()
        {
            return View();
        }
        [HttpGet]
        [AuthorizeController]
        public ActionResult Home(int? page)
        {
            int pagesize = 8;
            int pageNumber = (page ?? 1);
            var result = _db.Products.OrderBy(id => id.ID);
            return View(result.ToPagedList(pageNumber, pagesize));
        }

        public ActionResult HeaderMenuGuest()
        {
            return View();
        }

        [HttpGet]
        public ActionResult HomeGuest(int? page)
        {
            int pagesize = 8;
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
                if (x.Count == 1)
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

        [HttpGet]
        public ActionResult ChangePassword()
        {
            return View();
        }
        [HttpPost]
        public ActionResult ChangePassword(FormCollection USER)
        {
            User user = Session["User"] as User;
            //Kiểm tra User có tồn tại trong database hay không
            if (user == null || _db.Users.Find(user.Username.Trim()) == null)
                return Content("false");
            else if (USER["current"] =="")
                return Content("null");
            else if (USER["current"] == user.Password)
            {
                var u = _db.Users.Find(user.Username.Trim());
                if (USER["Password"] == "" || USER["Confirm"] == "")
                    return Content("null");
                else if (USER["Password"].Equals(USER["Confirm"]))
                {
                    u.Password = USER["Password"].Trim();
                    _db.Users.AddOrUpdate(u);
                    _db.SaveChanges();
                    return View("Login");
                }
                else
                {
                    return Content("NotEqual");
                }
            }
            else
                return Content("Error");
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
                if (resultInsert != null)
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


        //Thong tin cua user
        public ActionResult Profiles()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Profiles(FormCollection frm)
        {
            User user = Session["User"] as User;
            var u = _db.Users.Find(user.Username);
            u.Name = frm["Name"];
            u.Phone = frm["Phone"];
            u.Address = frm["Address"];
            u.Email = frm["Email"];
            u.Birthday = DateTime.Parse(frm["Birthday"]);

            _db.Users.AddOrUpdate(u);
            _db.SaveChanges();
            _db.SaveChanges();
            // return View();
            return RedirectToAction("Profiles", "Shop");
        }

        //hãng sản phẩm
        //[HttpPost]
        public ActionResult Brand(string id)
        {

            var brand = _db.Brands.Where(x => x.Name == id).SingleOrDefault();
            var list = new List<Product>();
            var pro = _db.Products.Where(x => x.Brand == brand.ID.ToString()).ToList();
            list = (List<Product>)pro;
            return View(list);
        }

        //Loại đồng hồ
        public ActionResult Style(string id)
        {
            var list = new List<Product>();
            var pro = _db.Products.Where(x => x.Style == id).ToList();
            list = (List<Product>)pro;
            return View(list);
        }


        public ActionResult ProDetail(string id)
        {
            var pro = _db.Products.Where(x => x.ID == id).ToList();
            return View(pro);
        }

        public ActionResult ImportExcel(demo.Model.ImportExcel importExcel)
        {
            if (ModelState.IsValid)
            {
                string path = Server.MapPath("~/Import/" + importExcel.file.FileName);
                importExcel.file.SaveAs(path);

                string excelConnectionString = @"Provider='Microsoft.ACE.OLEDB.12.0';Data Source='" + path + "';Extended Properties='Excel 12.0 Xml;IMEX=1'";
                OleDbConnection excelConnection = new OleDbConnection(excelConnectionString);

                //Sheet Name
                excelConnection.Open();
                string tableName = excelConnection.GetSchema("Tables").Rows[0]["TABLE_NAME"].ToString();
                excelConnection.Close();
                //End

                OleDbCommand cmd = new OleDbCommand("Select * from [" + tableName + "]", excelConnection);

                excelConnection.Open();

                OleDbDataReader dReader;
                dReader = cmd.ExecuteReader();

                string conString = "data source=.;initial catalog=shop;integrated security=True;MultipleActiveResultSets=True;App=EntityFramework&quot;";
                SqlConnection con = new SqlConnection(conString);
                SqlBulkCopy sqlBulk = new SqlBulkCopy(con);

                //Give your Destination table name
                sqlBulk.DestinationTableName = "Products";

                //Mappings

                sqlBulk.ColumnMappings.Add("ID", "ID");
                sqlBulk.ColumnMappings.Add("Name", "Name");
                sqlBulk.ColumnMappings.Add("Price", "Price");
                sqlBulk.ColumnMappings.Add("Quantity", "Quantity");
                sqlBulk.ColumnMappings.Add("Warranty", "Warranty");
                sqlBulk.ColumnMappings.Add("Price_New", "Price_New");
                sqlBulk.ColumnMappings.Add("TopHot", "TopHot");
                sqlBulk.ColumnMappings.Add("DateCreate", "DateCreate");
                sqlBulk.ColumnMappings.Add("Description", "Description");
                sqlBulk.ColumnMappings.Add("Style", "Style");
                sqlBulk.ColumnMappings.Add("Brand", "Brand");
                sqlBulk.ColumnMappings.Add("Country", "Country");
                sqlBulk.ColumnMappings.Add("Image", "Image");
                con.Open();
                sqlBulk.WriteToServer(dReader);
                con.Close();
                excelConnection.Close();

                ViewBag.Result = "Successfully Imported";
                return RedirectToAction("ListProduct", "Shop");
            }
            return View();
        }

        public void DownloadExcel()
        {
            var collection = _db.OrderDetails.ToList();
            ExcelPackage.LicenseContext = LicenseContext.Commercial;
            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
            // var collection= db.

            ExcelPackage Ep = new ExcelPackage();
            ExcelWorksheet Sheet = Ep.Workbook.Worksheets.Add("ReportOrder");
            Sheet.Cells["A1"].Value = "ID_Order";
            Sheet.Cells["B1"].Value = "Name";
            Sheet.Cells["C1"].Value = "ID_Product";
            Sheet.Cells["D1"].Value = "Product_Name";
            Sheet.Cells["E1"].Value = "Quantity";
            Sheet.Cells["F1"].Value = "Price";
            int row = 2;
            foreach (var item in collection)
            {

                Sheet.Cells[string.Format("A{0}", row)].Value = item.ID_Order;
                Sheet.Cells[string.Format("B{0}", row)].Value = item.Order.User.Name;
                Sheet.Cells[string.Format("C{0}", row)].Value = item.ID_Product;
                Sheet.Cells[string.Format("D{0}", row)].Value = item.Product.Name;
                Sheet.Cells[string.Format("E{0}", row)].Value = item.Quantity;
                Sheet.Cells[string.Format("F{0}", row)].Value = item.Price;
                row++;
            }


            Sheet.Cells["A:AZ"].AutoFitColumns();
            Response.Clear();
            Response.ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
            Response.AddHeader("content-disposition", "attachment: filename=" + "ReportOrder.xlsx");
            Response.BinaryWrite(Ep.GetAsByteArray());
            Response.End();
        }

        public ActionResult Users(int? page, string admin)
        {
            int pagesize = 4;
            int pageNumber = (page ?? 1);
            var result = _db.Users.Where(x => x.isAdmin.ToString() == admin).OrderBy(id => id.Username);
            return View(result.ToPagedList(pageNumber, pagesize));
        }
        [HttpGet]
        public ActionResult _Users(int? page, string admin)
        {
            int pagesize = 4;
            int pageNumber = (page ?? 1);
            var result = _db.Users.Where(x => x.isAdmin.ToString() == admin).OrderBy(id => id.Username);
            return View(result.ToPagedList(pageNumber, pagesize));
        }
        [HttpPost]
        public JsonResult DeleteUser(string id)
        {
            string UserName = id.ToString();
            var user = _db.Users.Where(x => x.Username == UserName).SingleOrDefault();
            _db.Users.Remove(user);
            _db.SaveChanges();
            return Json(new
            {
                status = true
            });
        }
        public JsonResult GetAllDistrictByProvinceId(string id)
        {
            if (id == "")
                id = "1";
            List<string> Districts = new List<string>();
            using (var db = new shopEntities())
            {
                var data = db.Districts.Where(x => x.ProvinceId.ToString() == id).OrderBy(x => x.Name).ToList();
                foreach(var item in data)
                {
                    Districts.Add(item.Name);
                }    
                return Json(Districts, JsonRequestBehavior.AllowGet);
            }
        }
        public JsonResult GetAllWardByDistrictId(string id)
        {
            if (id == "")
                id = "1";
            List<string> Wards = new List<string>();
            using (var db = new shopEntities())
            {
                id = db.Districts.Where(d => d.Name == id).SingleOrDefault().Id.ToString();
                var data = db.Wards.Where(x => x.DistrictID.ToString() == id).OrderBy(x => x.Name).ToList();
                foreach (var item in data)
                {
                    Wards.Add(item.Name);
                }
                return Json(Wards, JsonRequestBehavior.AllowGet);
            }
        }
    }
}