using System;
using System.Configuration;
using System.Data.Entity.Validation;
using System.Net.Mail;

namespace SS.CorporateTV.Core.Utils
{
    public class Util
    {
        private static SimpleSolutions.wcSecure.Decifra wcSec = new SimpleSolutions.wcSecure.Decifra();

        public static string Decifrar(string cifra)
        {
            return (wcSec.Decifrar(cifra));
        }

        #region Tratamento de Erro

        public enum ImportanciaErro
        {
            Normal,
            Critico
        }

        public static string TratamentoErro(ImportanciaErro? importancia, string origem, Exception erro, UtilizadorAutenticado utilizador)
        {
            String erroMsg = erro.ToString();

            EscreveTabelaErros(Convert.ToInt16(importancia), origem, erroMsg, utilizador);

            if (importancia == ImportanciaErro.Critico)
            {
                string msg = String.Format("{0}  - Utilizador: {1}", erroMsg, utilizador);
                EnviaEmailErro(msg.Replace("\r\n", ""));
            }

            return erro.Message.Replace("\r\n", "");
        }

        private static void EscreveTabelaErros(short importancia, string origemErro, string descricao, UtilizadorAutenticado utilizador)
        {
            try
            {
                using (Model.Context ctx = new Model.Context())
                {
                    ctx.LogErro.Add(new Model.LogErro()
                    {
                        Importancia = importancia,
                        Origem = origemErro,
                        Descricao = descricao,
                        DataHora = DateTime.Now,
                        UserName = (utilizador != null ? utilizador.Nome : string.Empty)
                    });
                    ctx.SaveChanges(utilizador, false);
                }
            }
            catch (DbEntityValidationException e)
            {
                foreach (var eve in e.EntityValidationErrors)
                {
                    Console.WriteLine("Entity of type \"{0}\" in state \"{1}\" has the following validation errors:",
                        eve.Entry.Entity.GetType().Name, eve.Entry.State);
                    foreach (var ve in eve.ValidationErrors)
                    {
                        Console.WriteLine("- Property: \"{0}\", Error: \"{1}\"",
                            ve.PropertyName, ve.ErrorMessage);
                    }
                }
                throw;
            }
        }

        public static bool EnviaEmailErro(string msg)
        {
            try
            {
                MailMessage ErrorMail = new MailMessage(ConfigurationManager.AppSettings["AppEmail"],
                   ConfigurationManager.AppSettings["SysAdminEmail"],
                   ConfigurationManager.AppSettings["ErrorEmailSubject"],
                   msg);

                ErrorMail.IsBodyHtml = false;
                SmtpClient smtp = new SmtpClient(ConfigurationManager.AppSettings["SmtpServer"]);
                AutenticaSMTP(smtp);
                smtp.Send(ErrorMail);
                return true;
            }
            catch
            {
                return false;
            }
        }

        #endregion


        #region Funções Diversas

        private static void AutenticaSMTP(SmtpClient smtp)
        {
            if (Convert.ToBoolean(ConfigurationManager.AppSettings["AutenticaSMTP"]))
            {
                SimpleSolutions.wcSecure.Decifra decifra = new SimpleSolutions.wcSecure.Decifra();
                smtp.Credentials = new System.Net.NetworkCredential(decifra.Decifrar(ConfigurationManager.AppSettings["SmtpUser"]), decifra.Decifrar(ConfigurationManager.AppSettings["SmtpPassword"]));
            }
        }

        private static short[] week = { /*sunday*/6, /*monday*/0, 1, 2, 3, 4, 5 };
        public static short AcertoDiaSemana(int dayofweekSys)
        {
            return week[dayofweekSys];
        }

        #endregion
    }
}
