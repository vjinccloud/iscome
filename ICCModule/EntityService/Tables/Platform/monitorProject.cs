using System;
using System.Data.Linq.Mapping;

namespace ICCModule.Entity.Tables.Platform
{
    [Serializable]
    [Table(Name = "monitor_Projects")]
    public class monitorProject
    {
        /// <summary> 流水號
        /// 
        /// </summary> 
        [Column(IsPrimaryKey = true, IsDbGenerated = true, AutoSync = AutoSync.OnInsert)]
        public int ID { get; set; }

        /// <summary> 專案年度
        /// 
        /// </summary> 
        [Column]
        public int Year { get; set; }

        /// <summary> 專案名稱
        /// 
        /// </summary> 
        [Column]
        public string Name { get; set; }

        /// <summary> 監測頻率，對應 defCode 表
        /// 
        /// </summary> 
        [Column]
        public string Frequency { get; set; }

        /// <summary> 監測起
        /// 
        /// </summary> 
        [Column]
        public DateTime StartDate { get; set; }
        
        /// <summary> 監測迄
        /// 
        /// </summary> 
        [Column]
        public DateTime EndDate { get; set; }

        /// <summary> 顯示狀態
        /// 
        /// </summary> 
        [Column]
        public bool Show { get; set; }

        /// <summary> 創建時間
        /// 
        /// </summary> 
        [Column]
        public DateTime CreatedAt { get; set; }

        /// <summary> 更新時間
        /// 
        /// </summary> 
        [Column]
        public DateTime? UpdatedAt { get; set; }
    }
}
