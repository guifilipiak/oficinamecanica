using Parcker.Domain;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Parcker.Models
{
    public class VeiculoModel : Base
    {
        [Required(ErrorMessage = "Campo Modelo é obrigatório.")]
        public string Modelo { get; set; }
        public string Chassi { get; set; }
        private string _Placa;
        [Required(ErrorMessage = "Campo Placa é obrigatório.")]
        [Remote("ValidaExistenciaPlaca", "Veiculo", AdditionalFields = "Id", ErrorMessage = "Já existe um veículo cadastrado com esta placa.")]
        public virtual string Placa
        {
            get
            {
                return _Placa;
            }
            set
            {
                _Placa = ClearText(value);
            }
        }
        public string Renavam { get; set; }
        public string Cor { get; set; }
        [Required(ErrorMessage = "Campo Ano é obrigatório.")]
        public int? Ano { get; set; }
        [Display(Name = "Ano Fabricação")]
        public int? AnoFabricacao { get; set; }
        [Display(Name = "Kilometragem")]
        [Required(ErrorMessage = "Campo KM é obrigatório.")]
        public int? KM { get; set; }
        [Display(Name = "Proprietário do Veículo")]
        [Required(ErrorMessage = "Campo Proprietário é obrigatório.")]
        public int IdPessoa { get; set; }
        [Display(Name = "Marca")]
        [Required(ErrorMessage = "Campo Marca é obrigatório.")]
        public int IdMarca { get; set; }

        public ICollection<OrdemServicoModel> OrdensServico { get; set; }
    }
}