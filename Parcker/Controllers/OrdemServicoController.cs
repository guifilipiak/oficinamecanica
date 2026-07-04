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
                ViewBag.Funcionarios = entity.All<Funcionario>().Where(x => x.Ativo).Select(x => new SelectListItem() { Text = x.Pessoa.Nome, Value = x.Id.ToString() }).ToList();

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
            formaPagamentoColletion.Add(new SelectListItem() { Value = ((int)FormasPagamentoEnum.PIX).ToString(), Text = "PIX" });
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
                    if (configuracao == null)
                        return Json(new { IsValid = false, IsHaveEmail = true, Message = "Configuração de e-mail não encontrada!" });

                    var os = entity.GetById<OrdemServico>(id);
                    var usuario = entity.All<Usuario>().Where(x => x.Nome == User.Identity.Name).FirstOrDefault();

                    if (string.IsNullOrEmpty(destinatario))
                        destinatario = os.Cliente.Pessoa.Email;

                    if (string.IsNullOrWhiteSpace(destinatario))
                        return Json(new { IsValid = false, IsHaveEmail = false, Message = "Cliente não possui e-mail cadastrado!" });

                    var model = new PrinterOSModel()
                    {
                        OrdemServico = os,
                        Usuario = usuario
                    };

                    ControllerContext.RouteData.Values.Add("gerarPDF", true);

                    var pdf = new ViewAsPdf()
                    {
                        FileName = $"Impressao_OrdemServico_N{id}.pdf",
                        ViewName = "Printer",
                        PageSize = Size.A4,
                        IsGrayScale = false,
                        Model = model
                    }.BuildFile(ControllerContext);

                    var email = new Email(configuracao.Servidor, configuracao.Porta, configuracao.Remetente, configuracao.Usuario, configuracao.Senha);
                    var file = new Dictionary<string, Stream>();
                    var ms = new MemoryStream(pdf);
                    
                    file.Add($"Impressao_OrdemServico_N{id}.pdf", ms);

                    var nome_cliente = os.Cliente.Pessoa.Nome;
                    var valor_total = os.Total.ToString("N2");
                    var texto = Resources.EmailTemplate.Replace("%cliente%", nome_cliente).Replace("%txt1%", "Aqui está sua ordem de serviço, qualquer dúvida entre em contato.").Replace("%txt2%", $"OS N°: <b>{id}</b>").Replace("%txt3%", $"Valor Total: R$ {valor_total}");

                    email.EnviarEmail(
                        new[] { destinatario },
                        $"Parcker Auto Center - Ordem de Serviço N° {id}",
                        texto,
                        file
                    );

                    return Json(new { IsValid = true, IsHaveEmail = true, Message = "E-mail enviado com sucesso!" });
                }
                catch (Exception ex)
                {
                    return Json(new { IsValid = false, IsHaveEmail = true, Message = "Falha ao tentar enviar e-mail!" });
                }
            }
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
                        veiculo.Cambio,
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

        public ActionResult Checklist(int id)
        {
            using (var entity = new Entity())
            {
                var ordemServico = entity.GetById<OrdemServico>(id);
                if (ordemServico == null || ordemServico.Id == 0)
                {
                    return RedirectToAction("Index");
                }

                var checklist = entity.All<ChecklistVeiculo>().FirstOrDefault(x => x.IdOrdemServico == id);
                
                ChecklistVeiculoModel model;
                if (checklist == null)
                {
                    // Criar novo checklist com itens padrão
                    model = new ChecklistVeiculoModel
                    {
                        IdOrdemServico = id,
                        Itens = CriarItensChecklistPadrao()
                    };
                }
                else
                {
                    model = Mapper.Map<ChecklistVeiculoModel>(checklist);
                }

                ViewBag.OrdemServico = ordemServico;
                return View(model);
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Checklist(ChecklistVeiculoModel model)
        {
            using (var entity = new Entity())
            {
                try
                {
                    entity.BeginTransaction();

                    var checklistExistente = entity.All<ChecklistVeiculo>().FirstOrDefault(x => x.IdOrdemServico == model.IdOrdemServico);
                    
                    if (checklistExistente == null)
                    {
                        // Criar novo checklist
                        var novoChecklist = new ChecklistVeiculo
                        {
                            IdOrdemServico = model.IdOrdemServico,
                            Observacoes = model.Observacoes ?? "",
                            DataCriacao = DateTime.Now
                        };
                        entity.Add(novoChecklist);

                        // Adicionar itens
                        foreach (var item in model.Itens)
                        {
                            var novoItem = new ChecklistItem
                            {
                                IdChecklistVeiculo = novoChecklist.Id,
                                Sistema = item.Sistema ?? "",
                                Item = item.Item ?? "",
                                Verificado = item.Verificado,
                                Observacao = item.Observacao ?? "",
                                DataCriacao = DateTime.Now
                            };
                            entity.Add(novoItem);
                        }
                    }
                    else
                    {
                        // Atualizar checklist existente
                        checklistExistente.Observacoes = model.Observacoes ?? "";
                        entity.SaveOrUpdate(checklistExistente);

                        // Remover itens existentes e adicionar novos
                        var itensExistentes = entity.All<ChecklistItem>().Where(x => x.IdChecklistVeiculo == checklistExistente.Id).ToList();
                        foreach (var item in itensExistentes)
                        {
                            entity.Delete(item);
                        }

                        foreach (var item in model.Itens)
                        {
                            var novoItem = new ChecklistItem
                            {
                                IdChecklistVeiculo = checklistExistente.Id,
                                Sistema = item.Sistema ?? "",
                                Item = item.Item ?? "",
                                Verificado = item.Verificado,
                                Observacao = item.Observacao ?? "",
                                DataCriacao = DateTime.Now
                            };
                            entity.Add(novoItem);
                        }
                    }

                    entity.Commit();
                    return Json(new { IsValid = true, Message = "Checklist salvo com sucesso!" }, JsonRequestBehavior.AllowGet);
                }
                catch (Exception ex)
                {
                    entity.Rollback();
                    return Json(new { IsValid = false, Message = "Erro ao salvar checklist." }, JsonRequestBehavior.AllowGet);
                }
            }
        }

        private List<ChecklistItemModel> CriarItensChecklistPadrao()
        {
            return new List<ChecklistItemModel>
            {
                // Troca de óleo
                new ChecklistItemModel { Sistema = "Troca de Óleo", Item = "Anti-chama" },
                new ChecklistItemModel { Sistema = "Troca de Óleo", Item = "Cartão de óleo" },
                new ChecklistItemModel { Sistema = "Troca de Óleo", Item = "Filtro de ar" },
                new ChecklistItemModel { Sistema = "Troca de Óleo", Item = "Filtro de ar-condicionado" },
                new ChecklistItemModel { Sistema = "Troca de Óleo", Item = "Filtro de combustível" },
                new ChecklistItemModel { Sistema = "Troca de Óleo", Item = "Filtro de óleo do motor" },
                new ChecklistItemModel { Sistema = "Troca de Óleo", Item = "Óleo do motor" },

                // Sistema de freios
                new ChecklistItemModel { Sistema = "Sistema de Freios", Item = "Cilindro" },
                new ChecklistItemModel { Sistema = "Sistema de Freios", Item = "Disco dianteiro/traseiro" },
                new ChecklistItemModel { Sistema = "Sistema de Freios", Item = "Cilindro de roda traseira" },
                new ChecklistItemModel { Sistema = "Sistema de Freios", Item = "Flexível de freio dianteiro/traseiro" },
                new ChecklistItemModel { Sistema = "Sistema de Freios", Item = "Fluido de freio" },
                new ChecklistItemModel { Sistema = "Sistema de Freios", Item = "Freio de estacionamento (cabo e alavanca)" },
                new ChecklistItemModel { Sistema = "Sistema de Freios", Item = "Lona de freio" },
                new ChecklistItemModel { Sistema = "Sistema de Freios", Item = "Pastilha de freio dianteira/traseira" },
                new ChecklistItemModel { Sistema = "Sistema de Freios", Item = "Sapatas" },

                // Sistema de direção e suspensão
                new ChecklistItemModel { Sistema = "Sistema de Direção e Suspensão", Item = "Amortecedores dianteiros/traseiros" },
                new ChecklistItemModel { Sistema = "Sistema de Direção e Suspensão", Item = "Articulações de direção" },
                new ChecklistItemModel { Sistema = "Sistema de Direção e Suspensão", Item = "Balanceamento e geometria" },
                new ChecklistItemModel { Sistema = "Sistema de Direção e Suspensão", Item = "Barra de direção" },
                new ChecklistItemModel { Sistema = "Sistema de Direção e Suspensão", Item = "Buchas de suspensão" },
                new ChecklistItemModel { Sistema = "Sistema de Direção e Suspensão", Item = "Caixa de direção" },
                new ChecklistItemModel { Sistema = "Sistema de Direção e Suspensão", Item = "Coifas sem eixos" },
                new ChecklistItemModel { Sistema = "Sistema de Direção e Suspensão", Item = "Coluna da direção" },
                new ChecklistItemModel { Sistema = "Sistema de Direção e Suspensão", Item = "Coxins da caixa" },
                new ChecklistItemModel { Sistema = "Sistema de Direção e Suspensão", Item = "Coxins do motor" },
                new ChecklistItemModel { Sistema = "Sistema de Direção e Suspensão", Item = "Cubos roda dianteiro/traseiro" },
                new ChecklistItemModel { Sistema = "Sistema de Direção e Suspensão", Item = "Juntas homocinéticas" },
                new ChecklistItemModel { Sistema = "Sistema de Direção e Suspensão", Item = "Molas dianteiras/traseiras" },
                new ChecklistItemModel { Sistema = "Sistema de Direção e Suspensão", Item = "Pivôs" },
                new ChecklistItemModel { Sistema = "Sistema de Direção e Suspensão", Item = "Terminais de direção" },

                // Sistema de injeção
                new ChecklistItemModel { Sistema = "Sistema de Injeção", Item = "Análise de gases (HC: 140±2 / CO: 14± CO2)" },
                new ChecklistItemModel { Sistema = "Sistema de Injeção", Item = "Bicos injetores" },
                new ChecklistItemModel { Sistema = "Sistema de Injeção", Item = "Bomba de combustível" },
                new ChecklistItemModel { Sistema = "Sistema de Injeção", Item = "Cabo acelerador" },
                new ChecklistItemModel { Sistema = "Sistema de Injeção", Item = "Carburador / corpo borboleta (TBI)" },
                new ChecklistItemModel { Sistema = "Sistema de Injeção", Item = "Limpeza do sistema de injeção" },
                new ChecklistItemModel { Sistema = "Sistema de Injeção", Item = "Pré-filtro da bomba de combustível" },
                new ChecklistItemModel { Sistema = "Sistema de Injeção", Item = "Sensores (rotação / temperatura / posição)" },

                // Sistema de ignição
                new ChecklistItemModel { Sistema = "Sistema de Ignição", Item = "Bobinas" },
                new ChecklistItemModel { Sistema = "Sistema de Ignição", Item = "Cabos de vela" },
                new ChecklistItemModel { Sistema = "Sistema de Ignição", Item = "Distribuidor" },
                new ChecklistItemModel { Sistema = "Sistema de Ignição", Item = "Rotor" },
                new ChecklistItemModel { Sistema = "Sistema de Ignição", Item = "Tampa do distribuidor" },
                new ChecklistItemModel { Sistema = "Sistema de Ignição", Item = "Velas" },

                // Sistema de arrefecimento
                new ChecklistItemModel { Sistema = "Sistema de Arrefecimento", Item = "Bomba d'água / mangueira / tubulação / sensores" },
                new ChecklistItemModel { Sistema = "Sistema de Arrefecimento", Item = "Radiador" },
                new ChecklistItemModel { Sistema = "Sistema de Arrefecimento", Item = "Reservatório da água do radiador" },
                new ChecklistItemModel { Sistema = "Sistema de Arrefecimento", Item = "Tampa do reservatório" },
                new ChecklistItemModel { Sistema = "Sistema de Arrefecimento", Item = "Válvula termostática" },

                // Sistema elétrico
                new ChecklistItemModel { Sistema = "Sistema Elétrico", Item = "Ar-condicionado e ventilação" },
                new ChecklistItemModel { Sistema = "Sistema Elétrico", Item = "Bateria / regulador de voltagem" },
                new ChecklistItemModel { Sistema = "Sistema Elétrico", Item = "Bomba de combustível" },
                new ChecklistItemModel { Sistema = "Sistema Elétrico", Item = "Buzina" },
                new ChecklistItemModel { Sistema = "Sistema Elétrico", Item = "Cabo do acelerador" },
                new ChecklistItemModel { Sistema = "Sistema Elétrico", Item = "Cabos e terminais" },
                new ChecklistItemModel { Sistema = "Sistema Elétrico", Item = "Carburador" },
                new ChecklistItemModel { Sistema = "Sistema Elétrico", Item = "Lâmpadas (teto / portas / porta-malas / porta-luvas)" },
                new ChecklistItemModel { Sistema = "Sistema Elétrico", Item = "Lâmpadas (farol e sinaleira)" },
                new ChecklistItemModel { Sistema = "Sistema Elétrico", Item = "Limpeza do sistema de injeção" },
                new ChecklistItemModel { Sistema = "Sistema Elétrico", Item = "Motor de partida" },
                new ChecklistItemModel { Sistema = "Sistema Elétrico", Item = "Pré-filtro da bomba de combustível" },

                // Sistema de câmbio/embreagem
                new ChecklistItemModel { Sistema = "Sistema de Câmbio/Embreagem", Item = "Cabo de embreagem" },
                new ChecklistItemModel { Sistema = "Sistema de Câmbio/Embreagem", Item = "Cilindros / atuador" },
                new ChecklistItemModel { Sistema = "Sistema de Câmbio/Embreagem", Item = "Funcionamento das alavancas" },
                new ChecklistItemModel { Sistema = "Sistema de Câmbio/Embreagem", Item = "Kit embreagem" },
                new ChecklistItemModel { Sistema = "Sistema de Câmbio/Embreagem", Item = "Óleo do câmbio" },
                new ChecklistItemModel { Sistema = "Sistema de Câmbio/Embreagem", Item = "Retentor do volante e da prise" },

                // Correias
                new ChecklistItemModel { Sistema = "Correias", Item = "Auxiliares" },
                new ChecklistItemModel { Sistema = "Correias", Item = "Dentada" },
                new ChecklistItemModel { Sistema = "Correias", Item = "Retentores" },
                new ChecklistItemModel { Sistema = "Correias", Item = "Rolamentos tensores e guias" },

                // Vedação do motor
                new ChecklistItemModel { Sistema = "Vedação do Motor", Item = "Junta da tampa de válvulas" },
                new ChecklistItemModel { Sistema = "Vedação do Motor", Item = "Junta do cárter" },
                new ChecklistItemModel { Sistema = "Vedação do Motor", Item = "Retentores" },

                // Carroceria
                new ChecklistItemModel { Sistema = "Carroceria", Item = "Água / aditivo limpador de para-brisa" },
                new ChecklistItemModel { Sistema = "Carroceria", Item = "Esguicho de água do para-brisa" },
                new ChecklistItemModel { Sistema = "Carroceria", Item = "Estepe / chave de roda / macaco / triângulo" },
                new ChecklistItemModel { Sistema = "Carroceria", Item = "Extintor de incêndio (validade)" },
                new ChecklistItemModel { Sistema = "Carroceria", Item = "Palheta limpador" },
                new ChecklistItemModel { Sistema = "Carroceria", Item = "Portas (regulagens e lubrificação)" },

                // Sistema de exaustão
                new ChecklistItemModel { Sistema = "Sistema de Exaustão", Item = "Abraçadeiras / coxins / juntas" },
                new ChecklistItemModel { Sistema = "Sistema de Exaustão", Item = "Escapamento" }
            };
        }

        public ActionResult PrinterChecklist(int id, bool gerarPDF = false, bool vazio = false)
        {
            using (var entity = new Entity())
            {
                var ordemServico = entity.GetById<OrdemServico>(id);
                if (ordemServico == null)
                {
                    return RedirectToAction("Index");
                }

                ChecklistVeiculoModel checklist;
                if (vazio)
                {
                    checklist = new ChecklistVeiculoModel
                    {
                        IdOrdemServico = id,
                        Itens = CriarItensChecklistPadrao()
                    };
                }
                else
                {
                    var checklistDB = entity.All<ChecklistVeiculo>().FirstOrDefault(x => x.IdOrdemServico == id);
                    if (checklistDB != null)
                    {
                        checklist = Mapper.Map<ChecklistVeiculoModel>(checklistDB);
                        // Garantir que todos os itens padrão estejam presentes
                        var itensExistentes = checklist.Itens.ToList();
                        var itensPadrao = CriarItensChecklistPadrao();
                        
                        foreach (var itemPadrao in itensPadrao)
                        {
                            if (!itensExistentes.Any(x => x.Sistema == itemPadrao.Sistema && x.Item == itemPadrao.Item))
                            {
                                itensExistentes.Add(itemPadrao);
                            }
                        }
                        checklist.Itens = itensExistentes;
                    }
                    else
                    {
                        checklist = new ChecklistVeiculoModel
                        {
                            IdOrdemServico = id,
                            Itens = CriarItensChecklistPadrao()
                        };
                    }
                }

                ViewBag.OrdemServico = ordemServico;
                ViewBag.Vazio = vazio;
                ViewBag.Usuario = entity.All<Usuario>().Where(x => x.Nome == User.Identity.Name).FirstOrDefault();

                if (gerarPDF)
                    return new ViewAsPdf()
                    {
                        FileName = $"Checklist_OS_{id}.pdf",
                        ViewName = "PrinterChecklist",
                        PageSize = Size.A4,
                        Model = checklist
                    };
                else
                    return View(checklist);
            }
        }

        #endregion
    }
}