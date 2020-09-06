# HTML5 Packer

借助浏览器, 额外提供[fs](https://github.com/zhaouv/html5-packer/issues/1), 把单网页html5应用伪装成桌面app

适合场景: 轻量级的需要文件系统和GUI界面的程序

大概是给 _使用win10的电脑小白 (python node之类的都没有装)_ 提供能跑的本地脚本的最简洁的方式  
c写简单的GUI应用偏麻烦,  
python打包后体积太大,  
csharp windows自带运行时, 来作为server(用c做server也是可行的, 区别不大)  
网页+js 能非常简洁的给出GUI界面



> todo 如何不需要修改html直接把fs.js注入进去

# Usage

> 本节未完成,目前在主目录工作

下载release或build后保留`app.exe lib/ app/`

把单网页html5应用放在`./app`中 (默认入口是index.html)

**需要在\<head\>最开始处放置`<script src="/lib/fs.js"></script>`**  
否则程序会3分钟内自动关闭  
(毕竟是纯后台的服务, 小白不一定知道怎么关, 点x时同时关闭服务的脚本在/lib/fs.js里)  

程序图标..

网页图标由网页的 shortcut icon (favicon.ico) 决定

Windows: 双击app.exe启动 

Windows 8及以上可直接运行本软件，Windows 7需要安装.Net Framework [微软下载界面](https://dotnet.microsoft.com/download/dotnet-framework)

Linux: mono app.exe（[需要安装mono全家桶](linux_version.md)）



# Build

**win**

复制loadenv`copy _loadenv.cmd loadenv.cmd`后根据 Microsoft Visual Studio 的版本修改

```
loadenv
buildapp
```

**linux**

> todo linux 分支的待完善

[配置编译环境](linux_version.md)

```
bash buildapp.cmd
```

# fs
 
提供fs.js  
仿照nodejs的api提供一些函数  
暂时不以require('fs')来提供fs, 而是直接作为全局变量  
借助 XMLHttpRequest 实现  

> 准备提供的函数 未做完

网页没有Buffer类, 直接以字符串作为结果

```js
fs.readFile         // 内容 encoding 仅支持 utf8 和 base64, 文件名 encoding 不允许调整, 下略
fs.readFileSync
fs.writeFile
fs.writeFileSync
fs.readdir
fs.readdirSync
fs.mkdir            // 无论如何设置 options, 总是使用 { recursive: true }
fs.mkdirSync
fs.rename
fs.renameSync
fs.unlink           // unlink 和 rmdir 均以 { recursive: true } 来移除文件和目录
fs.unlinkSync
fs.rmdir
fs.rmdirSync

fs.promises.readFile
fs.promises.writeFile
fs.promises.readdir
fs.promises.mkdir
fs.promises.rename
fs.promises.unlink
fs.promises.rmdir

fs.unstandard.encode64
fs.unstandard.decode64
fs.unstandard.writeMultiFiles   // 类似writeFile, 但是一次写入多个文件
fs.unstandard.listFile          // 类似readdir, 列出所有文件和目录, data=[[files],[subdirs]]
fs.unstandard.deleteFile        // 递归的移除文件和目录
```


