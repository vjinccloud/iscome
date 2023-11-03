using System;
using System.Data.Linq.Mapping;


namespace ICCModule.Entity.Tables.Platform
{
    [Serializable]
    [Table(Name = "doctor_ScheduleHistories")]
    class doctorScheduleHistory
    {
        /// <summary> 流水號
        /// 
        /// </summary> 
        [Column(IsPrimaryKey = true, IsDbGenerated = true, AutoSync = AutoSync.OnInsert)]
        public long ID { get; set; }

        /// <summary> 值醫排程ID
        /// 
        /// </summary> 
        [Column]
        public long DoctorScheduleID { get; set; }

        /// <summary> 值醫備註
        /// 
        /// </summary> 
        [Column]
        public string DoctorComment { get; set; }

        /// <summary> 狀態
        /// 
        /// </summary> 
        [Column]
        public int Status { get; set; }

        /// <summary> 創建時間
        /// 
        /// </summary> 
        [Column]
        public DateTime CreatedAt { get; set; }

        /// <summary> 更新時間
        /// 
        /// </summary> 
        [Column]
        public DateTime UpdatedAt { get; set; }
    }
}
