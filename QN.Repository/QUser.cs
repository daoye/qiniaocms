#region
/********************************************************************
	This is NOT a freeware, use is subject to license terms.
	
	Copyright:	QiNiaoSoft Copyright (C) 2014 All rights Reserved.
	Created:	2014/11/30
	FileName: 	QUser.cs
	Author:		DaoYe
	History: 	30/11/2014 18:57 By DaoYe
	Purpose:	
*********************************************************************/
#endregion

using QN.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Principal;
using System.Text;

namespace QN
{
    public class QUser : GenericPrincipal
    {
        public QUser(IIdentity identity, string[] roles)
            : base(identity, roles)
        {

        }

        private member _member = null;

        /// <summary>
        ///用户详细信息
        /// </summary>
        public member member
        {
            get
            {
                if (null == _member)
                {
                    _member = R.session.Get<member>(Convert.ToInt32(this.Identity.Name));
                }

                return _member;
            }
        }

        public override bool IsInRole(string role)
        {
            return member.role.ToString().Equals(role);
        }

        public bool IsInRole(int role)
        {
            return IsInRole(role.ToString());
        }
    }
}