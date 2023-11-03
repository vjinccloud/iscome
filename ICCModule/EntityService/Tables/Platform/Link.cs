using IscomG2C.Utility;
using System;
using System.Data.Linq.Mapping;
using System.Web.Configuration;

namespace ICCModule.Entity.Tables.Platform
{
    [Serializable]
    [Table(Name = "Links")]
    public class Link
    {
        /// <summary> 流水號
        /// 
        /// </summary> 
        [Column(IsPrimaryKey = true, IsDbGenerated = true, AutoSync = AutoSync.OnInsert)]
        public long ID { get; set; }

        /// <summary> 標題
        /// 
        /// </summary> 
        [Column]
        public string Title { get; set; }

        /// <summary> 內容
        /// 
        /// </summary> 
        [Column]
        public string Url { get; set; }

        /// <summary>
        /// 圖片路徑
        /// </summary>
        /// <value>
        /// The image path.
        /// </value>
        [Column]
        public string ImagePath { get; set; }

        public string ImageUrl
        {
            get
            {
                string BackEndHostUrl = WebConfigurationManager.AppSettings["BackendHostUrl"];
                return String.Concat(BackEndHostUrl, ImagePath);
            }
        }

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
