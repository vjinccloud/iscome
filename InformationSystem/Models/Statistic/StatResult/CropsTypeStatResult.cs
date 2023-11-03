namespace InformationSystem.Models.Statistic.StatResult
{
    /// <summary>
    /// 作物類別圖表
    /// </summary>
    public class CropsTypeStatResult
    {
        /// <summary>
        /// 作物種類名稱
        /// </summary>
        public string CropsType { get; set; }

        /// <summary>
        /// 數量
        /// </summary>
        public int Count { get; set; }
    }
}