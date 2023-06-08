var message = [];
message.success = function (title, message) {
    $.toast({
        heading: title,
        text: message,
        position: 'top-right',
        stack: false,
        icon: 'success',
        hideAfter: 15000
    });
};

message.info = function (title, message) {
    $.toast({
        heading: title,
        text: message,
        position: 'top-right',
        stack: false,
        icon: 'info',
        hideAfter: 15000
    });
};

message.warning = function (title, message) {
    $.toast({
        heading: title,
        text: message,
        position: 'top-right',
        stack: false,
        icon: 'warning',
        hideAfter: 15000
    });
};

message.error = function (title, message) {
    $.toast({
        heading: title,
        text: message,
        position: 'top-right',
        stack: false,
        icon: 'error',
        hideAfter: 15000
    });
};

var forms, buttonSubmit, oldTextButtonSubmit;
$(document).ready(function () {
    //String.prototype.replaceAll = function (search, replacement) {
    //    var target = this;
    //    return target.replace(new RegExp(search, 'g'), replacement);
    //};

    $("[data-toggle=tooltip]").tooltip();

    String.prototype.replaceAll = function (search, replacement) {
        var target = this;
        return target.split(search).join(replacement);
    };

    forms = $("form");

    initMask();
});

function destroyMask(selector) {
    var container = selector ? $(selector) : $("body");
    if (container)
        container.find("input").unmask();
    else
        $("input").unmask();
}

function initMask(selector) {
    var container = selector ? $(selector) : $("body");
    container.find('.placa').mask('AAA-AAAA', { placeholder: "___-____", clearIfNotMatch: true, selectOnFocus: true });
    container.find('.ano').mask('0000', { placeholder: "____", clearIfNotMatch: true, selectOnFocus: true });
    container.find('.porcentagem').mask('000', { placeholder: "___", clearIfNotMatch: false, selectOnFocus: true, reverse: true });
    container.find('.numero').mask('999999999', { placeholder: "_________", selectOnFocus: true });
    container.find('.numero-menor').mask('9999', { placeholder: "____", selectOnFocus: true });
    container.find('.date').mask('00/00/0000', { placeholder: "__/__/____", clearIfNotMatch: true, selectOnFocus: true });
    container.find('.time').mask('00:00:00', { placeholder: "__:__:__", clearIfNotMatch: true, selectOnFocus: true });
    container.find('.datetime').mask('00/00/0000 00:00:00', { placeholder: "__/__/____ __:__:__", clearIfNotMatch: true, selectOnFocus: true });
    container.find('.cep').mask('00000-000', { placeholder: "_____-___", clearIfNotMatch: true, selectOnFocus: true });
    container.find('.telefone').mask('0000-00009', { placeholder: "_____-_____", selectOnFocus: true });
    container.find('.ddd_telefone').mask('(00) 0000-00009', { placeholder: "(__) _____-_____", selectOnFocus: true });
    container.find('.cpf').mask('000.000.000-00', { reverse: true, placeholder: "___.___.___-__", clearIfNotMatch: true, selectOnFocus: true });
    container.find('.rg').mask('000000000000', { reverse: true, placeholder: "____________", clearIfNotMatch: false, selectOnFocus: true });
    container.find('.renavam').mask('000000000000', { reverse: true, placeholder: "____________", clearIfNotMatch: true, selectOnFocus: true });
    container.find('.cnpj').mask('00.000.000/0000-00', { reverse: true, placeholder: "__.___.___/_____-__", clearIfNotMatch: true, selectOnFocus: true });
    container.find('.decimal2').mask('000.000.000.000.000,00', { reverse: true, selectOnFocus: true });
    container.find('.decimal').mask("#.##0,00", { reverse: true, selectOnFocus: true });
    container.find('[data-custom-mask]').each(function (e, elem) {
        createMask(elem, $(elem).data("custom-mask"));
    });
    container.find('[data-transform="lower"]').each(function (e, elem) {
        replaceToLower(elem);
    });
    container.find('[data-transform="upper"]').each(function (e, elem) {
        replaceToUpper(elem);
    });

    var datefields = container.find('.date');
    for (var i = 0; i < datefields.length; i++) {
        $(datefields[i]).datepicker({
            language: "pt-BR",
            todayHighlight: true,
            format: 'dd/mm/yyyy'
        });
    }
}

