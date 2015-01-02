using QN.Service;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Mvc;
using System.Web.WebPages;
using System.Web.Mvc.Html;
using System.Reflection;

namespace QN
{
    public abstract class ThemeViewPage<TModel> : WebViewPage<TModel>
    {
        private QLang langInstance = QLang.Instance();

        public ThemeViewPage()
        {
        }

        #region 属性定义

        #region 参数获取

        /// <summary>
        /// 获取URL参数
        /// </summary>
        /// <param name="key">键名</param>
        /// <returns></returns>
        public string get(string key)
        {
            return Request.QueryString[key] ?? string.Empty;
        }

        /// <summary>
        /// 获取URL参数
        /// </summary>
        /// <param name="key">键名</param>
        /// <returns></returns>
        public int get<T>(string key)
        {
            int result = 0;

            int.TryParse(get(key), out result);

            return result;
        }

        /// <summary>
        /// 获取表单参数
        /// </summary>
        /// <param name="key">键名</param>
        /// <returns></returns>
        public string form(string key)
        {
            return Request.Form[key] ?? string.Empty;
        }

        /// <summary>
        /// 获取表单参数
        /// </summary>
        /// <param name="key">键名</param>
        /// <returns></returns>
        public int form<T>(string key)
        {
            int result = 0;

            int.TryParse(form(key), out result);

            return result;
        }

        #endregion

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

        private int _datacount;

        /// <summary>
        /// 数据总数
        /// </summary>
        public int datacount { get { return _datacount; } }

        /// <summary>
        /// 页数
        /// </summary>
        private int _pagecount;

        /// <summary>
        /// 页数
        /// </summary>
        public int pagecount { get { return _pagecount; } }

        /// <summary>
        /// 获取当前主题的运行时路径
        /// </summary>
        public string themepath
        {
            get
            {
                return this.root + "sites/" + ThemeService.DomainToDirectoryName(currentsite.domain) + currentsite.theme;
            }
        }

        /// <summary>
        /// 站点基本信息
        /// </summary>
        public site currentsite
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
        /// 当前登录用户
        /// </summary>
        public QUser currentuser { get { return User as QUser; } }

        /// <summary>
        /// 返回上一页的url（已被编码）
        /// </summary>
        public string backurl
        {
            get
            {
                string returnurl = string.Empty;
                if (null != Request.QueryString["returnurl"])
                {
                    returnurl = Request.QueryString["returnurl"];
                }
                else
                {
                    returnurl = Request.UrlReferrer.PathAndQuery;
                }

                return Server.UrlEncode(returnurl);
            }
        }

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
                    return currentsite.name;
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
                    return currentsite.info;
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

        /// <summary>
        /// 当前选中的菜单项标识符
        /// </summary>
        public string activenavitem { get; set; }

        #endregion

        #region 数据

        private readonly SiteService siteService = new SiteService();
        private readonly UserService userService = new UserService();
        private readonly RoleService roleService = new RoleService();
        private readonly TermService termService = new TermService();
        private readonly PostService postService = new PostService();
        private readonly CommentService commentService = new CommentService();

        /// <summary>
        /// 获取站点列表（内部使用querystring的pageindex，和pagesize自动分页）
        /// </summary>
        /// <param name="order">排序表达式</param>
        /// <param name="where">条件表达式</param>
        /// <param name="wherevalue">条件表达式中的命名参数，请使对象的属性名称和参数名称保持一致</param>
        /// <returns></returns>
        public virtual IList<site> sites(string order = null, string where = null, object wherevalue = null)
        {
            return sites(get<int>("pageindex"), get<int>("pagesize"), order, where, wherevalue);
        }

