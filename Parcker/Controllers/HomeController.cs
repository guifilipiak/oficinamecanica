using AutoMapper;
using Parcker.Domain;
using Parcker.Models;
using Parcker.Models.Enums;
using Parcker.Repository;
using Parcker.Repository.Business;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Parcker.Controllers
{
    [Authorize]
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            var listYears = new List<int>();
            listYears.Add(DateTime.Now.Year);
            listYears.Add(DateTime.Now.Year - 1);
            listYears.Add(DateTime.Now.Year - 2);
            listYears.Add(DateTime.Now.Year - 3);
            ViewBag.ListaAnos = listYears.Select(x => new SelectListItem() { Text = x.ToString(), Value = x.ToString(), Selected = (x == DateTime.Now.Year) });
            return View();
        }

        /// <summary>
        /// Gera dados para dashboard simples de resumo de rendimentos de OS
        /// </summary>
        /// <param name="ano"></param>
        /// <returns></returns>
        public JsonResult BuscarResumoRendimentosOS(int? ano)
        {
            if (!ano.HasValue)
                ano = DateTime.Now.Year;

            try
            {
                var modelDynamic = new Dictionary<string, string>();
                using (var entity = new Entity())
                {
                    var totalOS = entity.All<OrdemServico>().Where(x => x.DataCriacao.Year == ano).Count();
                    var totalOSAndamento = entity.All<OrdemServico>()
                        .Where(x => x.DataCriacao.Year == ano && x.IdSituacaoServico == (int)SituacaoServicoEnum.EmAndamento)
                        .Count();
                    var totalOSFinalizada = entity.All<OrdemServico>()
                        .Where(x => x.DataCriacao.Year == ano && x.IdSituacaoServico == (int)SituacaoServicoEnum.Finalizado).ToList();
                    var percentOSFinalizada = totalOS != 0 ? Math.Round(Convert.ToDecimal(100 * totalOSFinalizada.Count() / totalOS)) : 0;
                    var valorTotalOSFinalizada = totalOSFinalizada.Count > 0 ? totalOSFinalizada.Sum(x => x.Total) : 0;

                    modelDynamic.Add("QuantOS", totalOS.ToString());
                    modelDynamic.Add("QuantOSAndamento", totalOSAndamento.ToString());
                    modelDynamic.Add("PercentOSFinalizada", percentOSFinalizada.ToString());
                    modelDynamic.Add("TotalOSFinalizada", valorTotalOSFinalizada.ToString("N2"));
                }
                return Json(new { IsValid = true, Data = modelDynamic }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { IsValid = false }, JsonRequestBehavior.AllowGet);
            }
        }

        /// <summary>
        /// Gera dados para gráfico de estatisticas de OS
        /// </summary>
        /// <param name="ano">[opcional]ano atual</param>
        /// <returns></returns>
        public JsonResult BuscarEstatisticaOS(int? ano)
        {
            var data = new DataPieChartAjaxModel()
            {
                Labels = new List<string>(),
                Datasets = new List<DatasetPieChart>()
            };
            if (!ano.HasValue)
                ano = DateTime.Now.Year;

            using (var entity = new Entity())
            {
                var situacoes = entity.All<SituacaoServico>().Select(x => new
                {
                    Situacao = x.Descricao,
                    Cor = x.Id == (int)SituacaoServicoEnum.Criado ? "#4E73DF"
                    : x.Id == (int)SituacaoServicoEnum.EmAndamento ? "#F6C23E"
                    : x.Id == (int)SituacaoServicoEnum.Cancelado ? "#E74A3B"
                    : x.Id == (int)SituacaoServicoEnum.AguardandoPagamento ? "#5A5C69"
                    : x.Id == (int)SituacaoServicoEnum.Finalizado ? "#1CC88A"
                    : "",
                    Count = x.OrdensServico.Where(o => o.IdSituacaoServico == x.Id && o.DataCriacao.Year == ano).Count()
                }).ToList();
                data.Labels.AddRange(situacoes.Select(x => x.Situacao));

                var ds = new DatasetPieChart()
                {
                    Data = new List<int>(),
                    BackgroundColor = new List<string>(),
                    HoverBackgroundColor = new List<string>()
                };

                foreach (var item in situacoes)
                {
                    ds.BackgroundColor.Add(item.Cor);
                    ds.HoverBackgroundColor.Add(item.Cor);
                    ds.HoverBorderColor = "rgba(234, 236, 244, 1)";
                    ds.Data.Add(item.Count);
                }
                data.Datasets.Add(ds);
            }

            return Json(Newtonsoft.Json.JsonConvert.SerializeObject(data), JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// Gera dados para gráfico de estatisticas de OS Mensal
        /// </summary>
        /// <param name="ano">[opcional]ano atual</param>
        /// <returns></returns>
        public JsonResult BuscarEstatisticaMensalOS(int? ano)
        {
            var data = new DataAreaChartAjaxModel()
            {
                Labels = new List<string>(),
                Datasets = new List<DatasetAreaChart>()
            };
            if (!ano.HasValue)
                ano = DateTime.Now.Year;

            using (var entity = new Entity())
            {
                var ds = new DatasetAreaChart()
                {
                    Data = new List<decimal>(),
                    Label = "Lucro",
                    LineTension = 0.3,
                    BackgroundColor = "rgba(98, 222, 154, 0.1)",
                    BorderColor = "rgba(98, 222, 154, 0.8)",
                    PointRadius = 3,
                    PointBackgroundColor = "rgba(98, 222, 154, 0.8)",
                    PointBorderColor = "rgba(98, 222, 154, 0.8)",
                    PointHoverRadius = 3,
                    PointHoverBackgroundColor = "rgba(98, 222, 154, 1)",
                    PointHoverBorderColor = "rgba(98, 222, 154, 1)",
                    PointHitRadius = 10,
                    PointBorderWidth = 2
                };

                for (int i = 0; i < 12; i++)
                {
                    var abrev = CultureInfo.CurrentUICulture.DateTimeFormat.GetAbbreviatedMonthName(i + 1).ToUpper();

                    var tq1 = entity.All<PagarReceber>()
                        .Where(x => x.IdTipoConta == (int)TipoContaEnum.Pagar
                               && x.DataPagamento.Value.Month == (i + 1)
                               && x.DataPagamento.Value.Year == ano
                               && x.IdSituacaoConta == (int)SituacaoContaEnum.Paga);
                    var t1 = tq1.Count() > 0 ? tq1.Sum(x => x.Valor) : 0;

                    var tq2 = entity.All<PagarReceber>()
                        .Where(x => x.IdTipoConta == (int)TipoContaEnum.Receber
                               && x.DataPagamento.Value.Month == (i + 1)
                               && x.DataPagamento.Value.Year == ano
                               && x.IdSituacaoConta == (int)SituacaoContaEnum.Paga);
                    var t2 = tq2.Count() > 0 ? tq2.Sum(x => x.Valor) : 0;

                    var total = t2 - t1;

                    data.Labels.Add(abrev);
                    ds.Data.Add(total);
                }

                data.Datasets.Add(ds);
            }

            return Json(Newtonsoft.Json.JsonConvert.SerializeObject(data), JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// Gera dados para gráfico de estatisticas de fluxo financeiro
        /// </summary>
        /// <param name="ano">[opcional]ano atual</param>
        /// <returns></returns>
        public JsonResult BuscarEstatisticaFluxoFinanceiro(int? ano)
        {
            var data = new DataAreaChartAjaxModel()
            {
                Labels = new List<string>(),
                Datasets = new List<DatasetAreaChart>()
            };
            if (!ano.HasValue)
                ano = DateTime.Now.Year;

            //adiciona despesas
            var ds1 = new DatasetAreaChart()
            {
                Data = new List<decimal>(),
                Label = "Despesa",
                LineTension = 0.3,
                BackgroundColor = "rgba(227, 84, 94, 0.1)",
                BorderColor = "rgba(227, 84, 94, 0.8)",
                PointRadius = 3,
                PointBackgroundColor = "rgba(227, 84, 94, 0.8)",
                PointBorderColor = "rgba(227, 84, 94, 0.8)",
                PointHoverRadius = 3,
                PointHoverBackgroundColor = "rgba(227, 84, 94, 1)",
                PointHoverBorderColor = "rgba(227, 84, 94, 1)",
                PointHitRadius = 10,
                PointBorderWidth = 2
            };

            //adiciona receitas
            var ds2 = new DatasetAreaChart()
            {
                Data = new List<decimal>(),
                Label = "Receita",
                LineTension = 0.3,
                BackgroundColor = "rgba(98, 222, 154, 0.1)",
                BorderColor = "rgba(98, 222, 154, 0.8)",
                PointRadius = 3,
                PointBackgroundColor = "rgba(98, 222, 154, 0.8)",
                PointBorderColor = "rgba(98, 222, 154, 0.8)",
                PointHoverRadius = 3,
                PointHoverBackgroundColor = "rgba(98, 222, 154, 1)",
                PointHoverBorderColor = "rgba(98, 222, 154, 1)",
                PointHitRadius = 10,
                PointBorderWidth = 2
            };

            using (var entity = new Entity())
            {
                for (int i = 0; i < 12; i++)
                {
                    data.Labels.Add(CultureInfo.CurrentUICulture.DateTimeFormat.GetAbbreviatedMonthName(i + 1).ToUpper());

                    var tq1 = entity.All<PagarReceber>()
                        .Where(x => x.IdTipoConta == (int)TipoContaEnum.Pagar
                               && x.DataPagamento.Value.Month == (i + 1)
                               && x.DataPagamento.Value.Year == ano
                               && x.IdSituacaoConta == (int)SituacaoContaEnum.Paga);
                    var t1 = tq1.Count() > 0 ? tq1.Sum(x => x.Valor) : 0;

                    var tq2 = entity.All<PagarReceber>()
                        .Where(x => x.IdTipoConta == (int)TipoContaEnum.Receber
                               && x.DataPagamento.Value.Month == (i + 1)
                               && x.DataPagamento.Value.Year == ano
                               && x.IdSituacaoConta == (int)SituacaoContaEnum.Paga);
                    var t2 = tq2.Count() > 0 ? tq2.Sum(x => x.Valor) : 0;

                    ds1.Data.Add(t1);
                    ds2.Data.Add(t2);
                }

                data.Datasets.Add(ds1);
                data.Datasets.Add(ds2);
            }

            return Json(Newtonsoft.Json.JsonConvert.SerializeObject(data), JsonRequestBehavior.AllowGet);
        }
    }
}