using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using ICCModule;
using ICCModule.Entity.Tables.Platform;
using ICCModule.EntityService.Service;
using ICCModule.Helper;
using InformationSystem.Models.Statistic.StatResult;

namespace InformationSystem.Models.Statistic.StatModel
{
    /// <summary>
    /// 作物類別、作物總面積與診斷次數圖表(直方圖)
    /// </summary>
    public class AllCropsTypeEachYearStat : IAreaStat<List<CropsTypeEachYearStatResult>>
    {
        /// <inheritdoc />
        public string StatisticName => "作物類別、作物總面積與診斷次數圖表(直方圖)";

        /// <summary>
        /// 統計範圍
        /// </summary>
        private const int STAT_RANGE_YEAR = 5;

        /// <summary>
        /// 建立年度統計報表
        /// </summary>
        /// <param name="year">指定年度</param>
        /// <param name="cropsType">指定作物</param>
        public AllCropsTypeEachYearStat(int year)
        {
            int startYear;
            this._startYear = year - STAT_RANGE_YEAR + 1;
            this._endYear = year;


            Expression<Func<doctorSchedule, bool>> filter = x => true;

            //取的統計年度區間內所有資料
            var queryStartDay = new DateTime(this._startYear, 1, 1);
            var queryEndDay = new DateTime(this._endYear, 1, 1).AddYears(1);
            filter = filter.And(x => x.ReserveDatetime >= queryStartDay && x.ReserveDatetime < queryEndDay);

            this._rawData = Service_doctorSchedule.GetList(filter);
        }


        /// <summary>
        /// 來源原始資料
        /// </summary>
        private readonly List<doctorSchedule> _rawData;

        /// <summary>
        /// 開始年度
        /// </summary>
        private readonly int _startYear;

        /// <summary>
        /// 結束年度
        /// </summary>
        private readonly int _endYear;



        object IAreaStat.Result()
        {
            return Result();
        }


        /// <inheritdoc />
        public List<CropsTypeEachYearStatResult> Result()
        {
            var gpData =
                this._rawData.GroupBy(x => x.ReserveDatetime.Year)
                    .Select(gp => new CropsTypeEachYearStatResult
                    {
                        Year = gp.Key,
                        TotalArea = (decimal)gp.Sum(x => x.PlantingArea),
                        TotalCount = gp.Count(),
                        TotalCrops = gp.Select(x => x.CropType).Distinct().Count(),
                        DistCount = gp.Select(x => x.District).Distinct().Count()
                    }).ToList();

            var result = new List<CropsTypeEachYearStatResult>();
            for (var y = this._startYear; y <= this._endYear; y++)
            {
                result.Add(gpData.FirstOrDefault(x => x.Year == y)
                           ?? new CropsTypeEachYearStatResult() { Year = y });
            }

            return result.ToList();
        }

        /// <inheritdoc />
        public byte[] Export()
        {
            var export = Result().Select(x => new
            {
                年度 = $"{x.Year - 1911}年",
                輔導人次 = x.TotalCount,
                輔導面積 = x.TotalArea,
                作物數 = x.TotalCrops,
                輔導行政區 = x.DistCount
            }).ToList();

            return ExcelHelper.RenderDataTableToExcel(ExcelHelper.ConvertToDataTable(export)).ToArray();
        }

        /// <inheritdoc />
        public string ExportFileName() => $"{this._startYear - 1911}年度輔導人次、輔導面積與作物種類統計結果.xlsx";
    }

}