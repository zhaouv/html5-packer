(function () {
    var prog1 = {};

    var _isset = function (val) {
        if (val == undefined || val == null || (typeof val == 'number' && isNaN(val))) {
            return false;
        }
        return true
    }

    var _http = function (type, url, formData, success, error, mimeType, responseType) {
        var xhr = new XMLHttpRequest();
        xhr.open(type, url, true);
        if (_isset(mimeType))
            xhr.overrideMimeType(mimeType);
        if (_isset(responseType))
            xhr.responseType = responseType;
        xhr.onload = function (e) {
            if (xhr.status == 200) {
                if (_isset(success)) {
                    success(xhr.response);
                }
            }
            else {
                if (_isset(error))
                    error("HTTP " + xhr.status);
            }
        };
        xhr.onabort = function () {
            if (_isset(error))
                error("Abort");
        }
        xhr.ontimeout = function () {
            if (_isset(error))
                error("Timeout");
        }
        xhr.onerror = function () {
            if (_isset(error))
                error("Error on Connection");
        }
        if (_isset(formData))
            xhr.send(formData);
        else xhr.send();
    }


    var postsomething = function (data, _ip, callback) {
        if (typeof (data) == typeof ([][0]) || data == null) data = JSON.stringify({ 1: 2 });

        _http("POST", _ip, data, function (data) {
            if (data.slice(0, 6) == 'error:') {
                callback(data, null);
            }
            else {
                callback(null, data);
            }
        }, function (e) {
            if (window.main != null && main.log) main.log(e);
            else console.log(e);
            callback(e + "：请检查启动服务是否处于正常运行状态。");
        }, "text/plain; charset=x-user-defined");
    }

    prog1.exit = function () {
        var data = '';
        data += 'type=utf8&';
        data += 'name=' + 'abc';
        postsomething(data, '/exitProg', function () { });
    }
    prog1.cancelExit = function () {
        var data = '';
        data += 'type=utf8&';
        data += 'name=' + 'abc';
        postsomething(data, '/cancalExit', function () { });
    }

    prog1.timer = setInterval(function(){ prog1.exit() }, 300);
    return prog1;
})();