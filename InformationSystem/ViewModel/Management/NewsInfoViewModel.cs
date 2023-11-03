using ICCModule.Entity.Tables;
using ICCModule.Entity.Tables.Platform;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace InformationSystem.ViewModel.Management
{
    public class NewsInfoViewModel
    {
        /// <summary>
        /// 類別查詢
        /// </summary>
        public List<defCode> Types { get; set; }

        public New New { get; set; }

        public List<HttpPostedFileBase> Files { get; set; }
    }
}