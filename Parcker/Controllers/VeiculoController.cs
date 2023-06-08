using AutoMapper;
using Parcker.Domain;
using Parcker.Models;
using Parcker.Models.Enums;
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
    public class VeiculoController : BaseController
    {
        // GET: Veiculo
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult Edit(int? id)
        {
            using (var entity = new Entity())
            {
                ViewBag.Pessoas = new List<Pessoa>().Select(x => new SelectListItem()).ToList();
                ViewBag.Marcas = entity.All<Marca>().Select(x => new SelectListItem() { Text = x.Descricao, Value = x.Id.ToString() }).ToList();

                if (!id.HasValue)
                    return View(new VeiculoModel()
                    {
                        DataCriacao = DateTime.Now
                    });
                else
                {
                    var modelMap = Mapper.Map<VeiculoModel>(entity.GetById<Veiculo>(id.Value));
                    if (modelMap != null)
                    {
                        ViewBag.Pessoas = entity.All<Pessoa>()
                            .Where(x => x.Id == modelMap.IdPessoa)
                            .Select(x => new SelectListItem()
                            {
                                Text = $"{(x.Tipo == (int)TipoPessoaEnum.Fisica ? (x.CPF + " - " + x.Nome) : (x.CNPJ + " - " + x.RazaoSocial))}",
                                Value = x.Id.ToString()
                            }).ToList();
                    }

                    return View(modelMap != null ? modelMap : new VeiculoModel()
                    {
                        Id = 0,
                        DataCriacao = DateTime.Now
                    });
                }
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public JsonResult Edit(VeiculoModel model)
        {
            if (!ModelState.IsValid)
                return Json(new { IsValid = false, Message = MensagemValidacao }, JsonRequestBehavior.AllowGet);
            else
                using (var entity = new Entity())
                {
                    try
                    {
                        var modelMap = Mapper.Map<Veiculo>(model);

                        if (model.Id == 0)
                            entity.Add(modelMap);
                        else
                            entity.Update(modelMap);

                        return Json(new { IsValid = true, Message = MensagemSalvo, Data = Mapper.Map<VeiculoModel>(modelMap) }, JsonRequestBehavior.AllowGet);
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
                    var model = entity.GetById<Veiculo>(id);
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
        public JsonResult ValidaExistenciaPlaca(int Id, string Placa)
        {
            Placa = (Placa ?? "").Replace("-", "");

            if (Id != 0)
                return Json(true, JsonRequestBehavior.AllowGet);

            if (string.IsNullOrEmpty(Placa))
                return Json(true, JsonRequestBehavior.AllowGet);

            using (var entity = new Entity())
            {
                return Json(!entity.All<Veiculo>()
                    .Any(x => x.Placa.ToUpper() == Placa.ToUpper()), JsonRequestBehavior.AllowGet);
            }
        }

        public JsonResult ListarPessoas(string query)
        {
            using (var entity = new Entity())
            {
                var result = entity.All<Pessoa>().Where(x =>
                x.Nome.ToLower().Contains(query.ToLower()) ||
                x.RazaoSocial.ToLower().Contains(query.ToLower()) ||
                x.CPF.Contains(query) || x.CNPJ.Contains(query)).Select(x => new
                {
                    x.Id,
                    DescricaoCompleta = $"{(x.Tipo == (int)TipoPessoaEnum.Fisica ? (x.CPF + " - " + x.Nome) : (x.CNPJ + " - " + x.RazaoSocial))}"
                }).Take(10).ToList();

                return Json(result, JsonRequestBehavior.AllowGet);
            }
        }

        [HttpPost]
        public JsonResult RetornaListaVeiculos(DataTableAjaxModel model)
        {
            using (var entity = new Entity())
            {
                int recordsTotal;
                int recordsFiltered;

                //select
                var list = entity.All<Veiculo>();

                recordsTotal = list.Count();

                //where
                if (model.search.value != null)
                {
                    list = list.Where(x => x.Placa.Contains(model.search.value) ||
                        x.Marca.Descricao.Contains(model.search.value) ||
                        x.Modelo.Contains(model.search.value) ||
                        x.Pessoa.Nome.Contains(model.search.value));
                }

                recordsFiltered = list.Count();

                //select
                var data = list.Select(x => new
                {
                    x.Id,
                    x.Placa,
                    x.Ano,
                    x.AnoFabricacao,
                    x.Modelo,
                    Marca = x.Marca.Descricao,
                    x.KM,
                    Proprietario = $"{(x.Pessoa.Tipo == (int)TipoPessoaEnum.Fisica ? (x.Pessoa.CPF + " - " + x.Pessoa.Nome) : (x.Pessoa.CNPJ + " - " + x.Pessoa.RazaoSocial))}",
                    x.Renavam,
                    x.Cor,
                    x.Chassi,
                    x.DataCriacao
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