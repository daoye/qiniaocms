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


        #region 数据

        private readonly ThemeService themeService = new ThemeService();

        /// <summary>
        /// 当前站点的所有主题
        /// </summary>
        /// <returns></returns>
        public IList<theme> themes()
        {
            return themeService.List();
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

        #endregion
    }
}