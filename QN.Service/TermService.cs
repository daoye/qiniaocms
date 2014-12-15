#region
/********************************************************************
	This is NOT a freeware, use is subject to license terms.
	
	Copyright:	QiNiaoSoft Copyright (C) 2014 All rights Reserved.
	Created:	2014/12/06
	FileName: 	ClassifyService.cs
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
    /// 分类管理服务
    /// </summary>
    public class TermService
    {
        /// <summary>
        /// 获取指定分类的子分类
        /// </summary>
        /// <returns></returns>
        public IList<term> List()
        {
            return List(0);
        }

        /// <summary>
        /// 获取指定分类的子分类
        /// </summary>
        /// <param name="parent"></param>
        /// <returns></returns>
        public IList<term> List(int parent)
        {
            ICriteria query = R.session.CreateCriteria<term>();

            query = query.Add(Expression.Eq("siteid", R.siteid));

            if (parent > -1)
            {
                query = query.Add(Expression.Eq("parent", parent));
            }

            return query.List<term>();
        }

        /// <summary>
        /// 根据路径获取分类列表
        /// </summary>
        /// <param name="path">路径，路径的格式形如：/1/2/ </param>
        /// <returns></returns>
        public IList<term> List(string path)
        {
            ICriteria query = R.session.CreateCriteria<term>();

            query = query.Add(Expression.Eq("siteid", R.siteid));

            if (!string.IsNullOrWhiteSpace(path))
            {
                query = query.Add(Expression.Like("deeppath", "/" + path + "/"));
            }

            return query.List<term>();
        }


        public IList<term> List(int start, int limit, string where, object whereValues, string order)
        {
            if (start <= 0)
            {
                start = 1;
            }
            if (limit <= 0)
            {
                limit = 20;
            }

            string hql = "from term";
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

            if (start * limit > 0)
            {
                query = query.SetFirstResult((start - 1) * limit)
                             .SetMaxResults(limit);
            }

            return query.List<term>();
        }

        public void Add(term entity)
        {
            using (ITransaction trans = R.session.BeginTransaction())
            {
                try
                {
                    if (entity.parent == 0)
                    {
                        entity.deep = 1;
                        entity.deeppath += "/" + entity.id + "/";

                        R.session.Save(entity);
                    }
                    else
                    {
                        R.session.Save(entity);

                        term parent = Get(entity.parent);

                        if (null == parent)
                        {
                            throw new QRunException("此分类对应的父分类不存在。");
                        }

                        entity.deep = parent.deep + 1;
                        entity.deeppath = parent.deeppath + entity.id + "/";

                        R.session.Update(entity);
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

        public void Update(term term)
        {
            using (ITransaction trans = R.session.BeginTransaction())
            {
                try
                {
                    term entity = Get(term.id);

                    if (null == entity)
                    {
                        throw new QRunException("将被更新的对象无法找到。");
                    }

                    entity.AssigningForm(term);

                    if (entity.parent == 0)
                    {
                        entity.deep = 1;
                        entity.deeppath += "/" + entity.id + "/";
                    }
                    else
                    {
                        term parent = Get(entity.parent);

                        if (null == parent)
                        {
                            throw new QRunException("此分类对应的父分类不存在。");
                        }

                        entity.deep = parent.deep + 1;
                        entity.deeppath = parent.deeppath + entity.id + "/";
                    }

                    RefereshChildDeepPath(entity);

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
        /// 调整某分类下子分类的deep和deeppath
        /// </summary>
        /// <param name="id"></param>
        private void RefereshChildDeepPath(term parent)
        {
            foreach (term c in R.session.CreateCriteria<term>().Add(Expression.Eq("parent", parent.id)).List<term>())
            {
                c.deep = parent.deep + 1;
                c.deeppath = parent.deeppath + c.id + "/";

                RefereshChildDeepPath(c);
            }
        }

        /// <summary>
        /// 删除分类
        /// </summary>
        /// <param name="entity"></param>
        public void Remove(term entity)
        {
            if (!entity.super)
            {
                using (ITransaction trans = R.session.BeginTransaction())
                {
                    try
                    {
                        //将所有属于该分类的文章移动至默认分类
                        term defaultTerm = R.session.CreateCriteria<term>()
                                             .Add(Expression.Eq("super", true))
                                             .List<term>().FirstOrDefault();

                        if (null != defaultTerm)
                        {
                            foreach (post p in R.session.CreateCriteria<post>().Add(Expression.Eq("termid", entity.id)).List<post>())
                            {
                                p.termid = defaultTerm.id;
                            }
                        }

                        R.session.Delete(entity);

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

        public void Remove(int Id)
        {
            term entity = Get(Id);

            if (null != entity)
            {
                Remove(entity);
            }
        }

        public term Get(int Id)
        {
            return R.session.Get<term>(Id);
        }
    }
}
