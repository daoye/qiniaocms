#region
/********************************************************************
	This is NOT a freeware, use is subject to license terms.
	
	Copyright:	QiNiaoSoft Copyright (C) 2014 All rights Reserved.
	Created:	2014/12/06
	FileName: 	RoleService.cs
	Author:		DaoYe
	History: 	6/12/2014 14:18 By DaoYe
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
    public class RoleService
    {
        public IList<role> List(int start, int limit = 20)
        {
            return List(start, limit, null, null, null, null);
        }

        public IList<role> List(int start, int limit, string where, params object[] whereValues)
        {
            return List(start, limit, where, whereValues, null, null);
        }

        public IList<role> List(int start, int limit, string where, object whereValues, string order, out int pageCount, out int dataCount)
        {
            if (start <= 0)
            {
                start = 1;
            }
            if (limit <= 0)
            {
                limit = 10;
            }

            string hql = " from role";
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

            return query.List<role>();
        }

        /// <summary>
        /// 获取指定的角色
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public role Get(int id)
        {
            return R.session.Get<role>(id);
        }

        public void Add(role entity, IEnumerable<acl> acls)
        {
            using (ITransaction trans = R.session.BeginTransaction())
            {
                try
                {
                    R.session.Save(entity);

                    if (null != acls)
                    {
                        foreach (acl a in acls)
                        {
                            a.roleid = entity.id;
                            R.session.Save(a);
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

        public void Update(role entity, IEnumerable<acl> acls)
        {
            using (ITransaction trans = R.session.BeginTransaction())
            {
                try
                {
                    role role = Get(entity.id);

                    if (null == role)
                    {
                        throw new QRunException("将被更新的对象无法找到。");
                    }

                    role.AssigningForm(entity);

                    R.session.Update(role);

                    if (null != acls)
                    {
                        foreach (acl a in R.session.CreateCriteria<acl>().Add(Expression.Eq("roleid", entity.id)).List<acl>())
                        {
                            R.session.Delete(a);
                        }

                        foreach (acl a in acls)
                        {
                            a.roleid = entity.id;
                            R.session.Save(a);
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

        public void Remove(params role[] entitys)
        {
            using (ITransaction trans = R.session.BeginTransaction())
            {
                try
                {
                    foreach (role r in entitys)
                    {
                        if (!r.super)
                        {
                            R.session.Delete(r);
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
            role entity = R.session.Get<role>(id);

            if (null == entity)
            {
                throw new QRunException("找不到指定的角色。");
            }

            Remove(entity);
        }

        public void Remove(int[] id)
        {
            List<role> members = new List<role>();

            foreach (int i in id)
            {
                role s = Get(i);
                if (null != s)
                {
                    members.Add(s);
                }
            }

            Remove(members.ToArray());
        }
    }
}
