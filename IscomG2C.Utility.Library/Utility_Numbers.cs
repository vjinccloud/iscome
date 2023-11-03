using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Text;

namespace IscomG2C.Utility
{
    /// <summary>中文數字
    /// 
    /// </summary>
    public class Utility_Numbers
    {
        /// <summary>阿拉伯數字轉中文數字
        /// 
        /// </summary>
        /// <param name="number"></param>
        /// <param name="inFinancial">金融用的大寫數字?</param>
        /// <returns></returns>
        public static string GetChineseNumber(int number, bool inFinancial = true)
        {

            string[] chineseNumber = null;
            string[] unit = null;
            //是否使用金融用的大寫數字?
            if (inFinancial)
            {
                chineseNumber = new string[] { "零", "壹", "貳", "參", "肆", "伍", "陸", "柒", "捌", "玖" };
                unit = new string[] { "", "拾", "佰", "仟", "萬", "拾萬", "佰萬", "仟萬", "億", "拾億", "佰億", "仟億", "兆", "拾兆", "佰兆", "仟兆" };
            }
            else
            {
                chineseNumber = new string[] { "零", "一", "二", "三", "四", "五", "六", "七", "八", "九" };
                unit = new string[] { "", "十", "百", "千", "萬", "十萬", "百萬", "千萬", "億", "十億", "百億", "千億", "兆", "十兆", "百兆", "千兆" };
            }

            try
            {
                StringBuilder ret = new StringBuilder();
                string inputNumber = number.ToString();
                int idx = inputNumber.Length;
                bool needAppendZero = false;
                foreach (char c in inputNumber)
                {
                    idx--;
                    if (c > '0')
                    {
                        if (needAppendZero)
                        {
                            ret.Append(chineseNumber[0]);
                            needAppendZero = false;
                        }
                        ret.Append(chineseNumber[(int)(c - '0')] + unit[idx]);
                    }
                    else
                        needAppendZero = true;
                }
                string result = (ret.Length == 0) ? chineseNumber[0] : ret.ToString();
                //特殊:如果結果為"一十" 修改為"十"

                if (number >= 10 && number <= 19)
                {
                    result = result.Replace("一十", "十").Replace("壹拾", "拾");
                }
                return result;
            }
            catch (Exception ex)
            {
                ErrorLog.SaveErrorLog(ex);
                return "";
            }
        }


        /// <summary>判斷目前輸入的是不是數字(含小數點)
        /// 
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public static bool isNum(string data)
        {
            double num = 0;
            return double.TryParse(data, out num);
        }

        /// <summary>字串轉數字
        /// 
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public static Int32 ToInt32(string data, int defaultvalue = 0)
        {
            Int32 value = defaultvalue;
            Int32.TryParse(data, out value);
            return value;
        }

        public static Int32 ToInt32(double data, int defaultvalue = 0)
        {
            Int32 value = defaultvalue;
            try
            {
                value = Convert.ToInt32(data);
            }
            catch (Exception ex)
            {
                ErrorLog.SaveErrorLog(ex);
                return value;
            }
            return value;
        }

        /// <summary>字串轉數字
        /// 
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public static Int64 ToInt64(string data, int defaultvalue = 0)
        {
            Int64 value = defaultvalue;
            Int64.TryParse(data, out value);
            return value;
        }

        /// <summary>字串轉換成雙精度浮點數
        /// 
        /// </summary>
        /// <returns></returns>
        public static double ToDouble(string data, int defaultvalue = 0)
        {
            double value = defaultvalue;
            double.TryParse(data, out value);
            return value;
        }
    }
}