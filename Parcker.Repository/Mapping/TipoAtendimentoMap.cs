using FluentNHibernate.Mapping;
using Parcker.Domain;

namespace Parcker.Repository.Mapping
{
    public class TipoAtendimentoMap : ClassMap<TipoAtendimento>
    {
        public TipoAtendimentoMap()
        {
            Table("TipoAtendimento");
            Id(x => x.Id).GeneratedBy.Identity();
            Map(x => x.Descricao).Not.Nullable().Length(100);
            Map(x => x.DataCriacao).Not.Nullable();
        }
    }
}