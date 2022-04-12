using SS.CorporateTV.BO.App_Start;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading;
using System.Web;
using System.Web.Http;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;

namespace SS.CorporateTV.BO
{
    // Note: For instructions on enabling IIS6 or IIS7 classic mode, 
    // visit http://go.microsoft.com/?LinkId=9394801
    public class MvcApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();

            WebApiConfig.Register(GlobalConfiguration.Configuration);
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);
        }

        protected void Application_AuthenticateRequest(object sender, EventArgs e)
        {
            try
            {
                ControladorSite.Empresa = new Core.Empresa(null).AbrirEmpresa(HttpContext.Current.Request.Url.Host);

                if(ControladorSite.Empresa == null)
                {
                    Response.Redirect(Util.LinkProduto);
                }

                if (User != null && User.Identity.IsAuthenticated)
                {
                    ControladorSite.Utilizador = new Core.Utilizador(null).AbrirUtilizadorAutenticado(Convert.ToInt64(User.Identity.Name), ControladorSite.Empresa.ID);

                    if (ControladorSite.Utilizador == null || ControladorSite.Utilizador.EmpresaID != ControladorSite.Empresa.ID)
                    {
                        System.Web.Security.FormsAuthentication.SignOut();
                        Response.RedirectToRoute("Login");
                    }

                    CultureInfo ci = new CultureInfo("pt-PT");
                    Thread.CurrentThread.CurrentCulture = Thread.CurrentThread.CurrentUICulture = ci;
                }
            }
            catch (Exception ex)
            {
                Core.Utils.Util.TratamentoErro(null, GetType().FullName, ex, null);
            }
        }
    }
}