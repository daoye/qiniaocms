using QN.Service;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Web;
using System.Web.Mvc;

namespace QN
{
    public abstract class AdminViewPage<TModel> : ThemeViewPage<TModel>
    {
        /// <summary>
        /// 从现有url生成一个新URL，这个URL接收一些参数，如果当前url存在此参数则覆盖，否则新增，并保留原有参数
        /// </summary>
        /// <param name="paras"></param>
        /// <returns></returns>
        public string parameterurl(object paras)
        {
            if (null == paras)
            {
                throw new ArgumentNullException("paras");
            }

            Dictionary<string, string> parameters = new Dictionary<string, string>();

            foreach (PropertyInfo pinfo in paras.GetType().GetProperties())
            {
                if (!parameters.ContainsKey(pinfo.Name))
                {
                    parameters.Add(pinfo.Name.ToLower(), pinfo.GetValue(paras, null).ToString());
                }
            }

            return parameterurl(parameters);
        }

        /// <summary>
        /// 从现有url生成一个新URL，这个URL接收一些参数，如果当前url存在此参数则覆盖，否则新增，并保留原有参数
        /// </summary>
        /// <param name="paras"></param>
        /// <returns></returns>
        public string parameterurl(Dictionary<string, string> parameters)
        {
            if (null == parameters)
            {
                throw new ArgumentNullException("parameters");
            }
            List<string> resultParameters = new List<string>();
            string query = Request.Url.Query.TrimStart('?');

            if (!string.IsNullOrWhiteSpace(query))
            {
                foreach (string p in query.Split('&'))
                {
                    string[] tmp = p.Split('=');
                    string k = tmp[0].ToLower();
                    if (!parameters.ContainsKey(k))
                    {
                        resultParameters.Add(p);
                    }
                }
            }
            foreach (string k in parameters.Keys)
            {
                resultParameters.Add(k + "=" + parameters[k]);
            }


            return Request.Url.AbsolutePath + "?" + string.Join("&", resultParameters.ToArray());
        }

        /// <summary>
        /// 生成一个用来排序的url
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public string orderurl(string orderkey)
        {
            Dictionary<string, string> paras = new Dictionary<string, string>();

            if (null != Request.QueryString["order"])
            {
                if (Request.QueryString["order"].ToLower().EndsWith("asc"))
                {
                    paras.Add("order", orderkey + " desc");
                }
                else
                {
                    paras.Add("order", orderkey + " asc");
                }
            }
            else
            {
                paras.Add("order", orderkey + " asc");
            }

            return parameterurl(paras);
        }

        /// <summary>
        /// 获取一个排序样式class值
        /// </summary>
        /// <param name="orderkey"></param>
        /// <returns></returns>
        public string orderclass(string orderkey)
        {
            string order = Request.QueryString["order"];

            if (!string.IsNullOrWhiteSpace(order))
            {
                order = order.ToLower();
                string[] orderResult = order.Split(' ');

                orderkey = orderkey.ToLower();

                if (orderResult[0] == orderkey)
                {
                    if (orderResult.Length == 1 || orderResult[1] == "asc")
                    {
                        return "sorting_asc";
                    }
                    else
                    {
                        return "sorting_desc";
                    }
                }
            }

            return "sorting";
        }

        public IHtmlString previewpic(string url, string mimetype, object htmlAttributes)
        {
            if (!string.IsNullOrWhiteSpace(mimetype))
            {
                if (mimetype.StartsWith("image"))
                {
                    return img(url, htmlAttributes);
                }
            }

            return new MvcHtmlString(string.Empty);
        }

        public bool isdashboard()
        {
            object action = null;
            object controller = null;
            object area = null;
            if (!this.ViewContext.RequestContext.RouteData.Values.TryGetValue("action", out action))
            {
                return false;
            }
            if (!this.ViewContext.RequestContext.RouteData.Values.TryGetValue("controller", out controller))
            {
                return false;
            }
            if (!this.ViewContext.RequestContext.RouteData.DataTokens.TryGetValue("area", out area))
            {
                return false;
            }

            return string.Compare(action.ToString(), "dashboard", true) == 0
                && string.Compare(controller.ToString(), "home", true) == 0
                && string.Compare(area.ToString(), "admin", true) == 0;
        }

        #region 数据

        private readonly ThemeService themeService = new ThemeService();
        private readonly CarteService carteService = new CarteService();
        private readonly ACLService aclService = new ACLService();
        private readonly OnlineInfoService onlineService = new OnlineInfoService();
        private readonly RoleService roleService = new RoleService();

