using System;
using System.Data.Linq.Mapping;

namespace ICCModule.Entity.Tables.Platform
{
    [Serializable]
    [Table(Name = "monitor_Area_Point_Dates")]
    public class monitorAreaPointDate
    {
        /// <summary> 流水號
        /// 
        /// </summary> 
        [Column(IsPrimaryKey = true, IsDbGenerated = true, AutoSync = AutoSync.OnInsert)]
        public int ID { get; set; }

        /// <summary> 區域點位ID
        /// 
        /// </summary> 
        [Column]
        public int AreaPointID { get; set; }

        /// <summary> 日期
        /// 
        /// </summary> 
        [Column]
        public DateTime Date { get; set; }

        /// <summary> 害蟲數
        /// 
        /// </summary> 
        [Column]
        public decimal Pests { get; set; }

        /// <summary> 調查株樹
        /// 
        /// </summary> 
        [Column]
        public int Surveys { get; set; }

        /// <summary> 受害株樹
        /// 
        /// </summary> 
        [Column]
        public int Victims { get; set; }

        /// <summary> 作物生長期
        /// 
        /// </summary> 
        [Column]
        public string CropGrowthPeriod { get; set; }

        /// <summary> 調查面積
        /// 
        /// </summary> 
        [Column]
        public decimal SurveyArea { get; set; }

        /// <summary> 發現FAW
        /// 
        /// </summary> 
        [Column]
        public bool DiscoverFAW { get; set; }

        /// <summary> 受害程度(概述)
        /// 
        /// </summary> 
        [Column]
        public string DamageContent { get; set; }

        /// <summary> 地址
        /// 
        /// </summary> 
        [Column]
        public string Adress { get; set; }

        /// <summary> 其他蟻種資訊
        /// 
        /// </summary> 
        [Column]
        public string OtherArts { get; set; }

        /// <summary> 其他蟻種數量
        /// 
        /// </summary> 
        [Column]
        public decimal OtherArtsNum { get; set; }

        /// <summary> 溫度
        /// 
        /// </summary> 
        [Column]
        public decimal Tempeature { get; set; }

        /// <summary> 0級發生面積(公頃)
        /// 
        /// </summary> 
        [Column]
        public decimal Level_0 { get; set; }

        /// <summary> 1級發生面積(公頃)
        /// 
        /// </summary> 
        [Column]
        public decimal Level_1 { get; set; }

        /// <summary> 2級發生面積(公頃)
        /// 
        /// </summary> 
        [Column]
        public decimal Level_2 { get; set; }

        /// <summary> 3級發生面積(公頃)
        /// 
        /// </summary> 
        [Column]
        public decimal Level_3 { get; set; }

        /// <summary> 4級發生面積(公頃)
        /// 
        /// </summary> 
        [Column]
        public decimal Level_4 { get; set; }

        /// <summary> 備註
        /// 
        /// </summary> 
        [Column]
        public string Comment { get; set; }

        /// <summary> 監測情形紀錄
        /// 
        /// </summary> 
        [Column]
        public string MonitoringRecord { get; set; }

        /// <summary> 日累積雨量
        /// 
        /// </summary> 
        [Column]
        public int DailyRainfall { get; set; }

        /// <summary> 創建時間
        /// 
        /// </summary> 
        [Column]
        public DateTime CreatedAt { get; set; }

        /// <summary> 更新時間
        /// 
        /// </summary> 
        [Column]
        public DateTime? UpdatedAt { get; set; }
    }
}
