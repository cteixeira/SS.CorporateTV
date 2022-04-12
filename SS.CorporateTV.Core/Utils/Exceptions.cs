using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SS.CorporateTV.Core
{
    public class Exceptions
    {
        public class UserNameRepetido : Exception { }

        public class DadosIncorrectos : Exception { }

        public class SemPermissao : Exception { }

        public class DesignacaoRepetida : Exception { }

        public class PasswordIncorrecta : Exception { }

        public class NovaPasswordDiferenteConfirmacao : Exception { }
    }
}
