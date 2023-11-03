using ICCModule.Entity.Tables;
using ICCModule.Entity.Tables.Platform;
using ICCModule.EntityService.Service;
using ICCModule.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Website.Helper;

namespace Website.ViewModel.Member
{
    public class ManageViewModel
    {
        public ManageViewModel()
        {
            Types = Service_defCode.GetList("UserIdentify");
            CityDistricts = CommonDataHelper.CityDistricts;
        }

        /// <summary>
        /// 身分別
        /// </summary>
        public List<defCode> Types { get; set; }

        /// <summary>
        /// 縣市，行政區，以縣市為索引
        /// </summary>
        public List<CityDistricts> CityDistricts { get; set; }

        /// <summary>
        /// 使用者資訊
        /// </summary>
        public memberInfo Info { get; set; }

        /// <summary>
        /// 預約植醫記錄，已結案之外的狀態
        /// </summary>
        public List<doctorSchedule> DoctorSchedules { get; set; }

        /// <summary>
        /// 預約活動紀錄
        /// </summary>
        public List<activityRegistraction> ActivityRegistractions { get; set; }
    }
}