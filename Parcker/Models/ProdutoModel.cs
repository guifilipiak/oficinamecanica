using Parcker.Domain;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Parcker.Models
{
    public class ProdutoModel : Base
    {
        [Required(ErrorMessage = "O campo {0} é obrigatório.")]
        [StringLength(50, ErrorMessage = "Tamanho máximo 50 caracteres.")]
        public string Nome { get; set; }
        [Display(Name = "Descrição sobre o Produto")]
        [StringLength(100, ErrorMessage = "Tamanho máximo 100 caracteres.")]
        public string Descricao { get; set; }
        [Display(Name = "Valor Unitário")]
        [Required(ErrorMessage = "O campo {0} é obrigatório.")]
        public decimal ValorUnitario { get; set; }
        [Required(ErrorMessage = "O campo {0} é obrigatório.")]
        public int Estoque { get; set; }
        [Display(Name = "Ativo")]
        public bool Ativo { get; set; }
        [Display(Name = "Alertar estoque abaixo do mínimo")]
        public bool AlertaEstoqueMinimo { get; set; }
        [Display(Name = "Estoque Mínimo")]
        public int? EstoqueMinimo { get; set; }
        [Display(Name = "Marca")]
        public int? IdMarca { get; set; }
        [Display(Name = "Fornecedor")]
        public int? IdFornecedor { get; set; }
        [Display(Name = "Código Externo")]
        [StringLength(60, ErrorMessage = "Tamanho máximo 60 caracteres.")]
        [Remote("ValidaExistenciaCodigoExterno","Produto", AdditionalFields = "Id", ErrorMessage = "Já existe um produto cadastrado com este código.")]
        public string CodigoExterno { get; set; }
        [Display(Name = "Código de Barras")]
        [StringLength(200, ErrorMessage = "Tamanho máximo 200 caracteres.")]
        [Remote("ValidaExistenciaCodigoBarras", "Produto", AdditionalFields = "Id", ErrorMessage = "Já existe um produto cadastrado com este código de barras.")]
        public string CodigoBarras { get; set; }
    }
}