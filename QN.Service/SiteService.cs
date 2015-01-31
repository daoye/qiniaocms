#region
/********************************************************************
	This is NOT a freeware, use is subject to license terms.
	
	Copyright:	QiNiaoSoft Copyright (C) 2014 All rights Reserved.
	Created:	2014/11/30
	FileName: 	SiteService.cs
	Author:		DaoYe
	History: 	30/11/2014 18:58 By DaoYe
	Purpose:	
*********************************************************************/
#endregion

using QN.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Linq.Dynamic;
using System.IO;
using NHibernate;
using NHibernate.Criterion;

namespace QN.Service
{
    /// <summary>
    /// 站点管理服务
    /// </summary>
    public class SiteService
    {
        private readonly PostService postService = new PostService();
        private readonly UserService userService = new UserService();
        private readonly TermService termService = new TermService();
        private readonly OptionService optionService = new OptionService();

        /// <summary>
        /// 获取所有站点
        /// </summary>
        /// <returns></returns>
        public IList<site> List()
        {
            int a, b;
            return List(-1, -1, null, null, null, out a, out b);
        }

        public IList<site> List(int start, int limit, string where, object whereValues, string order, out int pageCount, out int dataCount)
        {
            if (start <= 0)
            {
                start = 1;
            }
            if (limit <= 0)
            {
                limit = 10;
            }

            string hql = "from site";
            if (!string.IsNullOrWhiteSpace(where))
            {
                hql += " where " + where;
            }

            if (!string.IsNullOrWhiteSpace(order))
            {
                hql += " order by " + order;
            }

            IQuery query = R.session.CreateQuery(hql);

            if (null != whereValues && !string.IsNullOrEmpty(where))
            {
                query.SetProperties(whereValues);
            }

            dataCount = Count(where, whereValues);

            if (limit <= 0)
            {
                pageCount = dataCount > 1 ? 1 : 0;
            }
            else
            {
                if (dataCount > 0)
                {
                    pageCount = dataCount / limit;

                    if (dataCount % limit > 0)
                    {
                        pageCount++;
                    }
                }
                else
                {
                    pageCount = 0;
                }
            }

            if (start * limit > 0)
            {
                query = query.SetFirstResult((start - 1) * limit)
                             .SetMaxResults(limit);
            }

            return query.List<site>();
        }

        /// <summary>
        /// 查询符合条件的网站数量
        /// </summary>
        /// <param name="where"></param>
        /// <param name="whereValues"></param>
        /// <returns></returns>
        public int Count(string where = null, object whereValues = null)
        {
            string hql = "select count(*) from site ";

            if (!string.IsNullOrWhiteSpace(where))
            {
                hql += " where " + where;
            }

            IQuery countQuery = R.session.CreateQuery(hql);

            if (null != whereValues && !string.IsNullOrEmpty(where))
            {
                countQuery.SetProperties(whereValues);
            }

            return Convert.ToInt32(countQuery.UniqueResult());
        }

        /// <summary>
        /// 增加网站
        /// </summary>
        /// <param name="site">网站基本信息</param>
        /// <param name="user">管理员帐户信息</param>
        /// <returns></returns>
        public void Add(site site, user user)
        {
            using (ITransaction trans = R.session.BeginTransaction())
            {
                try
                {
                    R.session.Save(site);

                    #region 创建默认分类

                    term postterm = new term()
                    {
                        name = "默认分类",
                        modified = DateTime.Now,
                        date = DateTime.Now,
                        siteid = site.id,
                        type = "post",
                        slug = "default"
                    };

                    R.session.Save(postterm);

                    option superterm = new option()
                    {
                        name = "super-term-id",
                        siteid = R.siteid,
                        value = postterm.id.ToString()
                    };

                    R.session.Save(superterm);

                    #endregion

                    #region 创建管理员角色及赋予权限

                    role role = new role()
                    {
                        name = "管理员",
                        siteid = site.id
                    };
                    R.session.Save(role);

                    foreach(carte ca in  R.session.CreateCriteria<carte>().List<carte>())
                    {
                        //排除网站管理和角色管理的权限
                        if (string.Compare("Sites", ca.controller, true) == 0
                            || string.Compare("Roles", ca.controller, true) == 0)
                        {
                            continue;
                        }

                        acl acl = new acl()
                        {
                            action = ca.action,
                            area = ca.area,
                            controller = ca.controller,
                            roleid = role.id,
                            value = ca.id.ToString()
                        };

                        R.session.Save(acl);
                    }

                    #endregion

                    #region 创建管理员

                    user.siteid = site.id;
                    user.status = R.user_status_nomal;
                    user.registered = DateTime.Now;
                    user.roleid = role.id;

                    R.session.Save(user);

                    option superuser = new option()
                    {
                        name = "super-user-id",
                        siteid = R.siteid,
                        value = user.id.ToString()
                    };

                    R.session.Save(superuser);

                    #endregion

                    #region 创建主题

                    CreateTheme(site.firstdomain(), site.theme);

                    #endregion

                    trans.Commit();
                }
                catch
                {
                    trans.Rollback();
                    throw;
                }
            }
        }

