using Parcker.Domain;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Parcker.Models
{
    public class PrinterPRModel
    {
        public Usuario Usuario { get; set; }
        public List<PagarReceber> PagarReceber { get; set; }
        [Required(ErrorMessage = "O campo data início é obrigatório.")]
        public DateTime DataInicio { get; set; }
        [Required(ErrorMessage = "O campo data fim é obrgatório.")]
        public DateTime DataFinal { get; set; }
        public bool GerarPDF { get; set; }
        [Required(ErrorMessage = "Selecione no mínimo 1 situação.")]
        public int[] IdSituacaoConta { get; set; }
        [Required(ErrorMessage = "Selecione no mínimo 1 forma de pagamento.")]
        public int[] IdFormaPagamento { get; set; }
    }
}