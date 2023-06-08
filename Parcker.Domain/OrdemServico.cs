using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Parcker.Domain
{
    public class OrdemServico : Base
    {
        public virtual DateTime? DataFinalizacao { get; set; }
        public virtual string DescricaoServico { get; set; }
        public virtual string Observacoes { get; set; }
        public virtual decimal? Entrada { get; set; }
        public virtual decimal? Desconto { get; set; }
        public virtual decimal SubTotal { get; set; }
        public virtual decimal Total { get; set; }
        public virtual int? KM { get; set; }
        public virtual int IdSituacaoServico { get; set; }
        public virtual int IdVeiculo { get; set; }
        public virtual int? IdCupomDesconto { get; set; }
        public virtual int IdCliente { get; set; }

        public virtual SituacaoServico SituacaoServico { get; set; }
        public virtual Veiculo Veiculo { get; set; }
        public virtual CupomDesconto CupomDesconto { get; set; }
        public virtual Cliente Cliente { get; set; }

        public virtual ICollection<Foto> Fotos { get; set; }
        public virtual ICollection<ProdServOS> Itens { get; set; }
        public virtual ICollection<HistoricoPagamento> HistoricoPagamentos { get; set; }
    }
}
