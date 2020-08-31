
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

                Process.Start("chrome.exe", "--app=\"data:text/html,<html><body><script>window.resizeTo(800,600);window.location='" + url + "testprog1.html';</script></body></html>\"");
                
                HttpServer httpServer = new HttpServer(port, new List<Route>()
                {
                    new Route()
                    {
                        Callable = new FileSystemRouteHandler() {BasePath = ".", ShowDirectories = true}.Handle,
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
    }

}