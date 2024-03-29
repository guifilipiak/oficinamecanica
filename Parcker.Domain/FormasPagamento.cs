﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Parcker.Domain
{
    public class FormasPagamento
    {
        public virtual int Id { get; set; }
        public virtual string Descricao { get; set; }
        public virtual bool Ativo { get; set; }

        public virtual ICollection<PagarReceber> PagarReceber { get; set; }
    }
}
