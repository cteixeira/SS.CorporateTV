using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using DayPilot.Web.Mvc;
using DayPilot.Web.Mvc.Events.Calendar;
using DayPilot.Web.Mvc.Json;
using System.Collections;
using DayPilot.Web.Mvc.Enums;
//using BeforeCellRenderArgs = DayPilot.Web.Mvc.Events.Calendar.BeforeCellRenderArgs;
using TimeRangeSelectedArgs = DayPilot.Web.Mvc.Events.Calendar.TimeRangeSelectedArgs;


namespace SS.CorporateTV.BO.Controllers
{
    [Authorize]
    public class ProgramacaoController : Controller
    {
        #region DayPilot Calendar
        class Dpc : DayPilot.Web.Mvc.DayPilotCalendar
        {
            public int ProgramacaoID = 0;

            //protected override void OnEventClick(EventClickArgs e)
            //{
            //    base.OnEventClick(e);
            //}

            protected override void OnEventResize(EventResizeArgs e)
            {
                var programacaoAgendamento = new Core.ProgramacaoAgendamento(ControladorSite.Utilizador);
                var agendamento = programacaoAgendamento.Abrir(long.Parse(e.Id));
                agendamento.DiaSemana = Core.Utils.Util.AcertoDiaSemana((short)e.NewStart.DayOfWeek);
                agendamento.Inicio = e.NewStart.TimeOfDay;
                if (e.NewEnd.TimeOfDay == new TimeSpan(0, 0, 0))
                    agendamento.Fim = new TimeSpan(23, 59, 59);
                else
                    agendamento.Fim = e.NewEnd.TimeOfDay;

                programacaoAgendamento.Actualizar(agendamento);

                Update();
            }
           
            protected override void OnEventMove(EventMoveArgs e)
            {
                var programacaoAgendamento = new Core.ProgramacaoAgendamento(ControladorSite.Utilizador);

                if (e.OldStart.Date != DateTime.Now.Date)
                {
                    var agendamento = programacaoAgendamento.Abrir(long.Parse(e.Id));
                    agendamento.DiaSemana = Core.Utils.Util.AcertoDiaSemana((short)e.NewStart.DayOfWeek);
                    agendamento.Inicio = e.NewStart.TimeOfDay;
                    if (e.NewEnd.TimeOfDay == new TimeSpan(0, 0, 0))
                        agendamento.Fim = new TimeSpan(23, 59, 59);
                    else
                        agendamento.Fim = e.NewEnd.TimeOfDay;

                    programacaoAgendamento.Actualizar(agendamento);
                }
                //else // external drag&drop desativo
                //{
                //    var agendamento = new Core.Model.ProgramacaoAgendamento();
                //    agendamento.ConteudoID = long.Parse(e.Id);

                //    agendamento.ProgramacaoID = this.ProgramacaoID;
                //    agendamento.DiaSemana = (short)e.NewStart.DayOfWeek;
                //    agendamento.Inicio = e.NewStart.TimeOfDay;

                //    if (e.NewEnd.TimeOfDay == new TimeSpan(0, 0, 0))
                //        agendamento.Fim = new TimeSpan(23, 59, 59);
                //    else
                //        agendamento.Fim = e.NewEnd.TimeOfDay;

                //    programacaoAgendamento.Inserir(agendamento);
                //}

                Update();
            }

            protected override void OnTimeRangeSelected(TimeRangeSelectedArgs e)
            {
                Core.ProgramacaoAgendamento obj = new Core.ProgramacaoAgendamento(ControladorSite.Utilizador);
                var registo = new Models.ScheduleModel();
                var newobj = obj.Inserir(registo.ToBDModel(e));

                Update();
            }

            protected override void OnInit(InitArgs e)
            {
                var lista = new Core.ProgramacaoAgendamento(ControladorSite.Utilizador).Lista(this.ProgramacaoID);

                List<Models.ScheduleModel> modelList = new List<Models.ScheduleModel>();

                foreach (var item in lista)
                    modelList.Add(new Models.ScheduleModel(item));

                Events = modelList;

                DataIdField = "id";
                DataTextField = "text";
                DataStartField = "eventstart";
                DataEndField = "eventend";

                Update();
            }

            protected override void OnFinish()
            {
                if (UpdateType == CallBackUpdateType.None)
                {
                    return;
                }

                var lista = new Core.ProgramacaoAgendamento(ControladorSite.Utilizador).Lista(this.ProgramacaoID);

                List<Models.ScheduleModel> modelList = new List<Models.ScheduleModel>();

                foreach (var item in lista)
                    modelList.Add(new Models.ScheduleModel(item));

                Events = modelList;

                DataIdField = "id";
                DataTextField = "text";
                DataStartField = "eventstart";
                DataEndField = "eventend";
            }

