using ICCModule.EntityService.Service;
using IscomG2C.Utility;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data.Linq.Mapping;

namespace ICCModule.Entity.Tables.Platform
{
    [Serializable]
    [Table(Name = "doctor_Schedule")]
    public class doctorSchedule
    {
        /// <summary> 識別ID
        /// 
        /// </summary> 
        [Column(IsPrimaryKey = true, IsDbGenerated = true, AutoSync = AutoSync.OnInsert)]
        public long ID { get; set; }

        /// <summary> 
        /// 輔導單位
        /// </summary> 
        [Column]
        public string OrgType { get; set; } = "";

        /// <summary> 
        /// 輔導單位行政區
        /// </summary> 
        [Column]
        public string OrgDistrict { get; set; } = "";

        /// <summary> 
        /// 輔導單位名稱
        /// </summary> 
        [Column]
        public string OrgName { get; set; } = "";

        /// <summary> 來源，對應到 defCode 表
        /// 
        /// </summary> 
        [Column]
        public string Origin { get; set; }
        

        /// <summary>
        /// 來源
        /// </summary> 
        public string OriginName { get; set; }

        /// <summary> 問診方式，對應到 defCode 表
        /// 
        /// </summary> 
        [Column]
        public string Inquiry { get; set; } = "";

        /// <summary> u藥檢不合格抽檢單位，對應到 defCode 表
        /// 
        /// </summary> 
        [Column]
        public string UnqualifiedUnit { get; set; }

        /// <summary> 不合格抽檢單位其他選項備註
        /// 
        /// </summary> 
        [Column]
        public string OtherUnit { get; set; }

        /// <summary> 報名掛號會員ID
        /// 
        /// </summary> 
        [Column]
        public long MemberInfoID { get; set; }

        /// <summary> 姓名
        /// 
        /// </summary> 
        [Column]
        public string Name { get; set; }

        /// <summary> 性別
        /// 
        /// </summary> 
        [Column]
        public string Sexy { get; set; }

        /// <summary>
        /// 取回性別
        /// </summary>
        public string SexyStr
        {
            get
            {
                if (String.IsNullOrEmpty(Sexy))
                {
                    return String.Empty;
                }
                return Sexy == "M" ? "男" : (Sexy == "F" ? "女" : "第三性");
            }
        }

        /// <summary> 行政區編碼
        /// 
        /// </summary> 
        [Column]
        public string Zip { get; set; } = "";

        /// <summary> 行政區名稱
        /// 
        /// </summary> 
        [Column]
        public string District { get; set; }

        /// <summary> 
        /// 預約日期時段
        /// </summary> 
        [Column]
        public DateTime ReserveDatetime { get; set; }

        public string ReserveDatetimeStr
        {
            get
            {
                if (ReserveDatetime == null)
                {
                    return "";
                }

                return String.Format("{0:yyyy-MM-dd}", ReserveDatetime);
            }

            set
            {
                ReserveDatetime = (DateTime)Utility_DateTime.StringToDateTime(value);
            }
        }

        public string ReserveTime
        {
            get
            {
                return String.Format("{0:HH:mm}", ReserveDatetime);
            }
            set
            {
                string DateStr = String.Format("{0:yyyy-MM-dd}", ReserveDatetime);
                ReserveDatetime = DateTime.Parse($"{DateStr} {value}");
            }
        }

        /// <summary>
        /// 依照預約時間計算上午或下午
        /// </summary>
        public string Period
        {
            get
            {
                if (ReserveDatetime == null)
                {
                    return "";
                }
                string Time = String.Format("{0:HH:mm}", ReserveDatetime);
                string[] times = Time.Split(':');
                return Convert.ToInt32(times[0]) > 12 ? "下午" : "上午";
            }
        }

        /// <summary>
        /// 台灣年月日
        /// </summary>
        public string ReserveDateTaiwainStr
        {
            get
            {
                return Utility_DateTime.ToFormat_inTaiwanYear(ReserveDatetime);
            }
        }

        /// <summary> 作物種類
        /// 
        /// </summary> 
        [Column]
        public string CropType { get; set; } = "";

        /// <summary> 作物名稱
        /// 
        /// </summary> 
        [Column]
        public string CropName { get; set; } = "";

        /// <summary>
        /// 行動電話
        /// </summary>
        [Column]
        public string Mobile { get; set; }

        /// <summary> 栽種面積公頃
        /// 
        /// </summary> 
        [Column]
        public Double PlantingArea { get; set; }

        /// <summary> 耕作方式，對應到 defCode 表
        /// 
        /// </summary> 
        [Column]
        public string FarmingMethod { get; set; }

        /// <summary>
        /// 取回耕作方式對應的名稱
        /// </summary>
        public string FarmingMethodStr
        {
            get
            {
                if (String.IsNullOrEmpty(FarmingMethod))
                {
                    return String.Empty;
                }

                return Service_defCode.GetDetail("FarmingMethod", FarmingMethod).Name;
            }
        }

