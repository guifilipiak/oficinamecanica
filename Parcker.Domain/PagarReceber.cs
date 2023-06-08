using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Parcker.Domain
{
    public class PagarReceber : Base
    {
        public virtual DateTime? DataPagamento { get; set; }
        public virtual DateTime? DataVencimento { get; set; }
        public virtual string Descricao { get; set; }
        public virtual decimal Valor { get; set; }
        public virtual int Parcela { get; set; }
        public virtual int TotalParcela { get; set; }
        public virtual int GrupoParcela { get; set; }
        public virtual int Recorrente { get; set; }
        public virtual int IdFormaPagamento { get; set; }
        public virtual int IdTipoConta { get; set; }
        public virtual int IdSituacaoConta { get; set; }
        public virtual int? IdPessoa { get; set; }
        public virtual int IdClassificacao { get; set; }

        public virtual TipoConta TipoConta { get; set; }
        public virtual SituacaoConta SituacaoConta { get; set; }
        public virtual FormasPagamento FormasPagamento { get; set; }
        public virtual Pessoa Pessoa { get; set; }
        public virtual Classificacao Classificacao { get; set; }

        public virtual ICollection<HistoricoPagamento> HistoricoPagamentos { get; set; }
    }
}
