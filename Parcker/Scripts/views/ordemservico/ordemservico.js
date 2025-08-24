
$(document).ready(function () {
    if ($('#dataTable').length > 0) {
        $('#dataTable').DataTable({
            processing: true,
            serverSide: true,
            order: [[0, "desc"]],
            ajax: {
                url: URLBase + 'OrdemServico/RetornaListaOrdensServico',
                type: 'POST',
                dataSrc: 'data'
            },
            columns: [
                {
                    title: 'N°',
                    data: 'Codigo'
                },
                {
                    title: 'Criado',
                    data: 'DataCriacao',
                    render: function (data, type, row) {
                        return JSONDateTime(data);
                    }
                },
                { title: 'Descrição', data: 'DescricaoServico' },
                { title: 'Cliente', data: 'Cliente' },
                { title: 'Veiculo', data: 'Veiculo' },
                { title: 'Tipo', data: 'TipoAtendimento', width: "80px" },
                { title: 'Situação', data: 'SituacaoServico' },
                { title: 'Total', data: 'Total' },
                {
                    title: 'Pagamento',
                    data: 'Pagamento',
                    width: "100px",
                    className: "text-center",
                    render: function (data, type, row) {
                        switch (data) {
                            default:
                            case "EM PROCESSO":
                                return '<span class="badge badge-secondary">EM PROCESSO</span>';
                            case "PAGO":
                                return '<span class="badge badge-success">PAGO</span>';
                            case "PENDENTE":
                                return '<span class="badge badge-warning">PENDENTE</span>';
                            case "CANCELADO":
                                return '<span class="badge badge-danger">CANCELADO</span>';
                        }
                    }
                },
                {
                    title: '',
                    data: 'Id',
                    width: "80px",
                    className: "text-center",
                    render: function (data, type, row) {
                        var dropdown = '<div class="dropdown">' +
                            '<button class="btn btn-sm btn-outline-secondary dropdown-toggle" type="button" data-toggle="dropdown">Ações</button>' +
                            '<div class="dropdown-menu">' +
                            '<a class="dropdown-item" href="' + URLBase + 'OrdemServico/Printer/' + data + '?gerarPDF=False" target="_blank"><i class="fa fa-print"></i> Imprimir</a>' +
                            '<a class="dropdown-item" href="' + URLBase + 'OrdemServico/Edit/' + data + '"><i class="fa fa-edit"></i> Editar</a>';
                        
                        if (row.Pagamento == "PENDENTE") {
                            dropdown += '<a class="dropdown-item" href="' + URLBase + 'OrdemServico/Edit/' + data + '"><i class="fa fa-money-bill"></i> Informar Pagamento</a>';
                        }
                        
                        dropdown += '<div class="dropdown-divider"></div>' +
                            '<a class="dropdown-item text-danger" href="#" onclick="confirmDelete(\'OrdemServico/Delete\',' + data + ', function () { $(\'#dataTable\').DataTable().ajax.reload(); })"><i class="fa fa-trash"></i> Remover</a>' +
                            '</div></div>';
                        
                        return dropdown;
                    }
                }
            ],
            language: {
                "lengthMenu": "Exibindo _MENU_ registro(s) por P&aacute;gina",
                "zeroRecords": "Nenhum registro encontrado.",
                "info": "Mostrando P&aacute;gina _PAGE_ de _PAGES_",
                "infoEmpty": "Sem dados",
                "infoFiltered": "(Encontrado _MAX_ total de registros)",
                "search": "Procurar",
                "paginate": {
                    "next": "Pr&oacute;ximo",
                    "previous": "Anterior",
                    "first": "Primeiro",
                    "last": "&Uacute;ltimo"
                },
                "processing": "Carregando..."
            }
        });
    } else {
        if ($("#IdSituacaoServico").val() == "5" && $("#PagamentoRecebido").val() == "False") {
            $("#IdOrdemServico").val($("#Id").val());
            // Calcular saldo (Total - Entrada) para o valor padrão
            var total = decimalFormat($("#Total").val());
            var entrada = decimalFormat($("#Entrada").val());
            var saldoOS = total - entrada;
            $("#Valor").val(decimalFormat(saldoOS, "pt-BR"));
            $("#modalPagamento").modal("show");

            // Calcular saldo inicial quando a modal abrir
            $("#modalPagamento").on('shown.bs.modal', function () {
                calculaSaldo();
            });

            $("#formItemPagamento").submit(function (e) {
                e.preventDefault();

                if (!$("#formItemPagamento").data("submited")) {
                    var obj = {
                        dataPagamento: $("#DataPagamento").val(),
                        idFormaPagamento: $("#IdFormaPagamento").val(),
                        textFormaPagamento: $("#IdFormaPagamento").find("option:selected").text(),
                        valor: $("#Valor").val(),
                        totalParcela: $("#TotalParcela").val()
                    };

                    adicionaItemPagamento(e, obj);
                }

                $("#formItemPagamento").data("submited", true);
            });
        }
    }

    if ($('input[name="search"]').length > 0) {
        $('input[name="search"]').autoComplete({
            source: function (query, response) {
                $.getJSON(URLBase + "OrdemServico/ListarItens", { query: query }, function (data) { response(data); });
            },
            minChars: 1,
            noCache: true,
            type: "GET",
            renderItem: function (item, search) {
                window["selected_item_" + item.Tipo + "_" + item.Id] = JSON.stringify(item);
                return '<div class="autocomplete-suggestion" data-tipo="' + item.Tipo + '" data-id="' + item.Id + '" data-val="' + search + '">' + item.DescricaoCompleta + '</div>';
            },
            onSelect: function (e, term, item) {
                var jsonItem = JSON.parse(window["selected_item_" + $(item).data("tipo") + "_" + $(item).data("id")]);
                inserirNovoItem(jsonItem.Id, jsonItem.Tipo, jsonItem.Descricao, jsonItem.ValorUnitario);
                $('input[name="search"]').val("").focus();
            },
        });
    }

    $(window).keydown(function (event) {
        if (event.keyCode == 13) {
            event.preventDefault();
            return false;
        }
    });

    $("#IdSituacaoServico").change(function () {
        validarTrocaSituacao(this);
    });

    $("#Entrada").change(function () {
        calcularTotais();
    });

    if ($('.selectpicker-veiculos').length > 0) {
        $('.selectpicker-veiculos').selectpicker().ajaxSelectPicker({
            ajax: {
                url: URLBase + "OrdemServico/ListarVeiculos",
                type: 'GET',
                cache: false,
                data: {
                    query: '{{{q}}}'
                }
            },
            preprocessData: function (data) {
                var i, l = data.length, array = [];
                if (l) {
                    for (i = 0; i < l; i++) {
                        array.push($.extend(true, data[i], {
                            text: data[i].DescricaoCompleta,
                            value: data[i].Id
                            //data: {
                            //    subtext: data[i].Email
                            //}
                        }));
                    }
                }
                return array;
            },
            noneSelectedText: 'Selecione...'
        });

        $('#IdVeiculo').change(function () {
            var value = $(this).val();
            if (value) {
                $.ajax({
                    url: URLBase + "OrdemServico/ObterVeiculo",
                    cache: false,
                    data: {
                        id: value
                    },
                    success: function (result) {
                        if (result) {
                            $("#KM").val(result.KM);
                        }
                    }
                });
            } else
                $("#KM").val("0");
        });
    }

    if ($('.selectpicker-clientes').length > 0) {
        $('.selectpicker-clientes').selectpicker().ajaxSelectPicker({
            ajax: {
                url: URLBase + "OrdemServico/ListarClientes",
                type: 'GET',
                cache: false,
                data: {
                    query: '{{{q}}}'
                }
            },
            preprocessData: function (data) {
                var i, l = data.length, array = [];
                if (l) {
                    for (i = 0; i < l; i++) {
                        array.push($.extend(true, data[i], {
                            text: data[i].DescricaoCompleta,
                            value: data[i].Id
                            //data: {
                            //    subtext: data[i].Email
                            //}
                        }));
                    }
                }
                return array;
            },
            noneSelectedText: 'Selecione...'
        });
    }

    changeFormaPagamento($("IdFormaPagamento").val());

    $("#IdFormaPagamento").change(function () {
        $("#TotalParcela").val(1);
        changeFormaPagamento($(this).val());
    });
});

