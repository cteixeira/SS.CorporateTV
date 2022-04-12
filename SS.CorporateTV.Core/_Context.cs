using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Text;
using System.Transactions;

namespace SS.CorporateTV.Core.Model
{
    public partial class Context
    {
        private string[] ignorarTabelas = { "LogEvento", "LogErro", "LogProcesso", "Integracao", "C_VariaveisAmbiente", "HonorarioEquipaCirurgica" };
        private string[] ignorarPropriedades = { "DataAlteracao", "AlteracaoUtilizadorID" };
        private const int maxLength = 50;

        public override int SaveChanges()
        {
            return SaveChanges(null, false);
        }

        public int SaveChanges(UtilizadorAutenticado utilizador, bool logEventos)
        {
            long utilizadorID = 0;
            int changes = 0;

            if (Utilizador != null && logEventos)
            {
                utilizadorID = utilizador.ID;

                List<ChangeLog> listaAlteracoes = new List<ChangeLog>();
                var now = DateTime.UtcNow;

                using (var scope = new TransactionScope())
                {
                    var addedEntries = ChangeTracker.Entries()
                        .Where(e => e.State == EntityState.Added
                            && !ignorarTabelas.Contains(e.Entity.GetType().Name)).ToList();

                    var modifiedEntries = ChangeTracker.Entries()
                        .Where(e => (e.State == EntityState.Deleted || e.State == EntityState.Modified)
                            && !ignorarTabelas.Contains(e.Entity.GetType().Name)).ToList();

                    if (addedEntries.Count != 0 || modifiedEntries.Count != 0)
                    {
                        if (modifiedEntries.Count > 5)
                        {
                            var entry = modifiedEntries.First();
                            listaAlteracoes.AddRange(ApplyAuditLog(entry, entry.State));
                        }
                        else
                        {
                            foreach (var entry in modifiedEntries)
                            {
                                listaAlteracoes.AddRange(ApplyAuditLog(entry, entry.State));
                            }
                            if (addedEntries.Count != 0)
                            {
                                changes = base.SaveChanges();
                                foreach (var entry in addedEntries)
                                {
                                    listaAlteracoes.AddRange(ApplyAuditLog(entry, EntityState.Added));
                                }
                            }
                        }
                    }
                    changes = base.SaveChanges();

                    scope.Complete();
                }

                if (utilizadorID != 0)
                {
                    foreach (ChangeLog changeLog in listaAlteracoes)
                    {
                        //using (Model.Context ctx = new Model.Context())
                        //{
                        this.LogEvento.Add(new Model.LogEvento()
                        {
                            Tabela = changeLog.Tabela.Length <= maxLength ? changeLog.Tabela : changeLog.Tabela.Substring(0, maxLength),
                            Identificador = changeLog.Identificador,
                            Campo = changeLog.Campo,
                            EventoID = changeLog.Estado,
                            ValorAnterior = changeLog.ValorAnterior,
                            ValorNovo = changeLog.ValorNovo,
                            UtilizadorID = utilizadorID,
                            DataHora = now
                        });
                        //ctx.SaveChanges(utilizador, false);
                        base.SaveChanges();
                        //}
                    }
                }
            }
            else
            {
                changes = base.SaveChanges();
            }

            return changes;
        }

        object GetPrimaryKeyValue(DbEntityEntry entry)
        {
            var objectStateEntry = ((IObjectContextAdapter)this).ObjectContext.ObjectStateManager.GetObjectStateEntry(entry.Entity);
            return objectStateEntry.EntityKey.EntityKeyValues[0].Value;
        }

        public class ChangeLog
        {
            public string Tabela { get; set; }
            public string Campo { get; set; }
            public string Identificador { get; set; }
            public int Estado { get; set; }
            public string ValorAnterior { get; set; }
            public string ValorNovo { get; set; }
            public DateTime DataHora { get; set; }
        }

        private List<ChangeLog> ApplyAuditLog(DbEntityEntry entry, EntityState estado)
        {
            List<ChangeLog> listaAlteracoes = new List<ChangeLog>();

            if (estado == EntityState.Added)
            {
                var entity = entry.Entity;

                if (entity != null)
                {
                    ChangeLog log = new ChangeLog
                    {
                        DataHora = DateTime.Now,
                        Tabela = entry.Entity.GetType().Name,
                        Identificador = GetPrimaryKeyValue(entry).ToString(),
                        Estado = (int)Enum.Evento.Adicionado
                    };
                    listaAlteracoes.Add(log);
                }
            }
            else if (entry.State == EntityState.Deleted)
            {
                var entity = entry.Entity.GetType().BaseType.Name;

                if (entity != null)
                {
                    string originalValue = string.Empty;

                    foreach (var prop in entry.OriginalValues.PropertyNames)
                    {
                        if (!ignorarPropriedades.Contains(prop))
                        {
                            try
                            {
                                if (entry.GetDatabaseValues()[prop] != null)
                                    originalValue += prop + ":" + entry.GetDatabaseValues()[prop].ToString() + " | ";
                            }
                            catch
                            { }
                        }
                    }
                    if (!string.IsNullOrEmpty(originalValue))
                        originalValue = originalValue.Remove(originalValue.Length - 2);

                    ChangeLog log = new ChangeLog
                    {
                        DataHora = DateTime.Now,
                        Tabela = entity,//entry.Entity.GetType().Name,
                        Identificador = GetPrimaryKeyValue(entry).ToString(),
                        Estado = (int)Enum.Evento.Apagado,
                        ValorAnterior = originalValue
                    };
                    listaAlteracoes.Add(log);
                }
            }
            else if (estado == EntityState.Modified)
            {
                var entityName = entry.Entity.GetType().Name;

                if (!ignorarTabelas.Contains(entityName))
                {
                    var primaryKey = GetPrimaryKeyValue(entry);

                    foreach (var prop in entry.OriginalValues.PropertyNames)
                    {
                        if (!ignorarPropriedades.Contains(prop))
                        {
                            try
                            {
                                string originalValue;
                                string currentValue;
                                if (entry.GetDatabaseValues()[prop] != null)
                                    originalValue = entry.GetDatabaseValues()[prop].ToString();
                                else
                                    originalValue = string.Empty;

                                if (entry.CurrentValues[prop] != null)
                                    currentValue = entry.CurrentValues[prop].ToString();
                                else
                                    currentValue = string.Empty;

                                if (originalValue != currentValue)
                                {
                                    ChangeLog log = new ChangeLog()
                                    {
                                        Tabela = entityName,
                                        Identificador = primaryKey.ToString(),
                                        Campo = prop,
                                        Estado = (int)Enum.Evento.Modificado,
                                        ValorAnterior = originalValue,
                                        ValorNovo = currentValue
                                    };
                                    listaAlteracoes.Add(log);
                                }
                            }
                            catch
                            { }
                        }
                    }
                }
            }
            return listaAlteracoes;
        }

    }
}
