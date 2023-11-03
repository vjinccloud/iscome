using ICCModule.Entity.Tables;
using ICCModule.Entity.Tables.Platform;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace InformationSystem.ViewModel.Management
{
    public class BusinessFormListViewModel
    {
        /// <summary>
        /// 權限查詢
        /// </summary>
        public List<sysRole> Types { get; set; }

        /// <summary>
        /// 表單列表
        /// </summary>
        public List<BusinessForm> BusinessForms { get; set; }

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
        public bool? Show { get; set; }
        public List<string> Codes { get; set; }
    }
}