using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace IscomG2C.Utility
{

    /// <summary>小工具-特殊字串處理
    /// 
    /// </summary>
    public static class Utility_String
    {

        /// <summary>移除括號或中括號
        /// 
        /// </summary>
        /// <param name="inputStr"></param>
        /// <returns></returns>
        public static string RemoveBrackets(string inputStr)
        {

            List<char> FrontBrackets = new List<char>();
            FrontBrackets.Add('(');
            FrontBrackets.Add('（');
            FrontBrackets.Add('[');
            string retStr = inputStr;//.IndexOfAny(FrontBrackets.ToArray());
            try
            {
                retStr = inputStr.Split(FrontBrackets.ToArray())[0];//.IndexOfAny(FrontBrackets.ToArray());
                retStr = retStr.Trim();
            }
            catch (Exception ex)
            {
                ErrorLog.SaveErrorLog(ex);
            }
            return retStr;
        }

        /// <summary>移除底盤型式的括號 ,但是要保留大括號
        /// 
        /// </summary>
        /// <param name="inputStr"></param>
        /// <returns></returns>
        public static string RemoveBrackets_ByChassisName(string inputStr)
        {

            List<char> FrontBrackets = new List<char>();
            FrontBrackets.Add('(');
            FrontBrackets.Add('[');
            string retStr = inputStr;//.IndexOfAny(FrontBrackets.ToArray());
            try
            {
                retStr = inputStr.Split(FrontBrackets.ToArray())[0];//.IndexOfAny(FrontBrackets.ToArray());
                retStr = retStr.Trim();
            }
            catch (Exception ex)
            {
                ErrorLog.SaveErrorLog(ex);
            }
            return retStr;
        }

        /// <summary>移除客戶名稱的中括號
        /// 
        /// </summary>
        /// <param name="inputStr"></param>
        /// <returns></returns>
        public static string RemoveBrackets_ByCustName(string inputStr)
        {

            List<char> FrontBrackets = new List<char>();
            FrontBrackets.Add('[');
            string retStr = inputStr;//.IndexOfAny(FrontBrackets.ToArray());
            try
            {
                retStr = inputStr.Split(FrontBrackets.ToArray())[0];//.IndexOfAny(FrontBrackets.ToArray());
                retStr = retStr.Trim();
            }
            catch (Exception ex)
            {
                ErrorLog.SaveErrorLog(ex);
            }
            return retStr;
        }

        /// <summary>移除換行符號
        /// 
        /// </summary>
        /// <param name="inputStr"></param>
        /// <returns></returns>
        public static string RemoveNewLine(string inputStr)
        {

            if (string.IsNullOrEmpty(inputStr))
                return "";
            //移除換行
            inputStr = inputStr.Replace("\n", "");//替換掉換行符號
            inputStr = inputStr.Replace("\r", "");//替換掉換行符號

            return inputStr;
        }

        /// <summary>分割含有「||」及「|」字串並回傳分割後陣列
        /// 
        /// </summary>
        /// <param name="mglv_value"></param>
        /// <returns></returns>
        public static string[] SplitVerticalBar(string mglv_value)
        {
            //第一次分割
            string[] mgValue = mglv_value.Split(new string[] { "||" }, StringSplitOptions.RemoveEmptyEntries);
            //第二次分割
            string[] mgValue2 = mgValue[1].Split('|');

            return mgValue2;
        }

        /// <summary>分割含有「||」字串並回傳分割後陣列
        /// 
        /// </summary>
        /// <param name="mglv_value"></param>
        /// <returns></returns>
        public static string[] SplitDoubleVerticalBar(string input)
        {
            //第一次分割
            string[] mgValue = input.Split(new string[] { "||" }, StringSplitOptions.RemoveEmptyEntries);

            return mgValue;
        }

        /// <summary>處理字串分割 若無值或分割失敗則會則回傳空陣列
        /// 
        /// </summary>
        /// <param name="input">輸入的字串</param>
        /// <param name="mark">分割的記號</param>
        /// <returns></returns>
        public static List<string> GetList_Split(string input, string mark, bool isRemoveEmptyEntries = true)
        {
            if (string.IsNullOrEmpty(input))
                return new List<string>();

            try
            {
                string[] strs = null;
                if (isRemoveEmptyEntries)
                {

                    strs = input.Split(new string[] { mark }, StringSplitOptions.RemoveEmptyEntries);
                }
                else
                {
                    strs = input.Split(new string[] { mark }, StringSplitOptions.None);
                }

                return strs.ToList();
            }
            catch (Exception ex)
            {
                ErrorLog.SaveErrorLog(ex);
                return new List<string>();
            }

        }

        /// <summary>分割含有「||」字串並回傳分割後List
        /// 
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public static List<string> GetList_SplitByDoubleVerticalBar(string input)
        {
            return GetList_Split(input, "||");
        }

        /// <summary>特殊:分割審查報告編號,根據字串內的 "," ";" " " 來分割 並且去除括號內容
        /// ex:
        /// "A99CCBKU-340,A99CCBKW-340,A99CCBZZ-340"
        /// [A99CCBKU-340][A99CCBKW-340][A99CCBZZ-340]
        /// 
        /// 使用的功能:[符合性宣告表]
        /// 
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public static List<string> GetList_Split_ReportNo(string input)
        {
            if (string.IsNullOrEmpty(input))
                return new List<string>();

            try
            {
                string[] strs = input.Split(new string[] { ",", ";", " ", "、", "，", "\r\n", "\n", "/" }, StringSplitOptions.RemoveEmptyEntries);

                for (int i = 0; i < strs.Length; i++)
                {
                    strs[i] = RemoveBrackets(strs[i]);//移除括號
                }

                return strs.ToList();
            }
            catch (Exception ex)
            {
                ErrorLog.SaveErrorLog(ex);
                return new List<string>();
            }
        }

        /// <summary>分割字串 並且回傳指定index的部分
        /// EX:
        /// string input="123;456;789;000", string mark=";",int index=2
        /// 則 return "789"
        /// 
        /// </summary>
        /// <param name="input">要分割的字串</param>
        /// <param name="mark">分割符號</param>
        /// <param name="index">要截取的索引值 預設為0</param>
        /// <returns></returns>
        public static string GetSplit(string input, string mark, int index = 0)
        {
            var list = GetList_Split(input, mark);
            if (list.Count > index)
            {
                return list[index];
            }
            return "";
        }

        /// <summary>判斷string List內是否有Value值? 如果沒有則加入此Value 但是空值不會加入
        /// 
        /// </summary>
        /// <param name="ItemList"></param>
        /// <param name="Value"></param>
        /// <returns></returns>
        public static List<string> AddList_IfNotExist(List<string> ItemList, string Value)
        {
            if (Utility_String.IsNullValue(Value))
                return ItemList;//如果Value值為空值或--- 則不處理

            if (ItemList.Exists(x => x.Trim() == Value.Trim()))
            {
                return ItemList;//存在相同的值 不處理
            }
            ItemList.Add(Value.Trim());

            return ItemList;
        }

        /// <summary>如果需要以字串拼接的方式來帶入參數 則需要替換掉單引號 避免隱碼攻擊
        /// 
        /// </summary>
        /// <param name="Param"></param>
        /// <returns></returns>
        public static string CleanSQLParam(string Param)
        {
            if (Param == null)
                return "";

            return Param.Replace("'", "''");
        }

        /// <summary>如果資料為空值，回傳 "---"
        /// 
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static string GetEmptyString(string str)
        {
            if (string.IsNullOrWhiteSpace(str))
                return "---";
            else
                return str;
        }

        /// <summary>取得子字串,如果超出範圍則回傳""
        /// 
        /// </summary>
        /// <param name="input"></param>
        /// <param name="beg"></param>
        /// <param name="length"></param>
        /// <returns></returns>
        public static string Get_SubString(string input, int beg, int length)
        {
            if (input == null)
                return "";
            if (length < 1)
            {
                return "";
            }
            //起始index不可小於0
            if (beg < 0)
            {
                beg = 0;
            }
            //如果在字串範圍內
            if (input.Length >= beg + length)
                return input.Substring(beg, length);
            else if (input.Length > beg)
            {   //如果指定長度已經超過字串範圍 則取後面的
                return input.Substring(beg);
            }
            else
            {
                return "";
            }
        }

        /// <summary>取得橫線後面的值
        /// 
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public static string Get_SubString_AfterDash(string input)
        {

            var list = GetList_Split(input, "-");
            if (list.Count > 1)
            {
                return list[1];
            }
            return "";
        }

        /// <summary>特殊:根據輸入的審查報告編號取得對應的法規項目代碼
        /// EX:
        /// A99CCBKU-340 => 340
        /// A99CCBKW-340A => 340
        /// </summary>
        /// <param name="input">審查報告編號</param>
        /// <returns></returns>
        public static string Get_SubString_VSTDItemCode(string input)
        {
            var spList = GetList_Split(input, "-");

            string itemcode = "";

            if (spList.Count > 1)
                itemcode = spList[1];

            //取前三碼
            itemcode = Get_SubString(itemcode, 0, 3);

            return itemcode;
        }

        /// <summary>特殊:根據輸入的審查報告編號取得對應的法規項目代碼
        /// EX:
        /// A99CCBKU-340 => A99CCBKU
        /// A99CCBKW-340A => A99CCBKW
        /// </summary>
        /// <param name="input">審查報告編號</param>
        /// <returns></returns>
        public static string Get_SubString_VSTDCaseID(string input)
        {
            var spList = GetList_Split(input, "-");


            if (spList.Count > 0)
                return spList[0];
            else
                return "";
        }

        /// <summary>根據指定的開始與結尾標記 取得中間的字串內容
        /// 
        /// </summary>
        /// <param name="input">輸入的文字</param>
        /// <param name="begMark">開始標記</param>
        /// <param name="endMark">結尾標記</param>
        /// <returns>中間的字串內容</returns>
        public static string Get_Between(string input, string begMark, string endMark)
        {
            try
            {
                //檢查開始標記
                if (string.IsNullOrEmpty(input))
                    return "";

                //取得substring 的起始
                int beg = input.IndexOf(begMark);
                if (beg == -1)
                    return "";

                beg = beg + begMark.Length;


                int end = input.IndexOf(endMark);
                if (end == -1)
                    return "";

                int length = end - beg;

                //取得substring
                return input.Substring(beg, length);
            }
            catch 
            {
                return string.Empty;
            }
        }

        /// <summary>如果為空字串則填入"---"
        /// 
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public static string Get_EmptyFillDash(string input)
        {   //如果為空字串則填入"---"
            return (string.IsNullOrWhiteSpace(input)) ? "---" : input;
        }

        /// <summary>如果為空字串則填入"---"
        /// 
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public static string EmptyFillDash(this string input)
        {
            //如果為空字串則填入"---"
            return (string.IsNullOrWhiteSpace(input)) ? "---" : input;
        }


        /// <summary>檢查字串是否無值 會偵測到 null, "", "---", " "等狀況
        /// 
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public static bool IsNullValue(string input)
        {
            if (string.IsNullOrWhiteSpace(input))
                return true;

            //排除掉 ---
            input = input.Replace("-", "");

            if (string.IsNullOrWhiteSpace(input))
                return true;

            return false;

        }

        /// <summary>去空白
        /// 
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public static string Trim(string input)
        {
            if (input == null)
                return "";
            return input.Trim();
        }

        /// <summary>資料補值方法
        /// 條件:
        /// 若原始值有值 則回傳原始值
        /// 否則 
        ///     若備原值有值 則會回傳備原值 
        /// 否則
        ///     還是回傳原始值
        /// 
        /// </summary>
        /// <param name="Value_Origion">原始值</param>
        /// <param name="Value_Support">備原值(若原始值無值 則使用備原值)</param>
        /// <returns></returns>
        public static string ValueSupport(string Value_Origion, string Value_Support)
        {
            //原始值有值
            if (!IsNullValue(Value_Origion))
                return Value_Origion;
            //不然改用備原值
            if (!IsNullValue(Value_Support))
                return Value_Support;

            return Value_Origion;
        }

        /// <summary>根據輸入的字串列 排除掉重複的並回傳
        /// 
        /// </summary>
        /// <returns></returns>
        public static List<string> GetList_Distinct(List<string> inputList)
        {
            //逐筆查詢底盤型式 取得底盤登錄的報告
            List<string> DistinctList = new List<string>();
            foreach (var BaseType in inputList)
            {
                //判斷是否存在?
                if (DistinctList.Exists(x => x == BaseType))
                    continue;//存在則不加入
                //加入
                DistinctList.Add(BaseType);
            }
            return DistinctList;
        }

        /// <summary>根據List string拼接SQL查詢需要的 in 條件的字串
        /// EX: [test][test2][test3][test4] => 'test','test2','test3','test4'
        /// </summary>
        /// <param name="SafeEquip_List"></param>
        /// <param name="condStr"></param>
        /// <returns></returns>
        public static string GetString_SQL_In_Condition(List<string> SafeEquip_List)
        {
            string condStr = "";
            for (int i = 0; i < SafeEquip_List.Count; i++)
            {
                var str = SafeEquip_List[i] ?? "";
                str = str.Trim();
                if (i != 0)
                {
                    condStr += ",";
                }
                condStr += "'" + str + "'";

            }
            return condStr;
        }


        /// <summary>根據字串列 與指定標記 回傳拼接後的結果
        /// Ex: [AAA][BBB][CCC], "、" => AAA、BBB、CCC
        /// </summary>
        /// <param name="inputList">輸入</param>
        /// <param name="mark">分隔符號</param>
        /// <param name="isSkipNullValue">是否省略無值標記?</param>
        /// <returns></returns>
        public static string GetString_By_StringList(List<string> inputList, string mark, bool isSkipNullValue = true)
        {
            if (inputList == null)
                return "";

            //拼接字串
            string Output = "";
            foreach (var item in inputList)
            {
                //排除掉無值輸入的情況
                if (isSkipNullValue && Utility_String.IsNullValue(item))
                {
                    continue;//略過此筆資料
                }
                //如果不是空字串 則加入分隔符號
                if (Output != "")
                    Output += mark;
                //拼接字串
                Output += item;
            }
            return Output;
        }

        /// <summary>比較字串是否相等 會排除掉頭尾空白的問題
        /// 
        /// </summary>
        /// <param name="inputA"></param>
        /// <param name="inputB"></param>
        /// <returns></returns>
        public static bool Compare(string inputA, string inputB)
        {
            //若為空值 則補為空字串 避免trim出錯
            if (inputA == null)
                inputA = "";
            if (inputB == null)
                inputB = "";

            if (inputA.Trim() == inputB.Trim())
            {
                return true;
            }

            return false;
        }

        /// <summary>取得字串右半部開始的部分
        /// 
        /// </summary>
        /// <param name="s"></param>
        /// <param name="length"></param>
        /// <returns></returns>
        public static string Right(this string s, int length)
        {
            length = Math.Max(length, 0);

            if (s.Length > length)
            {
                return s.Substring(s.Length - length, length);
            }
            else
            {
                return s;
            }
        }

        /// <summary>檢查是否存在指定字串(相當於string內建的Contains 但是會自動排除掉null值 並且將搜尋目標自動作去除頭尾空白的動作)
        /// 如果input 或 Target本身為空值或null 則會回傳fasle
        /// </summary>
        /// <param name="input"></param>
        /// <param name="Target"></param>
        /// <returns></returns>
        public static bool Contains(string input, string Target)
        {
            //如果input 或 Target本身為空值或null 則會回傳fasle
            if (string.IsNullOrEmpty(input))
                return false;
            if (string.IsNullOrEmpty(Target))
                return false;

            //回傳Contains的結果
            return input.Contains(Target.Trim());
        }

        /// <summary>字串轉換成int32
        /// 
        /// </summary>
        /// <param name="s"></param>
        /// <returns></returns>
        public static int ToInt32(this string s)
        {
            int result = 0;
            int.TryParse(s, out result);
            return result;
        }


    }
}