using FluentNHibernate.Mapping;
using Parcker.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Parcker.Repository.Mapping
{
    class CategoriaMap : ClassMap<Categoria>
    {
        public CategoriaMap()
        {
            Id(x => x.Id);
            Map(x => x.Descricao);

            HasMany(x => x.Marcas).Inverse().LazyLoad();

            Table("Categoria");
        }
    }
}
