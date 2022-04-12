using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SS.CorporateTV.Core
{
    public class Enum
    {
        public enum TipoConteudo
        {
            Imagem = 1,
            Video = 2,
            Canal = 3
        }

        public enum PerfilUtilizador
        {
            SysAdmin = 1,
            Configurador = 2
        }

        public enum Permissao
        {
            Visualizar,
            Gravar,
            Apagar
        }

        public enum Evento
        {
            Modificado = 1,
            Adicionado,
            Apagado
        }
    }
}
