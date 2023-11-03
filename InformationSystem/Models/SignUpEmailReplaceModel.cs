using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace InformationSystem.Models
{
    public class SignUpEmailReplaceModel
    {
        /// <summary>
        /// 收件人
        /// </summary>
        public string NameRecipient { get; set; }

        /// <summary>
        /// 帳號驗證Url
        /// </summary>
        public string AccountVerifyUrl { get; set; }

        /// <summary>
        /// 網站網址
        /// </summary>
        public string WebsiteUrl { get; set; }
    }
}