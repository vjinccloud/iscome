using System;
using System.Collections.Generic;
using System.Data.Linq.Mapping;

namespace ICCModule.Entity.Tables.Platform
{
    [Serializable]
    [Table(Name = "LineBroadcast")]
    public class LineBroadcast
    {
        /// <summary> 
        ///流水號
        /// </summary>
        [Column(IsPrimaryKey = true, IsDbGenerated = true, AutoSync = AutoSync.OnInsert)]
        public int Id { get; set; }

        /// <summary> 
        /// 推播類別
        /// </summary>
        [Column]
        public string MessageType { get; set; }

        /// <summary> 
        /// 推播對象
        /// </summary>
        [Column]
        public string MessageTarget { get; set; }

        /// <summary> 
        /// 對象Id
        /// </summary>
        [Column]
        public string TargetUser { get; set; }
        
        /// <summary> 
        /// 對象Id
        /// </summary>
        public List<string> TargetUserList { get; set; }

        /// <summary> 
        /// 推播內容
        /// </summary>
        [Column]
        public string Contents { get; set; }

        /// <summary> 
        ///推播時間
        /// </summary>
        [Column]
        public DateTime CreateDate { get; set; }

    }
}
