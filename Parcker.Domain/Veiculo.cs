using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Parcker.Domain
{
    public class Veiculo : Base
    {
        public virtual string Modelo { get; set; }
        public virtual string Chassi { get; set; }
        public virtual string Placa { get; set; }
        public virtual string Renavam { get; set; }
        public virtual string Cor { get; set; }
        public virtual int? Ano { get; set; }
        public virtual int? AnoFabricacao { get; set; }
        public virtual int? KM { get; set; }
        public virtual int IdPessoa { get; set; }
        public virtual int IdMarca { get; set; }

        public virtual Pessoa Pessoa { get; set; }
        public virtual Marca Marca { get; set; }

        public virtual ICollection<Alerta> Alertas { get; set; }
        public virtual ICollection<OrdemServico> OrdensServico { get; set; }
    }
}
