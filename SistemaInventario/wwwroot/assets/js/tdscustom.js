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

system.save = function (url, formdata, callback) {
    $.ajax({
        url: url,
        type: 'POST',
        processData: false,
        contentType: false,
        data: formdata,
        success: callback
    });
};