using Parcker.Domain;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Parcker.Models
{
    public class ConfiguracaoEmailModel : Base
    {
        [Display(Name = "Servidor")]
        public string Servidor { get; set; }
        [Display(Name = "Porta")]
        public int? Porta { get; set; }
        [Display(Name = "Remetente")]
        public string Remetente { get; set; }
        [Display(Name = "Usuário")]
        public string Usuario { get; set; }
        [Display(Name = "Senha")]
        public string Senha { get; set; }
    }
}
