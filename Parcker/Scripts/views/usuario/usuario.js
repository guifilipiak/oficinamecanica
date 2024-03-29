﻿$(document).ready(function () {
    var _tempData;
    var _tempCpfCnpj;

    $(".tipo-pessoa").change(function () {
        switch ($(this).val()) {
            case "1":
                $("#fisica").show();
                $("#juridica").hide();
                $("#juridica").find("input, select, textarea").val("");
                break;
            case "2":
                $("#juridica").show();
                $("#fisica").hide();
                $("#fisica").find("input, select, textarea").val("");
                break;
            default:
        }
    });

    $("#Pessoa_CPF, #Pessoa_CNPJ").change(function () {
        if ($(this).val() != "" && $(this).val() != "" && $(this).val() != _tempCpfCnpj) {
            _tempCpfCnpj = $(this).val();
            $.ajax({
                url: URLBase + "Cliente/RetornarDadosPessoaPorCpfCnpj",
                data: {
                    cpfCnpj: $(this).val()
                },
                cache: false,
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
                                var form = $("#Pessoa_CPF, #Pessoa_CNPJ").parents("form");
                                $("#juridica, #fisica").find("input, select, textarea").val("");
                                destroyMask(form[0]);
                                form.find("#Pessoa_Tipo").val(_tempData.Tipo);
                                form.find("#Pessoa_Tipo").change();
                                form.find("#Pessoa_DataCriacao").val(JSONDateTime(_tempData.DataCriacao));
                                form.find("#Pessoa_Pais").val(_tempData.Pais);
                                form.find("#Pessoa_Id").val(_tempData.Id);
                                form.find("#IdPessoa").val(_tempData.Id);
                                if (result.Data.Tipo == 1) {
                                    form.find("#Pessoa_Nome").val(_tempData.Nome);
                                    form.find("#Pessoa_RG").val(_tempData.RG);
                                    form.find("#Pessoa_CPF").val(_tempData.CPF);
                                } else if (result.Data.Tipo == 2) {
                                    form.find("#Pessoa_RazaoSocial").val(_tempData.RazaoSocial);
                                    form.find("#Pessoa_CNPJ").val(_tempData.CNPJ);
                                    form.find("#Pessoa_Fantasia").val(_tempData.Fantasia);
                                }
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
                                initMask(form[0]);
                            } else if (result.dismiss === Swal.DismissReason.cancel) {
                                //cancelado
                                $("#CPF, #CNPJ").val("");
                            }
                        });
                    }
                }
            });
        }
    });

    $(".tipo-pessoa").change();
});

callback = function (type, result) {
    switch (type) {
        case "success":
            if (result && result.IsValid) {
                $("#IdPessoa").val(result.Data.IdPessoa);
                $("#Pessoa_Id").val(result.Data.IdPessoa);
            }
            break;
        default:
    }
};