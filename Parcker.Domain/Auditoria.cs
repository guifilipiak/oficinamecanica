using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Parcker.Domain
{
    public class Auditoria : Base
    {
        public virtual string Descricao { get; set; }
        public virtual int IdUsuario { get; set; }

        public virtual Usuario Usuario { get; set; }
    }
}
