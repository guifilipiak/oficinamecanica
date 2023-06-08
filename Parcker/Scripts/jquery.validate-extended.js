/* validate extension control jquery */
jQuery.extend(jQuery.validator.methods, {
    date: function (value, element) {
        var dataType = $(element).data().type;
        switch (dataType) {
            case "date":
                if (value != "") {
                    if (value.match("^(?=\\d{2}([-.,\\/])\\d{2}\\1\\d{4}$)(?:0[1-9]|1\\d|[2][0-8]|29(?!.02.(?!(?!(?:[02468][1-35-79]|[13579][0-13-57-9])00)\\d{2}(?:[02468][048]|[13579][26])))|30(?!.02)|31(?=.(?:0[13578]|10|12))).(?:0[1-9]|1[012]).\\d{4}$")) {
                        return true;
                    }
                    else {
                        return false;
                    }
                }
                else {
                    return true;
                }
                break;

            case "datetime":
                var data = value.slice(0, 10);
                var hora = value.slice(11, 16);
                if (value != "") {
                    if (data.match("^(?=\\d{2}([-.,\\/])\\d{2}\\1\\d{4}$)(?:0[1-9]|1\\d|[2][0-8]|29(?!.02.(?!(?!(?:[02468][1-35-79]|[13579][0-13-57-9])00)\\d{2}(?:[02468][048]|[13579][26])))|30(?!.02)|31(?=.(?:0[13578]|10|12))).(?:0[1-9]|1[012]).\\d{4}$")) {
                        if (hora.match("^([0-9]|0[0-9]|1[0-9]|2[0-3]):[0-5][0-9]$")) {
                            return true;
                        } else {
                            return false;
                        }
                    }
                    else {
                        return this.optional(element) || /^\d\d?\/\d\d?\/\d\d\d?\d?$/.test(value);
                    }
                } else {
                    return true;
                }
                break;

            case "datetimeseconds":
                var data = value.slice(0, 10);
                var hora = value.slice(11, 19);
                if (value != "") {
                    if (data.match("^(?=\\d{2}([-.,\\/])\\d{2}\\1\\d{4}$)(?:0[1-9]|1\\d|[2][0-8]|29(?!.02.(?!(?!(?:[02468][1-35-79]|[13579][0-13-57-9])00)\\d{2}(?:[02468][048]|[13579][26])))|30(?!.02)|31(?=.(?:0[13578]|10|12))).(?:0[1-9]|1[012]).\\d{4}$")) {
                        if (hora.match("^([0-9]|0[0-9]|1[0-9]|2[0-3]):[0-5][0-9]$")) {
                            return true;
                        } else {
                            return false;
                        }
                    }
                    else {
                        return this.optional(element) || /^\d\d?\/\d\d?\/\d\d\d?\d?$/.test(value);
                    }
                } else {
                    return true;
                }
                break;

            case "time":
                if (value != "") {
                    if (value.match("^([0-9]|0[0-9]|1[0-9]|2[0-3]):[0-5][0-9]$")) {
                        return true;
                    } else {
                        return false;
                    }
                } else {
                    return true;
                }
                break;

            case "timeseconds":
                if (value != "") {
                    if (value.match("^([0-9]|0[0-9]|1[0-9]|2[0-3]):[0-5][0-9]$")) {
                        return true;
                    } else {
                        return false;
                    }
                } else {
                    return true;
                }
                break;

            default:
                return this.optional(element) || /^\d\d?\/\d\d?\/\d\d\d?\d?$/.test(value);
                break;
        }
    },
    number: function (value, element) {
        var dataType = $(element).data().type;
        switch (dataType) {
            case "decimal":
                return /^(([1-9]\d{0,2}(\.\d{3})*(,\d+))|([0]{1}(,\d+)))$/.test(value);
                break;
            default:
                return this.optional(element) || /^-?(?:\d+|\d{1,3}(?:\.\d{3})+)(?:,\d+)?$/.test(value);
                break;
        }
    }
});

$.validator.methods.range = function (value, element, param) {
    var globalizedValue = value.replace(".", ""); globalizedValue = globalizedValue.replace(",", ".");
    return this.optional(element) || (globalizedValue >= param[0] && globalizedValue <= param[1]);
};

$.validator.methods.number = function (value, element) {
    return this.optional(element) || /^-?(?:\d+|\d{1,3}(?:[\s\.,]\d{3})+)(?:[\.,]\d+)?$/.test(value);
};

// This will set `ignore` for all validation calls
jQuery.validator.setDefaults({
    // This will ignore all hidden elements alongside `contenteditable` elements
    // that have no `name` attribute
    ignore: ":hidden, [contenteditable='true']:not([name])"
});