            protected override void OnCommand(CommandArgs e)
            {
                switch (e.Command)
                {
                    case "navigate":
                        StartDate = (DateTime)e.Data["start"];
                        Update(CallBackUpdateType.Full);
                        break;

                    case "refresh":
                        Update(CallBackUpdateType.Full);
                        break;

                    case "selected":
                        //if (SelectedEvents.Count > 0)
                        //{
                        //    EventInfo ei = SelectedEvents[0];
                        //    SelectedEvents.RemoveAt(0);
                        //    UpdateWithMessage("Event removed from selection: " + ei.Text);
                        //}

                        break;

                    case "delete":
                        //string id = (string)e.Data["id"];
                        //new EventManager(Controller).EventDelete(id);
                        Update(CallBackUpdateType.EventsOnly);
                        break;

                    case "previous":
                        StartDate = StartDate.AddDays(-7);
                        Update(CallBackUpdateType.Full);
                        break;

                    case "next":
                        StartDate = StartDate.AddDays(7);
                        Update(CallBackUpdateType.Full);
                        break;

                    case "today":
                        StartDate = DateTime.Today;
                        Update(CallBackUpdateType.Full);
                        break;
                }
            }

        }

        #endregion

        #region Propriedades

        public List<Core.Enum.Permissao> Permissoes
        {
            get
            {
                return new Core.Programacao(ControladorSite.Utilizador).Permissoes;
            }
        }

        //public static List<string> DaysOfWeekList
        //{
        //    get
        //    {
        //        return (new int[] { 0, 1, 2, 3, 4, 5, 6 }.Select(i => ((DayOfWeek)i).ToString())).ToList();
        //    }
        //}

         
        public static IEnumerable<SelectListItem> ConstroiListaDiasSemana
        {
            get
            {
                List<SelectListItem> lista = new List<SelectListItem>();

                foreach (var day in Enum.GetValues(typeof(DayOfWeek)))
                    lista.Add(new SelectListItem() { Text = System.Globalization.CultureInfo.CurrentCulture.DateTimeFormat.GetDayName((DayOfWeek)day), Value = (Core.Utils.Util.AcertoDiaSemana((int)day).ToString()) });

                return lista.OrderBy(l => l.Value);
            }
        }

        public static IEnumerable<SelectListItem> ConstroiListaHoras
        {
            get
            {
                List<SelectListItem> lista = new List<SelectListItem>();

                var start = new TimeSpan(0,0,0);
                var clockQuery = from offset in Enumerable.Range(0, 48)
                                 select TimeSpan.FromMinutes(30 * offset);

                foreach (var time in clockQuery)
                    lista.Add(new SelectListItem() { Text = (start + time).ToString("hh\\:mm"), Value = (start + time).ToString("hh\\:mm") });

                lista.Add(new SelectListItem() { Text = (start).ToString("hh\\:mm"), Value = (new TimeSpan(23,59,0)).ToString("hh\\:mm") });

                return lista;
            }
        }

        

        #endregion Propriedades

        #region Acções Programacao

        [HttpPost]
        [ValidateInput(false)]
        public ActionResult Gravar(Models.ProgramacaoModel registo)
        {
            try
            {                

                if (ModelState.IsValid)
                {
                    Core.Programacao obj = new Core.Programacao(ControladorSite.Utilizador);

                    string aftersave = "";

                    if (registo.ProgramacaoID.HasValue)
                    {
                        obj.Actualizar(registo.ToBDModel());
                    }
                    else
                    {
                        var newobj = obj.Inserir(registo.ToBDModel());
                        aftersave = String.Format("CallDetailAfterSave({0});", newobj.ProgramacaoID);
                    }                   

                    return JavaScript(Util.JavascriptAccao(true, Resources.Geral.GravarSucesso, "success", Url.Action("_Lista")) + aftersave);
                }
                else
                    throw new Exception(Resources.Erro.ModeloInvalido);
            }
            catch (Core.Exceptions.UserNameRepetido)
            {
                return JavaScript(Util.JavascriptAccao(false, Resources.Erro.UserNameRepetido, "error"));
            }
            catch (Exception ex)
            {
                Core.Utils.Util.TratamentoErro(null, GetType().FullName, ex, ControladorSite.Utilizador);
                return JavaScript(Util.JavascriptAccao(true, Resources.Erro.Geral, "error"));
            }
        }

