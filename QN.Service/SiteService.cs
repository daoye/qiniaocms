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
    /// 网站管理服务
    /// </summary>
    public class SiteService
    {
        private readonly PostService postService = new PostService();
        private readonly UserService userService = new UserService();
        private readonly TermService termService = new TermService();

        /// <summary>
        /// 获取所有网站
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
                        name = R.default_term_id,
                        siteid = site.id,
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
                            roleid = role.id,
                            carteid = ca.id
                        };

                        R.session.Save(acl);
                    }

                    #endregion

                    #region 创建管理员

                    user.siteid = site.id;
                    user.status = R.user_status_nomal;
                    user.date = DateTime.Now;
                    user.roleid = role.id;

                    R.session.Save(user);

                    option superuser = new option()
                    {
                        name = R.superuser_id,
                        siteid = site.id,
                        value = user.id.ToString()
                    };

                    R.session.Save(superuser);

                    #endregion

                    #region 创建主题

                    CreateTheme(site.firstdomain(), site.theme);

                    #endregion

                    #region 创建默认菜单

                    term nav = new term()
                    {
                        date = DateTime.Now,
                        modified = DateTime.Now,
                        name = "默认菜单",
                        siteid = site.id,
                        type = "nav"
                    };

                    R.session.Save(nav);

                    option defaultnav = new option()
                    {
                        name = R.default_nav_id,
                        siteid = site.id,
                        value = nav.id.ToString()
                    };

                    R.session.Save(defaultnav);

                    post navitem = new post()
                    {
                        name = "首页",
                        type = "nav",
                        content = site.firstdomain(),
                        status = R.status_publish,
                        date = DateTime.Now,
                        modified = DateTime.Now,
                        termid = nav.id,
                        siteid = site.id
                    };

                    R.session.SaveOrUpdate(navitem);

                    #endregion

                    trans.Commit();

                    QCache.Remove("site-list");
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

                    entity.AssigningForm(site, "date");
                    R.session.Update(entity);

                    CreateTheme(entity.firstdomain(), entity.theme);

                    trans.Commit();

                    QCache.Remove("site-list");
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
                            #region 删除post、comment、postmeta、commentmeta表信息

                            //删除该网站的所有文章
                            foreach (post p in R.session.CreateCriteria<post>()
                                                        .Add(Expression.Eq("siteid", entity.id))
                                                        .List<post>())
                            {
                                //删除文章的所有评论
                                foreach (comment cmt in R.session.CreateCriteria<comment>()
                                                                 .Add(Expression.Eq("postid", p.id))
                                                                 .List<comment>())
                                {
                                    //删除评论的附加属性
                                    foreach (commentmeta cmtmeta in R.session.CreateCriteria<commentmeta>()
                                                                             .Add(Expression.Eq("commentid", cmt.id))
                                                                             .List<commentmeta>())
                                    {
                                        R.session.Delete(cmtmeta);
                                    }


                                    R.session.Delete(cmt);
                                }

                                //删除文章的附加属性
                                foreach (postmeta pmeta in R.session.CreateCriteria<postmeta>()
                                                                    .Add(Expression.Eq("postid", p.id))
                                                                    .List<postmeta>())
                                {
                                    R.session.Delete(pmeta);
                                }

                                if (p.type == "file")
                                {
                                    //文件类型的话，需要把文件也删除掉
                                    string filePath = System.Web.HttpContext.Current.Server.MapPath("~" + p.content);
                                    if (File.Exists(filePath))
                                    {
                                        File.Delete(filePath);
                                    }
                                }

                                R.session.Delete(p);
                            }

                            #endregion

                            #region 删除term表信息

                            //删除该网站的所有分类
                            foreach (term c in R.session.CreateCriteria<term>()
                                                        .Add(Expression.Eq("siteid", entity.id))
                                                        .List<term>())
                            {
                                R.session.Delete(c);
                            }
                            #endregion

                            #region 删除user、usermeta表信息

                            //删除该网站的所有用户
                            foreach (user c in R.session.CreateCriteria<user>()
                                                             .Add(Expression.Eq("siteid", entity.id))
                                                             .List<user>())
                            {
                                //删除用户扩展属性
                                foreach (usermeta umeta in R.session.CreateCriteria<usermeta>()
                                                             .Add(Expression.Eq("userid", entity.id))
                                                             .List<usermeta>())
                                {
                                    R.session.Delete(umeta);
                                }


                                R.session.Delete(c);
                            }


                            #endregion

                            #region 删除role、acl表信息

                            //删除该网站的所有角色
                            foreach (role c in R.session.CreateCriteria<role>()
                                                             .Add(Expression.Eq("siteid", entity.id))
                                                             .List<role>())
                            {
                                //删除角色的权限
                                foreach (acl ac in R.session.CreateCriteria<acl>()
                                                                 .Add(Expression.Eq("roleid", c.id))
                                                                 .List<acl>())
                                {
                                    R.session.Delete(ac);
                                }

                                R.session.Delete(c);
                            }

                            #endregion

                            #region 删除carte表信息

                            //删除该网站的自定义菜单
                            foreach (carte c in R.session.CreateCriteria<carte>()
                                                             .Add(Expression.Eq("siteid", entity.id))
                                                             .List<carte>())
                            {
                                R.session.Delete(c);
                            }

                            #endregion

                            #region 删除option表信息

                            //删除该网站的所有配置信息
                            foreach (option c in R.session.CreateCriteria<option>()
                                                             .Add(Expression.Eq("siteid", entity.id))
                                                             .List<option>())
                            {
                                R.session.Delete(c);
                            }

                            #endregion

                            #region 删除site、sitemeta表信息

                            //删除该网站的所有扩展属性
                            foreach (sitemeta sm in R.session.CreateCriteria<sitemeta>()
                                                             .Add(Expression.Eq("siteid", entity.id))
                                                             .List<sitemeta>())
                            {
                                R.session.Delete(sm);
                            }

                            R.session.Delete(entity);

                            #endregion

                            #region 删除主题目录

                            //删除该网站的主题
                            string domainPath = QConfiger.DomainToDirectoryName(entity.firstdomain());
                            string sitePath = System.Web.HttpContext.Current.Server.MapPath(Path.Combine("~/site", Path.DirectorySeparatorChar.ToString(), domainPath));
                            if (Directory.Exists(sitePath))
                            {
                                QFile.DeleteDirectory(sitePath);
                            }

                            #endregion
                        }
                    }

                    trans.Commit();

                    QCache.Remove("site-list");
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

        /// <summary>
        /// 判断某个域名是否已被使用
        /// </summary>
        /// <param name="domain"></param>
        /// <returns></returns>
        public bool IsExistsDomain(string domain, int id)
        {
            return R.session.CreateCriteria<site>()
                            .Add(Expression.Like("domain", "%" + domain + "%"))
                            .Add(Expression.Not(Expression.Eq("id", id)))
                            .SetProjection(Projections.RowCount())
                            .UniqueResult<int>() > 0;
        }

        public static void CreateTheme(string domain, string themename)
        {
            string sitePath = Path.Combine(System.Web.HttpContext.Current.Server.MapPath("~/Sites"), QConfiger.DomainToDirectoryName(domain), themename);
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