$(document).ready(function () {
    var _tempData;
    var _tempCnpj;

    $("#Pessoa_CEP").change(function () {
        var value = $(this).val().replace("-", "");
        if (value && value.length == 8) {
            Swal.fire({
                title: 'Buscando Endereço',
                html: 'Aguarde...',
                type: 'info',
                allowOutsideClick: false,
                onBeforeOpen: () => {
                    Swal.showLoading();
                }
            });
            $.ajax({
                url: "https://viacep.com.br/ws/" + value + "/json/",
                type: "GET",
                success: function (result) {
                    $("#Pessoa_Endereco").val(result.logradouro);
                    $("#Pessoa_Complemento").val(result.complemento);
                    $("#Pessoa_Bairro").val(result.bairro);
                    $("#Pessoa_Cidade").val(result.localidade);
                    $("#Pessoa_UF").val(result.uf);
                    $("#Pessoa_Endereco").val(result.logradouro);

                    swal.close();
                },
                error: function () {
                    Swal.fire({
                        title: 'Desculpe',
                        html: 'Não conseguimos localizar este endereço.',
                        type: 'warning'
                    });
                },
                complete: function () {
                    $("#Pessoa_Numero").focus();
                }
            });
        }
    });

    $("#Pessoa_CNPJ").change(function () {
        if ($(this).val() != _tempCnpj) {
            _tempCnpj = $(this).val();
            $.ajax({
                url: URLBase + "Fornecedor/RetornarDadosPessoaPorCnpj",
                data: {
                    cnpj: $(this).val()
                },
                type: "POST",
                success: function (result) {
                    if (result.IsValid) {
                        _tempData = result.Data;
                        Swal.fire({
                            title: 'Já existe um cadastro no sistema, deseja utilizar?',
                            text: 'Os dados atuais serão substituidos.',
                            type: 'warning',
                            showCancelButton: true,
                            confirmButtonText: 'Sim, desejo utilizar!',
                            cancelButtonText: 'Não, quero informar outro!'
                        }).then((r) => {
                            if (r.value) {
                                var form = $("#Pessoa_CNPJ").parents("form");
                                $("#juridica").find("input, select, textarea").val("");
                                destroyMask(form);
                                form.find("#Pessoa_DataCriacao").val(JSONDateTime(_tempData.DataCriacao));
                                form.find("#Pessoa_Pais").val(_tempData.Pais);
                                form.find("#Pessoa_Id").val(_tempData.Id);
                                form.find("#IdPessoa").val(_tempData.Id);
                                form.find("#Pessoa_RazaoSocial").val(_tempData.RazaoSocial);
                                form.find("#Pessoa_CNPJ").val(_tempData.CNPJ);
                                form.find("#Pessoa_Fantasia").val(_tempData.Fantasia);
                                form.find("#Pessoa_CEP").val(_tempData.CEP);
                                form.find("#Pessoa_Endereco").val(_tempData.Endereco);
                                form.find("#Pessoa_Numero").val(_tempData.Numero);
                                form.find("#Pessoa_Complemento").val(_tempData.Complemento);
                                form.find("#Pessoa_Cidade").val(_tempData.Cidade);
                                form.find("#Pessoa_Bairro").val(_tempData.Bairro);
                                form.find("#Pessoa_UF").val(_tempData.UF);
                                form.find("#Pessoa_Email").val(_tempData.Email);
                                form.find("#Pessoa_Telefone1").val(_tempData.Telefone1);
                                form.find("#Pessoa_Telefone2").val(_tempData.Telefone2);
                                initMask();
                            } else if (result.dismiss === Swal.DismissReason.cancel) {
                                //cancelado
                                $("#CNPJ").val("");
                            }
                        });
                    }
                }
            });
        }
    });

    if ($('#dataTable').length > 0) {
        $('#dataTable').DataTable({
            processing: true,
            serverSide: true,
            ajax: {
                url: URLBase + 'Fornecedor/RetornaListaFornecedores',
                type: 'POST',
                dataSrc: 'data'
            },
            columns: [
                { title: 'Razão Social', data: 'RazaoSocial' },
                { title: 'CNPJ', data: 'CNPJ' },
                { title: 'Telefone', data: 'Telefone' },
                { title: 'Endereço', data: 'Endereco' },
                {
                    title: '',
                    data: 'Id',
                    width: "115px",
                    render: function (data, type, row) {
                        var buttons = "<a href='" + URLBase + "Fornecedor/Edit/" + data + "' class='btn btn-link'><i class='fa fa-edit'></i>&nbsp;Editar</a>" +
                            '<button class="btn btn-link" onclick="confirmDelete(\'Fornecedor/Delete\',' + data + ', function () { $(\'#dataTable\').DataTable().ajax.reload(); })"><i class="fa fa-trash"></i>&nbsp;Remover</button>';
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
    }

    if ($("#Id").val() != 0) {
        desabilitaCamposObrigatorios();
    }
});

function desabilitaCamposObrigatorios() {
    $("#Pessoa_CNPJ").prop("readonly", true);
    $("#Pessoa_CNPJ").unbind("change");
}

callback = function (type, result) {
    switch (type) {
        case "success":
            if (result && result.IsValid) {
                $("#IdPessoa").val(result.Data.IdPessoa);
                $("#Pessoa_Id").val(result.Data.IdPessoa);

                desabilitaCamposObrigatorios();
            }
            break;
        default:
    }
};