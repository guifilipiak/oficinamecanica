using FluentNHibernate.Mapping;
using Parcker.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Parcker.Repository.Mapping
{
    public class HistoricoPagamentoMap : ClassMap<HistoricoPagamento>
    {
        public HistoricoPagamentoMap()
        {
            Id(x => x.Id);
            Map(x => x.DataCriacao);
            Map(x => x.IdOrdemServico);
            Map(x => x.IdPagarReceber);

            References(x => x.OrdemServico)
                .Column("IdOrdemServico")
                .Not.Insert()
                .Not.Update();

            References(x => x.PagarReceber)
                .Column("IdPagarReceber")
                .Not.Insert()
                .Not.Update();
        }
    }
}