        /// <summary>
        /// 获取站点列表
        /// </summary>
        /// <param name="pageindex">起始页</param>
        /// <param name="pagesize">分页大小</param>
        /// <param name="order">排序表达式</param>
        /// <param name="where">条件表达式</param>
        /// <param name="wherevalue">条件表达式中的命名参数，请使对象的属性名称和参数名称保持一致</param>
        /// <returns></returns>
        public virtual IList<site> sites(int pageindex, int pagesize = 10, string order = null, string where = null, object wherevalue = null)
        {
            return siteService.List(pageindex, pagesize, where, wherevalue, order, out _pagecount, out _datacount);
        }

        /// <summary>
        /// 根据站点ID获取站点详细信息
        /// </summary>
        /// <param name="id">站点ID</param>
        /// <returns></returns>
        public virtual site site(int id)
        {
            return siteService.Get(id);
        }

        /// <summary>
        /// 根据站点域名获取站点详细信息
        /// </summary>
        /// <param name="domain">域名</param>
        /// <returns></returns>
        public virtual site site(string domain)
        {
            return null;
        }

        /// <summary>
        /// 获取用户列表（内部使用QueryString的pageindex，和pagesize自动分页）
        /// </summary>
        /// <param name="order">排序表达式</param>
        /// <param name="where">条件表达式</param>
        /// <param name="wherevalue">条件表达式中的命名参数，请使对象的属性名称和参数名称保持一致</param>
        /// <param name="autopage">指示是否使用分页，如果设置为false，则查询所有</param>
        /// <returns></returns>
        public virtual IList<user> users(string order = null, string where = null, object wherevalue = null, bool autopage = true)
        {
            int index = -1, size = -1;
            if (autopage)
            {
                index = get<int>("pageindex");
                size = get<int>("pagesize");
            }

            return users(index, size, order, where, wherevalue);
        }

        /// <summary>
        /// 获取用户列表
        /// </summary>
        /// <param name="pageindex">起始页</param>
        /// <param name="pagesize">分页大小</param>
        /// <param name="order">排序表达式</param>
        /// <param name="where">条件表达式</param>
        /// <param name="wherevalue">条件表达式中的命名参数，请使对象的属性名称和参数名称保持一致</param>
        /// <returns></returns>
        public virtual IList<user> users(int pagesize, int pageindex = 1, string order = null, string where = null, object wherevalue = null)
        {
            return userService.List(pageindex, pagesize, where, wherevalue, order, out _pagecount, out _datacount);
        }

        /// <summary>
        /// 根据用户ID获取用户详细信息
        /// </summary>
        /// <param name="id">用户ID</param>
        /// <returns></returns>
        public virtual user user(int id)
        {
            return userService.Get(id) ?? new user();
        }

        /// <summary>
        /// 根据用户登录名获取用户详细信息
        /// </summary>
        /// <param name="login">登录名</param>
        /// <returns></returns>
        public virtual user user(string login)
        {
            return null;
        }

        /// <summary>
        /// 获取角色列表（内部使用querystring的pageindex，和pagesize自动分页）
        /// </summary>
        /// <param name="order">排序表达式</param>
        /// <param name="where">条件表达式</param>
        /// <param name="wherevalue">条件表达式中的命名参数，请使对象的属性名称和参数名称保持一致</param>
        /// <param name="autopage">指示是否使用分页，如果设置为false，则查询所有</param>
        /// <returns></returns>
        public virtual IList<role> roles(string order = null, string where = null, object wherevalue = null, bool autopage = true)
        {
            int index = -1, size = -1;
            if (autopage)
            {
                index = get<int>("pageindex");
                size = get<int>("pagesize");
            }

            return roles(index, size, order, where, wherevalue);
        }

        /// <summary>
        /// 获取角色列表
        /// </summary>
        /// <param name="pageindex">起始页</param>
        /// <param name="pagesize">分页大小</param>
        /// <param name="order">排序表达式</param>
        /// <param name="where">条件表达式</param>
        /// <param name="wherevalue">条件表达式中的命名参数，请使对象的属性名称和参数名称保持一致</param>
        /// <returns></returns>
        public virtual IList<role> roles(int pagesize, int pageindex = 1, string order = null, string where = null, object wherevalue = null)
        {
            return roleService.List(pageindex, pagesize, where, wherevalue, order, out _pagecount, out _datacount);
        }