function replaceToLower(elem) {
    $(elem).keyup(function () {
        this.value = this.value.toLowerCase();
    });
}

function replaceToUpper(elem) {
    $(elem).keyup(function () {
        this.value = this.value.toUpperCase();
    });
}

function escapeRegExp(str) {
    return str.replace(/[.*+?^${}()|[\]\\]/g, "\\$&");
}

function createMask(element, mask) {
    var placeholder = mask.replaceAll("0", "_").replaceAll("A", "_");
    $(element).mask(mask, {
        placeholder: placeholder,
        clearIfNotMatch: false, selectOnFocus: true
    });
}

function formBegin() {
    buttonSubmit = forms.find("button[type=submit]");
    oldTextButtonSubmit = buttonSubmit.html();
    if (forms.valid()) {
        buttonSubmit.html("Aguarde...").prop("disabled", true);
    }

    if (typeof callback !== 'undefined')
        callback("begin");
}

function formComplete() {
    if (buttonSubmit)
        buttonSubmit.html(oldTextButtonSubmit).prop("disabled", false);

    if (typeof callback !== 'undefined')
        callback("complete");
}

function formFailure(xhr) {
    message.error("Vish", xhr.Message);
    if (typeof callback !== 'undefined')
        callback("error", xhr);
}

function formSuccess(result) {
    if (result.IsValid) {
        forms.find("#Id").val(result.Data.Id);
        $("#page-title").html("Editando registro n°" + result.Data.Id);
        message.success("OK", result.Message);
    }
    else if (!result.IsValid && result.IsValid != undefined)
        message.warning("Atenção", result.Message);

    if (typeof callback !== 'undefined')
        callback("success", result);
}

