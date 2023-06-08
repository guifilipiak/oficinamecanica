using FluentNHibernate.Mapping;
using Parcker.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Parcker.Repository.Mapping
{
    class ClienteMap : ClassMap<Cliente>
    {
        public ClienteMap()
        {
            Id(x => x.Id);
            Map(x => x.DataCriacao);
            Map(x => x.Apelido);
            Map(x => x.Pontos);
            Map(x => x.IdPessoa);

            References(x => x.Pessoa)
                .Column("IdPessoa")
                .Not.Insert()
                .Not.Update();

            HasMany(x => x.OrdensServico).Inverse().LazyLoad();

            Table("Cliente");
        }
    }
}
