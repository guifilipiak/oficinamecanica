using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Parcker.Domain
{
    public class Marca : Base
    {
        public virtual string Descricao { get; set; }
        public virtual string UrlLogo { get; set; }
        public virtual int? IdCategoria { get; set; }

        public virtual Categoria Categoria { get; set; }
        public virtual ICollection<Veiculo> Veiculos { get; set; }
        public virtual ICollection<Produto> Produtos { get; set; }
    }
}
