#region
/********************************************************************
	This is NOT a freeware, use is subject to license terms.
	
	Copyright:	QiNiaoSoft Copyright (C) 2014 All rights Reserved.
	Created:	2014/12/06
	FileName: 	ACLService.cs
	Author:		DaoYe
	History: 	6/12/2014 14:19 By DaoYe
	Purpose:	
*********************************************************************/
#endregion

using NHibernate.Criterion;
using QN.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace QN.Service
{
    /// <summary>
    /// 权限控制
    /// </summary>
    public class ACLService
    {

        /// <summary>
        /// 判断某个用户是否拥有某个权限
        /// </summary>
        /// <param name="memberId">用户ID</param>
        /// <param name="ACLCode">权限值</param>
        /// <returns></returns>
        public static bool HasACLCode(int memberId, params string[] ACLCode)
        {
            member member = R.session.Get<member>(memberId);

            if (null == member)
            {
                return false;
            }

            role role = R.session.Get<role>(member.role);

            if (role == null)
            {
                return false;
            }

            return R.session.CreateCriteria<acl>()
                            .Add(Expression.Eq("id", member.role))
                            .Add(Expression.In("value", ACLCode))
                            .SetProjection(Projections.RowCount())
                            .UniqueResult<int>() > 0;
        }

        /// <summary>
        /// 获取指定的访问控制信息
        /// </summary>
        /// <param name="aclId"></param>
        /// <returns></returns>
        public acl Get(int aclId)
        {
            return R.session.Get<acl>(aclId);
        }

        public IList<acl> List()
        {
            return R.session.CreateCriteria<acl>()
                            .List<acl>();
        }

        public IList<acl> ListByRole(int roleId)
        {
            return R.session.CreateCriteria<acl>()
                            .Add(Expression.Eq("roleid", roleId))
                            .List<acl>();
        }
    }
}