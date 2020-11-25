﻿using demo.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace demo.Controllers
{
    public class Check
    {
        public static bool CheckUser(FormCollection user)
        {
            if (user["Address"] != null && user["Birthday"] != null && user["Name"] != null && user["Password"] != null
                && user["Phone"] != null && user["Username"] != null && user["Email"] != null)
                return true;
            else
                return false;
        }
        public static bool CheckProduct(FormCollection pro)
        {
            if (pro["Name"] != null && pro["Image"] != null && pro["Price"] != null && pro["Warranty"] != null
                && pro["DateCreate"] != null && pro["Description"] != null && pro["Style"] != null
                && pro["Brand"] != null && pro["Country"]!= null)
                return true;
            else
                return false;
        }
        public static Product convertFtoUPro(FormCollection pro)
        {
            Product product = new Product();
            product.Brand = pro["Brand"];
            product.Country = pro["Country"];
            product.DateCreate = DateTime.Parse(pro["DateCreate"]);
            product.Description = pro["Description"];
            product.Image = pro["Image"];
            product.Name = pro["Name"];
            product.Price = int.Parse(pro["Price"]);
            product.Style = pro["Style"];
            product.Warranty = int.Parse(pro["Warranty"]);
            return product;
        }

        public static User convertFtoU(FormCollection user)
        {
            User u = new User();
            u.Address = user["Address"];
            u.Birthday = DateTime.Parse(user["Birthday"]);
            u.Email = user["Email"];
            u.Name = user["Name"];
            u.Password = user["Password"];
            u.Phone = user["Phone"];
            u.Username = user["Username"];
            return u;
        }
    }
}