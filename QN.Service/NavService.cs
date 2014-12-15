#region
/********************************************************************
	This is NOT a freeware, use is subject to license terms.
	
	Copyright:	QiNiaoSoft Copyright (C) 2014 All rights Reserved.
	Created:	2014/11/30
	FileName: 	CarteService.cs
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
using NHibernate.Criterion;
using NHibernate;

namespace QN.Service
{
    /// <summary>
    /// 菜单管理服务
    /// </summary>
    public class NavService
    {
        /// <summary>
        /// 获取所有菜单
        /// </summary>
        /// <returns></returns>
        public IList<nav> List(string type)
        {
            return R.session.CreateCriteria<nav>()
                            .Add(Expression.Eq("type", type))
                            .List<nav>();
        }

        /// <summary>
        /// 获取指定分类的子分类
        /// </summary>
        /// <param name="parent"></param>
        /// <returns></returns>
        public IList<nav> List(int parent)
        {
            return R.session.CreateCriteria<nav>()
                            .Add(Expression.Eq("parent", parent))
                            .List<nav>();
        }

        public void Add(nav entity)
        {
            using (ITransaction trans = R.session.BeginTransaction())
            {
                try
                {
                    R.session.Save(entity);

                    if (entity.parent == 0)
                    {
                        entity.deep = 1;
                        entity.deeppath = "/" + entity.id + "/";
                    }
                    else
                    {
                        nav parent = Get(entity.parent);

                        if (null == parent)
                        {
                            throw new QRunException("此导航对应的父级导航不存在。");
                        }

                        entity.deeppath = parent.deeppath + entity.id + "/";
                        entity.deep = parent.deep + 1;
                    }

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

        public void Update(nav entity)
        {
            using (ITransaction trans = R.session.BeginTransaction())
            {
                try
                {
                    if (entity.parent == 0)
                    {
                        entity.deep = 1;
                        entity.deeppath = "/" + entity.id + "/";
                    }
                    else
                    {
                        nav parent = Get(entity.parent);

                        if (null == parent)
                        {
                            throw new QRunException("此导航对应的父级导航不存在。");
                        }

                        entity.deeppath = parent.deeppath + entity.id + "/";
                        entity.deep = parent.deep + 1;
                    }

                    R.session.Update(entity);

                    RefereshChildDeepPath(entity);

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
        private void RefereshChildDeepPath(nav parent)
        {
            foreach (nav c in R.session.CreateCriteria<nav>().Add(Expression.Eq("parent", parent.id)).List<nav>())
            {
                c.deep = parent.deep + 1;
                c.deeppath = parent.deeppath + c.id + "/";

                RefereshChildDeepPath(c);
            }
        }


        /// <summary>
        /// 删除菜单
        /// </summary>
        /// <param name="carte"></param>
        public void Remove(nav entity)
        {
            using(ITransaction trans = R.session.BeginTransaction())
            {
                try
                {
                    foreach(nav n in R.session.CreateCriteria<nav>()
                                      .Add(Expression.Like("deeppath", "/" + entity.id + "/"))
                                      .List<nav>())
                    {
                        R.session.Delete(n);
                    }
                }
                catch
                {
                    trans.Rollback();
                    throw;
                }
            }
        }

        /// <summary>
        /// 删除菜单
        /// </summary>
        /// <param name="Id"></param>
        public void Remove(int Id)
        {
            nav entity = Get(Id);

            if (null != entity)
            {
                Remove(entity);
            }
        }

        /// <summary>
        /// 获取菜单
        /// </summary>
        /// <param name="Id"></param>
        /// <returns></returns>
        public nav Get(int Id)
        {
            return R.session.Get<nav>(Id);
        }
    }
}
