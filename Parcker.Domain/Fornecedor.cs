using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Parcker.Domain
{
    public class Fornecedor : Base
    {
        public virtual int IdPessoa { get; set; }

        public virtual Pessoa Pessoa { get; set; }

        public virtual ICollection<Produto> Produtos { get; set; }
    }
}
