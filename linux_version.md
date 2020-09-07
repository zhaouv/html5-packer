# Debian系配置

测试系统：Ubuntu 20.04.1 LTS (Focal Fossa)

如果只需要运行C# (.Net)程序，安装`mono`即可：

```
sudo apt-get install mono-complete
```

如果需要编译C# (.Net)程序，需要先添加mono的仓库，然后才能安装

```
sudo apt-key adv --keyserver hkp://keyserver.ubuntu.com:80 --recv-keys 3FA7E0328081BFF6A14DA29AA6A19B38D3D831EF
sudo sh -c 'echo "deb https://download.mono-project.com/repo/ubuntu stable-bionic main" > /etc/apt/sources.list.d/mono-official-stable.list'
sudo apt update
sudo apt install monodevelop
```

# Arch系配置

测试系统：Manjaro (5.7.17-2)

如果只需要运行C# (.Net)程序，安装`mono`即可：

```
sudo pacman -S --noconfirm mono
```

如果需要编译C# (.Net)程序，那还要额外安装全家桶：
* monodevelop-bin
* mono-msbuild
* mono-msbuild-sdkresolver

```
yay -S --noconfirm monodevelop-bin mono-msbuild mono-msbuild-sdkresolver
```

# 汉字显示方框

如果mono运行程序的汉字都是方框，可以看看默认字体是否支持中文，并且检查是否有覆盖设置（例如/etc/fonts/local.conf）。

常见的中文字体如下：
* WenQuanYi Micro Hei （文泉驿）
* Noto Sans CJK
