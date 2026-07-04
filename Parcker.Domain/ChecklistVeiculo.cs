using System;
using System.Collections.Generic;

namespace Parcker.Domain
{
    public class ChecklistVeiculo : Base
    {
        public virtual int IdOrdemServico { get; set; }
        public virtual string Observacoes { get; set; }

        public virtual OrdemServico OrdemServico { get; set; }
        public virtual ICollection<ChecklistItem> Itens { get; set; }
    }
}