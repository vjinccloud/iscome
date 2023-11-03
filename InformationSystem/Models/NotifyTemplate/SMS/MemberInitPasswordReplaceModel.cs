using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace InformationSystem.Models.NotifyTemplate.SMS
{
    public class MemberInitPasswordReplaceModel
    {
        /// <summary>
        /// 帳號
        /// </summary>
        public string Account { get; set; }

        /// <summary>
        /// 網站
        /// </summary>
        public string WebUrl { get; set; }

        /// <summary>
        /// 初始密碼
        /// </summary>
        public string LoginPass { get; set; }
    }
}