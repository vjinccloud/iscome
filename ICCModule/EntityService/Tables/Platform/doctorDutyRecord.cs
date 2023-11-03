using ICCModule.EntityService.Service;
using IscomG2C.Utility;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data.Linq.Mapping;

namespace ICCModule.Entity.Tables.Platform
{
    [Serializable]
    [Table(Name = "doctor_DutyRecords")]
    public class doctorDutyRecord
    {
        /// <summary> 流水號
        /// 
        /// </summary> 
        [Column(IsPrimaryKey = true, IsDbGenerated = true, AutoSync = AutoSync.OnInsert)]
        public long ID { get; set; }

        /// <summary>
        /// 創建者的登入ID
        /// </summary>
        [Column]
        public string CreateUserLoginID { get; set; }

        /// <summary> 日期
        /// 
        /// </summary> 
        [Column]
        public DateTime Date { get; set; }

        public string DateStr
        {
            get
            {
                if (Date == null)
                {
                    return String.Empty;
                }
                return String.Format("{0:yyyy-MM-dd}", Date);
            }

            set
            {
                Date = DateTime.Parse(value);
            }
        }

        /// <summary>
        /// 時間
        /// </summary>
        [Column]
        public string Time { get; set; }

        /// <summary>
        /// 時段，僅有 上午 am 或 下午 pm
        /// </summary>
        [Column]
        public string Period { get; set; }

        /// <summary>
        /// 行政區
        /// </summary>
        [Column]
        public string District { get; set; }
        public string DistrictName { get; set; }

        /// <summary>
        /// 植物醫生序號
        /// </summary>
        [Column]
        public string DoctorId { get; set; }
        public string DoctorName { get; set; }

        /// <summary> 植醫排程對應ID
        /// 
        /// </summary> 
        [Column]
        public long DoctorScheduleID { get; set; }

        /// <summary>
        /// 取回 植醫紀錄，可能為空
        /// </summary>
        public doctorSchedule doctorSchedule 
        { 
            get
            {
                return Service_doctorSchedule.GetDetail(DoctorScheduleID);
            }
        }

        /// <summary> 狀態
        /// 
        /// </summary> 
        [Column]
        public byte Status { get; set; }

        /// <summary>
        /// 是否刪除
        /// </summary> 
        [Column]
        public bool IsDel { get; set; }

        /// <summary> 變更說明
        /// 
        /// </summary> 
        [Column]
        public string UpdateComment { get; set; }

        public List<RecordUpdate> UpdateDetail
        {
            get
            {
                if (String.IsNullOrEmpty(UpdateComment))
                {
                    return null;
                }
                else
                {
                    return JsonConvert.DeserializeObject<List<RecordUpdate>>(UpdateComment);
                }
            }

            set
            {
                UpdateComment = JsonConvert.SerializeObject(value);
            }
        }

        /// <summary> 創建時間
        /// 
        /// </summary> 
        [Column]
        public DateTime CreatedAt { get; set; }

        /// <summary>
        /// 創建者
        /// </summary> 
        [Column]
        public string CreateUser { get; set; }

        /// <summary> 更新時間
        /// 
        /// </summary> 
        [Column]
        public DateTime? UpdatedAt { get; set; }

        /// <summary>
        /// 更新者
        /// </summary> 
        [Column]
        public string UpdateUser { get; set; }
    }

    [Serializable]
    public class RecordUpdate
    {
        public DateTime UpdatedAt { get; set; }

        /// <summary>
        /// 取得更新日期民國年
        /// </summary>
        public string UpdatedAtTaiwanFormat
        {
            get
            {
                return Utility_DateTime.ToFormat_inTaiwanYear(UpdatedAt, "yyy/MM/dd");
            }
        }

        public string UpdatedLoginID { get; set; }

        public string UpdatedUser { get; set; }

        public string Reason { get; set; }
    }

    public enum DutyRecordStatus
    {
        Created = 0,
        Completed = 1,
        Changed = 10,
        Deleted = 99
    }
}
