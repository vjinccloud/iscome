using System;
using System.Data.Linq.Mapping;


namespace ICCModule.Entity.Tables.Platform
{
    [Serializable]
    [Table(Name = "resume_Products")]
    public class resumeProduct
    {
        /// <summary> 流水號
        /// 
        /// </summary> 
        [Column(IsPrimaryKey = true, IsDbGenerated = true, AutoSync = AutoSync.OnInsert)]
        public int ID { get; set; }

        /// <summary> 組織代碼
        /// 
        /// </summary> 
        [Column]
        public string OrganizationCode { get; set; }

        /// <summary> 業者名稱
        /// 
        /// </summary> 
        [Column]
        public string VendorName { get; set; }

        /// <summary> 生產者名稱
        /// 
        /// </summary> 
        [Column]
        public string ProducerName { get; set; }

        /// <summary> 負責人
        /// 
        /// </summary> 
        [Column]
        public string Principal { get; set; }

        /// <summary> 聯絡人
        /// 
        /// </summary> 
        [Column]
        public string ContactPerson { get; set; }

        /// <summary> 地址
        /// 
        /// </summary> 
        [Column]
        public string Address { get; set; }

        /// <summary>
        /// 經緯度
        /// </summary> 
        [Column]
        public string Location { get; set; }

        /// <summary> 公開電話
        /// 
        /// </summary> 
        [Column]
        public string Phone { get; set; }

        /// <summary> 驗證機構
        /// 
        /// </summary> 
        [Column]
        public string VerificationAgency { get; set; }

        /// <summary> 驗證類型
        /// 
        /// </summary> 
        [Column]
        public string VerificationTypes { get; set; }
        
        /// <summary> 驗證品項
        /// 
        /// </summary> 
        [Column]
        public string VerificationItems { get; set; }

        /// <summary> 驗證面積
        /// 
        /// </summary> 
        [Column]
        public double? VerificationArea { get; set; }

        /// <summary> 地段
        /// 
        /// </summary> 
        [Column]
        public string Lot { get; set; }

        /// <summary> 地號
        /// 
        /// </summary> 
        [Column]
        public string LandNum { get; set; }

        /// <summary> 驗證到期日
        /// 
        /// </summary> 
        [Column]
        public DateTime? ExpirationDate { get; set; }
        
        /// <summary> 
        /// 最近檢查日
        /// </summary> 
        [Column]
        public DateTime? LastCheckDate { get; set; }

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
