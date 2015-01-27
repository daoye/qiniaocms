using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace QN
{
    /// <summary>
    /// 微博
    /// </summary>
    public class OnlineMicroblogDTO
    {
        /// <summary>
        /// 微博地址
        /// </summary>
        public string Url { get; set; }

        /// <summary>
        /// 微博内容
        /// </summary>
        public string Content { get; set; }

        /// <summary>
        /// 发表日期
        /// </summary>
        public string Date { get; set; }

        /// <summary>
        /// 微博图片
        /// </summary>
        public string Picture { get; set; }

        /// <summary>
        /// 是否包含转发内容
        /// </summary>
        public bool IsHaveForword { get; set; }

        /// <summary>
        /// 转发内容地址
        /// </summary>
        public string SourceUrl { get; set; }

        /// <summary>
        /// 转发内容
        /// </summary>
        public string SourceContent { get; set; }

        /// <summary>
        /// 转发内容的发表日期
        /// </summary>
        public string SourceDate { get; set; }

        /// <summary>
        /// 转发内容的图片
        /// </summary>
        public string SourcePicture { get; set; }
    }
}
