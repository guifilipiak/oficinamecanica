$(document).ready(function () {
    if ($('#dataTableAlerta').length > 0) {
        $('#dataTableAlerta').DataTable({
            processing: true,
            serverSide: true,
            ajax: {
                url: URLBase + 'Alerta/RetornaListaAlertas',
                type: 'POST',
                dataSrc: 'data'
            },
            columns: [
                { title: 'Descrição', data: 'Descricao' },
                { title: 'Veículo', data: 'Veiculo' },
                { title: 'Proprietário', data: 'Proprietario' },
                {
                    title: 'Previsão de Alerta',
                    data: 'DataPrevisaoAlerta',
                    render: function (data, type, row) {
                        return JSONDateTime(data);
                    }
                },
                {
                    title: 'Ativo',
                    data: 'Ativo',
                    width: "50px",
                    className: "text-center",
                    render: function (data, type, row) {
                        if (data == 1)
                            return '<img src="' + URLBase + "Content/img/bolinha_verde.png" + '" width="25px" />'
                        else
                            return '<img src="' + URLBase + "Content/img/bolinha_vermelha.png" + '" width="25px" />'
                    }
                },
                {
                    title: '',
                    data: 'Id',
                    width: "115px",
                    render: function (data, type, row) {
                        var buttons = "<a href='" + URLBase + "Alerta/Edit/" + data + "' class='btn btn-link'><i class='fa fa-edit'></i>&nbsp;Editar</a>" +
                            '<button class="btn btn-link" onclick="confirmDelete(\'Alerta/Delete\',' + data + ', function () { $(\'#dataTable\').DataTable().ajax.reload(); })"><i class="fa fa-trash"></i>&nbsp;Remover</button>';
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

    if ($('.selectpicker-veiculos').length > 0) {
        $('.selectpicker-veiculos').selectpicker().ajaxSelectPicker({
            ajax: {
                url: URLBase + "Alerta/ListarVeiculos",
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
});