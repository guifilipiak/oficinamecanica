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
    public class CategoriaController : BaseController
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
                if (!id.HasValue)
                    return View(new CategoriaModel()
                    {
                        DataCriacao = DateTime.Now
                    });
                else
                {
                    var modelMap = Mapper.Map<CategoriaModel>(entity.GetById<Categoria>(id.Value));
                    return View(modelMap != null ? modelMap : new CategoriaModel()
                    {
                        Id = 0,
                        DataCriacao = DateTime.Now
                    });
                }
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(CategoriaModel model)
        {
            if (!ModelState.IsValid)
                return Json(new { IsValid = false, Message = MensagemValidacao }, JsonRequestBehavior.AllowGet);
            else
                using (var entity = new Entity())
                {
                    try
                    {
                        var modelMap = Mapper.Map<Categoria>(model);

                        if (model.Id == 0)
                        {
                            entity.Add(modelMap);
                        }
                        else
                        {
                            var existente = entity.GetById<Categoria>(model.Id);
                            if (existente == null)
                            {
                                entity.Add(modelMap);
                            }
                            else
                            {
                                var original = existente.DataCriacao;
                                Mapper.Map(model, existente);
                                existente.DataCriacao = original;
                                modelMap = existente;
                                entity.SaveOrUpdate(existente);
                            }
                        }

                        return Json(new { IsValid = true, Message = MensagemSalvo, Data = Mapper.Map<CategoriaModel>(modelMap) }, JsonRequestBehavior.AllowGet);
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
                    var model = entity.GetById<Categoria>(id);
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
        public JsonResult RetornaListaCategorias(DataTableAjaxModel model)
        {
            using (var entity = new Entity())
            {
                int recordsTotal;
                int recordsFiltered;

                //select
                var list = entity.All<Categoria>();

                recordsTotal = list.Count();

                //where
                if (!string.IsNullOrWhiteSpace(model.search.value))
                {
                    list = list.Where(x => x.Descricao.Contains(model.search.value));
                }

                recordsFiltered = list.Count();

                //select
                var data = list.Select(x => new
                {
                    x.Id,
                    x.Descricao
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