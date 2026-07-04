using FluentNHibernate.Mapping;
using Parcker.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Parcker.Repository.Mapping
{
    class FuncionarioMap : ClassMap<Funcionario>
    {
        public FuncionarioMap()
        {
            Id(x => x.Id);
            Map(x => x.Cargo);
            Map(x => x.Ativo);
            Map(x => x.IdPessoa);
            Map(x => x.DataCriacao);

            References(x => x.Pessoa)
                .Column("IdPessoa")
                .Not.Insert()
                .Not.Update();

            HasMany(x => x.OrdensServico).Inverse().LazyLoad();

            Table("Funcionario");
        }
    }
}