function calculaSaldo() {
    var total = decimalFormat($("#Total").val());
    var entrada = decimalFormat($("#Entrada").val());
    var saldoOS = total - entrada;
    var totalLinhas = 0;
    var saldo = 0;
    var rows = $("#tableItemPagamento").find("tbody tr");
    var hasItems = false;

    if (rows.find("td[data-index=0]").length == 0) {
        hasItems = true;
        for (var i = 0; i < rows.length; i++) {
            totalLinhas += decimalFormat($(rows[i]).find("#Valor").val());
        }
    }

    saldo = saldoOS - totalLinhas;

    if (saldo <= saldoOS && saldo != 0) {
        $("#saldoPagamento").removeClass("text-success");
        $("#saldoPagamento").addClass("text-danger");
        $("#saldoPagamento").parent().removeClass("text-success");
        $("#saldoPagamento").parent().addClass("text-danger");
    }
    else {
        $("#saldoPagamento").removeClass("text-danger");
        $("#saldoPagamento").addClass("text-success");
        $("#saldoPagamento").parent().removeClass("text-danger");
        $("#saldoPagamento").parent().addClass("text-success");
    }

    $("#saldoPagamento").html(decimalFormat(saldo, 'pt-BR'));

    if (hasItems && saldo == 0) {
        $("#btnSubmitModal").prop("disabled", false);
        $("#btnAddFormaPagamento").prop("disabled", true);
    }
    else {
        $("#btnSubmitModal").prop("disabled", true);
        $("#btnAddFormaPagamento").prop("disabled", false);
    }
}

