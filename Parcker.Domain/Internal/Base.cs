using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Parcker.Domain
{
    public class Base
    {
        public virtual int Id { get; set; }
        private DateTime _DataCriacao;
        public virtual DateTime DataCriacao
        {
            get
            {
                if (_DataCriacao == DateTime.MinValue)
                {
                    _DataCriacao = DateTime.Now;
                    return _DataCriacao;
                }
                else
                    return _DataCriacao;
            }
            set
            {
                _DataCriacao = value;
            }
        }

        protected static string ClearText(string inputText)
        {
            if (inputText != null)
                return new Regex("[^a-zA-Z0-9]").Replace(inputText, "");
            else
                return string.Empty;
        }
    }
}
