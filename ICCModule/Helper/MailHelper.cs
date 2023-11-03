using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Configuration;
using System.Net.Mail;
using ICCModule.Helper;
using IscomG2C.Utility;
using System.Net;
using System.IO;
using System.Net.Mime;

namespace ICCModule.Helper
{
    /// <summary>寄信模組
    /// 
    /// </summary>
    public class MailHelper
    {
        /// <summary>信件伺服器位址
        /// 
        /// </summary>
        public string m_MailServerHost { get; set; }
        /// <summary>信件伺服器帳號
        /// 
        /// </summary>
        public string m_MailServerAccount { get; set; }
        /// <summary>信件伺服器密碼
        /// 
        /// </summary>
        public string m_MailServerLoginPass { get; set; }
        /// <summary>是否認證
        /// 
        /// </summary>
        public bool m_IsCredentials { get; set; }
        /// <summary>寄件者信箱
        /// 
        /// </summary>
        public string m_SenderAddress { get; set; }

        /// <summary>port
        /// 
        /// </summary>
        public Int32 m_MailServerPort { set; get; }

        /// <summary>
        /// 寄件者名稱
        /// </summary>
        public string m_SenderName { get; set; }

        //public string m_DisplayName { get; set; }

        /// <summary>
        /// port
        /// </summary>
        //public int m_Port { get; set; }

        public MailHelper()
        {
            this.m_MailServerHost = AppSettingHandler.GetAppSetting(AppSettingHandler.AppSetting.SMTP_Server);
            this.m_MailServerAccount = AppSettingHandler.GetAppSetting(AppSettingHandler.AppSetting.SMTP_Account);
            this.m_MailServerLoginPass = AppSettingHandler.GetAppSetting(AppSettingHandler.AppSetting.SMTP_Password);
            this.m_IsCredentials = true;
            this.m_SenderAddress = AppSettingHandler.GetAppSetting(AppSettingHandler.AppSetting.SMTP_Sender);
            this.m_MailServerPort = Convert.ToInt32(AppSettingHandler.GetAppSetting(AppSettingHandler.AppSetting.SMTP_Port));
            this.m_SenderName = AppSettingHandler.GetAppSetting(AppSettingHandler.AppSetting.SMTP_displayName);
        }

        public MailHelper(string m_MailServerHost, string m_MailServerAccount, string m_MailServerPassword, string m_SenderAddress, bool m_IsCredentials = false)
        {
            this.m_MailServerHost = m_MailServerHost;
            this.m_MailServerAccount = m_MailServerAccount;
            this.m_MailServerLoginPass = m_MailServerPassword;
            this.m_IsCredentials = m_IsCredentials;
            this.m_SenderAddress = m_SenderAddress;
        }

