using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Parcker.Domain
{
    public class Pessoa : Base
    {
        public virtual int Tipo { get; set; }
        public virtual string RazaoSocial { get; set; }
        public virtual string Fantasia { get; set; }
        public virtual string CNPJ { get; set; }
        public virtual string Nome { get; set; }
        public virtual string CPF { get; set; }
        public virtual string RG { get; set; }
        public virtual string CEP { get; set; }
        public virtual string Pais { get; set; }
        public virtual string UF { get; set; }
        public virtual string Cidade { get; set; }
        public virtual string Bairro { get; set; }
        public virtual string Endereco { get; set; }
        public virtual string Telefone1 { get; set; }
        public virtual string Telefone2 { get; set; }
        public virtual string Email { get; set; }
        public virtual int? Numero { get; set; }
        public virtual string Complemento { get; set; }

        public virtual Cliente Cliente { get; set; }
        public virtual Usuario Usuario { get; set; }
        public virtual Fornecedor Fornecedor { get; set; }

        public virtual ICollection<Veiculo> Veiculos { get; set; }
        public virtual ICollection<PagarReceber> PagarReceber { get; set; }
    }
}
