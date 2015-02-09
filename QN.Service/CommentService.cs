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
        public IList<comment> List(int start, int limit = 20)
        {
            return List(start, limit, null, null, null, null);
        }

        public IList<comment> List(int start, int limit, string where, params object[] whereValues)
        {
            return List(start, limit, where, whereValues, null, null);
        }

        public IList<comment> List(int start, int limit, string where, object whereValues, string order, out int pageCount, out int dataCount)
        {
            string hql = " from comment";
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
                hql += " order by date desc";
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

            return query.List<comment>();
        }

        /// <summary>
        /// 查询符合条件的评论数量
        /// </summary>
        /// <param name="where"></param>
        /// <param name="whereValues"></param>
        /// <returns></returns>
        public int Count(string where = null, object whereValues = null)
        {
            string hql = "select count(*) from comment ";

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
        ///添加评论
        /// </summary>
        /// <param name="entity"></param>
        public void Add(comment entity)
        {
            using (ITransaction trans = R.session.BeginTransaction())
            {
                try
                {
                    entity.siteid = R.siteid;

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

        public void Remove(params comment[] entitys)
        {
            using (ITransaction trans = R.session.BeginTransaction())
            {
                try
                {
                    foreach(comment entity in entitys)
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
            comment entity = Get(id);

            if (null != entity)
            {
                Remove(entity);
            }
        }

        public void Remove(int[] id)
        {
            List<comment> comments = new List<comment>();

            foreach (int i in id)
            {
                comment s = Get(i);
                if (null != s)
                {
                    comments.Add(s);
                }
            }

            Remove(comments.ToArray());
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

        public comment Get(int id)
        {
            return R.session.Get<comment>(id);
        }
    }
}