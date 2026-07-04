using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Parcker.Domain
{
    public class Funcionario : Base
    {
        public virtual string Cargo { get; set; }
        public virtual bool Ativo { get; set; }
        public virtual int IdPessoa { get; set; }

        public virtual Pessoa Pessoa { get; set; }
        public virtual ICollection<OrdemServico> OrdensServico { get; set; }
    }
}