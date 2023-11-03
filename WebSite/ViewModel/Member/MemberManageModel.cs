using ICCModule.Entity.Tables;
using ICCModule.Entity.Tables.Platform;
using ICCModule.EntityService.Service;
using ICCModule.Models;
using IscomG2C.Utility;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;

namespace Website.ViewModel.Member
{
    public class MemberManageModel
    {
        public MemberManageModel()
        {
            List<defCode> _Types = Service_defCode.GetList("UserIdentify");
            UserType = _Types;
            string _directory = AppDomain.CurrentDomain.BaseDirectory;
            _directory = String.Concat(_directory, "/assets/");
            using (StreamReader r = new StreamReader(String.Concat(_directory, "taiwan_districts.json")))
            {
                string json = r.ReadToEnd();
                List<CityDistricts> items = JsonConvert.DeserializeObject<List<CityDistricts>>(json);
                CityDistricts = items.FirstOrDefault(x => x.Name == "高雄市")?.Districts ?? new List<District>();
            }
        }
        public ManageMemberEditModel MemberData { get; set; }
        public List<activityPlan> PlanList { get; set; }
        public List<doctorSchedule> ScheduleList { get; set; }
        public List<defCode> UserType { get; set; }

        /// <summary>
        /// 縣市，行政區，以縣市為索引
        /// </summary>
        public List<District> CityDistricts { get; set; }

    }

    public class ManageMemberEditModel
    {
        /// <summary> 流水號
        ///
        /// </summary>
        public long ID { get; set; }

        /// <summary> 會員帳號
        ///
        /// </summary>
        public string Account { get; set; }

        /// <summary> 會員名稱
        ///
        /// </summary>
        public string Name { get; set; }

        /// <summary> 信箱
        ///
        /// </summary>
        public string Email { get; set; }

        /// <summary>
        /// 驗證方式
        /// </summary>
        public string VerifyMethod { get; set; }

        /// <summary> 行動電話
        ///
        /// </summary>
        public string Mobile { get; set; }

        /// <summary> 出生年
        ///
        /// </summary>
        public DateTime? YearOfBirth { get; set; }

        public string Year
        {
            get { return Utility_DateTime.ToFormat_inTaiwanYear(YearOfBirth, "yyy"); }
            set { YearOfBirth = Utility_DateTime.ParseExact(value) ?? DateTime.Now; }
        }

        public int NewYear { get; set; }

        /// <summary> 性別
        ///
        /// </summary>
        public string Sexy { get; set; }

        /// <summary> 縣市
        ///
        /// </summary>
        public string City { get; set; }

        /// <summary> 行政區
        ///
        /// </summary>
        public string District { get; set; }

        /// <summary>
        /// 身分證字號
        /// </summary>
        public String Identify { get; set; }

        /// <summary> 黑名單過期時間
        ///
        /// </summary>
        public DateTime? BlacklistExpiredAt { get; set; }

        /// <summary>
        /// 植醫逾期次數
        /// </summary>
        public byte ExpiredTimes { get; set; }

        /// <summary> 
        /// 
        /// </summary>
        public string LineMessageId { get; set; }
        
        /// <summary> 
        /// 
        /// </summary>
        public string LineNonce { get; set; }

        /// <summary> 
        /// Line綁定代碼
        /// </summary>
        public string LineBindCode { get; set; }

        /// <summary> 
        /// 
        /// </summary>
        public string GoogleId { get; set; }

        /// <summary> 
        /// 
        /// </summary>
        public string FacebookId { get; set; }

        /// <summary> 
        /// 
        /// </summary>
        public string LineLoginId { get; set; }

        /// <summary> 
        /// Line綁定時限
        /// </summary>
        public DateTime? LineBindLimit { get; set; }
    }

    public class NotifyModel
    {
        public long ID { get; set; }
        public string SubscribeEpidemic { get; set; }
        public List<string> SubscribeEpidemicList { get; set; }
    }
}