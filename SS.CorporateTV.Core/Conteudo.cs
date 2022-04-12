using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;

namespace SS.CorporateTV.Core
{
    public class Conteudo : _Base<Model.Conteudo>
    {
        public int ID { get; set; }
        public string Designacao { get; set; }

        public new List<Enum.Permissao> Permissoes { get { return ConstroiPermissoes(); } }

        public Conteudo(UtilizadorAutenticado utilizador)
            : base(utilizador) 
        {
        }

        public IEnumerable<Model.Conteudo> Lista(string nome, long empresaID)
        {
            if (!Permissoes.Contains(Enum.Permissao.Visualizar))
                throw new Exceptions.SemPermissao();

            using (Model.Context ctx = new Model.Context())
            {
                var query = ctx.Conteudo
                    .Where(u => (string.IsNullOrEmpty(nome) || u.Designacao.Contains(nome)) && u.EmpresaID == empresaID )
                    .AsQueryable();

                return query
                    .OrderBy(p => p.Designacao)
                    .ToList();
            }
        }

        public IEnumerable<Model.Conteudo> Lista(long empresaID)
        {
            if (!Permissoes.Contains(Enum.Permissao.Visualizar))
                throw new Exceptions.SemPermissao();

            using (Model.Context ctx = new Model.Context())
            {
                var query = ctx.Conteudo
                    .Where(u => u.EmpresaID == empresaID || !u.EmpresaID.HasValue)
                    .AsQueryable();

                return query
                    .OrderBy(p => p.Designacao)
                    .ToList();
            }
        }

        public Model.Conteudo Insere(Model.Conteudo obj)
        {
            if (!Permissoes.Contains(Enum.Permissao.Gravar))
                throw new Exceptions.SemPermissao();

            if (!ObjectoValidoInsercao(obj))
                throw new Exceptions.DadosIncorrectos();

            obj.DataAlteracao = DateTime.Now;

            using (Model.Context db = new Model.Context())
            {
                DbSet<Model.Conteudo> table = db.Set<Model.Conteudo>();
                table.Add(obj);
                db.SaveChanges(utilizador, true);

                return obj;
            }
        }

        public override void Actualizar(Model.Conteudo obj)
        {
            if (!Permissoes.Contains(Enum.Permissao.Gravar))
                throw new Exceptions.SemPermissao();

            if (!ObjectoValido(obj))
                throw new Exceptions.DadosIncorrectos();

            using (Model.Context db = new Model.Context())
            {
                DbSet<Model.Conteudo> table = db.Set<Model.Conteudo>();
                ConteudoVideo objVideo = new ConteudoVideo(utilizador);
                ConteudoImagem objImagem = new ConteudoImagem(utilizador);
                var cnt = table.SingleOrDefault(c => c.ConteudoID == obj.ConteudoID);
                cnt.Designacao = obj.Designacao;
                cnt.EmpresaID = obj.EmpresaID;
                cnt.Tipo = obj.Tipo;
                cnt.DataAlteracao = DateTime.Now;
                //obj.QRCode = cnt.QRCode;
                if (obj.Tipo == (short)Core.Enum.TipoConteudo.Canal)
                {
                    if (cnt.ConteudoVideo != null && cnt.ConteudoVideo.Count > 1)
                    {
                        foreach (var item in cnt.ConteudoVideo)
                            objVideo.Apagar(item.ConteudoVideoID);
                    }
                    else if (cnt.ConteudoVideo != null && cnt.ConteudoVideo.Count == 1)
                    {
                        var stream = cnt.ConteudoVideo.First();
                        stream.Designacao = obj.ConteudoVideo.First().Designacao;
                        stream.Url = obj.ConteudoVideo.First().Url;
                    }
                    else if (cnt.ConteudoVideo == null || cnt.ConteudoVideo.Count == 0)
                    {
                        cnt.ConteudoVideo = obj.ConteudoVideo;
                    }
                    
                }
                else if(obj.Tipo == (short)Core.Enum.TipoConteudo.Imagem)
                {
                    foreach (var item in cnt.ConteudoVideo)
                        objVideo.Apagar(item.ConteudoVideoID);
                }
                else if (obj.Tipo == (short)Core.Enum.TipoConteudo.Video)
                {
                    foreach (var item in cnt.ConteudoImagem)
                        objImagem.Apagar(item.ConteudoImagemID);
                }
                table.Attach(cnt);
                db.Entry(cnt).State = EntityState.Modified;
                db.SaveChanges(utilizador, true);
            }
        }

