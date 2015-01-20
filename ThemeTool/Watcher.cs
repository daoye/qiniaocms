using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace ThemeTool
{
    public static class Watcher
    {
        private static IList<Data> list = null;

        private static string[] filters = new string[]{".aspx",
            ".ascx",".js",".css",".jpg",".png",".jepg",
            ".gif",".bmp",".exe",".rar",".zip",".master",
            ".config",".xml",".json",".swf"
        };

        public static void Run(params Data[] data)
        {
            list = data;

            foreach (Data d in data)
            {
                if (!Directory.Exists(d.Src))
                {
                    continue;
                }

                FileSystemWatcher fsw = new FileSystemWatcher(d.Src);
                fsw.Changed += fsw_Changed;
                fsw.Created += fsw_Created;
                fsw.Deleted += fsw_Deleted;
                fsw.EnableRaisingEvents = true;
                fsw.IncludeSubdirectories = true;
            }
        }

        static void fsw_Deleted(object sender, FileSystemEventArgs e)
        {
            Remove(e);
        }

        static void fsw_Created(object sender, FileSystemEventArgs e)
        {
            Create(e);
        }

        static void fsw_Changed(object sender, FileSystemEventArgs e)
        {
            Create(e);
        }

        private static void Remove(FileSystemEventArgs e)
        {
            string[] p = e.FullPath.Split(Path.DirectorySeparatorChar);
            int index = p.ToList().FindIndex((m) =>
            {
                return string.Compare(m, "themes", true) == 0;
            });

            if (index > -1)
            {
                index++;
                string shortPath = string.Join(Path.DirectorySeparatorChar.ToString(), p.Skip(index + 1).ToArray());
                string srcPath = string.Join(Path.DirectorySeparatorChar.ToString(), p.Take(index + 1).ToArray());

                Data d = list.FirstOrDefault(m => string.Compare(srcPath, m.Src, true) == 0);

                if (null != d)
                {
                    string fullPath = Path.Combine(d.Target, shortPath);

                    if (File.Exists(fullPath))
                    {
                        File.Delete(fullPath);
                    }
                }
            }
        }

        private static void Create(FileSystemEventArgs e)
        {
            string extend = Path.GetExtension(e.Name).ToLower();

            if(!filters.Contains(extend))
            {
                return;
            }

            string[] p = e.FullPath.Split(Path.DirectorySeparatorChar);
            int index = p.ToList().FindIndex((m) =>
            {
                return string.Compare(m, "themes", true) == 0;
            });

            if (index > -1)
            {
                index++;
                string shortPath = string.Join(Path.DirectorySeparatorChar.ToString(), p.Skip(index + 1).ToArray());
                string srcPath = string.Join(Path.DirectorySeparatorChar.ToString(), p.Take(index + 1).ToArray());

                Data d = list.FirstOrDefault(m => string.Compare(srcPath, m.Src, true) == 0);

                if (null != d)
                {
                    string fullPath = Path.Combine(d.Target, shortPath);

                    if (File.Exists(fullPath))
                    {
                        File.Delete(fullPath);
                    }

                    //File.Copy(e.FullPath, fullPath);
                }
            }
        }
    }
}