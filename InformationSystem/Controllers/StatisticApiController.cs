using DocumentFormat.OpenXml.Bibliography;
using DocumentFormat.OpenXml.Wordprocessing;
using ICCModule.Models;
using InformationSystem.Models.Statistic.StatModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using System.Web.Mvc;
using InformationSystem.Models.Statistic;
using Microsoft.Ajax.Utilities;
using Newtonsoft.Json;
using ICCModule.Entity.Tables.Platform;

namespace InformationSystem.Controllers
{
    /// <summary>
    /// 統計功能的API
    /// </summary>
    public class StatisticApiController : Controller
    {
        /// <summary>
        /// 輔導人次、輔導面積與作物種類(行政區) 報表資訊
        /// </summary>
        /// <param name="year">指定年度</param>
        /// <param name="startDate">開始日期</param>
        /// <param name="endDate">結束日期</param>
        /// <param name="district">行政區</param>
        /// <returns></returns>
        public JsonResult GetDistStat(int year, DateTime? startDate, DateTime? endDate, string[] district)
        {
            var statModel = new DistStat(year, startDate, endDate, district ?? Array.Empty<string>());
            return AreaStatResult(statModel);
        }

        /// <summary>
        /// 輔導人次、輔導面積與作物種類(作物類別圖) 報表資訊
        /// </summary>
        /// <param name="year"></param>
        /// <param name="startDate"></param>
        /// <param name="endDate"></param>
        /// <param name="district"></param>
        /// <returns></returns>
        public JsonResult GetCropsTypeStat(int year, DateTime? startDate, DateTime? endDate, string[] district)
        {
            var statModel = new CropsTypeStat(year, startDate, endDate, district ?? Array.Empty<string>());
            return AreaStatResult(statModel);
        }


        /// <summary>
        /// 輔導人次、輔導面積與作物種類(直方圖)-所有作物 報表資訊
        /// </summary>
        /// <param name="year">指定年度，未指定則使用當前年度</param>
        /// <returns></returns>
        public JsonResult GetAllCropsTypeEachYearStat(int? year)
        {
            var statModel = new AllCropsTypeEachYearStat(year ?? DateTime.Now.Year);
            return AreaStatResult(statModel);
        }

        /// <summary>
        /// 輔導人次、輔導面積與作物種類(直方圖)-所有作物 檔案匯出
        /// </summary>
        /// <param name="year">指定年度，未指定則使用當前年度</param>
        /// <returns></returns>
        public FileResult ExportAllCropsTypeEachYearStat(int? year)
        {
            var statModel = new AllCropsTypeEachYearStat(year ?? DateTime.Now.Year);
            return AreaStatExport(statModel);
        }

        /// <summary>
        /// 輔導人次、輔導面積與作物種類(直方圖)-指定作物 報表資訊
        /// </summary>
        /// <param name="year">指定年度，未指定則使用當前年度</param>
        /// <param name="cropsType">指定作物</param>
        /// <returns></returns>
        public JsonResult GetCropsTypeEachYearStat(int? year, string cropsType)
        {
            var statModel = new CropsTypeEachYearStat(year ?? DateTime.Now.Year, cropsType);
            return AreaStatResult(statModel);
        }

        /// <summary>
        /// 輔導人次、輔導面積與作物種類(直方圖)-指定作物 檔案匯出
        /// </summary>
        /// <param name="year">指定年度，未指定則使用當前年度</param>
        /// <param name="cropsType">指定作物</param>
        /// <returns></returns>
        public FileResult ExportCropsTypeEachYearStat(int? year, string cropsType)
        {
            var statModel = new CropsTypeEachYearStat(year ?? DateTime.Now.Year, cropsType);
            return AreaStatExport(statModel);
        }

        /// <summary>
        /// 診斷次數與服務面積圖表 報表資訊
        /// </summary>
        /// <param name="startDate">開始日期</param>
        /// <param name="endDate">結束日期</param>
        /// <returns></returns>
        public JsonResult GetDiagnosisAndServiceArea(DateTime? startDate, DateTime? endDate)
        {
            var statModel = new DiagnosisAndServiceArea(startDate, endDate);
            return AreaStatResult(statModel);
        }

        /// <summary>
        /// 診斷次數與服務面積圖表 檔案匯出
        /// </summary>
        /// <param name="startDate">開始日期</param>
        /// <param name="endDate">結束日期</param>
        /// <returns></returns>
        public FileResult ExportDiagnosisAndServiceArea(DateTime? startDate, DateTime? endDate)
        {
            var statModel = new DiagnosisAndServiceArea(startDate, endDate);
            return AreaStatExport(statModel);
        }

        /// <summary>
        /// 作物類別與服務面積 報表資訊
        /// </summary>
        /// <param name="startDate">開始日期</param>
        /// <param name="endDate">結束日期</param>
        /// <returns></returns>
        public JsonResult GetCropsAndServiceArea(DateTime? startDate, DateTime? endDate)
        {
            var statModel = new CropsAndServiceArea(startDate, endDate);
            return AreaStatResult(statModel);
        }

        /// <summary>
        /// 作物類別與服務面積 檔案匯出
        /// </summary>
        /// <param name="startDate">開始日期</param>
        /// <param name="endDate">結束日期</param>
        /// <returns></returns>
        public FileResult ExportCropsAndServiceArea(DateTime? startDate, DateTime? endDate)
        {
            var statModel = new CropsAndServiceArea(startDate, endDate);
            return AreaStatExport(statModel);
        }

        #region 服務面積統計的共用功能

        private JsonResult AreaStatResult(IAreaStat statModel)
        {
            return Json(statModel.Result(), JsonRequestBehavior.AllowGet);
        }

        private FileResult AreaStatExport(IAreaStat statModel)
        {
            var fileName = statModel.ExportFileName();
            var mime = MimeMapping.GetMimeMapping(fileName);
            return File(statModel.Export(), mime, fileName);
        }

        #endregion
    }
}