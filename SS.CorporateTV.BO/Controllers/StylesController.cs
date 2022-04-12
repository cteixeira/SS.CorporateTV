using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using SS.CorporateTV.BO.Web.ExtensionMethods;

namespace SS.CorporateTV.BO.Controllers
{
    public class StylesController : Controller
    {
        //
        // GET: /Styles/

        public ActionResult Index()
        {
            return Content("Styles folder");
        }

        protected override void HandleUnknownAction(string actionName)
        {
            var res = this.CssFromView(actionName);
            res.ExecuteResult(ControllerContext);
        }

    }
}
