using AutoMapper;
using Parcker.Domain;
using Parcker.Models;
using Parcker.Models.Enums;
using Parcker.Repository.Business;
using Rotativa;
using Rotativa.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Linq.Dynamic;
using Parcker.Common;
using System.IO;
using Parcker.Properties;

namespace Parcker.Controllers
{
    public class OrdemServicoController : BaseController
    {
        // GET: OrdemServico
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult Edit(int? id, int tipo = 1)
        {
            using (var entity = new Entity())
            {
                ViewBag.Veiculos = new List<Veiculo>().Select(x => new SelectListItem()).ToList();
                ViewBag.Clientes = new List<Cliente>().Select(x => new SelectListItem()).ToList();
                ViewBag.Tipo = tipo;

                ViewBag.Situacoes = entity.All<SituacaoServico>().Select(x => new SelectListItem() { Text = x.Descricao, Value = x.Id.ToString() }).ToList();
                ViewBag.Descontos = entity.All<CupomDesconto>().Where(x => x.Ativo && x.DataValidade >= DateTime.Now).Select(x => new SelectListItem() { Text = $"{x.Codigo}-{x.Descricao} R$ {x.ValorDesconto.ToString("N2")}", Value = x.Id.ToString() }).ToList();

                if (!id.HasValue)
                {
                    var newModel = new OrdemServicoModel()
                    {
                        DataCriacao = DateTime.Now,
                        IdSituacaoServico = (int)SituacaoServicoEnum.Criado,
                        Desconto = 0,
                        Entrada = 0,
                        IdVeiculo = 0,
                        IdCliente = 0,
                        KM = 0,
                        IdTipoAtendimento = tipo,
                        DataValidadeOrcamento = DateTime.Now.AddDays(7)
                    };
                    
                    return View(newModel);
                }
                else
                {
                    var modelMap = Mapper.Map<OrdemServicoModel>(entity.GetById<OrdemServico>(id.Value));
                    if (modelMap != null)
                    {
                        ViewBag.Tipo = modelMap.IdTipoAtendimento;
                        var veiculo = entity.All<Veiculo>()
                            .Where(x => x.Id == modelMap.IdVeiculo);

                        //carrega combos com valores selecionados
                        ViewBag.Veiculos = veiculo
                            .Select(x => new SelectListItem()
                            {
                                Text = $"{x.Placa} - {x.Modelo}",
                                Value = x.Id.ToString()
                            }).ToList();
                        ViewBag.Clientes = entity.All<Cliente>()
                            .Where(x => x.Id == modelMap.IdCliente)
                            .Select(x => new SelectListItem()
                            {
                                Text = $"{(x.Pessoa.Tipo == (int)TipoPessoaEnum.Fisica ? (x.Pessoa.CPF + " - " + x.Pessoa.Nome) : (x.Pessoa.CNPJ + " - " + x.Pessoa.RazaoSocial))}",
                                Value = x.Id.ToString()
                            }).ToList();
                    }

                    return View(modelMap != null ? modelMap : new OrdemServicoModel()
                    {
                        Id = 0,
                        DataCriacao = DateTime.Now,
                        IdSituacaoServico = (int)SituacaoServicoEnum.Criado,
                        Desconto = 0,
                        Entrada = 0,
                        KM = 0,
                        IdTipoAtendimento = tipo,
                        DataValidadeOrcamento = DateTime.Now.AddDays(7)
                    });
                }
            }
        }

        public PartialViewResult FormModalPagamento(int idOdemServico)
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

            using (var entity = new Entity())
            {
                var ordemServico = entity.GetById<OrdemServico>(idOdemServico);
                return PartialView(new Parcker.Models.PagarReceberModel()
                {
                    Id = 0,
                    IdFormaPagamento = FormasPagamentoEnum.Dinheiro,
                    Valor = ordemServico?.Total ?? 0,
                    TotalParcela = 1,
                    DataPagamento = ordemServico?.DataFinalizacao ?? DateTime.Now,
                    Descricao = "Não informada"
                });
            }
        }

