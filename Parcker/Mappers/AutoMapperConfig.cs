using AutoMapper;
using Parcker.Domain;
using Parcker.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Parcker.Mappers
{
    public static class AutoMapperConfig
    {
        public static void RegisterMappings()
        {
            Mapper.Initialize(x =>
            {
                x.CreateMap<Veiculo, VeiculoModel>();
                x.CreateMap<VeiculoModel, Veiculo>();

                x.CreateMap<Marca, MarcaModel>();
                x.CreateMap<MarcaModel, Marca>();

                x.CreateMap<Categoria, CategoriaModel>();
                x.CreateMap<CategoriaModel, Categoria>();

                x.CreateMap<Cliente, ClienteModel>();
                x.CreateMap<ClienteModel, Cliente>();

                x.CreateMap<Pessoa, PessoaModel>();
                x.CreateMap<PessoaModel, Pessoa>();

                x.CreateMap<Produto, ProdutoModel>();
                x.CreateMap<ProdutoModel, Produto>();

                x.CreateMap<Servico, ServicoModel>();
                x.CreateMap<ServicoModel, Servico>();

                x.CreateMap<Fornecedor, FornecedorModel>();
                x.CreateMap<FornecedorModel, Fornecedor>();

                x.CreateMap<Alerta, AlertaModel>();
                x.CreateMap<AlertaModel, Alerta>();

                x.CreateMap<OrdemServico, OrdemServicoModel>();
                x.CreateMap<OrdemServicoModel, OrdemServico>();

                x.CreateMap<ProdServOS, ProdServOSModel>();
                x.CreateMap<ProdServOSModel, ProdServOS>();

                x.CreateMap<CupomDesconto, CupomDescontoModel>();
                x.CreateMap<CupomDescontoModel, CupomDesconto>();

                x.CreateMap<Usuario, UsuarioModel>();
                x.CreateMap<UsuarioModel, Usuario>();

                x.CreateMap<PagarReceber, PagarReceberModel>()
                .ForMember(dest => dest.Recorrente, opt => opt.MapFrom(src => src.Recorrente == 1));
                x.CreateMap<PagarReceberModel, PagarReceber>()
                .ForMember(dest => dest.Recorrente, opt => opt.MapFrom(src => src.Recorrente ? 1 : 0)); ;
            });
        }
    }
}