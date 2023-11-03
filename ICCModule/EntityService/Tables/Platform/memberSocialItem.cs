using System;
using System.Data.Linq.Mapping;

namespace ICCModule.Entity.Tables.Platform
{
    [Serializable]
    [Table(Name = "member_SocialItems")]
    class memberSocialItem
    {
        /// <summary> 信箱
        /// 
        /// </summary> 
        [Column]
        public string Email { get; set; }

        /// <summary> 社群登入令牌
        /// 
        /// </summary> 
        [Column]
        public string SocialID { get; set; }

        /// <summary> 社群名稱
        /// 
        /// </summary> 
        [Column]
        public string Service { get; set; }

        /// <summary> 創建時間
        /// 
        /// </summary> 
        [Column]
        public DateTime CreatedAt { get; set; }

        /// <summary> 更新時間
        /// 
        /// </summary> 
        [Column]
        public DateTime UpdatedAt { get; set; }
    }
}
