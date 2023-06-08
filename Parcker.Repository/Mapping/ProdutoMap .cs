using FluentNHibernate.Mapping;
using Parcker.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Parcker.Repository.Mapping
{
    class ProdutoMap : ClassMap<Produto>
    {
        public ProdutoMap()
        {
            Id(x => x.Id);
            Map(x => x.Nome);
            Map(x => x.DataCriacao);
            Map(x => x.Ativo);
            Map(x => x.AlertaEstoqueMinimo);
            Map(x => x.Descricao);
            Map(x => x.Estoque);
            Map(x => x.EstoqueMinimo);
            Map(x => x.ValorUnitario);
            Map(x => x.IdFornecedor);
            Map(x => x.IdMarca);
            Map(x => x.CodigoBarras);
            Map(x => x.CodigoExterno);

            References(x => x.Marca)
                .Column("IdMarca")
                .Not.Insert()
                .Not.Update();

            References(x => x.Fornecedor)
                .Column("IdFornecedor")
                .Not.Insert()
                .Not.Update();

            HasMany(x => x.Itens).Inverse().LazyLoad();

            Table("Produto");
        }
    }
}
