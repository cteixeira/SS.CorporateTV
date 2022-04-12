using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SS.CorporateTV.Core
{
    public class ProgramacaoAgendamento : _Base<Model.ProgramacaoAgendamento>
    {
        public ProgramacaoAgendamento(UtilizadorAutenticado utilizador)
            : base(utilizador) {
        }

        public IEnumerable<Model.ProgramacaoAgendamento> Lista(long programacaoID)
        {
            if (!Permissoes.Contains(Enum.Permissao.Visualizar))
                throw new Exceptions.SemPermissao();

            using (Model.Context ctx = new Model.Context())
            {
                var query = ctx.ProgramacaoAgendamento
                    .Include("Conteudo")
                    .Where(u => u.ProgramacaoID == programacaoID)
                    .AsQueryable();

                return query
                    .OrderBy(p => p.DiaSemana)
                    .ToList();
            }
        }

        public override Model.ProgramacaoAgendamento Inserir(Model.ProgramacaoAgendamento obj)
        {
            if (!Permissoes.Contains(Enum.Permissao.Gravar))
                throw new Exceptions.SemPermissao();

            bool existeSobreposicao = ExisteSobreposicao(obj);

            if (!existeSobreposicao)
            {
                Core.Programacao objProg = new Core.Programacao(utilizador);
                objProg.ActualizarData(obj.ProgramacaoID);

                return base.Inserir(obj);
            }
            
            return null;
        }

        public void ApagarAgendamentos(long programacaoID)
        {
            if (!Permissoes.Contains(Enum.Permissao.Apagar))
                throw new Exceptions.SemPermissao();

            using (Model.Context db = new Model.Context())
            {
                var query = db.ProgramacaoAgendamento
                    .Where(u => u.ProgramacaoID == programacaoID)
                    .AsQueryable();

                foreach (var prg in query)
                {
                    this.Apagar(prg.ProgramacaoAgendamentoID);
                }

                db.SaveChanges(utilizador, true);

                Core.Programacao obj = new Core.Programacao(utilizador);
                obj.ActualizarData(programacaoID);
            }
        }

        public override void Actualizar(Model.ProgramacaoAgendamento obj)
        {
            if (!Permissoes.Contains(Enum.Permissao.Gravar))
                throw new Exceptions.SemPermissao();

            bool existeSobreposicao = ExisteSobreposicao(obj);

            if (!existeSobreposicao)
            {
                base.Actualizar(obj);
                Core.Programacao objProg = new Core.Programacao(utilizador);
                objProg.ActualizarData(obj.ProgramacaoID);
            }
        }


        public Model.ProgramacaoAgendamento AbrirAgendamentoNoAr(long programacaoID)
        {
            if (!Permissoes.Contains(Enum.Permissao.Visualizar))
                throw new Exceptions.SemPermissao();

            using (Model.Context ctx = new Model.Context())
            {
                //Obter o conteúdo a ser transmitido neste momento
                DateTime now = DateTime.Now;
                int diaSemana = Core.Utils.Util.AcertoDiaSemana((short)now.DayOfWeek);
                var query = ctx.ProgramacaoAgendamento
                    .Include("Conteudo")
                    .Include("Programacao")
                    .Where(u => u.ProgramacaoID == programacaoID 
                        && u.DiaSemana == diaSemana
                        && u.Inicio < now.TimeOfDay && u.Fim >= now.TimeOfDay)
                    .AsQueryable();

                var agendamento = query.FirstOrDefault();

                if (agendamento != null)
                    return agendamento;
                else
                {
                    //Caso não exista conteúdo neste momento, preencher o Fim com o o inicio do próximo conteúdo
                    agendamento = ctx.ProgramacaoAgendamento
                       .Include("Conteudo")
                       .Include("Programacao")
                       .Where(u => u.ProgramacaoID == programacaoID
                           && u.DiaSemana == (short)now.DayOfWeek 
                           && u.Inicio >= now.TimeOfDay)
                        .OrderBy(u => u.Inicio)
                       .FirstOrDefault();

                    if (agendamento != null)
                    {
                        agendamento.Conteudo.Tipo = 0;
                        agendamento.Fim = agendamento.Inicio;
                    }
                    else
                    {
                        //Não existem mais conteúdos agendados neste dia; preencher o Fim com o último segundo do dia
                        agendamento = new Model.ProgramacaoAgendamento();
                        agendamento.Programacao = new Core.Programacao(utilizador).Abrir(programacaoID);
                        agendamento.Conteudo = new Model.Conteudo();
                        agendamento.Conteudo.Tipo = 0;
                        agendamento.Fim = new TimeSpan(23, 59, 59);
                    }
                    return agendamento;
                }
            }
        }

        public Model.ProgramacaoAgendamento AbrirAgendamentoASeguir(long programacaoID, long agendamentoID)
        {
            //if (!Permissoes.Contains(Enum.Permissao.Visualizar))
            //    throw new Exceptions.SemPermissao();

            using (Model.Context ctx = new Model.Context())
            {
                if (agendamentoID > 0)
                {
                    var agendamento = this.Abrir(agendamentoID);

                    var agendamentoASeguir = ctx.ProgramacaoAgendamento
                        .Include("Conteudo")
                        .Where(u => u.ProgramacaoID == agendamento.ProgramacaoID
                            && u.DiaSemana == agendamento.DiaSemana
                            && u.Inicio >= agendamento.Fim)
                        .OrderBy(u => u.Inicio)
                        .FirstOrDefault();

                    if (agendamentoASeguir == null)
                        agendamentoASeguir = ctx.ProgramacaoAgendamento
                        .Include("Conteudo")
                        .Where(u => u.ProgramacaoID == agendamento.ProgramacaoID
                            && agendamento.DiaSemana == 6 ? u.DiaSemana >= 0 : u.DiaSemana > agendamento.DiaSemana)
                        .OrderBy(u => u.Inicio)
                        .FirstOrDefault();

                    return agendamentoASeguir;
                }
                else
                {
                    DateTime now = DateTime.Now;

                    var agendamentoASeguir = ctx.ProgramacaoAgendamento
                        .Include("Conteudo")
                        .Where(u => u.ProgramacaoID == programacaoID
                            && u.DiaSemana == (short)now.DayOfWeek
                            && u.Inicio >= now.TimeOfDay)
                        .OrderBy(u => u.Inicio)
                        .FirstOrDefault();

                    return agendamentoASeguir;

                }
            }
        }

        #region Métodos auxiliares

        internal bool ObjectoValidoInsercao(Model.ProgramacaoAgendamento obj)
        {
            return obj.ProgramacaoID != null;
        }

        internal override bool ObjectoValido(Model.ProgramacaoAgendamento obj)
        {

            return obj.ProgramacaoID != null;
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


        internal bool ExisteSobreposicao(Model.ProgramacaoAgendamento obj)
        {
            bool existeSobreposicao = false;

            using (Model.Context ctx = new Model.Context())
            {
                existeSobreposicao = ctx.ProgramacaoAgendamento
                    .Where(u => u.ProgramacaoAgendamentoID != obj.ProgramacaoAgendamentoID && u.ProgramacaoID == obj.ProgramacaoID && u.DiaSemana == obj.DiaSemana &&
                        ((u.Inicio < obj.Inicio && u.Fim > obj.Inicio) || (u.Inicio < obj.Fim && u.Fim > obj.Fim)  || (obj.Inicio < u.Inicio && obj.Fim > u.Inicio)
                        || (obj.Inicio == u.Inicio) || (obj.Fim == u.Fim) )) 
                    .Any();
            }
            return existeSobreposicao;
        }

        #endregion Métodos auxiliares
    }
}
