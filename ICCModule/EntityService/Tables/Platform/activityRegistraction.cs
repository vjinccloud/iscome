using System;
using System.Data.Linq.Mapping;

namespace ICCModule.Entity.Tables.Platform
{
    [Serializable]
    [Table(Name = "activity_Registrations")]
    public class activityRegistraction
    {
        /// <summary> 流水號
        /// 
        /// </summary> 
        [Column(IsPrimaryKey = true, IsDbGenerated = true, AutoSync = AutoSync.OnInsert)]
        public long ID { get; set; }

        /// <summary> 
        /// 會員ID
        /// </summary> 
        [Column]
        public long? MemberId { get; set; }
        
        /// <summary> 活動開放時段ID
        /// 
        /// </summary> 
        [Column]
        public long PlanOpenTimeID { get; set; }

        /// <summary> 姓名
        /// 
        /// </summary> 
        [Column]
        public string Name { get; set; }

        /// <summary> 身分證字號
        /// 
        /// </summary> 
        [Column]
        public string IdentifiedID { get; set; }

        /// <summary> 電話
        /// 
        /// </summary> 
        [Column]
        public string Phone { get; set; }

        /// <summary> 上課時數
        /// 
        /// </summary> 
        [Column]
        public double ClassHours { get; set; }

        /// <summary>
        /// 所在城市
        /// </summary>
        [Column]
        public string City { get; set; }

        /// <summary>
        /// 農藥管理人員證件號
        /// </summary>
        [Column]
        public string PesticideManagementStaffID { get; set; }
        
        /// <summary>
        /// 農藥管理人員證件過期日期
        /// </summary>
        [Column]
        public DateTime? PesticideManagementStaffExpiryDate { get; set; }

        /// <summary>
        /// 服務單位
        /// </summary>
        [Column]
        public string ServiceUnit { get; set; }

        /// <summary>
        /// 餐點種類 (0: 素 1: 葷)
        /// </summary>
        [Column]
        public bool? MealsType { get; set; }

        /// <summary> 狀態
        /// 
        /// </summary> 
        [Column]
        public byte? Status { get; set; }

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
