console.log('======= local saves start =======');

// --v--
    // 这段其实不需要, 只是为了执行 html5-packer 的lib/workerforexitserver.js里的 exitProg 让 packer 的server不会一直在后台运行
if(window.fs==null || fs.version==null){
    main.log('请在index.html的<head>的最开始处放置"<script src="/lib/fs.js"></script>"')
    var exitWorker = new Worker('/lib/workerforexitserver.js');
}
// --^--

fs.mkdir('saves',function (err,data) {
    if (err) {
        alert('创建存档目录失败: '+err)
    }
})

utils.prototype.setLocalForage = function (key, value, successCallback, errorCallback) {

    if (!core.platform.useLocalForage) {
        if (this.setLocalStorage(key, value)) {
            if (successCallback) successCallback();
        }
        else {
            if (errorCallback) errorCallback();
        }
        return;
    }

    if (value == null) {
        this.removeLocalForage(key);
        return;
    }

    fs.writeFile('saves/'+core.firstData.name + "_" + key, JSON.stringify(value), 'utf-8',function (err, data) {
        if (err) {
            if (errorCallback) errorCallback(err);
        }
        else {
            if (key == 'autoSave') core.saves.ids[0] = true;
            else if (/^save\d+$/.test(key)) core.saves.ids[parseInt(key.substring(4))] = true;
            if (successCallback) successCallback();
        }
    } )
}

utils.prototype.getLocalForage = function (key, defaultValue, successCallback, errorCallback) {

    if (!core.platform.useLocalForage) {
        var value = this.getLocalStorage(key, defaultValue);
        if (successCallback) successCallback(value);
        return;
    }

    fs.readFile('saves/'+core.firstData.name + "_" + key, 'utf-8' , function (err, value) {
        if (err) {
            if (errorCallback) errorCallback(err);
        }
        else {
            if (!successCallback) return;
            try {
                var res = JSON.parse(value);
                if (value != null) {
                    successCallback(res == null ? defaultValue : res);
                    return;
                }
            }
            catch (e) {
                main.log(e);
            }
            successCallback(defaultValue);
        }
    })
}

utils.prototype.removeLocalForage = function (key, successCallback, errorCallback) {

    if (!core.platform.useLocalForage) {
        this.removeLocalStorage(key);
        if (successCallback) successCallback();
        return;
    }

    fs.deleteFile('saves/'+core.firstData.name + "_" + key, function (err, data) {
        if (err) {
            if (errorCallback) errorCallback(err);
        }
        else {
            if (key == 'autoSave') delete core.saves.ids[0];
            else if (/^save\d+$/.test(key)) delete core.saves.ids[parseInt(key.substring(4))];
            if (successCallback) successCallback();
        }
    })
}

////// 获得所有存在存档的存档位 //////
control.prototype.getSaveIndexes = function (callback) {
    var indexes = {};
    if (core.platform.useLocalForage) {
        fs.readdir('saves',function (err, filenames) {
            filenames.forEach(function (key) {
                core.control._getSaveIndexes_getIndex(indexes, key);
            })
            callback(indexes);
        })
    }
    else {
        Object.keys(localStorage).forEach(function (key) {
            core.control._getSaveIndexes_getIndex(indexes, key);
        });
        callback(indexes);
    }
}

control.prototype._getSaveIndexes_getIndex = function (indexes, name) {
    var e = new RegExp('^'+core.firstData.name+"_(save\\d+|autoSave)$").exec(name);
    if (e) {
        if (e[1]=='autoSave') indexes[0]=true;
        else indexes[parseInt(e[1].substring(4))] = true;
        console.log()
    }
}


actions.prototype._clickStorageRemove_all = function () {
    core.drawText("\t[操作失败]移除了此功能。");
}
actions.prototype._clickStorageRemove_current = function () {
    core.drawText("\t[操作失败]移除了此功能。");
}
console.log('======= local saves end =======');