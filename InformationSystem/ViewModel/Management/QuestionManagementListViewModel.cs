using ICCModule.Entity.Tables;
using ICCModule.Entity.Tables.Platform;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace InformationSystem.ViewModel.Management
{
    public class QuestionManagementListViewModel
    {
        /// <summary>
        /// 類別查詢
        /// </summary>
        public List<defCode> Types { get; set; }

        /// <summary>
        /// 消息列表
        /// </summary>
        public List<QuestionManagement> List { get; set; }

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
}