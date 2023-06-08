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
        [Required(ErrorMessage = "O campo descrição do serviço é obrigatório.")]
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
        [Required(ErrorMessage = "O campo veículo é obrigatório.")]
        [Display(Name = "Veículo")]
        public int IdVeiculo { get; set; }
        [Display(Name = "Cupom de Desconto")]
        public int? IdCupomDesconto { get; set; }
        [Required(ErrorMessage = "O campo cliente é obrigatório.")]
        [Display(Name = "Cliente")]
        public int IdCliente { get; set; }
        [Display(Name = "Kilometragem")]
        [Required(ErrorMessage = "Campo KM é obrigatório.")]
        public int? KM { get; set; }

        public ICollection<Foto> Fotos { get; set; }
        public ICollection<ProdServOSModel> Itens { get; set; }
        public ICollection<HistoricoPagamento> HistoricoPagamentos { get; set; }
    }
}