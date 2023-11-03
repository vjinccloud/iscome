using System;
using System.Collections.Generic;
using System.Linq;
using InformationSystem.Helper;

namespace InformationSystem.Models.Statistic.StatResult
{
    /// <summary>
    /// 輔導人次行政區統計的結果
    /// </summary>
    public class DistStatResult
    {
        private decimal _totalArea;

        /// <summary>
        /// 總面積
        /// </summary>
        public decimal TotalArea
        {
            get => Math.Round(this._totalArea, 2);
            set => this._totalArea = value;
        }

        /// <summary>
        /// 次數
        /// </summary>
        public int TotalCount { get; set; }

        /// <summary>
        /// 作物數
        /// </summary>
        public int TotalCrops { get; set; }

        /// <summary>
        /// 輔導區域數
        /// </summary>
        public int DistCount => DistPoints.Count();

        /// <summary>
        /// 行政區清單
        /// </summary>
        public List<DistCenterPoint> DistPoints { get; set; }
    }
}