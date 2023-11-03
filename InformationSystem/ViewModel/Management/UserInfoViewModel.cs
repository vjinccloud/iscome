using ICCModule.Entity.Tables;
using ICCModule.Entity.Tables.Platform;
using ICCModule.EntityService.Service;
using ICCModule.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;

namespace InformationSystem.ViewModel.Management
{
    public class UserInfoViewModel
    {
        public UserInfoViewModel()
        {
            List<defCode> _Types = Service_defCode.GetList("UserIdentify");
            Types = _Types;
            string _directory = AppDomain.CurrentDomain.BaseDirectory;
            _directory = String.Concat(_directory, "/assets/");
            using (StreamReader r = new StreamReader(String.Concat(_directory, "taiwan_districts.json")))
            {
                string json = r.ReadToEnd();
                List<CityDistricts> items = JsonConvert.DeserializeObject<List<CityDistricts>>(json);
                Districts = items.Find(x => x.Name == "高雄市").Districts;
            }
        }

        /// <summary>
        /// 身分別
        /// </summary>
        public List<defCode> Types { get; set; }

        /// <summary>
        /// 後端使用者資訊
        /// </summary>
        /// <value></value>
        public sysUserInfo Info { get; set; }

        /// <summary>
        /// 行政區列表
        /// </summary>
        /// <value></value>
        public List<District> Districts { get; set; }
    }
}