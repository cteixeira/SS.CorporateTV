using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using SS.CorporateTV.BO.Models;

namespace SS.CorporateTV.BO.Controllers
{
    [Authorize]
    public class UtilizadorController : Controller
    {
        #region Propriedades

        private const int takeIni = 50;
        private const int takeNext = 10;

        public List<Core.Enum.Permissao> Permissoes
        {
            get
            {
                return new Core.Utilizador(ControladorSite.Utilizador).Permissoes;
            }
        }

        #endregion

        #region Login/Logout

        [AllowAnonymous]
        public ActionResult Login()
        {
            if (User != null && User.Identity.IsAuthenticated)
                return RedirectToRoute("Default");

            if (ControladorSite.Empresa == null)
                return Redirect(Util.LinkProduto);

            if (Util.MaqProd)
                return View(new Models.Login());
            else
                return View("LoginAdministrativo", new Models.LoginAdministrativo());
        }

        [AllowAnonymous]
        [HttpPost]
        public ActionResult Login(Models.Login model, string returnUrl)
        {
            if (!Util.MaqProd)
                return RedirectToAction("Login");

            if (ControladorSite.Empresa == null)
                return Redirect(Util.LinkProduto);

            try
            {
                if (ModelState.IsValid)
                {
                    string dominio = Util.MaqProd ? Util.Domain : Server.MachineName;

                    SimpleSolutions.wcSecure.Cifra cifra = new SimpleSolutions.wcSecure.Cifra();

                    ControladorSite.Utilizador = new Core.Utilizador(null).Autenticar(model.UserName, cifra.Cifrar(model.Password), dominio, ControladorSite.Empresa.ID);

                    if (ControladorSite.Utilizador != null)
                    {
                        FormsAuthentication.SetAuthCookie(ControladorSite.Utilizador.ID.ToString(), model.RememberMe);

                        if (Url.IsLocalUrl(returnUrl))
                            return Redirect(returnUrl);
                        else
                            return RedirectToRoute("Home");
                    }
                    else
                    {
                        ModelState.AddModelError("", Resources.Utilizador.UtilizadorInvalido);
                        return View(model);
                        //throw new Core.Exceptions.DadosIncorrectos();
                    }
                }
                else
                    throw new Core.Exceptions.DadosIncorrectos();
            }
            catch (Core.Exceptions.DadosIncorrectos)
            {
                ModelState.AddModelError("", Resources.Erro.DadosIncorrectos);
                return View(model);
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", Resources.Erro.Geral);
                Core.Utils.Util.TratamentoErro(null, GetType().FullName, ex, null);
                return View(model);
            }
        }

        [AllowAnonymous]
        [HttpPost]
        public ActionResult LoginAdministrativo(Models.LoginAdministrativo model, string returnUrl)
        {
            if (Util.MaqProd)
                return RedirectToAction("Login");

            if (ControladorSite.Empresa == null)
                return Redirect(Util.LinkProduto);

            try
            {
                if (ModelState.IsValid)
                {

                    ControladorSite.Utilizador = new Core.Utilizador(null).AbrirUtilizadorAutenticado(model.UtilizadorSeleccionado.Value, ControladorSite.Empresa.ID);

                    if (ControladorSite.Utilizador != null)
                    {
                        FormsAuthentication.SetAuthCookie(ControladorSite.Utilizador.ID.ToString(), false);

                        if (Url.IsLocalUrl(returnUrl))
                            return Redirect(returnUrl);
                        else
                            return RedirectToRoute("Default");
                    }
                    else
                        ModelState.AddModelError("", Resources.Utilizador.UtilizadorInvalido);
                }
                else
                    ModelState.AddModelError("", Resources.Erro.DadosIncorrectos);
            }
            catch (Core.Exceptions.DadosIncorrectos)
            {
                ModelState.AddModelError("", Resources.Erro.DadosIncorrectos);
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", Resources.Erro.Geral);
                Core.Utils.Util.TratamentoErro(null, GetType().FullName, ex, null);
            }

            return View(model);
        }

        public ActionResult Sair()
        {
            FormsAuthentication.SignOut();
            return RedirectToAction("Login");
        }

        #endregion Login/Logout

        public ViewResult Index()
        {
            try
            {
                if (!Permissoes.Contains(Core.Enum.Permissao.Visualizar))
                    throw new Exception(Resources.Erro.Acesso);

                ViewData.Add("Permissoes", Permissoes);
                return View(ConstroiLista(null));
            }
            catch (Exception ex)
            {
                Core.Utils.Util.TratamentoErro(null, GetType().FullName, ex, ControladorSite.Utilizador);
                return View("Erro", ex);
            }
        }