function confirmDelete(url, id, callback) {
    Swal.fire({
        title: 'Você tem certeza disso?',
        text: 'Não será possível reverter esta ação!',
        type: 'warning',
        showCancelButton: true,
        confirmButtonText: 'Sim, desejo excluir!',
        cancelButtonText: 'Não, quero cancelar!'
    }).then((result) => {
        if (result.value) {
            $.ajax({
                url: URLBase + url,
                type: "POST",
                data: {
                    id: id
                },
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

                    if (callback)
                        callback(result);
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

function JSONDate(dateStr) {
    var m, day;
    jsonDate = dateStr;
    var d = new Date(parseInt(jsonDate.substr(6)));
    m = d.getMonth() + 1;
    if (m < 10)
        m = '0' + m
    if (d.getDate() < 10)
        day = '0' + d.getDate()
    else
        day = d.getDate();
    return (m + '/' + day + '/' + d.getFullYear());
}

function JSONDatePtBr(dateStr) {
    var m, day;
    jsonDate = dateStr;
    var d = new Date(parseInt(jsonDate.substr(6)));
    m = d.getMonth() + 1;
    if (m < 10)
        m = '0' + m;
    if (d.getDate() < 10)
        day = '0' + d.getDate();
    else
        day = d.getDate();
    return (day + '/' + m + '/' + d.getFullYear());
}

function JSONDateTime(dateStr) {
    if (dateStr) {
        jsonDate = dateStr;
        var d = new Date(parseInt(jsonDate.substr(6)));
        var m, day;
        m = d.getMonth() + 1;
        if (m < 10)
            m = '0' + m
        if (d.getDate() < 10)
            day = '0' + d.getDate()
        else
            day = d.getDate();
        var formattedDate = day + "/" + m + "/" + d.getFullYear();
        var hours = (d.getHours() < 10) ? "0" + d.getHours() : d.getHours();
        var minutes = (d.getMinutes() < 10) ? "0" + d.getMinutes() : d.getMinutes();
        var seconds = (d.getSeconds() < 10) ? "0" + d.getSeconds() : d.getSeconds();
        var formattedTime = hours + ":" + minutes + ":" + seconds;
        formattedDate = formattedDate + " " + formattedTime;
        return formattedDate;
    } else
        return "";
}

function decimalFormat(number, format) {
    if (format != "pt-BR")
        return parseFloat(number.replaceAll(".", "").replaceAll(",", ".") || "0");
    else
        return new Intl.NumberFormat(format, { minimumFractionDigits: 2, maximumFractionDigits: 2 }).format(number);
}

function decimalFormatCustom(number, decimals, dec_point, thousands_sep) {
    // *     example: number_format(1234.56, 2, ',', ' ');
    // *     return: '1 234,56'
    number = (number + '').replace(',', '').replace(' ', '');
    var n = !isFinite(+number) ? 0 : +number,
        prec = !isFinite(+decimals) ? 0 : Math.abs(decimals),
        sep = (typeof thousands_sep === 'undefined') ? ',' : thousands_sep,
        dec = (typeof dec_point === 'undefined') ? '.' : dec_point,
        s = '',
        toFixedFix = function (n, prec) {
            var k = Math.pow(10, prec);
            return '' + Math.round(n * k) / k;
        };
    // Fix for IE parseFloat(0.55).toFixed(0) = 0;
    s = (prec ? toFixedFix(n, prec) : '' + Math.round(n)).split('.');
    if (s[0].length > 3) {
        s[0] = s[0].replace(/\B(?=(?:\d{3})+(?!\d))/g, sep);
    }
    if ((s[1] || '').length < prec) {
        s[1] = s[1] || '';
        s[1] += new Array(prec - s[1].length + 1).join('0');
    }
    return s.join(dec);
}

function alertaExpirado(id, title, descricao) {
    Swal.fire({
        title: title,
        type: 'info',
        showCancelButton: false,
        showConfirmButton: false,
        html:
            '<p class="lead"/>' + descricao + '<p/>' +
            '<div class="form-group">' +
            '   <button id="alertar" type="button" class="btn btn-lg btn-success"><i class="fa fa-envelope"></i>&nbsp;Alertar</button>' +
            '   <button id="cancelar" type="button" class="btn btn-lg btn-warning"><i class="fa fa-clock"></i>&nbsp;Ignorar</button>' +
            '   <button id="desativar" type="button" class="btn btn-lg btn-danger"><i class="fa fa-ban"></i>&nbsp;Desativar</button>' +
            '</div>',
        onBeforeOpen: () => {
            var alertar = $('#alertar');
            var cancelar = $('#cancelar');
            var desativar = $('#desativar');

            alertar.click(function () {
                $.ajax({
                    url: URLBase + "Alerta/Alertar",
                    type: "POST",
                    cache: false,
                    data: {
                        id: id
                    },
                    success: function (result) {
                        if (result.IsValid)
                            message.success("OK", result.Message);
                        else
                            message.success("Erro", result.Message);

                        Swal.close();
                        atualizaAlertasExpirados();
                    }
                });
            });

            cancelar.click(function () {
                $.ajax({
                    url: URLBase + "Alerta/Adiar",
                    type: "POST",
                    cache: false,
                    data: {
                        id: id
                    },
                    success: function (result) {
                        if (result.IsValid)
                            message.success("OK", result.Message);
                        else
                            message.success("Erro", result.Message);

                        Swal.close();
                        atualizaAlertasExpirados();
                    }
                });
            });

            desativar.click(function () {
                $.ajax({
                    url: URLBase + "Alerta/Desativar",
                    type: "POST",
                    cache: false,
                    data: {
                        id: id
                    },
                    success: function (result) {
                        if (result.IsValid)
                            message.success("OK", result.Message);
                        else
                            message.success("Erro", result.Message);

                        Swal.close();
                        atualizaAlertasExpirados();
                    }
                });
            });
        }
    });
}

function atualizaAlertasExpirados() {
    $.ajax({
        url: URLBase + "Alerta/ExpiredAlertMenu",
        cache: false,
        success: function (html) {
            $("#nav-alertas").replaceWith(html);
        }
    });

    var table = $('#dataTableAlerta');
    if (table)
        table.DataTable().ajax.reload();
}