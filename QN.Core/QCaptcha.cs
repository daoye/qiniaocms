#region
/********************************************************************
	This is NOT a freeware, use is subject to license terms.
	
	Copyright:	QiNiaoSoft Copyright (C) 2014 All rights Reserved.
	Created:	2014/11/30
	FileName: 	QCaptcha.cs
	Author:		DaoYe
	History: 	30/11/2014 18:56 By DaoYe
	Purpose:	
*********************************************************************/
#endregion

using Captcha;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Text;

namespace QN
{
    /// <summary>
    /// 验证码辅助类
    /// </summary>
    public class QCaptcha
    {
        /// <summary>
        /// 从一个包含文字的文件中，随机选择一个文字创建验证码，并将被选中的文章保存在session中，并返回图片的实例
        /// </summary>
        /// <param name="sessionKey">用来缓存验证码的session key</param>
        /// <param name="wordsFilePath">包含文章的文本文件</param>
        /// <returns></returns>
        public static Bitmap Create(string sessionKey, string wordsFilePath)
        {
            var captcha = new SimpleCaptcha();
            captcha.SessionName = sessionKey;
            captcha.WordsFile = wordsFilePath;

            return captcha.CreateImage();
        }

        /// <summary>
        /// 使用指定的字符创建一个验证码图片，并返回改图片的实例
        /// </summary>
        /// <param name="word">验证码内容</param>
        /// <returns></returns>
        public static Bitmap Create(string word)
        {
            var captcha = new SimpleCaptcha();

            return captcha.CreateImage(word);
        }
    }
}