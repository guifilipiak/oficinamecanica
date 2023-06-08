using FluentNHibernate.Mapping;
using Parcker.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Parcker.Repository.Mapping
{
    class ServicoMap : ClassMap<Servico>
    {
        public ServicoMap()
        {
            Id(x => x.Id);
            Map(x => x.DataCriacao);
            Map(x => x.Descricao);
            Map(x => x.HorasTrabalho);
            Map(x => x.ValorUnitario);

            HasMany(x => x.Itens).Inverse().LazyLoad();

            Table("Servico");
        }
    }
}
