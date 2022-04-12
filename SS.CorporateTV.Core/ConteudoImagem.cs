using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;

namespace SS.CorporateTV.Core
{
    public class ConteudoImagem : _Base<Model.ConteudoImagem>
    {
        public ConteudoImagem(UtilizadorAutenticado utilizador)
            : base(utilizador) 
        {
        }
        
        public IEnumerable<Model.ConteudoImagem> Lista(long ConteudoID)
        {
            if (!Permissoes.Contains(Enum.Permissao.Visualizar))
                throw new Exceptions.SemPermissao();

            using (Model.Context ctx = new Model.Context())
            {
                var query = ctx.ConteudoImagem
                    .Where(u => u.ConteudoID == ConteudoID)
                    .AsQueryable();

                return query
                    .OrderBy(p => !p.Ordem.HasValue)
                    .ThenBy(p => p.Ordem) //NULL no fim
                    .ToList();
            }
        }

        public override Model.ConteudoImagem Inserir(Model.ConteudoImagem obj)
        {
            if (!Permissoes.Contains(Enum.Permissao.Gravar))
                throw new Exceptions.SemPermissao();

            if (!ObjectoValido(obj))
                throw new Exceptions.DadosIncorrectos();

            using (Model.Context db = new Model.Context())
            {
                Model.ConteudoImagem ficheiroRepetido = null;
                Model.ConteudoImagem ficheiroRetorno;
                if (ficheiroRepetido == null)
                {
                    DbSet<Model.ConteudoImagem> table = db.Set<Model.ConteudoImagem>();
                    table.Add(obj);
                    db.SaveChanges();
                    ficheiroRetorno = obj;
                }
                else
                {
                    ficheiroRetorno = ficheiroRepetido;
                }

                return ficheiroRetorno;
            }
        }

        #region Métodos auxiliares

        internal bool ObjectoValidoInsercao(Model.ConteudoImagem obj)
        {
            if (VerificaDesignacaoRepetida(obj))
                throw new Exceptions.DesignacaoRepetida();

            return !string.IsNullOrEmpty(obj.UrlImagem);
        }

        internal override bool ObjectoValido(Model.ConteudoImagem obj)
        {
            if (VerificaDesignacaoRepetida(obj))
                throw new Exceptions.DesignacaoRepetida();

            return !string.IsNullOrEmpty(obj.Designacao);
        }

        private bool VerificaDesignacaoRepetida(Model.ConteudoImagem obj)
        {
            using (Model.Context ctx = new Model.Context())
            {
                return ctx.ConteudoImagem.Any(i =>
                    (obj.ConteudoImagemID < 1 || i.ConteudoImagemID != obj.ConteudoImagemID)
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
