using FluentNHibernate.Mapping;
using Parcker.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Parcker.Repository.Mapping
{
    class ProdServOSMap : ClassMap<ProdServOS>
    {
        public ProdServOSMap()
        {
            Id(x => x.Id);
            Map(x => x.DataCriacao);
            Map(x => x.IdOrdemServico);
            Map(x => x.IdProduto);
            Map(x => x.IdServico);
            Map(x => x.Quantidade);
            Map(x => x.Tipo);
            Map(x => x.ValorUnitario);
            Map(x => x.ValorTotal);
            Map(x => x.Desconto);

            References(x => x.Servico)
                .Column("IdServico")
                .Not.Insert()
                .Not.Update();

            References(x => x.Produto)
                .Column("IdProduto")
                .Not.Insert()
                .Not.Update();

            References(x => x.OrdemServico)
                .Column("IdOrdemServico")
                .Not.Insert()
                .Not.Update();

            Table("ProdServOS");
        }
    }
}
