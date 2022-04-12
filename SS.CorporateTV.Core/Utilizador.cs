using SimpleSolutions.wcSecure;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.Entity;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;

namespace SS.CorporateTV.Core
{
    public class UtilizadorAutenticado
    {

        public long ID { get; set; }
        public string UserName { get; set; }
        public string Nome { get; set; }
        public int Perfil { get; set; }
        public long EmpresaID { get; set; }

        public UtilizadorAutenticado(Model.Utilizador obj)
        {
            this.ID = obj.UtilizadorID;
            this.UserName = obj.Username;
            this.Nome = obj.Nome;
            this.Perfil = obj.PerfilUtilizador;
            this.EmpresaID = obj.EmpresaID;
        }

    }

    public class Utilizador : _Base<Model.Utilizador>
    {
        public Utilizador(UtilizadorAutenticado utilizador)
            : base(utilizador)
        {
        }

        public IEnumerable<Model.Utilizador> Lista(string nome, long empresaID, bool isForReadOnly)
        {
            if (!Permissoes.Contains(Enum.Permissao.Visualizar) && !isForReadOnly)
                throw new Exceptions.SemPermissao();

            using (Model.Context ctx = new Model.Context())
            {
                return ctx.Utilizador
                    .Where(u => (string.IsNullOrEmpty(nome) || u.Nome.Contains(nome)) && u.EmpresaID == empresaID
                        ).OrderBy(u => u.Nome).ToList();
            }
        }

        public const int LOGON32_LOGON_INTERACTIVE = 2;
        public const int LOGON32_PROVIDER_DEFAULT = 0;

        [DllImport("advapi32.dll", CharSet = CharSet.Auto)]
        public static extern int LogonUser(String lpszUserName,
                                           String lpszDomain,
                                           String lpszPassword,
                                           int dwLogonType,
                                           int dwLogonProvider,
                                           ref IntPtr phToken);


        public UtilizadorAutenticado AbrirUtilizadorAutenticado(long id, long empresaID)
        {
            using (Model.Context ctx = new Model.Context())
            {

                Model.Utilizador utilizador = ctx.Utilizador
                    .FirstOrDefault(u => u.UtilizadorID == id && u.EmpresaID == empresaID);

                if (utilizador != null)
                    return new UtilizadorAutenticado(utilizador);

                return null;
            }
        }

        public UtilizadorAutenticado Autenticar(string userName, string password, string domain, long empresaID)
        {
            using (Model.Context ctx = new Model.Context())
            {
                Model.Utilizador utilizador = ctx.Utilizador
                    .FirstOrDefault(i => i.Username == userName && i.Password == password && i.EmpresaID == empresaID);

                if (utilizador != null)
                    return new UtilizadorAutenticado(utilizador);

                return null;
            }
        }

        public void AlterarPassword(long utilizadorID, string passwordAntiga, string passwordNova, string confirmarPasswordNova)
        {
            using (Model.Context ctx = new Model.Context())
            {
                var user = ctx.Utilizador.FirstOrDefault(u => u.UtilizadorID == utilizadorID);

                if (user == null)
                    throw new Core.Exceptions.DadosIncorrectos();

                if (!new Decifra().Decifrar(user.Password).Equals(passwordAntiga))
                    throw new Core.Exceptions.PasswordIncorrecta();

                if (!passwordNova.Equals(confirmarPasswordNova))
                    throw new Core.Exceptions.NovaPasswordDiferenteConfirmacao();

                user.Password = new Cifra().Cifrar(passwordNova);

                DbSet<Model.Utilizador> table = ctx.Set<Model.Utilizador>();
                table.Attach(user);
                ctx.Entry(user).State = EntityState.Modified;
                ctx.SaveChanges();
            }
        }

        public override Model.Utilizador Inserir(Model.Utilizador obj)
        {
            obj.Password = new Cifra().Cifrar(obj.Password);
            return base.Inserir(obj);
        }

        #region Métodos auxiliares

        internal override bool ObjectoValido(Model.Utilizador obj)
        {
            if (VerificaUserNameRepetido(obj))
                throw new Exceptions.UserNameRepetido();

            return !string.IsNullOrEmpty(obj.Nome)
                && !string.IsNullOrEmpty(obj.Username)
                && obj.PerfilUtilizador > 0;
        }

        private bool VerificaUserNameRepetido(Model.Utilizador obj)
        {
            using (Model.Context ctx = new Model.Context())
            {
                return ctx.Utilizador.Any(i =>
                    (obj.UtilizadorID < 1 || i.UtilizadorID != obj.UtilizadorID)
                    && i.EmpresaID == utilizador.EmpresaID
                    && i.Username == obj.Username);
            }
        }
        
        internal override List<Enum.Permissao> ConstroiPermissoes()
        {
            if (utilizador != null)
            {
                if (utilizador.Perfil == (int)Enum.PerfilUtilizador.SysAdmin)
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
