using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace InformationSystem.Models
{
    public class ForgetEmailReplaceModel
    {
        /// <summary>
        /// 收件人
        /// </summary>
        public string NameRecipient { get; set; }

        /// <summary>
        /// 申請時間 yyyy/MM/dd HH:mm
        /// </summary>
        public string ApplicationTime { get; set; }

        /// <summary>
        /// 重設密碼網站網址
        /// </summary>
        public string ResetPasswordUrl { get; set; }
    }
}