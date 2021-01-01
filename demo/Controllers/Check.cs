using demo.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Mvc;

namespace demo.Controllers
{
    public class Check
    {
        public static bool CheckUser(FormCollection user)
        {
            if (user["ward"] !="" && user["district"] != "" && user["province"] != "" && user["Address"] != "" && user["Birthday"] != "" 
                && user["Name"] != "" && user["Password"] != "" && user["Phone"] != "" && user["Username"] != "" && user["Email"] != "")
                return true;
            else
                return false;
        }
        public static bool CheckProduct(FormCollection pro)
        {
            if (pro["Name"] != "" && pro["Image"] != "" && pro["Price"] != "" && pro["Warranty"] != ""
                && pro["DateCreate"] != "" && pro["Description"] != "" && pro["Style"] != ""
                && pro["Brand"] != "" && pro["Country"]!= "")
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
            byte[] image = Encoding.ASCII.GetBytes(pro["Image"]);
            product.Image = image;
            product.Name = pro["Name"];
            product.Price = int.Parse(pro["Price"]);
            product.Style = pro["Style"];
            product.Warranty = int.Parse(pro["Warranty"]);
            return product;
        }

        public static User convertFtoU(FormCollection user)
        {
            User u = new User();
            var address = user["Address"] + ", " + user["ward"] + ", " + user["district"] + ", " + user["province"];
            u.Address = address;
            u.Birthday = DateTime.Parse(user["Birthday"]);
            u.Email = user["Email"];
            u.Name = user["Name"];
            u.Password = user["Password"];
            u.Phone = user["Phone"];
            u.Username = user["Username"];
            u.isAdmin = false;
            return u;
        }
    }
}