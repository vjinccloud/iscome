namespace InformationSystem.Models.Statistic.StatResult
{
    /// <summary>
    /// 作物類別、作物總面積與診斷次數圖表(直方圖)的統計結果
    /// </summary>
    public class CropsTypeEachYearStatResult
    {
        /// <summary>
        /// 年度
        /// </summary>
        public int Year { get; set; }

        /// <summary>
        /// 總面積
        /// </summary>
        public decimal TotalArea { get; set; }

        /// <summary>
        /// 輔導次數
        /// </summary>
        public int TotalCount { get; set; }

        /// <summary>
        /// 作物數
        /// </summary>
        public int TotalCrops { get; set; }

        /// <summary>
        /// 輔導區域數
        /// </summary>
        public int DistCount { get; set; }
    }
}