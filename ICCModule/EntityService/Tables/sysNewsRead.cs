using System;
using System.Collections.Generic;
using System.Data.Linq.Mapping;
using System.Text;
using System.Web.Mvc;

namespace ICCModule.Entity.Tables
{

    [Serializable]
    [Table(Name = "sysNewsRead")]
    public class sysNewsRead
    {
        /// <summary> 
        /// 登入者ID
        /// </summary> 
        [Column(IsPrimaryKey = true)]
        public string LoginID { get; set; }
        /// <summary>
        /// 讀取日期
        /// </summary> 
        [Column]
        public DateTime ReadDate { get; set; }
    }
}
