using ICCModule.Entity.Tables;
using ICCModule.Entity.Tables.Platform;
using ICCModule.EntityService.Service;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;

namespace Website.ViewModel
{
    public class LoginReq
    {
        /// <summary>
        /// 帳號
        /// </summary>
        public string Account { get; set; }
        
        /// <summary>
        /// 密碼
        /// </summary>
        public string LoginPass { get; set; }

        /// <summary>
        /// 驗證碼
        /// </summary>
        public string VerifyCode { get; set; }

        /// <summary>
        /// 登入方式(1-google/2-FB)
        /// </summary>
        public int LoginType { get; set; } = 0;

        /// <summary>
        /// 第三方序號
        /// </summary>
        public string LoginId { get; set; }
    }
}