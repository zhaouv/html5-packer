using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using SimpleHttpServer.Models;

using System.Threading.Tasks;

namespace _prog1
{
    class fsRoute
    {
        public static string BasePath = ".";
        private static int exitMark=1;

        public static int delayExit(int time){
            int thismark = ++exitMark;
            Task.Run(async ()=>
            {
                await Task.Delay(time);
                if (thismark==exitMark)System.Environment.Exit(0);
            });
            return thismark;
        }

        public static HttpResponse route(HttpRequest request)
        {
            // Console.WriteLine(request.Content);
            string[] strings = request.Content.Split('&');
            Dictionary<string, string> dictionary = new Dictionary<string, string>();
            foreach (string s in strings)
            {
                // string[] keyvalue = s.Split('=');
                int index = s.IndexOf("=");
                if (index > 0)
                {
                    dictionary.Add(s.Substring(0, index), s.Substring(index+1));
                }
                // dictionary.Add(keyvalue[0], System.Web.HttpUtility.UrlDecode(keyvalue[1], Encoding.UTF8));
            }

            // Console.WriteLine(request.Path);

            if (request.Path.StartsWith("exitProg"))
            {
                delayExit(3000);
                return new HttpResponse()
                {
                    ContentAsUTF8 = "exitProg set.",
                    StatusCode = "200",
                    ReasonPhrase = "OK",
                };
            }

            if (request.Path.StartsWith("cancalExit"))++exitMark;

            if (request.Path.StartsWith("readFile"))
                return readFileHandler(dictionary);

            if (request.Path.StartsWith("writeFile"))
                return writeFileHandler(dictionary);

            if (request.Path.StartsWith("writeMultiFiles"))
                return writeMultiFilesHandler(dictionary);

            if (request.Path.StartsWith("readdir"))
                return readdirHandler(dictionary);

            if (request.Path.StartsWith("makeDir"))
                return makeDirHandler(dictionary);

            if (request.Path.StartsWith("moveFile"))
                return moveFileHandler(dictionary);

            if (request.Path.StartsWith("deleteFile"))
                return deleteFileHandler(dictionary);

            return new HttpResponse()
            {
                ContentAsUTF8 = "Request Not found.",
                ReasonPhrase = "Not Found",
                StatusCode = "404",
            };
            
        }

        private static HttpResponse readFileHandler(Dictionary<String,String> dictionary)
        {
            
            string type = dictionary["type"];
            if (type == null || !type.Equals("base64")) type = "utf8";
            string filename = dictionary["name"];
            if (filename == null || !File.Exists(Path.Combine(BasePath, filename)))
            {
                return new HttpResponse()
                {
                    ContentAsUTF8 = "File Not Exists!",
                    StatusCode = "404",
                    ReasonPhrase = "Not found"
                };
            }
            byte[] bytes = File.ReadAllBytes(Path.Combine(BasePath, filename));
            return new HttpResponse()
            {
                ContentAsUTF8 = type=="base64"?Convert.ToBase64String(bytes):Encoding.UTF8.GetString(bytes),
                StatusCode = "200",
                ReasonPhrase = "OK"
            };
        }

        private static HttpResponse writeFileHandler(Dictionary<String,String> dictionary)
        {
            string type = dictionary["type"];
            if (type == null || !type.Equals("base64")) type = "utf8";
            string filename = dictionary["name"], content = dictionary["value"];
            byte[] bytes;
            if (type == "base64")
                bytes = Convert.FromBase64String(content);
            else
                bytes = Encoding.UTF8.GetBytes(content);
            File.WriteAllBytes(Path.Combine(BasePath, filename), bytes);
            return new HttpResponse()
            {
                ContentAsUTF8 = Convert.ToString(bytes.Length),
                StatusCode = "200",
                ReasonPhrase = "OK"
            };
        }

