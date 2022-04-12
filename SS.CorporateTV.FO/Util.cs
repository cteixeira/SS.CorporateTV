using System;
using System.Configuration;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SS.CorporateTV.FO
{
    public class Util
    {
        private static int _intervaloatualizarstream = Convert.ToInt32(ConfigurationManager.AppSettings["intervaloatualizarstream"]);

        public static int IntervaloAtualizarStream { get { return _intervaloatualizarstream; } }
    }
}