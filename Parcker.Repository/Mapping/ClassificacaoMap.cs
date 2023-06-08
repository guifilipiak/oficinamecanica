using FluentNHibernate.Mapping;
using Parcker.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Parcker.Repository.Mapping
{
    public class ClassificacaoMap : ClassMap<Classificacao>
    {
        public ClassificacaoMap()
        {
            Id(x => x.Id);
            Map(x => x.Descricao);
            Map(x => x.IdTipoConta);

            References(x => x.TipoConta)
                .Column("IdTipoConta")
                .Not.Insert()
                .Not.Update();

            HasMany(x => x.PagarReceber).Inverse().LazyLoad();

            Table("Classificacao");
        }
    }
}
