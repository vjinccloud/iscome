using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Configuration;
using System.Web.Configuration;

namespace ICCModule.Helper
{
    [Serializable]
    public class AppSettingHelper
    {
        /// <summary>取得AppSetting值
        /// 
        /// </summary>
        /// <param name="Key"></param>
        /// <returns></returns>
        public static string GetAppSetting(string Key)
        {
            try
            {
                return WebConfigurationManager.AppSettings[Key];
            }
            catch (Exception ex)
            {
                //查無
                return "";
            }
        }

        public static string Get_SystemName()
        {
            return GetAppSetting("SystemName");
        }

        /// <summary>是否為偵錯模式? 1.登入不用檢查密碼 2.信件不會真的寄送給收件者 而是測試用信箱
        /// 
        /// </summary>
        /// <returns></returns>
        public static bool Get_Debug_Mode()
        {
            if (GetAppSetting("Debug_Mode").ToLower() == "false")
            {
                return false;
            }

            //預設為true 避免測試機有人打錯字...
            return true;
        }

        /// <summary>SMTP是否寄件?
        /// 
        /// </summary>
        /// <returns></returns>
        public static bool Get_SMTP_SendOrNot()
        {
            if (GetAppSetting("SMTP_SendOrNot").ToLower() != "true")
            {
                return false;
            }

            return true;
        }

        /// <summary>測試用的收信端信箱
        /// 
        /// </summary>
        /// <returns></returns>
        public static string Get_SMTP_DebugTarget()
        {
            return GetAppSetting("SMTP_DebugTarget");
        }

        /// <summary>
        /// 是否啟動維護狀態
        /// </summary>
        public static bool DownForMaintenance
            => GetAppSetting("DownForMaintenance")
                ?.Equals("true", StringComparison.CurrentCultureIgnoreCase) ?? false;
    }
}