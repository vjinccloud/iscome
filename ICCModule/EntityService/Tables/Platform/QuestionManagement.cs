using ICCModule.EntityService.Service;
using System;
using System.Data.Linq.Mapping;

namespace ICCModule.Entity.Tables.Platform
{
    [Serializable]
    [Table(Name = "QuestionManagement")]
    public class QuestionManagement
    {
        /// <summary> 流水號
        /// 
        /// </summary> 
        [Column(IsPrimaryKey = true, IsDbGenerated = true, AutoSync = AutoSync.OnInsert)]
        public long ID { get; set; }

        /// <summary> 問題類別
        /// 
        /// </summary> 
        [Column]
        public string Type { get; set; }

        /// <summary> 問題
        /// 
        /// </summary> 
        [Column]
        public string Question { get; set; }

        /// <summary> 答案
        /// 
        /// </summary> 
        [Column]
        public string Answer { get; set; }

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
        /// Gets the name of the type from defCode Table
        /// </summary>
        /// <value>
        /// The name of the type.
        /// </value>
        public string TypeName
        {
            get { return Service_defCode.GetDetail("QuestionType", Type)?.Name; }
        }
    }
}
