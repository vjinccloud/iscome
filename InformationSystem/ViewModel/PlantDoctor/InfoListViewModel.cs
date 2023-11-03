using ICCModule.Entity.Tables;
using ICCModule.Entity.Tables.Platform;
using ICCModule.EntityService.Service;
using ICCModule.Models;
using ICCModule.ViewModel;
using InformationSystem.Helper;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Web;

namespace InformationSystem.ViewModel.PlantDoctor
{
    public class InfoListViewModel
    {
        public InfoListViewModel()
        {
            Districts = CommonDataHelper.GetDistricts("高雄市");

            CounselingList = Service_defCode.GetList("PlantDoctorCounselingUnit");
            OriginList = Service_defCode.GetList("PlantDoctorRecordOrigin");
            InquiryList = Service_defCode.GetList("PlantDoctorInquiry");
            DrugTestList = Service_defCode.GetList("DrugTestUnqualifiedSamplingUnit");
            CropTypes = Service_cropType.GetList(x => x.Enable);
            FarmingMethodList = Service_defCode.GetList("FarmingMethod");
            OnsetLocationList = Service_defCode.GetList("OnsetLocation");
            FarmingHistoryList = Service_defCode.GetList("FarmingHistory");
            EarlyCropsList = Service_defCode.GetList("EarlyCrops");
            PestTypeList = Service_defCode.GetList("PestType");
            PreventionList = Service_defCode.GetList("PreventionRecommendations");
            TransferList = Service_defCode.GetList("TransferDiagnosis");
            StatusList = Service_defCode.GetList("PlantDoctorRecordStatus");

            Schedule = new doctorSchedule();
            IsAdd = true;

            // 前台網址
            string Front_HostName = ConfigurationManager.AppSettings["Front_HostName"];
            FrontPesticidesInfoUrl = $"{Front_HostName}/DiseasePrevention/PesticidesInfo";
        }

        /// <summary>
        /// 輔導單位列表
        /// </summary>
        public List<defCode> CounselingList { get; set; }
        
        /// <summary>
        /// 來源列表
        /// </summary>
        public List<defCode> OriginList { get; set; }

        /// <summary>
        /// 問診方式列表
        /// </summary>
        public List<defCode> InquiryList { get; set; }

        /// <summary>
        /// 藥檢不合格抽檢列表
        /// </summary>
        public List<defCode> DrugTestList { get; set; }

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
        /// 耕作歷史列表
        /// </summary>
        public List<defCode> FarmingHistoryList { get; set; }

        /// <summary>
        /// 前期作物列表
        /// </summary>
        public List<defCode> EarlyCropsList { get; set; }

        /// <summary>
        /// 害物種類列表
        /// </summary>
        public List<defCode> PestTypeList { get; set; }

        /// <summary>
        /// 防治建議類別列表
        /// </summary>
        public List<defCode> PreventionList { get; set; }

        /// <summary>
        /// 後送診斷列表
        /// </summary>
        public List<defCode> TransferList { get; set; }

        /// <summary>
        /// 狀態列表
        /// </summary>
        public List<defCode> StatusList { get; set; }

        /// <summary>
        /// 預約時段列表，受到值醫排班影響
        /// </summary>
        public List<string> ReserveTimeList { get; set; }

        /// <summary>
        /// 當前值醫紀錄或新的
        /// </summary>
        public doctorSchedule Schedule { get; set; }

        /// <summary> 
        /// 作物病徵舊檔案
        /// </summary> 
        public List<FileManagement> OldCropSymptomsFiles { get; set; } = new List<FileManagement>();
        /// <summary> 
        /// 使用農藥、肥料舊檔案
        /// </summary> 
        public List<FileManagement> OldRecentlyFertilizerFiles { get; set; } = new List<FileManagement>();
        /// <summary> 
        /// 編輯作物病徵舊檔案
        /// </summary> 
        public List<long> EditOldCropSymptomsFiles { get; set; } = new List<long>();
        /// <summary> 
        /// 編輯使用農藥、肥料舊檔案
        /// </summary> 
        public List<long> EditOldRecentlyFertilizerFiles { get; set; } = new List<long>();
        /// <summary>
        /// 作物病徵 圖或影片
        /// </summary>
        public List<HttpPostedFileBase> CropSymptomsFiles { get; set; }
        /// <summary>
        /// 植物醫生名單
        /// </summary>
        public List<sysUserSelect> DoctorList { get; set; }

        /// <summary>
        /// 最近使用農藥、肥料 圖或影片
        /// </summary>
        public List<HttpPostedFileBase> RecentlyFertilizerFiles { get; set; }

        /// <summary>
        /// 前台農藥資訊查詢網址
        /// </summary>
        public string FrontPesticidesInfoUrl { get; set; }

        /// <summary>
        /// 新增案件?
        /// </summary>
        public bool IsAdd { get; set; }

        /// <summary>
        /// 地圖用 API Key
        /// </summary>
        public string MapApiKey { get; set; }
    }
}