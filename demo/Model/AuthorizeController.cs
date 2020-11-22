using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace demo.Model
{
    public class AuthorizeController: ActionFilterAttribute
    {
        shopEntities db = new shopEntities();
        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            int iUserID = Convert.ToInt32(HttpContext.Current.Session["Is Login"]);
            if (iUserID != 1)
            {

                filterContext.Result = new RedirectResult("~/Shop/Login");
            }

        }
    }
}