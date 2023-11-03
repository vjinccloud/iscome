using ICCModule.Entity.Tables;
using ICCModule.Entity.Tables.Platform;
using ICCModule.EntityService.Service;
using ICCModule.Models;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;
using Website.Helper;

namespace Website.ViewModel.PlantDoctor
{
    public class InfoViewModel
    {
        public InfoViewModel()
        {
            Districts = CommonDataHelper.GetDistricts("高雄市");

            CropTypes = Service_cropType.GetList();
            FarmingMethodList = Service_defCode.GetList("FarmingMethod");
            OnsetLocationList = Service_defCode.GetList("OnsetLocation");

            Schedule = new doctorSchedule();
            Readonly = false;
        }

        /// <summary>
        /// 行政區
        /// </summary>
        public List<District> Districts { get; set; }

        /// <summary>
        /// 作物類別，內含作物細項
        /// </summary>
        public List<cropType> CropTypes { get; set; }

        /// <summary>
        /// 耕作方式列表
        /// </summary>
        public List<defCode> FarmingMethodList { get; set; }

        /// <summary>
        /// 發病位置列表
        /// </summary>
        public List<defCode> OnsetLocationList { get; set; }

        /// <summary>
        /// 預約日期
        /// </summary>
        public string ReserveDate { get; set; }

        /// <summary>
        /// 預約日期台灣年月日
        /// </summary>
        public string ReserveTaiwanDate { get; set; }

        /// <summary>
        /// 預約時段
        /// </summary>
        public string ReservePeriod { get; set; }

        /// <summary>
        /// 當前值醫紀錄或新的
        /// </summary>
        public doctorSchedule Schedule { get; set; }

        /// <summary>
        /// 作物病徵 圖或影片
        /// </summary>
        public List<HttpPostedFileBase> CropSymptomsFiles { get; set; }

        /// <summary>
        /// 最近使用農藥、肥料 圖或影片
        /// </summary>
        public List<HttpPostedFileBase> RecentlyFertilizerFiles { get; set; }

        /// <summary>
        /// 同意書
        /// </summary>
        public bool Agree { get; set; }

        /// <summary>
        /// 驗證碼
        /// </summary>
        public string VerifyCode { get; set; }

        public bool Readonly { get; set; }
    }
}