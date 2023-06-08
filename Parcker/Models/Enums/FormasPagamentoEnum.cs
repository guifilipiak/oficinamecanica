using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Web;

namespace Parcker.Models.Enums
{
    public enum FormasPagamentoEnum
    {
        Boleto = 1,
        Dinheiro = 2,
        [Description("Cartão de Crédito")]
        CartaoCredito = 3,
        [Description("Cartão de Débito")]
        CartaoDebito = 4,
        Cheque = 5,
        [Description("Transferência Bancária")]
        TransferenciaBancaria = 6,
        [Description("Depósito")]
        Deposito = 7
    }
}