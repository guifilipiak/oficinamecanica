using FluentNHibernate.Mapping;
using Parcker.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Parcker.Repository.Mapping
{
    class FotoMap : ClassMap<Foto>
    {
        public FotoMap()
        {
            Id(x => x.Id);
            Map(x => x.Descricao);
            Map(x => x.UrlFoto);
            Map(x => x.IdOrdemServico);

            References(x => x.OrdemServico)
                .Column("IdOrdemServico")
                .Not.Insert()
                .Not.Update();

            Table("Foto");
        }
    }
}
