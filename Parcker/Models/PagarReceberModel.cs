using Parcker.Domain;
using Parcker.Models.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Parcker.Models
{
    public class PagarReceberModel : Base
    {
        [Display(Name = "Data de Pagamento")]
        [Required(ErrorMessage = "O campo data de pagamento é obrigatória.")]
        public DateTime? DataPagamento { get; set; }
        [Display(Name = "Data de Vencimento")]
        public DateTime? DataVencimento { get; set; }
        [Display(Name = "Descrição")]
        [Required(ErrorMessage = "O campo descrição é obrigatório.")]
        public string Descricao { get; set; }
        [Display(Name = "Valor")]
        [Required(ErrorMessage = "O campo valor é obrigatório.")]
        [DisplayFormat(ApplyFormatInEditMode = true, NullDisplayText = "0,00", DataFormatString = "{0:n2}")]
        [Remote("ValidarValor", "PagarReceber", ErrorMessage = "O valor não pode ser zero.")]
        public decimal Valor { get; set; }
        public int Parcela { get; set; }
        [Display(Name = "Parcelas")]
        [Remote("ValidarValorParcela", "PagarReceber", AdditionalFields = "IdFormaPagamento, Valor", ErrorMessage = "A parcela mínima é de R$ 1,00.")]
        [Range(1, 24, ErrorMessage = "É permitido apenas parcelas de 1x até 24x.")]
        public int TotalParcela { get; set; }
        public int GrupoParcela { get; set; }
        [Display(Name = "Definir como modelo")]
        public bool Recorrente { get; set; }
        [Display(Name = "Forma de Pagamento")]
        [Required(ErrorMessage = "O campo forma de pagamento é obrigatório.")]
        public FormasPagamentoEnum IdFormaPagamento { get; set; }
        [Display(Name = "Tipo de Conta")]
        [Required(ErrorMessage = "O campo tipo conta é obrigatório.")]
        public TipoContaEnum IdTipoConta { get; set; }
        [Display(Name = "Situação")]
        public SituacaoContaEnum IdSituacaoConta { get; set; }
        [Display(Name = "Credor/Pagador")]
        public int? IdPessoa { get; set; }
        [Display(Name = "Classificação")]
        [Required(ErrorMessage = "O campo classificação é obrigatório.")]
        public ClassificacaoEnum IdClassificacao { get; set; }
    }
}