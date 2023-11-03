using System;
using System.Data.Linq.Mapping;

namespace ICCModule.Entity.Tables.Platform
{
    [Serializable]
    [Table(Name = "member_SMSVerifications")]
    class memberSMSVerifications
    {
        /// <summary> 行動電話
        /// 
        /// </summary> 
        [Column]
        public string Mobile { get; set; }

        /// <summary> 驗證用令牌
        /// 
        /// </summary> 
        [Column]
        public string Token { get; set; }

        /// <summary> 創建時間
        /// 
        /// </summary> 
        [Column]
        public DateTime CreatedAt { get; set; }
    }
}
