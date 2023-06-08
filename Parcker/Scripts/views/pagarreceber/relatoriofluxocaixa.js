$(document).ready(function () {
    $("#btnVisualizar").click(function () {
        $.ajax({
            url: URLBase + "PagarReceber/PrinterViewer",
            type: 'POST',
            cache: false,
            data: $("form").serialize(),
            beforeSend: function () {
                $("#containerRelatorio").html("<p class='lead pt-3 text-center'>Carregando relatório...</p>");
            },
            success: function (data) {
                $("#containerRelatorio").html(data);
            },
            error: function () {
                $("#containerRelatorio").html("<p class='lead pt-3 text-center'>Não foi possível gerar o relatório.</p>");
            }
        });
    });
});