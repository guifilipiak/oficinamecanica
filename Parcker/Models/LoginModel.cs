using Parcker.Domain;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Parcker.Models
{
    public class LoginModel
    {
        [Required(ErrorMessage = "Digite o seu email!")]
        public string Email { get; set; }
        [Required(ErrorMessage = "Digite a sua senha!")]
        public string Senha { get; set; }
        public bool LembrarMe { get; set; }
    }
}