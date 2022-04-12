using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using SS.CorporateTV.BO;
using SS.CorporateTV.BO.Resources;
using SS.CorporateTV.BO.Web.ExtensionMethods;
using Core = SS.CorporateTV.Core;

namespace SS.CorporateTV.BO.Controllers
{
    [Authorize]
    public class ConfigTvController : Controller
    {
        public List<Core.Enum.Permissao> Permissoes
        {
            get
            {
                return new Core.TV(ControladorSite.Utilizador).Permissoes;
            }
        }
        public ActionResult Index()
        {
            try
            {
                if(ControladorSite.Utilizador == null)
                    Response.RedirectToRoute("Login?ReturnUrl=" + View("GestaoTv"));

                if (!Permissoes.Contains(Core.Enum.Permissao.Visualizar))
                    throw new Exception(Resources.Erro.Acesso);

                ViewData.Add("Permissoes", Permissoes);

                return View(ConstroiLista(ControladorSite.Empresa.ID));
            }
            catch (Exception ex)
            {
                Core.Utils.Util.TratamentoErro(null, GetType().FullName, ex, ControladorSite.Utilizador);
                return View("Erro", new Exception(Resources.GestaoTv.Error));
            }
        }


        private List<Models.ConfigTvModel> ConstroiLista(long empresaID)
        {
            try
            {
                List<Models.ConfigTvModel> modelList = new List<Models.ConfigTvModel>();
                var lista = new Core.TV(ControladorSite.Utilizador).Lista(empresaID);

                foreach (var item in lista)
                    modelList.Add(new Models.ConfigTvModel(item));

                return modelList;
            }
            catch (Exception ex)
            {
                Core.Utils.Util.TratamentoErro(null, GetType().FullName, ex, ControladorSite.Utilizador);
            }
            return new List<Models.ConfigTvModel>();
        }

        public ActionResult Guardar(long id)
        {
            createCoockie(id);

            return Redirect(Util.FrontOfficeTVUrl);
        }

        public void createCoockie(long idtv)
        {
            var myCookie = new HttpCookie("CookieCorpTV");

            myCookie.Values.Add("tvid", idtv + "");
            myCookie.Values.Add("utilizadorid", ControladorSite.Utilizador.ID + "");

            myCookie.Expires = DateTime.Now.AddDays(1);//.AddYears(100);

            Response.Cookies.Add(myCookie);

        }

    }
}
