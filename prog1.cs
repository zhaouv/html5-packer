
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Microsoft.Win32;
using System.Diagnostics;
using System.Net;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;
using SimpleHttpServer;
using SimpleHttpServer.Models;
using SimpleHttpServer.RouteHandlers;


//string str = System.Environment.CurrentDirectory;

namespace _prog1
{

    class prog1
    {
        static void Main(string[] args)
        {

            int port = 1055;
            while (portInUse(port)) port++;
            string url = "http://127.0.0.1:" + port + "/";

            if (checkChrome())
            {
                string chrome_exe = "chrome.exe";
                if (platformCode()!=0)
                {
                    chrome_exe = "google-chrome-stable";
                }
                Process.Start(chrome_exe, "--app=\"data:text/html,<html><body><script>window.resizeTo(800,600);window.location='" + url + "app/index.html';</script></body></html>\"");
                
                fsRoute.BasePath = ".";
                HttpServer httpServer = new HttpServer(port, new List<Route>()
                {
                    new Route()
                    {
                        Callable = new FileSystemRouteHandler() {BasePath = "app", ShowDirectories = true}.Handle,
                        UrlRegex = "^/(.*)$",
                        Method = "GET"
                    },
                    new Route()
                    {
                        Callable = fsRoute.route,
                        UrlRegex = "^/(.*)$",
                        Method = "POST"
                    },
                });
                Thread thread = new Thread(new ThreadStart(httpServer.Listen));
                thread.Start();

                Console.WriteLine("已启动服务："+url);
                Console.WriteLine("当前目录："+Path.GetFileName(Directory.GetCurrentDirectory()));
            } 
            else 
            {
                MessageBox.Show("需要安装Chrome浏览器", "警告", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            }
        }
        

        static private bool portInUse(int port)
        {
            IPGlobalProperties ipGlobalProperties = IPGlobalProperties.GetIPGlobalProperties();
            IPEndPoint[] ipEndPoints = ipGlobalProperties.GetActiveTcpListeners();
            foreach (IPEndPoint ipEndPoint in ipEndPoints)
            {
                if (ipEndPoint.Port == port) return true;
            }
            return false;
        }


        static private bool checkChrome()
        {
            if (platformCode()!=0)
            {
                return true;
            }
            RegistryKey browserKeys = Registry.LocalMachine.OpenSubKey(@"SOFTWARE\WOW6432Node\Clients\StartMenuInternet");
            if (browserKeys == null)
                browserKeys = Registry.LocalMachine.OpenSubKey(@"SOFTWARE\Clients\StartMenuInternet");
            string[] names = browserKeys.GetSubKeyNames();
            foreach (string name in names)
            {
                if (name.ToLower().Contains("chrome"))
                    return true;
            }
            return false;
        }

        // 0 win
        // 1 linux
        // 2 mac
        // -1 unknown
        static private int platformCode()
        {
            // https://stackoverflow.com/questions/38790802/determine-operating-system-in-net-core
            string windir = Environment.GetEnvironmentVariable("windir");
            if (!string.IsNullOrEmpty(windir) && windir.Contains(@"\") && Directory.Exists(windir))
            {
                return 0;
            }
            else if (File.Exists(@"/proc/sys/kernel/ostype"))
            {
                string osType = File.ReadAllText(@"/proc/sys/kernel/ostype");
                if (osType.StartsWith("Linux", StringComparison.OrdinalIgnoreCase))
                {
                    // Note: Android gets here too
                    return 1;
                }
                else
                {
                    return -1;
                }
            }
            else if (File.Exists(@"/System/Library/CoreServices/SystemVersion.plist"))
            {
                // Note: iOS gets here too
                return 2;
            }
            else
            {
                return -1;
            }
        }
    }

}