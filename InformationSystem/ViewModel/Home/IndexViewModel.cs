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

namespace InformationSystem.ViewModel.Home
{
    public class IndexViewModel
    {
        public IndexViewModel()
        {
            string _directory = AppDomain.CurrentDomain.BaseDirectory;
            _directory = String.Concat(_directory, "/assets/");
            using (StreamReader r = new StreamReader(String.Concat(_directory, "taiwan_districts.json")))
            {
                string json = r.ReadToEnd();
                List<CityDistricts> items = JsonConvert.DeserializeObject<List<CityDistricts>>(json);
                Districts = items.Find(x => x.Name == "高雄市").Districts;
            }

            StatusList = Service_defCode.GetList("PlantDoctorRecordStatus");
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
        }

        public List<BusinessForm> Forms { get; set; }

        /// <summary>
        /// 行政區
        /// </summary>
        public List<District> Districts { get; set; }

        /// <summary>
        /// 全植物醫師
        /// </summary>
        public List<sysUserSelect> AllDoctors { get; set; }
        /// <summary>
        /// 權限
        /// </summary>
        public string RoleCode { get; set; }

        /// <summary>
        /// 狀態列表
        /// </summary>
        public List<defCode> StatusList { get; set; }

        /// <summary>
        /// 狀態列表
        /// </summary>
        public List<sysNews> sysNewsList { get; set; }
        public int FormPage { get; set; }
        public int sysNewsPage { get; set; }
        /// <summary>
        /// 指定日值醫紀錄，預設為當日
        /// </summary>
        public List<doctorSchedule> doctorSchedules { get; set; }

        public string District { get; set; }
        public string DoctorId { get; set; }
        public DateTime? ReserveDatetime { get; set; }

        public string TabName { get; set; }
        public string QueryDateType { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public string KeyWord { get; set; }
        public DateTime ReadDate { get; set; }
    }
}