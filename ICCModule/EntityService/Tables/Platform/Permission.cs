using System;
using System.Data.Linq.Mapping;

namespace ICCModule.Entity.Tables.Platform
{
    [Serializable]
    [Table(Name = "Permissions")]
    public class Permission
    {
        /// <summary>
        /// 權限索引
        /// </summary> 
        [Column(IsPrimaryKey = true)]
        public int Id { get; set; }

        /// <summary>
        /// 權限識別碼
        /// </summary> 
        [Column]
        public string Code { get; set; }

        /// <summary>
        /// 功能項
        /// </summary> 
        [Column]
        public string MainFunctionName { get; set; }

        /// <summary>
        /// 細項名稱
        /// </summary> 
        [Column]
        public string FunctionName { get; set; }

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

        /// <summary>
        /// 已被選擇
        /// </summary>
        public bool Selected { get; set; }
    }
}
