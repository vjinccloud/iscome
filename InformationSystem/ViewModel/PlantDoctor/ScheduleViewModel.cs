using ICCModule.Entity.Tables;
using ICCModule.EntityService.Service;
using ICCModule.Models;
using ICCModule.Models.PlantDoctor;
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
    public class ScheduleViewModel
    {
        public ScheduleViewModel()
        {
            var am = Service_defCode.GetList("PlantDoctorAm");
            AmList = new List<string>();
            foreach (defCode d in am)
            {
                AmList.Add(d.Code);
            }
            var pm = Service_defCode.GetList("PlantDoctorPm");
            PmList = new List<string>();
            foreach (defCode d in pm)
            {
                PmList.Add(d.Code);
            }
            string _directory = AppDomain.CurrentDomain.BaseDirectory;
            _directory = String.Concat(_directory, "/assets/");
            using (StreamReader r = new StreamReader(String.Concat(_directory, "taiwan_districts.json")))
            {
                string json = r.ReadToEnd();
                List<CityDistricts> items = JsonConvert.DeserializeObject<List<CityDistricts>>(json);
                AllDistricts = items.Find(x => x.Name == "高雄市").Districts;
            }
            AllDoctors = Service_sysUserInfo.GetEnableDoctorList();

            var getUserId = SessionHelper.Get("LoginID");
            if (!string.IsNullOrEmpty(getUserId) && AllDoctors.Any(x => x.LoginID == getUserId && x.RoleID == "R08"))
            {
                AllDoctors = AllDoctors.Where(x => x.LoginID == getUserId).ToList();
                var thisDoctor = AllDoctors.Where(x => x.LoginID == getUserId).FirstOrDefault();
                var selectDistrict = (thisDoctor.District ?? "").Split(',').ToList();
                AllDistricts = AllDistricts.Where(x => selectDistrict.Contains(x.Zip)).ToList();
                RoleCode = thisDoctor.RoleID;
            }
        }

        /// <summary>
        /// 排程事件
        /// </summary>
        public List<ScheduleEvent> Events { get; set; }

        /// <summary>
        /// 透過 defCode 表中取出設定的上午場時段
        /// </summary>
        public List<string> AmList { get; set; }

        /// <summary>
        /// 透過 defCode 表中取出設定的下午場時段
        /// </summary>
        public List<string> PmList { get; set; }

        /// <summary>
        /// 全行政區
        /// </summary>
        public List<District> AllDistricts { get; set; }
        
        /// <summary>
        /// 全植物醫師
        /// </summary>
        public List<sysUserSelect> AllDoctors { get; set; }

        /// <summary>
        /// 行政區
        /// </summary>
        public string District { get; set; }
        /// <summary>
        /// 植物醫師
        /// </summary>
        public string DoctorId { get; set; }
        public string RoleCode { get; set; }
    }
}