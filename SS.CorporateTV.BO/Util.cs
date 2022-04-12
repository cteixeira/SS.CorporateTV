using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;

namespace SS.CorporateTV.BO
{
    public class Util
    {        
        public static string GetCookieOrDefault(HttpRequestBase request, string name, string key)
        {
            return request.Cookies[name] == null ? "" : request.Cookies[name][key];
        }

        public static string TituloPagina(string nomePagina)
        {
            return String.Concat(Resources.Geral.NomeAppMobile, " - ", nomePagina);
        }


        public static string JavascriptAccao(bool escondeModal, string msg, string btnClass)
        {
            string script = escondeModal ? @"EscondeModal(); Notifica('{0}','{1}');" : @"Notifica('{0}','{1}');";
            return String.Format(script, msg, btnClass);
        }
        
        public static string JavascriptAccao(string modalParaEsconder, string requestRelativeUri)
        {
            var escondeModal = (modalParaEsconder != null);
            string script = @"EscondeModal('" + modalParaEsconder + "');ActualizaLista('{0}');";

            return String.Format(script, requestRelativeUri);
        }

        public static string JavascriptAccao(bool escondeModal, string msg, string btnClass, string requestRelativeUri)
        {
            string script = escondeModal ? @"EscondeModal(); Notifica('{0}','{1}');ActualizaLista('{2}');" : @"Notifica('{0}','{1}');ActualizaLista('{2}');";
            return String.Format(script, msg, btnClass, requestRelativeUri);
        }

        public static string JavascriptAccao(string modalParaEsconder, string msg, string btnClass, string requestRelativeUri)
        {
            string script = @"EscondeModal('" + modalParaEsconder + "'); Notifica('{0}','{1}');ActualizaLista('{2}');";
            return String.Format(script, msg, btnClass, requestRelativeUri);
        }

        #region Constantes WebConfig

        private static bool _maqProd = Convert.ToBoolean(ConfigurationManager.AppSettings["MaqProd"]);
        private static string _domain = ConfigurationManager.AppSettings["Domain"];
        private static int _takeIni = Convert.ToInt32(ConfigurationManager.AppSettings["TakeIni"]);
        private static int _takeNext = Convert.ToInt32(ConfigurationManager.AppSettings["TakeNext"]);
        private static int _sessaoCookie = Convert.ToInt32(ConfigurationManager.AppSettings["sessaoCookie"]);
        private static bool _demoAtivo = Convert.ToBoolean(ConfigurationManager.AppSettings["DemoAtivo"]);
        private static string _linkProduto = ConfigurationManager.AppSettings["LinkProduto"];
        private static string _frontOfficeTVUrl = ConfigurationManager.AppSettings["FrontOfficeTVUrl"];


        public static bool MaqProd { get { return _maqProd; } }
        public static string Domain { get { return _domain; } }
        public static int TakeIni { get { return _takeIni; } }
        public static int TakeNext { get { return _takeNext; } }
        public static int SessaoCookie { get { return _sessaoCookie; } }
        public static bool DemoAtivo { get { return _demoAtivo; } }
        public static string LinkProduto { get { return _linkProduto; } }
        public static string FrontOfficeTVUrl { get { return _frontOfficeTVUrl; } }

        #endregion
    }
}