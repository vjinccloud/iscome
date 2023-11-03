using ICCModule.Entity.Tables;
using ICCModule.Entity.Tables.Platform;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace InformationSystem.ViewModel.Management
{
    public class PlantListViewModel
    {
        /// <summary>
        /// 作物列表
        /// </summary>
        public List<crops> crops { get; set; }

        /// <summary>
        /// 作物類別序號
        /// </summary>
        public int TypeId { get; set; }

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