using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace SS.CorporateTV.FO
{
    [SessionState(System.Web.SessionState.SessionStateBehavior.Required)]
    public class ControladorSite
    {
        public static Core.EmpresaAtiva Empresa
        {
            get
            {
                if (HttpContext.Current.Items["Empresa"] != null)
                {
                    var empresa = (Core.EmpresaAtiva)HttpContext.Current.Items["Empresa"];
                    if (String.Equals(empresa.Url, HttpContext.Current.Request.Url.Host, StringComparison.CurrentCultureIgnoreCase))
                            return empresa;
                }
                return null;
            }
            set
            {
                HttpContext.Current.Items["Empresa"] = value;
            }
        }

        public static Core.UtilizadorAutenticado Utilizador
        {
            get
            {
                if (HttpContext.Current.Items["Utilizador"] != null)
                    return (Core.UtilizadorAutenticado)HttpContext.Current.Items["Utilizador"];
                else
                return null;
            }
            set
            {
                HttpContext.Current.Items["Utilizador"] = value;
            }
        }
    }
}