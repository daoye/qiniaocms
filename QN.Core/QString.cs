#region
/********************************************************************
	This is NOT a freeware, use is subject to license terms.
	
	Copyright:	QiNiaoSoft Copyright (C) 2014 All rights Reserved.
	Created:	2014/11/30
	FileName: 	QString.cs
	Author:		DaoYe
	History: 	30/11/2014 22:14 By DaoYe
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
    /// 一些与字符串计算相关的辅助类
    /// </summary>
    public class QString
    {
        private static Random r = new Random();

        /// <summary>
        /// 随机生成一段字符串
        /// </summary>
        /// <param name="minLen">最小长度</param>
        /// <param name="maxLen">最大长度</param>
        /// <returns>生成的字符串</returns>
        public static string RandStr(int minLen, int maxLen)
        {
            string result = string.Empty;

            int len = r.Next(minLen, maxLen);

            int i =0;
            while (i < len)
            {
                result += (char)r.Next(97, 122);
                i++;
            }

            return result;
        }
    }
}
