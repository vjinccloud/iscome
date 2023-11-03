using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace IscomG2C.Utility.Logic
{
    public static class DateFormatLogic
    {
        /// <summary>
        /// "yyyy-MM-dd HH:mm:ss.fff"
        /// (年/月/日 時:分:秒)
        /// </summary>
        public static string timeFormat_2 = "yyyy-MM-dd HH:mm:ss.fff";

        /// <summary>
        /// "yyyy/MM/dd HH:mm:ss"
        /// (年/月/日 時:分:秒)
        /// </summary>
        public static string timeFormat = "yyyy/MM/dd HH:mm:ss";

        /// <summary>
        /// "yyyy/MM/dd"
        /// (年/月/日)
        /// </summary>
        public static string yyyyMMdd = "yyyy/MM/dd";

        /// <summary>
        /// "yyyy/MM"
        /// (年/月)
        /// </summary>
        public static string yyyyMM = "yyyy/MM";

        /// <summary>
        /// "HH:mm"
        /// </summary>
        public static string HHmm = "HH:mm";

        /// <summary>驗證日期格式
        /// 
        /// </summary>
        /// <param name="strDate"></param>
        /// <returns></returns>
        public static bool IsIllegalForDate(string strDate)
        {
            bool isSuccess = false;
            DateTime resultDate = DateTime.Now;
            if (!String.IsNullOrEmpty(strDate.Trim()))
            {
                isSuccess = DateTime.TryParse(strDate.Trim(), out resultDate);
            }
            else
            {
                isSuccess = true;
            }
            return isSuccess;
        }

        /// <summary>
        /// 取得目前的時間
        /// </summary>
        /// <param name="format">轉換格式</param>
        /// <returns></returns>
        public static string GetNowTime(string format)
        {
            DateTime now = DateTime.Now;
            string nowTimeStr = now.ToString(format);
            return nowTimeStr;
        }

        # region 日期轉換
        /// <summary>
        /// 轉換時間格式 DateTime to String
        /// </summary>
        /// <param name="date">時間</param>
        /// <param name="format">轉換格式</param>
        /// <returns></returns>
        public static string ParseDateTime(DateTime? date, string format)
        {
            if (date.HasValue)
            {
                string dateStr = date.Value.ToString(format);
                return dateStr;
            }
            else
            {
                return "";
            }
        }

        /// <summary>
        /// 轉換時間字串 String to DateTime
        /// </summary>
        /// <param name="date">時間字串</param>
        /// <param name="timeFormat">轉換格式</param>
        /// <returns></returns>
        public static DateTime ParseDateStrToDate(string date, string timeFormat = "yyyy-MM-dd HH:mm:ss.fff")
        {
            if (!String.IsNullOrEmpty(date))
            {
                DateTime dt = new DateTime();
                try
                {
                    dt = DateTime.ParseExact(date, timeFormat, System.Globalization.CultureInfo.CurrentCulture);

                }
                catch
                {
                    // do nothing
                }
                return dt;
            }
            else
            {
                return DateTime.MinValue;
            }
        }
        # endregion 日期轉換

        # region 日期計算

        /// <summary>
        /// 計算相差月份
        /// </summary>
        /// <param name="timeFormat">時間格式</param>
        /// <param name="startDate">開始日期</param>
        /// <param name="endDate">結束日期</param>
        /// <param name="sErrMsg">執行訊息</param>
        /// <returns></returns>
        public static int DiffMonths(string timeFormat, string startDate, string endDate, ref string sErrMsg)
        {
            try
            {
                DateTime startDateTime = Convert.ToDateTime(startDate);
                DateTime endDateTime = Convert.ToDateTime(endDate);
                int totalMonth = endDateTime.Year * 12 + endDateTime.Month - startDateTime.Year * 12 - startDateTime.Month + 1;
                sErrMsg = "Successful";
                return totalMonth;
            }
            catch (Exception e)
            {
                sErrMsg = e.Message;
                return 0;
            }
        }

        # endregion 日期計算

        # region 計算一週的開始和結束日期
        /// <summary>
        /// 計算本週結束日期(禮拜一)
        /// </summary>
        /// <param name="someDate"> 該週的任一天 </param>
        /// <returns> 回傳禮拜一的日期，其中時、分、秒街與傳入值相同 </returns>
        public static DateTime CalculateFirstDateOfWeek(DateTime someDate)
        {
            int i = someDate.DayOfWeek - DayOfWeek.Monday;
            if (i == -1)
            {
                i = 6; // i值 > = 0 ，因为枚举原因，Sunday排在最前，此时Sunday-Monday=-1，必须+7=6。
            }
            TimeSpan ts = new TimeSpan(i, 0, 0, 0);
            return someDate.Subtract(ts);
        }
        /// <summary>
        /// 計算本週結束日期(禮拜日)
        /// </summary>
        /// <param name="someDate"> 該週的任一天 </param>
        /// <returns> 回傳禮拜日的日期，其中時、分、秒街與傳入值相同 </returns>
        public static DateTime CalculateLastDateOfWeek(DateTime someDate)
        {
            int i = someDate.DayOfWeek - DayOfWeek.Sunday;
            if (i != 0)
            {
                i = 7 - i; // 因为枚举原因，Sunday排在最前，相减间隔要被7减。
            }
            TimeSpan ts = new TimeSpan(i, 0, 0, 0);
            return someDate.Add(ts);
        }
        /// <summary>
        /// 判斷選擇的日期是否在在本週內(根據系統時間)
        /// </summary>
        /// <param name="someDate"></param>
        /// <returns></returns>
        public static bool IsThisWeek(DateTime someDate)
        {
            //得到someDate對應的禮拜一
            DateTime someMon = CalculateFirstDateOfWeek(someDate);
            //得到禮拜一
            DateTime nowMon = CalculateFirstDateOfWeek(DateTime.Now);
            TimeSpan ts = someMon - nowMon;
            if (ts.Days < 0)
            {
                ts = -ts; //取正
            }
            if (ts.Days >= 7)
            {
                return false;
            }
            else
            {
                return true;
            }
        }
        # endregion
    }
}
