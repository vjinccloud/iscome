using System;
using System.Data.Linq.Mapping;

namespace ICCModule.Entity.Tables.Platform
{
    [Serializable]
    [Table(Name = "control_History_Crops_Contents")]
    public class controlHistoryCropsContents
    {
        /// <summary> 
        /// 流水號
        /// </summary> 
        [Column(IsPrimaryKey = true, IsDbGenerated = true, AutoSync = AutoSync.OnInsert)]
        public int Id { get; set; }

        /// <summary> 
        /// 防治曆作物序號
        /// </summary> 
        [Column]
        public int ControlHistoryCropId { get; set; }

        /// <summary> 
        /// 防治階段
        /// </summary> 
        [Column]
        public string ControlStage { get; set; }

        /// <summary> 
        /// 名稱
        /// </summary> 
        [Column]
        public string Name { get; set; }

        /// <summary> 
        /// 顯示方式
        /// </summary> 
        [Column]
        public bool ShowType { get; set; }

        /// <summary> 
        /// 區間-起
        /// </summary> 
        [Column]
        public string StartBlock { get; set; }

        /// <summary> 
        /// 區間-迄
        /// </summary> 
        [Column]
        public string EndBlock { get; set; }

        /// <summary> 
        /// 創建時間
        /// </summary> 
        [Column]
        public DateTime CreatedAt { get; set; }

        /// <summary> 
        /// 更新時間
        /// </summary> 
        [Column]
        public DateTime? UpdatedAt { get; set; }
    }
}
