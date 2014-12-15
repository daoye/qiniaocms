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

namespace QN.Service
{
    /// <summary>
    /// 文章管理服务
    /// </summary>
    public class PostService
    {
        private readonly CommentService commentService = new CommentService();

        /// <summary>
        /// 获取文章列表
        /// </summary>
        /// <returns></returns>
        public IList<post> List()
        {
            return List(0, 0, 0);
        }

        public IList<post> List(int termid, int start, int limit)
        {
            int a, b;
            return List(start, limit, termid >= 0 ? "termid=:termid" : null, new { termid = termid }, null, out a, out b);
        }

        public IList<post> List(int start, int limit, string where, object whereValues, string order, out int pageCount, out int dataCount)
        {
            if (start <= 0)
            {
                start = 1;
            }
            if (limit <= 0)
            {
                limit = 20;
            }

            string hql = "from post";
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


            dataCount = query.UniqueResult<int>();

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

            R.session.Save(entity);
        }

        public void Update(post post)
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
        }

        public void Remove(post entity)
        {
            using (ITransaction trans = R.session.BeginTransaction())
            {
                try
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
            post entity = Get(Id);

            if (null != entity)
            {
                Remove(entity);
            }
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
    }
}