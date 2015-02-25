#region
/********************************************************************
	This is NOT a freeware, use is subject to license terms.
	
	Copyright:	QiNiaoSoft Copyright (C) 2014 All rights Reserved.
	Created:	2014/12/06
	FileName: 	PostService.cs
	Author:		DaoYe
	History: 	6/12/2014 14:18 By DaoYe
	Purpose:	
*********************************************************************/
#endregion

using QN.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Linq.Dynamic;
using NHibernate;
using NHibernate.Criterion;
using System.IO;

namespace QN.Service
{
    /// <summary>
    /// 文章管理服务
    /// </summary>
    public class PostService
    {
        private readonly CommentService commentService = new CommentService();

        public IList<post> List(int start, int limit = 20)
        {
            return List(start, limit, null, null);
        }

        public IList<post> List(int start, int limit, string where, object whereValues)
        {
            int a, b;
            return List(start, limit, where, whereValues, null, out a, out b);
        }

        public IList<post> List(int start, int limit, string where, object whereValues, string order, out int pageCount, out int dataCount)
        {
            string hql = " from post";
            if (!string.IsNullOrWhiteSpace(where))
            {
                hql += " where " + where;
            }

            if (!string.IsNullOrWhiteSpace(order))
            {
                hql += " order by " + order;
            }
            else
            {
                hql += " order by order asc, modified desc";
            }

            IQuery query = R.session.CreateQuery(hql);

            if (null != whereValues && !string.IsNullOrEmpty(where))
            {
                query.SetProperties(whereValues);
            }

            dataCount = Count(where, whereValues);

            pageCount = 1;

            if (start > 0 && limit > 0)
            {
                if (dataCount > 0)
                {
                    pageCount = dataCount / limit;

                    if (dataCount % limit > 0)
                    {
                        pageCount++;
                    }
                }

                query = query.SetFirstResult((start - 1) * limit)
                             .SetMaxResults(limit);
            }

            return query.List<post>();
        }

        /// <summary>
        /// 查询符合条件的内容数量
        /// </summary>
        /// <param name="where"></param>
        /// <param name="whereValues"></param>
        /// <returns></returns>
        public int Count(string where = null, object whereValues = null)
        {
            string hql = "select count(*)  from post where siteid=" + R.siteid;
            if (!string.IsNullOrWhiteSpace(where))
            {
                hql += " and " + where;
            }

            IQuery countQuery = R.session.CreateQuery(hql);

            if (null != whereValues && !string.IsNullOrEmpty(where))
            {
                countQuery.SetProperties(whereValues);
            }

            return Convert.ToInt32(countQuery.UniqueResult());
        }

        public void Add(post entity)
        {
            using(ITransaction trans = R.session.BeginTransaction())
            {
                try
                {
                    if (string.IsNullOrWhiteSpace(entity.slug))
                    {
                        string shortStr = string.Empty;
                        while (IsExistsSlug(shortStr = QString.RandStr(5, 10), 0)) ;

                        entity.slug = shortStr;
                    }
                    else
                    {
                        if (IsExistsSlug(entity.slug, 0))
                        {
                            throw new QRunException("别名“" + entity.slug + "” 已存在。");
                        }
                    }

                    if (entity.type == "post" && entity.termid <= 0)
                    {
                        entity.termid = opt.get<int>(R.siteid, R.default_term_id);
                    }

                    entity.date = DateTime.Now;
                    R.session.Save(entity);

                    trans.Commit();
                }
                catch
                {
                    trans.Rollback();
                    throw;
                }
            }
        }

        public void Update(post post)
        {
            using (ITransaction trans = R.session.BeginTransaction())
            {
                try
                {
                    post entity = Get(post.id);

                    if (null == entity)
                    {
                        throw new QRunException("将被更新的文章无法找到。");
                    }

                    if (!string.IsNullOrWhiteSpace(post.slug))
                    {
                        if (IsExistsSlug(post.slug, entity.id))
                        {
                            throw new QRunException("别名“" + post.slug + "” 已存在。");
                        }
                    }

                    entity.AssigningForm(post, "date", "type", "siteid");
                    entity.modified = DateTime.Now;

                    if (entity.type == "post" && entity.termid <= 0)
                    {
                        entity.termid = opt.get<int>(R.siteid, R.default_term_id);
                    }

                    R.session.Update(entity);

                    trans.Commit();
                }
                catch
                {
                    trans.Rollback();
                    throw;
                }
            }
        }

