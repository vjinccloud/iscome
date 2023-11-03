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

namespace InformationSystem.ViewModel
{
    /// <summary>
    /// 
    /// </summary>
    public class LineModel
    {
        /// <summary>
        /// 查詢內容
        /// </summary>
        public LineQueryModel QueryReq { get; set; } = new LineQueryModel();
        /// <summary>
        /// 資料
        /// </summary>
        public List<LineMessageLogShowModel> Data { get; set; } = new List<LineMessageLogShowModel>();
        /// <summary>
        /// 資料頁數
        /// </summary>
        public int TotalPage { get; set; }
    }
    /// <summary>
    /// 查詢
    /// </summary>
    public class LineQueryModel
    {
        /// <summary>
        /// 查詢時間-起
        /// </summary>
        public DateTime? StartDate { get; set; }
        /// <summary>
        /// 查詢時間-迄
        /// </summary>
        public DateTime? EndDate { get; set; }
        public int Page { get; set; } = 1;
        public string Action { get; set; } = "";
    }
    /// <summary>
    /// 
    /// </summary>
    public class LineMessageLogShowModel
    {
        /// <summary> 
        ///會員帳號
        /// </summary>
        public string MemberAccount { get; set; }

        /// <summary> 
        /// 姓名
        /// </summary>
        public string Name { get; set; }

        /// <summary> 
        /// 訊息
        /// </summary>
        public string UserMessage { get; set; }

        /// <summary> 
        /// 日期起日
        /// </summary>
        public DateTime CreateDate { get; set; }
    }
}