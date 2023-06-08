$(document).ready(function () {
    buscarResumoRendimentosOS();
});

function atualizaDashBoard(ano) {
    if (ano) {
        $("#filtroAno").val(ano);
        $("#filtroAnoText").html(ano);
    }

    buscarResumoRendimentosOS();
    buscarEstatisticaMensalOS();
    buscarEstatisticaOS();
    buscarEstatisticaFluxoFinanceiro();
}

function buscarResumoRendimentosOS() {
    $.ajax({
        url: URLBase + "Home/BuscarResumoRendimentosOS",
        cache: false,
        data: {
            ano: $("#filtroAno").val()
        },
        success: function (json) {
            if (json.IsValid) {
                $("#quantOS").html(json.Data.QuantOS);
                $("#quantOSAndamento").html(json.Data.QuantOSAndamento);
                $("#percentOSFinalizada").html(json.Data.PercentOSFinalizada + "%");
                $("#progressOSFinalizada").css("width", json.Data.PercentOSFinalizada + "%");
                $("#totalOSFinalizada").html(json.Data.TotalOSFinalizada);
            }
        }
    });
}