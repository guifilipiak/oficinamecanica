using Parcker.Domain;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Parcker.Models
{
    public class OrdemServicoModel : Base
    {
        public DateTime? DataFinalizacao { get; set; }
        [Display(Name = "Descrição do Serviço")]
        public string DescricaoServico { get; set; }
        [Display(Name = "Observações")]
        public string Observacoes { get; set; }
        [DisplayFormat(ApplyFormatInEditMode = true, NullDisplayText = "0,00", DataFormatString = "{0:n2}")]
        public decimal? Entrada { get; set; }
        [DisplayFormat(ApplyFormatInEditMode = true, NullDisplayText = "0,00", DataFormatString = "{0:n2}")]
        public decimal? Desconto { get; set; }
        [DisplayFormat(ApplyFormatInEditMode = true, NullDisplayText = "0,00", DataFormatString = "{0:n2}")]
        public decimal SubTotal { get; set; }
        [DisplayFormat(ApplyFormatInEditMode = true, NullDisplayText = "0,00", DataFormatString = "{0:n2}")]
        public decimal Total { get; set; }
        [Display(Name = "Situação")]
        public int IdSituacaoServico { get; set; }
        [CustomValidation(typeof(OrdemServicoModel), "ValidateIdVeiculo")]
        [Display(Name = "Veículo")]
        public int? IdVeiculo { get; set; }
        [Display(Name = "Cupom de Desconto")]
        public int? IdCupomDesconto { get; set; }
        [CustomValidation(typeof(OrdemServicoModel), "ValidateIdCliente")]
        [Display(Name = "Cliente")]
        public int? IdCliente { get; set; }
        [Display(Name = "Kilometragem")]
        [CustomValidation(typeof(OrdemServicoModel), "ValidateKM")]
        public int? KM { get; set; }
        public int IdTipoAtendimento { get; set; }
        [CustomValidation(typeof(OrdemServicoModel), "ValidateDataValidadeOrcamento")]
        [DataType(DataType.Date)]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:dd/MM/yyyy}")]
        [Display(Name = "Validade do Orçamento")]
        public DateTime? DataValidadeOrcamento { get; set; }

        public ICollection<Foto> Fotos { get; set; }
        public ICollection<ProdServOSModel> Itens { get; set; }
        public ICollection<HistoricoPagamento> HistoricoPagamentos { get; set; }

        public static ValidationResult ValidateIdVeiculo(int? idVeiculo, ValidationContext context)
        {
            var model = (OrdemServicoModel)context.ObjectInstance;
            if (model.IdTipoAtendimento == 1 && (!idVeiculo.HasValue || idVeiculo.Value == 0))
            {
                return new ValidationResult("O campo veículo é obrigatório.");
            }
            return ValidationResult.Success;
        }

        public static ValidationResult ValidateIdCliente(int? idCliente, ValidationContext context)
        {
            var model = (OrdemServicoModel)context.ObjectInstance;
            if (model.IdTipoAtendimento == 1 && (!idCliente.HasValue || idCliente.Value == 0))
            {
                return new ValidationResult("O campo cliente é obrigatório.");
            }
            return ValidationResult.Success;
        }

        public static ValidationResult ValidateKM(int? km, ValidationContext context)
        {
            var model = (OrdemServicoModel)context.ObjectInstance;
            if (model.IdTipoAtendimento == 1 && !km.HasValue)
            {
                return new ValidationResult("Campo KM é obrigatório.");
            }
            return ValidationResult.Success;
        }

        public static ValidationResult ValidateDataValidadeOrcamento(DateTime? dataValidade, ValidationContext context)
        {
            var model = (OrdemServicoModel)context.ObjectInstance;
            if (model.IdTipoAtendimento == 2 && !dataValidade.HasValue)
            {
                return new ValidationResult("Campo Validade do Orçamento é obrigatório.");
            }
            return ValidationResult.Success;
        }
    }
}