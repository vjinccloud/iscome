using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace InformationSystem.Models
{
    public class AdminEmailReplaceModel
    {
        /// <summary>
        /// 收件人
        /// </summary>
        public string NameRecipient { get; set; }

        /// <summary>
        /// 帳號
        /// </summary>
        public string Account { get; set; }

        /// <summary>
        /// 密碼
        /// </summary>
        public string LoginPass { get; set; }

        /// <summary>
        /// 網站網址
        /// </summary>
        public string WebsiteUrl { get; set; }
    }
}