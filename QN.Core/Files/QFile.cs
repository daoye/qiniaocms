using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace QN
{
    /// <summary>
    /// 文件辅助类
    /// </summary>
    public class QFile
    {
        /// <summary>
        /// 递归复制文件和文件夹
        /// </summary>
        /// <param name="des">目标</param>
        /// <param name="srcDir">源</param>
        /// <param name="isCreateSrcDir">是否创建源目录</param>
        public static void DeepCopy(string des, DirectoryInfo srcDir, bool isCreateSrcDir = false)
        {
            string desPath = string.Empty;

            if (isCreateSrcDir)
            {
                desPath = Path.Combine(des, srcDir.Name);
            }
            else
            {
                desPath = des;
            }

            if (!Directory.Exists(desPath))
            {
                Directory.CreateDirectory(desPath);
            }

            foreach (FileInfo file in srcDir.GetFiles())
            {
                file.CopyTo(Path.Combine(desPath, file.Name), true);
            }
            foreach (DirectoryInfo dirinfo in srcDir.GetDirectories())
            {
                DeepCopy(desPath, dirinfo, true);
            }
        }

        /// <summary>
        /// 递归复制文件和文件夹
        /// </summary>
        /// <param name="des">目标</param>
        /// <param name="srcDir">源</param>
        /// <param name="isCreateSrcDir">是否创建源目录</param>
        public static void DeepCopy(string des, string srcDir, bool isCreateSrcDir = false)
        {
            DeepCopy(des, new DirectoryInfo(srcDir), isCreateSrcDir);
        }

        /// <summary>
        /// 删除目录及文件
        /// </summary>
        /// <param name="path"></param>
        public static void DeleteDirectory(string path)
        {
            if(Directory.Exists(path))
            {
                Directory.Delete(path, true);
            }
        }

        /// <summary>
        /// 过滤路径中的敏感符号，防止读取文件时被跨目录
        /// </summary>
        /// <param name="path">需要过滤的目录</param>
        /// <returns></returns>
        public static string SecurityPath(string path)
        {
            if (string.IsNullOrEmpty(path)) return string.Empty;

            path = new Regex(@"\s").Replace(path, "");//过滤路径中的空格
            path = new Regex(@"(\.+[\\/]+)").Replace(path, "");//过滤路径中的退目录符号

            return path;
        }

    }
}
