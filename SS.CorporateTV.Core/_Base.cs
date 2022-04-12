using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;

namespace SS.CorporateTV.Core
{
    public abstract class _Base<T> where T : class
    {
        protected UtilizadorAutenticado utilizador { get; set; }

        public List<Enum.Permissao> Permissoes { get { return ConstroiPermissoes(); } }

        protected _Base(UtilizadorAutenticado utilizador)
        {
            this.utilizador = utilizador;
        }

        public virtual IEnumerable<T> Lista()
        {
            if (!Permissoes.Contains(Enum.Permissao.Visualizar))
                throw new Exceptions.SemPermissao();

            using (Model.Context db = new Model.Context())
            {
                DbSet<T> table = db.Set<T>();
                return table.ToList<T>();
            }
        }

        public virtual T Abrir(long id)
        {
           // if (!Permissoes.Contains(Enum.Permissao.Visualizar))
           //     throw new Exceptions.SemPermissao();

            using (Model.Context db = new Model.Context())
            {
                DbSet<T> table = db.Set<T>();
                return table.Find(id);
            }
        }

        public virtual T Inserir(T obj)
        {
            if (!Permissoes.Contains(Enum.Permissao.Gravar))
                throw new Exceptions.SemPermissao();

            if (!ObjectoValido(obj))
                throw new Exceptions.DadosIncorrectos();

            using (Model.Context db = new Model.Context())
            {
                DbSet<T> table = db.Set<T>();
                table.Add(obj);
                db.SaveChanges(utilizador, true);
                return obj;
            }
        }

        public virtual void Actualizar(T obj)
        {
            if (!Permissoes.Contains(Enum.Permissao.Gravar))
                throw new Exceptions.SemPermissao();

            if (!ObjectoValido(obj))
                throw new Exceptions.DadosIncorrectos();

            using (Model.Context db = new Model.Context())
            {
                DbSet<T> table = db.Set<T>();
                table.Attach(obj);
                db.Entry(obj).State = EntityState.Modified;
                db.SaveChanges(utilizador, true);
            }
        }

        public virtual void Apagar(long id)
        {
            if (!Permissoes.Contains(Enum.Permissao.Apagar))
                throw new Exceptions.SemPermissao();

            using (Model.Context db = new Model.Context())
            {
                DbSet<T> table = db.Set<T>();
                T existing = table.Find(id);
                table.Remove(existing);
                db.SaveChanges(utilizador, true);
            }
        }

        internal abstract bool ObjectoValido(T obj);

        internal abstract List<Enum.Permissao> ConstroiPermissoes();

    }
}
