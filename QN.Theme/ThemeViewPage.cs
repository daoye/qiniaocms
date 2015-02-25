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
using System.IO;
using QN.Plugin;

namespace QN
{

    public abstract class ThemeViewPage<TModel> : WebViewPage<TModel>
    {
        private QLang langInstance = QLang.Instance();

        public ThemeViewPage()
        {
        }

        /// <summary>
        /// 当前正在处理的页面是否是模板页
        /// </summary>
        private bool isthemeview
        {
            get
            {
                bool flag = false;
                if (this.VirtualPath.ToLower().StartsWith("~/sites") || this.VirtualPath.ToLower().StartsWith("~/themes"))
                {
                    flag = true;
                }

                return flag;
            }
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
        /// 起始页
        /// </summary>
        public int pageindex
        {
            get
            {
                int _start = 1;
                int.TryParse(HttpContext.Current.Request["pageindex"], out _start);

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
                int _pagesize = 10;
                int.TryParse(HttpContext.Current.Request["pagesize"], out _pagesize);

                if (_pagesize < 1)
                {
                    _pagesize = 10;
                }

                return _pagesize;
            }
        }

        protected int _datacount;

        /// <summary>
        /// 数据总数
        /// </summary>
        public int datacount { get { return _datacount; } }

        /// <summary>
        /// 页数
        /// </summary>
        protected int _pagecount;

        /// <summary>
        /// 页数
        /// </summary>
        public int pagecount { get { return _pagecount; } }

        /// <summary>
        /// 表示网站客户端根目录
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
        /// 表示当前主题服务端根目录
        /// </summary>
        public string svroot
        {
            get
            {
                if (string.IsNullOrEmpty(this.VirtualPath))
                {
                    return string.Empty;
                }
                else if (this.VirtualPath.ToLower().StartsWith("~/sites"))
                {
                    return "~/sites/" + QConfiger.DomainToDirectoryName(currentsite.firstdomain()) + currentsite.theme;
                }
                else if (this.VirtualPath.ToLower().StartsWith("~/themes"))
                {
                    return "~/themes/" + currentsite.theme;
                }

                return string.Empty;
            }
        }

        /// <summary>
        /// 表示主题客户端根目录
        /// </summary>
        public string themeroot
        {
            get
            {
                string tmp = string.Empty;

                if (this.VirtualPath.ToLower().StartsWith("~/sites"))
                {
                    tmp = "sites/";
                }
                else if (this.VirtualPath.ToLower().StartsWith("~/themes"))
                {
                    tmp = "themes/";
                }
                return this.root + tmp + QConfiger.DomainToDirectoryName(currentsite.firstdomain()) + currentsite.theme;
            }
        }

        /// <summary>
        /// 当前站点的域名
        /// </summary>
        public string currentdomain
        {
            get { return currentsite.firstdomain(); }
        }

        /// <summary>
        /// 网站基本信息
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

                return R.site;
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
                string returnurl = "";
                if (null != Request.QueryString["returnurl"])
                {
                    returnurl = Request.QueryString["returnurl"];
                }
                else if (null != Request.UrlReferrer)
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
            get { return " Powered by 奇鸟CMS"; }
        }

        /// <summary>
        /// 程序版本号
        /// </summary>
        public string version
        {
            get { return R.Version; }
        }

        /// <summary>
        /// 程序版权信息
        /// </summary>
        public string copyright
        {
            get { return "成都奇鸟软件有限公司 Copyright (c) 2014-" + DateTime.Now.Year + " 版权所有"; }
        }

        /// <summary>
        /// 当前选中的菜单项标的标识符
        /// </summary>
        public string activeflag { get { return ViewBag.activeflag; } set { ViewBag.activeflag = value; } }

        /// <summary>
        /// 获取一个唯一的随机ID
        /// </summary>
        public string randid
        {
            get
            {
                return Guid.NewGuid().ToString().Replace("-", "").ToLower();
            }
        }

        #endregion

