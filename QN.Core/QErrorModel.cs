#region
/********************************************************************
	This is NOT a freeware, use is subject to license terms.
	
	Copyright:	QiNiaoSoft Copyright (C) 2014 All rights Reserved.
	Created:	2014/12/06
	FileName: 	QErrorModel.cs
	Author:		DaoYe
	History: 	6/12/2014 14:18 By DaoYe
	Purpose:	
*********************************************************************/
#endregion

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace QN
{
    /// <summary>
    /// 错误信息模型
    /// </summary>
    public class QErrorModel : Dictionary<string, string>
    {
        private bool _IsShowTip = true;
        private bool _IsShowError = true;

        /// <summary>
        /// 是否显示提示信息
        /// </summary>
        public bool IsShowTip { get { return _IsShowTip; } set { _IsShowTip = value; } }

        /// <summary>
        /// 是否显示错误信息
        /// </summary>
        public bool IsShowError { get { return _IsShowError; } set { _IsShowError = value; } }

        /// <summary>
        /// 是否存在指定的键
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public virtual bool HasKey(string key)
        {
            return this.Keys.Any(m => m == key);
        }

        /// <summary>
        /// 是否包含错误信息
        /// </summary>
        /// <returns></returns>
        public virtual bool HasError()
        {
            return this.Values.Any(m => !string.IsNullOrWhiteSpace(m));
        }

        /// <summary>
        /// 显示提示或错误信息
        /// </summary>
        /// <param name="key">键</param>
        /// <param name="tip">提示</param>
        /// <param name="error">错误信息</param>
        /// <returns></returns>
        public virtual void Display(string key, string tip)
        {
            if (HasKey(key) && !string.IsNullOrWhiteSpace(this[key]) && IsShowError)
            {
                System.Web.HttpContext.Current.Response.Write("<span class='verified-error'>" + this[key] + "</span>");
            }
            else if (IsShowTip)
            {
                System.Web.HttpContext.Current.Response.Write("<span class='verified-tip'>" + tip + "</span>");
            }
        }
    }
}