using Parcker.Domain;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Parcker.Models
{
    public class UsuarioModel : Base
    {
        [Required(ErrorMessage = "O campo {0} é obrigatório!")]
        [Display(Name = "Nome de Usuário")]
        public string Nome { get; set; }
        public string Senha { get; set; }
        public bool Ativo { get; set; }
        [Display(Name = "Administrador")]
        public bool Admin { get; set; }
        public int? IdPessoa { get; set; }
        public PessoaModel Pessoa { get; set; }
        public ConfiguracaoEmailModel ConfiguracaoEmail { get; set; }
    }
}