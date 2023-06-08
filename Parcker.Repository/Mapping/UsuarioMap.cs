using FluentNHibernate.Mapping;
using Parcker.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Parcker.Repository.Mapping
{
    class UsuarioMap : ClassMap<Usuario>
    {
        public UsuarioMap()
        {
            Id(x => x.Id);
            Map(x => x.DataCriacao);
            Map(x => x.Nome);
            Map(x => x.Senha);
            Map(x => x.Ativo);
            Map(x => x.Admin);
            Map(x => x.IdPessoa);

            References(x => x.Pessoa)
                .Column("IdPessoa")
                .Not.Insert()
                .Not.Update();

            HasMany(x => x.Auditorias).Inverse().LazyLoad();

            Table("Usuario");
        }
    }
}
