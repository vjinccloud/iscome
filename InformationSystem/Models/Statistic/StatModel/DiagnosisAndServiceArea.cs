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
    /// 診斷次數與服務面積圖表
    /// </summary>
    public class DiagnosisAndServiceArea : IAreaStat<List<DiagnosisAndServiceAreaResult>>
    {
        /// <summary>
        /// 建立診斷次數與服務面積圖表模組
        /// </summary>
        /// <param name="startDate">開始日期</param>
        /// <param name="endDate">結束日期</param>
        public DiagnosisAndServiceArea(DateTime? startDate = null, DateTime? endDate = null)
        {
            Expression<Func<doctorSchedule, bool>> filter = x => true;
            if (startDate.HasValue)
            {
                var queryStartDay = startDate.Value.Date;
                filter = filter.And(x => x.ReserveDatetime >= startDate);
            }

            if (endDate.HasValue)
            {
                var queryEndDay = endDate.Value.Date.AddDays(1);
                filter = filter.And(x => x.ReserveDatetime < queryEndDay);
            }

            this._rawData = Service_doctorSchedule.GetList(filter);
        }
        
        /// <summary>
        /// 來源原始資料
        /// </summary>
        private readonly List<doctorSchedule> _rawData;
        
        /// <inheritdoc />
        public string StatisticName => "診斷次數與服務面積圖表";

        object IAreaStat.Result()
        {
            return Result();
        }

        /// <inheritdoc />
        public List<DiagnosisAndServiceAreaResult> Result()
        {
            var groupData = 
                this._rawData.GroupBy(x => new { x.ReserveDatetime.Year, x.ReserveDatetime.Month })
                .Select(x => new DiagnosisAndServiceAreaResult
                {
                    Year = x.Key.Year,
                    Month = x.Key.Month,
                    TotalArea = x.Sum(o => (decimal)o.PlantingArea),
                    TotalCount = x.Count()
                }).OrderBy(x => x.Year)
                .ThenBy(x=>x.Month)
                .ToList();

            return groupData;
        }

        /// <inheritdoc />
        public byte[] Export()
        {
            var export = Result().Select(x => new
            {
                年月 = x.YearMonth,
                總面積 = x.TotalArea,
                次數 = x.TotalCount,
            }).ToList();

            return ExcelHelper.RenderDataTableToExcel(ExcelHelper.ConvertToDataTable(export)).ToArray();
        }

        /// <inheritdoc />
        public string ExportFileName() => $"各月統計_{DateTime.Now.ToString("yyyyMMddHHmmss")}.xlsx";
    }
}