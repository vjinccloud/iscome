using System;
using System.Data.Linq.Mapping;

namespace ICCModule.Entity.Tables.Platform
{
    [Serializable]
    [Table(Name = "resume_Product_Checks")]
    public class resumeProductCheck
    {
        /// <summary> 流水號
        /// 
        /// </summary> 
        [Column(IsPrimaryKey = true, IsDbGenerated = true, AutoSync = AutoSync.OnInsert)]
        public int ID { get; set; }

        /// <summary> 
        /// 產品ID
        /// </summary> 
        [Column]
        public int ProductID { get; set; }

        /// <summary> 檢查日期
        /// 
        /// </summary> 
        [Column]
        public DateTime Date { get; set; }

        /// <summary> 標示檢查結果
        /// 
        /// </summary> 
        [Column]
        public bool? TagResult { get; set; }

        /// <summary> 品質檢查結果
        /// 
        /// </summary> 
        [Column]
        public bool? QuilityResult { get; set; }

        /// <summary> 裁處說明
        /// 
        /// </summary> 
        [Column]
        public string ArbitrationInstructions { get; set; }

        /// <summary> 創建時間
        /// 
        /// </summary> 
        [Column]
        public DateTime? CreatedAt { get; set; }

        /// <summary> 更新時間
        /// 
        /// </summary> 
        [Column]
        public DateTime? UpdatedAt { get; set; }
    }
}
