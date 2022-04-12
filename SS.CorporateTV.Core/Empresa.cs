using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;

namespace SS.CorporateTV.Core
{
    public class EmpresaAtiva
    {
        public long ID { get; set; }
        public string Designacao { get; set; }
        public string Url { get; set; }
        public string Logo { get; set; }
        public string Cor1 { get; set; }
        public string Cor2 { get; set; }
        public string Cor3 { get; set; }

        public EmpresaAtiva(Model.Empresa obj)
        {
            this.ID = obj.EmpresaID;
            this.Designacao = obj.Designacao;
            this.Url = obj.Url;
            this.Logo = obj.Logo;
            this.Cor1 = obj.Cor1;
            this.Cor2 = obj.Cor2;
            this.Cor3 = obj.Cor3;
        }

    }

    public class Empresa : _Base<Model.Empresa>
    {
        public Empresa(UtilizadorAutenticado utilizador)
            : base(utilizador)
        {
        }

        public EmpresaAtiva AbrirEmpresa(string host)
        {
            using (Model.Context ctx = new Model.Context())
            {

                Model.Empresa empresa = ctx.Empresa
                    .FirstOrDefault(u => u.Url == host);

                if (empresa != null)
                    return new EmpresaAtiva(empresa);

                return null;
            }
        }

        #region Métodos auxiliares

        internal override bool ObjectoValido(Model.Empresa obj)
        {
            if (VerificaDesignacaoRepetida(obj))
                throw new Exceptions.UserNameRepetido();

            return !string.IsNullOrEmpty(obj.Designacao)
                && !string.IsNullOrEmpty(obj.Url)
                && obj.Activo;
        }

        private bool VerificaDesignacaoRepetida(Model.Empresa obj)
        {
            using (Model.Context ctx = new Model.Context())
            {
                return ctx.Empresa.Any(i =>
                    (obj.EmpresaID < 1 || i.EmpresaID != obj.EmpresaID)
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