        #region 数据辅助

        private readonly SiteService siteService = new SiteService();
        private readonly UserService userService = new UserService();
        private readonly RoleService roleService = new RoleService();
        private readonly TermService termService = new TermService();
        private readonly PostService postService = new PostService();
        private readonly CommentService commentService = new CommentService();

        /// <summary>
        /// 获取网站列表（内部使用querystring的pageindex，和pagesize自动分页）
        /// </summary>
        /// <param name="order">排序表达式</param>
        /// <param name="where">条件表达式</param>
        /// <param name="wherevalue">条件表达式中的命名参数，请使对象的属性名称和参数名称保持一致</param>
        /// <param name="autopage">是否自动分页，默认true</param>
        /// <returns></returns>
        public IList<site> sites(string order = null, string where = null, object wherevalue = null, bool autopage = true)
        {
            int index = -1, size = -1;
            if (autopage)
            {
                index = pageindex;
                size = pagesize;
            }

            return sites(size, index, order, where, wherevalue);
        }

        /// <summary>
        /// 获取网站列表
        /// </summary>
        /// <param name="pagesize">分页大小</param>
        /// <param name="pageindex">起始页</param>
        /// <param name="order">排序表达式</param>
        /// <param name="where">条件表达式</param>
        /// <param name="wherevalue">条件表达式中的命名参数，请使对象的属性名称和参数名称保持一致</param>
        /// <returns></returns>
        public IList<site> sites(int pagesize, int pageindex = 1, string order = null, string where = null, object wherevalue = null)
        {
            return siteService.List(pageindex, pagesize, where, wherevalue, order, out _pagecount, out _datacount);
        }

        /// <summary>
        /// 查询符合条件的网站数量
        /// </summary>
        /// <param name="where">条件表达式</param>
        /// <param name="wherevalues">条件表达式中命名参数值</param>
        /// <returns></returns>
        public int sitecount(string where = null, object wherevalues = null)
        {
            return siteService.Count(where, wherevalues);
        }

        /// <summary>
        /// 根据网站ID获取网站详细信息
        /// </summary>
        /// <param name="id">网站ID</param>
        /// <returns></returns>
        public site site(int id)
        {
            return siteService.Get(id);
        }

        /// <summary>
        /// 根据网站域名获取网站详细信息
        /// </summary>
        /// <param name="domain">域名</param>
        /// <returns></returns>
        public site site(string domain)
        {
            return siteService.Get(domain);
        }

        /// <summary>
        /// 获取用户列表（内部使用QueryString的pageindex，和pagesize自动分页）
        /// </summary>
        /// <param name="order">排序表达式</param>
        /// <param name="where">条件表达式</param>
        /// <param name="wherevalue">条件表达式中的命名参数，请使对象的属性名称和参数名称保持一致</param>
        /// <param name="autopage">指示是否使用分页，如果设置为false，则查询所有</param>
        /// <returns></returns>
        public IList<user> users(string order = null, string where = null, object wherevalue = null, bool autopage = true)
        {
            int index = -1, size = -1;
            if (autopage)
            {
                index = pageindex;
                size = pagesize;
            }

            return users(size, index, order, where, wherevalue);
        }

        /// <summary>
        /// 获取用户列表
        /// </summary>
        /// <param name="pagesize">分页大小</param>
        /// <param name="pageindex">起始页</param>
        /// <param name="order">排序表达式</param>
        /// <param name="where">条件表达式</param>
        /// <param name="wherevalue">条件表达式中的命名参数，请使对象的属性名称和参数名称保持一致</param>
        /// <param name="autopage">是否自动分页，默认true</param>
        /// <returns></returns>
        public IList<user> users(int pagesize, int pageindex = 1, string order = null, string where = null, object wherevalue = null, bool autopage = true)
        {
            int index = -1, size = -1;
            if (autopage)
            {
                index = pageindex;
                size = pagesize;
            }

            string innerWhere = "(siteid=" + R.siteid + ") ";
            if (!string.IsNullOrWhiteSpace(where))
            {
                where = innerWhere + " and (" + where + ")";
            }
            else
            {
                where = innerWhere;
            }

            return userService.List(pageindex, pagesize, where, wherevalue, order, out _pagecount, out _datacount);
        }

