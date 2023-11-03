using ICCModule.EntityService.Service;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Data.Linq.Mapping;
using System.Web.Mvc;

namespace ICCModule.Entity.Tables.Platform
{
    [Serializable]
    [Table(Name = "activity_Plans")]
    public class activityPlan
    {
        /// <summary> 流水號
        /// 
        /// </summary> 
        [Column(IsPrimaryKey = true, IsDbGenerated = true, AutoSync = AutoSync.OnInsert)]
        public long ID { get; set; }

        /// <summary> 活動類別
        /// 
        /// </summary> 
        [Column]
        public string Type { get; set; }

        /// <summary>
        /// Gets the name of the type from defCode Table
        /// </summary>
        /// <value>
        /// The name of the type.
        /// </value>
        public string TypeName
        {
            get { return Service_defCode.GetDetail("ActivityType", Type)?.Name; }
        }

        /// <summary> 活動名稱
        /// 
        /// </summary> 
        [Column]
        public string Name { get; set; }

        /// <summary> 活動起日
        /// 
        /// </summary> 
        [Column]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy/MM/dd}")]
        public DateTime StartDate { get; set; }

        public string StartDateStr
        {
            get
            {
                if (StartDate == null)
                {
                    return DateTime.Now.ToString("yyyy/MM/dd");
                }

                return StartDate.ToString("yyyy/MM/dd");
            }

            set
            {
                StartDate = Convert.ToDateTime(String.Concat(value, " 00:00"));
            }
        }

        /// <summary> 活動迄日
        /// 
        /// </summary> 
        [Column]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy/MM/dd}")]
        public DateTime EndDate { get; set; }

        public string EndDateStr
        {
            get
            {
                if (EndDate == null)
                {
                    return "";
                }

                return EndDate.ToString("yyyy/MM/dd");
            }

            set
            {
                EndDate = Convert.ToDateTime(String.Concat(value, " 00:00"));
            }
        }

        /// <summary> 地點
        /// 
        /// </summary> 
        [Column]
        public string Address { get; set; }

        /// <summary> 花費時數
        /// 
        /// </summary> 
        [Column]
        public double ClassHours { get; set; }

        /// <summary> 活動內容
        /// 
        /// </summary> 
        [Column]
        [AllowHtml]
        public string Context { get; set; }

        /// <summary> 承辦人
        /// 
        /// </summary> 
        [Column]
        public string Undertaker { get; set; }

        /// <summary> 承辦電話
        /// 
        /// </summary> 
        [Column]
        public string Phone { get; set; }

        /// <summary> 承辦電子信箱
        /// 
        /// </summary> 
        [Column]
        public string Email { get; set; }

        /// <summary> 顯示狀態
        /// 
        /// </summary> 
        [Column]
        public bool Show { get; set; } = true;

        /// <summary> 活動狀態
        /// 
        /// </summary> 
        [Column]
        public byte Status { get; set; }

        /// <summary> 開放報名
        /// 
        /// </summary> 
        [Column]
        public bool Open { get; set; } = false;

        /// <summary> 報名起日
        /// 
        /// </summary> 
        [Column]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy/MM/dd}")]
        public DateTime? RegistrationStartDate { get; set; }

        public string RegistrationStartDateStr
        {
            get
            {
                if (RegistrationStartDate == null)
                {
                    return "";
                }

                return RegistrationStartDate?.ToString("yyyy/MM/dd");
            }

            set
            {
                RegistrationStartDate = Convert.ToDateTime(String.Concat(value, " 00:00"));
            }
        }

        /// <summary> 報名迄日
        /// 
        /// </summary> 
        [Column]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy/MM/dd}")]
        public DateTime? RegistrationEndDate { get; set; }

        public string RegistrationEndDateStr
        {
            get
            {
                if (RegistrationEndDate == null)
                {
                    return "";
                }

                return RegistrationEndDate?.ToString("yyyy/MM/dd");
            }

            set
            {
                RegistrationEndDate = Convert.ToDateTime(String.Concat(value, " 00:00"));
            }
        }

        /// <summary> 報名欄位設定
        /// 
        /// </summary> 
        [Column]
        public string FieldSetting { get; set; }

        public PlanField Fields
        {
            get
            {
                if (String.IsNullOrEmpty(FieldSetting) || FieldSetting == "null")
                {
                    return new PlanField();
                }

                JObject keyValuePairs = JObject.Parse(FieldSetting);

                PlanField pf = new PlanField();
                if (keyValuePairs != null)
                    foreach (var prop in pf.GetType().GetProperties())
                    {
                        var propertyInfo = pf.GetType().GetProperty(prop.Name);
                        if (keyValuePairs[prop.Name] != null) propertyInfo.SetValue(pf, (bool)keyValuePairs[prop.Name], null);
                        else propertyInfo.SetValue(pf, false, null);
                    }

                return pf;
            }

            set
            {
                FieldSetting = JsonConvert.SerializeObject(value);
            }
        }

        /// <summary> 創建時間
        /// 
        /// </summary> 
        [Column(IsDbGenerated = true)]
        public DateTime? CreatedAt { get; set; }

        /// <summary> 更新時間
        /// 
        /// </summary> 
        [Column]
        public DateTime? UpdatedAt { get; set; }

        /// <summary>
        /// 開放場次列表
        /// </summary>
        public List<activityPlanOpenTime> OpenTimes { get; set; }

        /// <summary>
        /// 是否額滿
        /// </summary>
        public bool IsFull
        {
            get
            {
                var list = Service_activityPlanOpenTIme.GetList(ID);

                bool full = true;
                int i = 0;
                foreach (var l in list)
                {
                    // 沒有限制代表未額滿
                    if (l.Nums == 0)
                    {
                        full = false;
                        i++;
                    }
                    else if (l.Nums == l.RegisteredNums)
                    {
                        i++;
                    }
                }

                if (!full)
                {
                    return false;
                }

                return i == list.Count;
            }
        }
    }

    /// <summary>
    /// 開放報名控制項
    /// </summary>
    public class PlanField
    {
        public bool NameCheck { get; set; }
        public bool NameNeed { get; set; }

        public bool IdentifyCheck { get; set; }
        public bool IdentifyNeed { get; set; }

        public bool PhoneCheck { get; set; }
        public bool PhoneNeed { get; set; }

        public bool PesticideManagementStaffIDCheck { get; set; }
        public bool PesticideManagementStaffIDNeed { get; set; }

        public bool PesticideManagementStaffExpiryDateCheck { get; set; }
        public bool PesticideManagementStaffExpiryDateNeed { get; set; }

        public bool ServiceUnitCheck { get; set; }
        public bool ServiceUnitNeed { get; set; }

        public bool MealsTypeCheck { get; set; }
        public bool MealsTypeNeed { get; set; }

        public bool CropsNameCheck { get; set; }
        public bool CropsNameNeed { get; set; }

        public bool CropsAreaCheck { get; set; }
        public bool CropsAreaNeed { get; set; }
    }
}
