#region
/********************************************************************
	This is NOT a freeware, use is subject to license terms.
	
	Copyright:	QiNiaoSoft Copyright (C) 2014 All rights Reserved.
	Created:	2014/12/06
	FileName: 	CommentService.cs
	Author:		DaoYe
	History: 	6/12/2014 14:19 By DaoYe
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
    /// 评论管理服务
    /// </summary>
    public class CommentService
    {
        /// <summary>
        /// 获取评论列表
        /// </summary>
        /// <returns></returns>
        public IList<comment> List()
        {
            return List(0);
        }

        /// <summary>
        /// 获取评论列表
        /// </summary>
        /// <returns></returns>
        public IList<comment> List(int postId)
        {
            int a, b;
            return List(0, 0, "postid=:postid", new { postid = postId }, null, out a, out b);
        }

        /// <summary>
        /// 获取评论列表
        /// </summary>
        /// <param name="postId">文章ID</param>
        /// <param name="memberId">评论发表人Id</param>
        /// <param name="start">起始页</param>
        /// <param name="limit">分页大小</param>
        /// <param name="where">条件表达式</param>
        /// <param name="whereValues">参数值</param>
        /// <param name="order">排序表达式</param>
        /// <param name="orderValues">参数值</param>
        /// <returns></returns>
        public IList<comment> List(int start, int limit, string where, object whereValues, string order, out int pageCount, out int dataCount)
        {
            string hql = "from comment ";
            if (!string.IsNullOrWhiteSpace(where))
            {
                hql += " where " + where;
            }

            if (!string.IsNullOrWhiteSpace(order))
            {
                hql += " order by " + order;
            }

            IQuery query = R.session.CreateQuery(hql);

            if (null != whereValues)
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

            return query.List<comment>();
        }

        /// <summary>
        ///添加评论
        /// </summary>
        /// <param name="entity"></param>
        public void Add(comment entity)
        {
            using (ITransaction trans = R.session.BeginTransaction())
            {
                try
                {
                    R.session.Save(entity);

                    if (entity.parent == 0)
                    {
                        entity.deep = 1;
                        entity.deeppath = "/" + entity.id + "/";
                    }
                    else
                    {
                        comment parent = Get(entity.parent);

                        if (null == parent)
                        {
                            throw new QRunException("此评论对应的父级不存在。");
                        }

                        entity.deeppath = parent.deeppath + entity.id + "/";
                        entity.deep = parent.deep + 1;
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

        /// <summary>
        /// 修改评论
        /// </summary>
        /// <param name="comment"></param>
        public void Update(comment comment)
        {
            comment entity = Get(comment.id);

            if (null == entity)
            {
                throw new QRunException("将被更新的对象无法找到。");
            }

            entity.AssigningForm(comment);

            R.session.Update(comment);
        }

        public void Remove(comment entity)
        {
            using (ITransaction trans = R.session.BeginTransaction())
            {
                try
                {
                    //删除所有级联评论
                    foreach (comment c in R.session.CreateCriteria<comment>()
                                                    .Add(Expression.Like("deeppath", "/" + entity.id + "/"))
                                                    .List<comment>())
                    {
                        R.session.Delete(c);
                    }

                    //删除该评论的所有扩展属性
                    foreach (commentmeta c in R.session.CreateCriteria<commentmeta>()
                                                        .Add(Expression.Eq("commentid", "/" + entity.id + "/"))
                                                        .List<commentmeta>())
                    {
                        R.session.Delete(c);
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
            comment entity = Get(Id);

            if (null != entity)
            {
                Remove(entity);
            }
        }

        /// <summary>
        /// 审核评论
        /// </summary>
        /// <param name="id"></param>
        /// <param name="status"></param>
        public void Audit(int id, string status)
        {
            comment entity = Get(id);
            entity.status = status;

            R.session.Update(entity);
        }

        public comment Get(int Id)
        {
            return R.session.Get<comment>(Id);
        }
    }
}