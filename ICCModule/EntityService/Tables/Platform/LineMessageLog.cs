using System;
using System.Data.Linq.Mapping;

namespace ICCModule.Entity.Tables.Platform
{
    [Serializable]
    [Table(Name = "LineMessageLog")]
    public class LineMessageLog
    {
        /// <summary> 
        ///流水號
        /// </summary>
        [Column(IsPrimaryKey = true, IsDbGenerated = true, AutoSync = AutoSync.OnInsert)]
        public int Id { get; set; }

        /// <summary> 
        ///會員序號
        /// </summary>
        [Column]
        public int? MemberId { get; set; }

        /// <summary> 
        /// 姓名
        /// </summary>
        [Column]
        public string Name { get; set; }

        /// <summary> 
        /// Line ID
        /// </summary>
        [Column]
        public string LineUserId { get; set; }

        /// <summary> 
        /// 訊息
        /// </summary>
        [Column]
        public string UserMessage { get; set; }

        /// <summary> 日期起日
        ///
        /// </summary>
        [Column]
        public DateTime CreateDate { get; set; }

    }
}
