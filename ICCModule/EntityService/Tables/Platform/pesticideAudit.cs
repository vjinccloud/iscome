using System;
using System.Data.Linq.Mapping;

namespace ICCModule.Entity.Tables.Platform
{
    [Serializable]
    [Table(Name = "pesticide_Audits")]
    public class pesticideAudit
    {
        /// <summary> 流水號
        /// 
        /// </summary> 
        [Column(IsPrimaryKey = true, IsDbGenerated = true, AutoSync = AutoSync.OnInsert)]
        public int ID { get; set; }

        /// <summary> 流水號
        /// 農藥業者識別ID
        /// </summary> 
        [Column]
        public string SellerID { get; set; }

        /// <summary> 檢查日期
        /// 
        /// </summary> 
        [Column]
        public DateTime Date { get; set; }

        /// <summary> 檢查結果
        /// 
        /// </summary> 
        [Column]
        public bool Result { get; set; }

        /// <summary> 說明
        /// 
        /// </summary> 
        [Column]
        public string Instructions { get; set; }
        
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
