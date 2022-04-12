using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using SS.CorporateTV.Core;

namespace SS.CorporateTV.BO
{
    [SessionState(System.Web.SessionState.SessionStateBehavior.Required)]
    public class ControladorSite
    {
        public static EmpresaAtiva Empresa
        {
            get
            {
                if (HttpContext.Current.Items["Empresa"] != null)
                {
                    var empresa = (EmpresaAtiva)HttpContext.Current.Items["Empresa"];
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

        public static UtilizadorAutenticado Utilizador
        {
            get
            {
                if (HttpContext.Current.Items["Utilizador"] != null)
                    return (UtilizadorAutenticado)HttpContext.Current.Items["Utilizador"];
                return null;
            }
            set
            {
                    HttpContext.Current.Items["Utilizador"] = value;
            }
        }
    }
}