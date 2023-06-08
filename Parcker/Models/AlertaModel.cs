using AutoMapper;
using Parcker.Domain;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace Parcker.Models
{
    public class AlertaModel : Base
    {
        [Display(Name = "Descrição")]
        public string Descricao { get; set; }
        [Display(Name ="Último Alerta Enviado")]
        public DateTime? DataAlertaEnviado { get; set; }
        [Display(Name = "Intervalo para Próximo Alerta")]
        public int? Dias { get; set; }
        [Display(Name = "Veículo")]
        [Required(ErrorMessage = "O campo {0} é obrigatório!")]
        public int IdVeiculo { get; set; }
        public bool Ativo { get; set; }
        [NotMapped]
        [IgnoreMap]
        public string DescricaoAlertaMenu { get; set; }
    }
}