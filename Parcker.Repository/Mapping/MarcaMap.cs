using FluentNHibernate.Mapping;
using Parcker.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Parcker.Repository.Mapping
{
    class MarcaMap : ClassMap<Marca>
    {
        public MarcaMap()
        {
            Id(x => x.Id);
            Map(x => x.Descricao);
            Map(x => x.IdCategoria);
            Map(x => x.UrlLogo);

            References(x => x.Categoria)
                .Column("IdCategoria")
                .Not.Insert()
                .Not.Update();

            HasMany(x => x.Veiculos).Inverse().LazyLoad();
            HasMany(x => x.Produtos).Inverse().LazyLoad();

            Table("Marca");
        }
    }
}
