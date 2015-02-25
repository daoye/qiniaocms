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
        public IList<term> List(int start, int limit = 20)
        {
            return List(start, limit, null, null, null, null);
        }

        public IList<term> List(int start, int limit, string where, params object[] whereValues)
        {
            int a, b;
            return List(start, limit, where, whereValues, null, out a, out b);
        }

        public IList<term> List(int start, int limit, string where, object whereValues, string order, out int pageCount, out int dataCount)
        {
            string hql = " from term";
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
                hql += " order by id desc";
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

            return query.List<term>();
        }

        /// <summary>
        /// 查询符合条件的分类数量
        /// </summary>
        /// <param name="where"></param>
        /// <param name="whereValues"></param>
        /// <returns></returns>
        public int Count(string where = null, object whereValues = null)
        {
            string hql = "select count(*) from term ";

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

        public void Add(term entity)
        {
            using (ITransaction trans = R.session.BeginTransaction())
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

                    entity.date = DateTime.Now;
                    entity.modified = DateTime.Now;
                    entity.siteid = R.siteid;

                    if (entity.parent == 0)
                    {
                        R.session.Save(entity);

                        entity.deep = 1;
                        entity.deeppath += "/" + entity.id + "/";

                        R.session.Update(entity);
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

        /// <summary>
        /// 添加菜单
        /// </summary>
        /// <param name="entity"></param>
        public void AddNav(term entity)
        {
            using (ITransaction trans = R.session.BeginTransaction())
            {
                try
                {
                    entity.date = DateTime.Now;
                    entity.modified = DateTime.Now;
                    entity.siteid = R.siteid;

                    R.session.Save(entity);

                    entity.deep = 1;
                    entity.deeppath += "/" + entity.id + "/";

                    R.session.Update(entity);

                    string defaultID = opt.get(R.siteid, R.default_nav_id);
                    if (string.IsNullOrEmpty(defaultID) || "0".Equals(defaultID))
                    {
                        opt.set(R.siteid, R.default_nav_id, entity.id.ToString());
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

                    if (!string.IsNullOrWhiteSpace(term.slug))
                    {
                        if (IsExistsSlug(term.slug, entity.id))
                        {
                            throw new QRunException("别名“" + term.slug + "” 已存在。");
                        }
                    }

                    entity.AssigningForm(term, new string[] { "siteid", "count", "super" });

                    entity.modified = DateTime.Now;

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
        public void Remove(params term[] entitys)
        {
            int superid = 0;
            int.TryParse(opt.get(R.siteid, R.default_term_id), out superid);

            using (ITransaction trans = R.session.BeginTransaction())
            {
                try
                {
                    foreach (term entity in entitys)
                    {
                        if (entity.id != superid)
                        {
                            if (entity.type == "post")
                            {
                                //将所有属于该分类的文章移动至默认分类
                                term defaultTerm = R.session.CreateCriteria<term>()
                                                     .Add(Expression.Eq("id", superid))
                                                     .List<term>().FirstOrDefault();

                                if (null != defaultTerm)
                                {
                                    foreach (post p in R.session.CreateCriteria<post>().Add(Expression.Eq("termid", entity.id)).List<post>())
                                    {
                                        p.termid = defaultTerm.id;
                                    }
                                }
                            }
                            else if (entity.type == "album")
                            {
                                foreach (post p in R.session.CreateCriteria<post>().Add(Expression.Eq("termid", entity.id)).List<post>())
                                {
                                    R.session.Delete(p);
                                }
                            }
                            else if (entity.type == "nav")
                            {
                                //删除菜单下的所有菜单项
                                foreach (post p in R.session.CreateCriteria<post>().Add(Expression.Eq("termid", entity.id)).List<post>())
                                {
                                    R.session.Delete(p);
                                }

                                //如果当前被删除的菜单是默认菜单，则重新指定一个默认菜单
                                if (entity.id.ToString() == opt.get(R.siteid, R.default_nav_id))
                                {
                                    term newDef = R.session.CreateCriteria<term>()
                                                           .Add(Expression.Eq("type", "nav"))
                                                           .Add(Expression.Eq("siteid", R.siteid))
                                                           .Add(Expression.Not(Expression.Eq("id", entity.id)))
                                                           .SetMaxResults(1)
                                                           .List<term>()
                                                           .FirstOrDefault();

                                    int defaultNavid = 0;
                                    if (null != newDef)
                                    {
                                        defaultNavid = newDef.id;
                                    }
                                    opt.set(R.siteid, R.default_nav_id, defaultNavid.ToString());
                                }
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

        public void Remove(int id)
        {
            term entity = Get(id);

            if (null != entity)
            {
                Remove(entity);
            }
        }

        public void Remove(int[] id)
        {
            List<term> terms = new List<term>();

            foreach (int i in id)
            {
                term s = Get(i);
                if (null != s)
                {
                    terms.Add(s);
                }
            }

            Remove(terms.ToArray());
        }

        public term Get(int id)
        {
            return R.session.Get<term>(id);
        }

        /// <summary>
        /// 根据别名获取分类
        /// </summary>
        /// <param name="slug">别名</param>
        /// <returns></returns>
        public term Get(string slug)
        {
            return R.session.CreateCriteria<term>()
                            .Add(Expression.Eq("slug", slug))
                            .Add(Expression.Eq("siteid", R.siteid))
                            .List<term>().FirstOrDefault();
        }

        /// <summary>
        /// 获取导航菜单，如果id为0，表示默认菜单
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public term GetNav(int id = 0)
        {
            if (id > 0)
            {
                return Get(id);
            }
            else
            {
                string value = opt.get(R.siteid, R.default_nav_id);
                int termid = 0;
                int.TryParse(value, out termid);

                return R.session.CreateCriteria<term>()
                        .Add(Expression.Eq("type", "nav"))
                        .Add(Expression.Eq("id", termid))
                        .List<term>()
                        .FirstOrDefault();
            }
        }

        /// <summary>
        /// 保存并设置默认菜单
        /// </summary>
        /// <param name="term"></param>
        public void SetDefaultNav(term term)
        {
            using (ITransaction trans = R.session.BeginTransaction())
            {
                try
                {
                    R.session.SaveOrUpdate(term);

                    opt.set(R.siteid, R.default_nav_id, term.id.ToString());

                    trans.Commit();
                }
                catch
                {
                    trans.Rollback();
                    throw;
                }
            }
        }

        public static IList<term> RefereshName(IList<term> terms)
        {
            return RefereshName(terms, ((char)0xA0).ToString(), 4);
        }

        public static IList<term> RefereshName(IList<term> terms, string preChar, int charLen)
        {
            return RefereshName(terms, null, 0, preChar, charLen, 0);
        }

        private static IList<term> RefereshName(IList<term> source, IList<term> newlist, int pid, string preChar, int charLen, int deepIndex)
        {
            if (null == newlist)
            {
                newlist = new List<term>();
            }

            foreach (term t in source.Where(m => m.parent == pid))
            {
                term t2 = new term();
                t2.AssigningForm(t);

                string tempName = string.Empty;
                for (int i = 0; i < deepIndex; i++)
                {
                    for (int j = 0; j < charLen; j++)
                    {
                        tempName += preChar;
                    }
                }

                t2.name = tempName + t2.name;

                newlist.Add(t2);

                RefereshName(source, newlist, t2.id, preChar, charLen, ++deepIndex);
                deepIndex--;
            }

            return newlist;
        }

        /// <summary>
        /// 确定某个别名是否正在使用
        /// </summary>
        /// <param name="slug">别名</param>
        /// <param name="id">要被排除的termmid</param>
        /// <returns></returns>
        public bool IsExistsSlug(string slug, int id)
        {
            if (string.IsNullOrWhiteSpace(slug))
            {
                throw new QRunException("slug 不能为空。");
            }

            term result = Get(slug);

            if (null == result || result.id == id || result.siteid != R.siteid)
            {
                return false;
            }

            return true;
        }
    }
}
