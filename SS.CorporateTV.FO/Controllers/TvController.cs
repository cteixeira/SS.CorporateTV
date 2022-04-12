using SS.CorporateTV.FO.Models;
using SS.CorporateTV.FO.Web.ExtensionMethods;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace SS.CorporateTV.FO.Controllers
{
    [CustomAuthorize]
    public class TvController : Controller
    {
        [CustomAuthorize]
        public ActionResult Index()
        {
            try
            {
                var agendamento = ObterAgendamento();

                if (agendamento == null)
                    return Redirect(ConfigurationManager.AppSettings["BackOfficeConfigTVUrl"]);

                
                return View(agendamento);
            }
            catch (Exception ex)
            {
                Core.Utils.Util.TratamentoErro(null, GetType().FullName, ex, null);
                return View("ErrorTv", new Exception(Resources.GestaoTv.Error));
            }
        }

        private ConfigDone ObterAgendamento()
        {
            HttpCookie myCookie = Request.Cookies["CookieCorpTV"];
            string idtv;
            string utilizadorid;
            if (myCookie == null)
            {
                return null;
                //return RedirectToAction("Index", "ConfigTv");
            }
            idtv = myCookie["tvid"];
            utilizadorid = myCookie["utilizadorid"];

            ControladorSite.Utilizador = new Core.Utilizador(null).AbrirUtilizadorAutenticado(int.Parse(utilizadorid), ControladorSite.Empresa.ID);
            if (ControladorSite.Utilizador == null)
            {
                return null;
            }

            Core.TV tv = new Core.TV(ControladorSite.Utilizador);
            Core.Model.Tv modelTv = new Core.Model.Tv();
            modelTv = tv.Abrir(long.Parse(idtv));

            Core.ProgramacaoAgendamento agendamentos = new Core.ProgramacaoAgendamento(ControladorSite.Utilizador);
            Core.Model.ProgramacaoAgendamento modelAgendaNoAr = agendamentos.AbrirAgendamentoNoAr(modelTv.ProgramacaoID);

            //if (!tv.Ativo)
            //    return View("ErrorTv", new Exception(CorporateTV_FO.Resources.GestaoTv.TvOff));
            //                string sourcestream = tv.Stream.Source;

            return new ConfigDone() { TvID = modelTv.TvID, ConteudoNoAr = new Conteudo(modelAgendaNoAr) };
        }

        [CustomAuthorize]
        public ActionResult AtualizarStream(long agendamentoNoAr, long conteudoNoAr)
        {
            try
            {
                var model = ObterAgendamento();

                if (model == null)
                    return null;

                if (model.ConteudoNoAr.ProgramacaoIDNoAr == agendamentoNoAr
                    && model.ConteudoNoAr.ConteudoIDNoAr == conteudoNoAr
                    && (!model.ConteudoNoAr.Modificado))
                {
                    return null;
                }

                var resp = Json(model, JsonRequestBehavior.AllowGet);
                resp.MaxJsonLength = int.MaxValue;
                return resp;
            }
            catch (Exception ex)
            {
                Core.Utils.Util.TratamentoErro(null, GetType().FullName, ex, null);
                return Json(null, JsonRequestBehavior.AllowGet);
            }
        }

        [CustomAuthorize]
        public ActionResult ConfigTV()
        {
            try
            {                
                return Redirect(ConfigurationManager.AppSettings["BackOfficeConfigTVUrl"]);
            }
            catch (Exception ex)
            {
                 Core.Utils.Util.TratamentoErro(null, GetType().FullName, ex, null);
                return View("ErrorTv", new Exception(Resources.GestaoTv.Error));
            }
        }
    }
}
