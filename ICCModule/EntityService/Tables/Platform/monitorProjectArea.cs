using System;
using System.Data.Linq.Mapping;

namespace ICCModule.Entity.Tables.Platform
{
    [Serializable]
    [Table(Name = "monitor_Project_Areas")]
    public class monitorProjectArea
    {
        /// <summary> 流水號
        /// 
        /// </summary> 
        [Column(IsPrimaryKey = true, IsDbGenerated = true, AutoSync = AutoSync.OnInsert)]
        public int ID { get; set; }

        /// <summary> 對應專案ID
        /// 
        /// </summary> 
        [Column]
        public int ProjectID { get; set; }

        /// <summary> 調查對象類型
        /// 
        /// </summary> 
        [Column]
        public string Type { get; set; }

        /// <summary> 
        /// 外部人員帳號
        /// </summary> 
        [Column]
        public string OuterUser { get; set; }

        /// <summary> 行政區
        /// 
        /// </summary> 
        [Column]
        public string Distist { get; set; }

        /// <summary> 調查人員
        /// 
        /// </summary> 
        [Column]
        public string Inspectors { get; set; }

        /// <summary> 調查點位數量
        /// 
        /// </summary> 
        [Column]
        public int Points { get; set; }

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
