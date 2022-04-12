using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;

namespace SS.CorporateTV.Core
{
    public class ConteudoVideo : _Base<Model.ConteudoVideo>
    {
        public ConteudoVideo(UtilizadorAutenticado utilizador)
            : base(utilizador) 
        {
        }
        
        public IEnumerable<Model.ConteudoVideo> Lista(long ConteudoID)
        {
            if (!Permissoes.Contains(Enum.Permissao.Visualizar))
                throw new Exceptions.SemPermissao();

            using (Model.Context ctx = new Model.Context())
            {
                var query = ctx.ConteudoVideo
                    .Where(u => u.ConteudoID == ConteudoID)
                    .AsQueryable();

                return query
                    .OrderBy(p => !p.Ordem.HasValue)
                    .ThenBy(p => p.Ordem) //NULL no fim
                    .ToList(); 
            }
        }

        public override Model.ConteudoVideo Inserir(Model.ConteudoVideo obj)
        {
            if (!Permissoes.Contains(Enum.Permissao.Gravar))
                throw new Exceptions.SemPermissao();

            if (!ObjectoValido(obj))
                throw new Exceptions.DadosIncorrectos();

            using (Model.Context db = new Model.Context())
            {
                DbSet<Model.ConteudoVideo> table = db.Set<Model.ConteudoVideo>();
                table.Add(obj);
                db.SaveChanges();

                return obj;
            }
        }

        #region Métodos auxiliares

        internal bool ObjectoValidoInsercao(Model.ConteudoVideo obj)
        {
            if (VerificaDesignacaoRepetida(obj))
                throw new Exceptions.DesignacaoRepetida();

            return !string.IsNullOrEmpty(obj.Url);
        }

        internal override bool ObjectoValido(Model.ConteudoVideo obj)
        {
            if (VerificaDesignacaoRepetida(obj))
                throw new Exceptions.DesignacaoRepetida();

            return !string.IsNullOrEmpty(obj.Designacao);
        }

        private bool VerificaDesignacaoRepetida(Model.ConteudoVideo obj)
        {
            using (Model.Context ctx = new Model.Context())
            {
                return ctx.ConteudoVideo.Any(i =>
                    (obj.ConteudoVideoID < 1 || i.ConteudoVideoID != obj.ConteudoVideoID)
                    && (i.ConteudoID == obj.ConteudoID)
                    && (i.Designacao == obj.Designacao));
            }
        }

        internal override List<Enum.Permissao> ConstroiPermissoes()
        {
            if (utilizador.Perfil.Equals((int)Enum.PerfilUtilizador.SysAdmin)
                || utilizador.Perfil.Equals((int)Enum.PerfilUtilizador.Configurador))
            {
                return new List<Enum.Permissao>() { Enum.Permissao.Visualizar, Enum.Permissao.Gravar, Enum.Permissao.Apagar };
            }

            return new List<Enum.Permissao>() { Enum.Permissao.Visualizar };
        }

        #endregion Métodos auxiliares
    }
}
