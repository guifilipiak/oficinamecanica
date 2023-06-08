using FluentNHibernate.Mapping;
using Parcker.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Parcker.Repository.Mapping
{
    class SituacaoServicoMap : ClassMap<SituacaoServico>
    {
        public SituacaoServicoMap()
        {
            Id(x => x.Id);
            Map(x => x.Descricao);

            HasMany(x => x.OrdensServico).Inverse().LazyLoad();

            Table("SituacaoServico");
        }
    }
}
