using Parcker.Domain;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Parcker.Models
{
    public class FuncionarioModel : Base
    {
        public string Cargo { get; set; }
        
        public bool Ativo { get; set; }
        
        public int IdPessoa { get; set; }
        
        [Required(ErrorMessage = "Os dados da pessoa são obrigatórios.")]
        public virtual PessoaModel Pessoa { get; set; }
        
        public virtual IList<OrdemServicoModel> OrdensServico { get; set; }
    }
}