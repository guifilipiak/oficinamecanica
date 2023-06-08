using FluentNHibernate.Mapping;
using Parcker.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Parcker.Repository.Mapping
{
    public class TipoContaMap : ClassMap<TipoConta>
    {
        public TipoContaMap()
        {
            Id(x => x.Id);
            Map(x => x.Descricao);

            HasMany(x => x.PagarReceber).Inverse().LazyLoad();

            Table("TipoConta");
        }
    }
}
