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
    public class FornecedorController : BaseController
    {
        // GET: Fornecedor
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult Edit(int? id)
        {
            ViewBag.ListUF = ListaUF();

            using (var entity = new Entity())
            {
                if (!id.HasValue)
                    return View(new FornecedorModel()
                    {
                        DataCriacao = DateTime.Now,
                        Pessoa = new PessoaModel()
                        {
                            Tipo = 2,
                            DataCriacao = DateTime.Now
                        }
                    });
                else
                {
                    var modelMap = Mapper.Map<FornecedorModel>(entity.GetById<Fornecedor>(id.Value));
                    return View(modelMap != null ? modelMap : new FornecedorModel()
                    {
                        Id = 0,
                        DataCriacao = DateTime.Now,
                        Pessoa = new PessoaModel()
                        {
                            Tipo = 2,
                            DataCriacao = DateTime.Now
                        }
                    });
                }
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(FornecedorModel model)
        {
            if (ModelState.IsValid)
            {
                //inicia gravacao
                using (var entity = new Entity())
                {
                    try
                    {
                        entity.BeginTransaction();

                        var fornecedor = Mapper.Map<Fornecedor>(model);

                        if (model.Id == 0)
                        {
                            #region Validacao Manual
                            bool error = false;
                            //valida campos
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

                            if (error)
                            {
                                return Json(new { IsValid = false, Message = MensagemValidacao }, JsonRequestBehavior.AllowGet);
                            }
                            #endregion

                            if (entity.All<Fornecedor>().Where(x => x.Pessoa.CNPJ == model.Pessoa.CNPJ && !string.IsNullOrEmpty(model.Pessoa.CNPJ)).Any())
                            {
                                //ja existe um fornecedor cadastrado com este documento
                                return Json(new { IsValid = false, Message = "Já existe um fornecedor cadastro com este número de documento, verifique e tente novamente!" }, JsonRequestBehavior.AllowGet);
                            }

                            if (fornecedor.Pessoa.Id != 0)
                                //atualiza pessoa
                                entity.Update(fornecedor.Pessoa);
                            else
                                //adiciona pessoa
                                entity.Add(fornecedor.Pessoa);

                            //adiciona fornecedor
                            fornecedor.IdPessoa = fornecedor.Pessoa.Id;
                            entity.Add(fornecedor);
                        }
                        else
                        {
                            //atualiza pessoa
                            entity.Update(fornecedor.Pessoa);

                            //atualiza fornecedor
                            fornecedor.Pessoa = null;
                            entity.Update(fornecedor);
                        }

                        entity.Commit();

                        return Json(new { IsValid = true, Message = MensagemSalvo, Data = Mapper.Map<FornecedorModel>(fornecedor) }, JsonRequestBehavior.AllowGet);
                    }
                    catch (Exception ex)
                    {
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
                    var model = entity.GetById<Fornecedor>(id);
                    if (model != null)
                        entity.Delete(model);

                    return Json(new { IsValid = true, Message = MensagemRemovido }, JsonRequestBehavior.AllowGet);
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError("", ex);
                    return Json(new { IsValid = false, Message = MensagemErro }, JsonRequestBehavior.AllowGet);
                }
            }
        }

        #region JSON
        [HttpPost]
        public JsonResult RetornarDadosPessoaPorCnpj(string cnpj)
        {
            using (var entity = new Entity())
            {
                var search = ClearText(cnpj);
                var model = entity.All<Pessoa>().Where(x => x.CNPJ == search).FirstOrDefault();

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
        public JsonResult RetornaListaFornecedores(DataTableAjaxModel model)
        {
            using (var entity = new Entity())
            {
                int recordsTotal;
                int recordsFiltered;

                //select
                var list = entity.All<Fornecedor>();

                recordsTotal = list.Count();

                //where
                if (model.search.value != null)
                {
                    list = list.Where(x => x.Pessoa.Nome.Contains(model.search.value));
                }

                recordsFiltered = list.Count();

                //select
                var data = list.Select(x => new
                {
                    x.Id,
                    x.Pessoa.RazaoSocial,
                    x.Pessoa.CNPJ,
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