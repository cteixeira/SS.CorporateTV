using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;

namespace SS.CorporateTV.Core
{
    public class TV : _Base<Model.Tv>
    {
        public TV(UtilizadorAutenticado utilizador)
            : base(utilizador)
        {
        }

        public override Model.Tv Abrir(long id)
        {
            using (Model.Context ctx = new Model.Context())
            {
                Model.Tv tv = ctx.Tv
                    .Include("Programacao")
                    .FirstOrDefault(u => u.TvID == id);

                if (tv != null)
                    return tv;

                return null;
            }
        }
       
        public IEnumerable<Model.Tv> Lista(string nome)
        {
            if (!Permissoes.Contains(Enum.Permissao.Visualizar))
                throw new Exceptions.SemPermissao();

            using (Model.Context ctx = new Model.Context())
            {
                var query = ctx.Tv
                    .Include("Programacao")
                    .Where(u => u.EmpresaID == utilizador.EmpresaID && (string.IsNullOrEmpty(nome) || u.Designacao.Contains(nome)))
                    .AsQueryable();

                return query
                    .OrderBy(p => p.Designacao)
                    .ToList();
            }
        }

        public IEnumerable<Model.Tv> Lista(long empresaID)
        {
            if (!Permissoes.Contains(Enum.Permissao.Visualizar))
                throw new Exceptions.SemPermissao();

            using (Model.Context ctx = new Model.Context())
            {
                var query = ctx.Tv
                    .Where(u => u.EmpresaID == empresaID)
                    .AsQueryable();

                return query
                    .OrderBy(p => p.Designacao)
                    .ToList();
            }
        }

        #region Métodos auxiliares

        internal override bool ObjectoValido(Model.Tv obj)
        {
            if (VerificaDesignacaoRepetida(obj))
                throw new Exceptions.UserNameRepetido();

            return !string.IsNullOrEmpty(obj.Designacao);
        }

        private bool VerificaDesignacaoRepetida(Model.Tv obj)
        {
            using (Model.Context ctx = new Model.Context())
            {
                return ctx.Tv.Any(i =>
                    (obj.TvID < 1 || i.TvID != obj.TvID)
                    && i.EmpresaID == utilizador.EmpresaID
                    && i.Designacao == obj.Designacao);
            }
        }

        internal override List<Enum.Permissao> ConstroiPermissoes()
        {
            if (utilizador != null)
            {
                if (utilizador.Perfil.Equals((int)Enum.PerfilUtilizador.SysAdmin)
                || utilizador.Perfil.Equals((int)Enum.PerfilUtilizador.Configurador))
                {
                    return new List<Enum.Permissao>() { Enum.Permissao.Visualizar, 
                                                        Enum.Permissao.Gravar, 
                                                        Enum.Permissao.Apagar };
                }
                return new List<Enum.Permissao>();

            }

            //Caso máquina de testes login administrativo tem que ter lista de utilizadores
            if (!Convert.ToBoolean(ConfigurationManager.AppSettings["MaqProd"]))
                return new List<Enum.Permissao>() { Enum.Permissao.Visualizar, Enum.Permissao.Gravar };

            return new List<Enum.Permissao>();
        }

        #endregion Métodos auxiliares
    }
}
