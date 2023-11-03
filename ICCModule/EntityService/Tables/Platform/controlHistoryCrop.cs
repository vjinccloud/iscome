using System;
using System.Data.Linq.Mapping;

namespace ICCModule.Entity.Tables.Platform
{
    [Serializable]
    [Table(Name = "control_History_Crops")]
    public class controlHistoryCrop
    {
        /// <summary> 流水號
        /// 
        /// </summary> 
        [Column(IsPrimaryKey = true, IsDbGenerated = true, AutoSync = AutoSync.OnInsert)]
        public int ID { get; set; }

        /// <summary> 防治曆ID
        /// 
        /// </summary> 
        [Column]
        public int ControlID { get; set; }

        /// <summary> 作物防治曆名稱
        /// 
        /// </summary> 
        [Column]
        public string Name { get; set; }

        /// <summary> 排序
        /// 
        /// </summary> 
        [Column]
        public int Sort { get; set; }

        /// <summary> 顯示
        /// 
        /// </summary> 
        [Column]
        public bool Show { get; set; }

        /// <summary> 類型
        /// 
        /// </summary> 
        [Column]
        public bool Type { get; set; }

        /// <summary> 
        /// 天數
        /// </summary> 
        [Column]
        public int? DayCount { get; set; }

        /// <summary> 備註
        /// 
        /// </summary> 
        [Column]
        public string Comment { get; set; }

        /// <summary> 防治內容
        /// 
        /// </summary> 
        [Column]
        public string Context { get; set; }

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
