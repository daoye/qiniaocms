using QN.Service;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Mvc;
using System.Web.WebPages;

namespace QN
{
    public abstract class ThemeViewPage<TModel> : WebViewPage<TModel>
    {
        private QLang langInstance = QLang.Instance();

        public ThemeViewPage()
        {
        }

        #region 属性定义

        /// <summary>
        /// 是否登录
        /// </summary>
        public bool logined
        {
            get
            {
                return UserService.IsLogined;
            }
        }

        /// <summary>
        /// 网站根目录
        /// </summary>
        public string root
        {
            get
            {
                string path = HttpContext.Current.Request.ApplicationPath;
                if (!path.EndsWith("/"))
                {
                    path += "/";
                }

                return path;
            }
        }

        /// <summary>
        /// 起始页
        /// </summary>
        public int pageindex
        {
            get
            {
                int _start = 1;
                int.TryParse(HttpContext.Current.Request["start"], out _start);

                if (_start < 1)
                {
                    _start = 1;
                }

                return _start;
            }
        }

        /// <summary>
        /// 分页大小
        /// </summary>
        public int pagesize
        {
            get
            {
                int _pagesize = 20;
                int.TryParse(HttpContext.Current.Request["pagesize"], out _pagesize);

                if (_pagesize < 1)
                {
                    _pagesize = 20;
                }

                return _pagesize;
            }
        }

        /// <summary>
        /// 数据总数
        /// </summary>
        public int datacount { get; private set; }

        /// <summary>
        /// 页数
        /// </summary>
        public int pagecount { get; private set; }

        /// <summary>
        /// 获取当前主题的运行时路径
        /// </summary>
        public string themepath
        {
            get
            {
                return this.root + "sites/" + ThemeService.DomainToDirectoryName(site.domain) + site.theme;
            }
        }

        /// <summary>
        /// 站点基本信息
        /// </summary>
        public site site
        {
            get
            {
                //string key = "siteinfo";
                //Site result = QSession.Get<Site>(key);

                //if (null == result)
                //{
                //    result = SiteService.CurrentSite();
                //    QSession.Set(key, result);
                //}

                //return result;

                return SiteService.CurrentSite();
            }
        }

        /// <summary>
        /// 表示当前登录用户
        /// </summary>
        public QUser quser { get { return User as QUser; } }

        private string _title;

        /// <summary>
        /// 网站标题
        /// </summary>
        public string title
        {
            get
            {
                if (string.IsNullOrWhiteSpace(_title))
                {
                    return site.name;
                }

                return _title;
            }
            set { _title = value; }
        }

        private string _description;

        /// <summary>
        /// 描述信息
        /// </summary>
        public string description
        {
            get
            {
                if (string.IsNullOrWhiteSpace(_description))
                {
                    return site.info;
                }

                return _description;
            }
            set
            {
                _description = value;
            }
        }

        private string _keywords;

        /// <summary>
        /// 关键词
        /// </summary>
        public string keywords
        {
            get
            {
                return _keywords;
            }
            set
            {
                _keywords = value;
            }
        }

        /// <summary>
        /// 程序技术支持信息
        /// </summary>
        public string powered
        {
            get { return " Powered by 奇鸟WEB系统"; }
        }

        /// <summary>
        /// 程序版本号
        /// </summary>
        public string version
        {
            get { return "1.0.0"; }
        }

        /// <summary>
        /// 程序版权信息
        /// </summary>
        public string copyright
        {
            get { return "成都奇鸟软件有限公司 Copyright (c) 2014 版权所有"; }
        }

        #endregion

        public virtual string posts()
        {
            return "哈哈哈";
        }

        /// <summary>
        /// 本地化语言
        /// </summary>
        /// <param name="id">语言Id</param>
        /// <returns></returns>
        public IHtmlString lang(string id)
        {
            return new MvcHtmlString(langInstance.Lang(id));
        }

        /// <summary>
        /// 输出一个或多个包含脚本引用的代码块，请参考<seealso cref="css"/>
        /// </summary>
        /// <param name="paths">可以包含一个或多个文件路径</param>
        public IHtmlString scripts(params string[] paths)
        {
            StringBuilder result = new StringBuilder();

            if (paths.Length != 0)
            {
                string Tstr = "<script src=\"{0}\" type=\"text/javascript\"></script>";
                foreach (string p in paths)
                {
                    result.Append(string.Format(Tstr, this.Url.Content(p)));
                }
            }

            return new MvcHtmlString(result.ToString());
        }

        /// <summary>
        /// 输出一个或多个包含样式表引用的代码块，请参考<seealso cref="scripts"/>
        /// </summary>
        /// <param name="paths">可以包含一个或多个文件路径</param>
        public IHtmlString css(params string[] paths)
        {
            StringBuilder result = new StringBuilder();

            if (paths.Length != 0)
            {
                string Tstr = "<link rel=\"stylesheet\" type=\"text/css\" href=\"{0}\" />";
                foreach (string p in paths)
                {
                    result.Append(string.Format(Tstr, this.Url.Content(p)));
                }
            }

            return new MvcHtmlString(result.ToString());
        }

        /// <summary>
        /// 加载基本脚本库（目前此方法会加载Jquery1.8.2，和一个basePath的JS全局变量）,注：basePath表示网站的根目录
        /// </summary>
        public virtual IHtmlString basejs()
        {
            StringBuilder result = new StringBuilder();

#if DEBUG
            result.Append(scripts("~/Scripts/jquery-1.8.2.js").ToHtmlString());
#else
            result.Append(Scripts("~/Scripts/jquery-1.8.2.min.js").ToHtmlString());
#endif

            result.Append("<script type=\"text/javascript\">window.basePath = '" + this.root + "';</script>");

            return new MvcHtmlString(result.ToString());
        }

        /// <summary>
        /// 加载针对低版本浏览器的Hack脚本和样式
        /// </summary>
        /// <returns></returns>
        public virtual IHtmlString iehack()
        {
            StringBuilder sb = new StringBuilder();
            sb.Append("<!--[if lte IE 6]><script type=\"text/javascript\" src=\"" + root + "Scripts/fuckIE/DD_belatedPNG_0.0.8a-min.js\"></script><script type=\"text/javascript\">window.onload=function(){DD_belatedPNG.fix(\".pngFix,.pngFix:hover\");}</script><![endif]-->");
            sb.Append("<!--[if lte IE 9]>");
            sb.Append("<script type=\"text/javascript\" src=\"" + root + "Scripts/fuckIE/html5shiv.min.js\"></script>");
            sb.Append("<script type=\"text/javascript\" src=\"" + root + "Scripts/fuckIE/selectivizr-min.js\"></script>");
            sb.Append("<script type=\"text/javascript\" src=\"" + root + "Scripts/fuckIE/respond.min.js\"></script>");
            sb.Append("<![endif]-->");

            return new MvcHtmlString(sb.ToString());
        }
    }
}
