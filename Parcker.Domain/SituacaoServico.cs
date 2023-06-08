using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Parcker.Domain
{
    public class SituacaoServico : Base
    {
        public virtual string Descricao { get; set; }

        public virtual ICollection<OrdemServico> OrdensServico { get; set; }
    }
}
