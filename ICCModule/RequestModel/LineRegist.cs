using ICCModule.Entity.Tables;
using ICCModule.Entity.Tables.Platform;
using ICCModule.EntityService.Service;
using ICCModule.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;

namespace ICCModule.RequestModel
{
    /// <summary>
    /// 
    /// </summary>
    public class LineRegist
    {
        /// <summary>
        /// 姓名
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// 帳號
        /// </summary>
        public string LoginID { get; set; }
        /// <summary>
        /// 手機
        /// </summary>
        public string Mobile { get; set; }
        /// <summary>
        /// 所在縣市
        /// </summary>
        public string City { get; set; }
        /// <summary>
        /// 所在區域
        /// </summary>
        public string District { get; set; }
        /// <summary>
        /// 身分別
        /// </summary>
        public string Identify { get; set; }
        /// <summary>
        /// 密碼
        /// </summary>
        public string Pwd { get; set; }
        /// <summary>
        /// 驗證密碼
        /// </summary>
        public string CheckPwd { get; set; }
        /// <summary>
        /// 驗證碼
        /// </summary>
        public string CheckCode { get; set; }
        /// <summary>
        /// 
        /// 
        /// </summary>
        public string GoogleId { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string FacebookId { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string LineId { get; set; }
    }
}