        /// <summary>
        /// 查询符合条件的用户数量
        /// </summary>
        /// <param name="where">条件表达式</param>
        /// <param name="wherevalues">条件表达式中命名参数值</param>
        /// <returns></returns>
        public int usercount(string where = null, object wherevalues = null)
        {
            string innerWhere = "(siteid=" + R.siteid + ") ";
            if (!string.IsNullOrWhiteSpace(where))
            {
                where = innerWhere + " and (" + where + ")";
            }
            else
            {
                where = innerWhere;
            }

            return userService.Count(where, wherevalues);
        }

        /// <summary>
        /// 根据用户ID获取用户详细信息
        /// </summary>
        /// <param name="id">用户ID</param>
        /// <returns></returns>
        public user user(int id)
        {
            return userService.Get(id) ?? new user();
        }

        /// <summary>
        /// 根据用户登录名获取用户详细信息
        /// </summary>
        /// <param name="login">登录名</param>
        /// <returns></returns>
        public user user(string login)
        {
            return userService.Get(login);
        }

        /// <summary>
        /// 获取角色列表（内部使用querystring的pageindex，和pagesize自动分页）
        /// </summary>
        /// <param name="order">排序表达式</param>
        /// <param name="where">条件表达式</param>
        /// <param name="wherevalue">条件表达式中的命名参数，请使对象的属性名称和参数名称保持一致</param>
        /// <param name="autopage">指示是否使用分页，如果设置为false，则查询所有</param>
        /// <returns></returns>
        public IList<role> roles(string order = null, string where = null, object wherevalue = null, bool autopage = true)
        {
            int index = -1, size = -1;
            if (autopage)
            {
                index = pageindex;
                size = pagesize;
            }

            return roles(size, index, order, where, wherevalue);
        }

        /// <summary>
        /// 获取角色列表
        /// </summary>
        /// <param name="pagesize">分页大小</param>
        /// <param name="pageindex">起始页</param>
        /// <param name="order">排序表达式</param>
        /// <param name="where">条件表达式</param>
        /// <param name="wherevalue">条件表达式中的命名参数，请使对象的属性名称和参数名称保持一致</param>
        /// <returns></returns>
        public IList<role> roles(int pagesize, int pageindex = 1, string order = null, string where = null, object wherevalue = null)
        {
            string innerWhere = "(siteid=" + R.siteid + " or siteid = 0) ";
            if (!string.IsNullOrWhiteSpace(where))
            {
                where = innerWhere + " and (" + where + ")";
            }
            else
            {
                where = innerWhere;
            }

            return roleService.List(pageindex, pagesize, where, wherevalue, order, out _pagecount, out _datacount);
        }

        /// <summary>
        /// 查询符合条件的角色数量
        /// </summary>
        /// <param name="where">条件表达式</param>
        /// <param name="wherevalues">条件表达式中命名参数值</param>
        /// <returns></returns>
        public int rolecount(string where = null, object wherevalues = null)
        {
            string innerWhere = "(siteid=" + R.siteid + " or siteid = 0) ";
            if (!string.IsNullOrWhiteSpace(where))
            {
                where = innerWhere + " and (" + where + ")";
            }
            else
            {
                where = innerWhere;
            }

            return roleService.Count(where, wherevalues);
        }

        /// <summary>
        /// 根据角色ID获取角色详细信息
        /// </summary>
        /// <param name="id">角色ID</param>
        /// <returns></returns>
        public role role(int id)
        {
            return roleService.Get(id);
        }

