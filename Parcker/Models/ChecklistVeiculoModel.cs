using Parcker.Domain;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Parcker.Models
{
    public class ChecklistVeiculoModel : Base
    {
        public int IdOrdemServico { get; set; }
        
        [Display(Name = "Observações Gerais")]
        [StringLength(1000, ErrorMessage = "As observações gerais não podem exceder 1000 caracteres.")]
        public string Observacoes { get; set; }

        public ICollection<ChecklistItemModel> Itens { get; set; }
    }
}