using Parcker.Domain;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Parcker.Models
{
    public class CategoriaModel : Base
    {
        [Required(ErrorMessage = "Campo Descrição é obrigatório.")]
        public string Descricao { get; set; }
    }
}