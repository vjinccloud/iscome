using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Configuration;
using System.Net.Mail;
using ICCModule.Helper;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Web;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace ICCModule.Helper
{
    /// <summary>
    /// 
    /// </summary>
    public class PwdStrengHelper
    {
        public static string GetVeriCode(int codeLenth)
        {
            int number;
            char code;
            string regCode = null;

            Random random = new Random(Guid.NewGuid().GetHashCode());
            for (int i = 0; i < codeLenth; i++)
            {
                number = random.Next();
                code = (char)('0' + (char)(number % 10));

                regCode += code.ToString();
            }
            return regCode;
        }

        /// <summary>
        /// 密碼強度檢視-4選3
        /// </summary>
        /// <returns></returns>
        /// <param name="pwdValue"></param>
        public static bool StrongPassword(string pwdValue)
        {
            var chooseThree = "";
            if (pwdValue.Any(x => IsBigLetter(x))) chooseThree += "A";
            if (pwdValue.Any(x => IsSmallLetter(x))) chooseThree += "a";
            if (pwdValue.Any(x => IsDigit(x))) chooseThree += "1";
            if (pwdValue.Any(x => IsSymbol(x))) chooseThree += "!";

            return chooseThree.Length < 3 || pwdValue.Length < 8;
        }

        public static bool IsBigLetter(char c)
        {
            return (c >= 'A' && c <= 'Z');
        }

        public static bool IsSmallLetter(char c)
        {
            return (c >= 'a' && c <= 'z');
        }

        public static bool IsDigit(char c)
        {
            return c >= '0' && c <= '9';
        }

        public static bool IsSymbol(char c)
        {
            return c > 32 && c < 127 && !IsDigit(c) && !IsBigLetter(c) && !IsSmallLetter(c);
        }
    }
}
