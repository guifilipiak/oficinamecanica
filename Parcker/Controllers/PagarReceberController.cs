using Parcker.Domain;
using Parcker.Models;
using Parcker.Models.Enums;
using Parcker.Repository.Business;
using System.Linq.Dynamic;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using AutoMapper;
using Rotativa;
using Rotativa.Options;

namespace Parcker.Controllers
{
    public class PagarReceberController : BaseController
    {
        // GET: PagarReceber
        public ActionResult Index()
        {
            var formaPagamentoColletion = new List<SelectListItem>();
            formaPagamentoColletion.Add(new SelectListItem() { Value = ((int)FormasPagamentoEnum.Dinheiro).ToString(), Text = "Dinheiro" });
            formaPagamentoColletion.Add(new SelectListItem() { Value = ((int)FormasPagamentoEnum.CartaoCredito).ToString(), Text = "Cartão de Crédito" });
            formaPagamentoColletion.Add(new SelectListItem() { Value = ((int)FormasPagamentoEnum.CartaoDebito).ToString(), Text = "Cartão de Débito" });
            formaPagamentoColletion.Add(new SelectListItem() { Value = ((int)FormasPagamentoEnum.Boleto).ToString(), Text = "Boleto" });
            formaPagamentoColletion.Add(new SelectListItem() { Value = ((int)FormasPagamentoEnum.Cheque).ToString(), Text = "Cheque" });
            formaPagamentoColletion.Add(new SelectListItem() { Value = ((int)FormasPagamentoEnum.Deposito).ToString(), Text = "Depósito" });
            formaPagamentoColletion.Add(new SelectListItem() { Value = ((int)FormasPagamentoEnum.TransferenciaBancaria).ToString(), Text = "Transferência Bancária" });
            ViewBag.FormaPagamento = formaPagamentoColletion;

            return View();
        }

        public ActionResult Edit(int? id)
        {
            using (var entity = new Entity())
            {
                ViewBag.Pessoas = new List<Pessoa>().Select(x => new SelectListItem()).ToList();
                ViewBag.Classificacao = entity.All<Classificacao>().Select(x => new SelectListItem() { Text = x.Descricao, Value = x.Id.ToString() }).ToList();

                var formaPagamentoColletion = new List<SelectListItem>();
                formaPagamentoColletion.Add(new SelectListItem() { Value = ((int)FormasPagamentoEnum.Dinheiro).ToString(), Text = "Dinheiro" });
                formaPagamentoColletion.Add(new SelectListItem() { Value = ((int)FormasPagamentoEnum.CartaoCredito).ToString(), Text = "Cartão de Crédito" });
                formaPagamentoColletion.Add(new SelectListItem() { Value = ((int)FormasPagamentoEnum.CartaoDebito).ToString(), Text = "Cartão de Débito" });
                formaPagamentoColletion.Add(new SelectListItem() { Value = ((int)FormasPagamentoEnum.Boleto).ToString(), Text = "Boleto" });
                formaPagamentoColletion.Add(new SelectListItem() { Value = ((int)FormasPagamentoEnum.Cheque).ToString(), Text = "Cheque" });
                formaPagamentoColletion.Add(new SelectListItem() { Value = ((int)FormasPagamentoEnum.Deposito).ToString(), Text = "Depósito" });
                formaPagamentoColletion.Add(new SelectListItem() { Value = ((int)FormasPagamentoEnum.TransferenciaBancaria).ToString(), Text = "Transferência Bancária" });
                ViewBag.FormaPagamento = formaPagamentoColletion;

                if (!id.HasValue || id == 0)
                    return View(new PagarReceberModel()
                    {
                        IdTipoConta = TipoContaEnum.Receber,
                        DataCriacao = DateTime.Now,
                        GrupoParcela = 0,
                        Parcela = 1,
                        TotalParcela = 1,
                        IdFormaPagamento = FormasPagamentoEnum.Dinheiro,
                        IdSituacaoConta = SituacaoContaEnum.Pendente,
                        Valor = 0,
                        Descricao = "Não informada"
                    });
                else
                {
                    var modelMap = Mapper.Map<PagarReceberModel>(entity.GetById<PagarReceber>(id.Value));
                    if (modelMap != null)
                    {
                        ViewBag.Pessoas = entity.All<Pessoa>()
                            .Where(x => x.Id == modelMap.IdPessoa)
                            .Select(x => new SelectListItem()
                            {
                                Text = $"{(x.Tipo == (int)TipoPessoaEnum.Fisica ? (x.CPF + " - " + x.Nome) : (x.CNPJ + " - " + x.RazaoSocial))}",
                                Value = x.Id.ToString()
                            }).ToList();

                        if (modelMap.IdFormaPagamento == FormasPagamentoEnum.CartaoCredito)
                        {
                            modelMap = Mapper.Map<PagarReceberModel>(entity.GetById<PagarReceber>(modelMap.GrupoParcela));
                            modelMap.Valor = modelMap.Valor * modelMap.TotalParcela;
                        }
                    }

                    return View(modelMap != null ? modelMap : new PagarReceberModel()
                    {
                        DataCriacao = DateTime.Now,
                        GrupoParcela = 0,
                        Parcela = 1,
                        TotalParcela = 1,
                        IdFormaPagamento = FormasPagamentoEnum.Dinheiro,
                        IdSituacaoConta = SituacaoContaEnum.Pendente,
                        Valor = 0,
                        Descricao = "Não informada"
                    });
                }
            }
        }

