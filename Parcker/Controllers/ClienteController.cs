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
                        Ativo = true,
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

                            // Verifica se já existe uma pessoa com este CPF/CNPJ
                            var documento = model.Pessoa.Tipo == 1 ? model.Pessoa.CPF : model.Pessoa.CNPJ;
                            var pessoaExistente = entity.All<Pessoa>().Where(x => 
                                (model.Pessoa.Tipo == 1 && x.CPF == documento && !string.IsNullOrEmpty(documento)) ||
                                (model.Pessoa.Tipo == 2 && x.CNPJ == documento && !string.IsNullOrEmpty(documento))
                            ).FirstOrDefault();
                            
                            if (pessoaExistente != null)
                            {
                                // Atualiza os dados da pessoa existente
                                if (model.Pessoa.Tipo == 1)
                                {
                                    pessoaExistente.Nome = cliente.Pessoa.Nome;
                                }
                                else
                                {
                                    pessoaExistente.RazaoSocial = cliente.Pessoa.RazaoSocial;
                                    pessoaExistente.Fantasia = cliente.Pessoa.Fantasia;
                                }
                                pessoaExistente.Telefone1 = cliente.Pessoa.Telefone1;
                                pessoaExistente.Telefone2 = cliente.Pessoa.Telefone2;
                                pessoaExistente.Email = cliente.Pessoa.Email;
                                pessoaExistente.Endereco = cliente.Pessoa.Endereco;
                                pessoaExistente.Numero = cliente.Pessoa.Numero;
                                pessoaExistente.Bairro = cliente.Pessoa.Bairro;
                                pessoaExistente.Cidade = cliente.Pessoa.Cidade;
                                pessoaExistente.UF = cliente.Pessoa.UF;
                                pessoaExistente.CEP = cliente.Pessoa.CEP;
                                entity.Update(pessoaExistente);
                                cliente.IdPessoa = pessoaExistente.Id;
                            }
                            else
                            {
                                // Cria nova pessoa
                                if (cliente.Pessoa.Id != 0)
                                    entity.Update(cliente.Pessoa);
                                else
                                    entity.Add(cliente.Pessoa);
                                cliente.IdPessoa = cliente.Pessoa.Id;
                            }

                            // Verifica se já existe cliente para esta pessoa
                            if (entity.All<Cliente>().Where(x => x.IdPessoa == cliente.IdPessoa).Any())
                            {
                                entity.Rollback();
                                return Json(new { IsValid = false, Message = "Já existe um cliente cadastrado para esta pessoa." }, JsonRequestBehavior.AllowGet);
                            }

                            entity.Add(cliente);
                        }
                        else
                        {
                            //atualiza pessoa
                            entity.Update(cliente.Pessoa);

                            //atualiza cliente
                            cliente.Pessoa = null;
                            var merge = entity.Merge(cliente);
                            entity.Update(merge);
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

                        if (model.Pessoa.Veiculos != null && model.Pessoa.Veiculos.Any())
                        {
                            return Json(new { IsValid = false, Message = "Este cliente possui veículos cadastrados e não pode ser removido." }, JsonRequestBehavior.AllowGet);
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
                if (!string.IsNullOrWhiteSpace(model.search.value))
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
                    Endereco = $"Cep: {x.Pessoa.CEP} - {x.Pessoa.Endereco}, N:{x.Pessoa.Numero}, {x.Pessoa.Bairro} - {x.Pessoa.Cidade}/{x.Pessoa.UF}",
                    Ativo = x.Ativo ? "Sim" : "Não"
                }).ToList();

                //order (preserva ordenação por múltiplas colunas)
                if (model.order != null && model.order.Any())
                {
                    var ordenacao = string.Join(", ", model.order
                        .Where(x => !string.IsNullOrEmpty(model.columns[x.column].data))
                        .Select(x => $"{model.columns[x.column].data} {x.dir}"));
                    if (!string.IsNullOrEmpty(ordenacao))
                        data = data.OrderBy(ordenacao).ToList();
                }

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