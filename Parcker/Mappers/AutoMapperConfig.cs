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
                x.CreateMap<OrdemServicoModel, OrdemServico>()
                    .ForMember(dest => dest.Itens, opt => opt.Ignore())
                    .ForMember(dest => dest.Fotos, opt => opt.Ignore())
                    .ForMember(dest => dest.HistoricoPagamentos, opt => opt.Ignore())
                    .ForMember(dest => dest.ChecklistVeiculo, opt => opt.Ignore());

                x.CreateMap<ProdServOS, ProdServOSModel>();
                x.CreateMap<ProdServOSModel, ProdServOS>()
                    .ForMember(dest => dest.OrdemServico, opt => opt.Ignore())
                    .ForMember(dest => dest.Produto, opt => opt.Ignore())
                    .ForMember(dest => dest.Servico, opt => opt.Ignore());

                x.CreateMap<CupomDesconto, CupomDescontoModel>();
                x.CreateMap<CupomDescontoModel, CupomDesconto>();

                x.CreateMap<Usuario, UsuarioModel>();
                x.CreateMap<UsuarioModel, Usuario>();

                x.CreateMap<PagarReceber, PagarReceberModel>()
                .ForMember(dest => dest.Recorrente, opt => opt.MapFrom(src => src.Recorrente == 1));
                x.CreateMap<PagarReceberModel, PagarReceber>()
                .ForMember(dest => dest.Recorrente, opt => opt.MapFrom(src => src.Recorrente ? 1 : 0));

                x.CreateMap<ChecklistVeiculo, ChecklistVeiculoModel>();
                x.CreateMap<ChecklistVeiculoModel, ChecklistVeiculo>();

                x.CreateMap<ChecklistItem, ChecklistItemModel>();
                x.CreateMap<ChecklistItemModel, ChecklistItem>();

                x.CreateMap<Funcionario, FuncionarioModel>()
                .ForMember(dest => dest.OrdensServico, opt => opt.MapFrom(src => src.OrdensServico));
                x.CreateMap<FuncionarioModel, Funcionario>()
                .ForMember(dest => dest.OrdensServico, opt => opt.Ignore());
            });
        }
    }
}