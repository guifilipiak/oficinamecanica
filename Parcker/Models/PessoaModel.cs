using Parcker.Domain;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Parcker.Models
{
    public class PessoaModel : Base
    {
        [Display(Name = "Tipo de Pessoa")]
        public int Tipo { get; set; }
        public string Nome { get; set; }
        [Display(Name = "Razão Social")]
        public string RazaoSocial { get; set; }
        public string Fantasia { get; set; }
        private string _CPF { get; set; }
        public string CPF
        {
            get
            {
                return _CPF;
            }
            set
            {
                _CPF = ClearText(value);
            }
        }
        private string _CNPJ { get; set; }
        public string CNPJ
        {
            get
            {
                return _CNPJ;
            }
            set
            {
                _CNPJ = ClearText(value);
            }
        }
        public string RG { get; set; }
        private string _CEP { get; set; }
        public string CEP
        {
            get
            {
                return _CEP;
            }
            set
            {
                _CEP = ClearText(value);
            }
        }
        [Display(Name = "País")]
        public string Pais { get; set; }
        [Display(Name = "Estado")]
        public string UF { get; set; }
        public string Cidade { get; set; }
        public string Bairro { get; set; }
        [Display(Name = "Endereço")]
        public string Endereco { get; set; }
        private string _Telefone1 { get; set; }
        [Display(Name = "Tel.Fixo")]
        public string Telefone1
        {
            get
            {
                return _Telefone1;
            }
            set
            {
                _Telefone1 = ClearText(value);
            }
        }
        private string _Telefone2 { get; set; }
        [Display(Name = "Tel.Celular")]
        public string Telefone2
        {
            get
            {
                return _Telefone2;
            }
            set
            {
                _Telefone2 = ClearText(value);
            }
        }
        [Display(Name = "E-Mail")]
        public string Email { get; set; }
        [Display(Name = "Número")]
        public int? Numero { get; set; }
        public string Complemento { get; set; }

        public ICollection<VeiculoModel> Veiculos { get; set; }
    }
}