        /// <summary>
        /// 根据角色ID获取角色详细信息
        /// </summary>
        /// <param name="id">角色ID</param>
        /// <returns></returns>
        public virtual role role(int id)
        {
            return roleService.Get(id);
        }

        /// <summary>
        /// 获取角色列表（内部使用querystring的pageindex，和pagesize自动分页）
        /// </summary>
        /// <param name="type">分类类型</param>
        /// <param name="order">排序表达式</param>
        /// <param name="where">条件表达式</param>
        /// <param name="wherevalue">条件表达式中的命名参数，请使对象的属性名称和参数名称保持一致</param>
        /// <param name="autopage">指示是否使用分页，如果设置为false，则查询所有</param>
        /// <returns></returns>
        public virtual IList<term> terms(string type, string order = null, string where = null, object wherevalue = null, bool autopage = true)
        {
            int index = -1, size = -1;
            if (autopage)
            {
                index = get<int>("pageindex");
                size = get<int>("pagesize");
            }

            return terms(type, index, size, order, where, wherevalue);
        }

        /// <summary>
        /// 获取角色列表
        /// </summary>
        /// <param name="pageindex">起始页</param>
        /// <param name="pagesize">分页大小</param>
        /// <param name="order">排序表达式</param>
        /// <param name="where">条件表达式</param>
        /// <param name="wherevalue">条件表达式中的命名参数，请使对象的属性名称和参数名称保持一致</param>
        /// <returns></returns>
        public virtual IList<term> terms(string type, int pagesize, int pageindex = 1, string order = null, string where = null, object wherevalue = null)
        {
            if (!string.IsNullOrWhiteSpace(where))
            {
                where += " and ";
            }
            else
            {
                where = string.Empty;
            }

            where += "type = '" + type + "'";

            return termService.List(pageindex, pagesize, where, wherevalue, order, out _pagecount, out _datacount);
        }

        /// <summary>
        /// 根据分类ID获取分类信息
        /// </summary>
        /// <param name="id">分类ID</param>
        /// <returns></returns>
        public virtual term term(int id)
        {
            return termService.Get(id) ?? new term();
        }

        /// <summary>
        /// 根据分类别名获取分类信息
        /// </summary>
        /// <param name="slug">分类别名</param>
        /// <returns></returns>
        public virtual term term(string slug)
        {
            return null;
        }

        /// <summary>
        /// 获取内容列表（内部使用querystring的pageindex，和pagesize自动分页）
        /// </summary>
        /// <param name="order">排序表达式</param>
        /// <param name="where">条件表达式</param>
        /// <param name="wherevalue">条件表达式中的命名参数，请使对象的属性名称和参数名称保持一致</param>
        /// <param name="posttype">内容类型，可以使用：post，page，media等，默认post</param>
        /// <param name="autopage">指示是否使用分页，如果设置为false，则查询所有</param>
        /// <returns></returns>
        public virtual IList<post> posts(string order = null, string where = null, object wherevalue = null, string posttype = "post", bool autopage = true)
        {
            int index = -1, size = -1;
            if (autopage)
            {
                index = get<int>("pageindex");
                size = get<int>("pagesize");
            }

            return posts(0, index, size, order, where, wherevalue, posttype);
        }

        /// <summary>
        /// 获取内容列表（内部使用querystring的pageindex，和pagesize自动分页）
        /// </summary>
        /// <param name="termid">分类id</param>
        /// <param name="order">排序表达式</param>
        /// <param name="where">条件表达式</param>
        /// <param name="wherevalue">条件表达式中的命名参数，请使对象的属性名称和参数名称保持一致</param>
        /// <param name="autopage">指示是否使用分页，如果设置为false，则查询所有</param>
        /// <returns></returns>
        public virtual IList<post> posts(int termid, string order = null, string where = null, object wherevalue = null, bool autopage = true)
        {
            int index = -1, size = -1;
            if (autopage)
            {
                index = get<int>("pageindex");
                size = get<int>("pagesize");
            }

            return posts(termid, index, size, order, where, wherevalue, null);
        }