        /// <summary>寄信(直接從Web.Config取得相關參數 僅需要指定 收件者 主題 內容 即可)
        /// 
        /// </summary>
        /// <param name="sErrMsg"></param>
        /// <param name="TargetMail">目標信箱</param>
        /// <param name="Subject">主題</param>
        /// <param name="MailBody">內文(含HTML語法)</param>
        /// <returns></returns>
        public bool SendMail(ref string sErrMsg, List<string> TargetMail, string Subject, string MailBody)
        {
            //若寄件功能未被啟用 為了偵錯方便 可能暫時關掉
            if (AppSettingHelper.Get_SMTP_SendOrNot() == false)
            {
                sErrMsg = "已關閉寄件功能";
                return false;
            }

            MailHandler m_MailHelper = new MailHandler();
            //設定郵件伺服器資訊
            m_MailHelper.MailServerHost = this.m_MailServerHost;
            m_MailHelper.MailServerPort = this.m_MailServerPort;
            m_MailHelper.SmtpAccount = this.m_MailServerAccount;
            m_MailHelper.SmtpLoginPass = this.m_MailServerLoginPass;
            //啟用SSL憑證
            m_MailHelper.EnableSsl = true;
            //啟用認證
            m_MailHelper.IsCredentials = true;
            //允許 HTML 格式
            m_MailHelper.IsBodyHtml = true;
            try
            {
                //寄件者資訊
                m_MailHelper.MailFrom = new MailAddress(this.m_SenderAddress, this.m_SenderName);
                //信件主旨
                m_MailHelper.MailSubject = Subject;
                m_MailHelper.MailBody = MailBody;

                //設定收件者
                MailAddressCollection _toMailCollection = new MailAddressCollection();
                //測試模式嗎?
                bool isDebug_Mode = AppSettingHelper.Get_Debug_Mode();
                //如果是測試模式 則信件不會被正式寄出 而是寄給指定的測試用信相
                if (false)
                {
                    //取得測試用信件
                    string mailAddress = AppSettingHelper.Get_SMTP_DebugTarget();
                    if (string.IsNullOrWhiteSpace(mailAddress))
                    {   //沒有定義測試用信箱 不處理了...
                        return false;
                    }

                    _toMailCollection.Add(new MailAddress(mailAddress));
                    m_MailHelper.MailTo = _toMailCollection;
                }
                else
                {   //否則的話 將真正要寄送的信箱位置寫入

                    //逐筆解析目標信件
                    foreach (var mailAddress in TargetMail)
                    {
                        //篩選一下格式
                        if (string.IsNullOrWhiteSpace(mailAddress))
                            continue;
                        if (mailAddress.Contains("@") == false)
                            continue;//沒有@ 排除
                        //加入信箱
                        _toMailCollection.Add(new MailAddress(mailAddress));
                    }

                    m_MailHelper.MailTo = _toMailCollection;
                }
                //信件內容
                AlternateView _alterView = AlternateView.CreateAlternateViewFromString(m_MailHelper.MailBody, null, "text/html");
                //_alterView.LinkedResources.Add(logo);
                m_MailHelper.MailView = _alterView;
                m_MailHelper.Send(m_MailHelper, ref sErrMsg);
                if (!string.IsNullOrEmpty(sErrMsg))
                {
                    ErrorLog.SaveErrorLog(sErrMsg, "MailHelper");
                }
            }
            catch (Exception ex)
            {
                ErrorLog.SaveErrorLog(ex, "MailHelper");
                sErrMsg = "寄件錯誤!" + ex.ToString();
                return false;
            }
            return true;
        }

        /// <summary>寄信(直接從Web.Config取得相關參數 僅需要指定 收件者 主題 內容 即可)
        /// 
        /// </summary>
        /// <param name="sErrMsg"></param>
        /// <param name="TargetMail">目標信箱</param>
        /// <param name="Subject">主題</param>
        /// <param name="MailBody">內文(含HTML語法)</param>
        /// <returns></returns>
        public bool SendMail(ref string sErrMsg, string TargetMail, string Subject, string MailBody)
        {
            //沒有設定目標信箱
            if (string.IsNullOrWhiteSpace(TargetMail)
                || TargetMail == "@")
            {
                sErrMsg = "沒有設定目標信箱";
                return false;
            }

            List<string> TargetMailList = new List<string>();
            TargetMailList.Add(TargetMail);
            return SendMail(ref sErrMsg, TargetMailList, Subject, MailBody);
        }
        public bool Send(ref string sErrMsg, string address, string subject, string body)
        {
            try
            {
                //<!-- (寄信者, 收信者) -->
                MailMessage message = new MailMessage(this.m_MailServerAccount, address);
                message.IsBodyHtml = true;
                // <!-- E-mail編碼 -->
                message.BodyEncoding = System.Text.Encoding.UTF8;
                // <!-- E-mail主旨 -->
                message.Subject = subject;
                //<!-- 優先權 -->
                message.Priority = MailPriority.Normal;
                //<!-- E-mail內容 -->
                message.Body = body;
                using (SmtpClient smtp = new SmtpClient(this.m_MailServerHost, this.m_MailServerPort))
                {

                    smtp.Credentials = new NetworkCredential(this.m_MailServerAccount, this.m_MailServerLoginPass);
                    smtp.EnableSsl = true;
                    smtp.Send(message);
                }
                return true;
            }
            catch (Exception ex)
            {
                ErrorLog.SaveErrorLog(ex, "MailHelper");
                sErrMsg = "寄件錯誤!" + ex.ToString();
                return false;
            }
        }

