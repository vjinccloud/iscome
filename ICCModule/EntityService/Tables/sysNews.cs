using System;
using System.Collections.Generic;
using System.Data.Linq.Mapping;
using System.Text;
using System.Web.Mvc;

namespace ICCModule.Entity.Tables
{

    [Serializable]
    [Table(Name = "sysNews")]
    public class sysNews
    {
        /// <summary> 
        /// 流水號
        /// </summary> 
        [Column(IsPrimaryKey = true, IsDbGenerated = true, AutoSync = AutoSync.OnInsert)]
        public int Id { get; set; }
        /// <summary>
        /// 主旨
        /// </summary> 
        [Column]
        public string Title { get; set; }
        /// <summary>
        /// 開始日期
        /// </summary> 
        [Column]
        public DateTime StartDate { get; set; }
        /// <summary>
        /// 結束日期
        /// </summary> 
        [Column]
        public DateTime EndDate { get; set; }
        /// <summary>
        /// 是否至頂
        /// </summary> 
        [Column]
        public bool IsTop { get; set; }
        /// <summary>
        /// 內容
        /// </summary> 
        [Column]
        [AllowHtml]
        public string Contents { get; set; }
        /// <summary>
        /// 創建日期
        /// </summary> 
        [Column]
        public DateTime CreateDate { get; set; }
        /// <summary>
        /// 創建人
        /// </summary> 
        [Column]
        public string CreateUser { get; set; }
        /// <summary>
        /// 修改日期
        /// </summary> 
        [Column]
        public DateTime? ModifyDate { get; set; }
        /// <summary>
        /// 修改人
        /// </summary> 
        [Column]
        public string ModifyUser { get; set; }
    }
}
