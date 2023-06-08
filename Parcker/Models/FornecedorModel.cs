using Parcker.Domain;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Parcker.Models
{
    public class FornecedorModel : Base
    {
        public int? IdPessoa { get; set; }
        public PessoaModel Pessoa { get; set; }
    }
}