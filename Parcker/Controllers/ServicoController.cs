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
    public class ServicoController : BaseController
    {
        // GET: Servico
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult Edit(int? id)
        {
            using (var entity = new Entity())
            {
                if (!id.HasValue)
                    return View(new ServicoModel()
                    {
                        DataCriacao = DateTime.Now
                    });
                else
                {
                    var modelMap = Mapper.Map<ServicoModel>(entity.GetById<Servico>(id.Value));
                    return View(modelMap != null ? modelMap : new ServicoModel()
                    {
                        Id = 0,
                        DataCriacao = DateTime.Now
                    });
                }
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(ServicoModel model)
        {
            if (!ModelState.IsValid)
                return Json(new { IsValid = false, Message = MensagemValidacao }, JsonRequestBehavior.AllowGet);
            else
                using (var entity = new Entity())
                {
                    try
                    {
                        var modelMap = Mapper.Map<Servico>(model);

                        if (model.Id == 0)
                            entity.Add(modelMap);
                        else
                            entity.Update(modelMap);

                        return Json(new { IsValid = true, Message = MensagemSalvo, Data = Mapper.Map<ServicoModel>(modelMap) }, JsonRequestBehavior.AllowGet);
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
                    var model = entity.GetById<Produto>(id);
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
        public JsonResult RetornaListaServicos(DataTableAjaxModel model)
        {
            using (var entity = new Entity())
            {
                int recordsTotal;
                int recordsFiltered;

                //select
                var list = entity.All<Servico>();

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
                    x.Descricao,
                    x.HorasTrabalho,
                    ValorUnitario = x.ValorUnitario.ToString("n2")
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