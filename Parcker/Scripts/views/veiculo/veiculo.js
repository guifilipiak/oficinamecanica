$(document).ready(function () {
    if ($('#dataTable').length > 0) {
        $('#dataTable').DataTable({
            processing: true,
            serverSide: true,
            ajax: {
                url: URLBase + 'Veiculo/RetornaListaVeiculos',
                type: 'POST',
                dataSrc: 'data'
            },
            columns: [
                { title: 'Placa', data: 'Placa' },
                { title: 'Ano', data: 'Ano' },
                { title: 'Fabr.', data: 'AnoFabricacao' },
                { title: 'Modelo', data: 'Modelo' },
                { title: 'Marca', data: 'Marca' },
                { title: 'KM', data: 'KM' },
                { title: 'Proprietário', data: 'Proprietario' },
                { title: 'Renavam', data: 'Renavam', visible: false },
                { title: 'Cor', data: 'Cor' },
                { title: 'Chassi', data: 'Chassi', visible: false },
                {
                    title: '',
                    data: 'Id',
                    width: "115px",
                    render: function (data, type, row) {
                        var buttons = '<a href="' + URLBase + 'Veiculo/Edit/' + data + '" class="btn btn-link"><i class="fa fa-edit"></i>&nbsp;Editar</a>' +
                            '<button class="btn btn-link" onclick="confirmDelete(\'Veiculo/Delete\',' + data + ', function () { $(\'#dataTable\').DataTable().ajax.reload(); })"><i class="fa fa-trash"></i>&nbsp;Remover</button>';
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
});