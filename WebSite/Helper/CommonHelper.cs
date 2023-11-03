using ICCModule.Helper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Website.Helper
{
    public static class CommonHelper
    {
        /// <summary>檢查與Session值內的驗證碼是否符合?
        /// 
        /// </summary>
        /// <param name="m_sValidate"></param>
        /// <returns></returns>
        public static bool Check_VerifyCode(string m_sValidate, string session_key = "LoginValidate")
        {
            m_sValidate = m_sValidate ?? "";
            //抓取偵錯模式標記
            bool bDebug = AppSettingHelper.Get_Debug_Mode();

            //如果是測試模式 則略過驗證碼
            if (bDebug == true)
                return true;

            string value = HttpContext.Current.Session[session_key].ToString();

            //如果不是測試模式 有輸入驗證碼 且 驗證碼符合
            if (value == null)
                return false;

            if (value.ToUpper() != m_sValidate.ToUpper())
                return false;

            //通過檢查驗證碼檢查
            return true;
        }
    }
}