using System.Collections.Generic;
using ICCModule.Entity.Tables.Platform;

namespace InformationSystem.Models.Statistic
{
    /// <summary>
    /// 服務面積統計圖表的介面
    /// </summary>
    /// <typeparam name="TResult">統計結果的資料型態</typeparam>
    public interface IAreaStat<out TResult> : IAreaStat
    {

        /// <summary>
        /// 統計結果
        /// </summary>
        /// <returns></returns>
        TResult Result();
    }

    public interface IAreaStat
    {
        /// <summary>
        /// 統計報表名稱
        /// </summary>
        string StatisticName { get; }

        /// <summary>
        /// 統計結果
        /// </summary>
        /// <returns></returns>
        object Result();

        /// <summary>
        /// 匯出報表
        /// </summary>
        /// <returns></returns>
        byte[] Export();

        /// <summary>
        /// 匯出的報表檔案名稱
        /// </summary>
        /// <returns></returns>
        string ExportFileName();
    }

}