        /// <summary>
        /// 获取内容列表
        /// </summary>
        /// <param name="pageindex">起始页</param>
        /// <param name="pagesize">分页大小</param>
        /// <param name="order">排序表达式</param>
        /// <param name="where">条件表达式</param>
        /// <param name="wherevalue">条件表达式中的命名参数，请使对象的属性名称和参数名称保持一致</param>
        /// <param name="posttype">内容类型，可以使用：post，page，media等，默认post</param>
        /// <returns></returns>
        public virtual IList<post> posts(int pagesize, int pageindex = 1, string order = null, string where = null, object wherevalue = null, string posttype = "post")
        {
            return posts(0, pagesize, pageindex, order, where, wherevalue, posttype);
        }

        /// <summary>
        /// 获取内容列表
        /// </summary>
        /// <param name="termid">分类id</param>
        /// <param name="pageindex">起始页</param>
        /// <param name="pagesize">分页大小</param>
        /// <param name="order">排序表达式</param>
        /// <param name="where">条件表达式</param>
        /// <param name="wherevalue">条件表达式中的命名参数，请使对象的属性名称和参数名称保持一致</param>
        /// <returns></returns>
        public virtual IList<post> posts(int termid, int pagesize, int pageindex, string order = null, string where = null, object wherevalue = null)
        {
            return posts(termid, pagesize, pageindex, order, where, wherevalue, null);
        }

        /// <summary>
        /// 获取内容列表
        /// </summary>
        /// <param name="termid">分类id</param>
        /// <param name="pageindex">起始页</param>
        /// <param name="pagesize">分页大小</param>
        /// <param name="order">排序表达式</param>
        /// <param name="where">条件表达式</param>
        /// <param name="wherevalue">条件表达式中的命名参数，请使对象的属性名称和参数名称保持一致</param>
        /// <param name="posttype">内容类型，可以使用：post，page，media等，默认post</param>
        /// <returns></returns>
        public virtual IList<post> posts(int termid, int pagesize, int pageindex, string order, string where = null, object wherevalue = null, string posttype = "post")
        {
            if (string.IsNullOrWhiteSpace(where))
            {
                where = string.Empty;
            }
            else
            {
                where += " and ";
            }

            if (string.IsNullOrWhiteSpace(posttype))
            {
                posttype = "post";
            }

            where += " posttype='" + posttype + "'";

            if (termid > 0)
            {
                where += " and termid = " + termid;
            }

            return postService.List(pageindex, pagesize, where, wherevalue, order, out _pagecount, out _datacount);
        }

        /// <summary>
        /// 根据ID获取内容
        /// </summary>
        /// <param name="id">id</param>
        /// <returns></returns>
        public virtual post post(int id)
        {
            return postService.Get(id);
        }

        /// <summary>
        /// 根据别名获取内容
        /// </summary>
        /// <param name="slug">别名</param>
        /// <returns></returns>
        public virtual post post(string slug)
        {
            return postService.Get(0);
        }

        /// <summary>
        /// 获取评论列表（内部使用querystring的pageindex，和pagesize自动分页）
        /// </summary>
        /// <param name="order">排序表达式</param>
        /// <param name="where">条件表达式</param>
        /// <param name="wherevalue">条件表达式中的命名参数，请使对象的属性名称和参数名称保持一致</param>
        /// <param name="commenttype">评论类型，可以使用：comment，page，media等，默认comment</param>
        /// <param name="autopage">指示是否使用分页，如果设置为false，则查询所有</param>
        /// <returns></returns>
        public virtual IList<comment> comments(int postid = 0, string order = null, string where = null, object wherevalue = null, bool autopage = true)
        {
            int index = -1, size = -1;
            if (autopage)
            {
                index = get<int>("pageindex");
                size = get<int>("pagesize");
            }

            return comments(index, size, postid, order, where, wherevalue);
        }

