using System.Web;
using System.Web.Mvc;

namespace SS.CorporateTV.FO
{
    public class FilterConfig
    {
        public static void RegisterGlobalFilters(GlobalFilterCollection filters)
        {
            filters.Add(new HandleErrorAttribute());
        }
    }
}