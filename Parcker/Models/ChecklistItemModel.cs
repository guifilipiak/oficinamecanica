using Parcker.Domain;
using System.ComponentModel.DataAnnotations;

namespace Parcker.Models
{
    public class ChecklistItemModel : Base
    {
        public int IdChecklistVeiculo { get; set; }
        
        [Display(Name = "Sistema")]
        public string Sistema { get; set; }
        
        [Display(Name = "Item")]
        public string Item { get; set; }
        
        [Display(Name = "Verificado")]
        public bool Verificado { get; set; }
        
        [Display(Name = "Observação")]
        [StringLength(500, ErrorMessage = "A observação não pode exceder 500 caracteres.")]
        public string Observacao { get; set; }
    }
}