using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Parcker.Domain
{
    public class ProdServOS : Base
    {
        public virtual int Quantidade { get; set; }
        public virtual int IdOrdemServico { get; set; }
        public virtual int? IdProduto { get; set; }
        public virtual int? IdServico { get; set; }
        public virtual int Tipo { get; set; }
        public virtual decimal ValorUnitario { get; set; }
        public virtual decimal Desconto { get; set; }
        public virtual decimal ValorTotal { get; set; }

        public virtual OrdemServico OrdemServico { get; set; }
        public virtual Produto Produto { get; set; }
        public virtual Servico Servico { get; set; }
    }
}
