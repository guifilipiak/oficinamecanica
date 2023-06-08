// Set new default font family and font color to mimic Bootstrap's default styling
Chart.defaults.global.defaultFontFamily = 'Nunito', '-apple-system,system-ui,BlinkMacSystemFont,"Segoe UI",Roboto,"Helvetica Neue",Arial,sans-serif';
Chart.defaults.global.defaultFontColor = '#858796';

buscarEstatisticaFluxoFinanceiro();
buscarEstatisticaMensalOS();

function buscarEstatisticaMensalOS() {
    $.ajax({
        url: URLBase + "Home/BuscarEstatisticaMensalOS",
        cache: false,
        data: {
            ano: $("#filtroAno").val()
        },
        success: function (json) {
            var ctx = document.getElementById("myAreaChart");
            var myLineChart = new Chart(ctx, {
                type: 'line',
                data: JSON.parse(json),
                options: {
                    maintainAspectRatio: false,
                    layout: {
                        padding: {
                            left: 10,
                            right: 25,
                            top: 25,
                            bottom: 0
                        }
                    },
                    scales: {
                        xAxes: [{
                            time: {
                                unit: 'date'
                            },
                            gridLines: {
                                display: true,
                                drawBorder: true
                            },
                            ticks: {
                                maxTicksLimit: 10
                            }
                        }],
                        yAxes: [{
                            ticks: {
                                maxTicksLimit: 10,
                                padding: 10,
                                // Include a dollar sign in the ticks
                                callback: function (value, index, values) {
                                    return 'R$ ' + decimalFormat(value, "pt-BR");
                                }
                            },
                            gridLines: {
                                color: "rgb(234, 236, 244)",
                                zeroLineColor: "rgb(234, 236, 244)",
                                drawBorder: false,
                                borderDash: [2],
                                zeroLineBorderDash: [2]
                            }
                        }]
                    },
                    legend: {
                        display: false
                    },
                    tooltips: {
                        backgroundColor: "rgb(255,255,255)",
                        bodyFontColor: "#858796",
                        titleMarginBottom: 10,
                        titleFontColor: '#6e707e',
                        titleFontSize: 14,
                        borderColor: '#dddfeb',
                        borderWidth: 1,
                        xPadding: 15,
                        yPadding: 15,
                        displayColors: true,
                        intersect: true,
                        mode: 'index',
                        caretPadding: 10,
                        callbacks: {
                            label: function (tooltipItem, chart) {
                                var datasetLabel = chart.datasets[tooltipItem.datasetIndex].label || '';
                                return datasetLabel + ': R$ ' + decimalFormat(tooltipItem.yLabel, "pt-BR");
                            }
                        }
                    }
                }
            });
        }
    });
}

function buscarEstatisticaFluxoFinanceiro(){
    $.ajax({
        url: URLBase + "Home/BuscarEstatisticaFluxoFinanceiro",
        cache: false,
        data: {
            ano: $("#filtroAno").val()
        },
        success: function (json) {
            var ctx = document.getElementById("myAreaChart2");
            var myLineChart = new Chart(ctx, {
                type: 'line',
                data: JSON.parse(json),
                options: {
                    maintainAspectRatio: false,
                    layout: {
                        padding: {
                            left: 10,
                            right: 25,
                            top: 25,
                            bottom: 0
                        }
                    },
                    scales: {
                        xAxes: [{
                            time: {
                                unit: 'date'
                            },
                            gridLines: {
                                display: true,
                                drawBorder: true
                            },
                            ticks: {
                                maxTicksLimit: 10
                            }
                        }],
                        yAxes: [{
                            ticks: {
                                maxTicksLimit: 10,
                                padding: 10,
                                // Include a dollar sign in the ticks
                                callback: function (value, index, values) {
                                    return 'R$ ' + decimalFormat(value, "pt-BR");
                                }
                            },
                            gridLines: {
                                color: "rgb(234, 236, 244)",
                                zeroLineColor: "rgb(234, 236, 244)",
                                drawBorder: false,
                                borderDash: [2],
                                zeroLineBorderDash: [2]
                            }
                        }]
                    },
                    legend: {
                        display: false
                    },
                    tooltips: {
                        backgroundColor: "rgb(255,255,255)",
                        bodyFontColor: "#858796",
                        titleMarginBottom: 10,
                        titleFontColor: '#6e707e',
                        titleFontSize: 14,
                        borderColor: '#dddfeb',
                        borderWidth: 1,
                        xPadding: 15,
                        yPadding: 15,
                        displayColors: true,
                        intersect: true,
                        mode: 'index',
                        caretPadding: 10,
                        callbacks: {
                            label: function (tooltipItem, chart) {
                                var datasetLabel = chart.datasets[tooltipItem.datasetIndex].label || '';
                                return datasetLabel + ': R$ ' + decimalFormat(tooltipItem.yLabel, "pt-BR");
                            }
                        }
                    }
                }
            });
        }
    });
}