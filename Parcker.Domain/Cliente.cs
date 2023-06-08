using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Parcker.Domain
{
    public class Cliente : Base
    {
        public virtual string Apelido { get; set; }
        public virtual int? Pontos { get; set; }
        public virtual int IdPessoa { get; set; }

        public virtual Pessoa Pessoa { get; set; }

        public virtual ICollection<OrdemServico> OrdensServico { get; set; }
    }
}
