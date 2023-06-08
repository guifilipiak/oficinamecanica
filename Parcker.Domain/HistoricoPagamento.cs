using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Parcker.Domain
{
    public class HistoricoPagamento : Base
    {
        public virtual int IdOrdemServico { get; set; }
        public virtual int IdPagarReceber { get; set; }

        public virtual OrdemServico OrdemServico { get; set; }
        public virtual PagarReceber PagarReceber { get; set; }
    }
}
