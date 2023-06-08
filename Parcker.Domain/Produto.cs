using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Parcker.Domain
{
    public class Produto : Base
    {
        public virtual string Nome { get; set; }
        public virtual string Descricao { get; set; }
        public virtual decimal ValorUnitario { get; set; }
        public virtual int Estoque { get; set; }
        public virtual bool Ativo { get; set; }
        public virtual int? IdMarca { get; set; }
        public virtual int? IdFornecedor { get; set; }
        public virtual bool AlertaEstoqueMinimo { get; set; }
        public virtual int? EstoqueMinimo { get; set; }
        public virtual string CodigoExterno { get; set; }
        public virtual string CodigoBarras { get; set; }

        public virtual Marca Marca { get; set; }
        public virtual Fornecedor Fornecedor { get; set; }
        public virtual ICollection<ProdServOS> Itens { get; set; }
    }
}
