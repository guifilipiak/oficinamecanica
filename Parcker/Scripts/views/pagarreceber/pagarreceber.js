$(document).ready(function () {
    if ($('#dataTable').length > 0) {
        $('#dataTable').DataTable({
            processing: true,
            serverSide: true,
            searching: true,
            //order: [[0, "desc"]],
            ajax: {
                url: URLBase + 'PagarReceber/RetornaLista',
                type: 'POST',
                dataSrc: 'data',
                data: function (d) {
                    d["object"] = {
                        IdTipoConta: $("#IdTipoConta").val() || 0,
                        IdFormaPagamento: $("#IdFormaPagamento").val() || 0,
                        DataPagamento: $("#DataPagamento").val()
                    };
                    return d;
                }
            },
            drawCallback: function () {
                $("[data-toggle=tooltip]").tooltip();
            },
            columns: [
                //{
                //    title: 'N°',
                //    data: 'Codigo'
                //},
                //{
                //    title: 'Criado',
                //    data: 'DataCriacao',
                //    render: function (data, type, row) {
                //        return JSONDateTime(data);
                //    }
                //},
                {
                    title: 'Tipo',
                    data: 'IdTipoConta',
                    width: "30px",
                    className: "text-center",
                    render: function (data, type, row) {
                        if (data == 1)
                            return '<img title="Despesa" data-toggle="tooltip" src="' + URLBase + "Content/img/loss.png" + '" width="30px" />';
                        else
                            return '<img title="Receita" data-toggle="tooltip" src="' + URLBase + "Content/img/revenue.png" + '" width="30px" />';
                    }
                },
                { title: 'Classificação', data: 'Classificacao' },
                { title: 'Obs', data: 'Descricao' },
                { title: 'Pagamento', data: 'DataPagamento' },
                {
                    title: 'Vencimento',
                    data: 'DataVencimento',
                    render: function (data, type, row) {
                        var dias = parseInt(row.DiasVencimento) * (-1);
                        if (dias > 0 && row.IdSituacaoConta == 1)
                            return '<span class="text-danger" title="Vencida">' + row.DataVencimento + ' - ' + dias + 'd</span>';
                        else
                            return row.DataVencimento;
                    }
                },
                { title: 'Cobrador/Pagante', data: 'Pessoa' },
                { title: 'Parc.', data: 'GrupoParcelaTexto' },
                { title: 'Form.Pag', data: 'FormasPagamento' },
                { title: 'Valor', data: 'Valor' },
                //{ title: 'Situação', data: 'SituacaoConta' },
                {
                    title: '',
                    data: 'Id',
                    width: "225px",
                    render: function (data, type, row) {
                        var buttons = "";
                        var situacaoText = row.IdTipoConta == 1 ? "Paga" : "Rece";
                        var situacaoTitleText = row.IdTipoConta == 1 ? "Confirmar Pagamento" : "Confirmar Recebimento";
                        var situacaoTitleText2 = row.IdTipoConta == 1 ? "Conta Paga" : "Pagamento Recebido";

                        if (row.IdSituacaoConta == 1) {
                            //Pendente
                            buttons += '<button class="btn btn-link" data-toggle="tooltip" title="' + situacaoTitleText + '" onclick="alterarSituacaoConta(' + data + ',' + 2 + ')"><i class="fa fa-check"></i>&nbsp;' + situacaoText + '</button>';
                            buttons += '<button class="btn btn-link" data-toggle="tooltip" title="Inativar Conta" onclick="alterarSituacaoConta(' + data + ',' + 3 + ')"><i class="fa fa-ban"></i>&nbsp;Inativa</button>';
                        } else if (row.IdSituacaoConta == 2) {
                            //Paga
                            buttons += '<button class="btn btn-link text-success" disabled data-toggle="tooltip" title="' + situacaoTitleText2 + '"><i class="fa fa-check"></i>&nbsp;' + situacaoText + '</button>';
                            buttons += '<button class="btn btn-link" disabled><i class="fa fa-ban"></i>&nbsp;Inativa</button>';
                        } else {
                            //Inativa
                            buttons += '<button class="btn btn-link" data-toggle="tooltip" title="Reativar Conta" onclick="alterarSituacaoConta(' + data + ',' + 1 + ')"><i class="fa fa-undo"></i>&nbsp;Ativa</button>';
                            buttons += '<button class="btn btn-link text-danger" disabled data-toggle="tooltip" title="Conta Inativa"><i class="fa fa-ban"></i>&nbsp;Inativa</button>';
                        }

                        buttons += "<a href='" + URLBase + "PagarReceber/Edit/" + data + "' class='btn btn-link'><i class='fa fa-eye'></i>&nbsp;Ver</a>";
                        buttons += '<button class="btn btn-link" onclick="confirmDelete(\'PagarReceber/Delete\',' + data + ', function () { $(\'#dataTable\').DataTable().ajax.reload(); })"><i class="fa fa-trash"></i>&nbsp;Remover</button>';
                        return buttons;
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

        $("#formFilter").submit(function (e) {
            e.preventDefault();
            filterTable();
            return false;
        });
    } else {

        changeTipoConta($("#IdTipoConta").val());
        changeFormaPagamento($("#IdFormaPagamento").val());

        $('.selectpicker-pessoas').selectpicker().ajaxSelectPicker({
            ajax: {
                url: URLBase + "Veiculo/ListarPessoas",
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
                        }));
                    }
                }
                return array;
            },
            noneSelectedText: 'Selecione...'
        });

        $("#IdTipoConta").change(function () {
            $("#IdClassificacao").val("");
            changeTipoConta($(this).val());
        });

        $("#IdFormaPagamento").change(function () {
            $("#TotalParcela").val(1);
            changeFormaPagamento($(this).val());
        });
    }
});

