$(document).ready(function () {
    $('#dataTable').DataTable({
        processing: true,
        serverSide: true,
        ajax: {
            url: URLBase + 'Produto/RetornaListaProdutos',
            type: 'POST',
            dataSrc: 'data'
        },
        columns: [
            { title: 'Nome', data: 'Nome' },
            { title: 'Descrição', data: 'Descricao', visible: false },
            { title: 'Marca', data: 'Marca' },
            { title: 'Estoque', data: 'Estoque' },
            { title: 'Valor Unitário', data: 'ValorUnitario' },
            {
                title: 'Ativo',
                data: 'Ativo',
                width: "50px",
                className: "text-center",
                render: function (data, type, row) {
                    if (data == 1)
                        return '<img src="' + URLBase + "Content/img/bolinha_verde.png" + '" width="25px" />';
                    else
                        return '<img src="' + URLBase + "Content/img/bolinha_vermelha.png" + '" width="25px" />';
                }
            },
            {
                title: 'Estoque Mínimo',
                data: 'AlertaEstoqueMinimo',
                width: "50px",
                className: "text-center",
                render: function (data, type, row) {
                    if (data == 1)
                        return '<img src="' + URLBase + "Content/img/bolinha_verde.png" + '" width="25px" />';
                    else
                        return '<img src="' + URLBase + "Content/img/bolinha_vermelha.png" + '" width="25px" />';
                }
            },
            {
                title: '',
                data: 'Id',
                width: "115px",
                render: function (data, type, row) {
                    var buttons = "<a href='" + URLBase + "Produto/Edit/" + data + "' class='btn btn-link'><i class='fa fa-edit'></i>&nbsp;Editar</a>" +
                        '<button class="btn btn-link" onclick="confirmDelete(\'Produto/Delete\',' + data + ', function () { $(\'#dataTable\').DataTable().ajax.reload(); })"><i class="fa fa-trash"></i>&nbsp;Remover</button>';
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