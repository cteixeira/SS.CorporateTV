using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace SS.CorporateTV.BO
{
    public class RouteConfig
    {
        public static void RegisterRoutes(RouteCollection routes)
        {
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");

            //routes.MapRoute(
            //  name: "Home",
            //  url: "{controller}/{action}/{id}",
            //  defaults: new { controller = "Default", action = "Index", id = UrlParameter.Optional }
            //);

            routes.MapRoute(
              name: "Conteudo",
              url: "{controller}/{action}/{id}",
              defaults: new { controller = "Conteudo", action = "Index", id = UrlParameter.Optional }
            );

            routes.MapRoute(
                name: "Programacao",
                url: "{controller}/{action}/{id}",
                defaults: new { controller = "Programacao", action = "Index", id = UrlParameter.Optional }
            );

            routes.MapRoute(
                 name: "GestaoTv",
                 url: "{controller}/{action}/{id}",
                 defaults: new { controller = "GestaoTv", action = "Index", id = UrlParameter.Optional }
            );

            routes.MapRoute(
                    name: "Utilizador",
                    url: "{controller}/{action}/{id}",
                    defaults: new { controller = "Utilizador", action = "Index", id = UrlParameter.Optional }
            );

            routes.MapRoute(
                name: "Default",
                url: "{*url}",
                defaults: new { controller = "Conteudo", action = "Index", id = UrlParameter.Optional }
            );
        }
    }
}