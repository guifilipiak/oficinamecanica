using AutoMapper;
using Parcker.Domain;
using Parcker.Models;
using Parcker.Repository.Business;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Web;
using System.Web.Mvc;
using System.Linq.Dynamic;

namespace Parcker.Controllers
{
    public class CupomDescontoController : BaseController
    {
        // GET: CupomDesconto
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult Edit(int? id)
        {
            using (var entity = new Entity())
            {
                if (!id.HasValue)
                    return View(new CupomDescontoModel()
                    {
                        Codigo = GerarCodigoCupom()
                    });
                else
                {
                    var modelMap = Mapper.Map<CupomDescontoModel>(entity.GetById<CupomDesconto>(id.Value));
                    return View(modelMap != null ? modelMap : new CupomDescontoModel()
                    {
                        Codigo = GerarCodigoCupom()
                    });
                }
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(CupomDescontoModel model)
        {
            if (!ModelState.IsValid)
                return Json(new { IsValid = false, Message = MensagemValidacao }, JsonRequestBehavior.AllowGet);
            else
                using (var entity = new Entity())
                {
                    try
                    {
                        var modelMap = Mapper.Map<CupomDesconto>(model);

                        if (model.Id == 0)
                            entity.Add(modelMap);
                        else
                            entity.Update(modelMap);

                        return Json(new { IsValid = true, Message = MensagemSalvo, Data = Mapper.Map<CupomDescontoModel>(modelMap) }, JsonRequestBehavior.AllowGet);
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
                    var model = entity.GetById<CupomDesconto>(id);
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
        public JsonResult RetornaListaCuponsDesconto(DataTableAjaxModel model)
        {
            using (var entity = new Entity())
            {
                int recordsTotal;
                int recordsFiltered;

                //select
                var list = entity.All<CupomDesconto>();

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
                    x.Codigo,
                    x.DataValidade,
                    x.Quantidade,
                    x.ValorDesconto,
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
        #endregion

        #region Gerador de Codigos
        public string GerarCodigoCupom()
        {
            using (var entity = new Entity())
            {
                Random rand = new Random();
                var countCD = entity.All<CupomDesconto>().Count();
                var bytes = Encoding.ASCII.GetBytes($"PK{countCD}{rand.Next(0, countCD + 1000)}");
                return "#" + Convert.ToBase64String(bytes).Replace("=", rand.Next(0, 20).ToString()).ToUpper();
            }
        }
        #endregion
    }
}