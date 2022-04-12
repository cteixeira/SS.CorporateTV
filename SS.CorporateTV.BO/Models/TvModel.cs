using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Core = SS.CorporateTV.Core;

namespace SS.CorporateTV.BO.Models
{
    public class TvModel
    {
        public long? ID { get; set; }

        [DisplayLocalizado(typeof(Resources.GestaoTv), "Designacao")]
        [RequiredLocalizado(typeof(Resources.GestaoTv), "Designacao")]
        [MaxStringLocalizado(200, typeof(Resources.GestaoTv), "Designacao")]
        public string Designacao { get; set; }

        [DisplayLocalizado(typeof(Resources.GestaoTv), "Programacao")]
        [RequiredLocalizado(typeof(Resources.GestaoTv), "Programacao")]
        public long? ProgramacaoID { get; set; }

        public string ProgramacaoDesignacao;

        public TvModel() { }

        public TvModel(Core.Model.Tv tv)
        {
            this.ID = tv.TvID;
            this.Designacao = tv.Designacao;
            this.ProgramacaoID = tv.ProgramacaoID;
            if (tv.Programacao != null)
                this.ProgramacaoDesignacao = tv.Programacao.Designacao;
        }

        public Core.Model.Tv ToBDModel()
        {
            Core.Model.Tv tv = new Core.Model.Tv();
            
            if (ControladorSite.Empresa != null)
                tv.EmpresaID = ControladorSite.Empresa.ID;

            if (ID.HasValue)
                tv.TvID = this.ID.Value;

            tv.Designacao = this.Designacao;

            if (ProgramacaoID.HasValue)
                tv.ProgramacaoID = this.ProgramacaoID.Value;

            return tv;
        }
    }

    public class TvFiltro
    {
        [DisplayLocalizado(typeof(Resources.GestaoTv), "Pesquisa")]
        public string Pesquisa { get; set; }
    }
}