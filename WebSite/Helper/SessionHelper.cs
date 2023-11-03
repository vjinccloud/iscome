using ICCModule.Entity.Tables;
using IscomG2C.Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Website
{
    /// <summary>
    /// Session幫手 
    /// 1.把Session存取的指令簡化 方便非Controller也能呼叫
    /// 2.針對讀取Session值做特別處理 避免過程中噴錯
    /// 3.將需要透過Session存取的參數統一控管,方便追蹤
    /// </summary>
    public class SessionHelper
    {
        public static void Set(string Key, object Value)
        {
            HttpContext.Current.Session[Key] = Value;
        }

        public static void Set(string Key, string Value)
        {
            HttpContext.Current.Session[Key] = Value;
        }

        public static string Get(string Key)
        {
            if (HttpContext.Current.Session[Key] == null)
                return "";
            try
            {
                return HttpContext.Current.Session[Key].ToString();
            }
            catch (Exception ex)
            {
                ErrorLog.SaveErrorLog(ex, "SessionHelper");
                return "";
            }
        }

        /// <summary>
        /// 取得-登入的帳號
        /// </summary>
        /// <returns></returns>
        public static string Get_LoginID()
        { 
            return Get("MemberID"); 
        }

        public static string Get_UserName()
        { 
            return Get("UserName"); 
        }

        /// <summary>
        /// 取得-目前打開的選單
        /// </summary>
        /// <returns></returns>
        public static string Get_MenuID()
        { return Get("MenuID"); }

        /// <summary>
        /// 取得-當前可存取的功能清單
        /// </summary>
        /// <returns></returns>
        public static List<VW_FuntionMenu_LV2> Get_Authed_FuntionMenu()
        {
            if (HttpContext.Current.Session["Authed_FuntionMenu"] == null)
                return null;
            try
            {
                return (List<VW_FuntionMenu_LV2>)HttpContext.Current.Session["Authed_FuntionMenu"];
            }
            catch (Exception ex)
            {
                ErrorLog.SaveErrorLog(ex, "SessionHelper");
                return null;
            }
        }
    }
}