using Parcker.Domain;
using Parcker.Repository.Business;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web.Mvc;

namespace Parcker.Controllers
{
    public class BaseController : Controller
    {
        #region Usuario Logado
        private Usuario _usuarioLogado { get; set; }
        public Usuario UsuarioLogado
        {
            get
            {
                if (_usuarioLogado == null)
                {
                    using (var entity = new Entity())
                    {
                        return entity.All<Usuario>().Where(x => x.Nome == User.Identity.Name).FirstOrDefault();
                    }
                }
                else
                    return _usuarioLogado;
            }
        }
        #endregion

        #region Mensagens
        public string MensagemSalvo
        {
            get
            {
                return "Registro salvo com sucesso!";
            }
        }

        public string MensagemRemovido
        {
            get
            {
                return "Registro removido com sucesso!";
            }
        }

        public string MensagemValidacao
        {
            get
            {
                return string.Join(Environment.NewLine, ModelState.Where(x => x.Value.Errors.Count > 0).Select(x => x.Value.Errors.Select(z => z.ErrorMessage + "</br>").FirstOrDefault()).ToList());
            }
        }

        public string MensagemErro
        {
            get
            {
                return "Não foi possível continuar, tente novamente ou entre em contato com o administrador do site!";
            }
        }
        #endregion

        #region Combos

        public static IEnumerable<SelectListItem> ListaUF()
        {
            List<SelectListItem> list = new List<SelectListItem>();
            list.Add(new SelectListItem()
            {
                Text = "Acre",
                Value = "AC"
            });
            list.Add(new SelectListItem()
            {
                Text = "Alagoas",
                Value = "AL"
            });
            list.Add(new SelectListItem()
            {
                Text = "Amazonas",
                Value = "AM"
            });
            list.Add(new SelectListItem()
            {
                Text = "Amapá",
                Value = "AP"
            });
            list.Add(new SelectListItem()
            {
                Text = "Bahia",
                Value = "BA"
            });
            list.Add(new SelectListItem()
            {
                Text = "Bahia",
                Value = "BA"
            });
            list.Add(new SelectListItem()
            {
                Text = "Ceará",
                Value = "CE"
            });
            list.Add(new SelectListItem()
            {
                Text = "Distrito Federal",
                Value = "DF"
            });
            list.Add(new SelectListItem()
            {
                Text = "Espírito Santo",
                Value = "ES"
            });
            list.Add(new SelectListItem()
            {
                Text = "Goiás",
                Value = "GO"
            });
            list.Add(new SelectListItem()
            {
                Text = "Maranhão",
                Value = "MA"
            });
            list.Add(new SelectListItem()
            {
                Text = "Minas Gerais",
                Value = "MG"
            });
            list.Add(new SelectListItem()
            {
                Text = "Mato Grosso do Sul",
                Value = "MS"
            });
            list.Add(new SelectListItem()
            {
                Text = "Mato Grosso",
                Value = "MT"
            });
            list.Add(new SelectListItem()
            {
                Text = "Pará",
                Value = "PA"
            });
            list.Add(new SelectListItem()
            {
                Text = "Paraíba",
                Value = "PB"
            });
            list.Add(new SelectListItem()
            {
                Text = "Pernambuco",
                Value = "PE"
            });
            list.Add(new SelectListItem()
            {
                Text = "Piauí",
                Value = "PI"
            });
            list.Add(new SelectListItem()
            {
                Text = "Paraná",
                Value = "PR"
            });
            list.Add(new SelectListItem()
            {
                Text = "Rio de Janeiro",
                Value = "RJ"
            });
            list.Add(new SelectListItem()
            {
                Text = "Rio Grande do Norte",
                Value = "RN"
            });
            list.Add(new SelectListItem()
            {
                Text = "Rondônia",
                Value = "RO"
            });
            list.Add(new SelectListItem()
            {
                Text = "Roraima",
                Value = "RR"
            });
            list.Add(new SelectListItem()
            {
                Text = "Rio Grande do Sul",
                Value = "RS"
            });
            list.Add(new SelectListItem()
            {
                Text = "Santa Catarina",
                Value = "SC"
            });
            list.Add(new SelectListItem()
            {
                Text = "Sergipe",
                Value = "SE"
            });
            list.Add(new SelectListItem()
            {
                Text = "São Paulo",
                Value = "SP"
            });
            list.Add(new SelectListItem()
            {
                Text = "Tocantins",
                Value = "TO"
            });
            return list;
        }

        public static IEnumerable<SelectListItem> ListaTipoPessoa()
        {
            List<SelectListItem> list = new List<SelectListItem>();
            list.Add(new SelectListItem()
            {
                Text = "Pessoa Física",
                Value = "1"
            });
            list.Add(new SelectListItem()
            {
                Text = "Pessoa Jurídica",
                Value = "2"
            });
            return list;
        }

        #endregion

        #region Utils
        protected static string ClearText(string inputText)
        {
            if (inputText != null)
                return new Regex("[^a-zA-Z0-9]").Replace(inputText, "");
            else
                return string.Empty;
        }
        #endregion
    }
}