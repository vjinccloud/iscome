using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using ICCModule;
using ICCModule.Entity.Tables.Platform;
using ICCModule.EntityService.Service;
using InformationSystem.Helper;
using InformationSystem.Models.Statistic.StatResult;
using Microsoft.Ajax.Utilities;
using Microsoft.EntityFrameworkCore.Internal;


namespace InformationSystem.Models.Statistic.StatModel
{
    /// <summary>
    /// 輔導人次、輔導面積與輔導作物種類(行政區)的統計模組
    /// </summary>
    public class DistStat : IAreaStat<DistStatResult>
    {
        /// <inheritdoc />
        public string StatisticName => "輔導人次、輔導面積與輔導作物種類(行政區)";

        /// <summary>
        /// 建立行政區統計報表
        /// </summary>
        /// <param name="year">指定年度</param>
        /// <param name="startDate">開始日期</param>
        /// <param name="endDate">結束日期</param>
        /// <param name="district">行政區(中文)</param>
        public DistStat(int year, DateTime? startDate = null, DateTime? endDate = null, params string[] district)
        {
            if ((startDate.HasValue && startDate.Value.Year != year)
                || (endDate.HasValue && endDate.Value.Year != year))
            {
                throw new ArgumentException("查詢的日期必須包含在選擇的年度");
            }

            Expression<Func<doctorSchedule, bool>> filter = x => true;

            var queryStartDay = startDate ?? new DateTime(year, 1, 1);
            var queryEndDay = endDate ?? new DateTime(year, 1, 1).AddYears(1);

            filter = filter.And(x => x.ReserveDatetime >= queryStartDay)
                           .And(x => x.ReserveDatetime < queryEndDay);

            if (district.Any(x=>!string.IsNullOrWhiteSpace(x)))
            {
                filter = filter.And(x => district.Contains(x.District));
            }

            this._rawData = Service_doctorSchedule.GetList(filter);
        }


        /// <summary>
        /// 來源原始資料
        /// </summary>
        private readonly List<doctorSchedule> _rawData;

        object IAreaStat.Result()
        {
            return Result();
        }

        /// <inheritdoc />
        public DistStatResult Result()
        {
            // 輔導農友人次 TotalCount：診斷紀錄的總筆數（1筆診斷紀錄算1次）。
            // 輔導面積 TotalArea：診斷紀錄輔導面積的總和。
            // 輔導作物數 TotalCrops：診斷紀錄輔導作物種類總數量。
            // 輔導區域
            var result = new DistStatResult
            {
                TotalArea = (decimal)this._rawData.Sum(x => x.PlantingArea),
                TotalCount = this._rawData.Count(),
                TotalCrops = this._rawData.Where(x => !string.IsNullOrEmpty(x.CropType)).Distinct().Count(),
                DistPoints = DistCenterHelper.GetDistCenter(this._rawData.Select(x => x.District).Distinct().ToArray())
            };

            return result;
        }

        /// <inheritdoc />
        public byte[] Export()
        {
            throw new System.NotImplementedException();
        }

        /// <inheritdoc />
        public string ExportFileName()
        {
            throw new System.NotImplementedException();
        }
    }
}