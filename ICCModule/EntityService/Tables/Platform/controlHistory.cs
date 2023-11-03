using System;
using System.Data.Linq.Mapping;

namespace ICCModule.Entity.Tables.Platform
{
    [Serializable]
    [Table(Name = "control_Histories")]
    public class controlHistory
    {
        /// <summary> 流水號
        /// 
        /// </summary> 
        [Column(IsPrimaryKey = true, IsDbGenerated = true, AutoSync = AutoSync.OnInsert)]
        public int ID { get; set; }

        /// <summary> 防治名稱
        /// 
        /// </summary> 
        [Column]
        public string Name { get; set; }

        /// <summary> 類型
        /// 
        /// </summary> 
        [Column]
        public string Type { get; set; }

        /// <summary> 
        /// 啟用狀態
        /// </summary> 
        [Column]
        public bool Enable { get; set; }

        /// <summary> 
        /// 點閱率
        /// </summary> 
        [Column]
        public int ClickCount { get; set; }

        /// <summary> 基本資料 RichText
        /// 
        /// </summary> 
        [Column]
        public string Information { get; set; }

        /// <summary> 說明 RichText
        /// 
        /// </summary> 
        [Column]
        public string Explanation { get; set; }

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