function adicionaItemPagamento(form, obj) {

    setTimeout(function () {
        $("#formItemPagamento").data("submited", false);
    }, 200);

    if (!$("#formItemPagamento").valid())
        return;

    //VALIDA SE EXISTE OUTRA FORMA DE PAGAMENTO (desabilitado por opcao do cliente)
    //if (existeItemFormaPagamento(obj.idFormaPagamento)) {
    //    message.warning("Atenção", "Você já informou esta forma de pagamento!");
    //    return;
    //}

    if ($("#tableItemPagamento").find("tbody tr > td").data("index") == 0) {
        $("#tableItemPagamento").find("tbody").html("");
    }

    var row = "<tr>" +
        "<td><input id=\"DataPagamento\" name=\"DataPagamento\" type=\"hidden\" value=\"" + obj.dataPagamento + "\" />" + obj.dataPagamento + "</td>" +
        "<td><input id=\"IdFormaPagamento\" name=\"IdFormaPagamento\" type=\"hidden\" value=\"" + obj.idFormaPagamento + "\" />" + obj.textFormaPagamento + (obj.idFormaPagamento == 3 ? ("   (" + obj.totalParcela + "x)") : "") + "</td>" +
        "<td class=\"text-right\"><input id=\"TotalParcela\" name=\"TotalParcela\" type=\"hidden\" value=\"" + obj.totalParcela + "\" /><input id=\"Valor\" name=\"Valor\" type=\"hidden\" value=\"" + obj.valor + "\" />" + obj.valor + "</td>" +
        "<td class=\"text-right\"><button type=\"button\" title=\"Remover\" class=\"btn btn-sm btn-danger\" onclick=\"removerItemPagamento(this)\"><i class=\"fa fa-trash-alt\"></i></button></td>" +
        "</tr>";
    $("#tableItemPagamento").find("tbody").append(row);

    calculaSaldo();
}

