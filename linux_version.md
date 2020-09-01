# Debian系支持

到时候再说。

# Arch系支持

如果需要编译运行C# (.Net)程序，那建议直接安装全家桶：
* mono
* monodevelop-bin
* mono-msbuild
* mono-msbuild-sdkresolver

```
sudo pacman -S --noconfirm mono
yay -S --noconfirm monodevelop-bin
yay -S --noconfirm mono-msbuild
yay -S --noconfirm mono-msbuild-sdkresolver
```

```c#
// TODO: 不能写死，要通用的方法拉起浏览器
// 拉起默认浏览器
Process.Start("xdg-open", "https://www.google.com");

// Google Chrome
Process.Start("google-chrome-stable", "--app=\"data:text/html,<html><body><script>window.resizeTo(800,600);window.location='" + url + "testprog1.html';</script></body></html>\"");


    if (request.Path.StartsWith("exitProg"))
        System.Environment.Exit(0);


    prog1.exit = function () {
        var data = '';
        data += 'type=utf8&';
        data += 'name=' + 'abc';
        postsomething(data, '/exitProg', function () { });
    }
```