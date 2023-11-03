using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net.Mail;

namespace ICCModule.Helper
{
    [Serializable]
    public class MailHandler
    {
        /// <summary>
        /// 信件主旨
        /// </summary>
        public string MailSubject { get; set; }
        /// <summary>
        /// 寄件者集合
        /// </summary>
        public MailAddress MailFrom { get; set; }
        /// <summary>
        /// 收件者集合
        /// </summary>
        public MailAddressCollection MailTo { get; set; }
        /// <summary>
        /// 信件內容
        /// </summary>
        public string MailBody { get; set; }
        /// <summary>
        /// CC 集合
        /// </summary>
        public MailAddressCollection MailCC { get; set; }
        /// <summary>
        /// 信件 View
        /// </summary>
        public AlternateView MailView { get; set; }
        /// <summary>STMP是否需要帳號密碼認證? (視使用的SMTP設定而異)
        /// 
        /// </summary>
        public bool IsCredentials { get; set; }
        /// <summary>
        /// 信件是否為 Html
        /// </summary>
        public bool IsBodyHtml { get; set; }
        /// <summary>
        /// 是否允許 SSL
        /// </summary>
        public bool EnableSsl { get; set; }
        /// <summary>
        /// 郵件伺服器位址
        /// </summary>
        public string MailServerHost { get; set; }
        /// <summary>
        /// 郵件伺服器埠號
        /// </summary>
        public int MailServerPort { get; set; }
        /// <summary>
        /// Smtp 帳號
        /// </summary>
        public string SmtpAccount { get; set; }
        /// <summary>
        /// Smtp 密碼
        /// </summary>
        public string SmtpLoginPass { get; set; }
        /// <summary>
        /// 附帶檔案
        /// </summary>
        public List<Attachment> _lstFile { get; set; }


        /// <summary>
        /// 寄信
        /// </summary>
        /// <param name="model">MailHelper 物件</param>
        /// <param name="sErrMsg">錯誤訊息</param>
        public void Send(MailHandler model, ref string sErrMsg)
        {
            if (model != null)
            {
                SmtpClient smtpServer = new SmtpClient();
                smtpServer.EnableSsl = model.EnableSsl;
                smtpServer.Host = model.MailServerHost;
                smtpServer.Port = model.MailServerPort;
                if (model.IsCredentials)
                {
                    smtpServer.Credentials = new System.Net.NetworkCredential(model.SmtpAccount, model.SmtpLoginPass);
                }
                MailMessage _mailMessage = new MailMessage();
                _mailMessage.Subject = model.MailSubject;
                _mailMessage.Body = model.MailBody;
                _mailMessage.IsBodyHtml = model.IsBodyHtml;
                _mailMessage.AlternateViews.Add(model.MailView);
                foreach (var item in model.MailTo)
                {
                    _mailMessage.To.Add(item);
                }
                _mailMessage.From = model.MailFrom;
                if (null != model._lstFile)
                {
                    foreach (var item in model._lstFile)
                    {
                        _mailMessage.Attachments.Add(item);
                    }
                }
                if (null != model.MailCC)
                {
                    foreach (var item in model.MailCC)
                    {
                        _mailMessage.CC.Add(item);
                    }
                }

                try
                {
                    smtpServer.Send(_mailMessage);
                }
                catch (Exception ex)
                {
                    sErrMsg = ex.ToString();
                }
                finally
                {
                    if (_mailMessage.Attachments!=null)
                    {
                        foreach(var item in _mailMessage.Attachments)
                        {
                            item.Dispose();
                        }
                    }
                }
            }
        }
    }
}
