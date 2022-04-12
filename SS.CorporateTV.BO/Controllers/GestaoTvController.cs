using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using SS.CorporateTV.BO.Models;

namespace SS.CorporateTV.BO.Controllers
{
    [Authorize]
    public class GestaoTvController : Controller
    {
        #region Propriedades

        private const int takeIni = 50;
        private const int takeNext = 10;

        public List<Core.Enum.Permissao> Permissoes
        {
            get
            {
                return new Core.TV(ControladorSite.Utilizador).Permissoes;
            }
        }

        #endregion

        public ActionResult Index()
        {
            try
            {
                if (ControladorSite.Utilizador == null)
                    throw new Exception(Resources.Erro.Acesso);

                if (!Permissoes.Contains(Core.Enum.Permissao.Visualizar))
                {   
                    throw new Exception(Resources.Erro.Acesso);
                }

                ViewData.Add("Permissoes", Permissoes);
                return View(ConstroiLista(null));
            }
            catch (Exception ex)
            {
                Core.Utils.Util.TratamentoErro(null, GetType().FullName, ex, ControladorSite.Utilizador);
                return View("Erro", ex);
            }
        }

        public ActionResult _Lista(Models.TvFiltro filtro)
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

                var model = new Models.TvModel();
                var tv = new Core.TV(ControladorSite.Utilizador);

                model = id.HasValue ? new Models.TvModel(tv.Abrir(id.Value)) : new Models.TvModel();

                ViewData.Add("TvModel", model);

                return PartialView(model);
            }
            catch (Core.Exceptions.SemPermissao)
            {
                return JavaScript(Util.JavascriptAccao(false, Resources.Erro.Acesso, "error"));
            }
            catch (Exception ex)
            {
                Core.Utils.Util.TratamentoErro(null, GetType().FullName, ex, ControladorSite.Utilizador);
                return JavaScript(Util.JavascriptAccao(true, Resources.Erro.Geral, "error"));
            }
        }

        public ActionResult Apagar(long id)
        {
            try
            {
                if (!Permissoes.Contains(Core.Enum.Permissao.Apagar))
                    throw new Core.Exceptions.SemPermissao();

                new Core.TV(ControladorSite.Utilizador).Apagar(id);
                return JavaScript(Util.JavascriptAccao(true, Resources.Geral.ApagarSucesso, "success", Url.Action("_Lista")));
            }
            catch (Exception ex)
            {
                Core.Utils.Util.TratamentoErro(null, GetType().FullName, ex, ControladorSite.Utilizador);
                return JavaScript(Util.JavascriptAccao(true, Resources.Erro.Geral, "error"));
            }

        }

        [HttpPost]
        [ValidateInput(false)]
        public ActionResult Gravar(Models.TvModel registo)
        {
            try
            {
                if (!Permissoes.Contains(Core.Enum.Permissao.Visualizar))
                    throw new Exception(Resources.Erro.Acesso);

                if (ModelState.IsValid)
                {
                    Core.TV obj = new Core.TV(ControladorSite.Utilizador);

                    string aftersave = "";

                    if (registo.ID.HasValue)
                    {
                        obj.Actualizar(registo.ToBDModel());
                    }
                    else
                    {
                        var newobj = obj.Inserir(registo.ToBDModel());
                        aftersave = String.Format("CallDetailAfterSave({0});", newobj.TvID);
                    }

                    return JavaScript(Util.JavascriptAccao(true, Resources.Geral.GravarSucesso, "success", Url.Action("_Lista")) + aftersave);
                }
                else
                    throw new Exception(Resources.Erro.ModeloInvalido);
            }
            catch (Core.Exceptions.DesignacaoRepetida)
            {
                return JavaScript(Util.JavascriptAccao(false, Resources.Erro.DesignacaoRepetida, "error"));
            }
            catch (Exception ex)
            {
                Core.Utils.Util.TratamentoErro(null, GetType().FullName, ex, ControladorSite.Utilizador);
                return JavaScript(Util.JavascriptAccao(true, Resources.Erro.Geral, "error"));
            }
        }

        #region Funcões Auxiliares

        private List<Models.TvModel> ConstroiLista(string nome)
        {
            try
            {
                List<Models.TvModel> modelList = new List<Models.TvModel>();
                var lista = new Core.TV(ControladorSite.Utilizador).Lista(nome);

                foreach (var item in lista)
                    modelList.Add(new Models.TvModel(item));

                return modelList;
            }
            catch (Exception ex)
            {
                Core.Utils.Util.TratamentoErro(null, GetType().FullName, ex, ControladorSite.Utilizador);
                return new List<Models.TvModel>();
            }
        }

        #endregion
    }
}
