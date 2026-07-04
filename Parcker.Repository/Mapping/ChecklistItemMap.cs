using FluentNHibernate.Mapping;
using Parcker.Domain;

namespace Parcker.Repository.Mapping
{
    public class ChecklistItemMap : ClassMap<ChecklistItem>
    {
        public ChecklistItemMap()
        {
            Table("ChecklistItem");
            Id(x => x.Id).GeneratedBy.Identity();
            Map(x => x.DataCriacao);
            Map(x => x.IdChecklistVeiculo);
            Map(x => x.Sistema).Length(100);
            Map(x => x.Item).Length(200);
            Map(x => x.Verificado);
            Map(x => x.Observacao).Length(500);
        }
    }
}