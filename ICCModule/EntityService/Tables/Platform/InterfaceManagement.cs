using IscomG2C.Utility;
using System;
using System.Data.Linq.Mapping;

namespace ICCModule.Entity.Tables.Platform
{
    [Serializable]
    [Table(Name = "Interface_Managements")]
    public class InterfaceManagement
    {
        /// <summary> 外部系統名稱
        /// 
        /// </summary> 
        [Column(IsPrimaryKey =true)]
        public string Name { get; set; }

        /// <summary> 單位名稱
        /// 
        /// </summary> 
        [Column]
        public string UnitName { get; set; }

        /// <summary> 網址
        /// 
        /// </summary> 
        [Column]
        public string Url { get; set; }

        /// <summary> 創建時間
        /// 
        /// </summary> 
        [Column(IsDbGenerated = true)]
        public DateTime CreatedAt { get; set; }

        /// <summary> 更新時間
        /// 
        /// </summary> 
        [Column]
        public DateTime? UpdatedAt { get; set; }

        /// <summary>
        /// 取回民國年的建立日期
        /// </summary>
        public string CreatedAtStr
        {
            get { return Utility_DateTime.ToFormat_inTaiwanYear(CreatedAt, "yyy/M/dd"); }
            set { CreatedAt = DateTime.Now; }
        }

        /// <summary>
        /// 取回民國年的更新日期
        /// </summary>
        public string UpdatedAtStr
        {
            get { return Utility_DateTime.ToFormat_inTaiwanYear(UpdatedAt, "yyy/M/dd HH:mm"); }
            set { UpdatedAt = Utility_DateTime.ParseExact(value) ?? DateTime.Now; }
        }
    }
}