        /// <summary>
        /// 获取分类列表
        /// </summary>
        /// <param name="type">分类类型</param>
        /// <param name="order">排序表达式</param>
        /// <param name="where">条件表达式</param>
        /// <param name="wherevalue">条件表达式中的命名参数，请使对象的属性名称和参数名称保持一致</param>
        /// <param name="autopage">指示是否使用分页，如果设置为false，则查询所有</param>
        /// <returns></returns>
        public IList<term> terms(string type, string order = null, string where = null, object wherevalue = null, bool autopage = true)
        {
            int index = -1, size = -1;
            if (autopage)
            {
                index = pageindex;
                size = pagesize;
            }

            return terms(type, size, index, order, where, wherevalue);
        }

        /// <summary>
        /// 获取分类列表
        /// </summary>
        /// <param name="pagesize">分页大小</param>
        /// <param name="pageindex">起始页</param>
        /// <param name="order">排序表达式</param>
        /// <param name="where">条件表达式</param>
        /// <param name="wherevalue">条件表达式中的命名参数，请使对象的属性名称和参数名称保持一致</param>
        /// <returns></returns>
        public IList<term> terms(string type, int pagesize, int pageindex = 1, string order = null, string where = null, object wherevalue = null)
        {
            string innerWhere = "(siteid=" + R.siteid + " and type = '" + type + "') ";
            if (!string.IsNullOrWhiteSpace(where))
            {
                where = innerWhere + " and (" + where + ")";
            }
            else
            {
                where = innerWhere;
            }

            return termService.List(pageindex, pagesize, where, wherevalue, order, out _pagecount, out _datacount);
        }

        /// <summary>
        /// 查询符合条件的分类数量
        /// </summary>
        /// <param name="where">条件表达式</param>
        /// <param name="wherevalues">条件表达式中命名参数值</param>
        /// <returns></returns>
        public int termcount(string where = null, object wherevalues = null)
        {
            string innerWhere = "(siteid=" + R.siteid + ") ";
            if (!string.IsNullOrWhiteSpace(where))
            {
                where = innerWhere + " and (" + where + ")";
            }
            else
            {
                where = innerWhere;
            }

            return termService.Count(where, wherevalues);
        }

        /// <summary>
        /// 根据分类ID获取分类信息
        /// </summary>
        /// <param name="id">分类ID</param>
        /// <returns></returns>
        public term term(int id)
        {
            return termService.Get(id) ?? new term();
        }

        /// <summary>
        /// 根据分类别名获取分类信息
        /// </summary>
        /// <param name="slug">分类别名</param>
        /// <returns></returns>
        public term term(string slug)
        {
            return termService.Get(slug) ?? new term();
        }