        private static HttpResponse writeMultiFilesHandler(Dictionary<String, String> dictionary)
        {
            string filename = dictionary["name"], content = dictionary["value"];

            string[] filenames = filename.Split(';'), contents = content.Split(';');
            long length = 0;
            for (int i = 0; i < filenames.Length; ++i)
            {
                if (i >= contents.Length) continue;
                byte[] bytes = Convert.FromBase64String(contents[i]);
                length += bytes.LongLength;
                File.WriteAllBytes(Path.Combine(BasePath, filenames[i]), bytes);
            }

            return new HttpResponse()
            {
                ContentAsUTF8 = Convert.ToString(length),
                StatusCode = "200",
                ReasonPhrase = "OK"
            };
        }

        private static HttpResponse readdirHandler(Dictionary<string, string> dictionary)
        {
            string name = dictionary["name"];
            if (name == null || !Directory.Exists(Path.Combine(BasePath, name)))
            {
                return new HttpResponse()
                {
                    ContentAsUTF8 = "Directory Not Exists!",
                    StatusCode = "404",
                    ReasonPhrase = "Not found"
                };
            }
            
            string content = "[[";

            string[] filenames = Directory.GetFiles(Path.Combine(BasePath, name));
            for (int i = 0; i < filenames.Length; i++) filenames[i] = "\"" + Path.GetFileName(filenames[i]) + "\"";
            content += string.Join(", ", filenames);

            content += "],[";

            string[] dirnames = Directory.GetDirectories(Path.Combine(BasePath, name));
            for (int i = 0; i < dirnames.Length; i++) dirnames[i] = "\"" + Path.GetFileName(dirnames[i]) + "\"";
            content += string.Join(", ", dirnames);

            content += "]]";
            //Console.WriteLine(content);
            return new HttpResponse()
            {
                ContentAsUTF8 = content,
                StatusCode = "200",
                ReasonPhrase = "OK"
            };
        }

        private static HttpResponse makeDirHandler(Dictionary<string, string> dictionary)
        {
            string name = dictionary["name"];
            if (Directory.Exists(Path.Combine(BasePath, name)))
            {
                return new HttpResponse()
                {
                    ContentAsUTF8 = "Directory Already Exists!",
                    StatusCode = "200",
                    ReasonPhrase = "OK"
                };
            }
            Directory.CreateDirectory(Path.Combine(BasePath, name));
            return new HttpResponse()
            {
                ContentAsUTF8 = "Make Directory Success",
                StatusCode = "200",
                ReasonPhrase = "OK"
            };
        }

        private static HttpResponse moveFileHandler(Dictionary<String, String> dictionary)
        {
            string src = dictionary["src"];
            string dest = dictionary["dest"];
            if (src == null || !File.Exists(Path.Combine(BasePath, src)))
            {
                return new HttpResponse()
                {
                    ContentAsUTF8 = "File Not Exists!",
                    StatusCode = "404",
                    ReasonPhrase = "Not found"
                };
            }
            if (dest == null)
            {
                return new HttpResponse()
                {
                    ContentAsUTF8 = "Must Provide Destination!",
                    StatusCode = "404",
                    ReasonPhrase = "Not found"
                };
            }
            File.Move(Path.Combine(BasePath, src), Path.Combine(BasePath, dest));
            return new HttpResponse()
            {
                ContentAsUTF8 = "Move Success",
                StatusCode = "200",
                ReasonPhrase = "OK"
            };
        }

        private static void deleteFile(string path)
        {
            if (File.Exists(path))
            {
                File.Delete(path);
            }
            else if (Directory.Exists(path))
            {
                foreach (string f in Directory.GetFileSystemEntries(path))
                {
                    deleteFile(f);
                }
                Directory.Delete(path);
            }
        }

        private static HttpResponse deleteFileHandler(Dictionary<String, String> dictionary)
        {
            string filename = dictionary["name"];
            if (filename == null || !(File.Exists(Path.Combine(BasePath, filename)) || Directory.Exists(Path.Combine(BasePath, filename))))
            {
                return new HttpResponse()
                {
                    ContentAsUTF8 = "File Not Exists!",
                    StatusCode = "404",
                    ReasonPhrase = "Not found"
                };
            }
            deleteFile(Path.Combine(BasePath, filename));
            return new HttpResponse()
            {
                ContentAsUTF8 = "Delete Success",
                StatusCode = "200",
                ReasonPhrase = "OK"
            };
        }
    }
}
