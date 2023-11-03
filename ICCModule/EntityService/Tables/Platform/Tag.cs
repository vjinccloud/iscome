using System;
using System.Data.Linq.Mapping;

namespace ICCModule.Entity.Tables.Platform
{
    [Serializable]
    [Table(Name = "Tags")]
    public class Tag
    {
        /// <summary> 標籤名稱(搜尋關鍵字用)
        /// 
        /// </summary> 
        [Column(IsPrimaryKey = true)]
        public string KeyName { get; set; }

        /// <summary> 搜尋次數
        /// 
        /// </summary> 
        [Column]
        public int Searches { get; set; }

        /// <summary> 顯示
        /// 
        /// </summary> 
        [Column]
        public bool Show { get; set; }

        /// <summary> 創建時間
        /// 
        /// </summary> 
        [Column(IsDbGenerated = true)]
        public DateTime? CreatedAt { get; set; }

        /// <summary> 更新時間
        /// 
        /// </summary> 
        [Column]
        public DateTime? UpdatedAt { get; set; }
    }
}
