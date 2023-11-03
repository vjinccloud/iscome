using System;
using System.Data.Linq.Mapping;

namespace ICCModule.Entity.Tables.Platform
{
    [Serializable]
    [Table(Name = "crops")]
    public class crops
    {
        public crops()
        {
            Sort = 0;
            Enable = true;
        }
        /// <summary> 流水號
        /// 
        /// </summary> 
        [Column(IsPrimaryKey = true, IsDbGenerated = true, AutoSync = AutoSync.OnInsert)]
        public int ID { get; set; }

        /// <summary> 作物類型對應ID
        /// 
        /// </summary> 
        [Column]
        public int TypeID { get; set; }

        /// <summary> 類別名稱
        /// 
        /// </summary> 
        [Column]
        public string Name { get; set; }

        /// <summary> 排序
        /// 
        /// </summary> 
        [Column]
        public int Sort { get; set; }

        /// <summary> 狀態
        /// 
        /// </summary> 
        [Column]
        public bool Enable { get; set; }

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
