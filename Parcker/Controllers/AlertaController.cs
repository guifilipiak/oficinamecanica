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
    public class AlertaController : BaseController
    {
        // GET: Alerta
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult Edit(int? id)
        {
            using (var entity = new Entity())
            {
                ViewBag.Veiculos = new List<Veiculo>().Select(x => new SelectListItem()).ToList();

                if (!id.HasValue)
                    return View(new AlertaModel()
                    {
                        DataCriacao = DateTime.Now
                    });
                else
                {
                    var modelMap = Mapper.Map<AlertaModel>(entity.GetById<Alerta>(id.Value));
                    if (modelMap != null)
                    {
                        var veiculo = entity.All<Veiculo>()
                                .Where(x => x.Id == modelMap.IdVeiculo);

                        //carrega combos com valores selecionados
                        ViewBag.Veiculos = veiculo
                            .Select(x => new SelectListItem()
                            {
                                Text = $"{x.Placa}/{x.Modelo} - {x.Pessoa.Nome ?? x.Pessoa.RazaoSocial}/{x.Pessoa.CPF ?? x.Pessoa.CNPJ}",
                                Value = x.Id.ToString()
                            }).ToList();
                    }

                    return View(modelMap != null ? modelMap : new AlertaModel()
                    {
                        Id = 0,
                        DataCriacao = DateTime.Now
                    });
                }
            }
        }

        public PartialViewResult ExpiredAlertMenu()
        {
            using (var entity = new Entity())
            {
                var alertas = entity.All<Alerta>().Where(x =>
                x.Ativo).Select(x => new AlertaModel
                {
                    Id = x.Id,
                    Dias = x.Dias,
                    Ativo = x.Ativo,
                    IdVeiculo = x.IdVeiculo,
                    DataCriacao = x.DataCriacao,
                    DataAlertaEnviado = x.DataAlertaEnviado,
                    Descricao = x.Descricao,
                    DescricaoAlertaMenu = $"{x.Veiculo.Placa} - {x.Veiculo.Modelo} / {x.Veiculo.Pessoa.Nome} - {x.Veiculo.Pessoa.Telefone1 ?? x.Veiculo.Pessoa.Telefone2}"
                }).ToList();

                var dateValueValidate = DateTime.Now;
                var alertasExpirados = alertas.Where(x => x.DataAlertaEnviado.HasValue ?
                x.DataAlertaEnviado.Value.AddDays(x.Dias.GetValueOrDefault()) <= dateValueValidate :
                x.DataCriacao.AddDays(x.Dias.GetValueOrDefault()) <= dateValueValidate).ToList();

                var produtosEstoqueMinimo = entity.All<Produto>().Where(x => x.AlertaEstoqueMinimo && x.Estoque <= x.EstoqueMinimo && x.EstoqueMinimo != null).ToList();
                ViewData["ProdutosEstoqueMinimo"] = produtosEstoqueMinimo;

                return PartialView(alertasExpirados);
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(AlertaModel model)
        {
            if (!ModelState.IsValid)
                return Json(new { IsValid = false, Message = MensagemValidacao }, JsonRequestBehavior.AllowGet);
            else
                using (var entity = new Entity())
                {
                    try
                    {
                        var modelMap = Mapper.Map<Alerta>(model);
                        entity.SaveOrUpdate(modelMap);

                        return Json(new { IsValid = true, Message = MensagemSalvo, Data = Mapper.Map<AlertaModel>(modelMap) }, JsonRequestBehavior.AllowGet);
                    }
                    catch (Exception ex)
                    {
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
                    var model = entity.GetById<Alerta>(id);
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
        public JsonResult Alertar(int id)
        {
            using (var entity = new Entity())
            {
                try
                {
                    entity.BeginTransaction();
                    var model = entity.GetById<Alerta>(id);
                    if (model != null)
                    {
                        model.DataAlertaEnviado = DateTime.Now;
                        //Envia E-Mail
                        //...
                    }
                    entity.Update(model);
                    entity.Commit();
                    return Json(new { IsValid = true, Message = "Proprietário alertado com sucesso!" }, JsonRequestBehavior.AllowGet);
                }
                catch (Exception)
                {
                    entity.Rollback();
                    return Json(new { IsValid = false, Message = "Ocorreu um erro ao alertar o proprietário!" }, JsonRequestBehavior.AllowGet);
                }
            }
        }

        public JsonResult Adiar(int id)
        {
            using (var entity = new Entity())
            {
                try
                {
                    entity.BeginTransaction();
                    var model = entity.GetById<Alerta>(id);
                    if (model != null)
                    {
                        model.DataAlertaEnviado = DateTime.Now;
                        entity.Update(model);
                    }
                    entity.Commit();
                    return Json(new { IsValid = true, Message = "Alerta adiado com sucesso!" }, JsonRequestBehavior.AllowGet);
                }
                catch (Exception)
                {
                    entity.Rollback();
                    return Json(new { IsValid = false, Message = "Ocorreu um erro ao adiar o alerta!" }, JsonRequestBehavior.AllowGet);
                }
            }
        }

        public JsonResult Desativar(int id)
        {
            using (var entity = new Entity())
            {
                try
                {
                    entity.BeginTransaction();
                    var model = entity.GetById<Alerta>(id);
                    if (model != null)
                    {
                        model.Ativo = false;
                        entity.Update(model);
                    }
                    entity.Commit();
                    return Json(new { IsValid = true, Message = "Alerta desativado com sucesso!" }, JsonRequestBehavior.AllowGet);
                }
                catch (Exception)
                {
                    entity.Rollback();
                    return Json(new { IsValid = false, Message = "Ocorreu um erro ao adiar o alerta!" }, JsonRequestBehavior.AllowGet);
                }
            }
        }

        [HttpPost]
        public JsonResult RetornaListaAlertas(DataTableAjaxModel model)
        {
            using (var entity = new Entity())
            {
                int recordsTotal;
                int recordsFiltered;

                //select
                var list = entity.All<Alerta>();

                recordsTotal = list.Count();

                //where
                if (model.search.value != null)
                {
                    list = list.Where(x => x.Descricao.Contains(model.search.value));
                }

                recordsFiltered = list.Count();

                //select
                var data = list.Select(x => new
                {
                    x.Id,
                    x.DataCriacao,
                    x.Descricao,
                    Placa = x.Veiculo.Placa,
                    Veiculo = x.Veiculo.Modelo,
                    Proprietario = x.Veiculo.Pessoa.Nome,
                    x.Dias,
                    DataPrevisaoAlerta = x.DataAlertaEnviado.HasValue ?
                        x.DataAlertaEnviado.Value.AddDays(x.Dias.GetValueOrDefault()) :
                        x.DataCriacao.AddDays(x.Dias.GetValueOrDefault()),
                    x.DataAlertaEnviado,
                    x.Ativo
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

        public JsonResult ListarVeiculos(string query)
        {
            using (var entity = new Entity())
            {
                var result = entity.All<Veiculo>().Where(x =>
                x.Placa.ToLower().Contains(query.ToLower()) ||
                x.Modelo.ToLower().Contains(query.ToLower()) ||
                x.Pessoa.Nome.ToLower().Contains(query.ToLower()) ||
                x.Pessoa.RazaoSocial.ToLower().Contains(query.ToLower())).Select(x => new
                {
                    x.Id,
                    DescricaoCompleta = $"{x.Placa}/{x.Modelo} - {x.Pessoa.Nome ?? x.Pessoa.RazaoSocial}/{x.Pessoa.CPF ?? x.Pessoa.CNPJ}",
                }).Take(10).ToList();

                return Json(result, JsonRequestBehavior.AllowGet);
            }
        }
        #endregion
    }
}