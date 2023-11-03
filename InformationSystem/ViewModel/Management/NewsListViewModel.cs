using ICCModule.Entity.Tables;
using ICCModule.Entity.Tables.Platform;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace InformationSystem.ViewModel.Management
{
    public class NewsListViewModel
    {
        /// <summary>
        /// 類別查詢
        /// </summary>
        public List<defCode> Types { get; set; }

        /// <summary>
        /// 消息列表
        /// </summary>
        public List<New> News { get; set; }

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
        public int Page { get; set; }
        /// <summary>
        /// 篩選條件
        /// </summary>
        public NewsFilter NewsFilters { get; set; } = new NewsFilter();
    }

    public class NewsFilter
    {
        /// <summary>
        /// 類別查詢
        /// </summary>
        public string Code { get; set; }
        /// <summary>
        /// 公告狀態
        /// </summary>
        public bool? Show { get; set; }
        /// <summary>
        /// 公告方式(true=彈窗)
        /// </summary>
        public bool? PopupShow { get; set; }

    }
}