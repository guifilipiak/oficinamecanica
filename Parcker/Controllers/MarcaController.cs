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
    public class MarcaController : BaseController
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
                ViewBag.Categorias = entity.All<Categoria>().Select(x => new SelectListItem() { Text = x.Descricao, Value = x.Id.ToString() });

                if (!id.HasValue)
                    return View(new MarcaModel()
                    {
                        DataCriacao = DateTime.Now
                    });
                else
                {
                    var modelMap = Mapper.Map<MarcaModel>(entity.GetById<Marca>(id.Value));
                    return View(modelMap != null ? modelMap : new MarcaModel()
                    {
                        Id = 0,
                        DataCriacao = DateTime.Now
                    });
                }
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(MarcaModel model)
        {
            if (!ModelState.IsValid)
                return Json(new { IsValid = false, Message = MensagemValidacao }, JsonRequestBehavior.AllowGet);
            else
                using (var entity = new Entity())
                {
                    try
                    {
                        var modelMap = Mapper.Map<Marca>(model);

                        if (model.Id == 0)
                            entity.Add(modelMap);
                        else
                            entity.Update(modelMap);

                        return Json(new { IsValid = true, Message = MensagemSalvo, Data = Mapper.Map<MarcaModel>(modelMap) }, JsonRequestBehavior.AllowGet);
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
                    var model = entity.GetById<Marca>(id);
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
        public JsonResult RetornaListaMarcas(DataTableAjaxModel model)
        {
            using (var entity = new Entity())
            {
                int recordsTotal;
                int recordsFiltered;

                //select
                var list = entity.All<Marca>();

                recordsTotal = list.Count();

                //where
                if (model.search.value != null)
                {
                    list = list.Where(x => x.Descricao.Contains(model.search.value));
                }

                recordsFiltered = list.Count();

                //order
                model.order.ForEach(x =>
                {
                    list = list.OrderBy($"{model.columns[x.column].data} {x.dir}");
                });

                //select skip take
                var data = list.Select(x => new
                {
                    x.Id,
                    x.Descricao,
                    Categoria = x.Categoria.Descricao
                })
                .Skip(model.start)
                .Take(model.length)
                .ToList();

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