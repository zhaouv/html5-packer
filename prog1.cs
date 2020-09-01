
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
            string platform = getPlatform();

            if (checkChrome())
            {
                string chrome_exe = "chrome.exe";
                if (platform == "Linux")
                {
                    chrome_exe = "google-chrome-stable";
                }
                
                Process.Start(chrome_exe, "--app=\"data:text/html,<html><body><script>window.resizeTo(800,600);window.location='" + url + "app/index.html';</script></body></html>\"");
                
                fsRoute.BasePath = ".";
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
                MessageBox.Show("现在尝试启动默认浏览器", "警告", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                if (platform == "Windows")
                {
                    Process.Start("https://www.google.com");
                }
                else if (platform == "Linux")
                {
                    Process.Start("xdg-open", "https://www.google.com");
                }
                // TODO
                else {
                    MessageBox.Show("TODO: Mac?", "警告", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                }
                
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
            string platform = getPlatform();

            if (platform == "Windows")
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

            else if (platform == "Linux")
            {
                string result = execShell("google-chrome-stable --version");
                if (result.Contains("Google Chrome")) return true;
            }
            return false;
        }


        static private string getPlatform()
        {
            // https://stackoverflow.com/questions/38790802/determine-operating-system-in-net-core
            string windir = Environment.GetEnvironmentVariable("windir");
            if (!string.IsNullOrEmpty(windir) && windir.Contains(@"\") && Directory.Exists(windir))
            {
                return "Windows";
            }
            else if (File.Exists(@"/proc/sys/kernel/ostype"))
            {
                string osType = File.ReadAllText(@"/proc/sys/kernel/ostype");
                if (osType.StartsWith("Linux", StringComparison.OrdinalIgnoreCase))
                {
                    // Note: Android gets here too
                    return "Linux";
                }
                else
                {
                    return "Unknown";
                }
            }
            else if (File.Exists(@"/System/Library/CoreServices/SystemVersion.plist"))
            {
                // Note: iOS gets here too
                return "Mac";
            }
            else
            {
                return "Unknown";
            }
        }

        static private string execShell(string cmd)
        {
            var process = new Process()
            {
                StartInfo = new ProcessStartInfo
                {
                    FileName = "/bin/bash",
                    Arguments = cmd,
                    RedirectStandardOutput = true,
                    UseShellExecute = false,
                    CreateNoWindow = true,
                }
            };
            process.Start();
            string result = process.StandardOutput.ReadToEnd();
            process.WaitForExit();
            return result;
        }
    }

}