        /// <summary>
        /// 获取评论列表
        /// </summary>
        /// <param name="pageindex">起始页</param>
        /// <param name="pagesize">分页大小</param>
        /// <param name="order">排序表达式</param>
        /// <param name="where">条件表达式</param>
        /// <param name="wherevalue">条件表达式中的命名参数，请使对象的属性名称和参数名称保持一致</param>
        /// <param name="commenttype">评论类型，可以使用：comment，page，media等，默认comment</param>
        /// <returns></returns>
        public virtual IList<comment> comments(int pagesize, int pageindex = 1, int postid = 0, string order = null, string where = null, object wherevalue = null)
        {
            if (postid > 0)
            {
                if (string.IsNullOrWhiteSpace(where))
                {
                    where = string.Empty;
                }
                else
                {
                    where += " and ";
                }
                where += " postid=" + postid;
            }

            return commentService.List(pageindex, pagesize, where, wherevalue, order, out _pagecount, out _datacount);
        }

        /// <summary>
        /// 根据ID获取评论
        /// </summary>
        /// <param name="id">id</param>
        /// <returns></returns>
        public virtual comment comment(int id)
        {
            return commentService.Get(id);
        }

        #endregion

        #region 辅助方法

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

            result.Append("<script type=\"text/javascript\">window.basepath = '" + this.root + "';</script>");

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

        /// <summary>
        /// 在此处显示一个分页控件
        /// </summary>
        /// <returns></returns>
        public IHtmlString pager()
        {
            int index = get<int>("pageindex");
            int pagesize = get<int>("pagecount");

            if (index < 1)
            {
                index = 1;
            }

            if (pagesize < 10)
            {
                pagesize = 10;
            }

            return pager(new pager()
            {
                datacount = _datacount,
                pagecount = _pagecount,
                pageindex = index,
                pagesize = pagesize
            });
        }

        /// <summary>
        /// 在此处显示一个分页控件
        /// </summary>
        /// <param name="pager">分页详细信息</param>
        /// <returns></returns>
        public IHtmlString pager(pager pager)
        {
            return Html.Partial("pager", pager);
        }

        /// <summary>
        /// 渲染一个图片标签（可以使用相对路径）
        /// </summary>
        /// <param name="url">图片路径</param>
        /// <param name="htmlAttributes">HTML属性</param>
        /// <returns></returns>
        public IHtmlString img(string url, object htmlAttributes)
        {
            if(string.IsNullOrWhiteSpace(url))
            {
                return new MvcHtmlString(string.Empty);
            }

            string resulturl = string.Empty;
            if (url.StartsWith("~"))
            {
                resulturl = Url.Content(url);
            }
            else if (url.StartsWith("/"))
            {
                resulturl = url;
            }
            else
            {
                resulturl = root + url;
            }

            Dictionary<string, string> attrs = new Dictionary<string, string>();

            if (null != htmlAttributes)
            {
                foreach (PropertyInfo p in htmlAttributes.GetType().GetProperties())
                {
                    if (!attrs.Keys.Contains(p.Name))
                    {
                        object val = p.GetValue(htmlAttributes, null);
                        if (null != val)
                        {
                            attrs.Add(p.Name, val.ToString());
                        }
                    }
                }
            }

            string attrStr = string.Join(" ", attrs.Select(m => m.Key + "=" + attrs[m.Key]));

            return new HtmlString(string.Format("<img src='{0}' {1} />", resulturl, attrStr));
        }

        /// <summary>
        /// 渲染一个图片标签（可以使用相对路径）
        /// </summary>
        /// <param name="url">图片路径</param>
        /// <returns></returns>
        public IHtmlString img(string url)
        {
            return img(url, null);
        }

        #endregion
    }
}
