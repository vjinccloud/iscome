using System;
using System.Data.Linq.Mapping;

namespace ICCModule.Entity.Tables.Platform
{
    [Serializable]
    [Table(Name = "member_PasswordResets")]
    class memberPasswordResets
    {
        /// <summary> 信箱
        /// 
        /// </summary> 
        [Column(IsPrimaryKey = true, IsDbGenerated = false)]
        public string Email { get; set; }

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
