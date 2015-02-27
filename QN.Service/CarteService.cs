#region
/********************************************************************
	This is NOT a freeware, use is subject to license terms.
	
	Copyright:	QiNiaoSoft Copyright (C) 2014 All rights Reserved.
	Created:	2014/12/06
	FileName: 	CarteService.cs
	Author:		DaoYe
	History: 	6/12/2014 14:19 By DaoYe
	Purpose:	
*********************************************************************/
#endregion

using NHibernate;
using NHibernate.Criterion;
using QN.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace QN.Service
{
    /// <summary>
    /// 菜单管理
    /// </summary>
    public class CarteService
    {
        public IList<carte> List(int start, int limit = 20)
        {
            return List(start, limit, null, null);
        }

        public IList<carte> List(int start, int limit, string where, object whereValues)
        {
            int a, b;
            return List(start, limit, where, whereValues, null, out a, out b);
        }

        public IList<carte> List(int start, int limit, string where, object whereValues, string order, out int pageCount, out int dataCount)
        {
            string hql = " from carte";
            if (!string.IsNullOrWhiteSpace(where))
            {
                hql += " where " + where;
            }

            if (!string.IsNullOrWhiteSpace(order))
            {
                hql += " order by ";
                if (string.Compare("rand", order) == 0)
                {
                    hql += DBAdapter.randExpression;
                }
                else
                {
                    hql += order;
                }
            }
            else
            {
                hql += " order by order asc";
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

            return query.List<carte>();
        }

        /// <summary>
        /// 查询符合条件的菜单数量
        /// </summary>
        /// <param name="where"></param>
        /// <param name="whereValues"></param>
        /// <returns></returns>
        public int Count(string where = null, object whereValues = null)
        {
            string hql = "select count(*) from carte ";

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

        public void Add(carte carte)
        {
            R.session.Save(carte);
        }

        public void Update(carte carte)
        {
            using (ITransaction trans = R.session.BeginTransaction())
            {
                try
                {
                    carte entity = R.session.Get<carte>(carte.id);

                    if (null == entity)
                    {
                        throw new QRunException("将被更新的对象无法找到。");
                    }

                    entity.AssigningForm(carte);

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

        public void Remove(params carte[] entitys)
        {
            using (ITransaction trans = R.session.BeginTransaction())
            {
                try
                {
                    foreach (carte carte in entitys)
                    {
                        R.session.Delete(carte);
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
            carte entity = R.session.Get<carte>(id);

            if (null != entity)
            {
                Remove(entity);
            }
        }

        public void Remove(int[] id)
        {
            List<carte> users = new List<carte>();

            foreach (int i in id)
            {
                carte s = Get(i);
                if (null != s)
                {
                    users.Add(s);
                }
            }

            Remove(users.ToArray());
        }

        public carte Get(int id)
        {
            return R.session.Get<carte>(id);
        }
    }
}