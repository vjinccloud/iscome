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
using System.Web.UI.WebControls;

namespace InformationSystem.ViewModel
{
    /// <summary>
    /// 
    /// </summary>
    public class LineBroadcastModel
    {
        /// <summary>
        /// 查詢內容
        /// </summary>
        public LineBroadcastQueryModel QueryReq { get; set; } = new LineBroadcastQueryModel();
        /// <summary>
        /// 資料
        /// </summary>
        public List<LineBroadcastShowModel> Data { get; set; } = new List<LineBroadcastShowModel>();
        /// <summary>
        /// 統計資料
        /// </summary>
        public List<LineBroadcastStasticModel> StasticData { get; set; } = new List<LineBroadcastStasticModel>();
        /// <summary>
        /// 統計資料
        /// </summary>
        public List<defCode> MessageType { get; set; } = Service_defCode.GetList("LineBroadcast");
        /// <summary>
        /// 資料頁數
        /// </summary>
        public int TotalPage { get; set; }
    }
    /// <summary>
    /// 查詢
    /// </summary>
    public class LineBroadcastQueryModel
    {
        /// <summary>
        /// 推播類型
        /// </summary>
        public string MessageType { get; set; }
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
    public class LineBroadcastShowModel
    {
        /// <summary> 
        ///推播類別
        /// </summary>
        public string MessageType { get; set; }

        /// <summary> 
        /// 推播對象
        /// </summary>
        public string MessageTarget { get; set; }

        /// <summary> 
        /// 對象人數
        /// </summary>
        public int TargetUserCount { get; set; }

        /// <summary> 
        /// 訊息
        /// </summary>
        public string Contents { get; set; }

        /// <summary> 
        /// 日期起日
        /// </summary>
        public DateTime CreateDate { get; set; }
    }
    public class LineBroadcastStasticModel
    {
        /// <summary>
        /// 月份
        /// </summary>
        public DateTime Date { get; set; }
        /// <summary>
        /// 統計次數
        /// </summary>
        public  int Count { get; set; }
    }

    public class AddBroadcastModel
    {
        public List<defCode> MessageTypes { get; set; } = Service_defCode.GetList("LineBroadcast");
        public List<ListItem> Members { get; set; } = Service_MemberInfo.GetList(x => x.LineMessageId != null).Where(x => !string.IsNullOrEmpty(x.LineMessageId.Trim())).Select(x => new ListItem { Text = x.Name, Value = x.ID.ToString() }).ToList();
        public LineBroadcast Data { get; set; } = new LineBroadcast();
    }
}