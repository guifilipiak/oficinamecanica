$(document).ready(function () {
    $('#dataTable').DataTable({
        processing: true,
        serverSide: true,
        ajax: {
            url: URLBase + 'Categoria/RetornaListaCategorias',
            type: 'POST',
            dataSrc: 'data'
        },
        columns: [
            { title: 'Descricao', data: 'Descricao' },
            {
                title: '',
                data: 'Id',
                width: "115px",
                render: function (data, type, row) {
                    var buttons = "<a href='" + URLBase + "Categoria/Edit/" + data + "' class='btn btn-link'><i class='fa fa-edit'></i>&nbsp;Editar</a>" +
                        '<button class="btn btn-link" onclick="confirmDelete(\'Categoria/Delete\',' + data + ', function () { $(\'#dataTable\').DataTable().ajax.reload(); })"><i class="fa fa-trash"></i>&nbsp;Remover</button>';
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

});