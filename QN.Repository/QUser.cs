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

        private user _user = null;

        /// <summary>
        ///用户详细信息
        /// </summary>
        public user info
        {
            get
            {
                if (null == _user)
                {
                    _user = R.session.Get<user>(Convert.ToInt32(this.Identity.Name));
                }

                return _user;
            }
        }

        public override bool IsInRole(string role)
        {
            return info.roleid.ToString().Equals(role);
        }

        public bool IsInRole(int role)
        {
            return IsInRole(role.ToString());
        }
    }
}