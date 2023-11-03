namespace InformationSystem.Models.Statistic.StatResult
{
    /// <summary>
    /// 診斷次數與服務面積圖表統計結果
    /// </summary>
    public class DiagnosisAndServiceAreaResult
    {
        /// <summary>
        /// 年月
        /// </summary>
        /// <returns></returns>
        public string YearMonth => $"{Year}-{Month}";

        /// <summary>
        /// 年
        /// </summary>
        public int Year { get; set; }
        
        /// <summary>
        /// 月
        /// </summary>
        public int Month { get; set; }
        
        /// <summary>
        /// 總面積
        /// </summary>
        /// <returns></returns>
        public decimal TotalArea { get; set; }

        /// <summary>
        /// 診斷次數
        /// </summary>
        public int TotalCount { get; set; }
    }
}