using System;
using System.Collections.Generic;
using System.Linq;
using ICCModule.Entity.Tables.Platform;
using ICCModule.EntityService.Service;
using ICCModule.Helper;
using InformationSystem.Models.Statistic.StatResult;

namespace InformationSystem.Models.Statistic.StatModel
{
    /// <summary>
    /// 作物類別、作物總面積與診斷次數圖表
    /// </summary>
    public class CropsTypeAndAreaDiagnosis : IAreaStat<List<CropsTypeAndAreaDiagnosisResult>>
    {
        /// <inheritdoc />
        public string StatisticName => "作物類別、作物總面積與診斷次數圖表";

        /// <summary>
        /// 建立作物類別、作物總面積與診斷次數圖表
        /// </summary>
        /// <param name="year">西元年</param>
        /// <param name="month">月份(1-12)</param>
        public CropsTypeAndAreaDiagnosis(int year, int month)
        {
            var queryStartDate = new DateTime(year, month, 1);
            var queryEndDate = queryStartDate.AddMonths(1);

            this._rawData = Service_doctorSchedule.GetList(x =>
                x.ReserveDatetime > queryStartDate && x.ReserveDatetime < queryEndDate);
        }

        /// <summary>
        /// 來源原始資料
        /// </summary>
        private readonly List<doctorSchedule> _rawData;

        /// <inheritdoc />
        object IAreaStat.Result()
        {
            return Result();
        }

        /// <inheritdoc />
        public List<CropsTypeAndAreaDiagnosisResult> Result()
        {
            return this._rawData
                .GroupBy(x => x.CropType)
                .Select(x => new CropsTypeAndAreaDiagnosisResult
                {
                    CropType = x.Key,
                    TotalArea = x.Sum(o => (decimal)o.PlantingArea),
                    TotalCount = x.Count()
                }).ToList();
        }

        /// <inheritdoc />
        public byte[] Export()
        {
            var export = Result().Select(x => new
            {
                植物類別 = x.CropType,
                總面積 = x.TotalArea,
                次數 = x.TotalCount,
            }).ToList();

            return ExcelHelper.RenderDataTableToExcel(ExcelHelper.ConvertToDataTable(export)).ToArray();
        }

        /// <inheritdoc />
        public string ExportFileName() => $"當月作物類別統計_{DateTime.Now:yyyyMMddHHmmss}.xlsx";

    }
}