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
    /// 作物與服務面積
    /// </summary>
    public class CropsAndServiceArea : IAreaStat<List<CropsAndServiceAreaResult>>
    {
        /// <summary>
        /// 建立作物與服務面積模組
        /// </summary>
        /// <param name="startDate">開始日期</param>
        /// <param name="endDate">結束日期</param>
        public CropsAndServiceArea(DateTime? startDate = null, DateTime? endDate = null)
        {
            Expression<Func<doctorSchedule, bool>> filter = x => true;
            if (startDate.HasValue)
            {
                var queryStartDay = startDate.Value.Date;
                filter = filter.And(x => x.ReserveDatetime >= queryStartDay);
            }

            if (endDate.HasValue)
            {
                var queryEndDay = endDate.Value.Date.AddDays(1);
                filter = filter.And(x => x.ReserveDatetime < queryEndDay);
            }

            this._rawData = Service_doctorSchedule.GetList(filter);
        }


        /// <inheritdoc />
        public string StatisticName => "作物類別與服務面積";

        /// <summary>
        /// 來源原始資料
        /// </summary>
        private readonly List<doctorSchedule> _rawData;


        object IAreaStat.Result()
        {
            return Result();
        }


        /// <inheritdoc />
        public List<CropsAndServiceAreaResult> Result()
        {
            var groupData = this._rawData.GroupBy(x => x.CropName)
                .Select(x => new CropsAndServiceAreaResult
                {
                    CropName = x.Key,
                    TotalArea = x.Sum(o => (decimal)o.PlantingArea),
                    TotalCount = x.Count()
                }).OrderBy(x => x.CropName).ToList();

            return groupData;
        }

        /// <inheritdoc />
        public byte[] Export()
        {
            var export = Result().Select(x => new
            {
                作物名稱 = x.CropName,
                總面積 = x.TotalArea,
                次數 = x.TotalCount,
            }).ToList();

            return ExcelHelper.RenderDataTableToExcel(ExcelHelper.ConvertToDataTable(export)).ToArray();
        }

        /// <inheritdoc />
        public string ExportFileName() => $"各月統計_{DateTime.Now.ToString("yyyyMMddHHmmss")}.xlsx";
    }
}