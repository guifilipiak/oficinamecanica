using AutoMapper;
using Parcker.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Parcker.Models
{
    public class ProdServOSModel : Base
    {
        [IgnoreMap]
        public int Index { get; set; }
        public int Quantidade { get; set; }
        public int IdOrdemServico { get; set; }
        public int? IdProduto { get; set; }
        public int? IdServico { get; set; }
        public int Tipo { get; set; }
        public decimal ValorUnitario { get; set; }
        public decimal Desconto { get; set; }
        public decimal ValorTotal { get; set; }

        public OrdemServico OrdemServico { get; set; }
        public Produto Produto { get; set; }
        public Servico Servico { get; set; }
    }
}