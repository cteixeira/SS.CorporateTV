using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using SS.CorporateTV.Core;
using Model = SS.CorporateTV.Core.Model;
using Enum = SS.CorporateTV.Core.Enum;
using System.Web.Script.Serialization;

namespace SS.CorporateTV.FO.Models
{
    public class ConfigDone
    {
        public long TvID { get; set; }

        public Conteudo ConteudoNoAr { get; set; }
    }

    public class Conteudo
    {
	    public long ProgramacaoIDNoAr { get; set; }
	    public Enum.TipoConteudo TipoConteudoAr { get; set; }
	    public string Stream { get; set; }
        //public Dictionary<byte[], int> Imagens { get; set; }
        public List<string> Imagens { get; set; }
        public string ImagensJson { get; set; }
        public List<string> Videos { get; set; }
        public string VideosJson { get; set; }
        public List<int> DuracaoImagens { get; set; }
        //public TimeSpan Inicio { get; set; }
        //public TimeSpan Fim { get; set; }
        public string TempoParaFim { get; set; }

        public long ConteudoIDNoAr { get; set; }
        public bool Modificado { get; set; }

        public Conteudo(Model.ProgramacaoAgendamento modelAgendamento)
        {
            if (modelAgendamento != null)
            {
                this.ProgramacaoIDNoAr = modelAgendamento.ProgramacaoID;
                this.ConteudoIDNoAr = modelAgendamento.Conteudo.ConteudoID;
                this.TipoConteudoAr = (Enum.TipoConteudo)modelAgendamento.Conteudo.Tipo;

                JavaScriptSerializer serializer = new JavaScriptSerializer();
                serializer.MaxJsonLength = Int32.MaxValue;

                switch ((Enum.TipoConteudo)modelAgendamento.Conteudo.Tipo)
                {
                    case Enum.TipoConteudo.Video:
                    case Enum.TipoConteudo.Canal:
                        ConteudoVideo conteudoVideo = new ConteudoVideo(ControladorSite.Utilizador);
                        var videos = conteudoVideo.Lista(modelAgendamento.ConteudoID);

                        Videos = new List<string>();
                        //DuracaoImagens = new List<int>();
                        foreach (var video in videos)
                        {
                            Videos.Add(video.Url);
                            //DuracaoImagens.Add(video.Duracao.HasValue ? video.Duracao.Value : 0);
                        }
                        VideosJson = serializer.Serialize(Videos);
                        break;
                    case Enum.TipoConteudo.Imagem:
                        ConteudoImagem conteudoImagem = new ConteudoImagem(ControladorSite.Utilizador);
                        var imagens = conteudoImagem.Lista(modelAgendamento.ConteudoID);

                        Imagens = new List<string>();
                        DuracaoImagens = new List<int>();
                        
                        foreach (var imagem in imagens)
                        {
                            Imagens.Add(Convert.ToBase64String(imagem.Imagem));
                            DuracaoImagens.Add((int)TimeSpan.FromMinutes(imagem.Duracao).TotalMilliseconds);
                            //Imagens.Add(imagem.Imagem, imagem.Duracao.ToString());
                        }
                        ImagensJson = serializer.Serialize(Imagens);
                        break;
                }

                this.TempoParaFim = modelAgendamento.Fim.Subtract(DateTime.Now.TimeOfDay).TotalMilliseconds.ToString("0");
                if (modelAgendamento.Conteudo.DataAlteracao > DateTime.Now.AddMilliseconds(-Util.IntervaloAtualizarStream) ||
                    modelAgendamento.Programacao.DataAlteracao > DateTime.Now.AddMilliseconds(-Util.IntervaloAtualizarStream))
                    this.Modificado = true;
                else
                    this.Modificado = false;
                //this.Modificado = modelAgendamento.Conteudo.DataAlteracao > DateTime.Now.AddMilliseconds(-Util.IntervaloAtualizarStream) ? true : false;
            }

        }
    }

}