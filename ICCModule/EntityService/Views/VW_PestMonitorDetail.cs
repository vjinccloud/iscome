using System;
using System.Data.Linq.Mapping;

namespace ICCModule.EntityService.Views
{
    [Table]
    public class VW_PestMonitorDetail
    {
        /// <summary>
        /// 專案名稱
        /// </summary>
        [Column]
        public string ProjName { get; set; }

        /// <summary>
        /// 專案年度
        /// </summary>
        [Column]
        public int ProjYear { get; set; }

        /// <summary>
        /// 顯示狀態
        /// </summary>
        [Column]
        public bool Show { get; set; }

        /// <summary>
        /// 行政區
        /// </summary>
        [Column]
        public string District { get; set; }

        /// <summary>
        /// 里
        /// </summary>
        [Column]
        public string Village { get; set; }

        /// <summary>
        /// 編號
        /// </summary>
        [Column]
        public string SerialNum { get; set; }

        /// <summary>
        /// 緯度
        /// </summary>
        [Column(DbType = "decimal(10,6)")]
        public decimal Lat { get; set; }

        /// <summary>
        /// 經度
        /// </summary>
        [Column(DbType = "decimal(10,6)")]
        public decimal Lng { get; set; }

        /// <summary>
        /// 作物類別
        /// </summary>
        [Column]
        public int Crops { get; set; }

        /// <summary>
        /// 作物
        /// </summary>
        [Column]
        public int CropType { get; set; }

        /// <summary>
        /// 量測日期
        /// </summary>
        [Column]
        public DateTime MeasureDate { get; set; }

        /// <summary>
        /// 害蟲數量
        /// </summary>
        [Column]
        public decimal PestNumbers { get; set; }

        /// <summary>
        /// 量測日期文字
        /// </summary>
        public string MeasureDateText => MeasureDate.ToString("yyyy/MM/dd");
    }
}
