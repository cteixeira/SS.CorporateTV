//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace SS.CorporateTV.Core.Model
{
    using System;
    using System.Collections.Generic;
    
    public partial class ConteudoVideo
    {
        public long ConteudoVideoID { get; set; }
        public long ConteudoID { get; set; }
        public string Designacao { get; set; }
        public string Url { get; set; }
        public Nullable<int> Duracao { get; set; }
        public Nullable<int> Ordem { get; set; }
    
        public virtual Conteudo Conteudo { get; set; }
    }
}