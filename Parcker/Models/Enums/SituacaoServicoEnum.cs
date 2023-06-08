using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Parcker.Models.Enums
{
    public enum SituacaoServicoEnum
    {
        Criado = 1,
        EmAndamento = 2,
        Cancelado = 3,
        AguardandoPagamento = 4,
        Finalizado = 5
    }
}