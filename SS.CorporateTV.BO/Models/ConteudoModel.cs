using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Web;

namespace SS.CorporateTV.BO.Models
{
    
    public class ConteudoModel
    {
        public long? ID { get; set; }

        [DisplayLocalizado(typeof(Resources.Conteudo), "Designacao")]
        [RequiredLocalizado(typeof(Resources.Conteudo), "Designacao")]        
        public string Designacao { get; set; }

        [DisplayLocalizado(typeof(Resources.Conteudo), "Tipo")]
        [RequiredLocalizado(typeof(Resources.Conteudo), "Tipo")]
        public int Tipo { get; set; }

        public string TipoDesignacao { get; set; }

        [DisplayLocalizado(typeof(Resources.Conteudo), "QRCode")]       
        public string QRCode { get; set; }

        public List<ConteudoImagemModel> Imagens { get; set; }

        public List<ConteudoVideoModel> Videos { get; set; }

        [DisplayLocalizado(typeof(Resources.Conteudo), "Url")]
        //[RequiredLocalizado(typeof(Resources.Conteudo),  "Url")]
        public string UrlCanal { get; set; }

        public ConteudoModel() { }

        public ConteudoModel(Core.Model.Conteudo model)
        {
            this.ID = model.ConteudoID;
            this.Designacao = model.Designacao;
            this.QRCode = model.QRCode;
            this.Tipo = model.Tipo;
            this.TipoDesignacao = Resources.TipoConteudo.ResourceManager.GetString(Enum.GetName(typeof(Core.Enum.TipoConteudo), model.Tipo)); ;
            //this.Imagens = new List<ConteudoImagemModel>();
            //var imagens = new Core.ConteudoImagem(ControladorSite.Utilizador).Lista(this.ID.Value).ToList();

            //if (imagens != null && imagens.Count > 0)
            //{
            //    foreach (var item in imagens)
            //    {
            //        Imagens.Add(new Models.ConteudoImagemModel(item));
            //    }
            //}

            //this.Videos = new List<ConteudoVideoModel>();
            //var videos = new Core.ConteudoVideo(ControladorSite.Utilizador).Lista(this.ID.Value).ToList();

            //if (videos != null && videos.Count > 0)
            //{
            //    foreach (var item in videos)
            //    {
            //        Videos.Add(new Models.ConteudoVideoModel(item));
            //    }
            //}

            if (model.Tipo == (short)Core.Enum.TipoConteudo.Canal)
            {
                var videos = new Core.ConteudoVideo(ControladorSite.Utilizador).Lista(this.ID.Value).ToList();

                if(videos != null && videos.Count > 0)
                    this.UrlCanal = videos.First().Url;
            }
        }

        public Core.Model.Conteudo ToBDModel()
        {
            Core.Model.Conteudo conteudo = new Core.Model.Conteudo();
            if (this.ID.HasValue)
                conteudo.ConteudoID = this.ID.Value;

            if (ControladorSite.Empresa != null)
                    conteudo.EmpresaID = ControladorSite.Empresa.ID;

            conteudo.Tipo = (short)this.Tipo;
            conteudo.Designacao = this.Designacao;
            conteudo.QRCode = this.QRCode;

            long conteudoID = 0;
            //if (this.Tipo == (short)Core.Enum.TipoConteudo.Canal && !this.ID.HasValue)
            //{
            //    Core.Conteudo obj = new Core.Conteudo(ControladorSite.Utilizador);
            //    conteudoID = obj.Inserir(conteudo).ConteudoID;
            //}
            if (this.Tipo == (short)Core.Enum.TipoConteudo.Canal)// && (this.ID.HasValue || conteudoID > 0))
            {
                if (conteudo.ConteudoVideo.Count == 0)
                {
                    var video = new Core.Model.ConteudoVideo();
                    //video.ConteudoID = this.ID.HasValue ? this.ID.Value : conteudoID;
                    video.Designacao = this.Designacao;
                    video.Duracao = 0;
                    video.Url = this.UrlCanal;

                    conteudo.ConteudoVideo.Add(video);
                }
                else
                {
                    var stream = conteudo.ConteudoVideo.First();
                    stream.Url = this.UrlCanal;
                    stream.Designacao = this.Designacao;
                    stream.Duracao = 0;
                }
            }

            return conteudo;
        }
    }

    public class ConteudoFiltro
    {
        [DisplayLocalizado(typeof(Resources.Conteudo), "Pesquisa")]
        public string Pesquisa { get; set; }
    }

    public class ConteudoImagemModel
    {
        private readonly static MD5CryptoServiceProvider md5 = new MD5CryptoServiceProvider();

