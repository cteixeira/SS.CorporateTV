using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Core = SS.CorporateTV.Core;

namespace SS.CorporateTV.BO.Models
{
    public class ConfigTvModel
    {
        public long ID { get; set; }

        public string Designacao { get; set; }

        public ConfigTvModel(Core.Model.Tv obj)
        {
            this.ID = obj.TvID;
            this.Designacao = obj.Designacao;
        }
    }
}