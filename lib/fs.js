fs = (function () {
    var fs = {
        version: '0.0.2'
    };

    // inital version from https://github.com/ckcz123/mota-js/issues/13

    // ref https://nodejs.org/api/fs.html
    //     http://nodejs.cn/api/fs.html

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

    var _httpSync = function (type, url, formData, mimeType, responseType) {
        var xhr = new XMLHttpRequest();
        xhr.open(type, url, false);
        if (_isset(mimeType))
            xhr.overrideMimeType(mimeType);
        if (_isset(responseType))
            xhr.responseType = responseType;
        if (_isset(formData))
            xhr.send(formData);
        else xhr.send();
        if (xhr.status == 200) {
            return xhr.response
        }
        else {
            throw ("HTTP " + xhr.status);
        }
    }


    var _post = function (data, _ip, callback) {
        if (data == null) data = JSON.stringify({ 1: 2 });

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

    var _postSync = function (data, _ip) {
        if (data == null) data = JSON.stringify({ 1: 2 });
        return _httpSync("POST", _ip, data, "text/plain; charset=x-user-defined");
    }

    var _checkEncoding = function (encoding) {
        if (typeof (encoding) !== 'string') {
            encoding = encoding.encoding || 'utf8'
        } else {
            if (encoding === 'utf-8') {
                encoding = 'utf8'
            }
            if (encoding !== 'utf8' && encoding !== 'base64') {
                throw "only support 'utf8' and 'base64' now."
            }
        }
        return encoding
    }
    var _checkArg = function (encoding, callback) {
        if (callback == null) {
            callback = encoding
            encoding = 'utf8'
        }
        if (typeof (callback) !== 'function') {
            throw "Callback must be a function."
        }
        return [_checkEncoding(encoding), callback]
    }
    var _checkFilename = function (filename) {
        if (typeof (filename) !== 'string')
            throw 'Type Error of filename';
    }

    fs.readFile = function (filename, encoding, callback) {
        _checkFilename(filename)
        encoding, callback = _checkArg(encoding, callback)
        var data = '';
        data += 'type=' + encoding + '&';
        data += 'name=' + filename;
        _post(data, '/readFile', callback);// todo 似乎是要把文本的也改成用base64传比较稳妥, 之前似乎遇到过bug
    }
    fs.readFileSync = function (filename, encoding) {
        _checkFilename(filename)
        encoding = _checkEncoding(encoding)
        var data = '';
        data += 'type=' + encoding + '&';
        data += 'name=' + filename;
        return _postSync(data, '/readFile');// todo 似乎是要把文本的也改成用base64传比较稳妥, 之前似乎遇到过bug
    }

    ////////////////////// todo 当前进行到了这里 ////////////////////////
    // todo 似乎是要把文本的也改成用base64传比较稳妥, 之前似乎遇到过bug

    fs.writeFile = function (filename, datastr, encoding, callback) {
        if (typeof (filename) != typeof ('') || typeof (datastr) != typeof (''))
            throw 'Type Error in fs.writeFile';
        if (encoding == 'utf-8') {
            //写文本文件
            //filename:支持"/"做分隔符
            //callback:function(err)
            //datastr:字符串
            var data = '';
            data += 'type=utf8&';
            data += 'name=' + filename;
            data += '&value=' + datastr;
            _post(data, '/writeFile', callback);
            return;
        }
        if (encoding == 'base64') {
            //写二进制文件
            //filename:支持"/"做分隔符
            //callback:function(err)
            //datastr:base64字符串
            var data = '';
            data += 'type=base64&';
            data += 'name=' + filename;
            data += '&value=' + datastr;
            _post(data, '/writeFile', callback);
            return;
        }
        throw 'Type Error in fs.writeFile';
    }

    fs.writeMultiFiles = function (filenames, datastrs, callback) {
        _post('name=' + filenames.join(';') + '&value=' + datastrs.join(';'), '/writeMultiFiles', callback);
    }

    fs.readdir = function (path, callback) {
        //callback:function(err, data)
        //path:支持"/"做分隔符,不以"/"结尾
        //data:[filename1,filename2,..] filename是字符串,只包含文件不包含目录
        if (typeof (path) != typeof (''))
            throw 'Type Error in fs.readdir';
        var data = '';
        data += 'name=' + path;
        _post(data, '/listFile', function (err, data) {
            try {
                data = JSON.parse(data);
            } catch (e) {
                err = "Invalid /listFile";
                data = null;
            }
            callback(err, data);
        });
        return;
    }

    /**
     * @param {string} path 支持"/"做分隔符
     * @param {() => {err: string, data}} callback
     */
    fs.mkdir = function (path, callback) {
        //callback:function(err, data)
        if (typeof (path) != typeof (''))
            throw 'Type Error in fs.readdir';
        var data = '';
        data += 'name=' + path;
        _post(data, '/makeDir', callback);
        return;
    }

    /**
     * @param {string} path 支持"/"做分隔符, 不以"/"结尾
     * @param {() => {err: string, data}} callback
     */
    fs.moveFile = function (src, dest, callback) {
        if (typeof (src) != typeof ('') || typeof (dest) != typeof (''))
            throw 'Type Error in fs.readdir';
        var data = '';
        data += 'src=' + src + "&dest=" + dest;
        _post(data, '/moveFile', callback);
        return;
    }

    /**
     * @param {string} path 支持"/"做分隔符, 不以"/"结尾
     * @param {() => {err: string, data}} callback
     */
    fs.deleteFile = function (path, callback) {
        if (typeof (path) != typeof (''))
            throw 'Type Error in fs.readdir';
        var data = '';
        data += 'name=' + path;
        _post(data, '/deleteFile', callback);
        return;
    }

    var exitWorker = new Worker('/lib/workerforexitserver.js');
    return fs;
})();