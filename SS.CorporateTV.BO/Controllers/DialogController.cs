using DayPilot.Web.Mvc.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using SS.CorporateTV.BO.Resources;

namespace SS.CorporateTV.BO.Controllers
{
    public class Event
    {
        public string Id { get; set; }
        public string Text { get; set; }
        public DateTime Start { get; set; }
        public DateTime End { get; set; }
    }

    //[HandleError]
    [Authorize]
    public class DialogController : Controller
    {
        #region Propriedades

        public List<Core.Enum.Permissao> Permissoes
        {
            get
            {
                return new Core.Programacao(ControladorSite.Utilizador).Permissoes;
            }
        }
        #endregion

        public ActionResult NovoAgendamento(string id)
        {           

            ViewData.Add("Permissoes", Permissoes);

            return View(new Models.ProgramacaoAgendamentoModel()
            {   
                 ProgramacaoID = long.Parse(Request.QueryString["pid"]),
                 DiaSemana = string.IsNullOrEmpty(Request.QueryString["start"]) ? 7 : Core.Utils.Util.AcertoDiaSemana((int)Convert.ToDateTime(Request.QueryString["start"]).DayOfWeek),
                 Inicio = string.IsNullOrEmpty(Request.QueryString["start"]) ? new TimeSpan(DateTime.Now.Hour, 0, 0) : (TimeSpan)Convert.ToDateTime(Request.QueryString["start"]).TimeOfDay,
                 InicioStr = string.IsNullOrEmpty(Request.QueryString["start"]) ? new TimeSpan(DateTime.Now.Hour, 0, 0).ToString("hh\\:mm") : Convert.ToDateTime(Request.QueryString["start"]).TimeOfDay.ToString("hh\\:mm"),
                 Fim = string.IsNullOrEmpty(Request.QueryString["start"]) ? new TimeSpan(DateTime.Now.AddHours(1).Hour, 0, 0) : (TimeSpan)Convert.ToDateTime(Request.QueryString["end"]).TimeOfDay,
                 FimStr = string.IsNullOrEmpty(Request.QueryString["start"]) ? new TimeSpan(DateTime.Now.AddHours(1).Hour, 0, 0).ToString("hh\\:mm") : Convert.ToDateTime(Request.QueryString["start"]).TimeOfDay.Add(new TimeSpan(0,30,0)).ToString("hh\\:mm"),
            }
            );
        }

        //[AcceptVerbs(HttpVerbs.Post)]
        [HttpPost]
        [ValidateInput(false)]
        public ActionResult NovoAgendamento(FormCollection form)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    TimeSpan start = TimeSpan.Parse(form["InicioStr"]);
                    TimeSpan end = TimeSpan.Parse(form["FimStr"]);
                    var conteudo = long.Parse(form["ConteudoID"]);

                    var programacaoAgendamento = new Core.ProgramacaoAgendamento(ControladorSite.Utilizador);
                    var agendamento = new Core.Model.ProgramacaoAgendamento();
                    agendamento.ConteudoID = conteudo;

                    agendamento.ProgramacaoID = long.Parse(form["ProgramacaoID"]);
                    agendamento.DiaSemana = short.Parse(form["DiaSemana"].Split(',')[0]);
                    agendamento.Inicio = start;

                    if (end == new TimeSpan(0, 0, 0))
                        agendamento.Fim = new TimeSpan(23, 59, 59);
                    else
                        agendamento.Fim = end;

                    programacaoAgendamento.Inserir(agendamento);

                    //return JavaScript(Util.JavascriptAccao(true, Resources.Geral.GravarSucesso, "success"));
                    return JavaScript(SimpleJsonSerializer.Serialize("OK"));

                }
                else
                    throw new Exception(Resources.Erro.ModeloInvalido);
            }
            catch (Exception ex)
            {
                Core.Utils.Util.TratamentoErro(null, GetType().FullName, ex, ControladorSite.Utilizador);
                return JavaScript(Util.JavascriptAccao(true, Resources.Erro.Geral, "error"));
            }
        }

    }
}
