using ICCModule.EntityService.Service;
using ICCModule.Helper;
using System;
using System.Data.Linq.Mapping;

namespace ICCModule.Entity.Tables.Platform
{
    [Serializable]
    [Table(Name = "pesticide_Residue_APITesting_Record")]
    public class pesticideResidueAPITestingRecord
    {
        /// <summary> 流水號
        /// 
        /// </summary> 
        [Column(IsPrimaryKey = true, IsDbGenerated = true, AutoSync = AutoSync.OnInsert)]
        public int ID { get; set; }

        /// <summary> 來源資料
        /// 
        /// </summary> 
        [Column]
        public string SourceData { get; set; }

        /// <summary> 創建時間
        /// 
        /// </summary> 
        [Column]
        public DateTime CreatedAt { get; set; }

        /// <summary> 存取IP
        /// 
        /// </summary> 
        [Column]
        public string IP { get; set; }

        /// <summary> 介接結果
        /// 
        /// </summary> 
        [Column]
        public string Message { get; set; }
    }
}