function existeItemFormaPagamento(idFormaPagamento) {
    return $("#tableItemPagamento").find("tbody").find("tr td input[id=IdFormaPagamento][value=" + idFormaPagamento + "]").length > 0;
}

function removerItemPagamento(e) {
    $(e).parents("tr").remove();

    if ($("#tableItemPagamento").find("tbody tr").length == 0) {
        $("#tableItemPagamento").find("tbody").html('<tr><td data-index="0" colspan="4" class="text-center">Nenhuma forma de pagamento informada</td></tr>');
    }

    calculaSaldo();
}

function changeFormaPagamento(value) {
    switch (value) {
        default:
            $("#divTotalParcela").find("#TotalParcela").val(1);
            $("#divTotalParcela").find("#TotalParcela").prop("readonly", true);
            break;
        case "3":
            $("#divTotalParcela").find("#TotalParcela").prop("readonly", false);
            break;
    }
}

function inserirNovoItem(id, tipo, descricao, valorUnitario) {
    var indexRow = $("#tableItens").find("tbody tr").not(".empty_row").length;
    var existRowItem = $("#tableItens").find("tbody tr[data-id=\"" + id + "\"][data-tipo=\"" + tipo + "\"]");

    var htmlRow = "<tr data-index=\"" + indexRow + "\" onchange=\"calculaLinha(this)\" data-tipo=\"" + tipo + "\" data-id=\"" + id + "\">" +
        "   <td>" +
        "        <input id=\"Grid_Id\" name=\"Grid_Id\" type=\"hidden\" value=\"0\">" +
        "        <input id=\"Grid_IdOrdemServico\" name=\"Grid_IdOrdemServico\" type=\"hidden\" value=\"" + $("#Id").val() + "\">" +
        "        <input id=\"Grid_IdProduto\" name=\"Grid_IdProduto\" type=\"hidden\" value=\"" + (tipo == 1 ? id : "") + "\">" +
        "        <input id=\"Grid_IdServico\" name=\"Grid_IdServico\" type=\"hidden\" value=\"" + (tipo == 2 ? id : "") + "\">" +
        "        <input id=\"Grid_Tipo\" name=\"Grid_Tipo\" type=\"hidden\" value=\"" + tipo + "\">" +
        "        <input id=\"Grid_Index\" name=\"Grid_Index\" type=\"hidden\" value=\"" + indexRow + "\">" +
        "        <span>" + descricao + "</span>" +
        "    </td>" +
        "    <td class=\"text-right\" width=\"50\">" +
        "        <input class=\"form-control numero-menor text-right\" id=\"Grid_Quantidade\" name=\"Grid_Quantidade\" type=\"number\" value=\"1\" placeholder=\"_________\" maxlength=\"9\">" +
        "    </td>" +
        "    <td class=\"text-right\" width=\"250\">" +
        "        <input class=\"form-control decimal text-right\" id=\"Grid_ValorUnitario\" name=\"Grid_ValorUnitario\" type=\"text\" value=\"" + valorUnitario + "\" placeholder=\"_________\" maxlength=\"9\">" +
        "    </td>" +
        "    <td class=\"text-right\" width=\"250\">" +
        "        <div class=\"input-group\"><input class=\"form-control decimal text-right\" id=\"Grid_Desconto\" name=\"Grid_Desconto\" type=\"text\" value=\"0\" placeholder=\"_________\" maxlength=\"9\">" +
        "        <button type=\"button\" class=\"btn btn-sm btn-primary\" onclick=\"calcPercItem(" + indexRow + ")\"><i class=\"fa fa-percent\"></i></button></div>" +
        "    </td>" +
        "    <td class=\"text-right\" width=\"250\">" +
        "        <input id=\"Grid_ValorTotal\" name=\"Grid_ValorTotal\" type=\"hidden\" value=\"" + valorUnitario + "\">" +
        "       <span id=\"totalTexto\" class=\"decimal\">" + valorUnitario + "</span>" +
        "    </td>" +
        "    <td width=\"130\" class=\"text-center\">" +
        "        <a href=\"javascript:removerLinha(" + indexRow + ");\"><i class=\"fa fa-trash\"></i>&nbsp;Remover</a>" +
        "    </td>" +
        "</tr>";

    //verifica se possui rows
    if (indexRow == 0)
        $("#tableItens").find("tbody tr.empty_row").remove();

    //verifica se ja existe uma linha de mesmo item e add quantidade + 1
    if (existRowItem.length > 0) {
        var quant = parseInt(existRowItem.find("#Grid_Quantidade").val() || "0");
        existRowItem.find("#Grid_Quantidade").val(quant + 1);
        calculaLinha(existRowItem);
    } else {
        $("#tableItens").find("tbody").append(htmlRow);

        setTimeout(function () {
            var lastRow = $("#tableItens").find("tbody tr").last();

            initMask(lastRow[0]);
            lastRow.data("unobtrusiveValidation", null);
            lastRow.data("validator", null);
            $.validator.unobtrusive.parse(lastRow);

        }, 200);
    }

    calcularTotais();
}

