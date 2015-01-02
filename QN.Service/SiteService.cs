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
    public enum SiteModifyError
    {
        OK,
        /// <summary>
        /// 绑定的域名已存在
        /// </summary>
        DomainExists
    }

    /// <summary>
    /// 站点管理服务
    /// </summary>
    public class SiteService
    {
        private readonly PostService postService = new PostService();
        private readonly UserService userService = new UserService();
        private readonly TermService termService = new TermService();

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
            IQuery countQuery = R.session.CreateQuery("select count(*) " + hql);

            if (null != whereValues && !string.IsNullOrEmpty(where))
            {
                query.SetProperties(whereValues);
                countQuery.SetProperties(whereValues);
            }

            dataCount = Convert.ToInt32(countQuery.UniqueResult());

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

        public SiteModifyError Add(site site)
        {
            using (ITransaction trans = R.session.BeginTransaction())
            {
                try
                {
                    if (IsExistsDomain(site.domain))
                    {
                        return SiteModifyError.DomainExists;
                    }

                    R.session.Save(site);

                    CreateTheme(site.domain, site.theme);

                    trans.Commit();

                    return SiteModifyError.OK;
                }
                catch
                {
                    trans.Rollback();
                    throw;
                }
            }
        }

        public SiteModifyError Update(site site)
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

                    if (string.Compare(site.domain, entity.domain, true) != 0 && IsExistsDomain(site.domain))
                    {
                        return SiteModifyError.DomainExists;
                    }

                    entity.AssigningForm(site);
                    R.session.Update(entity);

                    CreateTheme(entity.domain, entity.theme);

                    trans.Commit();

                    return SiteModifyError.OK;
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
            foreach (site entity in entitys)
            {
                if (!entity.super && entity.id != R.siteid)
                {
                    using (ITransaction trans = R.session.BeginTransaction())
                    {
                        try
                        {
                            R.session.Delete(entity);

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
                            string domainPath = ThemeService.DomainToDirectoryName(entity.domain);
                            string sitePath = Path.Combine(System.Web.HttpContext.Current.Server.MapPath("~/site"), domainPath);
                            if (Directory.Exists(sitePath))
                            {
                                Directory.Delete(sitePath, true);
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

        /// <summary>
        /// 获取当前站点信息
        /// </summary>
        /// <returns></returns>
        public static site CurrentSite()
        {
            return new site()
            {
                theme = "default",
                domain = "localhost:7777",
                name = "开发中"
            };

            //return R.session.CreateCriteria<site>().Add(Expression.Eq("id", R.siteid)).List<site>().FirstOrDefault();
        }

        /// <summary>
        /// 判断某个域名是否已被使用
        /// </summary>
        /// <param name="domain"></param>
        /// <returns></returns>
        public bool IsExistsDomain(string domain)
        {
            return R.session.CreateCriteria<site>()
                            .Add(Expression.Eq("domain", domain))
                            .SetProjection(Projections.RowCount())
                            .UniqueResult<int>() > 0;
        }

        private void CreateTheme(string domain, string themename)
        {
            string sitePath = Path.Combine(System.Web.HttpContext.Current.Server.MapPath("~/Sites"), ThemeService.DomainToDirectoryName(domain), themename);
            string themePath = Path.Combine(System.Web.HttpContext.Current.Server.MapPath("~/Themes"), themename);
            if (!Directory.Exists(themePath))
            {
                throw new QRunException("主题：" + themename + "不存在，找不到路径：" + themePath);
            }

            //如果不存在此主题，则复制一份
            if (!Directory.Exists(sitePath))
            {
                Directory.CreateDirectory(sitePath);
                QFileHelper.CopyDirectoryAndFiles(sitePath, themePath, false);
            }
        }
    }
}