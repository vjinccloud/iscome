using ICCModule.Entity.Tables;
using ICCModule.Entity.Tables.Platform;
using ICCModule.EntityService.Service;
using ICCModule.Models;
using ICCModule.ViewModel;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace InformationSystem.ViewModel.PlantDoctor
{
    public class RecordListViewModel
    {
        public RecordListViewModel()
        {
            string _directory = AppDomain.CurrentDomain.BaseDirectory;
            _directory = String.Concat(_directory, "/assets/");
            using (StreamReader r = new StreamReader(String.Concat(_directory, "taiwan_districts.json")))
            {
                string json = r.ReadToEnd();
                List<CityDistricts> items = JsonConvert.DeserializeObject<List<CityDistricts>>(json);
                Districts = items.Find(x => x.Name == "高雄市").Districts;
            }

            AllDoctors = Service_sysUserInfo.GetEnableDoctorList();
            var getUserId = SessionHelper.Get("LoginID");
            if (!string.IsNullOrEmpty(getUserId) && AllDoctors.Any(x => x.LoginID == getUserId && x.RoleID == "R08"))
            {
                AllDoctors = AllDoctors.Where(x => x.LoginID == getUserId).ToList();
                var thisDoctor = AllDoctors.Where(x => x.LoginID == getUserId).FirstOrDefault();
                var selectDistrict = (thisDoctor.District ?? "").Split(',').ToList();
                Districts = Districts.Where(x => selectDistrict.Contains(x.Zip)).ToList();
                RoleCode = thisDoctor.RoleID;
            }

            AllExperts = Service_sysUserInfo.GetEnableExpertList();
            var getExpoertId = SessionHelper.Get("ExpertID");
            if (!string.IsNullOrEmpty(getUserId) && AllExperts.Any(x => x.LoginID == getExpoertId && x.RoleID == "R10"))
            {
                AllExperts = AllExperts.Where(x => x.LoginID == getExpoertId).ToList();
                var thisExpert = AllExperts.Where(x => x.LoginID == getExpoertId).FirstOrDefault();
            }

            OriginList = Service_defCode.GetList("PlantDoctorRecordOrigin");
            StatusList = Service_defCode.GetList("PlantDoctorRecordStatus");
            TransferList = Service_defCode.GetList("TransferDiagnosis");
            PestTypeList = Service_defCode.GetList("PestType");
            CropTypes = Service_cropType.GetList(x => x.Enable);
            doctorSchedules = new List<doctorSchedule>();

            QueryDateType = "CreatedAt";
        }

        /// <summary>
        /// 頁數
        /// </summary>
        public int Page { get; set; } = 1;
        
        /// <summary>
        /// 總頁數
        /// </summary>
        public int TotalPage { get; set; }
        
        /// <summary>
        /// 查詢日期類型
        /// </summary>
        public string QueryDateType { get; set; }

        /// <summary>
        /// 起日
        /// </summary>
        public DateTime? StartDate { get; set; }

        /// <summary>
        /// 迄日
        /// </summary>
        public DateTime? EndDate { get; set; }
        
        /// <summary>
        /// 起日
        /// </summary>
        public string StartDateStr 
        { 
            get
            {
                if (StartDate.HasValue) return StartDate.Value.ToString("yyyy-MM-dd");
                return "";
            }
        }

        /// <summary>
        /// 迄日
        /// </summary>
        public string EndDateStr 
        {
            get
            {
                if (EndDate.HasValue) return EndDate.Value.ToString("yyyy-MM-dd");
                return "";
            }
        }

        /// <summary>
        /// 關鍵字
        /// </summary>
        public string KeyWord { get; set; }
        
        /// <summary>
        /// 案號
        /// </summary>
        public string CaseNo { get; set; }

        /// <summary>
        /// 農民姓名
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// 作物名稱
        /// </summary>
        public string CropName { get; set; }

        /// <summary>
        /// 行政區
        /// </summary>
        public string District { get; set; }

        /// <summary>
        /// 作物類別
        /// </summary>
        public string CropType { get; set; }

        /// <summary>
        /// 作物ID
        /// </summary>
        public string Crop { get; set; }

        /// <summary>
        /// 狀態
        /// </summary>
        public string Status { get; set; }

        /// <summary>
        /// 來源
        /// </summary>
        public string Origin { get; set; }

        /// <summary>
        /// 害物種類
        /// </summary>
        public string PestType { get; set; }
        
        /// <summary>
        /// 植物醫師帳號
        /// </summary>
        public string DoctorId { get; set; }

        /// <summary>
        /// 專家帳號
        /// </summary>
        public string ExpertId { get; set; }

        /// <summary>
        /// 視訊會議連結
        /// </summary>
        public string WebMeetingUrl { get; set; }         

        /// <summary>
        /// 後送診斷
        /// </summary>
        public string TransferDiagnosis { get; set; }

        /// <summary>
        /// 行政區
        /// </summary>
        public List<District> Districts { get; set; }

        /// <summary>
        /// 作物類別，內含作物細項
        /// </summary>
        public List<cropType> CropTypes { get; set; }

        /// <summary>
        /// 狀態列表
        /// </summary>
        public List<defCode> StatusList { get; set; }
        
        /// <summary>
        /// 後送診斷列表
        /// </summary>
        public List<defCode> TransferList { get; set; }

        /// <summary>
        /// 來源列表
        /// </summary>
        public List<defCode> OriginList { get; set; }

        /// <summary>
        /// 害物種類列表
        /// </summary>
        public List<defCode> PestTypeList { get; set; }

        /// <summary>
        /// 搜尋後的植醫紀錄
        /// </summary>
        public List<doctorSchedule> doctorSchedules { get; set; }
        /// <summary>
        /// 全植物醫師
        /// </summary>
        public List<sysUserSelect> AllDoctors { get; set; }
        /// <summary>
        /// 全植物醫師
        /// </summary>
        public List<sysUserSelect> AllExperts { get; set; }
        public string RoleCode { get; set; }
        public string ActionName { get; set; } = "";
    }
}