        /// <summary> 聯繫管道，保存 JSON
        /// 
        /// </summary> 
        [Column]
        public string ContactMethod { get; set; }

        /// <summary>
        /// 聯繫管道中繼
        /// </summary>
        public List<string> ContactMethodArr
        {
            get
            {
                if (String.IsNullOrEmpty(ContactMethod))
                {
                    return new List<string>();
                }

                return JsonConvert.DeserializeObject<List<string>>(ContactMethod);
            }
            set
            {
                ContactMethod = JsonConvert.SerializeObject(value);
            }
        }

        /// <summary>
        /// 聯繫通道字典集
        /// </summary>
        public static Dictionary<string, string> ContactMethods = new Dictionary<string, string>()
        {
            { "Mobile", "行動電話" },
            { "Email", "電子信箱" },
            { "Line", "Line" }
        };

        /// <summary>
        /// 取得聯繫通道名稱
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public static string GetContactMethodName(string key)
        {
            string value = String.Empty;
            return ContactMethods.TryGetValue(key, out value) ? value : String.Empty;
        }

        public string ContactMethodStr
        {
            get
            {
                List<string> list = new List<string>();
                foreach (string key in ContactMethodArr)
                {
                    list.Add(GetContactMethodName(key));
                }
                return String.Join(",", list.ToArray());
            }
        }

        /// <summary> 發病位置，保存 JSON
        /// 
        /// </summary> 
        [Column]
        public string OnsetLocation { get; set; }

        /// <summary>
        /// 發病位置中繼
        /// </summary>
        public List<string> OnsetLocationArr
        {
            get
            {
                if (String.IsNullOrEmpty(OnsetLocation))
                {
                    return new List<string>();
                }
                return JsonConvert.DeserializeObject<List<string>>(OnsetLocation);
            }
            set
            {
                OnsetLocation = JsonConvert.SerializeObject(value);
            }
        }

        /// <summary> 諮詢內容
        /// 
        /// </summary> 
        [Column]
        public string Context { get; set; }

        /// <summary> 作物病徵圖/影片位置
        /// 
        /// </summary> 
        [Column]
        public string CropSymptoms { get; set; }

        /// <summary> 最近使用農藥/肥料照片位置
        /// 
        /// </summary> 
        [Column]
        public string RecentlyFertilizer { get; set; }

        /// <summary> 田區位置
        /// 
        /// </summary> 
        [Column]
        public string FramLocation { get; set; }

        /// <summary> 肥料/營養劑
        /// 
        /// </summary> 
        [Column]
        public string FertilizerNutrient { get; set; }

        /// <summary> 殺菌劑
        /// 
        /// </summary> 
        [Column]
        public string Fungicide { get; set; }

        /// <summary> 殺蟲劑
        /// 
        /// </summary> 
        [Column]
        public string Insecticide { get; set; }

        /// <summary> 殺草劑
        /// 
        /// </summary> 
        [Column]
        public string Herbicide { get; set; }

        /// <summary> 其他資材
        /// 
        /// </summary> 
        [Column]
        public string OtherMedicines { get; set; }

        /// <summary> 耕作歷史，對應到 defCode 表
        /// 
        /// </summary> 
        [Column]
        public string FarmingHistory { get; set; }

        /// <summary> 連作耕作物
        /// 
        /// </summary> 
        [Column]
        public string ContinuousCrop { get; set; }

        /// <summary> 連作其他耕作物
        /// 
        /// </summary> 
        [Column]
        public string OtherCrop { get; set; }

        /// <summary> 害物種類，對應到 defCode 表
        /// 
        /// </summary> 
        [Column]
        public string PestType { get; set; }

        /// <summary>
        /// 害物種類中繼
        /// </summary>
        public List<string> PestTypeArr
        {
            get
            {
                if (String.IsNullOrEmpty(PestType))
                {
                    return new List<string>();
                }
                return JsonConvert.DeserializeObject<List<string>>(PestType);
            }
            set
            {
                PestType = JsonConvert.SerializeObject(value);
            }
        }

        /// <summary> 其他害物種類
        /// 
        /// </summary> 
        [Column]
        public string OtherPest { get; set; }

        /// <summary> 診斷日期
        /// 
        /// </summary> 
        [Column]
        public DateTime? DateDiagnosis { get; set; }

        public string DateDiagnosisStr
        {
            get
            {
                if (DateDiagnosis == null)
                {
                    return "";
                }

                return String.Format("{0:yyyy-MM-dd}", DateDiagnosis);
            }

            set
            {
                DateDiagnosis = Utility_DateTime.StringToDateTime(value);
            }
        }

