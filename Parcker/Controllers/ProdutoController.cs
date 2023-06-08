using AutoMapper;
using Parcker.Domain;
using Parcker.Models;
using Parcker.Models.Enums;
using Parcker.Repository.Business;
using System;
using System.Linq;
using System.Linq.Dynamic;
using System.Web.Mvc;

namespace Parcker.Controllers
{
    public class ProdutoController : BaseController
    {
        // GET: Produto
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult Edit(int? id)
        {
            using (var entity = new Entity())
            {
                ViewBag.Marcas = entity.All<Marca>().Select(x => new SelectListItem() { Text = x.Descricao, Value = x.Id.ToString() });
                ViewBag.Fornecedores = entity.All<Fornecedor>().Select(x => new SelectListItem() { Text = $"{(x.Pessoa.Tipo == (int)TipoPessoaEnum.Fisica ? (x.Pessoa.CPF + " - " + x.Pessoa.Nome) : (x.Pessoa.CNPJ + " - " + x.Pessoa.RazaoSocial))}", Value = x.Id.ToString() });

                if (!id.HasValue)
                    return View(new ProdutoModel()
                    {
                        DataCriacao = DateTime.Now,
                        Ativo = true,
                        Estoque = 0
                    });
                else
                {
                    var modelMap = Mapper.Map<ProdutoModel>(entity.GetById<Produto>(id.Value));
                    return View(modelMap != null ? modelMap : new ProdutoModel()
                    {
                        Id = 0,
                        DataCriacao = DateTime.Now
                    });
                }
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(ProdutoModel model)
        {
            if (!ModelState.IsValid)
                return Json(new { IsValid = false, Message = MensagemValidacao }, JsonRequestBehavior.AllowGet);
            else
                using (var entity = new Entity())
                {
                    try
                    {
                        var modelMap = Mapper.Map<Produto>(model);

                        if (model.Id == 0)
                            entity.Add(modelMap);
                        else
                            entity.Update(modelMap);

                        return Json(new { IsValid = true, Message = MensagemSalvo, Data = Mapper.Map<ProdutoModel>(modelMap) }, JsonRequestBehavior.AllowGet);
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
        public JsonResult RetornaListaProdutos(DataTableAjaxModel model)
        {
            using (var entity = new Entity())
            {
                int recordsTotal;
                int recordsFiltered;

                //select
                var list = entity.All<Produto>();

                recordsTotal = list.Count();

                //where
                if (model.search.value != null)
                {
                    list = list.Where(x => x.Nome.ToLower().Contains(model.search.value.ToLower()) ||
                    x.Descricao.ToLower().Contains(model.search.value.ToLower()) ||
                    x.CodigoExterno.ToLower().Contains(model.search.value));
                }

                recordsFiltered = list.Count();

                //select
                var data = list.Select(x => new
                {
                    x.Id,
                    x.Nome,
                    x.Descricao,
                    x.Estoque,
                    Fornecedor = x.Fornecedor.Pessoa.Nome,
                    Marca = x.Marca.Descricao,
                    ValorUnitario = x.ValorUnitario.ToString("n2"),
                    x.Ativo,
                    x.AlertaEstoqueMinimo,
                    x.EstoqueMinimo,
                    x.CodigoExterno
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

        public JsonResult ValidaExistenciaCodigoExterno(int Id, string CodigoExterno)
        {
            if (Id != 0)
                return Json(true, JsonRequestBehavior.AllowGet);

            using (var entity = new Entity())
            {
                return Json(!entity.All<Produto>()
                    .Any(x => x.CodigoExterno.ToUpper() == CodigoExterno.ToUpper()), JsonRequestBehavior.AllowGet);
            }
        }

        public JsonResult ValidaExistenciaCodigoBarras(int Id, string CodigoBarras)
        {
            if (Id != 0)
                return Json(true, JsonRequestBehavior.AllowGet);

            using (var entity = new Entity())
            {
                return Json(!entity.All<Produto>()
                    .Any(x => x.CodigoBarras.ToUpper() == CodigoBarras.ToUpper()), JsonRequestBehavior.AllowGet);
            }
        }

        #endregion
    }
}