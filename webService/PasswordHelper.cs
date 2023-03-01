using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Web;

namespace webService
{
    public class PasswordHelper
    {

        /// <summary>
        /// MD5 加密字符串
        /// </summary>
        /// <param name="rawPass">源字符串</param>
        /// <returns>加密后字符串</returns>
        public string MD5Encoding(string rawPass)
        {
            // 创建MD5类的默认实例：MD5CryptoServiceProvider
            MD5 md5 = MD5.Create();
            byte[] bs = Encoding.UTF8.GetBytes(rawPass);
            byte[] hs = md5.ComputeHash(bs);
            StringBuilder stb = new StringBuilder();
            foreach (byte b in hs)
            {
                // 以十六进制格式格式化
                stb.Append(b.ToString("x2"));
            }
            return stb.ToString();
        }

        /// <summary>
        /// MD5盐值加密
        /// </summary>
        /// <param name="rawPass">源字符串</param>
        /// <param name="salt">盐值</param>
        /// <returns>加密后字符串</returns>
        public string MD5Encoding(string rawPass, object salt)
        {
            if (salt == null) return rawPass;
            return MD5Encoding(rawPass + "{" + salt.ToString() + "}");
        }
        /// <summary>
        /// 获取盐
        /// </summary>
        /// <param name="len"></param>
        /// <returns></returns>
        public static string CreateVerifyCode(int len)
        {
            char[] data = { 'a','b','c','d','e','f','j','h','i','j','k','l','m',
                'n','o','p','q','r','s','t','u','v','w','x','y','z','0','1','2','3','4','5','6','7','8','9','A','B', 'C','D','E', 'F', 'G','H', 'I', 'J', 'K',
                'L','M', 'N', 'O', 'P', 'Q', 'R', 'S', 'T', 'U', 'V', 'W', 'X','Y', 'Z'};
            StringBuilder sb = new StringBuilder();
            Random rand = new Random();
            for (int i = 0; i < len; i++)
            {
                int index = rand.Next(data.Length);//[0,data.length)
                char ch = data[index];
                sb.Append(ch);
            }
            return sb.ToString();
        }

    }
}