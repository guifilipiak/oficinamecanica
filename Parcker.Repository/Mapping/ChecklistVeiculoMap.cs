using FluentNHibernate.Mapping;
using Parcker.Domain;

namespace Parcker.Repository.Mapping
{
    public class ChecklistVeiculoMap : ClassMap<ChecklistVeiculo>
    {
        public ChecklistVeiculoMap()
        {
            Table("ChecklistVeiculo");
            Id(x => x.Id).GeneratedBy.Identity();
            Map(x => x.DataCriacao);
            Map(x => x.IdOrdemServico);
            Map(x => x.Observacoes).Length(1000);

            References(x => x.OrdemServico).Column("IdOrdemServico").Not.Insert().Not.Update();
            HasMany(x => x.Itens).KeyColumn("IdChecklistVeiculo").Cascade.All();
        }
    }
}