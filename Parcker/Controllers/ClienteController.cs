using AutoMapper;
using Parcker.Domain;
using Parcker.Models;
using Parcker.Repository.Business;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Linq.Dynamic;

namespace Parcker.Controllers
{
    [Authorize]
    public class ClienteController : BaseController
    {
        // GET: Cliente
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult Edit(int? id)
        {
            ViewBag.ListTipoPessoa = ListaTipoPessoa();
            ViewBag.ListUF = ListaUF();

            using (var entity = new Entity())
            {
                if (!id.HasValue)
                    return View(new ClienteModel()
                    {
                        Pontos = 0,
                        DataCriacao = DateTime.Now,
                        Pessoa = new PessoaModel()
                        {
                            Tipo = 1,
                            DataCriacao = DateTime.Now
                        }
                    });
                else
                {
                    var modelMap = Mapper.Map<ClienteModel>(entity.GetById<Cliente>(id.Value));
                    return View(modelMap != null ? modelMap : new ClienteModel()
                    {
                        Id = 0,
                        DataCriacao = DateTime.Now,
                        Pessoa = new PessoaModel()
                        {
                            Tipo = 1,
                            DataCriacao = DateTime.Now
                        }
                    });
                }
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(ClienteModel model)
        {
            if (ModelState.IsValid)
            {
                //inicia gravacao
                using (var entity = new Entity())
                {
                    try
                    {
                        entity.BeginTransaction();

                        var cliente = Mapper.Map<Cliente>(model);

                        if (model.Id == 0)
                        {
                            #region Validacao Manual
                            bool error = false;
                            //valida campos
                            if (model.Pessoa.Tipo == 1)
                            {
                                if (string.IsNullOrEmpty(model.Pessoa.CPF))
                                {
                                    ModelState.AddModelError("Pessoa_CPF", "Campo CPF é obrigatório.");
                                    error = true;
                                }

                                if (string.IsNullOrEmpty(model.Pessoa.Nome))
                                {
                                    ModelState.AddModelError("Pessoa_Nome", "Campo Nome é obrigatório.");
                                    error = true;
                                }
                            }
                            else
                            {
                                if (string.IsNullOrEmpty(model.Pessoa.CNPJ))
                                {
                                    ModelState.AddModelError("Pessoa_CNPJ", "Campo CNPJ é obrigatório.");
                                    error = true;
                                }

                                if (string.IsNullOrEmpty(model.Pessoa.RazaoSocial))
                                {
                                    ModelState.AddModelError("Pessoa_RazaoSocial", "Campo RazaoSocial é obrigatório.");
                                    error = true;
                                }
                            }

                            if (error)
                            {
                                return Json(new { IsValid = false, Message = MensagemValidacao }, JsonRequestBehavior.AllowGet);
                            }
                            #endregion

                            if (entity.All<Cliente>().Where(x => (x.Pessoa.CPF == model.Pessoa.CPF && !string.IsNullOrEmpty(model.Pessoa.CPF)) || (x.Pessoa.CNPJ == model.Pessoa.CNPJ && !string.IsNullOrEmpty(model.Pessoa.CNPJ))).Any())
                            {
                                //ja existe um cliente cadastrado com este documento
                                return Json(new { IsValid = false, Message = "Já existe um cliente cadastro com este número de documento, verifique e tente novamente!" }, JsonRequestBehavior.AllowGet);
                            }

                            if (cliente.Pessoa.Id != 0)
                                //atualiza pessoa
                                entity.Update(cliente.Pessoa);
                            else
                                //adiciona pessoa
                                entity.Add(cliente.Pessoa);

                            //adiciona cliente
                            cliente.IdPessoa = cliente.Pessoa.Id;
                            entity.Add(cliente);
                        }
                        else
                        {
                            //atualiza pessoa
                            entity.Update(cliente.Pessoa);

                            //atualiza cliente
                            cliente.Pessoa = null;
                            entity.Update(cliente);
                        }

                        entity.Commit();

                        return Json(new { IsValid = true, Message = MensagemSalvo, Data = Mapper.Map<ClienteModel>(cliente) }, JsonRequestBehavior.AllowGet);
                    }
                    catch (Exception ex)
                    {
                        entity.Rollback();
                        ModelState.AddModelError("", ex);
                        return Json(new { IsValid = false, Message = MensagemErro }, JsonRequestBehavior.AllowGet);
                    }
                }
            }
            else
                return Json(new { IsValid = false, Message = MensagemValidacao }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult Delete(int id)
        {
            using (var entity = new Entity())
            {
                try
                {
                    entity.BeginTransaction();

                    var model = entity.GetById<Cliente>(id);
                    if (model != null)
                    {
                        if (model.OrdensServico != null && model.OrdensServico.Any())
                        {
                            return Json(new { IsValid = false, Message = "Este cliente possui ordens de serviço cadastradas e não pode ser removido." }, JsonRequestBehavior.AllowGet);
                        }

                        entity.Delete(model);
                    }

                    entity.Commit();

                    return Json(new { IsValid = true, Message = MensagemRemovido }, JsonRequestBehavior.AllowGet);
                }
                catch (Exception ex)
                {
                    entity.Rollback();
                    ModelState.AddModelError("", ex);
                    return Json(new { IsValid = false, Message = MensagemErro }, JsonRequestBehavior.AllowGet);
                }
            }
        }

        #region JSON
        [HttpPost]
        public JsonResult RetornarDadosPessoaPorCpfCnpj(string cpfCnpj)
        {
            using (var entity = new Entity())
            {
                var search = ClearText(cpfCnpj);
                var model = entity.All<Pessoa>().Where(x => x.CPF == search || x.CNPJ == search).FirstOrDefault();

                if (model != null)
                {
                    model.Veiculos = null;
                    model.PagarReceber = null;
                    return Json(new { IsValid = true, Data = model }, JsonRequestBehavior.AllowGet);
                }
                else
                    return Json(new { IsValid = false }, JsonRequestBehavior.AllowGet);
            }
        }

        [HttpPost]
        public JsonResult RetornaListaClientes(DataTableAjaxModel model)
        {
            using (var entity = new Entity())
            {
                int recordsTotal;
                int recordsFiltered;

                //select
                var list = entity.All<Cliente>();

                recordsTotal = list.Count();

                //where
                if (model.search.value != null)
                {
                    list = list.Where(x =>
                    x.Apelido.ToLower().Contains(model.search.value.ToLower()) ||
                    x.Pessoa.RazaoSocial.ToLower().Contains(model.search.value.ToLower()) ||
                    x.Pessoa.Nome.ToLower().Contains(model.search.value.ToLower()) ||
                    x.Pessoa.CPF == model.search.value ||
                    x.Pessoa.CNPJ == model.search.value);
                }

                recordsFiltered = list.Count();

                //select
                var data = list.Select(x => new
                {
                    x.Id,
                    x.Apelido,
                    x.Pontos,
                    NomeRazao = x.Pessoa.Tipo == 1 ? x.Pessoa.Nome : x.Pessoa.RazaoSocial,
                    CPFCNPJ = x.Pessoa.Tipo == 1 ? x.Pessoa.CPF : x.Pessoa.CNPJ,
                    Telefone = x.Pessoa.Telefone1,
                    Endereco = $"Cep: {x.Pessoa.CEP} - {x.Pessoa.Endereco}, N:{x.Pessoa.Numero}, {x.Pessoa.Bairro} - {x.Pessoa.Cidade}/{x.Pessoa.UF}"
                }).ToList();

                //order
                model.order.ForEach(x =>
                {
                    data = data.OrderBy($"{model.columns[x.column].data} {x.dir}").ToList();
                });

                //skip take
                data = data.Skip(model.start).Take(model.length).ToList();

                //return
                return Json(new
                {
                    model.draw,
                    recordsTotal,
                    recordsFiltered,
                    data
                }, JsonRequestBehavior.AllowGet);
            }
        }
        #endregion
    }
}