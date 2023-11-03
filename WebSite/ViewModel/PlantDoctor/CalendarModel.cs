using ICCModule.Entity.Tables;
using ICCModule.Entity.Tables.Platform;
using ICCModule.EntityService.Service;
using ICCModule.Models;
using ICCModule.ViewModel;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;
using Website.Helper;

namespace Website.ViewModel.PlantDoctor
{
    public class CalendarModel
    {
        /// <summary>
        /// 輔導單位
        /// </summary>
        public string OrgType { get; set; }

        /// <summary>
        /// 輔導單位名稱
        /// </summary>
        public string OrgName { get; set; }

        /// <summary>
        /// 輔導單位行政區
        /// </summary>
        public string OrgDistrict { get; set; }

        /// <summary>
        /// 作物病徵 圖或影片
        /// </summary>
        public List<sysUserSelect> DoctorList { get; set; }
    }
}