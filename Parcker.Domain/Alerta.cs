using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Parcker.Domain
{
    public class Alerta : Base
    {
        public virtual string Descricao { get; set; }
        public virtual DateTime? DataAlertaEnviado { get; set; }
        public virtual int? Dias { get; set; }
        public virtual int IdVeiculo { get; set; }
        public virtual bool Ativo { get; set; }

        public virtual Veiculo Veiculo { get; set; }
    }
}
