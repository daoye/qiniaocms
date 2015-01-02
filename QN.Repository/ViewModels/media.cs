using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace QN
{
    /// <summary>
    /// 表示媒体文件
    /// </summary>
    public class media
    {
        /// <summary>
        /// PostID
        /// </summary>
        public int postid { get; set; }

        /// <summary>
        /// 文件名称
        /// </summary>
        public string name { get; set; }

        /// <summary>
        /// 扩展名
        /// </summary>
        public string extendname { get; set; }

        /// <summary>
        /// 文件url
        /// </summary>
        public string url { get; set; }

        /// <summary>
        /// MimeType
        /// </summary>
        public string mimetype { get; set; }

        /// <summary>
        /// 文件类型
        /// </summary>
        public string classtype { get; set; }
    }
}
