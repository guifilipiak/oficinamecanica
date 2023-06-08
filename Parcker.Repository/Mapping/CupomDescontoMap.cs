using FluentNHibernate.Mapping;
using Parcker.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Parcker.Repository.Mapping
{
    class CupomDescontoMap : ClassMap<CupomDesconto>
    {
        public CupomDescontoMap()
        {
            Id(x => x.Id);
            Map(x => x.Descricao);
            Map(x => x.Codigo);
            Map(x => x.DataCriacao);
            Map(x => x.DataValidade);
            Map(x => x.Quantidade);
            Map(x => x.ValorDesconto);
            Map(x => x.Ativo);

            HasMany(x => x.OrdemServico).Inverse().LazyLoad();

            Table("CupomDesconto");
        }
    }
}