        public void ActualizarQRCode(Model.Conteudo obj)
        {
            if (!Permissoes.Contains(Enum.Permissao.Gravar))
                throw new Exceptions.SemPermissao();

            if (obj.QRCode == null)
                return;

            using (Model.Context db = new Model.Context())
            {
                DbSet<Model.Conteudo> table = db.Set<Model.Conteudo>();
                var cnt = table.SingleOrDefault(c => c.ConteudoID == obj.ConteudoID);
                if (cnt.QRCode != obj.QRCode)
                {
                    cnt.DataAlteracao = DateTime.Now;
                    cnt.QRCode = obj.QRCode;
                    table.Attach(cnt);
                    db.Entry(cnt).State = EntityState.Modified;
                    db.SaveChanges(utilizador, true);
                }
            }
        }

        public void ActualizarTipo(Model.Conteudo obj)
        {
            if (!Permissoes.Contains(Enum.Permissao.Gravar))
                throw new Exceptions.SemPermissao();           

            using (Model.Context db = new Model.Context())
            {
                DbSet<Model.Conteudo> table = db.Set<Model.Conteudo>();
                var cnt = table.SingleOrDefault(c => c.ConteudoID == obj.ConteudoID);
                if (cnt.Tipo != obj.Tipo)
                {
                    cnt.DataAlteracao = DateTime.Now;
                    cnt.Tipo = obj.Tipo;
                    table.Attach(cnt);
                    db.Entry(cnt).State = EntityState.Modified;
                    db.SaveChanges(utilizador, true);
                }
            }
        }

        public void ActualizarData(long id)
        {
            if (!Permissoes.Contains(Enum.Permissao.Gravar))
                throw new Exceptions.SemPermissao();

            using (Model.Context db = new Model.Context())
            {
                DbSet<Model.Conteudo> tabelaConteudo = db.Set<Model.Conteudo>();
                Model.Conteudo conteudo = tabelaConteudo.Find(id);
                conteudo.DataAlteracao = DateTime.Now;
                tabelaConteudo.Attach(conteudo);
                db.Entry(conteudo).State = EntityState.Modified;
                db.SaveChanges(utilizador, true);
            }
        }

        public override void Apagar(long id)
        {
            if (!Permissoes.Contains(Enum.Permissao.Apagar))
                throw new Exceptions.SemPermissao();

            using (Model.Context db = new Model.Context())
            {
                DbSet<Model.Conteudo> tabelaConteudo = db.Set<Model.Conteudo>();
                Model.Conteudo conteudo = tabelaConteudo.Find(id);
                tabelaConteudo.Remove(conteudo);
                db.SaveChanges(utilizador, true);
            }
        }

        #region Métodos auxiliares

        internal bool ObjectoValidoInsercao(Model.Conteudo obj)
        {
            if (VerificaDesignacaoRepetida(obj))
                throw new Exceptions.DesignacaoRepetida();

            return !string.IsNullOrEmpty(obj.Designacao);
        }

        internal override bool ObjectoValido(Model.Conteudo obj)
        {
            if (VerificaDesignacaoRepetida(obj))
                throw new Exceptions.DesignacaoRepetida();

            return !string.IsNullOrEmpty(obj.Designacao);
        }

        private bool VerificaDesignacaoRepetida(Model.Conteudo obj)
        {
            using (Model.Context ctx = new Model.Context())
            {
                return ctx.Conteudo.Any(i =>
                    (obj.ConteudoID < 1 || obj.ConteudoID != i.ConteudoID)
                    && (i.EmpresaID == utilizador.EmpresaID)
                    && (i.Designacao == obj.Designacao));
            }
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

        #endregion Métodos auxiliares

    }
}

