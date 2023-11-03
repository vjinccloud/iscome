using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using ICCModule;
using ICCModule.Entity.Tables.Platform;
using ICCModule.EntityService.Service;
using InformationSystem.Models.Statistic.StatResult;

namespace InformationSystem.Models.Statistic.StatModel
{
    /// <summary>
    /// 作物類別圖表
    /// </summary>
    public class CropsTypeStat : IAreaStat<List<CropsTypeStatResult>>
    {
        /// <inheritdoc />
        public string StatisticName => "作物類別圖表";

        /// <summary>
        /// 建立作物類別圖表
        /// </summary>
        /// <param name="year">指定年度</param>
        /// <param name="startDate">開始日期</param>
        /// <param name="endDate">結束日期</param>
        /// <param name="district">行政區(中文)</param>
        public CropsTypeStat(int year, DateTime? startDate = null, DateTime? endDate = null,params string[] district)
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

        public List<CropsTypeStatResult> Result()
        {
            var gpData = this._rawData.Where(x => !string.IsNullOrEmpty(x.CropType))
                .GroupBy(x => x.CropType).Select(x => new CropsTypeStatResult
                {
                    CropsType = x.Key,
                    Count = x.Count()
                });

            return gpData.ToList();
        }

        public byte[] Export()
        {
            throw new System.NotImplementedException("本報表尚不提供匯出功能");
        }

        public string ExportFileName()
        {
            throw new System.NotImplementedException();
        }
    }
}