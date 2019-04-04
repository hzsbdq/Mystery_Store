using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace SpongeBob_Mall.Filter
{
    public class IsAuthorizeAttribute : AuthorizeAttribute
    {
        protected override void HandleUnauthorizedRequest(AuthorizationContext filterContext)
        {
            //filterContext.HttpContext.Response.Redirect("/Login/Index?errorMSG=3");
            filterContext.Result = new RedirectToRouteResult(
                new RouteValueDictionary
                {
                    { "controller", "Login" },
                    { "action", "Index" }         //需要跳转到的 controller 对应的 action   
                });
        }
        protected override bool AuthorizeCore(HttpContextBase httpContext)
        {
            bool result = false;
            if (httpContext.Session["user"] != null)
            {
                result = true;
            }
            return result;
        }
    }
}