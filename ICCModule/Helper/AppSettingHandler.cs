using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Reflection;
using System.Configuration;

namespace ICCModule.Helper
{
    [Serializable]
    public class AppSettingHandler
    {
        public enum AppSetting
        {
            /// <summary>
            /// SMTP是否啟用
            /// </summary>
            SMTP_IsActive,
            /// <summary>
            /// SMTP主機
            /// </summary>
            SMTP_Server,
            /// <summary>
            /// Port號碼
            /// </summary>
            SMTP_Port,
            /// <summary>
            /// SMTP登入帳號
            /// </summary>
            SMTP_Account,
            /// <summary>
            /// SMTP登入密碼
            /// </summary>
            SMTP_Password,
            /// <summary>
            /// 寄件者信箱
            /// </summary>
            SMTP_Sender,
            /// <summary>
            /// 是否寄信
            /// </summary>
            SMTP_SendOrNot,
            /// <summary>
            /// TBN信件名稱
            /// </summary>
            SMTP_displayName,
            /// <summary>
            /// 每日
            /// </summary>
            DailySend,
            /// <summary>
            /// 每週
            /// </summary>
            WeekSend,
        }

        /// <summary>
        /// 取得指定名稱之AppSetting中之資料
        /// </summary>
        /// <param name="app">enum AppSetting</param>
        /// <returns></returns>
        public static string GetAppSetting(AppSetting app)
        {
            string _return = "";
            switch (app)
            {
                case AppSetting.SMTP_IsActive:
                    _return = ConfigurationManager.AppSettings["SMTP_IsActive"].ToString();
                    break;
                case AppSetting.SMTP_Server:
                    _return = ConfigurationManager.AppSettings["SMTP_Server"].ToString();
                    break;
                case AppSetting.SMTP_Port:
                    _return = ConfigurationManager.AppSettings["SMTP_Port"].ToString();
                    //_return = GetAppSetting("SMTP_Port").ToString();
                    break;
                case AppSetting.SMTP_Account:
                    _return = ConfigurationManager.AppSettings["SMTP_Account"].ToString();
                    //_return = GetAppSetting("SMTP_Account").ToString();
                    break;
                case AppSetting.SMTP_Password:
                    _return = ConfigurationManager.AppSettings["SMTP_Password"].ToString();
                    //_return = GetAppSetting("SMTP_Password").ToString();
                    break;
                case AppSetting.SMTP_Sender:
                    _return = ConfigurationManager.AppSettings["SMTP_Sender"].ToString();
                    //_return = GetAppSetting("SMTP_Sender").ToString();
                    break;
                case AppSetting.SMTP_SendOrNot:
                    _return = ConfigurationManager.AppSettings["SMTP_SendOrNot"].ToString();
                    //_return = GetAppSetting("SMTP_SendOrNot").ToString();
                    break;
                case AppSetting.SMTP_displayName:
                    _return = ConfigurationManager.AppSettings["SMTP_displayName"].ToString();
                    //_return = GetAppSetting("SMTP_displayName").ToString();
                    break;
                case AppSetting.DailySend:
                    _return = ConfigurationManager.AppSettings["DailySend"].ToString();
                    
                    break;
                case AppSetting.WeekSend:
                    _return = ConfigurationManager.AppSettings["WeekSend"].ToString();
                    break;
            }
            return _return;
        }
    }
}
