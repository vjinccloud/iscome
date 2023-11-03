using ICCModule.EntityService.Service;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Data.Linq.Mapping;
using System.Web.Mvc;

namespace ICCModule.Entity.Tables.Platform
{
    [Serializable]
    [Table(Name = "activity_Sign")]
    public class activitySign
    {
        /// <summary>
        /// 流水號
        /// </summary> 
        [Column(IsPrimaryKey = true, IsDbGenerated = true, AutoSync = AutoSync.OnInsert)]
        public int Id { get; set; }

        /// <summary> 
        /// 活動場次序號
        /// </summary> 
        [Column]
        public int ActivityOpenTimesId { get; set; }

        /// <summary> 
        /// 姓名
        /// </summary> 
        [Column]
        public string Name { get; set; }

        /// <summary> 
        /// 身份證字號
        /// </summary> 
        [Column]
        public string IdentityKey { get; set; }

        /// <summary> 
        /// 電話
        /// </summary> 
        [Column]
        public string Phone { get; set; }

        /// <summary> 
        /// 簽到日期
        /// </summary> 
        [Column]
        public DateTime SignInDate { get; set; }
    }
}
