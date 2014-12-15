#region
/********************************************************************
	This is NOT a freeware, use is subject to license terms.
	
	Copyright:	QiNiaoSoft Copyright (C) 2014 All rights Reserved.
	Created:	2014/11/30
	FileName: 	QJson.cs
	Author:		DaoYe
	History: 	30/11/2014 18:55 By DaoYe
	Purpose:	
*********************************************************************/
#endregion

using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace QN
{
    /// <summary>
    /// JSON辅助类
    /// </summary>
    public static class QJson
    {
        /// <summary>
        /// 将一个对象序列化成JSONS字符
        /// </summary>
        /// <param name="value">将被序列化的对象</param>
        /// <returns></returns>
        public static string Serialize(object value)
        {
            return JsonConvert.SerializeObject(value);
        }

        /// <summary>
        /// 将一个JSON字符串反序列化为对象
        /// </summary>
        /// <param name="value">将被范序列化的JSON字符串</param>
        /// <returns>得到的对象</returns>
        public static object Deserialize(string value)
        {
            return JsonConvert.DeserializeObject(value);
        }

        /// <summary>
        /// 将一个JSON字符串反序列化为对象
        /// </summary>
        /// <typeparam name="T">目标类型</typeparam>
        /// <param name="value">将被范序列化的JSON字符串</param>
        /// <returns>得到的对象</returns>
        public static T Deserialize<T>(string value)
        {
            return JsonConvert.DeserializeObject<T>(value);
        }
    }
}
