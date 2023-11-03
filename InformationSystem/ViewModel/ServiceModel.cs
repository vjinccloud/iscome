using ICCModule.Entity.Tables;
using ICCModule.Entity.Tables.Platform;
using ICCModule.EntityService.Service;
using ICCModule.Helper;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace InformationSystem.ViewModel
{
    public class ResponseModel
    {
        /// <summary>
        /// 是否成功
        /// </summary>
        public bool Success { get; set; } = false;
        /// <summary>
        /// 訊息
        /// </summary>
        public string Msg { get; set; } = "";
        /// <summary>
        /// 資料
        /// </summary>
        public dynamic Data { get; set; }
    }
    public class ServiceQueryModel
    {        
        /// <summary>
        /// 帳號
        /// </summary>
        [Required]
        public string Account { get; set; }
        /// <summary>
        /// 密碼
        /// </summary>
        [Required]
        public string LoginPass { get; set; }
        /// <summary>
        /// 類型(1:小黃薊馬/2:瓜實蠅/3:東方果實蠅)
        /// </summary>
        [Required]
        public int SearchType { get; set; }
        /// <summary>
        /// 查詢區間-起日
        /// </summary>
        public DateTime? StartDate { get; set; }
        /// <summary>
        /// 查詢區間-迄日
        /// </summary>
        public DateTime? EndDate { get; set; }
    }
}