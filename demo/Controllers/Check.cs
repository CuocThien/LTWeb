using demo.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace demo.Controllers
{
    public class Check
    {
        public static bool CheckUser(User user)
        {
            if (user.Address != null && user.Birthday != null && user.Name != null && user.Password != null
                && user.Phone != null && user.Username != null)
                return true;
            else
                return false;
        }
    }
}