using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Parcker.Domain
{
    public class ConfiguracaoEmail : Base
    {
        public virtual string Servidor { get; set; }
        public virtual int? Porta { get; set; }
        public virtual string Remetente { get; set; }
        public virtual string Usuario { get; set; }
        public virtual string Senha { get; set; }
    }
}
