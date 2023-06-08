using FluentNHibernate.Mapping;
using Parcker.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Parcker.Repository.Mapping
{
    class AlertaMap : ClassMap<Alerta>
    {
        public AlertaMap()
        {
            Id(x => x.Id);
            Map(x => x.Descricao);
            Map(x => x.DataAlertaEnviado);
            Map(x => x.DataCriacao);
            Map(x => x.Dias);
            Map(x => x.Ativo);
            Map(x => x.IdVeiculo);

            References(x => x.Veiculo)
                .Column("IdVeiculo")
                .Not.Insert()
                .Not.Update();

            Table("Alerta");
        }
    }
}
