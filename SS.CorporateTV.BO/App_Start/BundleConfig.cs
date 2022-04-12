using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Optimization;

namespace SS.CorporateTV.BO.App_Start
{
    public class BundleConfig
    {
        public static void RegisterBundles(BundleCollection bundles)
        {
            bundles.Add(new ScriptBundle("~/scripts/core").Include(
                          "~/Scripts/main.js",
                          "~/Scripts/moment.js",
                        "~/Scripts/moment.min.js",
                        "~/Scripts/daterangepicker.js",
                        "~/Scripts/notify-combined.*",
                        "~/Scripts/bootstrap.*",
                        "~/Scripts/bootstrap-multiselect.*",
                        "~/Scripts/respond.*",
                        "~/Scripts/jquery-1.10.2.*",
                         "~/Scripts/jquery-ui*",
                         "~/Scripts/interact.js",
                         "~/Scripts/daypilot.lite.js"
                        ));

            bundles.Add(new ScriptBundle("~/scripts/jqueryval").Include(
                        "~/Scripts/jquery.validate.*",
                        "~/Scripts/jquery.unobtrusive*",
                        "~/Scripts/extensaojQueryValidator*"
                        ));

            bundles.Add(new StyleBundle("~/Content/core").Include(
                       "~/Content/bootstrap.*",
                       "~/Content/Site.css",
                       "~/Content/bootstrap-multiselect*",
                       "~/Content/daterangepicker-bs3*",
                       "~/Content/font-awesome-4.3.0/css/font-awesome.min.css"
                     ));
        }
    }
}