var htmlRowEmpty = "<tr class=\"empty_row\">" +
    "   <td colspan=\"6\"><p class=\"p-5 text-center lead\">Não há itens nesta ordem de serviço</p></td>" +
    "</tr>";

function removerLinha(index) {
    $("#tableItens").find("tbody tr[data-index=\"" + index + "\"]").remove();

    if ($("#tableItens").find("tbody tr").length == 0)
        $("#tableItens").find("tbody").append(htmlRowEmpty);

    calcularTotais();

    $("#search").val("").focus();
}

function calcPercItem(index) {
    var linha = $("#tableItens").find("tbody tr[data-index=" + index + "]");
    Swal.fire({
        title: 'Qual a porcentagem do desconto?',
        text: "",
        type: 'info',
        html:
            '<div class="row"><div class="col-md-4 offset-4"><input type="text" class="form-control form-control-lg porcentagem text-center width-150" /></div></div>',
        onBeforeOpen: () => {
            var content = Swal.getContent();
            initMask(content);
        },
        showCancelButton: true,
        confirmButtonColor: '#3085d6',
        cancelButtonColor: '#d33',
        cancelButtonText: 'Cancelar',
        confirmButtonText: 'Calcular Agora!'
    }).then((result) => {
        if (result.value) {
            var value = $(Swal.getContent()).find("input").val();
            var perce = decimalFormat(value || "0", "en-US");
            var quant = parseInt(linha.find("#Grid_Quantidade").val() || "0");
            var valor = decimalFormat(linha.find("#Grid_ValorUnitario").val() || "0", "en-US");
            var desco = quant * valor * (perce / 100);

            linha.find("#Grid_Desconto").val(decimalFormat(desco, "pt-BR"));
            linha.find("#Grid_Desconto").change();
        }
    });
}

function calcularTotais() {
    var total = 0, desconto = 0, subtotal = 0, entrada = 0, saldo = 0;
    entrada = decimalFormat($("#Entrada").val());

    var linhas = $("#tableItens").find("tbody tr").not(".empty_row");
    for (var i = 0; i < linhas.length; i++) {
        var l_desconto = linhas.eq(i).find("#Grid_Desconto").val();
        var l_desconto_fmt = decimalFormat(l_desconto);
        var l_valorunitario = linhas.eq(i).find("#Grid_ValorUnitario").val();
        var l_quantidade = linhas.eq(i).find("#Grid_Quantidade").val();
        var l_subtotal_fmt = decimalFormat(l_quantidade) * decimalFormat(l_valorunitario);

        desconto += l_desconto_fmt;
        subtotal += l_subtotal_fmt;
        total += l_subtotal_fmt;
    }

    total = total - desconto;
    saldo = total - entrada;

    $("#SubTotal").val(decimalFormat(subtotal, "pt-BR"));
    $("#Desconto").val(decimalFormat(desconto, "pt-BR"));
    $("#Total").val(decimalFormat(total, "pt-BR"));
    $("#saldo").html("saldo: " + decimalFormat(saldo, "pt-BR"));
}