        /// <summary> 診斷結果
        /// 
        /// </summary> 
        [Column]
        public string ResultDiagnosis { get; set; }

        /// <summary> 對應後台使用者ID
        /// 
        /// </summary> 
        [Column]
        public string LoginID { get; set; } = "";

        /// <summary> 專家
        /// 
        /// </summary> 
        [Column]
        public string ExpertID { get; set; } = ""; 

        /// <summary> 植物醫生
        /// 
        /// </summary> 
        [Column]
        public string DoctorDiagnosis { get; set; }

        /// <summary> 值醫備註
        /// 
        /// </summary> 
        [Column]
        public string DoctorComment { get; set; }

        /// <summary> 防治建議類別，對應到 defCode 表
        /// 
        /// </summary> 
        [Column]
        public string RecommendationCategory { get; set; }

        /// <summary> 防治建議說明
        /// 
        /// </summary> 
        [Column]
        public string Recommendation { get; set; }

        /// <summary> 後送診斷
        /// 
        /// </summary> 
        [Column]
        public string TransferDiagnosis { get; set; }

        /// <summary> 預約狀態，對應到 defCode 表
        /// 
        /// </summary> 
        [Column]
        public string Status { get; set; }

        /// <summary>
        /// 預約狀態文字
        /// </summary>
        public string StatusStr
        {
            get
            {
                if (String.IsNullOrEmpty(Status))
                {
                    return String.Empty;
                }
                return Service_defCode.GetDetail("PlantDoctorRecordStatus", Status).Name;
            }
        }

        /// <summary>
        /// 當月索引編號
        /// </summary>
        [Column]
        public int MonthIndex { get; set; }

        /// <summary>
        /// 是否刪除，透過排班或預約列表刪除
        /// </summary>
        [Column]
        public bool IsDel { get; set; }

        /// <summary> 創建時間
        /// 
        /// </summary> 
        [Column]
        public DateTime? CreatedAt { get; set; }

        /// <summary> 創建者
        /// 
        /// </summary> 
        [Column]
        public string Creator { get; set; }

        /// <summary> 更新時間
        /// 
        /// </summary> 
        [Column]
        public DateTime? UpdatedAt { get; set; }

        /// <summary> 更新者
        /// 
        /// </summary> 
        [Column]
        public string Updater { get; set; }

        public string CreatedAtStr
        {
            get
            {
                if (CreatedAt == null)
                {
                    return "";
                }

                return String.Format("{0: yyyy\\/MM\\/dd HH:mm}", CreatedAt);
            }
        }

        public string UpdatedAtStr
        {
            get
            {
                if (UpdatedAt == null)
                {
                    return "";
                }

                return String.Format("{0: yyyy\\/MM\\/dd HH:mm}", UpdatedAt);
            }
        }

        /// <summary> 
        /// 轉案中
        /// </summary> 
        [Column]
        public bool? IsTransCase { get; set; }

        /// <summary> 
        /// 轉案行政區
        /// </summary> 
        [Column]
        public string TransDistrict { get; set; }

        /// <summary> 
        /// 轉案醫師序號
        /// </summary> 
        [Column]
        public string TransDocId { get; set; }

        /// <summary> 
        /// 轉案原因
        /// </summary> 
        [Column]
        public string TransReason { get; set; }

        /// <summary> 
        /// 轉案備註
        /// </summary> 
        [Column]
        public string TransRemark { get; set; }

        /// <summary> 
        /// 轉案時間
        /// </summary> 
        [Column]
        public DateTime? TransDate { get; set; }

        public string TransDateStr
        {
            get
            {
                if (TransDate == null)
                {
                    return "";
                }

                return String.Format("{0: yyyy\\/MM\\/dd HH:mm}", TransDate);
            }
        }

        /// <summary> 
        /// 轉案人
        /// </summary> 
        [Column]
        public string TransUser { get; set; }

        /// <summary> 
        /// 收案時間
        /// </summary> 
        [Column]
        public DateTime? AcceptCaseDate { get; set; }

        public string AcceptCaseDateStr
        {
            get
            {
                if (AcceptCaseDate == null)
                {
                    return "";
                }

                return String.Format("{0: yyyy\\/MM\\/dd HH:mm}", AcceptCaseDate);
            }
        }

        /// <summary> 
        /// 收案人
        /// </summary> 
        [Column]
        public string AcceptCaseUser { get; set; }

        public string CaseNo
        {
            get
            {
                return $"{(ReserveDatetime.Year - 1911).ToString() + ReserveDatetime.ToString("MMdd")}-{CropName}-{District}-{MonthIndex}";
            }
        }

        /// <summary>
        /// 取回對應的排班
        /// </summary>
        public doctorDutyRecord doctorDutyRecord
        {
            get
            {
                return Service_doctor_DutyRecord.GetDetail(ID.ToString());
            }
        }
    }
}