function filterTable() {
    $('#dataTable').DataTable().ajax.reload();
}

function changeTipoConta(value) {
    $("#IdClassificacao").find("option").not("option[value=\"\"]").prop("disabled", true);
    switch (value) {
        default:
        case "1":
            //pagar
            $("#imgTipoConta").prop("src", $("#imgTipoConta").data("pagar"));
            $("#DataVencimento").prop("disabled", false);
            $("label[for=IdPessoa]").text("Cobrador");
            $("#IdClassificacao").find("option[value=1],option[value=2],option[value=3],option[value=4],option[value=5],option[value=6],option[value=7],option[value=10]").prop("disabled", false);
            break;
        case "2":
            //receber
            $("#imgTipoConta").prop("src", $("#imgTipoConta").data("receber"));
            $("#DataVencimento").val("");
            $("#DataVencimento").prop("disabled", true);
            $("label[for=IdPessoa]").text("Pagador");
            $("#IdClassificacao").find("option[value=8],option[value=9]").prop("disabled", false);
            break;
    }

    if ($("#Id").val() != "0") {
        $("#DataVencimento").prop("disabled", true);
    }
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

function callback(type, result) {
    switch (type) {
        case "success":
            if (result.IsValid) {
                disableForm();
            }
            break;
        default:
    }
}

function disableForm() {
    $("input,textarea,select").prop("disabled", true);
    $('[data_live_search]').selectpicker('refresh');
    $("#btnSubmit").remove();
}

function alterarSituacaoConta(id, idSituacaoConta) {
    Swal.fire({
        title: 'Você tem certeza disso?',
        text: 'Não será possível reverter esta ação!',
        type: 'warning',
        showCancelButton: true,
        confirmButtonText: 'Sim!',
        cancelButtonText: 'Não, quero cancelar!'
    }).then((result) => {
        if (result.value) {
            $.ajax({
                url: URLBase + "PagarReceber/AlterarSituacaoConta",
                type: "POST",
                data: { id, idSituacaoConta },
                success: function (result) {
                    if (result.IsValid) {
                        Swal.fire(
                            'OK',
                            result.Message,
                            'success'
                        );
                    } else {
                        Swal.fire(
                            'Atenção',
                            result.Message,
                            'warning'
                        );
                    }

                    $("#dataTable").DataTable().draw();
                },
                error: function (xhr, ajaxOptions, thrownError) {
                    Swal.fire(
                        'Vish',
                        'Algo de errado aconteceu, tente novamente ou entre em contato com o administrador do site!',
                        'error'
                    );
                }
            });
        } else if (result.dismiss === Swal.DismissReason.cancel) {
            //cancelado
        }
    });
}