using FluentNHibernate.Mapping;
using Parcker.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Parcker.Repository.Mapping
{
    public class FormasPagamentoMap : ClassMap<FormasPagamento>
    {
        public FormasPagamentoMap()
        {
            Id(x => x.Id);
            Map(x => x.Descricao);
            Map(x => x.Ativo);

            HasMany(x => x.PagarReceber).Inverse().LazyLoad();

            Table("FormasPagamento");
        }
    }
}
