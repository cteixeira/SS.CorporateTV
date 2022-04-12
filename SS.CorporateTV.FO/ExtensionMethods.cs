using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace SS.CorporateTV.FO.Web.ExtensionMethods
{
    public class CustomAuthorize : AuthorizeAttribute
    {
        protected override bool AuthorizeCore(HttpContextBase httpContext)
        {            
            return (httpContext.Request.Cookies["CookieCorpTV"] != null);
        }

        protected override void HandleUnauthorizedRequest(AuthorizationContext filterContext)
        {
            filterContext.HttpContext.Response.Redirect(ConfigurationManager.AppSettings["BackOfficeConfigTVUrl"]);//?ReturnUrl=" + filterContext.HttpContext.Request.Path);
        }
    }
}