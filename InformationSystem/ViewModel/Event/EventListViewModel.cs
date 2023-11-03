using ICCModule.Entity.Tables;
using ICCModule.Entity.Tables.Platform;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace InformationSystem.ViewModel.Event
{
    public class EventListViewModel
    {
        /// <summary>
        /// 類別查詢
        /// </summary>
        public List<defCode> Types { get; set; }

        /// <summary>
        /// 活動列表
        /// </summary>
        public List<activityPlan> Plans { get; set; }
        
        /// <summary>
        /// 查詢條件
        /// </summary>
        public EventQueryReq QueryReq { get; set; }

        /// <summary>
        /// 當前頁數
        /// </summary>
        public int CurrentPage { get; set; }

        /// <summary>
        /// 總頁數
        /// </summary>
        public int TotalPage { get; set; }

        /// <summary>
        /// 當前顯示筆數
        /// </summary>
        public int PageCounts { get; set; }
    }
    public class EventQueryReq
    {
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public string Code { get; set; } = "";
        public bool? Show { get; set; }
        public List<string> DeleteIDs = null;
    }
}