        public long? ConteudoID { get; set; }

        public long? ConteudoImagemID { get; set; }

        [DisplayLocalizado(typeof(Resources.Conteudo), "Designacao")]
        [RequiredLocalizado(typeof(Resources.Conteudo), "Designacao")]
        public string Designacao { get; set; }

        public string Extensao { get; set; }

        public byte[] Binario { get; set; }

        public byte[] HashFicheiro { get; set; }

        [DisplayLocalizado(typeof(Resources.Conteudo), "UrlImagem")]
        public string Url { get; set; }

        [DisplayLocalizado(typeof(Resources.Conteudo), "Duracao")]
        [RequiredLocalizado(typeof(Resources.Conteudo), "Duracao")]
        public int Duracao { get; set; }

        public int? Ordem { get; set; }

        #region Construtores

        public ConteudoImagemModel() {
        }

        public ConteudoImagemModel(Core.Model.ConteudoImagem model)
        {
            this.ConteudoID = model.ConteudoID;
            this.ConteudoImagemID = model.ConteudoImagemID;
            this.Designacao = model.Designacao;
            this.Binario = model.Imagem;
            this.HashFicheiro = model.HashFicheiro;
            this.Extensao = model.Extensao;
            this.Url = model.UrlImagem;
            this.Duracao = model.Duracao;
            this.Ordem = model.Ordem;
        }

        #endregion Construtores

        #region Métodos

        public Core.Model.ConteudoImagem ToBDModel()
        {
            Core.Model.ConteudoImagem conteudo = new Core.Model.ConteudoImagem();
            
            if (this.ConteudoImagemID.HasValue)
                conteudo.ConteudoImagemID = this.ConteudoImagemID.Value;
            if (this.ConteudoID.HasValue)
            {
                conteudo.ConteudoID = this.ConteudoID.Value;
            }
            conteudo.Designacao = this.Designacao;
            conteudo.Duracao = this.Duracao;
            conteudo.Extensao = this.Extensao;
            conteudo.Imagem = this.Binario;
            conteudo.HashFicheiro = ConteudoImagemID != 0 ? HashFicheiro : md5.ComputeHash(Binario);
            conteudo.Ordem = this.Ordem;

            return conteudo;
        }

        public byte[] RetrieveBytes(HttpPostedFileBase file)
        {
            MemoryStream target = new MemoryStream();
            file.InputStream.CopyTo(target);
            Binario = target.ToArray();

            return Binario;
        }

        #endregion Métodos
    }

    public class UploadImagens
    {
        public IEnumerable<HttpPostedFileBase> HttpFiles { get; set; }
        public IList<ConteudoImagemModel> Imagem { get; set; }
    }

    public class ConteudoVideoModel
    {
        public long? ConteudoID { get; set; }

        public long? ConteudoVideoID { get; set; }

        [DisplayLocalizado(typeof(Resources.Conteudo), "Designacao")]
        [RequiredLocalizado(typeof(Resources.Conteudo), "Designacao")]
        public string Designacao { get; set; }

        [DisplayLocalizado(typeof(Resources.Conteudo), "UrlVideo")]
        public string Url { get; set; }

        [DisplayLocalizado(typeof(Resources.Conteudo), "Duracao")]
        [RequiredLocalizado(typeof(Resources.Conteudo), "Duracao")]
        public int? Duracao { get; set; }

        public int? Ordem { get; set; }

        #region Construtores

        public ConteudoVideoModel()
        {
        }

        public ConteudoVideoModel(Core.Model.ConteudoVideo model)
        {
            this.ConteudoID = model.ConteudoID;
            this.ConteudoVideoID = model.ConteudoVideoID;
            this.Designacao = model.Designacao;
            this.Url = model.Url;
            this.Duracao = model.Duracao;
            this.Ordem = model.Ordem;
        }

        #endregion Construtores

        #region Métodos

        public Core.Model.ConteudoVideo ToBDModel()
        {
            Core.Model.ConteudoVideo conteudo = new Core.Model.ConteudoVideo();

            if (this.ConteudoVideoID.HasValue)
                conteudo.ConteudoVideoID = this.ConteudoVideoID.Value;
            if (this.ConteudoID.HasValue)
            {
                conteudo.ConteudoID = this.ConteudoID.Value;
            }
            conteudo.Designacao = this.Designacao;
            conteudo.Url = this.Url;
            conteudo.Duracao = this.Duracao;
            conteudo.Ordem = this.Ordem;
            return conteudo;
        }

        #endregion Métodos
    }
}