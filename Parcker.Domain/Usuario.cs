using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Parcker.Domain
{
    public class Usuario : Base
    {
        public virtual string Nome { get; set; }
        public virtual string Senha { get; set; }
        public virtual bool Ativo { get; set; }
        public virtual bool Admin { get; set; }
        public virtual int? IdPessoa { get; set; }

        public virtual Pessoa Pessoa { get; set; }
        public virtual ICollection<Auditoria> Auditorias { get; set; }
    }
}