        public void Update(site site)
        {
            using (ITransaction trans = R.session.BeginTransaction())
            {
                try
                {
                    site entity = Get(site.id);
                    if (null == entity)
                    {
                        throw new QRunException("将被更新的对象无法找到。");
                    }

                    entity.AssigningForm(site);
                    R.session.Update(entity);

                    CreateTheme(entity.firstdomain(), entity.theme);

                    trans.Commit();
                }
                catch
                {
                    trans.Rollback();
                    throw;
                }
            }
        }

        public void Remove(params site[] entitys)
        {
            using (ITransaction trans = R.session.BeginTransaction())
            {
                try
                {
                    foreach (site entity in entitys)
                    {
                        if (Count() > 1 && entity.id != R.siteid)
                        {

                            //删除该网站的所有扩展属性
                            foreach (sitemeta sm in R.session.CreateCriteria<sitemeta>()
                                                             .Add(Expression.Eq("siteid", entity.id))
                                                             .List<sitemeta>())
                            {
                                R.session.Delete(sm);
                            }

                            //删除该网站的所有文章
                            foreach (post p in R.session.CreateCriteria<post>()
                                                             .Add(Expression.Eq("siteid", entity.id))
                                                             .List<post>())
                            {
                                postService.Remove(p);
                            }

                            //删除该网站的所有分类
                            foreach (term c in R.session.CreateCriteria<term>()
                                                             .Add(Expression.Eq("siteid", entity.id))
                                                             .List<term>())
                            {
                                termService.Remove(c);
                            }

                            //删除该网站的主题
                            string domainPath = ThemeService.DomainToDirectoryName(entity.firstdomain());
                            string sitePath = Path.Combine(System.Web.HttpContext.Current.Server.MapPath("~/site"), domainPath);
                            if (Directory.Exists(sitePath))
                            {
                                Directory.Delete(sitePath, true);
                            }

                            //删除该网站的所有用户
                            foreach (user c in R.session.CreateCriteria<user>()
                                                             .Add(Expression.Eq("siteid", entity.id))
                                                             .List<user>())
                            {
                                userService.Remove(c);
                            }

                            R.session.Delete(entity);
                        }
                    }

                    trans.Commit();
                }
                catch
                {
                    trans.Rollback();
                    throw;
                }
            }
        }

        public void Remove(int Id)
        {
            site entity = Get(Id);

            if (null != entity)
            {
                Remove(entity);
            }
        }

        public void Remove(int[] id)
        {
            List<site> sites = new List<site>();

            foreach (int i in id)
            {
                site s = Get(i);
                if (null != s)
                {
                    sites.Add(s);
                }
            }

            Remove(sites.ToArray());
        }

        public site Get(int Id)
        {
            return R.session.Get<site>(Id);
        }

        ///// <summary>
        ///// 获取当前站点信息
        ///// </summary>
        ///// <returns></returns>
        //public static site CurrentSite()
        //{
        //    return R.session.CreateCriteria<site>().Add(Expression.Eq("id", R.siteid)).List<site>().FirstOrDefault();
        //}

        /// <summary>
        /// 判断某个域名是否已被使用
        /// </summary>
        /// <param name="domain"></param>
        /// <returns></returns>
        public bool IsExistsDomain(string domain, int id)
        {
            return R.session.CreateCriteria<site>()
                            .Add(Expression.Eq("domain", domain))
                            .Add(Expression.Not(Expression.Eq("id", id)))
                            .SetProjection(Projections.RowCount())
                            .UniqueResult<int>() > 0;
        }

        public static void CreateTheme(string domain, string themename)
        {
            string sitePath = Path.Combine(System.Web.HttpContext.Current.Server.MapPath("~/Sites"), ThemeService.DomainToDirectoryName(domain), themename);
            string themePath = Path.Combine(System.Web.HttpContext.Current.Server.MapPath("~/Themes"), themename);
            if (!Directory.Exists(sitePath) && !Directory.Exists(themePath))
            {
                throw new QRunException("主题：" + themename + "不存在，找不到路径：" + themePath);
            }

            //如果不存在此主题，则复制一份
            if (!Directory.Exists(sitePath))
            {
                Directory.CreateDirectory(sitePath);
                QFile.DeepCopy(sitePath, themePath, false);
            }
        }
    }
}