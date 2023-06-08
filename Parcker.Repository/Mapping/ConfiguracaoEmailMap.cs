using FluentNHibernate.Mapping;
using Parcker.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Parcker.Repository.Mapping
{
    public class ConfiguracaoEmailMap : ClassMap<ConfiguracaoEmail>
    {
        public ConfiguracaoEmailMap()
        {
            Id(x => x.Id);
            Map(x => x.DataCriacao);
            Map(x => x.Remetente);
            Map(x => x.Senha);
            Map(x => x.Servidor);
            Map(x => x.Porta);
            Map(x => x.Usuario);

            Table("ConfiguracaoEmail");
        }
    }
}