function calculaLinha(linha) {
    if (!linha.length)
        linha = $(linha);

    var quant = parseInt(linha.find("#Grid_Quantidade").val() || "0");
    var valor = decimalFormat(linha.find("#Grid_ValorUnitario").val() || "0", "en-US");
    var desco = decimalFormat(linha.find("#Grid_Desconto").val() || "0", "en-US");
    var vlrTo = quant * valor - desco;

    linha.find("#totalTexto").html(decimalFormat(vlrTo, "pt-BR"));
    linha.find("#Grid_ValorTotal").val(decimalFormat(vlrTo, "pt-BR"));

    calcularTotais();
}

function callback(type, result) {
    switch (type) {
        case "success":
            if (result.IsValid) {
                // Se é um retorno de pagamento, fechar modal
                if (result.Pagamento) {
                    $("#modalPagamento").modal("hide");
                    return;
                }

                if (result.Data != "") {
                    for (var i = 0; i < result.Data.Itens.length; i++) {
                        var rowItem = $("#tableItens").find("tbody tr[data-index=\"" + result.Data.Itens[i].Index + "\"]");
                        rowItem.find("#Grid_Id").val(result.Data.Itens[i].Id);
                        rowItem.find("#Grid_IdOrdemServico").val(result.Data.Itens[i].IdOrdemServico);
                    }

                    $("#linkPrinter").prop("href", URLBase + "OrdemServico/Printer/" + result.Data.Id + "?gerarPDF=False");

                    if (result.Data.IdSituacaoServico == 3 || result.Data.IdSituacaoServico == 5) {
                        disableForm();
                    }

                    // Só abre modal se status = 5 (Finalizado) E não tem pagamento registrado
                    if (result.Data.IdSituacaoServico == 5 && $("#PagamentoRecebido").val() == "False") {
                        $("#IdOrdemServico").val(result.Data.Id);
                        $("#Valor").val(decimalFormat(result.Data.Total, "pt-BR"));
                        $("#DataPagamento").val(JSONDatePtBr(result.Data.DataFinalizacao));
                        //$("#modalPagamento").modal("show");
                    }
                } else {
                    $("#modalPagamento").modal("hide");
                }
            }
            break;
        case "error":
            alert(result);
            break;
        default:
    }
}

function disableForm() {
    $("input,textarea,select").not("#Descricao, #DataPagamento, #IdFormaPagamento, #Valor, #TotalParcela, #IdOrdemServico").prop("disabled", true);
    $('[data_live_search]').selectpicker('refresh');
    $("#filterItens").remove();
    $(".removeListItem").remove();
    $(".calcDescListItem").remove();
    $("#btnSubmitOS").remove();
}

function validarTrocaSituacao(sender) {
    var value = $(sender).val();
    var valueFunc = $("#IdSituacaoServicoFunc").val();
    if (valueFunc != value && (value == 3 || value == 5)) {
        Swal.fire({
            title: 'Você tem certeza disso?',
            text: 'Depois de FINALIZAR ou CANCELAR, não será mais possível alterar a OS!',
            type: 'warning',
            showCancelButton: true,
            confirmButtonText: 'Sim, desejo continuar!',
            cancelButtonText: 'Não, quero cancelar!'
        }).then((result) => {
            if (result.value) {
                $("#IdSituacaoServicoFunc").val(value);
            } else if (result.dismiss === Swal.DismissReason.cancel) {
                //cancelado
                $("#IdSituacaoServico").val(valueFunc);
            } else {
                $("#IdSituacaoServico").val(valueFunc);
            }
        });
    } else
        $("#IdSituacaoServicoFunc").val(value);
}