        /// <summary>寄信(直接從Web.Config取得相關參數 僅需要指定 收件者 主題 內容 即可)
        /// 
        /// </summary>
        /// <param name="sErrMsg"></param>
        /// <param name="TargetMail">目標信箱</param>
        /// <param name="Subject">主題</param>
        /// <param name="MailBody">內文(含HTML語法)</param>
        /// <param name="filePaths">附檔在WebServer檔案總管路徑</param>
        /// <param name="deleteFileAttachment">是否刪除在WebServer上的附件</param>
        /// <returns></returns>
        public static bool SendMail(ref string sErrMsg, List<string> TargetMail, string Subject, string MailBody, MemoryStream _files, ContentType _contentType)
        {
            var ms_MailServerHost = AppSettingHandler.GetAppSetting(AppSettingHandler.AppSetting.SMTP_Server);
            var ms_MailServerAccount = AppSettingHandler.GetAppSetting(AppSettingHandler.AppSetting.SMTP_Account);
            var ms_MailServerLoginPass = AppSettingHandler.GetAppSetting(AppSettingHandler.AppSetting.SMTP_Password);
            var ms_IsCredentials = true;
            var ms_SenderAddress = AppSettingHandler.GetAppSetting(AppSettingHandler.AppSetting.SMTP_Sender);
            var ms_MailServerPort = Convert.ToInt32(AppSettingHandler.GetAppSetting(AppSettingHandler.AppSetting.SMTP_Port));
            var ms_SenderName = AppSettingHandler.GetAppSetting(AppSettingHandler.AppSetting.SMTP_displayName);

            //若寄件功能未被啟用 為了偵錯方便 可能暫時關掉
            if (AppSettingHelper.Get_SMTP_SendOrNot() == false)
            {
                sErrMsg = "已關閉寄件功能";
                return false;
            }

            MailHandler m_MailHelper = new MailHandler();
            //設定郵件伺服器資訊
            m_MailHelper.MailServerHost = ms_MailServerHost;
            m_MailHelper.MailServerPort = ms_MailServerPort;
            m_MailHelper.SmtpAccount = ms_MailServerAccount;
            m_MailHelper.SmtpLoginPass = ms_MailServerLoginPass;
            //啟用SSL憑證
            m_MailHelper.EnableSsl = true;
            //啟用認證
            m_MailHelper.IsCredentials = true;
            //允許 HTML 格式
            m_MailHelper.IsBodyHtml = true;
            try
            {
                //寄件者資訊
                m_MailHelper.MailFrom = new MailAddress(ms_SenderAddress, ms_SenderName);
                //信件主旨
                m_MailHelper.MailSubject = Subject;
                m_MailHelper.MailBody = MailBody;

                //設定收件者
                MailAddressCollection _toMailCollection = new MailAddressCollection();
                //測試模式嗎?
                bool isDebug_Mode = AppSettingHelper.Get_Debug_Mode();
                //如果是測試模式 則信件不會被正式寄出 而是寄給指定的測試用信相
                if (false)
                {
                    //取得測試用信件
                    string mailAddress = AppSettingHelper.Get_SMTP_DebugTarget();
                    if (string.IsNullOrWhiteSpace(mailAddress))
                    {   //沒有定義測試用信箱 不處理了...
                        return false;
                    }

                    _toMailCollection.Add(new MailAddress(mailAddress));
                    m_MailHelper.MailTo = _toMailCollection;
                }
                else
                {   //否則的話 將真正要寄送的信箱位置寫入

                    //逐筆解析目標信件
                    foreach (var mailAddress in TargetMail)
                    {
                        //篩選一下格式
                        if (string.IsNullOrWhiteSpace(mailAddress))
                            continue;
                        if (mailAddress.Contains("@") == false)
                            continue;//沒有@ 排除
                        //加入信箱
                        _toMailCollection.Add(new MailAddress(mailAddress));
                    }

                    m_MailHelper.MailTo = _toMailCollection;
                }

                if (_files != null)
                {
                    var _attachment = new Attachment(_files, _contentType);
                    m_MailHelper._lstFile = new List<Attachment> { _attachment };
                }

                //信件內容
                AlternateView _alterView = AlternateView.CreateAlternateViewFromString(m_MailHelper.MailBody, null, "text/html");
                //_alterView.LinkedResources.Add(logo);
                m_MailHelper.MailView = _alterView;
                m_MailHelper.Send(m_MailHelper, ref sErrMsg);
                if (!string.IsNullOrEmpty(sErrMsg))
                {
                    ErrorLog.SaveErrorLog(sErrMsg, "MailHelper");
                }
                if (m_MailHelper._lstFile != null)
                    foreach (var item in m_MailHelper._lstFile)
                    {
                        item.Dispose();
                    }
            }
            catch (Exception ex)
            {
                ErrorLog.SaveErrorLog(ex, "MailHelper");
                sErrMsg = "寄件錯誤!" + ex.ToString();

                if (m_MailHelper._lstFile != null)
                    foreach (var item in m_MailHelper._lstFile)
                    {
                        item.Dispose();
                    }
                return false;
            }
            return true;
        }

    }
}
