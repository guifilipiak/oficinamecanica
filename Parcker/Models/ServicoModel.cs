using Parcker.Domain;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Parcker.Models
{
    public class ServicoModel : Base
    {
        [Display(Name = "Descrição")]
        [Required(ErrorMessage = "O campo {0} é obrigatório.")]
        public string Descricao { get; set; }
        [Display(Name = "Tempo Médio de Execução (Horas)")]
        public int HorasTrabalho { get; set; }
        [Display(Name = "Valor Unitário")]
        [Required(ErrorMessage = "O campo {0} é obrigatório.")]
        public decimal ValorUnitario { get; set; }
    }
}