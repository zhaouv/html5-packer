# HTML5 Packer

借助浏览器, 额外提供fs, 把单网页html5应用伪装成桌面app

> todo 更换prog1的名字

> todo 如何不需要修改html直接把fs.js注入进去

# Usage

> 本节未完成,目前在主目录工作

下载release或build后保留`app.exe lib/ app/`

把单网页html5应用放在`./app`中 (默认入口是index.html)

**需要在\<head\>最开始处放置`<script src="/lib/fs.js"></script>`**

程序图标..

网页图标由网页的 shortcut icon (favicon.ico) 决定

win:双击app.exe启动 

linux:[配置环境](linux_version.md) mono app.exe 或者双击启动(?需要chmod 755 app.exe)



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

