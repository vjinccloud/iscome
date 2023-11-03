namespace InformationSystem.Models.Statistic.StatResult
{
    /// <summary>
    /// 作物類別、作物總面積與診斷次數圖表 輸出結果
    /// </summary>
    public class CropsTypeAndAreaDiagnosisResult
    {
        /// <summary>
        /// 植物類別
        /// </summary>
        public string CropType { get; set; }

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