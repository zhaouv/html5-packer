

回顾NW.js和neutralinojs  
html5打包成app/exe  
感觉不如这么搞  
以python server为例子  
+ 打包成exe
+ 判定是否安装了chrome  
+ 找一个端口, 并以app模式启动chrome  
+ 网页中设置关闭时给server发个post用来关闭server  
双击exe启动时的质感和一般exe应该会没有区别  
chrome启动参数  
https://blog.csdn.net/mimishy2000/article/details/88315347

判定:  
https://www.jianshu.com/p/6314f6095658  
谷歌浏览器安装后都会 windows系统中注册表 “HKEY_CURRENT_USER”或“HKEY_LOCAL_MACHINE”目录下新增Software\Microsoft\Windows\CurrentVersion\App Paths\chrome.exe项
进而 cmd指令`start chrome`

```
start chrome --profile-directory="Default" --app="data:text/html,<html><body><script>window.moveTo(20,20);window.resizeTo(800,600);window.location='http://zhaouv.net';</script></body></html>"
```

server from https://github.com/jeske/SimpleHttpServer public domain


Process.Start("xdg-open", "https://www.google.com");

如果想改成不只是监听localhost,改[这里](SimpleHttpServer/HttpServer.cs#L44)
Int32 port = 13000;
IPAddress localAddr = IPAddress.Parse("127.0.0.1");

// TcpListener server = new TcpListener(port);
server = new TcpListener(localAddr, port);

