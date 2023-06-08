using Parcker.Domain;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Parcker.Models
{
    public class ClienteModel : Base
    {
        public int? IdPessoa { get; set; }
        public string Apelido { get; set; }
        public int? Pontos { get; set; }
        public PessoaModel Pessoa { get; set; }
    }
}