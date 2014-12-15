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
        public IList<role> List()
        {
            int a, b;
            return List(-1, -1, out a, out b);
        }

        public IList<role> List(int start, int limit, out int pageCount, out int dataCount)
        {
            ICriteria result = R.session.CreateCriteria<role>();

            result.SetProjection(Projections.RowCount());
            dataCount = result.UniqueResult<int>();

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
                result = result.SetFirstResult((start - 1) * limit)
                             .SetMaxResults(limit);
            }

            return result.List<role>();
        }

        /// <summary>
        /// 获取指定的角色
        /// </summary>
        /// <param name="roleId"></param>
        /// <returns></returns>
        public role Get(int roleId)
        {
            return R.session.Get<role>(roleId);
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

                    entity.AssigningForm(role);

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


        public void Remove(role entity)
        {
            if (!entity.super)
            {
                R.session.Delete(entity);
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
    }
}
