using System;
using System.Data.Linq.Mapping;

namespace ICCModule.Entity.Tables.Platform
{
    [Serializable]
    [Table(Name = "LineCommand")]
    public class LineCommand
    {
        /// </summary>
        [Column(IsPrimaryKey = true, IsDbGenerated = true, AutoSync = AutoSync.OnInsert)]
        public int Id { get; set; }

        /// <summary> 
        ///會員序號
        /// </summary>
        [Column]
        public string LineUserId { get; set; }

        /// <summary> 
        /// 姓名
        /// </summary>
        [Column]
        public string Command { get; set; }

        /// <summary> 
        /// Line ID
        /// </summary>
        [Column]
        public string Data { get; set; }

        [Column]
        public int Status { get; set; }

        [Column]
        public DateTime CreateDate { get; set; }



    }
}
