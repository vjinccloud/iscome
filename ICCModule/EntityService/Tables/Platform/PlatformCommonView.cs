using IscomG2C.Utility;
using System;
using System.Data.Linq.Mapping;
using System.Web.Mvc;

namespace ICCModule.Entity.Tables.Platform
{
    [Serializable]
    [Table(Name = "PlatformCommonViews")]
    public class PlatformCommonView
    {
        /// <summary> 流水號
        /// 
        /// </summary> 
        [Column(IsPrimaryKey = true, IsDbGenerated = true, AutoSync = AutoSync.OnInsert)]
        public long ID { get; set; }

        /// <summary> 分類群組
        /// 
        /// </summary> 
        [Column]
        public string GroupName { get; set; }

        /// <summary> 標題
        /// 
        /// </summary> 
        [Column]
        public string Title { get; set; }

        /// <summary> 內容
        /// 
        /// </summary> 
        [Column]
        [AllowHtml]
        public string Context { get; set; }

        /// <summary> 顯示
        /// 
        /// </summary> 
        [Column]
        public bool Show { get; set; }

        /// <summary> 最後更新人員
        /// 
        /// </summary> 
        [Column]
        public string LastEditorName { get; set; }

        /// <summary>
        /// 點閱數
        /// </summary>
        /// <value>
        /// The clicks.
        /// </value>
        [Column]
        public int Clicks { get; set; }

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
            get { return Utility_DateTime.ToFormat_inTaiwanYear(CreatedAt, "yyy/M/dd HH:mm"); }
            set { CreatedAt = Utility_DateTime.ParseExact(value) ?? DateTime.Now; }
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
