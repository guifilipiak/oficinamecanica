﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Parcker.Domain
{
    public class Foto : Base
    {
        public virtual string Descricao { get; set; }
        public virtual string UrlFoto { get; set; }
        public virtual int IdOrdemServico { get; set; }

        public virtual OrdemServico OrdemServico { get; set; }
    }
}
