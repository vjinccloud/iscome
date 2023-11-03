using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Configuration;
using System.Net.Mail;
using ICCModule.Helper;
using System.Security;
using System.Runtime.InteropServices;

namespace ICCModule.Helper
{
    /// <summary>
    /// 轉換
    /// </summary>
    public static partial class Extention
    {
        /// <summary>
        /// string轉int
        /// </summary>
        /// <param name="str">字符串</param>
        /// <returns></returns>
        public static int ToInt32(this string str)
        {
            var _res = 0;
            str = (str ?? "").Replace("\0", "");
            if (string.IsNullOrEmpty(str) || !int.TryParse(str, out _res))
                return 0;
            return Convert.ToInt32(str);
        }

        /// <summary>
        /// string轉decimal
        /// </summary>
        /// <param name="str">字符串</param>
        /// <returns></returns>
        public static decimal ToDecimal32(this string str)
        {
            decimal _res = 0;
            str = (str ?? "").Replace("\0", "");
            if (string.IsNullOrEmpty(str) || !decimal.TryParse(str, out _res))
                return 0;
            return decimal.Parse(str);
        }

        /// <summary>
        /// string轉double
        /// </summary>
        /// <param name="str">字符串</param>
        /// <returns></returns>
        public static double ToDouble32(this string str)
        {
            double _res = 0;
            str = (str ?? "").Replace("\0", "");
            if (string.IsNullOrEmpty(str) || !double.TryParse(str, out _res))
                return 0;
            return double.Parse(str);
        }

        /// <summary>
        /// string轉DateTime
        /// </summary>
        /// <param name="str">字符串</param>
        /// <returns></returns>
        public static DateTime? ToDateTime(this string str)
        {
            DateTime _res;
            str = str.Replace("\0", "");
            if (string.IsNullOrEmpty(str) || !DateTime.TryParse(str, out _res))
                return null;
            return Convert.ToDateTime(str);
        }

        /// <summary>
        /// string轉DateTime
        /// </summary>
        /// <param name="str">字符串</param>
        /// <returns></returns>
        public static DateTime? MinToDateTime(this string str)
        {
            str = str.Replace("\0", "");
            var splitChar = '/';
            if (str.Split('/').Length == 3)
            {

            }
            else if (str.Split('-').Length == 3)
            {
                splitChar = '-';
            }
            else return null;
            if (str.Split(splitChar)[0].ToInt32() == 0 || str.Split(splitChar)[1].ToInt32() == 0 || str.Split(splitChar)[2].ToInt32() == 0)
                return null;
            var _year = str.Split(splitChar)[0].ToInt32() > 1000 ? str.Split(splitChar)[0].ToInt32() : str.Split(splitChar)[0].ToInt32() + 1911;
            return new DateTime(_year, str.Split(splitChar)[1].ToInt32(), str.Split(splitChar)[2].ToInt32());
        }

        /// <summary>
        /// int與array比較轉decimal
        /// </summary>
        /// <param name="str"></param>
        /// <param name="midCount"></param>
        /// <param name="iArray"></param>
        /// <returns></returns>
        public static decimal IntInterval(this int str, List<int> iArray)
        {
            decimal response = 0;
            decimal sCount = 0, eCount = 0;
            if (iArray.Where(x => str >= x).Any())
                sCount = iArray.Where(x => str >= x).OrderByDescending(x => x).FirstOrDefault();
            eCount = iArray.Where(x => x > str).OrderBy(x => x).FirstOrDefault();
            response = iArray.Where(x => str >= x).Count() + Math.Round(((decimal)(str - sCount) / (eCount - sCount)), 1, MidpointRounding.AwayFromZero);
            return response;
        }

        /// <summary>
        /// 月日轉數字
        /// </summary>
        /// <param name="str">字符串</param>
        /// <returns></returns>
        public static decimal DateToDecimal(this string str)
        {
            DateTime _res;
            str = str.Replace("\0", "");
            if (string.IsNullOrEmpty(str) || !DateTime.TryParse(str, out _res))
                return 0;

            decimal response = 0;
            var _date = Convert.ToDateTime(str);
            response = _date.Month + Math.Round(((decimal)_date.Day / (decimal)(new DateTime(_date.AddMonths(1).Year, _date.AddMonths(1).Month, 1)).AddDays(-1).Day), 1, MidpointRounding.AwayFromZero);

            return response;
        }

        /// <summary>
        /// datetime轉民國年字串
        /// </summary>
        /// <param name="str">字符串</param>
        /// <returns></returns>
        public static string ToMinDateString(this DateTime? dt, string intervalStr = "/")
        {
            if (!dt.HasValue) return "";

            return (dt.Value.Year - 1911).ToString() + intervalStr + (dt.Value.Month).ToString() + intervalStr + (dt.Value.Day).ToString();
        }

        /// <summary>
        /// datetime轉字串(年月日)
        /// </summary>
        /// <param name="str">字符串</param>
        /// <returns></returns>
        public static string ToDateString(this DateTime? dt, string intervalStr = "/")
        {
            if (!dt.HasValue) return "";

            return (dt.Value.Year).ToString() + intervalStr + (dt.Value.Month).ToString() + intervalStr + (dt.Value.Day).ToString();
        }

        /// <summary>
        /// datetime轉12時制時間
        /// </summary>
        /// <param name="str">字符串</param>
        /// <returns></returns>
        public static string ToTimeapString(this DateTime? dt)
        {
            var _res = "";
            if (!dt.HasValue) return _res;

            if (dt.Value.Hour < 12) _res = "上午 " + dt.Value.ToString("hh:mm");
            else _res = "下午 " + dt.Value.ToString("hh:mm");

            return _res;
        }

        /// <summary>
        /// 計算日期為當月第幾周
        /// </summary>
        /// <param name="daytime"></param>
        /// <returns></returns>
        public static int GetWeekNumInMonth(this DateTime daytime)
        {
            int dayInMonth = daytime.Day;
            //本月第一天  
            DateTime firstDay = daytime.AddDays(1 - daytime.Day);
            //本月第一天是周幾  
            int weekday = (int)firstDay.DayOfWeek == 0 ? 7 : (int)firstDay.DayOfWeek;
            //本月第一周有幾天  
            int firstWeekEndDay = 7 - (weekday - 1);
            //當前日期和第一周之差  
            int diffday = dayInMonth - firstWeekEndDay;
            diffday = diffday > 0 ? diffday : 1;
            //當前是第幾周,如果整除7就減一天  
            int WeekNumInMonth = ((diffday % 7) == 0
             ? (diffday / 7 - 1)
             : (diffday / 7)) + 1 + (dayInMonth > firstWeekEndDay ? 1 : 0);
            return WeekNumInMonth;
        }
        public static SecureString GetPwdSecurity(string value)
        {
            SecureString result = new SecureString();
            foreach (char c in value)
            {
                result.AppendChar(c);
            }

            return result;
        }
        public static string SecureStringToString(SecureString value)
        {
            IntPtr valuePtr = IntPtr.Zero;
            try
            {
                valuePtr = Marshal.SecureStringToGlobalAllocUnicode(value);
                return Marshal.PtrToStringUni(valuePtr);
            }
            finally
            {
                Marshal.ZeroFreeGlobalAllocUnicode(valuePtr);
            }
        }
    }
}
