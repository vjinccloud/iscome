using ICCModule.Entity.Tables;
using ICCModule.Entity.Tables.Platform;
using ICCModule.EntityService.Service;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;

namespace InformationSystem.ViewModel.Management
{
    public class ForgotPasswordViewModel
    {
        /// <summary>
        /// 信箱
        /// </summary>
        public string Email { get; set; }

        /// <summary>
        /// 驗證碼
        /// </summary>
        public string VerifyCode { get; set; }
    }
}