        /// <summary>
        /// 当前网站的所有主题
        /// </summary>
        /// <returns></returns>
        public IList<theme> themes()
        {
            return themeService.List();
        }

        /// <summary>
        /// 根据目录名称获取主题信息
        /// </summary>
        /// <param name="dirname">目录名称</param>
        /// <returns></returns>
        public theme theme(string dirname)
        {
            return themes().FirstOrDefault(m => string.Compare(m.dirname, dirname, true) == 0);
        }

        /// <summary>
        /// 某个主题的所有可编辑文件
        /// </summary>
        /// <param name="theme"></param>
        /// <returns></returns>
        public List<string> themefiles(string theme)
        {
            return themeService.GetThemeFiles(theme);
        }

        /// <summary>
        /// 后台菜单列表
        /// </summary>
        /// <param name="order">排序表达式</param>
        /// <param name="where">条件表达式</param>
        /// <param name="wherevalue">条件表达式中的命名参数，请使对象的属性名称和参数名称保持一致</param>
        /// <returns></returns>
        public IList<carte> cartes(string order = null, string where = null, object wherevalue = null)
        {
            string innerWhere = "(siteid in (0," + R.siteid + ") and id in (select carteid from acl where roleid = " + currentuser.info.roleid + "))";
            if(!string.IsNullOrWhiteSpace(where))
            {
                innerWhere += " and (" + where + ")";
            }

            int a, b;

            return carteService.List(-1, -1, innerWhere, wherevalue, order, out a, out b);
        }

        ///// <summary>
        ///// 查询符合条件的菜单数量
        ///// </summary>
        ///// <param name="where">条件表达式</param>
        ///// <param name="wherevalues">条件表达式中命名参数值</param>
        ///// <returns></returns>
        //public int cartecount(string where, object wherevalues)
        //{
        //    return carteService.Count(where, wherevalues);
        //}

        /// <summary>
        /// 根据ID获取后台菜单项
        /// </summary>
        /// <param name="id">id</param>
        /// <returns></returns>
        public carte carte(int id)
        {
            return carteService.Get(id);
        }

        /// <summary>
        /// 指定角色的所有权限信息
        /// </summary>
        /// <returns></returns>
        public IList<acl> acls(int roleid)
        {
            return aclService.List(roleid);
        }

        /// <summary>
        /// 根据ID获取指定权限信息
        /// </summary>
        /// <param name="id">id</param>
        /// <returns></returns>
        public acl acl(int id)
        {
            return aclService.Get(id);
        }

        /// <summary>
        /// 获取角色列表，不包含内置角色（内部使用querystring的pageindex，和pagesize自动分页）
        /// </summary>
        /// <param name="order">排序表达式</param>
        /// <param name="where">条件表达式</param>
        /// <param name="wherevalue">条件表达式中的命名参数，请使对象的属性名称和参数名称保持一致</param>
        /// <param name="autopage">指示是否使用分页，如果设置为false，则查询所有</param>
        /// <returns></returns>
        public IList<role> roles_only(string order = null, string where = null, object wherevalue = null, bool autopage = true)
        {
            int index = -1, size = -1;
            if (autopage)
            {
                index = pageindex;
                size = pagesize;
            }

            return roles_only(size, index, order, where, wherevalue);
        }

        /// <summary>
        /// 获取角色列表，不包含内置角色
        /// </summary>
        /// <param name="pagesize">分页大小</param>
        /// <param name="pageindex">起始页</param>
        /// <param name="order">排序表达式</param>
        /// <param name="where">条件表达式</param>
        /// <param name="wherevalue">条件表达式中的命名参数，请使对象的属性名称和参数名称保持一致</param>
        /// <returns></returns>
        public IList<role> roles_only(int pagesize, int pageindex = 1, string order = null, string where = null, object wherevalue = null)
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

            return roleService.List(pageindex, pagesize, where, wherevalue, order, out _pagecount, out _datacount);
        }

        public IList<OnlineContributorDTO> onlinecontributor()
        {
            IList<OnlineContributorDTO> result = null;
            result = QCache.Get<IList<OnlineContributorDTO>>("onlinecontributor-list-cache-id");
            if (null == result)
            {
                result = onlineService.GetContributorList();
                QCache.Set("onlinecontributor-list-cache-id", result, 60 * 24, null);
            }

            return result;

        }

        #endregion
    }
}