        public ActionResult RelatorioFluxoCaixa()
        {
            var m = DateTime.Now.Month > 1 ? DateTime.Now.Month - 1 : 12;
            var y = DateTime.Now.Month > 1 ? DateTime.Now.Year : DateTime.Now.Year - 1;
            return View(new PrinterPRModel()
            {
                DataInicio = new DateTime(y, m, 1),
                DataFinal = new DateTime(y, m, DateTime.DaysInMonth(y, m)),
                GerarPDF = false
            });
        }

        [HttpPost]
        public PartialViewResult PrinterViewer(PrinterPRModel filtro)
        {
            if (ModelState.IsValid)
            {
                using (var entity = new Entity())
                {
                    var model = new PrinterPRModel()
                    {
                        Usuario = entity.All<Usuario>().Where(x => x.Nome == User.Identity.Name).FirstOrDefault(),
                        PagarReceber = entity.All<PagarReceber>()
                            .Where(x => x.DataPagamento >= filtro.DataInicio
                                && x.DataPagamento <= filtro.DataFinal
                                && filtro.IdSituacaoConta.Contains(x.IdSituacaoConta)
                                && filtro.IdFormaPagamento.Contains(x.IdFormaPagamento))
                            .OrderBy(x => x.DataPagamento)
                            .ToList(),
                        DataInicio = filtro.DataInicio,
                        DataFinal = filtro.DataFinal,
                        GerarPDF = filtro.GerarPDF,
                        IdFormaPagamento = filtro.IdFormaPagamento,
                        IdSituacaoConta = filtro.IdSituacaoConta
                    };

                    return PartialView(model);
                }
            }
            return PartialView(null);
        }

