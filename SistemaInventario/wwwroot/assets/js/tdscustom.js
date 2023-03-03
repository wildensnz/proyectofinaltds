var system = {};

system.get = function (url, formdata, callback) {
    $.ajax({
        url: url,
        type: 'GET',
        processData: false,
        contentType: false,
        data: formdata,
        success: callback,
        async: false
    });
};

system.save = function (url, formdata, callback, btn) {

    var formArray = $(form).serializeArray();
    let formData = new FormData();
    for (var i of formArray) {
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
            if (btn == null) $(btn).prop('disabled', true)
            Swal.fire("Error", xhr.responseText, "error");
        }
    });
};


system.clear = function (form) {
    $(form).find(":input").each(function () {
        var input = $(this);
        if (input.is('input:checkbox')) {
            $(input).prop("checked", false);
        }
        else {
            $(input).val("");
        }
    });
}