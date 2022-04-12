using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace SS.CorporateTV.BO.Models
{
    public class Login
    {
        public Login()
        {
            RememberMe = true;
        }

        [DisplayLocalizado(typeof(Resources.Utilizador), "UserName")]
        [RequiredLocalizado(typeof(Resources.Utilizador), "UserName")]
        public string UserName { get; set; }

        [DisplayLocalizado(typeof(Resources.Utilizador), "Password")]
        [RequiredLocalizado(typeof(Resources.Utilizador), "Password")]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        [DisplayLocalizado(typeof(Resources.Utilizador), "Recordar")]
        public bool RememberMe { get; set; }
    }

    public class LoginAdministrativo
    {
        [RequiredLocalizado(typeof(Resources.Utilizador), "SeleccioneUtilizador")]
        [DisplayLocalizado(typeof(Resources.Utilizador), "SeleccioneUtilizador")]
        public int? UtilizadorSeleccionado { get; set; }
    }

    public class UtilizadorModel
    {
        public long? ID { get; set; }

        public long? EmpresaID { get; set; }

        [DisplayLocalizado(typeof(Resources.Utilizador), "Nome")]
        [RequiredLocalizado(typeof(Resources.Utilizador), "Nome")]
        [MaxStringLocalizado(200, typeof(Resources.Utilizador), "Nome")]
        public string Nome { get; set; }

        [DisplayLocalizado(typeof(Resources.Utilizador), "UserName")]
        [RequiredLocalizado(typeof(Resources.Utilizador), "UserName")]
        [MaxStringLocalizado(40, typeof(Resources.Utilizador), "UserName")]
        public string UserName { get; set; }

        [DisplayLocalizado(typeof(Resources.Utilizador), "Password")]
        [RequiredLocalizado(typeof(Resources.Utilizador), "Password")]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        [DisplayLocalizado(typeof(Resources.Utilizador), "Perfil")]
        [RequiredLocalizado(typeof(Resources.Utilizador), "Perfil")]
        public int PerfilUtilizador { get; set; }

        public string PerfilDesignacao { get; set; }

        [DisplayLocalizado(typeof(Resources.Utilizador), "Activo")]
        [RequiredLocalizado(typeof(Resources.Utilizador), "Activo")]
        public bool Ativo { get; set; }


        public UtilizadorModel()
        {
            Ativo = true;
        }
        public UtilizadorModel(Core.Model.Utilizador obj)
        {
            this.ID = obj.UtilizadorID;
            this.Nome = obj.Nome;
            this.UserName = obj.Username;
            this.Password = obj.Password;
            this.PerfilUtilizador = obj.PerfilUtilizador;
            this.PerfilDesignacao = Enum.GetName(typeof(Core.Enum.PerfilUtilizador), obj.PerfilUtilizador);
        }

        public Core.Model.Utilizador ToBDModel()
        {
            Core.Model.Utilizador obj = new Core.Model.Utilizador();
            if (ID.HasValue)
                obj.UtilizadorID = this.ID.Value;
            if (ControladorSite.Empresa != null)
                obj.EmpresaID = ControladorSite.Empresa.ID;
            obj.Username = this.UserName;
            obj.Password = Password;
            obj.Nome = this.Nome;
            obj.PerfilUtilizador = (short)this.PerfilUtilizador;
            return obj;
        }
    }

    public class UtilizadorFiltro
    {
        [DisplayLocalizado(typeof(Resources.Utilizador), "Pesquisa")]
        public string Pesquisa { get; set; }
    }

    public class DetalhePasswordViewModel
    {
        public long UtilizadorID { get; set; }

        public string UtilizadorNome { get; set; }

        [DisplayLocalizado(typeof(Resources.Utilizador), "PasswordAntiga")]
        [RequiredLocalizado(typeof(Resources.Utilizador), "PasswordAntiga")]
        public string PasswordAntiga { get; set; }

        [DisplayLocalizado(typeof(Resources.Utilizador), "PasswordNova")]
        [RequiredLocalizado(typeof(Resources.Utilizador), "PasswordNova")]
        public string PasswordNova { get; set; }

        [DisplayLocalizado(typeof(Resources.Utilizador), "ConfirmarPasswordNova")]
        [RequiredLocalizado(typeof(Resources.Utilizador), "ConfirmarPasswordNova")]
        public string ConfirmarPasswordNova { get; set; }
    }
}