        public ActionResult Printer(int id, bool gerarPDF)
        {
            if (id == 0)
            {
                return RedirectToAction("Edit");
            }

            using (var entity = new Entity())
            {
                var model = new PrinterOSModel()
                {
                    Usuario = entity.All<Usuario>().Where(x => x.Nome == User.Identity.Name).FirstOrDefault(),
                    OrdemServico = entity.GetById<OrdemServico>(id)
                };
                if (gerarPDF)
                    return new ViewAsPdf()
                    {
                        FileName = $"Impressao_OrdemServico_N{id}.pdf",
                        ViewName = "Printer",
                        PageSize = Size.A4,
                        IsGrayScale = false,
                        Model = model
                    };
                else
                    return View(model);
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(OrdemServicoModel model, FormCollection inputs, int tipo = 1)
        {
            var itens = ConvertFormToEntityItens(inputs, model.IdSituacaoServico == (int)SituacaoServicoEnum.Cancelado);
            if (!ModelState.IsValid)
            {
                var errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage).ToList();
                return Json(new { IsValid = false, Messages = errors, Message = "Verifique os campos obrigatórios." }, JsonRequestBehavior.AllowGet);
            }
            else
                using (var entity = new Entity())
                {
                    try
                    {
                        entity.BeginTransaction();

                        //atualiza veiculo apenas se existir
                        if (model.IdVeiculo.HasValue && model.IdVeiculo.Value > 0)
                        {
                            var veiculo = entity.All<Veiculo>()
                                .Where(x => x.Id == model.IdVeiculo)
                                .FirstOrDefault();
                            if (veiculo != null)
                            {
                                veiculo.KM = model.KM;
                                entity.SaveOrUpdate(veiculo);
                            }
                        }

                        //criar ou atualizar os
                        var modelMap = Mapper.Map<OrdemServico>(model);
                        var isNewRecord = model.Id == 0;
                        
                        if (isNewRecord)
                        {
                            // Novo registro
                            modelMap.Id = 0;
                            modelMap.DataCriacao = DateTime.Now;
                            entity.Add(modelMap);
                        }
                        else
                        {
                            // Atualizar registro existente
                            entity.SaveOrUpdate(modelMap);
                        }

                        //atualizar itens da os
                        var itensDB = new List<ProdServOS>();
                        var itensDeletar = new List<ProdServOS>();
                        
                        if (model.Id > 0)
                        {
                            itensDB = entity.All<ProdServOS>().Where(x => x.IdOrdemServico == model.Id).ToList();
                            itensDeletar = itensDB.SkipWhile(x => itens.Where(z => z.Id == x.Id).Any()).ToList();
                        }

                        //adicionar e atualizar
                        List<ProdServOSModel> prodServOSModel = new List<ProdServOSModel>();
                        foreach (var item in itens)
                        {
                            //map
                            var itemMapped = Mapper.Map<ProdServOS>(item);
                            itemMapped.IdOrdemServico = modelMap.Id;

                            //merge
                            var merge = entity.Merge(itemMapped);
                            entity.SaveOrUpdate(merge);

                            //inverter merge
                            var mapperInverse = Mapper.Map<ProdServOSModel>(merge);
                            mapperInverse.Index = item.Index;
                            prodServOSModel.Add(mapperInverse);

                            //baixa estoque quando finalizar pedido
                            if (model.IdSituacaoServico == (int)SituacaoServicoEnum.Finalizado)
                            {
                                //atualiza estoque
                                if (item.IdProduto.HasValue)
                                {
                                    var produto = entity.GetById<Produto>(item.IdProduto.Value);
                                    produto.Estoque = produto.Estoque - item.Quantidade;

                                    if (produto.Estoque < 0)
                                    {
                                        entity.Rollback();
                                        return Json(new { IsValid = false, Message = $"O item {produto.Nome} está com estoque abaixo do necessário para fechar esta OS!" }, JsonRequestBehavior.AllowGet);
                                    }

                                    entity.SaveOrUpdate(produto);
                                }
                            }
                        }

                        //deletar
                        foreach (var item in itensDeletar)
                        {
                            entity.Delete(item);
                        }

                        //atualiza os
                        modelMap.SubTotal = itens.Sum(x => x.ValorUnitario * x.Quantidade);
                        modelMap.Desconto = itens.Sum(x => x.Desconto);
                        modelMap.Total = itens.Sum(x => x.ValorUnitario * x.Quantidade - x.Desconto);
                        if (modelMap.IdSituacaoServico == (int)SituacaoServicoEnum.Finalizado || modelMap.IdSituacaoServico == (int)SituacaoServicoEnum.Cancelado)
                        {
                            modelMap.DataFinalizacao = DateTime.Now;
                        }

                        //calcula desconto extra
                        if (modelMap.IdCupomDesconto.HasValue)
                        {
                            var desconto = entity.GetById<CupomDesconto>(modelMap.IdCupomDesconto.Value).ValorDesconto;
                            modelMap.Desconto += desconto;
                            modelMap.Total = modelMap.Total - desconto;
                        }

                        if ((modelMap.Total - modelMap.Entrada) < 0)
                        {
                            entity.Rollback();
                            return Json(new { IsValid = false, Message = "O valor da OS não pode ser negativa, verifique os itens e os descontos adicionados!" }, JsonRequestBehavior.AllowGet);
                        }

                        entity.SaveOrUpdate(modelMap);

                        entity.Commit();

                        var result = Mapper.Map<OrdemServicoModel>(modelMap);
                        result.Itens = prodServOSModel;

                        return Json(new { IsValid = true, Message = MensagemSalvo, Data = result }, JsonRequestBehavior.AllowGet);
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
        public ActionResult InformarPagamento(int IdOrdemServico, FormCollection inputs)
        {
            var itens = ConvertFormToEntityPaymentItens(inputs);
            using (var entity = new Entity())
            {
                try
                {
                    entity.BeginTransaction();

                    var ordem = entity.GetById<OrdemServico>(IdOrdemServico);
                    if (ordem == null)
                    {
                        entity.Rollback();
                        return Json(new { IsValid = false, Message = MensagemErro }, JsonRequestBehavior.AllowGet);
                    }

                    // Validar se há itens de pagamento
                    if (itens == null || itens.Count == 0)
                    {
                        entity.Rollback();
                        return Json(new { IsValid = false, Message = "É necessário informar pelo menos uma forma de pagamento." }, JsonRequestBehavior.AllowGet);
                    }

                    // Validar se o valor total dos pagamentos fecha com o valor da OS
                    var totalPagamentos = itens.Sum(x => x.Valor);
                    var saldoOS = ordem.Total - ordem.Entrada;
                    
                    if (Math.Abs((int)(totalPagamentos - saldoOS)) > 0.01m) // Tolerância de 1 centavo para arredondamentos
                    {
                        entity.Rollback();
                        return Json(new { IsValid = false, Message = $"O valor total dos pagamentos (R$ {totalPagamentos:N2}) deve ser igual ao saldo da OS (R$ {saldoOS:N2})." }, JsonRequestBehavior.AllowGet);
                    }

                    if (itens != null)
                    {
                        foreach (var model in itens)
                        {
                            var grupoParcela = 0;
                            var dataPagamento = model.DataPagamento ?? DateTime.Now;

                            if (model.IdFormaPagamento == FormasPagamentoEnum.CartaoCredito)
                                //TODO: VALIDAR PARA PROXIMOS PROJETOS INCLUSAO DOS 30 DIAS NA TELA DE CONFIGURACAO
                                dataPagamento = dataPagamento.AddDays(30);
                            else if (model.IdFormaPagamento == FormasPagamentoEnum.CartaoDebito)
                                //TODO: VALIDAR PARA PROXIMOS PROJETOS INCLUSAO DOS 1 DIA NA TELA DE CONFIGURACAO
                                dataPagamento = dataPagamento.AddDays(1);
                            else
                                model.TotalParcela = 1;

                            for (int i = 1; i <= model.TotalParcela; i++)
                            {
                                var newModel = new PagarReceber()
                                {
                                    Id = 0,
                                    Parcela = i,
                                    Valor = model.Valor / model.TotalParcela,
                                    GrupoParcela = grupoParcela,
                                    DataPagamento = dataPagamento,
                                    DataVencimento = null,
                                    Descricao = $"OS N° {ordem.Id}",
                                    IdClassificacao = (int)ClassificacaoEnum.ReceitaNormal,
                                    IdFormaPagamento = (int)model.IdFormaPagamento,
                                    IdPessoa = ordem.Cliente.IdPessoa,
                                    IdSituacaoConta = (int)SituacaoContaEnum.Paga,
                                    IdTipoConta = (int)TipoContaEnum.Receber,
                                    TotalParcela = model.TotalParcela
                                };

                                entity.Add(newModel);

                                if (grupoParcela == 0)
                                {
                                    newModel.GrupoParcela = newModel.Id;
                                    entity.Update(newModel);
                                    grupoParcela = newModel.Id;
                                }

                                dataPagamento = newModel.DataPagamento.Value.AddMonths(1);

                                entity.Add(new HistoricoPagamento()
                                {
                                    DataCriacao = DateTime.Now,
                                    IdOrdemServico = ordem.Id,
                                    IdPagarReceber = newModel.Id
                                });
                            }
                        }
                    }

                    entity.Commit();

                    return Json(new { IsValid = true, Message = MensagemSalvo, Data = "", Pagamento = true }, JsonRequestBehavior.AllowGet);
                }
                catch (Exception ex)
                {
                    entity.Rollback();
                    ModelState.AddModelError("", ex);
                    return Json(new { IsValid = false, Message = MensagemErro }, JsonRequestBehavior.AllowGet);
                }
            }
        }

        /// <summary>
        /// Faz a leitura dos dados de formulario e convert para objeto
        /// </summary>
        /// <param name="inputs"></param>
        /// <returns></returns>
        public List<ProdServOSModel> ConvertFormToEntityItens(FormCollection inputs, bool ignoreStorageValidate)
        {
            var collection = new List<ProdServOSModel>();
            if (inputs.GetValues("Grid_Index") != null)
            {
                var count = inputs.GetValues("Grid_Index").Length;
                for (int i = 0; i < count; i++)
                {
                    int? value = null;
                    collection.Add(new ProdServOSModel()
                    {
                        Index = Convert.ToInt32(string.IsNullOrEmpty(inputs.GetValues("Grid_Index")[i]) ? "0" : inputs.GetValues("Grid_Index")[i]),
                        Id = Convert.ToInt32(string.IsNullOrEmpty(inputs.GetValues("Grid_Id")[i]) ? "0" : inputs.GetValues("Grid_Id")[i]),
                        IdOrdemServico = Convert.ToInt32(string.IsNullOrEmpty(inputs.GetValues("Grid_IdOrdemServico")[i]) ? "0" : inputs.GetValues("Grid_IdOrdemServico")[i]),
                        IdProduto = string.IsNullOrEmpty(inputs.GetValues("Grid_IdProduto")[i]) ? value : Convert.ToInt32(inputs.GetValues("Grid_IdProduto")[i]),
                        IdServico = string.IsNullOrEmpty(inputs.GetValues("Grid_IdServico")[i]) ? value : Convert.ToInt32(inputs.GetValues("Grid_IdServico")[i]),
                        Desconto = Convert.ToDecimal(string.IsNullOrEmpty(inputs.GetValues("Grid_Desconto")[i]) ? "0" : inputs.GetValues("Grid_Desconto")[i]),
                        Quantidade = Convert.ToInt32(string.IsNullOrEmpty(inputs.GetValues("Grid_Quantidade")[i]) ? "1" : inputs.GetValues("Grid_Quantidade")[i]),
                        Tipo = Convert.ToInt32(string.IsNullOrEmpty(inputs.GetValues("Grid_Tipo")[i]) ? "1" : inputs.GetValues("Grid_Tipo")[i]),
                        ValorUnitario = Convert.ToDecimal(string.IsNullOrEmpty(inputs.GetValues("Grid_ValorUnitario")[i]) ? "0" : inputs.GetValues("Grid_ValorUnitario")[i]),
                        ValorTotal = Convert.ToDecimal(string.IsNullOrEmpty(inputs.GetValues("Grid_ValorTotal")[i]) ? "0" : inputs.GetValues("Grid_ValorTotal")[i])
                    });

                    if (!ignoreStorageValidate)
                    {
                        using (var entity = new Entity())
                        {
                            var item = collection[i];
                            //valida estoque produto
                            if (item.Tipo == (int)TipoItemEnum.Produto)
                            {
                                var produto = entity.GetById<Produto>(item.IdProduto.GetValueOrDefault());

                                //valida valor negativo
                                if (item.ValorTotal <= 0)
                                {
                                    ModelState.AddModelError("", $"O item {produto.Descricao} não pode ser adicionado com valor negativo!");
                                }

                                if (item.Quantidade > produto.Estoque)
                                {
                                    ModelState.AddModelError("", $"O item {produto.Descricao} está com estoque abaixo do informado!");
                                }
                            }
                            else
                            {
                                var servico = entity.GetById<Servico>(item.IdServico.GetValueOrDefault());

                                //valida valor negativo
                                if (item.ValorTotal <= 0)
                                {
                                    ModelState.AddModelError("", $"O item {servico.Descricao} não pode ser adicionado com valor negativo!");
                                }
                            }
                        }
                    }
                }
            }
            return collection;
        }

        /// <summary>
        /// Faz a leitura dos dados de formulario e convert para objeto
        /// </summary>
        /// <param name="inputs"></param>
        /// <returns></returns>
        public List<PagarReceberModel> ConvertFormToEntityPaymentItens(FormCollection inputs)
        {
            var collection = new List<PagarReceberModel>();
            if (inputs.GetValues("DataPagamento") != null)
            {
                var count = inputs.GetValues("DataPagamento").Length;
                for (int i = 0; i < count; i++)
                {
                    collection.Add(new PagarReceberModel()
                    {
                        DataPagamento = Convert.ToDateTime(string.IsNullOrEmpty(inputs.GetValues("DataPagamento")[i]) ? DateTime.Now.ToString("dd/MM/yyyy") : inputs.GetValues("DataPagamento")[i]),
                        IdFormaPagamento = (FormasPagamentoEnum)Convert.ToInt32(string.IsNullOrEmpty(inputs.GetValues("IdFormaPagamento")[i]) ? ((int)FormasPagamentoEnum.Dinheiro).ToString() : inputs.GetValues("IdFormaPagamento")[i]),
                        TotalParcela = Convert.ToInt32(string.IsNullOrEmpty(inputs.GetValues("TotalParcela")[i]) ? "1" : inputs.GetValues("TotalParcela")[i]),
                        Valor = Convert.ToDecimal(string.IsNullOrEmpty(inputs.GetValues("Valor")[i]) ? "0" : inputs.GetValues("Valor")[i])
                    });
                }
            }
            return collection;
        }

        [HttpPost]
        public JsonResult Delete(int id)
        {
            using (var entity = new Entity())
            {
                try
                {
                    entity.BeginTransaction();
                    var model = entity.GetById<OrdemServico>(id);
                    if (model != null)
                    {
                        entity.Delete(model.Itens);
                        entity.Delete(model.HistoricoPagamentos);
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
        public JsonResult EnviarEmailOS(int id, string destinatario)
        {
            using (var entity = new Entity())
            {
                try
                {
                    var configuracao = entity.All<ConfiguracaoEmail>().FirstOrDefault();
                    var os = entity.GetById<OrdemServico>(id);
                    var usuario = entity.All<Usuario>().Where(x => x.Nome == User.Identity.Name).FirstOrDefault();

                    var nome_cliente = os.Cliente.Pessoa.Nome;
                    var valor_total = os.Total.ToString("N2");

                    if (string.IsNullOrEmpty(destinatario))
                        destinatario = os.Cliente.Pessoa.Email;

                    var model = new PrinterOSModel()
                    {
                        OrdemServico = os,
                        Usuario = usuario
                    };

                    if (string.IsNullOrWhiteSpace(model.OrdemServico.Cliente.Pessoa.Email) && string.IsNullOrWhiteSpace(destinatario))
                        return Json(new { IsValid = false, IsHaveEmail = false, Message = "Cliente não possui e-mail cadastrado!" });

                    ControllerContext.RouteData.Values.Add("gerarPDF", true);

                    var pdf = new ViewAsPdf()
                    {
                        FileName = $"Impressao_OrdemServico_N{id}.pdf",
                        ViewName = "Printer",
                        PageSize = Size.A4,
                        IsGrayScale = false,
                        Model = model
                    }.BuildFile(ControllerContext);

                    Email email = new Email(configuracao.Servidor, configuracao.Porta, configuracao.Remetente, configuracao.Usuario, configuracao.Senha);
                    var file = new Dictionary<string, Stream>();

                    using (var ms = new MemoryStream(pdf))
                    {
                        file.Add($"Impressao_OrdemServico_N{id}.pdf", ms);

                        var texto = Resources.EmailTemplate.Replace("%cliente%", nome_cliente).Replace("%txt1%", "Aqui está sua ordem de serviço, qualquer dúvida entre em contato.").Replace("%txt2%", $"OS N°: <b>{id}</b>").Replace("%txt3%", $"Valor Total: R$ {valor_total}");

                        email.EnviarEmail(
                            new[] { destinatario },
                            $"Parcker Auto Center - Ordem de Serviço N° {id}",
                            texto,
                            file
                        );
                    }
                }
                catch (Exception ex)
                {
                    return Json(new { IsValid = false, IsHaveEmail = true, Message = "Falha ao tentar enviar e-mail!" });
                }
            }

            return Json(new { IsValid = true, IsHaveEmail = true, Message = "E-mail enviado com sucesso!" });
        }

        public JsonResult ObterVeiculo(int id)
        {
            using (var entity = new Entity())
            {
                try
                {
                    var veiculo = entity.GetById<Veiculo>(id);
                    return Json(new
                    {
                        veiculo.Id,
                        veiculo.KM,
                        Proprietario = veiculo.Pessoa?.Nome,
                        veiculo.Placa,
                        veiculo.Modelo,
                        Marca = veiculo.Marca?.Descricao
                    }, JsonRequestBehavior.AllowGet);
                }
                catch (Exception ex)
                {
                    throw ex;
                }
            }
        }

        public JsonResult ListarItens(string query)
        {
            using (var entity = new Entity())
            {
                var produtos = entity.All<Produto>().Where(x => x.CodigoExterno.ToLower().Contains(query.ToLower()) ||
                x.CodigoBarras == query ||
                (("P" + x.Id.ToString()).ToLower() == query.ToLower() || x.Descricao.ToLower().Contains(query.ToLower()))
                && x.Ativo).Select(x => new
                {
                    x.Id,
                    Tipo = (int)TipoItemEnum.Produto,
                    x.Descricao,
                    DescricaoCompleta = $"Cód.: P{x.Id} - {x.Descricao.ToUpper()} - R$ {x.ValorUnitario}",
                    ValorUnitario = x.ValorUnitario.ToString("F2")
                }).Take(10).ToList();

                var servicos = entity.All<Servico>().Where(x =>
                (("S" + x.Id.ToString()).ToLower() == query.ToLower() || x.Descricao.ToLower().Contains(query.ToLower())))
                .Select(x => new
                {
                    x.Id,
                    Tipo = (int)TipoItemEnum.Servico,
                    x.Descricao,
                    DescricaoCompleta = $"Cód.: S{x.Id} - {x.Descricao.ToUpper()} - R$ {x.ValorUnitario}",
                    ValorUnitario = x.ValorUnitario.ToString("F2")
                }).Take(10).ToList();

                var result = produtos.Concat(servicos).ToList();

                return Json(result, JsonRequestBehavior.AllowGet);
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
                    DescricaoCompleta = $"{x.Placa.ToUpper()} - {x.Modelo.ToUpper()}"
                }).Take(10).ToList();

                return Json(result, JsonRequestBehavior.AllowGet);
            }
        }

        public JsonResult ListarClientes(string query)
        {
            using (var entity = new Entity())
            {
                var result = entity.All<Cliente>().Where(x =>
                x.Apelido.ToLower().Contains(query.ToLower()) ||
                x.Pessoa.Nome.ToLower().Contains(query.ToLower()) ||
                x.Pessoa.RazaoSocial.ToLower().Contains(query.ToLower()) ||
                x.Pessoa.CPF.Contains(query) || x.Pessoa.CNPJ.Contains(query)).Select(x => new
                {
                    x.Id,
                    DescricaoCompleta = $"{(x.Pessoa.Tipo == (int)TipoPessoaEnum.Fisica ? (x.Pessoa.CPF + " - " + x.Pessoa.Nome) : (x.Pessoa.CNPJ + " - " + x.Pessoa.RazaoSocial))}"
                }).Take(10).ToList();

                return Json(result, JsonRequestBehavior.AllowGet);
            }
        }

        [HttpPost]
        public JsonResult RetornaListaOrdensServico(DataTableAjaxModel model)
        {
            using (var entity = new Entity())
            {
                int recordsTotal;
                int recordsFiltered;

                //select
                var list = entity.All<OrdemServico>();

                recordsTotal = list.Count();

                //where
                if (model.search.value != null)
                {
                    var listId = list.Where(x => x.Id.ToString() == model.search.value);
                    if (listId.Count() == 0)
                    {
                        list = list.Where(x => x.DescricaoServico.ToLower().Contains(model.search.value.ToLower()) ||
                        x.Cliente.Pessoa.Nome.ToLower().Contains(model.search.value.ToLower()) ||
                        x.Cliente.Pessoa.RazaoSocial.ToLower().Contains(model.search.value.ToLower()) ||
                        x.Cliente.Pessoa.CPF.Contains(model.search.value) ||
                        x.Cliente.Pessoa.CNPJ.Contains(model.search.value) ||
                        x.Veiculo.Placa.ToLower().Contains(model.search.value.ToLower()) ||
                        x.Veiculo.Modelo.ToLower().Contains(model.search.value.ToLower()));
                    }
                    else
                        list = listId;
                }

                recordsFiltered = list.Count();

                //select
                var data = list.Select(x => new
                {
                    x.Id,
                    Codigo = x.Id,
                    x.DataCriacao,
                    x.DescricaoServico,
                    Cliente = x.Cliente != null ? x.Cliente.Pessoa.Nome : "N/A",
                    Veiculo = x.Veiculo != null ? $"{x.Veiculo.Placa} - {x.Veiculo.Modelo}" : "N/A",
                    TipoAtendimento = x.IdTipoAtendimento == 1 ? "Ordem de Serviço" : "Orçamento",
                    SituacaoServico = x.SituacaoServico.Descricao,
                    Pagamento = x.HistoricoPagamentos.Count > 0 && x.IdSituacaoServico == (int)SituacaoServicoEnum.Finalizado ? "PAGO" : x.HistoricoPagamentos.Count == 0 && x.IdSituacaoServico == (int)SituacaoServicoEnum.Finalizado ? "PENDENTE" : x.IdSituacaoServico == (int)SituacaoServicoEnum.Cancelado ? "CANCELADO" : "EM PROCESSO",
                    Total = x.Total.ToString("N2"),
                    TotalInterno = x.Total
                }).ToList();

                //order
                model.order.ForEach(x =>
                {
                    //tratamento de cultura na ordenacao
                    var colOrdem = model.columns[x.column].data;
                    if (colOrdem == "Total")
                        colOrdem = "TotalInterno";

                    data = data.OrderBy($"{colOrdem} {x.dir}").ToList();
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
        [HttpPost]
        public JsonResult GerarOrdemServico(int id)
        {
            using (var entity = new Entity())
            {
                try
                {
                    entity.BeginTransaction();
                    var orcamento = entity.GetById<OrdemServico>(id);
                    if (orcamento != null && orcamento.IdTipoAtendimento == 2)
                    {
                        orcamento.IdTipoAtendimento = 1;
                        orcamento.DataValidadeOrcamento = null;
                        orcamento.IdSituacaoServico = 2;
                        entity.SaveOrUpdate(orcamento);
                        entity.Commit();
                        return Json(new { IsValid = true, Message = "Orçamento convertido em Ordem de Serviço com sucesso!" }, JsonRequestBehavior.AllowGet);
                    }
                    else
                    {
                        entity.Rollback();
                        return Json(new { IsValid = false, Message = "Registro não encontrado ou não é um orçamento." }, JsonRequestBehavior.AllowGet);
                    }
                }
                catch (Exception ex)
                {
                    entity.Rollback();
                    return Json(new { IsValid = false, Message = MensagemErro }, JsonRequestBehavior.AllowGet);
                }
            }
        }

        #endregion
    }
}