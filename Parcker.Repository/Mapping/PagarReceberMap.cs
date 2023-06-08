using FluentNHibernate.Mapping;
using Parcker.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Parcker.Repository.Mapping
{
    public class PagarReceberMap : ClassMap<PagarReceber>
    {
        public PagarReceberMap()
        {
            Id(x => x.Id);
            Map(x => x.IdClassificacao);
            Map(x => x.IdFormaPagamento);
            Map(x => x.IdPessoa);
            Map(x => x.IdSituacaoConta);
            Map(x => x.IdTipoConta);
            Map(x => x.GrupoParcela);
            Map(x => x.Parcela);
            Map(x => x.TotalParcela);
            Map(x => x.Valor);
            Map(x => x.Descricao);
            Map(x => x.DataVencimento);
            Map(x => x.DataPagamento);
            Map(x => x.DataCriacao);
            Map(x => x.Recorrente);

            References(x => x.Classificacao)
                .Column("IdClassificacao")
                .Not.Insert()
                .Not.Update();

            References(x => x.FormasPagamento)
                .Column("IdFormaPagamento")
                .Not.Insert()
                .Not.Update();

            References(x => x.Pessoa)
                .Column("IdPessoa")
                .Not.Insert()
                .Not.Update();

            References(x => x.SituacaoConta)
                .Column("IdSituacaoConta")
                .Not.Insert()
                .Not.Update();

            References(x => x.TipoConta)
                .Column("IdTipoConta")
                .Not.Insert()
                .Not.Update();

            HasMany(x => x.HistoricoPagamentos).Inverse().LazyLoad();

            Table("PagarReceber");
        }
    }
}
