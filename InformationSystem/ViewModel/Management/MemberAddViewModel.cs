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
    public class MemberAddViewModel
    {
        public MemberAddViewModel()
        {
            sysUserInfo = new sysUserInfo();
            List<defCode> _Types = Service_defCode.GetList("UserIdentify");
            Types = _Types;
            Roles = Service_sysRole.GetList("", "");
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
        /// 定義列表
        /// </summary>
        public List<sysRole> Roles { get; set; }

        /// <summary>
        /// 帳號新增
        /// </summary>
        public sysUserInfo sysUserInfo { get; set; }

        /// <summary>
        /// 行政區列表
        /// </summary>
        public List<District> Districts { get; set; }
    }
}