using FluentNHibernate.Mapping;
using Parcker.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Parcker.Repository.Mapping
{
    class FornecedorMap : ClassMap<Fornecedor>
    {
        public FornecedorMap()
        {
            Id(x => x.Id);
            Map(x => x.IdPessoa);

            References(x => x.Pessoa)
                .Column("IdPessoa")
                .Not.Insert()
                .Not.Update();

            HasMany(x => x.Produtos).Inverse().LazyLoad();

            Table("Fornecedor");
        }
    }
}
