using ICCModule.Entity.Tables;
using ICCModule.Entity.Tables.Platform;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Website.ViewModel.Home
{
    public class SearchViewModel
    {
        /// <summary>
        /// 搜尋列表
        /// </summary>
        public List<VW_Search> VW_Searchs { get; set; }
        /// <summary>
        /// 關鍵字
        /// </summary>
        public String KeyName { get; set; }
    }
}