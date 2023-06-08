using FluentNHibernate.Mapping;
using Parcker.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Parcker.Repository.Mapping
{
    class OrdemServicoMap : ClassMap<OrdemServico>
    {
        public OrdemServicoMap()
        {
            Id(x => x.Id);
            Map(x => x.DataCriacao);
            Map(x => x.DataFinalizacao);
            Map(x => x.DescricaoServico);
            Map(x => x.Observacoes);
            Map(x => x.Entrada);
            Map(x => x.Desconto);
            Map(x => x.SubTotal);
            Map(x => x.Total);
            Map(x => x.KM);
            Map(x => x.IdCupomDesconto);
            Map(x => x.IdSituacaoServico);
            Map(x => x.IdVeiculo);
            Map(x => x.IdCliente);

            References(x => x.Veiculo)
                .Column("IdVeiculo")
                .Not.Insert()
                .Not.Update();

            References(x => x.CupomDesconto)
                .Column("IdCupomDesconto")
                .Not.Insert()
                .Not.Update();

            References(x => x.Cliente)
                .Column("IdCliente")
                .Not.Insert()
                .Not.Update();

            References(x => x.SituacaoServico)
                .Column("IdSituacaoServico")
                .Not.Insert()
                .Not.Update();

            HasMany(x => x.Itens).Inverse().LazyLoad();
            HasMany(x => x.Fotos).Inverse().LazyLoad();
            HasMany(x => x.HistoricoPagamentos).Inverse().LazyLoad();

            Table("OrdemServico");
        }
    }
}
