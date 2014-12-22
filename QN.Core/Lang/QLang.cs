using QN.Lang;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace QN
{
    /// <summary>
    /// 本地化语言辅助类
    /// </summary>
    public class QLang
    {
        private static string[] _resourceFilePaths;

        private IList<IList<LangModel>> models = new List<IList<LangModel>>();

        /// <summary>
        /// 使用一系列语言文件路径实例化一个语言辅助类，语言文件的优先级与数组索引一致
        /// </summary>
        /// <param name="resourceFiles"></param>
        public QLang(string[] resourceFiles)
        {
            foreach (string p in resourceFiles)
            {
                if (!File.Exists(p))
                {
                    QLog.Error(new FileNotFoundException(string.Format("{0} File not found.", p)));

                    continue;
                }

                using (StreamReader reader = new StreamReader(p))
                {
                    string content = reader.ReadToEnd();

                    try
                    {
                        models.Add(QJson.Deserialize<IList<LangModel>>(content));
                    }
                    catch (Exception e)
                    {
                        QLog.Error(new InvalidCastException("Can't deserialize language Model using the resource file content, maybe the text format is error (just supported JSON).", e));
                    }
                }
            }
        }

        /// <summary>
        /// 根据ID，加载语言，如果找不到指定ID对应的本地化语言，则返回ID本身
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public string Lang(string id)
        {
            string result = id;

            foreach (IList<LangModel> list in models)
            {
                LangModel model = list.FirstOrDefault(m => m.id.Equals(id));

                if (null != model)
                {
                    result = model.value;
                    break;
                }
            }

            return result;
        }

        /// <summary>
        /// 初始化资源辅助类
        /// </summary>
        /// <param name="resourceFiles">可能存在语言资源文件的目录，有限顺序与数组的索引一致</param>
        public static void Intlize(string[] resourceFilePaths)
        {
            if (null == resourceFilePaths)
            {
                throw new ArgumentNullException("resourceFilePaths");
            }

            _resourceFilePaths = resourceFilePaths;
        }

        /// <summary>
        /// 使用当前线程的语言文化实例化一个语言辅助类
        /// </summary>
        /// <returns></returns>
        public static QLang Instance()
        {
            string culture = System.Threading.Thread.CurrentThread.CurrentCulture.Name;

            return Instance(culture);
        }

        /// <summary>
        /// 使用指定的语言文化实例化一个语言辅助类
        /// </summary>
        /// <param name="culture"></param>
        /// <returns></returns>
        public static QLang Instance(string culture)
        {
            List<string> files = new List<string>();

            foreach (string p in _resourceFilePaths)
            {
                string path = Path.Combine(p, culture + ".lang");
                if (File.Exists(path))
                {
                    files.Add(path);
                }
            }

            return new QLang(files.ToArray());
        }
    }
}