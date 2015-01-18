
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using Ionic.Zip;

namespace QN
{
    /// <summary>
    /// Zip 压缩解压辅助类
    /// </summary>
    public static class QZip
    {
        /// <summary>
        /// 压缩文件
        /// </summary>
        /// <param name="srcFile">将被压缩的源文件</param>
        /// <param name="targetFile">被压缩的文件</param>
        public static void Zip(string srcFile, string targetFile)
        {
            if (string.IsNullOrEmpty(srcFile))
                throw new ArgumentNullException(srcFile);

            if (string.IsNullOrEmpty(targetFile))
                throw new ArgumentNullException(targetFile);

            if (!Path.GetExtension(targetFile).ToLower().Equals(".zip"))
                throw new ArgumentNullException("targetFile", "目标文件名必须以.zip结尾。");

            using (ZipFile zip = new ZipFile(System.Text.Encoding.UTF8))
            {
                zip.AddFile(srcFile,"");
                zip.Save(targetFile);
            }
        }

        /// <summary>
        /// 解压缩文件
        /// </summary>
        /// <param name="srcFile">将被解压的Zip压缩文件</param>
        /// <param name="unpackDirectory">目标文件夹</param>
        public static void UnZip(string srcFile, string unpackDirectory)
        {
            if (string.IsNullOrEmpty(srcFile))
                throw new ArgumentNullException(srcFile);

            if (!Path.GetExtension(srcFile).ToLower().Equals(".zip"))
                throw new ArgumentNullException("srcFile", "目标文件名必须以.zip结尾。");

            if (string.IsNullOrEmpty(unpackDirectory))
                throw new ArgumentNullException(unpackDirectory); 

            using (ZipFile zip1 = ZipFile.Read(srcFile))
            {
                foreach (ZipEntry e in zip1)
                {
                    e.Extract(unpackDirectory, ExtractExistingFileAction.OverwriteSilently);
                }
            }
        }

        /// <summary>
        /// 压缩文件夹
        /// </summary>
        /// <param name="srcFolder">将被压缩的源文件夹</param>
        /// <param name="targetFile">目标文件</param>
        public static void ZipFolder(string srcFolder, string targetFile)
        {
            if (string.IsNullOrEmpty(srcFolder))
                throw new ArgumentNullException(srcFolder);

            if (string.IsNullOrEmpty(targetFile))
                throw new ArgumentNullException(targetFile);

            if (!Path.GetExtension(targetFile).ToLower().Equals(".zip"))
                throw new ArgumentNullException("targetFile", "目标文件名必须以.zip结尾。");

            using (ZipFile zip = new ZipFile())
            {
                zip.AddDirectory(srcFolder);
                zip.Save(targetFile);
            }
        }
    }
}
