using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Security.Cryptography;
using System.Text.RegularExpressions;

namespace IscomG2C.Utility
{
    /// <summary>加密工具
    /// 
    /// </summary>
    public static class Utility_Cryptography
    {

        /// <summary>根據 輸入的密碼 DB儲存的密碼 比較雜湊函數結果
        /// 
        /// </summary>
        /// <param name="inputPassword">輸入的密碼</param>
        /// <param name="DBPassword">DB儲存的密碼</param>
        /// <returns></returns>
        public static bool Compare_HashCode(string inputPassword, string DBPassword)
        {
            //取得雜湊加密結果 (不可解密)
            string result = Get_HashEncryption(inputPassword);
            return result.Equals(DBPassword);
        }

        /// <summary>取得雜湊加密結果(不可解密) 目前一律採用SHA256演算法
        /// 
        /// </summary>
        /// <param name="Text"></param>
        /// <returns></returns>
        public static string Get_HashEncryption(string Text)
        {
            Text = Text ?? "";
            SHA256 algorithm = SHA256.Create();//建立一個SHA256
            byte[] source = Encoding.Default.GetBytes(Text);//將字串轉為Byte[]
            byte[] crypto = algorithm.ComputeHash(source);//進行SHA256雜湊
            string result = Convert.ToBase64String(crypto);//把加密後的字串從Byte[]轉為字串
            return result;
        }
		/// <summary>取得密碼看是否符合資安規範
        /// 
        /// </summary>
        /// <param name="Text"></param>
        /// <returns></returns>
        public static bool Check_Password(string Text)
        {   
            var result = false;
            if (Regex.IsMatch(Text, "^.*(?=.{12,})(?=.*\\d)(?=.*[a-z])(?=.*[A-Z])(?=.*[@#$%^&+=]).*$"))
            {   
                result = true;
            }
            return result;
        }
    }
}
