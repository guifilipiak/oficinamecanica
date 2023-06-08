using AutoMapper;
using Parcker.Domain;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace Parcker.Models
{
    public class CupomDescontoModel : Base
    {
        [Display(Name = "Código Único")]
        [Required(ErrorMessage = "O campo {0} é obrigatório!")]
        public string Codigo { get; set; }
        [Display(Name = "Descrição")]
        [Required(ErrorMessage = "O campo {0} é obrigatório!")]
        public string Descricao { get; set; }
        [Display(Name = "Desconto")]
        [Required(ErrorMessage = "O campo {0} é obrigatório!")]
        public decimal ValorDesconto { get; set; }
        [Display(Name = "Validade")]
        [Required(ErrorMessage = "O campo {0} é obrigatório!")]
        public DateTime? DataValidade { get; set; }
        public int Quantidade { get; set; }
        public bool Ativo { get; set; }
    }
}