        public ActionResult Apagar(int id)
        {
            try
            {
                if (id > 0)
                {
                    if (!Permissoes.Contains(Core.Enum.Permissao.Apagar))
                        throw new Core.Exceptions.SemPermissao();

                    new Core.Programacao(ControladorSite.Utilizador).Apagar(id);
                    return JavaScript(Util.JavascriptAccao(true, Resources.Geral.ApagarSucesso, "success", Url.Action("_Lista")));
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

        public ActionResult _Lista(Models.ProgramacaoFiltro filtro)
        {
            try
            {
                if (!Permissoes.Contains(Core.Enum.Permissao.Visualizar))
                    throw new Exception(Resources.Erro.Acesso);
                ViewData.Add("Permissoes", Permissoes);

                return PartialView(ConstroiLista(filtro.Pesquisa));
            }
            catch (Exception ex)
            {
                Core.Utils.Util.TratamentoErro(null, GetType().FullName, ex, ControladorSite.Utilizador);
                return JavaScript(Util.JavascriptAccao(true, Resources.Erro.Geral, "error"));
            }
        }

        #endregion Propriedades

        public ActionResult Index()
        {
            if (ControladorSite.Utilizador == null)
                return RedirectToAction("~/Utilizador/Login");

            if (!Permissoes.Contains(Core.Enum.Permissao.Visualizar))
            { 
                throw new Exception(Resources.Erro.Acesso);
            }

            ViewData.Add("Permissoes", Permissoes);
            return View(ConstroiLista(null));
        }

        public ActionResult _Detalhe(long? id)
        {
            try
            {
                if (!Permissoes.Contains(Core.Enum.Permissao.Visualizar))
                    throw new Exception(Resources.Erro.Acesso);

                ViewData.Add("Permissoes", Permissoes);

                var model = new Models.ProgramacaoModel();
                var programacao = new Core.Programacao(ControladorSite.Utilizador);

                if(id.HasValue)
                {
                    model = new Models.ProgramacaoModel(programacao.Abrir(id.Value));
                }
                else
                {
                    string semDesignacao = "Programação";
                    model = new Models.ProgramacaoModel();
                    model.Designacao = semDesignacao;
                    int i = 1;
                    while (programacao.VerificaDesignacaoRepetida(model.Designacao, ControladorSite.Empresa.ID))
                    {
                        model.Designacao = semDesignacao + " " + i;
                        ++i;
                    }

                    var newobj = programacao.Inserir(model.ToBDModel());
                    model.ProgramacaoID = newobj.ProgramacaoID;
                }

                ViewData.Add("ProgramacaoID", model.ProgramacaoID);
                ViewData.Add("ProgramacaoModel", model);
                //ViewBag.DaysOfWeek = DaysOfWeekList;

                return PartialView(model);
            }
            catch (Core.Exceptions.SemPermissao)
            {
                return JavaScript(Util.JavascriptAccao(false, Resources.Erro.Acesso, "error"));
            }
            catch (Exception ex)
            {
                Core.Utils.Util.TratamentoErro(null, GetType().FullName, ex, ControladorSite.Utilizador);
                return JavaScript(Util.JavascriptAccao(true, Resources.Erro.Geral, "error"));
            }
        }

        public ActionResult Backend(int pid)
        {
            var dpc = new Dpc();
            dpc.ProgramacaoID = pid;
            return dpc.CallBack(this);
        }

        [HttpPost]
        public ActionResult ApagarEvento(int pid)
        {
            var programacaoAgendamento = new Core.ProgramacaoAgendamento(ControladorSite.Utilizador);
            var agendamento = programacaoAgendamento.Abrir(pid);
            var programacaoID = agendamento.ProgramacaoID;
            programacaoAgendamento.Apagar(pid);

            var programacao = new Core.Programacao(ControladorSite.Utilizador);
            programacao.ActualizarData(programacaoID);

            Response.StatusCode = (int)System.Net.HttpStatusCode.OK;
            return Content("ok", System.Net.Mime.MediaTypeNames.Text.Plain);   
        }

        #region Funções Auxiliares

        private List<Models.ProgramacaoModel> ConstroiLista(string nome)
        {
            try
            {
                List<Models.ProgramacaoModel> modelList = new List<Models.ProgramacaoModel>();
                var lista = new Core.Programacao(ControladorSite.Utilizador).Lista(nome);

                foreach (var item in lista)
                    modelList.Add(new Models.ProgramacaoModel(item));

                return modelList;
            }
            catch (Exception ex)
            {
                Core.Utils.Util.TratamentoErro(null, GetType().FullName, ex, ControladorSite.Utilizador);
            }
            return new List<Models.ProgramacaoModel>();
        }

        public static IEnumerable<SelectListItem> ConstroiListaProgramacao()
        {
            List<SelectListItem> listaProgramacao = new List<SelectListItem>();
            //{
            //    new SelectListItem
            //    {
            //        Selected = false,
            //        Text = Resources.Geral.ListaOpcaoVazia,
            //        Value = string.Empty
            //    }
            //};

            List<Models.ProgramacaoModel> modelList = new List<Models.ProgramacaoModel>();
            var lista = new Core.Programacao(ControladorSite.Utilizador).Lista();

            foreach (var item in lista)
            {
                listaProgramacao.Add(new SelectListItem()
                {
                    Text = item.Designacao,
                    Value = item.ProgramacaoID.ToString()
                });
            }

            return listaProgramacao;
        }


        public static string ObterPerfil(int perfil)
        {
            string s = Enum.GetNames(typeof(Core.Enum.PerfilUtilizador))[perfil - 1];
            return Enum.GetNames(typeof(Core.Enum.PerfilUtilizador))[perfil - 1];
        }

        #endregion

    }
}
