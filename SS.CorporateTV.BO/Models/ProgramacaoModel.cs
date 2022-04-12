using DayPilot.Web.Mvc.Events.Calendar;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Security.Policy;
using System.Web;

namespace SS.CorporateTV.BO.Models
{
    public class ProgramacaoModel
    {
        #region Propriedades

        public long? ProgramacaoID { get; set; }

        [DisplayLocalizado(typeof(Resources.Programacao), "Designacao")]
        [RequiredLocalizado(typeof(Resources.Programacao), "Designacao")]
        [MaxStringLocalizado(200, typeof(Resources.Programacao), "Designacao")]
        public String Designacao { get; set; }

        [DisplayLocalizado(typeof(Resources.Programacao), "Conteudos")]
        public int? ConteudoSeleccionado { get; set; }

        public List<ConteudoModel> Conteudos { get; set; }

        public List<ProgramacaoAgendamentoModel> Agendamento { get; set; }

        #endregion

        #region Construtores

        public ProgramacaoModel()
        {
        }

        public ProgramacaoModel(Core.Model.Programacao obj)
        {
            this.ProgramacaoID = obj.ProgramacaoID;
            this.Designacao = obj.Designacao;
            var conteudos = new Core.Conteudo(ControladorSite.Utilizador).Lista(/*EmpresaID*/).ToList();
            Conteudos = new List<ConteudoModel>();
            Agendamento = new List<ProgramacaoAgendamentoModel>();

            if (conteudos != null && conteudos.Count > 0)
            {
                foreach (var item in conteudos)
                {
                    Conteudos.Add(new Models.ConteudoModel(item));
                }
            }

            var agendamento = new Core.ProgramacaoAgendamento(ControladorSite.Utilizador).Lista(this.ProgramacaoID.Value).ToList();

            if (agendamento != null && agendamento.Count > 0)
            {
                foreach (var item in agendamento)
                {
                    Agendamento.Add(new Models.ProgramacaoAgendamentoModel(item));
                }
            }
        }

        public Core.Model.Programacao ToBDModel()
        {
            Core.Model.Programacao obj = new Core.Model.Programacao();
            if (ProgramacaoID.HasValue)
                obj.ProgramacaoID = this.ProgramacaoID.Value;

            obj.Designacao = this.Designacao;
            obj.EmpresaID = ControladorSite.Empresa.ID;
            obj.DataAlteracao = DateTime.Now;
            return obj;
        }

        #endregion

    }

    public class ProgramacaoAgendamentoModel
    {
        #region Propriedades

        public long? ProgramacaoID { get; set; }

        [DisplayLocalizado(typeof(Resources.Programacao), "Conteudo")]
        [RequiredLocalizado(typeof(Resources.Programacao), "Conteudo")]
        public long ConteudoID { get; set; }

        public long? ProgramacaoAgendamentoID { get; set; }

        [DisplayLocalizado(typeof(Resources.Programacao), "DiaSemana")]
        [RequiredLocalizado(typeof(Resources.Programacao), "DiaSemana")]
        public int DiaSemana { get; set; }

        [DisplayLocalizado(typeof(Resources.Programacao), "Inicio")]
        [DisplayFormat(DataFormatString = "{0:hh\\:mm}", ApplyFormatInEditMode = true)]
        [RequiredLocalizado(typeof(Resources.Programacao), "Inicio")]
        [RegularExpression(@"^([0-9]|0[0-9]|1[0-9]|2[0-3]):[0-5][0-9]$", ErrorMessage = "Hora inválida.")]
        public TimeSpan? Inicio { get; set; }

        [DisplayLocalizado(typeof(Resources.Programacao), "Inicio")]
        [DisplayFormat(DataFormatString = "{0:hh\\:mm}", ApplyFormatInEditMode = true)]
        [RequiredLocalizado(typeof(Resources.Programacao), "Inicio")]
        [RegularExpression(@"^([0-9]|0[0-9]|1[0-9]|2[0-3]):[0-5][0-9]$", ErrorMessage = "Hora inválida.")]
        public string InicioStr { get; set; }

        [DisplayLocalizado(typeof(Resources.Programacao), "Fim")]
        [DisplayFormat(DataFormatString = "{0:hh\\:mm}", ApplyFormatInEditMode = true)]
        [RequiredLocalizado(typeof(Resources.Programacao), "Fim")]
        [RegularExpression(@"^([0-9]|0[0-9]|1[0-9]|2[0-3]):[0-5][0-9]$", ErrorMessage = "Hora inválida.")]
        public TimeSpan? Fim { get; set; }

        [DisplayLocalizado(typeof(Resources.Programacao), "Fim")]
        [DisplayFormat(DataFormatString = "{0:hh\\:mm}", ApplyFormatInEditMode = true)]
        [RequiredLocalizado(typeof(Resources.Programacao), "Fim")]
        [RegularExpression(@"^([0-9]|0[0-9]|1[0-9]|2[0-3]):[0-5][0-9]$", ErrorMessage = "Hora inválida.")]
        [TimeGreaterThan(typeof(Resources.Programacao), "HoraFim", "InicioStr")]
        public string FimStr { get; set; }
        #endregion

        public ProgramacaoAgendamentoModel()
        {
        }

        public ProgramacaoAgendamentoModel(Core.Model.ProgramacaoAgendamento model)
        {
            this.DiaSemana = model.DiaSemana;
            this.Fim = model.Fim;
            this.FimStr = model.Fim.ToString("hh\\:mm");
            this.Inicio = model.Inicio;
            this.InicioStr = model.Inicio.ToString("hh\\:mm");
            this.ProgramacaoAgendamentoID = model.ProgramacaoAgendamentoID;
        }
    }

    public class ProgramacaoFiltro
    {
        [DisplayLocalizado(typeof(Resources.Utilizador), "Pesquisa")]
        public string Pesquisa { get; set; }
    }

    public class ScheduleModel
    {
        public Nullable<int> id { get; set; }
        public string text { get; set; }
        public Nullable<System.DateTime> eventstart { get; set; }
        public Nullable<System.DateTime> eventend { get; set; }

        public ScheduleModel()
        {
        }

        public ScheduleModel(Core.Model.ProgramacaoAgendamento model)
        {
            DateTime diaSemana = new DateTime(2016, 01, 4).AddDays(model.DiaSemana);
            this.id = (int)model.ProgramacaoAgendamentoID;
            this.eventstart = diaSemana + model.Inicio;
            this.eventend = diaSemana + model.Fim;
            this.text = model.Conteudo.Designacao;
        }

       public Core.Model.ProgramacaoAgendamento ToBDModel(TimeRangeSelectedArgs registo)
       {
            Core.Model.ProgramacaoAgendamento agendamento = new Core.Model.ProgramacaoAgendamento();
            agendamento.DiaSemana = (short)registo.Start.DayOfWeek;
            agendamento.Inicio = registo.Start.TimeOfDay;
            agendamento.Fim = registo.End.TimeOfDay;
            agendamento.ProgramacaoID = long.Parse(registo.Data[1].ToString());

            return agendamento;
       }
    }
}