        public ActionResult _Lista(Models.UtilizadorFiltro filtro)
        {
            try
            {
                if (!Permissoes.Contains(Core.Enum.Permissao.Visualizar))
                    throw new Exception(Resources.Erro.Acesso);

                ViewData.Add("Permissoes", Permissoes);
                return PartialView(ConstroiLista(filtro.Pesquisa));
            }
            catch (Exception ex)
            {
                Core.Utils.Util.TratamentoErro(null, GetType().FullName, ex, ControladorSite.Utilizador);
                return JavaScript(Util.JavascriptAccao(true, Resources.Erro.Geral, "error"));
            }
        }

        public ActionResult _Detalhe(long? id)
        {
            try
            {
                if (!Permissoes.Contains(Core.Enum.Permissao.Visualizar))
                    throw new Exception(Resources.Erro.Acesso);

                ViewData.Add("Permissoes", Permissoes);

                if (!id.HasValue)
                    return PartialView(new Models.UtilizadorModel());

                var utilizador = new Core.Utilizador(ControladorSite.Utilizador).Abrir(id.Value);

                return PartialView(new Models.UtilizadorModel(utilizador));
            }
            catch (Exception ex)
            {
                Core.Utils.Util.TratamentoErro(null, GetType().FullName, ex, ControladorSite.Utilizador);
                return JavaScript(Util.JavascriptAccao(true, Resources.Erro.Geral, "error"));
            }

        }

        [HttpPost]
        [ValidateInput(false)]
        public ActionResult Gravar(Models.UtilizadorModel registo)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    Core.Utilizador obj = new Core.Utilizador(ControladorSite.Utilizador);

                    string aftersave = "";

                    if (registo.ID.HasValue)
                    {
                        obj.Actualizar(registo.ToBDModel());
                    }
                    else
                    {
                        var newobj = obj.Inserir(registo.ToBDModel());
                        aftersave = String.Format("CallDetailAfterSave({0});", newobj.UtilizadorID);
                    }

