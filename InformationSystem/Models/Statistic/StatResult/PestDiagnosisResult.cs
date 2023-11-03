namespace InformationSystem.Models.Statistic.StatResult
{
    /// <summary>
    /// 害物類別、診斷次數圖表統計結果
    /// </summary>
    public class PestDiagnosisResult
    {
        /// <summary>
        /// 害物類別
        /// </summary>
        public string PestType { get; set; }

        /// <summary>
        /// 總面積
        /// </summary>
        public decimal TotalArea { get; set; }

        /// <summary>
        /// 次數
        /// </summary>
        public int TotalCount { get; set; }
    }
}