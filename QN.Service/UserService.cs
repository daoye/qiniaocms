#region
/********************************************************************
	This is NOT a freeware, use is subject to license terms.
	
	Copyright:	QiNiaoSoft Copyright (C) 2014 All rights Reserved.
	Created:	2014/11/30
	FileName: 	UserService.cs
	Author:		DaoYe
	History: 	30/11/2014 18:58 By DaoYe
	Purpose:	
*********************************************************************/
#endregion

using NHibernate;
using NHibernate.Criterion;
using QN.Repository;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Web;

namespace QN.Service
{
    public enum UserLoginError
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
    public class UserService
    {
        /// <summary>
        /// 当前会话是否是已经授权登录过的会话
        /// </summary>
        public static bool IsLogined
        {
            get
            {
                if (null == HttpContext.Current.User)
                {
                    return false;
                }

                return HttpContext.Current.User.Identity.IsAuthenticated;
            }
        }

        /// <summary>
        /// 获取当前用户的角色ID
        /// </summary>
        public static int Roles
        {
            get
            {
                return CurrentUser.info.roleid;
            }
        }

        /// <summary>
        /// 当前登录用户
        /// </summary>
        public static QUser CurrentUser
        {
            get
            {
                return HttpContext.Current.User as QUser;
            }
        }

        public IList<user> List(int start, int limit = 20)
        {
            return List(start, limit, null, null, null, null);
        }

        public IList<user> List(int start, int limit, string where, params object[] whereValues)
        {
            int a, b;
            return List(start, limit, where, whereValues, null, out a, out b);
        }

        public IList<user> List(int start, int limit, string where, object whereValues, string order, out int pageCount, out int dataCount)
        {
            string hql = " from user";
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

            return query.List<user>();
        }

        /// <summary>
        /// 查询符合条件的分类数量
        /// </summary>
        /// <param name="where"></param>
        /// <param name="whereValues"></param>
        /// <returns></returns>
        public int Count(string where = null, object whereValues = null)
        {
            string hql = "select count(*) from user ";

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

        public UserLoginError Add(user user)
        {
            if (IsExestsLoginName(user.login, user.id))
            {
                return UserLoginError.LoginExists;
            }

            user.date = DateTime.Now;
            //user.logined = DateTime.Now;

            R.session.Save(user);

            return UserLoginError.OK;
        }

        public UserLoginError Update(user user)
        {
            using (ITransaction trans = R.session.BeginTransaction())
            {
                try
                {
                    user entity = R.session.Get<user>(user.id);

                    if (null == entity)
                    {
                        throw new QRunException("将被更新的对象无法找到。");
                    }

                    if (IsExestsLoginName(user.login, user.id))
                    {
                        return UserLoginError.LoginExists;
                    }

                    entity.AssigningForm(user, "date");

                    R.session.Update(entity);

                    trans.Commit();

                    return UserLoginError.OK;
                }
                catch
                {
                    trans.Rollback();
                    throw;
                }
            }
        }

        public void Remove(params user[] entitys)
        {
            int superid = 0;
            int.TryParse(opt.get(R.siteid, "super-user-id"), out superid);

            using (ITransaction trans = R.session.BeginTransaction())
            {
                try
                {
                    foreach (user user in entitys)
                    {
                        if (user.id != superid && user.id != CurrentUser.info.id)
                        {
                            R.session.Delete(user);
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
            user entity = R.session.Get<user>(id);

            if (null != entity)
            {
                Remove(entity);
            }
        }

        public void Remove(int[] id)
        {
            List<user> users = new List<user>();

            foreach (int i in id)
            {
                user s = Get(i);
                if (null != s)
                {
                    users.Add(s);
                }
            }

            Remove(users.ToArray());
        }

        public user Get(int id)
        {
            return R.session.Get<user>(id);
        }

        public user Get(string login)
        {
            return R.session.CreateCriteria<user>()
                            .Add(Expression.Eq("login", login))
                            .Add(Expression.Eq("siteid", R.siteid))
                            .List<user>()
                            .FirstOrDefault();
        }

        /// <summary>
        /// 判断某个用户名是否存在
        /// </summary>
        /// <param name="uname"></param>
        /// <returns></returns>
        public bool IsExestsLoginName(string login, int id)
        {
            return R.session
                    .CreateCriteria<user>()
                    .Add(Expression.Eq("login", login))
                    .Add(Expression.Not(Expression.Eq("id", id)))
                    .Add(Expression.Eq("siteid", R.siteid))
                    .SetProjection(Projections.RowCount())
                    .UniqueResult<int>() > 0;
        }
    }
}