                    return JavaScript(Util.JavascriptAccao(true, Resources.Geral.GravarSucesso, "success", Url.Action("_Lista")) + aftersave);
                }
                else
                    throw new Exception(Resources.Erro.ModeloInvalido);
            }
            catch (Core.Exceptions.UserNameRepetido)
            {
                return JavaScript(Util.JavascriptAccao(false, Resources.Erro.UserNameRepetido, "error"));
            }
            catch (Exception ex)
            {
                Core.Utils.Util.TratamentoErro(null, GetType().FullName, ex, ControladorSite.Utilizador);
                return JavaScript(Util.JavascriptAccao(true, Resources.Erro.Geral, "error"));
            }
        }


        public ActionResult Apagar(int id)
        {
            try
            {
                if (id > 0)
                {
                    if (!Permissoes.Contains(Core.Enum.Permissao.Apagar))
                        throw new Core.Exceptions.SemPermissao();

                    new Core.Utilizador(ControladorSite.Utilizador).Apagar(id);
                    return JavaScript(Util.JavascriptAccao(true, Resources.Geral.ApagarSucesso, "success", Url.Action("_Lista")));
                }
                else
                    throw new Exception(Resources.Erro.ModeloInvalido);
            }
            catch (Exception ex)
            {
                Core.Utils.Util.TratamentoErro(null, GetType().FullName, ex, ControladorSite.Utilizador);
                return JavaScript(Util.JavascriptAccao(true, Resources.Erro.Geral, "error"));
            }

        }

        public ActionResult DetalhePassword()
        {
            try
            {
                var model = new DetalhePasswordViewModel();
                model.UtilizadorID = ControladorSite.Utilizador.ID;
                model.UtilizadorNome = ControladorSite.Utilizador.Nome;

                return View(model);
            }
            catch (Exception ex)
            {
                Core.Utils.Util.TratamentoErro(null, GetType().FullName, ex, ControladorSite.Utilizador);
                return JavaScript(Util.JavascriptAccao(true, Resources.Erro.Geral, "error"));
            }
        }

        [HttpPost]
        public ActionResult GravarPassword(DetalhePasswordViewModel registo)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    Core.Utilizador obj = new Core.Utilizador(ControladorSite.Utilizador);

                    obj.AlterarPassword(registo.UtilizadorID, registo.PasswordAntiga, registo.PasswordNova, registo.ConfirmarPasswordNova);

                    return JavaScript(Util.JavascriptAccao(true, Resources.Geral.GravarSucesso, "success", Url.Action("_Lista")) + "$('form').trigger('reset');");
                }
                else
                    throw new Exception(Resources.Erro.ModeloInvalido);
            }
            catch (Core.Exceptions.DadosIncorrectos)
            {
                return JavaScript(Util.JavascriptAccao(true, Resources.Erro.DadosIncorrectos, "error"));
            }
            catch (Core.Exceptions.NovaPasswordDiferenteConfirmacao)
            {
                return JavaScript(Util.JavascriptAccao(true, Resources.Erro.NovaPasswordDiferenteConfirmacao, "error"));
            }
            catch (Core.Exceptions.PasswordIncorrecta)
            {
                return JavaScript(Util.JavascriptAccao(true, Resources.Erro.PasswordIncorrecta, "error"));
            }
            catch (Exception ex)
            {
                Core.Utils.Util.TratamentoErro(null, GetType().FullName, ex, ControladorSite.Utilizador);
                return JavaScript(Util.JavascriptAccao(true, Resources.Erro.Geral, "error"));
            }
        }

        #region Funções Auxiliares

        private List<Models.UtilizadorModel> ConstroiLista(string nome)
        {
            try
            {
                List<Models.UtilizadorModel> modelList = new List<Models.UtilizadorModel>();
                var lista = new Core.Utilizador(ControladorSite.Utilizador).Lista(nome, ControladorSite.Empresa.ID, false);

                foreach (var item in lista)
                    modelList.Add(new Models.UtilizadorModel(item));

                return modelList;
            }
            catch (Exception ex)
            {
                Core.Utils.Util.TratamentoErro(null, "UtilizadorMestreController", ex, ControladorSite.Utilizador);
            }
            return new List<Models.UtilizadorModel>();
        }

        public static IEnumerable<SelectListItem> ConstroiListaPerfilUtilizadorDropdown()
        {
            try
            {
                List<SelectListItem> lista = new List<SelectListItem>();

                foreach (var en in Enum.GetValues(typeof(Core.Enum.PerfilUtilizador)))
                    lista.Add(new SelectListItem() { Text = en.ToString(), Value = ((int)en).ToString() });

                return lista;
            }
            catch (Exception ex)
            {
                Core.Utils.Util.TratamentoErro(null, "UtilizadorMestreController", ex, ControladorSite.Utilizador);

            }
            return new List<SelectListItem>();
        }


        public static string ObterPerfil(int perfil)
        {
            string s = Enum.GetNames(typeof(Core.Enum.PerfilUtilizador))[perfil - 1];
            return Enum.GetNames(typeof(Core.Enum.PerfilUtilizador))[perfil - 1];
        }

        #endregion

        #region Menu de Utilizador

        public static List<OpcaoMenu> ConstroiMenu()
        {
            List<OpcaoMenu> opcoes = new List<OpcaoMenu>();


            if (ControladorSite.Utilizador != null)
            {
                if (ControladorSite.Utilizador.Perfil == Convert.ToInt64(Core.Enum.PerfilUtilizador.SysAdmin))
                {
                    //AdicionaOpcaoMenu(opcoes, "Default", "Index", Resources.Menu.Home, "home");
                    AdicionaOpcaoMenu(opcoes, "Conteudo", "Index", Resources.Menu.Conteudos, "facetime-video");
                    AdicionaOpcaoMenu(opcoes, "Programacao", "Index", Resources.Menu.Programacao, "calendar");                    
                    AdicionaOpcaoMenu(opcoes, "GestaoTv", "Index", Resources.Menu.TVs, "hd-video");
                    AdicionaOpcaoMenu(opcoes, "Utilizador", "Index", Resources.Menu.Utilizadores, "user");
                    AdicionaOpcaoMenu(opcoes, "Utilizador", "DetalhePassword", Resources.Menu.MudarPassword, "user");

                }
                else if (ControladorSite.Utilizador.Perfil == Convert.ToInt64(Core.Enum.PerfilUtilizador.Configurador))
                {
                    AdicionaOpcaoMenu(opcoes, "Conteudo", "Index", Resources.Menu.Conteudos, "facetime-video");
                    AdicionaOpcaoMenu(opcoes, "Programacao", "Index", Resources.Menu.Programacao, "calendar");
                    AdicionaOpcaoMenu(opcoes, "GestaoTv", "Index", Resources.Menu.TVs, "hd-video");
                    AdicionaOpcaoMenu(opcoes, "Utilizador", "DetalhePassword", Resources.Menu.MudarPassword, "user");
                }

            }

            return opcoes;
        }

        private static void AdicionaOpcaoMenu(List<OpcaoMenu> opcoes, string controlador, string accao, string designacao, string icone)
        {
            if (!opcoes.Any(o => o.Controlador == controlador && o.Accao == accao))
            {
                OpcaoMenu opcao = new OpcaoMenu();
                opcao.Controlador = controlador;
                opcao.Accao = accao;
                opcao.Designacao = designacao;
                opcao.Icone = icone;
                opcoes.Add(opcao);
            }
        }

        public class OpcaoMenu
        {

            public string Controlador { get; set; }

            public string Accao { get; set; }

            public string Designacao { get; set; }

            public string Icone { get; set; }
        }

        #endregion Menu de Utilizador


    }
}