        [HttpPost]
        public ActionResult Printer(PrinterPRModel filtro)
        {
            if (ModelState.IsValid)
            {
                using (var entity = new Entity())
                {
                    var model = new PrinterPRModel()
                    {
                        Usuario = entity.All<Usuario>().Where(x => x.Nome == User.Identity.Name).FirstOrDefault(),
                        PagarReceber = entity.All<PagarReceber>()
                            .Where(x => x.DataPagamento >= filtro.DataInicio
                                && x.DataPagamento <= filtro.DataFinal
                                && filtro.IdSituacaoConta.Contains(x.IdSituacaoConta)
                                && filtro.IdFormaPagamento.Contains(x.IdFormaPagamento))
                            .OrderBy(x => x.DataPagamento)
                            .ToList(),
                        DataInicio = filtro.DataInicio,
                        DataFinal = filtro.DataFinal,
                        GerarPDF = filtro.GerarPDF,
                        IdFormaPagamento = filtro.IdFormaPagamento,
                        IdSituacaoConta = filtro.IdSituacaoConta
                    };
                    if (filtro.GerarPDF)
                    {
                        model.GerarPDF = false;
                        return new ViewAsPdf()
                        {
                            FileName = $"Impressao_Relatorio_Caixa_{DateTime.Now.ToShortDateString()}.pdf",
                            ViewName = "Printer",
                            PageSize = Size.A4,
                            IsGrayScale = false,
                            Model = model
                        };
                    }
                    else
                    {
                        model.GerarPDF = true;
                        return View(model);
                    }
                }
            }
            return PartialView(null);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public JsonResult Edit(PagarReceberModel model)
        {
            if (!ModelState.IsValid)
                return Json(new { IsValid = false, Message = MensagemValidacao }, JsonRequestBehavior.AllowGet);
            else
                using (var entity = new Entity())
                {
                    try
                    {
                        entity.BeginTransaction();

                        var modelMap = Mapper.Map<PagarReceber>(model);
                        if (model.Id == 0)
                        {
                            var grupoParcela = 0;
                            var dataPagamento = modelMap.DataPagamento ?? DateTime.Now;
                            var dataVencimento = modelMap.DataVencimento;

                            if (modelMap.IdFormaPagamento == (int)FormasPagamentoEnum.CartaoCredito)
                                //TODO: VALIDAR PARA PROXIMOS PROJETOS INCLUSAO DOS 30 DIAS NA TELA DE CONFIGURACAO
                                dataPagamento = dataPagamento.AddDays(30);
                            else if (modelMap.IdFormaPagamento == (int)FormasPagamentoEnum.CartaoDebito)
                                //TODO: VALIDAR PARA PROXIMOS PROJETOS INCLUSAO DOS 1 DIA NA TELA DE CONFIGURACAO
                                dataPagamento = dataPagamento.AddDays(1);
                            else
                                modelMap.TotalParcela = 1;

                            for (int i = 1; i <= modelMap.TotalParcela; i++)
                            {
                                var newModel = new PagarReceber()
                                {
                                    Id = 0,
                                    Parcela = i,
                                    Valor = modelMap.Valor / modelMap.TotalParcela,
                                    GrupoParcela = grupoParcela,
                                    DataPagamento = dataPagamento,
                                    DataVencimento = dataVencimento,
                                    Descricao = modelMap.Descricao,
                                    IdClassificacao = modelMap.IdClassificacao,
                                    IdFormaPagamento = modelMap.IdFormaPagamento,
                                    IdPessoa = modelMap.IdPessoa,
                                    IdSituacaoConta = modelMap.IdSituacaoConta,
                                    IdTipoConta = modelMap.IdTipoConta,
                                    TotalParcela = modelMap.TotalParcela
                                };

                                entity.Add(newModel);

                                if (grupoParcela == 0)
                                {
                                    newModel.GrupoParcela = newModel.Id;
                                    entity.Update(newModel);
                                    grupoParcela = newModel.Id;
                                    modelMap.Id = newModel.Id;
                                }

                                dataPagamento = newModel.DataPagamento.Value.AddMonths(1);
                                dataVencimento = dataVencimento.HasValue ? newModel.DataVencimento.Value.AddMonths(1) : dataVencimento;
                            }
                        }
                        else
                        {
                            entity.Update(modelMap);
                        }

                        entity.Commit();

                        return Json(new { IsValid = true, Message = MensagemSalvo, Data = Mapper.Map<PagarReceberModel>(modelMap) }, JsonRequestBehavior.AllowGet);
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

                    var model = entity.GetById<PagarReceber>(id);
                    if (model != null)
                    {
                        if (model.HistoricoPagamentos.Count > 0)
                            entity.Delete(model.HistoricoPagamentos);

                        entity.Delete(model);
                    }

                    entity.Commit();

                    return Json(new { IsValid = true, Message = MensagemRemovido }, JsonRequestBehavior.AllowGet);
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError("", ex);
                    return Json(new { IsValid = false, Message = MensagemErro }, JsonRequestBehavior.AllowGet);
                }
            }
        }

        [HttpPost]
        public JsonResult AlterarSituacaoConta(int id, SituacaoContaEnum idSituacaoConta)
        {
            using (var entity = new Entity())
            {
                try
                {
                    entity.BeginTransaction();

                    var model = entity.GetById<PagarReceber>(id);
                    if (model != null)
                    {
                        model.IdSituacaoConta = (int)idSituacaoConta;
                        entity.Update(model);
                    }

                    entity.Commit();

                    return Json(new { IsValid = true, Message = MensagemSalvo }, JsonRequestBehavior.AllowGet);
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError("", ex);
                    return Json(new { IsValid = false, Message = MensagemErro }, JsonRequestBehavior.AllowGet);
                }
            }
        }

        #region Obter Dados
        [HttpPost]
        public JsonResult RetornaLista(DataTableAjaxModel<PagarReceber> model, TipoContaEnum? tipoConta)
        {
            try
            {
                using (var entity = new Entity())
                {
                    int recordsTotal;
                    int recordsFiltered;

                    //select
                    var list = entity.All<PagarReceber>();

                    recordsTotal = list.Count();

                    //where
                    if (model.@object != null)
                    {
                        if (model.@object.DataPagamento.HasValue)
                        {
                            var proxDia = model.@object.DataPagamento.Value.AddDays(1);
                            list = list.Where(x => x.DataPagamento >= model.@object.DataPagamento && x.DataPagamento < proxDia);
                        }
                        if (model.@object.IdTipoConta != 0)
                            list = list.Where(x => x.IdTipoConta == model.@object.IdTipoConta);
                        if (model.@object.IdFormaPagamento != 0)
                            list = list.Where(x => x.IdFormaPagamento == model.@object.IdFormaPagamento);
                    }

                    if (model.search.value != null)
                    {
                        list = list.Where(x => x.Descricao.ToLower().Contains(model.search.value.ToLower()) ||
                        x.Pessoa.Nome.ToLower().Contains(model.search.value.ToLower()) ||
                        x.Pessoa.RazaoSocial.ToLower().Contains(model.search.value.ToLower()) ||
                        x.Pessoa.CPF.Contains(model.search.value) ||
                        x.Pessoa.CNPJ.Contains(model.search.value) ||
                        x.Classificacao.Descricao.ToLower().Contains(model.search.value.ToLower()));
                    }

                    recordsFiltered = list.Count();

                    //select
                    var data = list.Select(x => new
                    {
                        x.Id,
                        Codigo = x.Id,
                        x.DataCriacao,
                        x.Descricao,
                        DataPagamento = x.DataPagamento.HasValue ? x.DataPagamento.Value.ToString("dd/MM/yyyy") : "--",
                        DataVencimento = x.DataVencimento.HasValue ? x.DataVencimento.Value.ToString("dd/MM/yyyy") : "--",
                        DiasVencimento = x.DataVencimento.HasValue ? x.DataVencimento.Value < DateTime.Now ? x.DataVencimento.Value.Subtract(DateTime.Now).TotalDays : 0 : 0,
                        Pessoa = string.IsNullOrEmpty(x.Pessoa.Nome) ? x.Pessoa.RazaoSocial : x.Pessoa.Nome,
                        SituacaoConta = x.SituacaoConta.Descricao,
                        GrupoParcelaTexto = x.Parcela + "/" + x.TotalParcela,
                        x.GrupoParcela,
                        Classificacao = x.Classificacao.Descricao,
                        x.IdTipoConta,
                        x.IdSituacaoConta,
                        OrdenacaoSituacao = x.IdSituacaoConta == 2 ? 3 : x.IdSituacaoConta == 3 ? 2 : 1,
                        TipoConta = x.TipoConta.Descricao,
                        Valor = x.Valor.ToString("N2"),
                        ValorInterno = x.Valor,
                        FormasPagamento = x.FormasPagamento.Descricao
                    }).ToList();

                    //order
                    model.order.ForEach(x =>
                    {
                        //tratamento de cultura na ordenacao
                        var colOrdem = model.columns[x.column].data;
                        if (colOrdem == "Valor")
                            colOrdem = "ValorInterno";

                        if (colOrdem == "GrupoParcelaTexto")
                            colOrdem = "GrupoParcela";

                        data = data.OrderBy($"OrdenacaoSituacao, DiasVencimento desc, {colOrdem} {x.dir}").ToList();
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
            catch (Exception ex)
            {
                throw ex;
            }
        }
        #endregion

        #region Validacao
        /// <summary>
        /// Validar obrigatoriedade da Data de Pagamento quando situacao for paga e Data de Pagamento no Futuro
        /// </summary>
        /// <param name="DataPagamento"></param>
        /// <param name="IdSituacaoConta"></param>
        /// <returns></returns>
        public JsonResult ValidarDataPagamentoSituacao(string DataPagamento, SituacaoContaEnum IdSituacaoConta)
        {
            DateTime? dataPagamento;
            try
            {
                dataPagamento = DateTime.Parse(DataPagamento);
            }
            catch (Exception)
            {
                dataPagamento = null;
            }
            return Json(!(!dataPagamento.HasValue && IdSituacaoConta == SituacaoContaEnum.Paga), JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// Validar parcela mínima
        /// </summary>
        /// <param name="Valor"></param>
        /// <param name="TotalParcela"></param>
        /// <param name="IdSituacaoConta"></param>
        /// <returns></returns>
        public JsonResult ValidarValorParcela(decimal? Valor, int TotalParcela, FormasPagamentoEnum IdFormaPagamento)
        {
            if (Valor.HasValue && TotalParcela > 1 && IdFormaPagamento == FormasPagamentoEnum.CartaoCredito)
                return Json((Valor.Value / TotalParcela) >= 1, JsonRequestBehavior.AllowGet);
            else
                return Json(true, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// Valida valor menor que zero
        /// </summary>
        /// <param name="Valor"></param>
        /// <returns></returns>
        public JsonResult ValidarValor(decimal Valor)
        {
            return Json(Valor > decimal.Zero, JsonRequestBehavior.AllowGet);
        }
        #endregion
    }
}