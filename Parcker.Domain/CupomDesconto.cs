using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Parcker.Domain
{
    public class CupomDesconto : Base
    {
        public virtual string Codigo { get; set; }
        public virtual string Descricao { get; set; }
        public virtual decimal ValorDesconto { get; set; }
        public virtual DateTime DataValidade { get; set; }
        public virtual int Quantidade { get; set; }
        public virtual bool Ativo { get; set; }

        public virtual ICollection<OrdemServico> OrdemServico { get; set; }
    }
}
