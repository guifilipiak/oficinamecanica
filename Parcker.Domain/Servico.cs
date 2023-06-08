using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Parcker.Domain
{
    public class Servico : Base
    {
        public virtual string Descricao { get; set; }
        public virtual int HorasTrabalho { get; set; }
        public virtual decimal ValorUnitario { get; set; }

        public virtual ICollection<ProdServOS> Itens { get; set; }
    }
}
