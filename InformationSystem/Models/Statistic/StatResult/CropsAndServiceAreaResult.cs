namespace InformationSystem.Models.Statistic.StatResult
{
    /// <summary>
    /// 作物與服務面積統計結果
    /// </summary>
    public class CropsAndServiceAreaResult
    {
        /// <summary>
        /// 作物名稱
        /// </summary>
        public string CropName { get; set; }

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