using FluentNHibernate.Mapping;
using Parcker.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Parcker.Repository.Mapping
{
    class AuditoriaMap : ClassMap<Auditoria>
    {
        public AuditoriaMap()
        {
            Id(x => x.Id);
            Map(x => x.DataCriacao);
            Map(x => x.Descricao);
            Map(x => x.IdUsuario);

            References(x => x.Usuario)
                .Column("IdUsuario")
                .Not.Insert()
                .Not.Update();

            Table("Auditoria");
        }
    }
}
