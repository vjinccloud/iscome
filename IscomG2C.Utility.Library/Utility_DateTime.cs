using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace IscomG2C.Utility
{
    public class Utility_DateTime
    {
        /// <summary>轉換成字串 並轉成民國年
        /// 
        /// </summary>
        /// <param name="date"></param>
        /// <param name="format">格式</param>
        /// <param name="Default">預設值:用於null值的時候回傳的字串</param>
        /// <returns></returns>
        public static string ToFormat_inTaiwanYear(DateTime? input, string format = "yyy年MM月dd日", string Default = "---")
        {
            if (input == null)
                return Default;

            DateTime date = input.Value;
            //取代掉
            format = format
                .Replace("yyyy", "TaiwanYear")
                .Replace("yyy", "TaiwanYear");
            //年份
            string YearPart = (date.Year - 1911).ToString();

            //回傳年份日期
            string DatePart = date.ToString(format);

            //取代掉年度的部分
            return DatePart.Replace("TaiwanYear", YearPart);
        }


        /// <summary>根據輸入的時間字串 將它轉換成指定的格式
        /// 
        /// </summary>
        /// <param name="Input">原始的時間 允許null</param>
        /// <param name="Format">要轉換的格式 EX: yyyy-MM-dd HH:mm:ss</param>
        /// <param name="Default">預設值 用於輸入的時間格式無效時</param>
        /// <returns></returns>
        public static string ToFormat(DateTime? Input, string Format = "yyyy/MM/dd", string Default = "")
        {
            if (Input == null)
                return Default;//轉換失敗 回傳預設值

            try
            {
                return Input.Value.ToString(Format);
            }
            catch (Exception ex)
            {   //如果這樣還會出錯 表示你的Format有問題 _(:з」∠)_
                return Default;//轉換失敗 回傳預設值
            }
        }

        /// <summary>根據輸入的時間字串 將它轉換成指定的格式
        /// 
        /// </summary>
        /// <param name="Input">原始的時間字串</param>
        /// <param name="Format">要轉換的格式 EX: yyyy-MM-dd HH:mm:ss</param>
        /// <param name="Default">預設值 用於輸入的時間格式無效時</param>
        /// <returns></returns>
        public static string ToFormat(string Input, string Format, string Default = "")
        {
            DateTime? dt = StringToDateTime(Input);
            if (dt == null)
                return Default;//轉換失敗 回傳預設值

            try
            {
                return dt.Value.ToString(Format);
            }
            catch (Exception ex)
            {   //如果這樣還會出錯 表示你的Format有問題 _(:з」∠)_
                return Default;//轉換失敗 回傳預設值
            }
        }

        /// <summary>根據輸入的時間字串 將它轉換成指定的格式  "yyyy/MM/dd"
        /// 
        /// </summary>
        /// <param name="Input"></param>
        /// <param name="Default"></param>
        /// <returns></returns>
        public static string ToFormat_ShortDateString(string Input, string Default = "")
        {
            return ToFormat(Input, "yyyy/MM/dd", Default);
        }

        /// <summary>時間字串轉換為yyyyMMddHHmmss格式
        /// 
        /// </summary>
        /// <param name="Input"></param>
        /// <param name="Default">預設值(無法轉換時回傳)</param>
        /// <returns></returns>
        public static string ToFormat_yyyyMMddHHmmss(string Input, string Default = "")
        {
            DateTime? time = Convert(Input);

            if (time == null)
                return Default;

            return time.Value.ToString("yyyyMMddHHmmss");
        }

        /// <summary>取得民國年
        /// 
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public static int Get_TaiwanYear(DateTime? input)
        {
            if (input == null)
                return 0;
            return input.Value.Year - 1911;
        }

        /// <summary>允許Null之DateTime轉換成日期字串
        /// 
        /// </summary>
        /// <param name="input"></param>
        /// <param name="Default"></param>
        /// <returns></returns>
        public static string ToShortDateString(DateTime? input, string Default = "---")
        {
            if (input == null)
                return Default;
            return input.Value.ToString("yyyy/MM/dd");
        }

        /// <summary>字串轉時間(可支援yyyyMMddHHmmss格式)
        /// 
        /// </summary>
        /// <param name="Input"></param>
        /// <returns></returns>
        public static DateTime? StringToDateTime(string Input)
        {
            //一般的字串轉換
            DateTime? output = Convert(Input);
            //yyyyMMddHHmmss格式轉換
            if (output == null)
                output = ParseExact_yyyyMMddHHmmss(Input);
            return output;
        }

        /// <summary>一般字串轉換時間
        /// 
        /// </summary>
        /// <param name="Input"></param>
        /// <returns></returns>
        public static DateTime? Convert(string Input)
        {
            DateTime output = DateTime.Now;
            bool result = DateTime.TryParse(Input, out output);

            if (result == false)
                return null;
            else
                return output;
        }

        /// <summary>解析時間字串格式yyyyMMddHHmmss
        /// 
        /// </summary>
        /// <param name="Input"></param>
        /// <returns></returns>
        public static DateTime? ParseExact_yyyyMMddHHmmss(string Input)
        {
            DateTime? effdate = DateTime.Now;
            try
            {
                effdate = DateTime.ParseExact(Input, "yyyyMMddHHmmss", System.Globalization.CultureInfo.InvariantCulture);

            }
            catch (Exception ex)
            {
                return null;
            }
            return effdate;
        }
        /// <summary>解析日期字串
        /// 
        /// </summary>
        /// <param name="Input">日期字串</param>
        /// <param name="Format">解析格式</param>
        /// <returns></returns>
        public static DateTime? ParseExact(string Input, string Format = "yyyyMMddHHmmss")
        {
            DateTime? effdate = DateTime.Now;
            try
            {
                effdate = DateTime.ParseExact(Input, Format, System.Globalization.CultureInfo.InvariantCulture);

            }
            catch (Exception ex)
            {
                return null;
            }
            return effdate;
        }



        ///// <summary>將一般時間字串轉換為轉換為民國年 EX:104.01.01
        ///// 
        ///// </summary>
        ///// <param name="x"></param>
        ///// <param name="Default"></param>
        ///// <returns></returns>
        //public static string ToTaiwanCalendar(string x, string Default = "")
        //{
        //    //字串轉時間
        //    DateTime? DT = Utility_DateTime.StringToDateTime(x);

        //    if (DT == null)
        //        return Default;

        //    System.DateTime dt = DT.Value;

        //    string sYear = (dt.Year - 1911).ToString() + "." + dt.Month.ToString("00") + "." + dt.Day.ToString("00");
        //    return sYear;
        //}

        /// <summary>根據輸入本週任一天日期 與指定的星期 取得本週指定星期的日期
        /// EX: date=2015.09.24 (星期四) 
        /// week=週一 
        /// WeekShift= -1 (上一週)
        /// 則回傳2015.09.24的上周一日期 2015.09.14
        /// </summary>
        /// <param name="date">日期</param>
        /// <param name="week">星期</param>
        /// <param name="WeekShift">週數變動 0:本週 -1:前週 1:下週 -2:前二週 以此類推</param>
        /// <returns></returns>
        public static DateTime? GetDate_ByWeek(DateTime? date, DayOfWeek week, int WeekShift = 0)
        {
            if (date == null)
                return null;//無值

            //取得date對應的週
            DayOfWeek week_Input = date.Value.DayOfWeek;
            //計算跟指定週別的差異
            int DayDiff = week - week_Input;
            //根據週數變動 再調整日數
            DayDiff += (WeekShift * 7);

            //回傳修正後的日期
            return date.Value.AddDays(DayDiff);
        }

        /// <summary>根據輸入日期 與指定的起始星期 回傳起始星期的日期
        /// EX:  
        /// date=2015.09.22 (星期二) 
        /// BeginDayOfWeek=週四
        /// 則回傳2015.09.17(上週四)
        /// 
        /// EX 2:  
        /// date=2015.09.25 (星期五) 
        /// BeginDayOfWeek=週四
        /// 則回傳2015.09.24(本週四)
        /// </summary>
        /// <param name="date"></param>
        /// <param name="BeginDayOfWeek"></param>
        /// <param name="DayShift"></param>
        /// <returns></returns>
        public static DateTime? GetDate_BeginWeek(DateTime? date, DayOfWeek BeginDayOfWeek)
        {
            if (date == null)
                return null;//無值

            //取得date對應的週
            DayOfWeek week_Input = date.Value.DayOfWeek;
            //計算跟指定週別的差異
            int DayDiff = BeginDayOfWeek - week_Input;
            if (DayDiff > 0)
                DayDiff += -7;//跨週

            //回傳修正後的日期
            return date.Value.AddDays(DayDiff);
        }

        /// <summary>將日期的時間部分 綁回時間欄位 
        /// 
        /// </summary>
        /// <param name="inputDT">輸入日期的部分 會擷取日期的部分 省略原本的時間部分</param>
        /// <param name="TimeOfDay">輸入時間的部分</param>
        /// <returns></returns>
        public static DateTime? AddTimeOfDay(DateTime? inputDT, string TimeOfDay)
        {
            //如果沒有設定時間 退回
            if (inputDT == null)
                return inputDT;
            //取得日期部分
            DateTime DT = inputDT.Value.Date;

            //嘗試將日期與時間拼接起來
            DateTime? TempDT = Utility_DateTime.StringToDateTime(DT.ToString("yyyy-MM-dd" + " " + TimeOfDay));

            //如果拼接失敗 則回傳日期部分
            if (TempDT == null)
            {
                return DT;
            }

            //拼接成功則回傳新的DateTime
            return TempDT;

        }

        /// <summary>
        /// 取得國字星期?
        /// </summary>
        /// <param name="inputDT"></param>
        /// <returns></returns>
        public static string GetDayOfWeek(DateTime inputDT)
        {
            //取得date對應的週
            DayOfWeek weekday = inputDT.DayOfWeek;

            List<string> weekdays = new List<string>() { "日", "一", "二", "三", "四", "五", "六",};

            return weekdays.ToArray()[(int)weekday];
        }
    }
}