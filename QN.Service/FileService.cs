using NHibernate;
using NHibernate.Criterion;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace QN.Service
{
    public class FileService
    {
        public string AppRoot
        {
            get
            {
                return AppDomain.CurrentDomain.BaseDirectory;
            }
        }

        /// <summary>
        /// 文件上传保存路径
        /// </summary>
        public string UploadPath
        {
            get
            {
                string p = string.Join(Path.DirectorySeparatorChar.ToString(), new string[] { "content", "upload" });

                return p + Path.DirectorySeparatorChar.ToString();
            }
        }

        /// <summary>
        /// 允许上传的文件后缀
        /// </summary>
        public string[] AllowExtendNames
        {
            get
            {
                return new string[] { 
                    ".png", ".jpg", ".jpeg", ".gif", ".bmp",
                    ".flv", ".swf", ".mkv", ".avi", ".rm", ".rmvb", ".mpeg", ".mpg",
                    ".ogg", ".ogv", ".mov", ".wmv", ".mp4", ".webm", ".mp3", ".wav", ".mid",
                    ".rar", ".zip", ".tar", ".gz", ".7z", ".bz2", ".cab", ".iso",
                    ".doc", ".docx", ".xls", ".xlsx", ".ppt", ".pptx", ".pdf", ".txt", ".md", ".xml"
                };
            }
        }

        public media Upload(byte[] buffer, string filename,string mimetype)
        {
            return Upload(new MemoryStream(buffer), filename, mimetype);
        }

        public media Upload(Stream stream, string filename, string mimetype)
        {
            if (string.IsNullOrWhiteSpace(filename))
            {
                throw new ArgumentNullException("filename");
            }
            if (string.IsNullOrWhiteSpace(mimetype))
            {
                throw new ArgumentNullException("mimetype");
            }
            if (null == stream)
            {
                throw new ArgumentNullException("stream");
            }

            string extendName = Path.GetExtension(filename);
            if (string.IsNullOrWhiteSpace(extendName) || !AllowExtendNames.Contains(extendName))
            {
                throw new QRunException("不允许上传此类型的文件：" + extendName);
            }

            string fullName = string.Empty;

            using (ITransaction trans = R.session.BeginTransaction())
            {
                try
                {
                    post filePost = new post()
                    {
                        posttype = "file",
                        name = Path.GetFileName(filename),
                        title = Path.GetFileNameWithoutExtension(filename),
                        author = UserService.IsLogined ? (UserService.CurrentUser as QUser).info.id.ToString() : null,
                        mimetype = mimetype,
                        date = DateTime.Now,
                        modified = DateTime.Now,
                        siteid = R.siteid
                    };

                    string childPath = DateTime.Now.ToString("yyyyMMdd") + Path.DirectorySeparatorChar.ToString();
                    string fullPath = AppRoot + UploadPath + childPath;
                    if (!Directory.Exists(fullPath))
                    {
                        Directory.CreateDirectory(fullPath);
                    }
                    string newName = Guid.NewGuid().ToString().Replace("-", "") + extendName;
                    fullName = fullPath + newName;

                    using (FileStream fs = new FileStream(fullName, FileMode.Create))
                    {
                        byte[] buffer = new byte[stream.Length];
                        stream.Read(buffer, 0, buffer.Length);
                        fs.Write(buffer, 0, buffer.Length);
                    }

                    string apppath = System.Web.HttpContext.Current.Request.ApplicationPath;
                    if (!apppath.EndsWith("/"))
                    {
                        apppath += "/";
                    }

                    filePost.pic = apppath + (UploadPath + childPath + newName).Replace(Path.DirectorySeparatorChar.ToString(), "/");

                    R.session.Save(filePost);

                    trans.Commit();

                    return new media()
                    {
                        postid = filePost.id,
                        name = filePost.name,
                        extendname = extendName,
                        mimetype = mimetype,
                        url = filePost.pic
                    };
                }
                catch
                {
                    trans.Rollback();

                    try
                    {
                        if (File.Exists(fullName))
                        {
                            File.Delete(fullName);
                        }
                    }
                    catch (Exception e)
                    {
                        QLog.Error(e);
                    }

                    throw;
                }
            }
        }
    }
}