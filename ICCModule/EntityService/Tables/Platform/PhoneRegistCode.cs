using IscomG2C.Utility;
using System;
using System.Data.Linq.Mapping;
using System.Web.Configuration;

namespace ICCModule.Entity.Tables.Platform
{
    [Serializable]
    [Table(Name = "PhoneRegistCode")]
    public class PhoneRegistCode
    {
        /// <summary> 
        /// 流水號
        /// </summary> 
        [Column(IsPrimaryKey = true, IsDbGenerated = true, AutoSync = AutoSync.OnInsert)]
        public int Id { get; set; }

        /// <summary>
        /// 電話
        /// </summary> 
        [Column]
        public string PhoneNumber { get; set; }

        /// <summary>
        /// 驗證碼
        /// </summary> 
        [Column]
        public string CheckCode { get; set; }

        /// <summary>
        /// 過期時間
        /// </summary> 
        [Column]
        public DateTime ExpireDate { get; set; }
        /// <summary>
        /// 創建時間
        /// </summary> 
        [Column]
        public DateTime CreateDate { get; set; }

    }
}
