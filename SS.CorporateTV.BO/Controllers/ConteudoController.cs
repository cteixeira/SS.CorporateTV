using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using SS.CorporateTV.BO;
using Core = SS.CorporateTV.Core;

namespace SS.CorporateTV.BO.Controllers
{
    [Authorize]
    public class ConteudoController : Controller
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

        public ViewResult Index()
        {
            try
            {
                if (ControladorSite.Utilizador == null)
                    return View("Login");

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

        public ActionResult _Lista(Models.ConteudoFiltro filtro)
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

        public ActionResult _Imagens(Models.ConteudoImagemModel imagens)
        {
            try
            {
                if (!Permissoes.Contains(Core.Enum.Permissao.Visualizar))
                    throw new Exception(Resources.Erro.Acesso);

                ViewData.Add("Permissoes", Permissoes);
                return PartialView(ConstroiListaImagens(imagens.ConteudoID.Value));
            }
            catch (Exception ex)
            {
                Core.Utils.Util.TratamentoErro(null, GetType().FullName, ex, ControladorSite.Utilizador);
                return JavaScript(Util.JavascriptAccao(true, Resources.Erro.Geral, "error"));
            }
        }

        public PartialViewResult _ListaImagens(long id)
        {
            try
            {
                if (!Permissoes.Contains(Core.Enum.Permissao.Visualizar))
                    throw new Exception(Resources.Erro.Acesso);

                AddViewData(id);
                var model = ConstroiListaImagens(id);

                return PartialView("_ListaImagens", model);
            }
            catch (Exception ex)
            {
                Core.Utils.Util.TratamentoErro(null, GetType().FullName, ex, ControladorSite.Utilizador);
            }

            return null;
        }

        public ActionResult _QRCode(long? id)
        {
            try
            {
                if (!Permissoes.Contains(Core.Enum.Permissao.Visualizar))
                    throw new Exception(Resources.Erro.Acesso);

                ViewData.Add("Permissoes", Permissoes);

                Core.Conteudo conteudo = new Core.Conteudo(ControladorSite.Utilizador);

                if (!id.HasValue)
                    return PartialView(new Models.ConteudoModel());

                return PartialView(new Models.ConteudoModel(conteudo.Lista("", ControladorSite.Empresa.ID).First(u => u.ConteudoID == id)));
            }
            catch (Exception ex)
            {
                Core.Utils.Util.TratamentoErro(null, GetType().FullName, ex, ControladorSite.Utilizador);
                return JavaScript(Util.JavascriptAccao(true, Resources.Erro.Geral, "error"));
            }
        }

        public PartialViewResult _ListaVideos(long id)
        {
            try
            {
                if (!Permissoes.Contains(Core.Enum.Permissao.Visualizar))
                    throw new Exception(Resources.Erro.Acesso);

                AddViewData(id);
                var model = ConstroiListaVideos(id);

                return PartialView("_ListaVideos", model);
            }
            catch (Exception ex)
            {
                Core.Utils.Util.TratamentoErro(null, GetType().FullName, ex, ControladorSite.Utilizador);
            }

            return null;
        }

        public PartialViewResult _GravaTipoVideo(long id)
        {
            try
            {
                if (!Permissoes.Contains(Core.Enum.Permissao.Visualizar))
                    throw new Exception(Resources.Erro.Acesso);

                Core.Conteudo obj = new Core.Conteudo(ControladorSite.Utilizador);
                var conteudo = obj.Abrir(id);
                if (conteudo.Tipo != (short)Core.Enum.TipoConteudo.Video)
                {
                    conteudo.Tipo = (short)Core.Enum.TipoConteudo.Video;
                }
                obj.Actualizar(conteudo);

                return _ListaVideos(id);
            }
            catch (Exception ex)
            {
                Core.Utils.Util.TratamentoErro(null, GetType().FullName, ex, ControladorSite.Utilizador);
            }

            return null;
        }

        public PartialViewResult _GravaTipoImagem(long id)
        {
            try
            {
                if (!Permissoes.Contains(Core.Enum.Permissao.Visualizar))
                    throw new Exception(Resources.Erro.Acesso);

                Core.Conteudo obj = new Core.Conteudo(ControladorSite.Utilizador);
                var conteudo = obj.Abrir(id);
                if (conteudo.Tipo != (short)Core.Enum.TipoConteudo.Imagem)
                {
                    conteudo.Tipo = (short)Core.Enum.TipoConteudo.Imagem;
                }
                obj.Actualizar(conteudo);

                return _ListaImagens(id);
            }
            catch (Exception ex)
            {
                Core.Utils.Util.TratamentoErro(null, GetType().FullName, ex, ControladorSite.Utilizador);
            }

            return null;
        }

        public PartialViewResult _GravaTipoCanal(long id)
        {
            try
            {
                if (!Permissoes.Contains(Core.Enum.Permissao.Visualizar))
                    throw new Exception(Resources.Erro.Acesso);

                Core.Conteudo obj = new Core.Conteudo(ControladorSite.Utilizador);
                var conteudo = obj.Abrir(id);
                if (conteudo.Tipo != (short)Core.Enum.TipoConteudo.Canal)
                {
                    conteudo.Tipo = (short)Core.Enum.TipoConteudo.Canal;          
                }

                obj.ActualizarTipo(conteudo);

                //var videos = ConstroiListaVideos(id);
                //if (videos != null && videos.Count > 0)
                //{
                //    var stream = videos.First();
                //    .ConteudoVideo.Add(stream);
                //}
                return _ListaVideos(id);
                //return PartialView(new Models.ConteudoModel(obj.Lista("", ControladorSite.Empresa.ID).First(u => u.ConteudoID == id)));
            }
            catch (Exception ex)
            {
                Core.Utils.Util.TratamentoErro(null, GetType().FullName, ex, ControladorSite.Utilizador);
            }

            return null;
        }

        [HttpPost]
        [ValidateInput(false)]
        public ActionResult Gravar(Models.ConteudoModel registo)
        {
            try
            {
                if (!Permissoes.Contains(Core.Enum.Permissao.Visualizar))
                    throw new Exception(Resources.Erro.Acesso);

                if (ModelState.IsValid)
                {
                    Core.Conteudo obj = new Core.Conteudo(ControladorSite.Utilizador);

                    if (registo.Tipo == (short)Core.Enum.TipoConteudo.Canal)
                    {
                        if (string.IsNullOrEmpty(registo.UrlCanal))
                            return JavaScript(Util.JavascriptAccao(false, Resources.Erro.CampoObrigatorio, "error") + "$(\"#UrlCanal\").prop('required', true);");
                    }
                    //else if (registo.Tipo == (short)Core.Enum.TipoConteudo.Video)
                    //{
                    //    if(registo.Videos == null || registo.Videos.Count == 0)
                    //        return JavaScript(Util.JavascriptAccao(false, Resources.Erro.SemVideos, "error"));
                    //}
                    //else if (registo.Tipo == (short)Core.Enum.TipoConteudo.Imagem)
                    //{
                    //    if (registo.Imagens == null || registo.Imagens.Count == 0)
                    //        return JavaScript(Util.JavascriptAccao(false, Resources.Erro.SemImagens, "error"));
                    //}

                    string aftersave = "";

                    if (registo.ID.HasValue)
                    {
                        obj.Actualizar(registo.ToBDModel());
                    }
                    else
                    {
                        var newobj = obj.Inserir(registo.ToBDModel());
                        aftersave = String.Format("CallDetailAfterSave({0});", newobj.ConteudoID);
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

        [HttpPost]
        [ValidateInput(false)]
        public ActionResult GravarQRCode(Models.ConteudoModel registo)
        {
            try
            {
                if (!Permissoes.Contains(Core.Enum.Permissao.Visualizar))
                    throw new Exception(Resources.Erro.Acesso);

                if (registo.ID.HasValue)
                {
                    Core.Conteudo obj = new Core.Conteudo(ControladorSite.Utilizador);

                    string aftersave = "";
                    obj.ActualizarQRCode(registo.ToBDModel());

                    return JavaScript(Util.JavascriptAccao(false, Resources.Geral.GravarSucesso, "success", Url.Action("_Lista")) + aftersave);
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

        public ActionResult _DetalheExtra(long? id)
        {
            try
            {
                if (!Permissoes.Contains(Core.Enum.Permissao.Visualizar))
                    throw new Exception(Resources.Erro.Acesso);

                ViewData.Add("Permissoes", Permissoes);

                Core.Conteudo conteudo = new Core.Conteudo(ControladorSite.Utilizador);

                if (!id.HasValue)
                    return PartialView(new Models.ConteudoModel());

                var model = new Models.ConteudoModel(conteudo.Lista("", ControladorSite.Empresa.ID).First(u => u.ConteudoID == id));

                ViewData.Add("Videos", model.Videos);
                ViewData.Add("Imagens", model.Imagens);

                return PartialView(model);
            }
            catch (Exception ex)
            {
                Core.Utils.Util.TratamentoErro(null, GetType().FullName, ex, ControladorSite.Utilizador);
                return JavaScript(Util.JavascriptAccao(true, Resources.Erro.Geral, "error"));
            }
        }

        public PartialViewResult _Detalhe(long? id)
        {
            try
            {
                if (!Permissoes.Contains(Core.Enum.Permissao.Visualizar))
                    throw new Exception(Resources.Erro.Acesso);

                if (!id.HasValue)
                {
                    ViewData.Add("Permissoes", Permissoes);
                    var novoConteudo = new Models.ConteudoModel();

                    return PartialView(novoConteudo);
                }

                var conteudo = new Core.Conteudo(ControladorSite.Utilizador).Abrir(id.Value);
                var model = new Models.ConteudoModel(conteudo);
                ViewData.Add("Permissoes", Permissoes);
                ViewData.Add("Videos", model.Videos);
                ViewData.Add("Imagens", model.Imagens);

                return PartialView(model);
            }
            catch (Exception ex)
            {
                Core.Utils.Util.TratamentoErro(null, GetType().FullName, ex, ControladorSite.Utilizador);
            }
            return null;
        }

        public ActionResult Apagar(long id)
        {
            try
            {
                if (!Permissoes.Contains(Core.Enum.Permissao.Apagar))
                    throw new Core.Exceptions.SemPermissao();

                new Core.Conteudo(ControladorSite.Utilizador).Apagar(id);
                return JavaScript(Util.JavascriptAccao(true, Resources.Geral.ApagarSucesso, "success", Url.Action("_Lista")));
            }
            catch (Exception ex)
            {
                Core.Utils.Util.TratamentoErro(null, GetType().FullName, ex, ControladorSite.Utilizador);
                return JavaScript(Util.JavascriptAccao(true, Resources.Erro.Geral, "error"));
            }

        }

        [HttpDelete]
        public ActionResult ApagarLista(long conteudoId, string ids)
        {
            try
            {
                if (!string.IsNullOrEmpty(ids))
                {
                    if (!Permissoes.Contains(Core.Enum.Permissao.Apagar))
                        throw new Core.Exceptions.SemPermissao();

                    var lista = ids.Split(',');
                    long[] ficheirosIds = Array.ConvertAll(lista, s => long.Parse(s));
                   
                    foreach (long id in ficheirosIds)
                    {
                        new Core.ConteudoImagem(ControladorSite.Utilizador).Apagar(id);
                    }

                    AddViewData(conteudoId);
                }
                var model = ConstroiListaImagens(conteudoId);

                return PartialView("_ListaImagens", model);
                //return JavaScript(Util.JavascriptAccao(false, Resources.Geral.ApagarSucesso, "success", Url.Action("_Lista")));

                //else
                //    throw new Exception(Erro.ModeloInvalido);
            }
            catch (Exception ex)
            {
                Core.Utils.Util.TratamentoErro(null, GetType().FullName, ex, ControladorSite.Utilizador);
                return JavaScript(Util.JavascriptAccao(true, Resources.Erro.Geral, "error"));
            }
        }

        public PartialViewResult _NovaImagem(long id)
        {
            AddViewData(id);

            return PartialView(new Models.ConteudoImagemModel());
        }

        public PartialViewResult _NovoVideo(long id)
        {
            AddViewData(id);

            return PartialView(new Models.ConteudoVideoModel());
        }

        [HttpDelete]
        public ActionResult ApagarListaVideo(long conteudoId, string ids)
        {
            try
            {
                if (!string.IsNullOrEmpty(ids))
                {
                    if (!Permissoes.Contains(Core.Enum.Permissao.Apagar))
                        throw new Core.Exceptions.SemPermissao();

                    var lista = ids.Split(',');
                    long[] ficheirosIds = Array.ConvertAll(lista, s => long.Parse(s));

                    foreach (long id in ficheirosIds)
                    {
                        new Core.ConteudoVideo(ControladorSite.Utilizador).Apagar(id);
                    }

                    AddViewData(conteudoId);
                }
                var model = ConstroiListaVideos(conteudoId);

                return PartialView("_ListaVideos", model);
                //return JavaScript(Util.JavascriptAccao(false, Resources.Geral.ApagarSucesso, "success", Url.Action("_Lista")));

                //else
                //    throw new Exception(Erro.ModeloInvalido);
            }
            catch (Exception ex)
            {
                Core.Utils.Util.TratamentoErro(null, GetType().FullName, ex, ControladorSite.Utilizador);
                return JavaScript(Util.JavascriptAccao(true, Resources.Erro.Geral, "error"));
            }
        }

        [HttpPost]
        public ActionResult GravarLista(long conteudoId, string ids)
        {
            try
            {
                if (!string.IsNullOrEmpty(ids))
                {
                    if (!Permissoes.Contains(Core.Enum.Permissao.Gravar))
                        throw new Core.Exceptions.SemPermissao();

                    var lista = ids.Split(',');
                    long[] ficheirosIds = Array.ConvertAll(lista, s => long.Parse(s));

                    Core.ConteudoImagem obj = new Core.ConteudoImagem(ControladorSite.Utilizador);
                    Core.Model.ConteudoImagem conteudo;
                    
                    int ordem = 1;
                    foreach (long id in ficheirosIds)
                    {
                        conteudo = obj.Abrir(id);
                        conteudo.Ordem = ordem;
                        ++ordem;
                        obj.Actualizar(conteudo);
                    }
                    Core.Conteudo objConteudo = new Core.Conteudo(ControladorSite.Utilizador);
                    objConteudo.ActualizarData(conteudoId);

                    AddViewData(conteudoId);
                }
                var model = ConstroiListaImagens(conteudoId);

                return PartialView("_ListaImagens", model);
            }
            catch (Exception ex)
            {
                Core.Utils.Util.TratamentoErro(null, GetType().FullName, ex, ControladorSite.Utilizador);
                return JavaScript(Util.JavascriptAccao(true, Resources.Erro.Geral, "error"));
            }
        }

        [HttpPost]
        public ActionResult GravarListaVideos(long conteudoId, string ids)
        {
            try
            {
                if (!string.IsNullOrEmpty(ids))
                {
                    if (!Permissoes.Contains(Core.Enum.Permissao.Gravar))
                        throw new Core.Exceptions.SemPermissao();

                    var lista = ids.Split(',');
                    long[] ficheirosIds = Array.ConvertAll(lista, s => long.Parse(s));

                    Core.ConteudoVideo obj = new Core.ConteudoVideo(ControladorSite.Utilizador);
                    Core.Model.ConteudoVideo conteudo;

                    int ordem = 1;
                    foreach (long id in ficheirosIds)
                    {
                        conteudo = obj.Abrir(id);
                        conteudo.Ordem = ordem;
                        ++ordem;
                        obj.Actualizar(conteudo);
                    }
                    Core.Conteudo objConteudo = new Core.Conteudo(ControladorSite.Utilizador);
                    objConteudo.ActualizarData(conteudoId);

                    AddViewData(conteudoId);
                }
                var model = ConstroiListaVideos(conteudoId);

                return PartialView("_ListaVideos", model);
            }
            catch (Exception ex)
            {
                Core.Utils.Util.TratamentoErro(null, GetType().FullName, ex, ControladorSite.Utilizador);
                return JavaScript(Util.JavascriptAccao(true, Resources.Erro.Geral, "error"));
            }
        }

        [HttpPost]
        public ActionResult GravarImagens(long conteudoId)
        {
            try
            {
                List<Models.ConteudoImagemModel> registos = JsonConvert.DeserializeObject<List<Models.ConteudoImagemModel>>(Request["registos"]);

                if (ModelState.IsValid)
                {
                    if (Request.Files != null && registos != null && Request.Files.Count == registos.Count)
                    {
                        for (int i = 0; i < Request.Files.Count; i++)
                        {
                            registos[i].RetrieveBytes(Request.Files[i]);

                            registos[i].ConteudoID = conteudoId;

                            Core.ConteudoImagem obj = new Core.ConteudoImagem(ControladorSite.Utilizador);

                            obj.Inserir(registos[i].ToBDModel());
                        }
                    }
                    Core.Conteudo objConteudo = new Core.Conteudo(ControladorSite.Utilizador);
                    objConteudo.ActualizarData(conteudoId);

                    AddViewData(conteudoId);

                    var model = ConstroiListaImagens(conteudoId);

                    return PartialView("_ListaImagens", model);
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

        [HttpPost]
        public ActionResult GravarConteudoVideoLista(Models.ConteudoVideoModel registo)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    var regras = new Core.ConteudoVideo(ControladorSite.Utilizador);
                    regras.Actualizar(registo.ToBDModel());

                    if (registo.ConteudoID.HasValue)
                    {
                        Core.Conteudo objConteudo = new Core.Conteudo(ControladorSite.Utilizador);
                        objConteudo.ActualizarData(registo.ConteudoID.Value);
                    }
                    return JavaScript(Util.JavascriptAccao(false, Resources.Conteudo.ConteudoAtualizado, "success") + "getTab();");
                }
                else
                    throw new Exception(Resources.Erro.ModeloInvalido);
            }
            catch (Exception ex)
            {
                Core.Utils.Util.TratamentoErro(null, GetType().FullName, ex, ControladorSite.Utilizador);
                return JavaScript(Util.JavascriptAccao(false, ex.Message, "error"));
            }
        }
                
        [HttpPost]
        public ActionResult GravarConteudoImagemLista(Models.ConteudoImagemModel registo)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    var obj = new Core.ConteudoImagem(ControladorSite.Utilizador);

                    var imagem = obj.Abrir(registo.ConteudoImagemID.Value);
                    imagem.Designacao = registo.Designacao;
                    imagem.Duracao = registo.Duracao;
                    imagem.Ordem = registo.Ordem;

                    obj.Actualizar(imagem);

                    if (registo.ConteudoID.HasValue)
                    {
                        Core.Conteudo objConteudo = new Core.Conteudo(ControladorSite.Utilizador);
                        objConteudo.ActualizarData(registo.ConteudoID.Value);
                    }
                    return JavaScript(Util.JavascriptAccao(false, Resources.Conteudo.ConteudoAtualizado, "success") + "getTab();");
                }
                else
                    throw new Exception(Resources.Erro.ModeloInvalido);
            }
            catch (Exception ex)
            {
                Core.Utils.Util.TratamentoErro(null, GetType().FullName, ex, ControladorSite.Utilizador);
                return JavaScript(Util.JavascriptAccao(false, Resources.Erro.Geral, "error"));
            }
        }

        public ActionResult Download(long? id)
        {
            try
            {
                if (id.HasValue)
                {
                    if (!Permissoes.Contains(Core.Enum.Permissao.Visualizar))
                        throw new Exception(Resources.Erro.Acesso);

                    Core.Model.ConteudoImagem imagem = new Core.ConteudoImagem(ControladorSite.Utilizador).Abrir(id.Value);

                    if (imagem != null)
                    {
                        return File(imagem.Imagem, GetMimeType(imagem.Extensao), imagem.Designacao + "." + imagem.Extensao);
                    }
                    else
                        throw new Core.Exceptions.SemPermissao();
                }
                else
                    throw new Exception(Resources.Erro.ModeloInvalido);
            }
            catch (Exception ex)
            {
                Core.Utils.Util.TratamentoErro(null, GetType().FullName, ex, ControladorSite.Utilizador);
            }
            return null;
        }

        private void AddViewData(long id)
        {
            ViewData.Add("Permissoes", Permissoes);
            ViewData["ID"] = id;
        }

        #region Funções Auxiliares

        private List<Models.ConteudoModel> ConstroiLista(string nome)
        {
            try
            {
                List<Models.ConteudoModel> modelList = new List<Models.ConteudoModel>();
                var lista = new Core.Conteudo(ControladorSite.Utilizador).Lista(nome, ControladorSite.Empresa.ID);

                foreach (var item in lista)
                    modelList.Add(new Models.ConteudoModel(item));

                return modelList;
            }
            catch (Exception ex)
            {
                Core.Utils.Util.TratamentoErro(null, "ConteudoController", ex, ControladorSite.Utilizador);
            }
            return new List<Models.ConteudoModel>();
        }

        private List<Models.ConteudoImagemModel> ConstroiListaImagens(long conteudoID)
        {
            try
            {
                List<Models.ConteudoImagemModel> modelList = new List<Models.ConteudoImagemModel>();
                var lista = new Core.ConteudoImagem(ControladorSite.Utilizador).Lista(conteudoID);

                foreach (var item in lista)
                    modelList.Add(new Models.ConteudoImagemModel(item));

                return modelList;
            }
            catch (Exception ex)
            {
                Core.Utils.Util.TratamentoErro(null, "ConteudoController", ex, ControladorSite.Utilizador);
            }
            return new List<Models.ConteudoImagemModel>();
        }

        private List<Models.ConteudoVideoModel> ConstroiListaVideos(long conteudoID)
        {
            try
            {
                List<Models.ConteudoVideoModel> modelList = new List<Models.ConteudoVideoModel>();
                var lista = new Core.ConteudoVideo(ControladorSite.Utilizador).Lista(conteudoID);

                foreach (var item in lista)
                    modelList.Add(new Models.ConteudoVideoModel(item));

                return modelList;
            }
            catch (Exception ex)
            {
                Core.Utils.Util.TratamentoErro(null, "ConteudoController", ex, ControladorSite.Utilizador);
            }
            return new List<Models.ConteudoVideoModel>();
        }

        [HttpPost]
        public ActionResult GravarVideo(long conteudoId)
        {
            try
            {
                Models.ConteudoVideoModel registo = JsonConvert.DeserializeObject<Models.ConteudoVideoModel>(Request["registos"]);

                if (ModelState.IsValid)
                {
                    if (registo != null)
                    {
                        registo.ConteudoID = conteudoId;

                        Core.ConteudoVideo obj = new Core.ConteudoVideo(ControladorSite.Utilizador);

                        obj.Inserir(registo.ToBDModel());
                    }

                    Core.Conteudo objConteudo = new Core.Conteudo(ControladorSite.Utilizador);
                    objConteudo.ActualizarData(conteudoId);

                    AddViewData(conteudoId);

                    var model = ConstroiListaVideos(conteudoId);

                    return PartialView("_ListaVideos", model);
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

        public static IEnumerable<SelectListItem> ConstroiListaTipoConteudoDropdown()
        {
            List<SelectListItem> listaTipoConteudo = new List<SelectListItem>();
            //{
            //    new SelectListItem
            //    {
            //        Selected = true,
            //        Text = Resources.Geral.ListaOpcaoVazia,
            //        Value = string.Empty
            //    }
            //};

            var values = (System.Enum.GetValues(typeof(Core.Enum.TipoConteudo)).Cast<Core.Enum.TipoConteudo>().Select(
                e => new SelectListItem()
                {
                    Text = Resources.TipoConteudo.ResourceManager.GetString(e.ToString()),
                    Value = ((int)e).ToString(),
                    Selected = false
                })
            ).OrderBy(o => o.Text).ToList();

            listaTipoConteudo.AddRange(values);

            return listaTipoConteudo;
        }

        public static IEnumerable<SelectListItem> ConstroiListaConteudoDropdown()
        {
            List<SelectListItem> listaTipoConteudo = new List<SelectListItem> {
                new SelectListItem
                {
                    Selected = true,
                    Text = string.Empty,
                    Value = string.Empty
                }
            };

            var values = new Core.Conteudo(ControladorSite.Utilizador).Lista(ControladorSite.Utilizador.EmpresaID).Select(
                e => new SelectListItem()
                {
                    Text = e.Designacao,
                    Value = e.ConteudoID.ToString(),
                    Selected = false
                }
            ).OrderBy(o => o.Text).ToList();

            listaTipoConteudo.AddRange(values);

            return listaTipoConteudo;
        }

        private static IDictionary<string, string> _mappings =
            new Dictionary<string, string>(StringComparer.InvariantCultureIgnoreCase)
        {
        #region Big freaking list of mime types

        // combination of values from Windows 7 Registry and
        // from C:\Windows\System32\inetsrv\config\applicationHost.config
        // some added, including .7z and .dat
        {".323", "text/h323"},
        {".3g2", "video/3gpp2"},
        {".3gp", "video/3gpp"},
        {".3gp2", "video/3gpp2"},
        {".3gpp", "video/3gpp"},
        {".7z", "application/x-7z-compressed"},
        {".aa", "audio/audible"},
        {".AAC", "audio/aac"},
        {".aaf", "application/octet-stream"},
        {".aax", "audio/vnd.audible.aax"},
        {".ac3", "audio/ac3"},
        {".aca", "application/octet-stream"},
        {".accda", "application/msaccess.addin"},
        {".accdb", "application/msaccess"},
        {".accdc", "application/msaccess.cab"},
        {".accde", "application/msaccess"},
        {".accdr", "application/msaccess.runtime"},
        {".accdt", "application/msaccess"},
        {".accdw", "application/msaccess.webapplication"},
        {".accft", "application/msaccess.ftemplate"},
        {".acx", "application/internet-property-stream"},
        {".AddIn", "text/xml"},
        {".ade", "application/msaccess"},
        {".adobebridge", "application/x-bridge-url"},
        {".adp", "application/msaccess"},
        {".ADT", "audio/vnd.dlna.adts"},
        {".ADTS", "audio/aac"},
        {".afm", "application/octet-stream"},
        {".ai", "application/postscript"},
        {".aif", "audio/x-aiff"},
        {".aifc", "audio/aiff"},
        {".aiff", "audio/aiff"},
        {".air", "application/vnd.adobe.air-application-installer-package+zip"},
        {".amc", "application/x-mpeg"},
        {".application", "application/x-ms-application"},
        {".art", "image/x-jg"},
        {".asa", "application/xml"},
        {".asax", "application/xml"},
        {".ascx", "application/xml"},
        {".asd", "application/octet-stream"},
        {".asf", "video/x-ms-asf"},
        {".ashx", "application/xml"},
        {".asi", "application/octet-stream"},
        {".asm", "text/plain"},
        {".asmx", "application/xml"},
        {".aspx", "application/xml"},
        {".asr", "video/x-ms-asf"},
        {".asx", "video/x-ms-asf"},
        {".atom", "application/atom+xml"},
        {".au", "audio/basic"},
        {".avi", "video/x-msvideo"},
        {".axs", "application/olescript"},
        {".bas", "text/plain"},
        {".bcpio", "application/x-bcpio"},
        {".bin", "application/octet-stream"},
        {".bmp", "image/bmp"},
        {".c", "text/plain"},
        {".cab", "application/octet-stream"},
        {".caf", "audio/x-caf"},
        {".calx", "application/vnd.ms-office.calx"},
        {".cat", "application/vnd.ms-pki.seccat"},
        {".cc", "text/plain"},
        {".cd", "text/plain"},
        {".cdda", "audio/aiff"},
        {".cdf", "application/x-cdf"},
        {".cer", "application/x-x509-ca-cert"},
        {".chm", "application/octet-stream"},
        {".class", "application/x-java-applet"},
        {".clp", "application/x-msclip"},
        {".cmx", "image/x-cmx"},
        {".cnf", "text/plain"},
        {".cod", "image/cis-cod"},
        {".config", "application/xml"},
        {".contact", "text/x-ms-contact"},
        {".coverage", "application/xml"},
        {".cpio", "application/x-cpio"},
        {".cpp", "text/plain"},
        {".crd", "application/x-mscardfile"},
        {".crl", "application/pkix-crl"},
        {".crt", "application/x-x509-ca-cert"},
        {".cs", "text/plain"},
        {".csdproj", "text/plain"},
        {".csh", "application/x-csh"},
        {".csproj", "text/plain"},
        {".css", "text/css"},
        {".csv", "text/csv"},
        {".cur", "application/octet-stream"},
        {".cxx", "text/plain"},
        {".dat", "application/octet-stream"},
        {".datasource", "application/xml"},
        {".dbproj", "text/plain"},
        {".dcr", "application/x-director"},
        {".def", "text/plain"},
        {".deploy", "application/octet-stream"},
        {".der", "application/x-x509-ca-cert"},
        {".dgml", "application/xml"},
        {".dib", "image/bmp"},
        {".dif", "video/x-dv"},
        {".dir", "application/x-director"},
        {".disco", "text/xml"},
        {".dll", "application/x-msdownload"},
        {".dll.config", "text/xml"},
        {".dlm", "text/dlm"},
        {".doc", "application/msword"},
        {".docm", "application/vnd.ms-word.document.macroEnabled.12"},
        {".docx", "application/vnd.openxmlformats-officedocument.wordprocessingml.document"},
        {".dot", "application/msword"},
        {".dotm", "application/vnd.ms-word.template.macroEnabled.12"},
        {".dotx", "application/vnd.openxmlformats-officedocument.wordprocessingml.template"},
        {".dsp", "application/octet-stream"},
        {".dsw", "text/plain"},
        {".dtd", "text/xml"},
        {".dtsConfig", "text/xml"},
        {".dv", "video/x-dv"},
        {".dvi", "application/x-dvi"},
        {".dwf", "drawing/x-dwf"},
        {".dwp", "application/octet-stream"},
        {".dxr", "application/x-director"},
        {".eml", "message/rfc822"},
        {".emz", "application/octet-stream"},
        {".eot", "application/octet-stream"},
        {".eps", "application/postscript"},
        {".etl", "application/etl"},
        {".etx", "text/x-setext"},
        {".evy", "application/envoy"},
        {".exe", "application/octet-stream"},
        {".exe.config", "text/xml"},
        {".fdf", "application/vnd.fdf"},
        {".fif", "application/fractals"},
        {".filters", "Application/xml"},
        {".fla", "application/octet-stream"},
        {".flr", "x-world/x-vrml"},
        {".flv", "video/x-flv"},
        {".fsscript", "application/fsharp-script"},
        {".fsx", "application/fsharp-script"},
        {".generictest", "application/xml"},
        {".gif", "image/gif"},
        {".group", "text/x-ms-group"},
        {".gsm", "audio/x-gsm"},
        {".gtar", "application/x-gtar"},
        {".gz", "application/x-gzip"},
        {".h", "text/plain"},
        {".hdf", "application/x-hdf"},
        {".hdml", "text/x-hdml"},
        {".hhc", "application/x-oleobject"},
        {".hhk", "application/octet-stream"},
        {".hhp", "application/octet-stream"},
        {".hlp", "application/winhlp"},
        {".hpp", "text/plain"},
        {".hqx", "application/mac-binhex40"},
        {".hta", "application/hta"},
        {".htc", "text/x-component"},
        {".htm", "text/html"},
        {".html", "text/html"},
        {".htt", "text/webviewhtml"},
        {".hxa", "application/xml"},
        {".hxc", "application/xml"},
        {".hxd", "application/octet-stream"},
        {".hxe", "application/xml"},
        {".hxf", "application/xml"},
        {".hxh", "application/octet-stream"},
        {".hxi", "application/octet-stream"},
        {".hxk", "application/xml"},
        {".hxq", "application/octet-stream"},
        {".hxr", "application/octet-stream"},
        {".hxs", "application/octet-stream"},
        {".hxt", "text/html"},
        {".hxv", "application/xml"},
        {".hxw", "application/octet-stream"},
        {".hxx", "text/plain"},
        {".i", "text/plain"},
        {".ico", "image/x-icon"},
        {".ics", "application/octet-stream"},
        {".idl", "text/plain"},
        {".ief", "image/ief"},
        {".iii", "application/x-iphone"},
        {".inc", "text/plain"},
        {".inf", "application/octet-stream"},
        {".inl", "text/plain"},
        {".ins", "application/x-internet-signup"},
        {".ipa", "application/x-itunes-ipa"},
        {".ipg", "application/x-itunes-ipg"},
        {".ipproj", "text/plain"},
        {".ipsw", "application/x-itunes-ipsw"},
        {".iqy", "text/x-ms-iqy"},
        {".isp", "application/x-internet-signup"},
        {".ite", "application/x-itunes-ite"},
        {".itlp", "application/x-itunes-itlp"},
        {".itms", "application/x-itunes-itms"},
        {".itpc", "application/x-itunes-itpc"},
        {".IVF", "video/x-ivf"},
        {".jar", "application/java-archive"},
        {".java", "application/octet-stream"},
        {".jck", "application/liquidmotion"},
        {".jcz", "application/liquidmotion"},
        {".jfif", "image/pjpeg"},
        {".jnlp", "application/x-java-jnlp-file"},
        {".jpb", "application/octet-stream"},
        {".jpe", "image/jpeg"},
        {".jpeg", "image/jpeg"},
        {".jpg", "image/jpeg"},
        {".js", "application/x-javascript"},
        {".json", "application/json"},
        {".jsx", "text/jscript"},
        {".jsxbin", "text/plain"},
        {".latex", "application/x-latex"},
        {".library-ms", "application/windows-library+xml"},
        {".lit", "application/x-ms-reader"},
        {".loadtest", "application/xml"},
        {".lpk", "application/octet-stream"},
        {".lsf", "video/x-la-asf"},
        {".lst", "text/plain"},
        {".lsx", "video/x-la-asf"},
        {".lzh", "application/octet-stream"},
        {".m13", "application/x-msmediaview"},
        {".m14", "application/x-msmediaview"},
        {".m1v", "video/mpeg"},
        {".m2t", "video/vnd.dlna.mpeg-tts"},
        {".m2ts", "video/vnd.dlna.mpeg-tts"},
        {".m2v", "video/mpeg"},
        {".m3u", "audio/x-mpegurl"},
        {".m3u8", "audio/x-mpegurl"},
        {".m4a", "audio/m4a"},
        {".m4b", "audio/m4b"},
        {".m4p", "audio/m4p"},
        {".m4r", "audio/x-m4r"},
        {".m4v", "video/x-m4v"},
        {".mac", "image/x-macpaint"},
        {".mak", "text/plain"},
        {".man", "application/x-troff-man"},
        {".manifest", "application/x-ms-manifest"},
        {".map", "text/plain"},
        {".master", "application/xml"},
        {".mda", "application/msaccess"},
        {".mdb", "application/x-msaccess"},
        {".mde", "application/msaccess"},
        {".mdp", "application/octet-stream"},
        {".me", "application/x-troff-me"},
        {".mfp", "application/x-shockwave-flash"},
        {".mht", "message/rfc822"},
        {".mhtml", "message/rfc822"},
        {".mid", "audio/mid"},
        {".midi", "audio/mid"},
        {".mix", "application/octet-stream"},
        {".mk", "text/plain"},
        {".mmf", "application/x-smaf"},
        {".mno", "text/xml"},
        {".mny", "application/x-msmoney"},
        {".mod", "video/mpeg"},
        {".mov", "video/quicktime"},
        {".movie", "video/x-sgi-movie"},
        {".mp2", "video/mpeg"},
        {".mp2v", "video/mpeg"},
        {".mp3", "audio/mpeg"},
        {".mp4", "video/mp4"},
        {".mp4v", "video/mp4"},
        {".mpa", "video/mpeg"},
        {".mpe", "video/mpeg"},
        {".mpeg", "video/mpeg"},
        {".mpf", "application/vnd.ms-mediapackage"},
        {".mpg", "video/mpeg"},
        {".mpp", "application/vnd.ms-project"},
        {".mpv2", "video/mpeg"},
        {".mqv", "video/quicktime"},
        {".ms", "application/x-troff-ms"},
        {".msi", "application/octet-stream"},
        {".mso", "application/octet-stream"},
        {".mts", "video/vnd.dlna.mpeg-tts"},
        {".mtx", "application/xml"},
        {".mvb", "application/x-msmediaview"},
        {".mvc", "application/x-miva-compiled"},
        {".mxp", "application/x-mmxp"},
        {".nc", "application/x-netcdf"},
        {".nsc", "video/x-ms-asf"},
        {".nws", "message/rfc822"},
        {".ocx", "application/octet-stream"},
        {".oda", "application/oda"},
        {".odc", "text/x-ms-odc"},
        {".odh", "text/plain"},
        {".odl", "text/plain"},
        {".odp", "application/vnd.oasis.opendocument.presentation"},
        {".ods", "application/oleobject"},
        {".odt", "application/vnd.oasis.opendocument.text"},
        {".one", "application/onenote"},
        {".onea", "application/onenote"},
        {".onepkg", "application/onenote"},
        {".onetmp", "application/onenote"},
        {".onetoc", "application/onenote"},
        {".onetoc2", "application/onenote"},
        {".orderedtest", "application/xml"},
        {".osdx", "application/opensearchdescription+xml"},
        {".p10", "application/pkcs10"},
        {".p12", "application/x-pkcs12"},
        {".p7b", "application/x-pkcs7-certificates"},
        {".p7c", "application/pkcs7-mime"},
        {".p7m", "application/pkcs7-mime"},
        {".p7r", "application/x-pkcs7-certreqresp"},
        {".p7s", "application/pkcs7-signature"},
        {".pbm", "image/x-portable-bitmap"},
        {".pcast", "application/x-podcast"},
        {".pct", "image/pict"},
        {".pcx", "application/octet-stream"},
        {".pcz", "application/octet-stream"},
        {".pdf", "application/pdf"},
        {".pfb", "application/octet-stream"},
        {".pfm", "application/octet-stream"},
        {".pfx", "application/x-pkcs12"},
        {".pgm", "image/x-portable-graymap"},
        {".pic", "image/pict"},
        {".pict", "image/pict"},
        {".pkgdef", "text/plain"},
        {".pkgundef", "text/plain"},
        {".pko", "application/vnd.ms-pki.pko"},
        {".pls", "audio/scpls"},
        {".pma", "application/x-perfmon"},
        {".pmc", "application/x-perfmon"},
        {".pml", "application/x-perfmon"},
        {".pmr", "application/x-perfmon"},
        {".pmw", "application/x-perfmon"},
        {".png", "image/png"},
        {".pnm", "image/x-portable-anymap"},
        {".pnt", "image/x-macpaint"},
        {".pntg", "image/x-macpaint"},
        {".pnz", "image/png"},
        {".pot", "application/vnd.ms-powerpoint"},
        {".potm", "application/vnd.ms-powerpoint.template.macroEnabled.12"},
        {".potx", "application/vnd.openxmlformats-officedocument.presentationml.template"},
        {".ppa", "application/vnd.ms-powerpoint"},
        {".ppam", "application/vnd.ms-powerpoint.addin.macroEnabled.12"},
        {".ppm", "image/x-portable-pixmap"},
        {".pps", "application/vnd.ms-powerpoint"},
        {".ppsm", "application/vnd.ms-powerpoint.slideshow.macroEnabled.12"},
        {".ppsx", "application/vnd.openxmlformats-officedocument.presentationml.slideshow"},
        {".ppt", "application/vnd.ms-powerpoint"},
        {".pptm", "application/vnd.ms-powerpoint.presentation.macroEnabled.12"},
        {".pptx", "application/vnd.openxmlformats-officedocument.presentationml.presentation"},
        {".prf", "application/pics-rules"},
        {".prm", "application/octet-stream"},
        {".prx", "application/octet-stream"},
        {".ps", "application/postscript"},
        {".psc1", "application/PowerShell"},
        {".psd", "application/octet-stream"},
        {".psess", "application/xml"},
        {".psm", "application/octet-stream"},
        {".psp", "application/octet-stream"},
        {".pub", "application/x-mspublisher"},
        {".pwz", "application/vnd.ms-powerpoint"},
        {".qht", "text/x-html-insertion"},
        {".qhtm", "text/x-html-insertion"},
        {".qt", "video/quicktime"},
        {".qti", "image/x-quicktime"},
        {".qtif", "image/x-quicktime"},
        {".qtl", "application/x-quicktimeplayer"},
        {".qxd", "application/octet-stream"},
        {".ra", "audio/x-pn-realaudio"},
        {".ram", "audio/x-pn-realaudio"},
        {".rar", "application/octet-stream"},
        {".ras", "image/x-cmu-raster"},
        {".rat", "application/rat-file"},
        {".rc", "text/plain"},
        {".rc2", "text/plain"},
        {".rct", "text/plain"},
        {".rdlc", "application/xml"},
        {".resx", "application/xml"},
        {".rf", "image/vnd.rn-realflash"},
        {".rgb", "image/x-rgb"},
        {".rgs", "text/plain"},
        {".rm", "application/vnd.rn-realmedia"},
        {".rmi", "audio/mid"},
        {".rmp", "application/vnd.rn-rn_music_package"},
        {".roff", "application/x-troff"},
        {".rpm", "audio/x-pn-realaudio-plugin"},
        {".rqy", "text/x-ms-rqy"},
        {".rtf", "application/rtf"},
        {".rtx", "text/richtext"},
        {".ruleset", "application/xml"},
        {".s", "text/plain"},
        {".safariextz", "application/x-safari-safariextz"},
        {".scd", "application/x-msschedule"},
        {".sct", "text/scriptlet"},
        {".sd2", "audio/x-sd2"},
        {".sdp", "application/sdp"},
        {".sea", "application/octet-stream"},
        {".searchConnector-ms", "application/windows-search-connector+xml"},
        {".setpay", "application/set-payment-initiation"},
        {".setreg", "application/set-registration-initiation"},
        {".settings", "application/xml"},
        {".sgimb", "application/x-sgimb"},
        {".sgml", "text/sgml"},
        {".sh", "application/x-sh"},
        {".shar", "application/x-shar"},
        {".shtml", "text/html"},
        {".sit", "application/x-stuffit"},
        {".sitemap", "application/xml"},
        {".skin", "application/xml"},
        {".sldm", "application/vnd.ms-powerpoint.slide.macroEnabled.12"},
        {".sldx", "application/vnd.openxmlformats-officedocument.presentationml.slide"},
        {".slk", "application/vnd.ms-excel"},
        {".sln", "text/plain"},
        {".slupkg-ms", "application/x-ms-license"},
        {".smd", "audio/x-smd"},
        {".smi", "application/octet-stream"},
        {".smx", "audio/x-smd"},
        {".smz", "audio/x-smd"},
        {".snd", "audio/basic"},
        {".snippet", "application/xml"},
        {".snp", "application/octet-stream"},
        {".sol", "text/plain"},
        {".sor", "text/plain"},
        {".spc", "application/x-pkcs7-certificates"},
        {".spl", "application/futuresplash"},
        {".src", "application/x-wais-source"},
        {".srf", "text/plain"},
        {".SSISDeploymentManifest", "text/xml"},
        {".ssm", "application/streamingmedia"},
        {".sst", "application/vnd.ms-pki.certstore"},
        {".stl", "application/vnd.ms-pki.stl"},
        {".sv4cpio", "application/x-sv4cpio"},
        {".sv4crc", "application/x-sv4crc"},
        {".svc", "application/xml"},
        {".swf", "application/x-shockwave-flash"},
        {".t", "application/x-troff"},
        {".tar", "application/x-tar"},
        {".tcl", "application/x-tcl"},
        {".testrunconfig", "application/xml"},
        {".testsettings", "application/xml"},
        {".tex", "application/x-tex"},
        {".texi", "application/x-texinfo"},
        {".texinfo", "application/x-texinfo"},
        {".tgz", "application/x-compressed"},
        {".thmx", "application/vnd.ms-officetheme"},
        {".thn", "application/octet-stream"},
        {".tif", "image/tiff"},
        {".tiff", "image/tiff"},
        {".tlh", "text/plain"},
        {".tli", "text/plain"},
        {".toc", "application/octet-stream"},
        {".tr", "application/x-troff"},
        {".trm", "application/x-msterminal"},
        {".trx", "application/xml"},
        {".ts", "video/vnd.dlna.mpeg-tts"},
        {".tsv", "text/tab-separated-values"},
        {".ttf", "application/octet-stream"},
        {".tts", "video/vnd.dlna.mpeg-tts"},
        {".txt", "text/plain"},
        {".u32", "application/octet-stream"},
        {".uls", "text/iuls"},
        {".user", "text/plain"},
        {".ustar", "application/x-ustar"},
        {".vb", "text/plain"},
        {".vbdproj", "text/plain"},
        {".vbk", "video/mpeg"},
        {".vbproj", "text/plain"},
        {".vbs", "text/vbscript"},
        {".vcf", "text/x-vcard"},
        {".vcproj", "Application/xml"},
        {".vcs", "text/plain"},
        {".vcxproj", "Application/xml"},
        {".vddproj", "text/plain"},
        {".vdp", "text/plain"},
        {".vdproj", "text/plain"},
        {".vdx", "application/vnd.ms-visio.viewer"},
        {".vml", "text/xml"},
        {".vscontent", "application/xml"},
        {".vsct", "text/xml"},
        {".vsd", "application/vnd.visio"},
        {".vsi", "application/ms-vsi"},
        {".vsix", "application/vsix"},
        {".vsixlangpack", "text/xml"},
        {".vsixmanifest", "text/xml"},
        {".vsmdi", "application/xml"},
        {".vspscc", "text/plain"},
        {".vss", "application/vnd.visio"},
        {".vsscc", "text/plain"},
        {".vssettings", "text/xml"},
        {".vssscc", "text/plain"},
        {".vst", "application/vnd.visio"},
        {".vstemplate", "text/xml"},
        {".vsto", "application/x-ms-vsto"},
        {".vsw", "application/vnd.visio"},
        {".vsx", "application/vnd.visio"},
        {".vtx", "application/vnd.visio"},
        {".wav", "audio/wav"},
        {".wave", "audio/wav"},
        {".wax", "audio/x-ms-wax"},
        {".wbk", "application/msword"},
        {".wbmp", "image/vnd.wap.wbmp"},
        {".wcm", "application/vnd.ms-works"},
        {".wdb", "application/vnd.ms-works"},
        {".wdp", "image/vnd.ms-photo"},
        {".webarchive", "application/x-safari-webarchive"},
        {".webtest", "application/xml"},
        {".wiq", "application/xml"},
        {".wiz", "application/msword"},
        {".wks", "application/vnd.ms-works"},
        {".WLMP", "application/wlmoviemaker"},
        {".wlpginstall", "application/x-wlpg-detect"},
        {".wlpginstall3", "application/x-wlpg3-detect"},
        {".wm", "video/x-ms-wm"},
        {".wma", "audio/x-ms-wma"},
        {".wmd", "application/x-ms-wmd"},
        {".wmf", "application/x-msmetafile"},
        {".wml", "text/vnd.wap.wml"},
        {".wmlc", "application/vnd.wap.wmlc"},
        {".wmls", "text/vnd.wap.wmlscript"},
        {".wmlsc", "application/vnd.wap.wmlscriptc"},
        {".wmp", "video/x-ms-wmp"},
        {".wmv", "video/x-ms-wmv"},
        {".wmx", "video/x-ms-wmx"},
        {".wmz", "application/x-ms-wmz"},
        {".wpl", "application/vnd.ms-wpl"},
        {".wps", "application/vnd.ms-works"},
        {".wri", "application/x-mswrite"},
        {".wrl", "x-world/x-vrml"},
        {".wrz", "x-world/x-vrml"},
        {".wsc", "text/scriptlet"},
        {".wsdl", "text/xml"},
        {".wvx", "video/x-ms-wvx"},
        {".x", "application/directx"},
        {".xaf", "x-world/x-vrml"},
        {".xaml", "application/xaml+xml"},
        {".xap", "application/x-silverlight-app"},
        {".xbap", "application/x-ms-xbap"},
        {".xbm", "image/x-xbitmap"},
        {".xdr", "text/plain"},
        {".xht", "application/xhtml+xml"},
        {".xhtml", "application/xhtml+xml"},
        {".xla", "application/vnd.ms-excel"},
        {".xlam", "application/vnd.ms-excel.addin.macroEnabled.12"},
        {".xlc", "application/vnd.ms-excel"},
        {".xld", "application/vnd.ms-excel"},
        {".xlk", "application/vnd.ms-excel"},
        {".xll", "application/vnd.ms-excel"},
        {".xlm", "application/vnd.ms-excel"},
        {".xls", "application/vnd.ms-excel"},
        {".xlsb", "application/vnd.ms-excel.sheet.binary.macroEnabled.12"},
        {".xlsm", "application/vnd.ms-excel.sheet.macroEnabled.12"},
        {".xlsx", "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet"},
        {".xlt", "application/vnd.ms-excel"},
        {".xltm", "application/vnd.ms-excel.template.macroEnabled.12"},
        {".xltx", "application/vnd.openxmlformats-officedocument.spreadsheetml.template"},
        {".xlw", "application/vnd.ms-excel"},
        {".xml", "text/xml"},
        {".xmta", "application/xml"},
        {".xof", "x-world/x-vrml"},
        {".XOML", "text/plain"},
        {".xpm", "image/x-xpixmap"},
        {".xps", "application/vnd.ms-xpsdocument"},
        {".xrm-ms", "text/xml"},
        {".xsc", "application/xml"},
        {".xsd", "text/xml"},
        {".xsf", "text/xml"},
        {".xsl", "text/xml"},
        {".xslt", "text/xml"},
        {".xsn", "application/octet-stream"},
        {".xss", "application/xml"},
        {".xtp", "application/octet-stream"},
        {".xwd", "image/x-xwindowdump"},
        {".z", "application/x-compress"},
        {".zip", "application/x-zip-compressed"},
        #endregion Big freaking list of mime types
        };

        public static string GetMimeType(string extension)
        {
            if (extension == null)
            {
                throw new ArgumentNullException("extension");
            }

            if (!extension.StartsWith("."))
            {
                extension = "." + extension;
            }

            string mime;

            return _mappings.TryGetValue(extension, out mime) ? mime : "application/octet-stream";
        }

        #endregion



    }
}
