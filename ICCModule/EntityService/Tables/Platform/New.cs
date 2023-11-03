using ICCModule.EntityService.Service;
using IscomG2C.Utility;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Data.Linq.Mapping;
using System.Web.Mvc;

namespace ICCModule.Entity.Tables.Platform
{
    [Serializable]
    [Table(Name = "News")]
    public class New
    {
        /// <summary> 流水號
        ///
        /// </summary>
        [Column(IsPrimaryKey = true, IsDbGenerated = true, AutoSync = AutoSync.OnInsert)]
        public long ID { get; set; }

        /// <summary> 對應 defCode 的 Kinde
        ///
        /// </summary>
        [Column]
        public string Kind { get; set; }

        /// <summary> 對應 defCode 的 Code
        ///
        /// </summary>
        [Column]
        [Required]
        public string Code { get; set; }

        /// <summary> 標題
        ///
        /// </summary>
        [Column]
        [Required]
        public string Title { get; set; }

        /// <summary> 內容
        ///
        /// </summary>
        [Column]
        [AllowHtml]
        [Required]
        public string Context { get; set; }

        /// <summary>
        /// 點閱數
        /// </summary>
        /// <value>
        /// The clicks.
        /// </value>
        [Column]
        public int Clicks { get; set; }

        /// <summary>
        /// 顯示
        /// </summary>
        [Column]
        [Required]
        public bool Show { get; set; }

        /// <summary> 
        /// 彈窗顯示
        /// </summary>
        [Column]
        [Required]
        public bool PopupShow { get; set; }

        /// <summary> 日期起日
        ///
        /// </summary>
        [Column]
        [DataType(DataType.Date)]
        [Required]
        public DateTime? StartDate { get; set; }

        /// <summary> 日期迄日
        ///
        /// </summary>
        [Column]
        [DataType(DataType.Date)]
        [Required]
        public DateTime? EndDate { get; set; }

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
        /// 取回民國年的開始日期
        /// </summary>
        public string StartDateStr
        {
            get { return Utility_DateTime.ToFormat_inTaiwanYear(StartDate, "yyy/M/dd"); }
            set { StartDate = Utility_DateTime.ParseExact(value) ?? DateTime.Now; }
        }

        /// <summary>
        /// 取回民國年的結束日期
        /// </summary>
        public string EndDateStr
        {
            get { return Utility_DateTime.ToFormat_inTaiwanYear(EndDate, "yyy/M/dd"); }
            set
            {
                EndDate = Utility_DateTime.ParseExact(value);
            }
        }

        /// <summary>
        /// Gets the name of the type from defCode Table
        /// </summary>
        /// <value>
        /// The name of the type.
        /// </value>
        public string TypeName
        {
            get { return Service_defCode.GetDetail(Kind, Code)?.Name; }
        }

        /// <summary>
        /// 處理開放式並行存取
        /// </summary>
        /// <value>
        /// The row version.
        /// </value>
        [Timestamp]
        public byte[] RowVersion { get; set; }

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

        /// <summary>
        /// 取回相關的檔案資訊
        /// </summary>
        /// <value>
        /// The files.
        /// </value>
        public List<FileManagement> Files
        {
            get
            {
                return Service_FileManagement.GetList("News", ID.ToString());
            }
        }
        public List<long> EditOldFiles { get; set; } = new List<long>();
    }
}
