

chrome启动参数  
https://blog.csdn.net/mimishy2000/article/details/88315347

```
start chrome --profile-directory="Default" --app="data:text/html,<html><body><script>window.moveTo(20,20);window.resizeTo(800,600);window.location='http://zhaouv.net';</script></body></html>"
```

判定chrome:  
https://www.jianshu.com/p/6314f6095658  
谷歌浏览器安装后都会 windows系统中注册表 “HKEY_CURRENT_USER”或“HKEY_LOCAL_MACHINE”目录下新增Software\Microsoft\Windows\CurrentVersion\App Paths\chrome.exe项
进而 cmd指令`start chrome`


server from https://github.com/jeske/SimpleHttpServer public domain


如果想改成不只是监听localhost,改[这里](SimpleHttpServer/HttpServer.cs#L44)
Int32 port = 13000;
IPAddress localAddr = IPAddress.Parse("127.0.0.1");

// TcpListener server = new TcpListener(port);
server = new TcpListener(localAddr, port);
