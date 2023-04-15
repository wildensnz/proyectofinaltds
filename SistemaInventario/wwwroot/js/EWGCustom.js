var system = {};

system.get = function (url, callback) {
    $.ajax({
        url: url,
        type: 'GET',
        processData: false,
        contentType: false,
        success: callback,
        async: false
    });
};

system.save = function (url, form, callback, btn) {

    var formArray = $(form).serializeArray();
    let formData = new FormData();
    for (var i of formArray) {
        if (i.name == "Principal" || i.name == "EsPorcentaje" || i.name == "AplicarDefault")
            formData.append(i.name, i.value == "on" ? true : false)
        else
            formData.append(i.name, i.value)
    }
    if (btn != null) $(btn).prop('disabled', true)
    $.ajax({
        url: url,
        type: 'POST',
        processData: false,
        contentType: false,
        data: formData,
        success: callback,
        error: function (xhr, resp, text) {
            if (btn != null) $(btn).prop('disabled', true)
            Swal.fire("Error!", xhr.responseText, "error");
        }
    });
};

system.saveUpload = function (url, form, callback, btn) {

    let formData = new FormData($(form)[0]);
    if (btn != null) $(btn).prop('disabled', true)
    $.ajax({
        url: url,
        type: 'POST',
        processData: false,
        contentType: false,
        data: formData,
        success: callback,
        error: function (xhr, resp, text) {
            if (btn != null) $(btn).prop('disabled', true)
            Swal.fire("Error!", xhr.responseText, "error");
        }
    });
};


system.clear = function (form) {
    $(form).find(":input").each(function () {
        var input = $(this);
        if (input.is('input:checkbox')) {
            $(input).prop("checked", false);
        } else {
            if (!input.is('input:radio'))
                $(input).val("");
        }
    });
}


system.getCookie = function (cname) {
    let name = cname + "=";
    let decodedCookie = decodeURIComponent(document.cookie);
    let ca = decodedCookie.split(';');
    for (let i = 0; i < ca.length; i++) {
        let c = ca[i];
        while (c.charAt(0) == ' ') {
            c = c.substring(1);
        }
        if (c.indexOf(name) == 0) {
            return c.substring(name.length, c.length);
        }
    }
    return "";
}


var mesesYear = ['', 'Ene.', 'Feb.', 'Mar.', 'Abr.', 'May.', 'Jun.', 'Jul.', 'Ago.', 'Sep.', 'Oct.', 'Nov', 'Dic.']