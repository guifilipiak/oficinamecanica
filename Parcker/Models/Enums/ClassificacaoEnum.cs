using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Web;

namespace Parcker.Models.Enums
{
    public enum ClassificacaoEnum
    {
        [Description("Despesa com Fornecedor")]
        DespesasFornecedor = 1,
        [Description("Despesa com Infraestrutura")]
        DespesaInfraestrutura = 2,
        [Description("Despesa com Funcionário")]
        DespesaFuncionario = 3,
        [Description("Despesa com Luz")]
        DespesaLuz = 4,
        [Description("Despesa com Água")]
        DespesaAgua = 5,
        [Description("Despesa com Telefone e Internet")]
        DespesaTelefoneInternet = 6,
        [Description("Outras Despesas")]
        OutrasDespesas = 7,
        [Description("Receita Normal")]
        ReceitaNormal = 8,
        [Description("Outras Receitas")]
        OutrasReceitas = 9,
        [Description("Retirada")]
        Retirada = 10
    }
}