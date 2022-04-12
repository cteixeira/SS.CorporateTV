using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;

namespace SS.CorporateTV.Core
{
    public class Programacao : _Base<Model.Programacao>
    {
        public Programacao(UtilizadorAutenticado utilizador)
            : base(utilizador) {
        }

        public override IEnumerable<Model.Programacao> Lista()
        {
            return Lista(null);
        }

        public IEnumerable<Model.Programacao> Lista(string nome)
        {
            if (!Permissoes.Contains(Enum.Permissao.Visualizar))
                throw new Exceptions.SemPermissao();

            using (Model.Context ctx = new Model.Context())
            {
                var query = ctx.Programacao
                    .Where(u => u.EmpresaID == utilizador.EmpresaID && (string.IsNullOrEmpty(nome) || u.Designacao.Contains(nome) ))
                    .AsQueryable();

                return query
                    .OrderBy(p => p.Designacao)
                    .ToList();
            }
        }

        public override void Apagar(long id)
        {
            if (!Permissoes.Contains(Enum.Permissao.Apagar))
                throw new Exceptions.SemPermissao();

            new ProgramacaoAgendamento(utilizador).ApagarAgendamentos(id);

            base.Apagar(id);
        }

        public void ActualizarData(long id)
        {
            if (!Permissoes.Contains(Enum.Permissao.Gravar))
                throw new Exceptions.SemPermissao();

            using (Model.Context db = new Model.Context())
            {
                DbSet<Model.Programacao> tabelaProgramacao = db.Set<Model.Programacao>();
                Model.Programacao programacao = tabelaProgramacao.Find(id);
                programacao.DataAlteracao = DateTime.Now;
                tabelaProgramacao.Attach(programacao);
                db.Entry(programacao).State = EntityState.Modified;
                db.SaveChanges(utilizador, true);
            }
        }

        #region Métodos auxiliares

        public bool ContemAgendamentos(long programacaoID)
        {
            if (!Permissoes.Contains(Enum.Permissao.Visualizar))
                throw new Exceptions.SemPermissao();

            using (Model.Context ctx = new Model.Context())
            {
                return ctx.Programacao
                    .Include("ProgramacaoAgendamento")
                    .Where(u => u.EmpresaID == utilizador.EmpresaID && u.ProgramacaoID == programacaoID && u.ProgramacaoAgendamento.Count > 0)
                    .Any();
            }
        }

        internal bool ObjectoValidoInsercao(Model.Programacao obj)
        {
            if (VerificaDesignacaoRepetida(obj))
                throw new Exceptions.DesignacaoRepetida();

            return !string.IsNullOrEmpty(obj.Designacao);
        }

        internal override bool ObjectoValido(Model.Programacao obj)
        {
            if (VerificaDesignacaoRepetida(obj))
                throw new Exceptions.DesignacaoRepetida();

            return !string.IsNullOrEmpty(obj.Designacao);
        }

        private bool VerificaDesignacaoRepetida(Model.Programacao obj)
        {
            using (Model.Context ctx = new Model.Context())
            {
                return ctx.Programacao.Any(i =>
                    (obj.ProgramacaoID < 1 || i.ProgramacaoID != obj.ProgramacaoID)
                    && (i.EmpresaID == obj.EmpresaID)
                    && (i.Designacao == obj.Designacao));
            }
        }

        public bool VerificaDesignacaoRepetida(string designacao, long empresaID)
        {
            using (Model.Context ctx = new Model.Context())
            {
                return ctx.Programacao.Any(i => (i.Designacao == designacao && i.EmpresaID == empresaID));
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
