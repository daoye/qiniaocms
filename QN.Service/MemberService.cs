#region
/********************************************************************
	This is NOT a freeware, use is subject to license terms.
	
	Copyright:	QiNiaoSoft Copyright (C) 2014 All rights Reserved.
	Created:	2014/11/30
	FileName: 	MemberService.cs
	Author:		DaoYe
	History: 	30/11/2014 18:57 By DaoYe
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
    public enum MemberError
    {
        /// <summary>
        /// 成功
        /// </summary>
        OK,

        /// <summary>
        /// 用户名已存在
        /// </summary>
        LoginExists
    }

    /// <summary>
    /// 用户管理服务
    /// </summary>
    public class MemberService
    {
        public IList<member> List(int start, int limit = 20)
        {
            return List(start, limit, null, null, null, null);
        }

        public IList<member> List(int start, int limit, string where, params object[] whereValues)
        {
            return List(start, limit, where, whereValues, null, null);
        }

        public IList<member> List(int start, int limit, string where, object whereValues, string order, out int pageCount, out int dataCount)
        {
            if (start <= 0)
            {
                start = 1;
            }
            if (limit <= 0)
            {
                limit = 20;
            }

            string hql = "from member";
            if (!string.IsNullOrWhiteSpace(where) && !string.IsNullOrEmpty(where))
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

            return query.List<member>();
        }

        public MemberError Add(member member)
        {
            if (IsExestsLoginName(member.login))
            {
                return MemberError.LoginExists;
            }

            member.registered = DateTime.Now;
            member.logined = DateTime.Now;

            R.session.Save(member);

            return MemberError.OK;
        }

        public MemberError Update(member member)
        {
            member entity = R.session.Get<member>(member.id);

            if (null == entity)
            {
                throw new QRunException("将被更新的对象无法找到。");
            }

            if (member.login != entity.login && IsExestsLoginName(member.login))
            {
                return MemberError.LoginExists;
            }

            entity.AssigningForm(member);

            R.session.Update(entity);

            return MemberError.OK;
        }

        public void Remove(member member)
        {
            if (!member.super)
            {
                R.session.Delete(member);
            }
        }

        public void Remove(int id)
        {
            member entity = R.session.Get<member>(id);

            if (null != entity)
            {
                Remove(entity);
            }
        }

        public member Get(int id)
        {
            return R.session.Get<member>(id);
        }

        /// <summary>
        /// 判断某个用户名是否存在
        /// </summary>
        /// <param name="uname"></param>
        /// <returns></returns>
        public bool IsExestsLoginName(string login)
        {
            return R.session
                    .CreateCriteria<member>()
                    .Add(Expression.Eq("login", login))
                    .SetProjection(Projections.RowCount())
                    .UniqueResult<int>() > 0;
        }
    }
}
