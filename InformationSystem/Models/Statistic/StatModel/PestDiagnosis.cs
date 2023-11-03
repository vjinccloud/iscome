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
    /// 害物類別、診斷次數圖表
    /// </summary>
    public class PestDiagnosis : IAreaStat<List<PestDiagnosisResult>>
    {
        /// <summary>
        /// 建立害物類別、診斷次數圖表
        /// </summary>
        /// <param name="year">西元年</param>
        /// <param name="month">月份(1-12)</param>
        public PestDiagnosis(int year, int month)
        {
            var queryStartDate = new DateTime(year, month, 1);
            var queryEndDate = queryStartDate.AddMonths(1);

            this._rawData = Service_doctorSchedule.GetList(x =>
                x.ReserveDatetime > queryStartDate && x.ReserveDatetime < queryEndDate);
        }

        /// <summary>
        /// 統計報表名稱
        /// </summary>
        public string StatisticName => "害物類別、診斷次數圖表";

        /// <summary>
        /// 來源原始資料
        /// </summary>
        private readonly List<doctorSchedule> _rawData;
        
        /// <summary>
        /// 統計結果
        /// </summary>
        /// <returns></returns>
        public List<PestDiagnosisResult> Result()
        {
            //抓害物設定檔
            var pestTypeDef = Service_defCode.GetList("PestType");
            
            //統計害物資料
            var pestType = pestTypeDef.Select(x => new PestDiagnosisResult
            {
                PestType = x.Name,
                TotalArea = this._rawData
                    .Where(o => o.PestTypeArr.Contains(x.Code))
                    .Sum(o => (decimal)o.PlantingArea),
                TotalCount = this._rawData.Count(o => o.PestTypeArr.Contains(x.Code))
            }).ToList();
            
            //加入"其他"
            pestType.Add(new PestDiagnosisResult
            {
                PestType = "其他",
                TotalArea = this._rawData
                    .Where(o => o.PestTypeArr.Contains("Other"))
                    .Sum(o => (decimal)o.PlantingArea),
                TotalCount = this._rawData.Count(o => o.PestTypeArr.Contains("Other"))
            });
            
            return pestType.Where(x => x.TotalCount != 0).ToList();
        }

        /// <summary>
        /// 匯出報表
        /// </summary>
        /// <returns></returns>
        public byte[] Export()
        {
            var export = Result().Select(x => new
            {
                害物類別 = x.PestType,
                總面積 = x.TotalArea,
                次數 = x.TotalCount,
            }).ToList();

            return ExcelHelper.RenderDataTableToExcel(ExcelHelper.ConvertToDataTable(export)).ToArray();
        }

        /// <summary>
        /// 匯出的報表檔案名稱
        /// </summary>
        /// <returns></returns>
        public string ExportFileName() => $"當月害物類別統計_{DateTime.Now.ToString("yyyyMMddHHmmss")}.xlsx";
        
        object IAreaStat.Result()
        {
            return Result();
        }
    }
}