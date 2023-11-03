using ICCModule.Entity.Tables;
using ICCModule.Entity.Tables.Platform;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace InformationSystem.ViewModel.Management
{
    public class BusinessFormInfoViewModel
    {
        /// <summary>
        /// 類別查詢
        /// </summary>
        public List<sysRole> Types { get; set; }

        public BusinessForm BusinessForm { get; set; }

        public HttpPostedFileBase File { get; set; }
    }
}