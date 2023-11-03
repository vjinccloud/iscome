using ICCModule.Entity.Tables;
using ICCModule.Entity.Tables.Platform;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace InformationSystem.ViewModel.Management
{
    public class SysNewsModel
    {
        /// <summary>
        /// 消息列表
        /// </summary>
        public List<sysNews> SysNews { get; set; }

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
        public SysNewsFilter SysNewsFilters { get; set; } = new SysNewsFilter();
    }

    public class SysNewsFilter
    {
        /// <summary>
        /// 起始日期
        /// </summary>
        public DateTime? StartDate { get; set; }
        /// <summary>
        /// 結束日期
        /// </summary>
        public DateTime? EndDate { get; set; }
        /// <summary>
        /// 關鍵字
        /// </summary>
        public string Keyword { get; set; }
    }
}