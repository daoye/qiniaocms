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
            return List(start, limit, null, null, null, null);
        }

        public IList<post> List(int start, int limit, string where, params object[] whereValues)
        {
            return List(start, limit, where, whereValues, null, null);
        }

        public IList<post> List(int start, int limit, string where, object whereValues, string order, out int pageCount, out int dataCount)
        {
            if (start <= 0)
            {
                start = 1;
            }
            if (limit <= 0)
            {
                limit = 10;
            }

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

            return query.List<post>();
        }

        public int Count(string where, object whereValues)
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
            if (string.IsNullOrWhiteSpace(entity.slug))
            {
                string shortStr = string.Empty;
                while (IsExistsShortId(shortStr = QString.RandStr(5, 10))) ;

                entity.slug = shortStr;
            }
            else
            {
                if (IsExistsShortId(entity.slug))
                {
                    throw new QRunException("缩略名“" + entity.slug + "” 已存在。");
                }
            }

            if (entity.termid <= 0)
            {
                term defaultTerm = R.session.CreateCriteria<term>()
                                            .Add(Expression.Eq("super", true))
                                            .List<term>()
                                            .FirstOrDefault();
                if (null != defaultTerm)
                {
                    entity.termid = defaultTerm.id;
                }
            }

            entity.date = DateTime.Now;
            R.session.Save(entity);
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
                        if (entity.slug != post.slug && IsExistsShortId(post.slug))
                        {
                            throw new QRunException("缩略名“" + entity.slug + "” 已存在。");
                        }
                    }

                    entity.AssigningForm(post);
                    entity.modified = DateTime.Now;

                    if (entity.termid <= 0)
                    {
                        term defaultTerm = R.session.CreateCriteria<term>()
                                                    .Add(Expression.Eq("super", true))
                                                    .List<term>()
                                                    .FirstOrDefault();
                        if (null != defaultTerm)
                        {
                            entity.termid = defaultTerm.id;
                        }
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

                        if (entity.posttype == "file")
                        {
                            //文件类型的话，需要把文件也删除掉
                            string filePath = System.Web.HttpContext.Current.Server.MapPath("~/" + entity.content);
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
            return R.session.CreateCriteria<post>().Add(Expression.Eq("slug", slug)).List<post>().FirstOrDefault();
        }

        /// <summary>
        /// 确定某个短名称是否正在使用
        /// </summary>
        /// <param name="slug">将被确定的短名称</param>
        /// <returns></returns>
        public bool IsExistsShortId(string slug)
        {
            if (string.IsNullOrWhiteSpace(slug))
            {
                throw new QRunException("slug 不能为空。");
            }

            return Get(slug) != null;
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