        public void Remove(params post[] entitys)
        {
            using (ITransaction trans = R.session.BeginTransaction())
            {
                try
                {
                    foreach(post entity in entitys)
                    {
                        //删除该文章的所有评论
                        foreach (comment c in R.session.CreateCriteria<comment>()
                                                       .Add(Expression.Eq("postid", entity.id))
                                                       .List<comment>())
                        {
                            commentService.Remove(c);
                        }

                        //删除该文章的所有扩展属性
                        foreach (postmeta pm in R.session.CreateCriteria<postmeta>()
                                                         .Add(Expression.Eq("postid", entity.id))
                                                         .List<postmeta>())
                        {
                            R.session.Delete(pm);
                        }

                        if (entity.type == "file")
                        {
                            //文件类型的话，需要把文件也删除掉
                            string filePath = System.Web.HttpContext.Current.Server.MapPath("~" + entity.pic);
                            if (File.Exists(filePath))
                            {
                                File.Delete(filePath);
                            }
                        }

                        R.session.Delete(entity);
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

        public void Remove(int id)
        {
            post entity = Get(id);

            if (null != entity)
            {
                Remove(entity);
            }
        }

        public void Remove(int[] id)
        {
            List<post> posts = new List<post>();

            foreach (int i in id)
            {
                post s = Get(i);
                if (null != s)
                {
                    posts.Add(s);
                }
            }

            Remove(posts.ToArray());
        }

        public post Get(int Id)
        {
            return R.session.Get<post>(Id);
        }

        /// <summary>
        /// 根据缩略名获取文章
        /// </summary>
        /// <param name="slug">文章的缩略名</param>
        /// <returns></returns>
        public post Get(string slug)
        {
            return R.session.CreateCriteria<post>().Add(Expression.Eq("slug", slug))
                                                   .Add(Expression.Eq("siteid", R.siteid))
                                                   .List<post>()
                                                   .FirstOrDefault();
        }

        /// <summary>
        /// 确定某个别名是否正在使用
        /// </summary>
        /// <param name="slug">别名</param>
        /// <param name="id">要被排除的postid</param>
        /// <returns></returns>
        public bool IsExistsSlug(string slug, int id)
        {
            if (string.IsNullOrWhiteSpace(slug))
            {
                throw new QRunException("slug 不能为空。");
            }

            post result = Get(slug);

            if (null == result || result.id == id || result.siteid != R.siteid)
            {
                return false;
            }

            return true;
        }

        /// <summary>
        ///  增加文章的查看次数
        /// </summary>
        /// <param name="id"></param>
        public void AddViewCount(int id)
        {
            post entity = Get(id);

            if (null == entity)
            {
                return;
            }

            string key = (System.Web.HttpContext.Current.Request.UserHostAddress ?? "") + "viewcount" + id;
            if (null != QCache.Get(R.siteid.ToString(), key))
            {
                return;
            }

            entity.viewcount += 1;

            R.session.Update(entity);

            //记录指定ip在24小时内浏览过此文章
            QCache.Set(R.siteid.ToString(), key, "viewed", 24 * 60);
        }

        public void SaveNav(int termid, string termname, IList<navitem> items)
        {
            using (ITransaction trans = R.session.BeginTransaction())
            {
                try
                {
                    SaveNavItem(termid, items);

                    if (!string.IsNullOrWhiteSpace(termname))
                    {
                        term term = R.session.Get<term>(termid);
                        term.name = termname;
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

        public void SaveNavItem(int termid, IList<navitem> items)
        {
            foreach (post p in R.session.CreateCriteria<post>().Add(Expression.Eq("termid", termid)).List<post>())
            {
                R.session.Delete(p);
            }

            RecursiveNav(items, termid, null, 0, new List<post>());
        }

        private void RecursiveNav(IList<navitem> navitems, int termid, string itempid, int postpid, IList<post> result)
        {
            foreach (navitem i in navitems.Where(m => m.parentid == itempid))
            {
                post p = new post()
                {
                    content = i.url,
                    title = i.title,
                    type = "nav",
                    siteid = R.siteid,
                    order = i.order,
                    date = DateTime.Now,
                    modified = DateTime.Now,
                    termid = termid,
                    status = R.status_publish,
                    name = i.name,
                    parent = postpid
                };

                R.session.Save(p);

                result.Add(p);

                RecursiveNav(navitems, termid, i.itemid, p.id, result);
            }
        }

        public static IList<post> RefereshName(IList<post> posts)
        {
            return RefereshName(posts, ((char)0xA0).ToString(), 4);
        }

        public static IList<post> RefereshName(IList<post> posts, string preChar, int charLen)
        {
            return RefereshName(posts, null, 0, preChar, charLen, 0);
        }

        private static IList<post> RefereshName(IList<post> source, IList<post> newlist, int pid, string preChar, int charLen, int deepIndex)
        {
            if (null == newlist)
            {
                newlist = new List<post>();
            }

            foreach (post t in source.Where(m => m.parent == pid))
            {
                post t2 = new post();
                t2.AssigningForm(t);

                string tempName = string.Empty;
                for (int i = 0; i < deepIndex; i++)
                {
                    for (int j = 0; j < charLen; j++)
                    {
                        tempName += preChar;
                    }
                }

                t2.title = tempName + t2.title;

                newlist.Add(t2);

                RefereshName(source, newlist, t2.id, preChar, charLen, ++deepIndex);
                deepIndex--;
            }

            return newlist;
        }
    }
}