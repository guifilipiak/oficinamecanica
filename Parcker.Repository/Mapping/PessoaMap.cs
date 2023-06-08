using FluentNHibernate.Mapping;
using Parcker.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Parcker.Repository.Mapping
{
    class PessoaMap : ClassMap<Pessoa>
    {
        public PessoaMap()
        {
            Id(x => x.Id);
            Map(x => x.Nome);
            Map(x => x.Numero);
            Map(x => x.Pais);
            Map(x => x.RazaoSocial);
            Map(x => x.RG);
            Map(x => x.Telefone1);
            Map(x => x.Telefone2);
            Map(x => x.Tipo);
            Map(x => x.UF);
            Map(x => x.Bairro);
            Map(x => x.Cidade);
            Map(x => x.Fantasia);
            Map(x => x.Email);
            Map(x => x.Endereco);
            Map(x => x.DataCriacao);
            Map(x => x.CPF);
            Map(x => x.Complemento);
            Map(x => x.CNPJ);
            Map(x => x.CEP);

            HasMany(x => x.Veiculos).Inverse().LazyLoad();

            Table("Pessoa");
        }
    }
}
