using System;
using System.Data.Linq.Mapping;

namespace ICCModule.Entity.Tables.Platform
{
    [Serializable]
    [Table(Name = "member_Inventory")]
    public class memberInventory
    {
        /// <summary> 
        /// 流水號
        /// </summary> 
        [Column(IsPrimaryKey = true, IsDbGenerated = true, AutoSync = AutoSync.OnInsert)]
        public int Id { get; set; }

        /// <summary> 
        /// 使用者序號
        /// </summary> 
        [Column]
        public int MemberId { get; set; }

        /// <summary> 
        /// 許可證字
        /// </summary> 
        [Column]
        public string LicenseWord { get; set; }

        /// <summary> 
        /// 許可證號
        /// </summary> 
        [Column]
        public string LicenseNumber { get; set; }

        /// <summary> 
        /// 中文名稱
        /// </summary> 
        [Column]
        public string ChineseName { get; set; }

        /// <summary> 
        /// 含量
        /// </summary> 
        [Column]
        public string Contents { get; set; }

        /// <summary> 
        /// 劑型
        /// </summary> 
        [Column]
        public string DosageForm { get; set; }

        /// <summary> 
        /// 廠牌名稱
        /// </summary> 
        [Column]
        public string BrandName { get; set; }

        /// <summary> 
        /// 廠商名稱
        /// </summary> 
        [Column]
        public string TradeName { get; set; }

        /// <summary> 
        /// 有效期限
        /// </summary> 
        [Column]
        public string ValidityPeriod { get; set; }

        /// <summary> 
        /// 創建時間
        /// </summary> 
        [Column]
        public DateTime CreateDate { get; set; }
        /// <summary> 
        /// 更新時間
        /// </summary> 
        [Column]
        public DateTime? ModifyDate { get; set; }
    }
}
