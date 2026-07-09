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
    public class FuncionarioController : BaseController
    {
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
                {
                    return View(new FuncionarioModel() { Ativo = true, Pessoa = new PessoaModel() { Tipo = 1 } });
                }
                else
                {
                    var funcionario = entity.GetById<Funcionario>(id.Value);
                    var modelMap = Mapper.Map<FuncionarioModel>(funcionario);
                    return View(modelMap ?? new FuncionarioModel() { Ativo = true, Pessoa = new PessoaModel() });
                }
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(FuncionarioModel model)
        {
            if (!ModelState.IsValid)
                return Json(new { IsValid = false, Message = MensagemValidacao }, JsonRequestBehavior.AllowGet);

            using (var entity = new Entity())
            {
                try
                {
                    entity.BeginTransaction();

                    var funcionario = Mapper.Map<Funcionario>(model);

                    if (model.Id == 0)
                    {
                        // Validação manual
                        if (string.IsNullOrEmpty(model.Pessoa.CPF))
                        {
                            ModelState.AddModelError("Pessoa_CPF", "Campo CPF é obrigatório.");
                            entity.Rollback();
                            return Json(new { IsValid = false, Message = MensagemValidacao }, JsonRequestBehavior.AllowGet);
                        }

                        if (string.IsNullOrEmpty(model.Pessoa.Nome))
                        {
                            ModelState.AddModelError("Pessoa_Nome", "Campo Nome é obrigatório.");
                            entity.Rollback();
                            return Json(new { IsValid = false, Message = MensagemValidacao }, JsonRequestBehavior.AllowGet);
                        }

                        // Verifica se já existe uma pessoa com este CPF
                        var pessoaExistente = entity.All<Pessoa>().Where(x => x.CPF == funcionario.Pessoa.CPF && !string.IsNullOrEmpty(funcionario.Pessoa.CPF)).FirstOrDefault();
                        
                        if (pessoaExistente != null)
                        {
                            // Atualiza os dados da pessoa existente
                            pessoaExistente.Nome = funcionario.Pessoa.Nome;
                            pessoaExistente.Telefone1 = funcionario.Pessoa.Telefone1;
                            pessoaExistente.Email = funcionario.Pessoa.Email;
                            entity.Update(pessoaExistente);
                            funcionario.IdPessoa = pessoaExistente.Id;
                        }
                        else
                        {
                            // Cria nova pessoa
                            if (funcionario.Pessoa.Id != 0)
                                entity.Update(funcionario.Pessoa);
                            else
                                entity.Add(funcionario.Pessoa);
                            funcionario.IdPessoa = funcionario.Pessoa.Id;
                        }

                        // Verifica se já existe funcionário para esta pessoa
                        if (entity.All<Funcionario>().Where(x => x.IdPessoa == funcionario.IdPessoa).Any())
                        {
                            entity.Rollback();
                            return Json(new { IsValid = false, Message = "Já existe um funcionário cadastrado para esta pessoa." }, JsonRequestBehavior.AllowGet);
                        }

                        entity.Add(funcionario);
                    }
                    else
                    {
                        entity.Update(funcionario.Pessoa);
                        funcionario.Pessoa = null;
                        var merge = entity.Merge(funcionario);
                        entity.Update(merge);
                    }

                    entity.Commit();

                    return Json(new { IsValid = true, Message = MensagemSalvo, Data = Mapper.Map<FuncionarioModel>(funcionario) }, JsonRequestBehavior.AllowGet);
                }
                catch (Exception ex)
                {
                    entity.Rollback();
                    ModelState.AddModelError("", ex);
                    return Json(new { IsValid = false, Message = MensagemErro }, JsonRequestBehavior.AllowGet);
                }
            }
        }

        [HttpPost]
        public JsonResult Delete(int id)
        {
            using (var entity = new Entity())
            {
                try
                {
                    entity.BeginTransaction();

                    var model = entity.GetById<Funcionario>(id);
                    if (model != null)
                    {
                        if (model.OrdensServico != null && model.OrdensServico.Any())
                        {
                            return Json(new { IsValid = false, Message = "Este funcionário possui ordens de serviço cadastradas e não pode ser removido." }, JsonRequestBehavior.AllowGet);
                        }

                        if (model.Pessoa.Veiculos != null && model.Pessoa.Veiculos.Any())
                        {
                            return Json(new { IsValid = false, Message = "Este funcionário possui veículos cadastrados e não pode ser removido." }, JsonRequestBehavior.AllowGet);
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

        [HttpPost]
        public JsonResult RetornaListaFuncionarios(DataTableAjaxModel model)
        {
            using (var entity = new Entity())
            {
                int recordsTotal;
                int recordsFiltered;

                var list = entity.All<Funcionario>();
                recordsTotal = list.Count();

                if (model.search.value != null)
                {
                    list = list.Where(x => x.Pessoa.Nome.ToLower().Contains(model.search.value.ToLower()) ||
                                          x.Pessoa.CPF.Contains(model.search.value) ||
                                          x.Cargo.ToLower().Contains(model.search.value.ToLower()));
                }

                recordsFiltered = list.Count();

                var data = list.Select(x => new
                {
                    x.Id,
                    Nome = x.Pessoa.Nome,
                    CPF = x.Pessoa.CPF,
                    Telefone = x.Pessoa.Telefone1,
                    Email = x.Pessoa.Email,
                    x.Cargo,
                    Ativo = x.Ativo ? "Sim" : "Não"
                }).ToList();

                model.order.ForEach(x =>
                {
                    data = data.OrderBy($"{model.columns[x.column].data} {x.dir}").ToList();
                });

                data = data.Skip(model.start).Take(model.length).ToList();

                return Json(new
                {
                    model.draw,
                    recordsTotal,
                    recordsFiltered,
                    data
                }, JsonRequestBehavior.AllowGet);
            }
        }

        [HttpPost]
        public JsonResult RetornarDadosPessoaPorCpf(string cpfCnpj)
        {
            using (var entity = new Entity())
            {
                var search = ClearText(cpfCnpj);
                var model = entity.All<Pessoa>().Where(x => x.CPF == search).FirstOrDefault();

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
    }
}