#region
/********************************************************************
	This is NOT a freeware, use is subject to license terms.
	
	Copyright:	QiNiaoSoft Copyright (C) 2014 All rights Reserved.
	Created:	2014/11/30
	FileName: 	QEncryption.cs
	Author:		DaoYe
	History: 	30/11/2014 18:56 By DaoYe
	Purpose:	
*********************************************************************/
#endregion

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Security.Cryptography;
using System.IO;

namespace QN
{
    /// <summary>
    /// 加密辅助类
    /// </summary>
    public static class QEncryption
    {
        private static byte[] szKey = { 0x12, 0x33, 0x54, 0x1F, 0x4F, 0x88, 0xCD, 0xFF };
        private static byte[] szIV = { 0xAF, 0x11, 0x12, 0x88, 0xCD, 0x95, 0x34, 0x3D };

        /// <summary>
        /// 内存流加密
        /// </summary>
        /// <param name="strKeyString">需要加密的字符串</param>
        /// <returns>加密后的数据</returns>
        public static string DESMemEncryption(string strKeyString)
        {
            DES DESalg = DES.Create();

            MemoryStream mStream = new MemoryStream();
            CryptoStream cStream = new CryptoStream(mStream, DESalg.CreateEncryptor(szKey, szIV), CryptoStreamMode.Write);
            byte[] toEntry = Encoding.Unicode.GetBytes(strKeyString);

            cStream.Write(toEntry, 0, toEntry.Length);
            cStream.FlushFinalBlock();

            byte[] eData = mStream.ToArray();
            string str = "";
            for (int i = 0; i < eData.Length; i++)
            {
                str += Convert.ToChar(eData[i]);
            }

            return str;
        }

        /// <summary>
        /// 内存流解密
        /// </summary>
        /// <param name="strKeyString">需要解密的字符串</param>
        /// <returns>解密后的数据</returns>
        public static string DESMemDecryption(string strKeyString)
        {
            char[] str = strKeyString.ToCharArray();

            byte[] Data = new byte[str.Length];
            for (int i = 0; i < Data.Length; i++)
            {
                Data[i] = Convert.ToByte(str[i]);
            }

            MemoryStream msDecrypt = new MemoryStream(Data);

            DES DESalg = DES.Create();

            CryptoStream csDecrypt = new CryptoStream(msDecrypt, DESalg.CreateDecryptor(szKey, szIV), CryptoStreamMode.Read);

            byte[] fromEncrypt = new byte[Data.Length];

            csDecrypt.Read(fromEncrypt, 0, fromEncrypt.Length);

            return Encoding.Unicode.GetString(fromEncrypt);
        }

        /// <summary>
        /// 文件流解密
        /// </summary>
        /// <param name="strFilePath">文件路径</param>
        /// <returns>解密后的字符串</returns>
        public static string DESFileDecryption(string strFilePath)
        {
            FileStream fStream = System.IO.File.Open(strFilePath, FileMode.OpenOrCreate);

            DES DESalg = DES.Create();

            CryptoStream cStream = new CryptoStream(fStream,
                DESalg.CreateDecryptor(szKey, szIV),
                CryptoStreamMode.Read);

            StreamReader sReader = new StreamReader(cStream);
            string val = sReader.ReadToEnd();

            sReader.Close();
            cStream.Close();
            fStream.Close();

            return val;
        }

        /// <summary>
        /// 文件流加密
        /// </summary>
        /// <param name="strFilePath">文件路径</param>
        public static void DESFileEncryption(string strKeyString, string strFilePath)
        {
            byte[] Data = Encoding.Unicode.GetBytes(strKeyString.ToCharArray());

            FileStream fStream = System.IO.File.Open(strFilePath, FileMode.OpenOrCreate);

            DES DESalg = DES.Create();

            CryptoStream cStream = new CryptoStream(fStream,
                DESalg.CreateEncryptor(szKey, szIV),
                CryptoStreamMode.Write);

            StreamWriter sWriter = new StreamWriter(cStream);

            sWriter.WriteLine(Data);

            sWriter.Close();
            cStream.Close();
            fStream.Close();
        }

        /// <summary>
        /// 将字符串转换为MD5
        /// </summary>
        /// <param name="strKeyString">需要加密的字符串</param>
        /// <returns>转换后的MD5码</returns>
        public static string MD5Encryption(string strKeyString)
        {
            byte[] hashValue;
            byte[] message = Encoding.GetEncoding("GB2312").GetBytes(strKeyString);
            MD5 hashString = new MD5CryptoServiceProvider();
            string hex = "";
            hashValue = hashString.ComputeHash(message);
            foreach (byte x in hashValue)
            {
                hex += String.Format("{0:x2}", x);
            }

            return hex;
        }

        /// <summary>
        /// 将字符串转换为base64
        /// </summary>
        /// <param name="strKeyString">需要转换的字符串</param>
        /// <returns>加密后的字符串</returns>
        public static string Base64Encryption(string strKeyString)
        {
            if (null == strKeyString)
                return "";
            return Convert.ToBase64String(Encoding.Unicode.GetBytes(strKeyString.ToCharArray()), Base64FormattingOptions.None);
        }

        /// <summary>
        /// 将base64转换为字符串
        /// </summary>
        /// <param name="strKeyString">需要转换的base64码</param>
        /// <returns>解密后的字符串</returns>
        public static string Base64Decryption(string strKeyString)
        {
            try
            {
                if (null == strKeyString)
                    return "";
                return Encoding.Unicode.GetString(Convert.FromBase64String(strKeyString));
            }
            catch (FormatException)
            {
                return strKeyString;
            }
        }

        /// <summary>
        /// 计算文件的MD5值
        /// </summary>
        /// <param name="filepath">需要计算的文件名</param>
        /// <returns></returns>
        public static string FileMD5(String filepath)
        {
            MD5 md5 = new MD5CryptoServiceProvider();
            byte[] md5ch;
            using (FileStream fs = System.IO.File.OpenRead(filepath))
            {
                md5ch = md5.ComputeHash(fs);
            }
            md5.Clear();
            string strMd5 = "";
            for (int i = 0; i < md5ch.Length - 1; i++)
            {
                strMd5 += md5ch[i].ToString("x").PadLeft(2, '0');
            }
            return strMd5;
        }
    }
}
