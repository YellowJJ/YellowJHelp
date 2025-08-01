﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Security.Cryptography;
using YellowJHelpFw.Entry;
using YellowJHelpFw.IServer;

namespace YellowJHelpFw
{

    [AutoInject(typeof(IYJHelp))]
    /// <summary>
    /// 通用帮助方法
    /// </summary>
    public class YJHelp : IYJHelp
    {
        /// <summary>
        /// 消息注明
        /// </summary>
        public string message() { return "秃头猿YellowJ温馨提醒您："; }


        #region 加解密
        /// <summary>
        /// 32位MD5加密
        /// </summary>
        /// <param name="strText">要加密字符串</param>
        /// <param name="IsLower">是否以小写方式返回</param>
        /// <returns></returns>
        public string MD5Encrypt(string strText, bool IsLower)
        {
            MD5 md5 = new MD5CryptoServiceProvider();
            byte[] bytes = System.Text.Encoding.UTF8.GetBytes(strText);
            bytes = md5.ComputeHash(bytes);
            md5.Clear();

            string ret = "";
            for (int i = 0; i < bytes.Length; i++)
            {
                ret += Convert.ToString(bytes[i], 16).PadLeft(2, '0');
            }

            return ret.PadLeft(32, '0');
        }

        /// <summary>
        /// DES加密
        /// </summary>
        /// <param name="data">需要加密的值</param>
        /// <param name="KEY_64">密钥长度8位</param>
        /// <param name="IV_64">密钥长度8位</param>
        /// <returns></returns>
        public string Encode(string data, string KEY_64, string IV_64)
        {
            byte[] byKey = System.Text.ASCIIEncoding.ASCII.GetBytes(KEY_64);
            byte[] byIV = System.Text.ASCIIEncoding.ASCII.GetBytes(IV_64);

            DESCryptoServiceProvider cryptoProvider = new DESCryptoServiceProvider();
            int i = cryptoProvider.KeySize;
            MemoryStream ms = new MemoryStream();
            CryptoStream cst = new CryptoStream(ms, cryptoProvider.CreateEncryptor(byKey, byIV), CryptoStreamMode.Write);

            StreamWriter sw = new StreamWriter(cst);
            sw.Write(data);
            sw.Flush();
            cst.FlushFinalBlock();
            sw.Flush();
            return Convert.ToBase64String(ms.GetBuffer(), 0, (int)ms.Length);

        }
        /// <summary>
        /// DES解密
        /// </summary>
        /// <param name="data">需要加密的值</param>
        /// <param name="KEY_64">密钥长度8位</param>
        /// <param name="IV_64">密钥长度8位</param>
        /// <returns></returns>
        public string Decode(string data, string KEY_64, string IV_64)
        {
            byte[] byKey = System.Text.ASCIIEncoding.ASCII.GetBytes(KEY_64);
            byte[] byIV = System.Text.ASCIIEncoding.ASCII.GetBytes(IV_64);

            byte[] byEnc;
            try
            {
                byEnc = Convert.FromBase64String(data);
            }
            catch
            {
                return null;
            }

            DESCryptoServiceProvider cryptoProvider = new DESCryptoServiceProvider();
            MemoryStream ms = new MemoryStream(byEnc);
            CryptoStream cst = new CryptoStream(ms, cryptoProvider.CreateDecryptor(byKey, byIV), CryptoStreamMode.Read);
            StreamReader sr = new StreamReader(cst);
            return sr.ReadToEnd();
        }


        #endregion


        [AutoInject(typeof(ICache))]
        /// <summary>
        /// 缓存类
        /// </summary>
        public class Cache : ICache
        {
            #region 缓存定义
            //缓存容器 
            private static Dictionary<string, object> CacheDictionary = new Dictionary<string, object>();
            /// <summary>
            /// 添加缓存
            /// </summary>
            public void Add(string key, object value)
            {
                CacheDictionary.Add(key, value);
            }

            /// <summary>
            /// 获取缓存
            /// </summary>
            public T Get<T>(string key)
            {
                return (T)CacheDictionary[key];
            }

            /// <summary>
            /// 判断缓存是否存在
            /// </summary>
            /// <param name="key"></param>
            /// <returns></returns>
            public bool Exsits(string key)
            {
                return CacheDictionary.ContainsKey(key);
            }

            /// <summary>
            /// 删除缓存
            /// </summary>
            /// <param name="key"></param>
            public void Clear(string key)
            {
                CacheDictionary.Remove(key);
            }

            /// <summary>
            /// 删除所有
            /// </summary>
            /// <returns></returns>
            public void Clear()
            {
                CacheDictionary.Clear();
            }

            #endregion
        }
        /// <summary>
        /// 日志
        /// </summary>
        /// <param name="text">消息</param>
        /// <param name="address">相对文件名</param>
        public void YellowJLog(string text, string address)
        {

            string path = AppDomain.CurrentDomain.BaseDirectory;
            path = System.IO.Path.Combine(path
            , "YellowJ_Logs/" + address + "\\");

            if (!System.IO.Directory.Exists(path))
            {
                System.IO.Directory.CreateDirectory(path);
            }
            string fileFullName = System.IO.Path.Combine(path
            , string.Format("{0}.txt", DateTime.Now.ToString("yyyyMMdd")));


            using (StreamWriter output = System.IO.File.AppendText(fileFullName))
            {
                output.WriteLine(DateTime.Now.ToString() + " 日志信息：" + text);

                output.Close();
            }
        }
        /// <summary>
        /// 字段截取
        /// </summary>
        /// <param name="sourse">值</param>
        /// <param name="startstr">前字符</param>
        /// <param name="endstr">后字符</param>
        /// <returns></returns>
        public string MidStrEx(string sourse, string startstr, string endstr)
        {
            string result = string.Empty;
            int startindex, endindex;
            startindex = sourse.IndexOf(startstr);
            if (startindex == -1)
                return result;
            string tmpstr = sourse.Substring(startindex + startstr.Length);
            endindex = tmpstr.IndexOf(endstr);
            if (endindex == -1)
                return result;
            result = tmpstr.Remove(endindex);

            return result;
        }

        /// <summary>
        /// string中是否包含某个值
        /// </summary>
        /// <param name="data">数据包</param>
        /// <param name="value">值</param>
        /// <returns></returns>
        public bool IsString(string data, string value)
        {
            if (data.IndexOf(value, StringComparison.OrdinalIgnoreCase) > -1) return true; else return false;
        }

    }
}