using FluentNHibernate.Mapping;
using Parcker.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Parcker.Repository.Mapping
{
    class VeiculoMap : ClassMap<Veiculo>
    {
        public VeiculoMap()
        {
            Id(x => x.Id);
            Map(x => x.Placa);
            Map(x => x.Modelo);
            Map(x => x.KM);
            Map(x => x.DataCriacao);
            Map(x => x.Chassi);
            Map(x => x.Cor);
            Map(x => x.AnoFabricacao);
            Map(x => x.Ano);
            Map(x => x.Renavam);
            Map(x => x.IdMarca);
            Map(x => x.IdPessoa);

            References(x => x.Pessoa)
                .Column("IdPessoa")
                .Not.Insert()
                .Not.Update();

            References(x => x.Marca)
                .Column("IdMarca")
                .Not.Insert()
                .Not.Update();

            HasMany(x => x.Alertas).Inverse().LazyLoad();
            HasMany(x => x.OrdensServico).Inverse().LazyLoad();

            Table("Veiculo");
        }
    }
}