        /// <summary>
        /// 获取内容列表（内部使用querystring的pageindex，和pagesize自动分页）
        /// </summary>
        /// <param name="order">排序表达式</param>
        /// <param name="where">条件表达式</param>
        /// <param name="wherevalue">条件表达式中的命名参数，请使对象的属性名称和参数名称保持一致</param>
        /// <param name="type">内容类型，可以使用：post，page，media等，默认post</param>
        /// <param name="autopage">指示是否使用分页，如果设置为false，则查询所有</param>
        /// <returns></returns>
        public IList<post> posts(string order = null, string where = null, object wherevalue = null, string type = "post", bool autopage = true)
        {
            int index = -1, size = -1;
            if (autopage)
            {
                index = pageindex;
                size = pagesize;
            }

            return posts(0, size, index, order, where, wherevalue, type);
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
        public IList<post> posts(int termid, string order = null, string where = null, object wherevalue = null, bool autopage = true)
        {
            int index = -1, size = -1;
            if (autopage)
            {
                index = pageindex;
                size = pagesize;
            }

            return posts(termid, size, index, order, where, wherevalue, null);
        }

        /// <summary>
        /// 获取内容列表
        /// </summary>
        /// <param name="pagesize">分页大小</param>
        /// <param name="pageindex">起始页</param>
        /// <param name="order">排序表达式</param>
        /// <param name="where">条件表达式</param>
        /// <param name="wherevalue">条件表达式中的命名参数，请使对象的属性名称和参数名称保持一致</param>
        /// <param name="type">内容类型，可以使用：post，page，media等，默认post</param>
        /// <returns></returns>
        public IList<post> posts(int pagesize, int pageindex, string order = null, string where = null, object wherevalue = null, string type = "post")
        {
            return posts(0, pagesize, pageindex, order, where, wherevalue, type);
        }

        /// <summary>
        /// 获取内容列表
        /// </summary>
        /// <param name="termid">分类id</param>
        /// <param name="pagesize">分页大小</param>
        /// <param name="pageindex">起始页</param>
        /// <param name="order">排序表达式</param>
        /// <param name="where">条件表达式</param>
        /// <param name="wherevalue">条件表达式中的命名参数，请使对象的属性名称和参数名称保持一致</param>
        /// <returns></returns>
        public IList<post> posts(int termid, int pagesize, int pageindex, string order = null, string where = null, object wherevalue = null)
        {
            return posts(termid, pagesize, pageindex, order, where, wherevalue, null);
        }

        /// <summary>
        /// 获取内容列表
        /// </summary>
        /// <param name="termid">分类id</param>
        /// <param name="pagesize">分页大小</param>
        /// <param name="pageindex">起始页</param>
        /// <param name="order">排序表达式</param>
        /// <param name="where">条件表达式</param>
        /// <param name="wherevalue">条件表达式中的命名参数，请使对象的属性名称和参数名称保持一致</param>
        /// <param name="type">内容类型，可以使用：post，page，media等，默认post</param>
        /// <returns></returns>
        public IList<post> posts(int termid, int pagesize, int pageindex, string order, string where = null, object wherevalue = null, string type = "post")
        {
            string innerWhere = "(siteid=" + R.siteid + " and type='" + type + "' ";
            if (termid > 0)
            {
                innerWhere += " and termid = " + termid;
            }

            if (isthemeview)
            {
                innerWhere += " and status = 'publish'";
            }
            innerWhere += ") ";

            if (!string.IsNullOrWhiteSpace(where))
            {
                where = innerWhere + " and (" + where + ")";
            }
            else
            {
                where = innerWhere;
            }

            return postService.List(pageindex, pagesize, where, wherevalue, order, out _pagecount, out _datacount);
        }

        /// <summary>
        /// 查询符合条件的内容数量
        /// </summary>
        /// <param name="where">条件表达式</param>
        /// <param name="wherevalues">条件表达式中命名参数值</param>
        /// <returns></returns>
        public int postcount(string where = null, object wherevalues = null)
        {
            string innerWhere = "(siteid=" + R.siteid;

            if (isthemeview)
            {
                innerWhere += " and status = 'publish'";
            }
            innerWhere += ") ";

            if (!string.IsNullOrWhiteSpace(where))
            {
                where = innerWhere + " and (" + where + ")";
            }
            else
            {
                where = innerWhere;
            }

            return postService.Count(where, wherevalues);
        }

        /// <summary>
        /// 查询符合条件的内容数量
        /// </summary>
        /// <param name="termid">分类id</param>
        /// <param name="where">条件表达式</param>
        /// <param name="wherevalues">条件表达式中命名参数值</param>
        /// <returns></returns>
        public int postcount(int termid, string where = null, object wherevalues = null)
        {
            string innerWhere = "(siteid=" + R.siteid;
            if (termid > 0)
            {
                innerWhere += " and termid = " + termid;
            }

            if (isthemeview)
            {
                innerWhere += " and status = 'publish'";
            }
            innerWhere += ") ";

            if (!string.IsNullOrWhiteSpace(where))
            {
                where = innerWhere + " and (" + where + ")";
            }
            else
            {
                where = innerWhere;
            }

            return postService.Count(where, wherevalues);
        }

        /// <summary>
        /// 根据ID获取内容
        /// </summary>
        /// <param name="id">id</param>
        /// <returns></returns>
        public post post(int id)
        {
            return postService.Get(id);
        }

        /// <summary>
        /// 根据别名获取内容
        /// </summary>
        /// <param name="slug">别名</param>
        /// <returns></returns>
        public post post(string slug)
        {
            return postService.Get(slug);
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
        public IList<comment> comments(int postid = 0, string order = null, string where = null, object wherevalue = null, bool autopage = true)
        {
            int index = -1, size = -1;
            if (autopage)
            {
                index = pageindex;
                size = pagesize;
            }

            return comments(index, size, postid, order, where, wherevalue);
        }

        /// <summary>
        /// 获取评论列表
        /// </summary>
        /// <param name="pagesize">分页大小</param>
        /// <param name="pageindex">起始页</param>
        /// <param name="order">排序表达式</param>
        /// <param name="where">条件表达式</param>
        /// <param name="wherevalue">条件表达式中的命名参数，请使对象的属性名称和参数名称保持一致</param>
        /// <param name="commenttype">评论类型，可以使用：comment，page，media等，默认comment</param>
        /// <returns></returns>
        public IList<comment> comments(int pagesize, int pageindex = 1, int postid = 0, string order = null, string where = null, object wherevalue = null)
        {
            string innerWhere = "(siteid=" + R.siteid;
            if (postid > 0)
            {
                innerWhere += " and postid = " + postid;
            }

            if (isthemeview)
            {
                innerWhere += " and status = 'publish'";
            }
            innerWhere += ") ";

            if (!string.IsNullOrWhiteSpace(where))
            {
                where = innerWhere + " and (" + where + ")";
            }
            else
            {
                where = innerWhere;
            }

            return commentService.List(pageindex, pagesize, where, wherevalue, order, out _pagecount, out _datacount);
        }

        /// <summary>
        /// 查询符合条件的评论数量
        /// </summary>
        /// <param name="where">条件表达式</param>
        /// <param name="wherevalues">条件表达式中命名参数值</param>
        /// <returns></returns>
        public int commentcount(string where = null, object wherevalues = null)
        {
            string innerWhere = "(siteid=" + R.siteid;

            if (isthemeview)
            {
                innerWhere += " and status = 'publish'";
            }
            innerWhere += ") ";

            if (!string.IsNullOrWhiteSpace(where))
            {
                where = innerWhere + " and (" + where + ")";
            }
            else
            {
                where = innerWhere;
            }

            return commentService.Count(where, wherevalues);
        }

        /// <summary>
        /// 查询某内容对应的符合条件的评论数量
        /// </summary>
        /// <param name="postid">postID</param>
        /// <param name="where">条件表达式</param>
        /// <param name="wherevalues">条件表达式中命名参数值</param>
        /// <returns></returns>
        public int commentcount(int postid, string where = null, object wherevalues = null)
        {
            string innerWhere = "(siteid=" + R.siteid;
            if (postid > 0)
            {
                innerWhere += " and postid = " + postid;
            }

            if (isthemeview)
            {
                innerWhere += " and status = 'publish'";
            }
            innerWhere += ") ";

            if (!string.IsNullOrWhiteSpace(where))
            {
                where = innerWhere + " and (" + where + ")";
            }
            else
            {
                where = innerWhere;
            }


            return commentService.Count(where, wherevalues);
        }

        /// <summary>
        /// 根据ID获取评论
        /// </summary>
        /// <param name="id">id</param>
        /// <returns></returns>
        public comment comment(int id)
        {
            return commentService.Get(id);
        }

        /// <summary>
        /// 获取所有菜单
        /// </summary>
        /// <returns></returns>
        public IList<term> navs()
        {
            return termService.List(-1, -1, "type='nav' and siteid=" + R.siteid);
        }

        /// <summary>
        /// 获取指定的菜单，如果id为0，则获取当前默认菜单
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public term nav(int id = 0)
        {
            return termService.GetNav(id);
        }

        /// <summary>
        /// 获取指定的菜单的子项，如果id为0则获取默认菜单的子菜单
        /// </summary>
        /// <param name="termid">菜单ID，默认为0表示当前默认菜单</param>
        /// <param name="parent">父级菜单，默认为0表示只获取第一级，如果要获取全部，此值应该传递-1</param>
        /// <returns></returns>
        public IList<post> navitems(int termid = 0, int parent = -1)
        {
            if (termid <= 0)
            {
                term term = termService.GetNav();

                if (null == term)
                {
                    return new List<post>();
                }

                termid = term.id;
            }

            string where = "termid=:termid and siteid=:siteid";
            if (parent > -1)
            {
                where += " and parent=:parent";
            }

            string order = "order asc";

            int a, b;

            return postService.List(-1, -1, where, new
            {
                termid = termid,
                parent = parent,
                siteid = R.siteid
            }, order, out a, out b);
        }

        #endregion

        #region 辅助方法

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
        public T get<T>(string key)
        {
            try
            {
                return QTypeBuilder<T>.Unbox(get(key));
            }
            catch
            {
                return default(T);
            }
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
        public T form<T>(string key)
        {
            try
            {
                return QTypeBuilder<T>.Unbox(form(key));
            }
            catch
            {
                return default(T);
            }
        }

        /// <summary>
        /// 设置cookie值
        /// </summary>
        /// <param name="key">cookie名称</param>
        /// <param name="value">cookie值</param>
        public void cookie(string key, string value)
        {
            Response.Cookies.Add(new HttpCookie("qn_" + key) { Value = value, Path = "/" });
        }

        /// <summary>
        /// 获取Cookie的值
        /// </summary>
        /// <param name="key">cookie名称</param>
        /// <returns></returns>
        public string cookie(string key)
        {
            HttpCookie cookie = Request.Cookies["qn_" + key];
            if (null != cookie)
            {
                return cookie.Value;
            }

            return string.Empty;
        }

        /// <summary>
        /// 获取Cookie的值
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key">cookie名称</param>
        /// <returns></returns>
        public T cookie<T>(string key)
        {
            return QTypeBuilder<T>.Unbox(cookie(key));
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
                    result.Append(string.Format(Tstr, this.themeurl(p)));
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
                    result.Append(string.Format(Tstr, this.themeurl(p)));
                }
            }

            return new MvcHtmlString(result.ToString());
        }

        /// <summary>
        /// 加载基本脚本库（目前此方法会加载Jquery1.8.2，和一个basePath的JS全局变量）,注：basePath表示网站的根目录
        /// </summary>
        public IHtmlString basejs()
        {
            StringBuilder result = new StringBuilder();

#if DEBUG
            result.Append(scripts("~/Scripts/jquery-1.8.2.js").ToHtmlString());
#else
            result.Append(Scripts("~/Scripts/jquery-1.8.2.min.js").ToHtmlString());
#endif
            result.Append(scripts("~/Scripts/json2.js").ToHtmlString());
            result.Append("<script type=\"text/javascript\">window.basepath = '" + this.root + "';</script>");

            return new MvcHtmlString(result.ToString());
        }

        /// <summary>
        /// 加载针对低版本浏览器的Hack脚本和样式
        /// </summary>
        /// <returns></returns>
        public IHtmlString iehack()
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
        /// 在当前位置呈现导航
        /// </summary>
        /// <param name="termid"></param>
        /// <returns></returns>
        public IHtmlString navbar(int termid = 0)
        {
            return Html.Partial("navbar", new navbar() { termid = termid, parent = 0 });
        }

        /// <summary>
        /// 渲染一个图片标签（可以使用相对路径）
        /// </summary>
        /// <param name="url">图片路径</param>
        /// <param name="htmlAttributes">HTML属性</param>
        /// <returns></returns>
        public IHtmlString img(string url, object htmlAttributes)
        {
            if (string.IsNullOrWhiteSpace(url))
            {
                return new MvcHtmlString(string.Empty);
            }

            string resulturl = themeurl(url);

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

        /// <summary>
        /// 截断字符串（会自动清除掉所有HTML标记）
        /// </summary>
        /// <param name="input">将被截断的字符串</param>
        /// <param name="len">保留字符串的长度</param>
        /// <param name="start">截断起始位置</param>
        /// <returns></returns>
        public string cut(string input, int len, int start = 0)
        {
            if (string.IsNullOrEmpty(input))
            {
                return string.Empty;
            }

            return input.substr(start, len);
        }

        /// <summary>
        /// 返回一个适合在主题中使用的url路径
        /// </summary>
        /// <param name="path">相对地址</param>
        /// <returns></returns>
        public string themeurl(string path)
        {
            if (string.IsNullOrWhiteSpace(path))
            {
                return string.Empty;
            }
            else if (path.StartsWith("~/"))
            {
                return this.Url.Content(path);
            }
            else if (path.StartsWith("/"))
            {
                if (isthemeview)
                {
                    return themeroot + path;
                }
                else
                {
                    return path;
                }
            }
            else
            {
                return root + path;
            }
        }


        /// <summary>
        /// 终止当前页面的执行
        /// </summary>
        public void exit()
        {
            Response.End();
        }

        /// <summary>
        /// 终止当前页面的继续执行，并显示404页面
        /// </summary>
        /// <returns></returns>
        public void write404()
        {
            List<string> pages = new List<string>();
            pages.Add("~" + themeurl("/Views/404.cshtml"));
            pages.Add("~/Views/404.cshtml");

            foreach (string p in pages)
            {
                if (File.Exists(Server.MapPath(p)))
                {
                    string result = this.RenderPage(p).ToHtmlString();
                    QHttp.Write404(result);
                    return;
                }
            }

            throw new HttpException(404, "Page Not Found.");
        }

        /// <summary>
        /// 终止当前页面的处理，并向客户端发送json
        /// </summary>
        /// <param name="content">Json字符</param>
        public void writejson(string content)
        {
            HttpContext.Current.Response.Clear();
            HttpContext.Current.Response.ContentType = "application/json";
            HttpContext.Current.Response.Write(content);
            HttpContext.Current.Response.End();
        }

        /// <summary>
        /// 终止当前页面的处理，并向客户端发送json
        /// </summary>
        /// <param name="content">一个对象，将被序列化为json格式的字符串并发送到客户端</param>
        public void writejson(object content)
        {
            writejson(QJson.Serialize(content));
        }

        /// <summary>
        /// 终止当前页面的处理，并向客户端发送HTML
        /// </summary>
        /// <param name="content">HTML字符串</param>
        public void writehtml(string content)
        {
            HttpContext.Current.Response.Clear();
            HttpContext.Current.Response.ContentType = "text/html";
            HttpContext.Current.Response.Write(content);
            HttpContext.Current.Response.End();
        }

        /// <summary>
        /// 将JSON字符串序列化为对象
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="content">json字符串</param>
        /// <returns></returns>
        public T jsonparse<T>(string content)
        {
            return QJson.Deserialize<T>(content);
        }

        /// <summary>
        /// 将JSON字符串序列化为对象
        /// </summary>
        /// <param name="content">json字符串</param>
        /// <returns></returns>
        public object jsonparse(string content)
        {
            return QJson.Deserialize(content);
        }

        #endregion

        #region 动作

        //已注册的动作列表
        private Dictionary<string, Func<object[], object>> actions = new Dictionary<string, Func<object[], object>>();

        /// <summary>
        /// 执行某个指定的动作
        /// </summary>
        /// <param name="name">动作名称</param>
        /// <param name="paras">参数</param>
        /// <returns></returns>
        public actionresult action(string name, params object[] paras)
        {
            actionresult result = null;

            try
            {
                result = ActionManager.ApplyAction(name, paras);
            }
            catch (Exception ex)
            {
                result.success = false;
                result.message = ex.Message;
            }

            return result;
        }

        #endregion
    }
}