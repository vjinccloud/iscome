using ICCModule;
using ICCModule.ActionFilters;
using ICCModule.Entity.Tables.Platform;
using ICCModule.EntityService.Service;
using ICCModule.Helper;
using InformationSystem.ViewModel;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Web;
using System.Web.Mvc;

namespace InformationSystem.Controllers
{
    [LoginCheckFilter]//掛載此Filter，如果沒登入，就導去登入頁
    [InterceptorOfController]//系統操作Log
    public class PesticideResidueController : Controller
    {
        // 農藥殘留檢驗資料-List
        public ActionResult Inspection(PesticideResidueQueryReq req)
        {
            Expression<Func<pesticideResidueTesting, bool>> filter = x => true;

            if (req.SampleStartDate.HasValue) filter = filter.And(x => x.SampleDate.Date >= req.SampleStartDate);
            if (req.SampleStartDate.HasValue) filter = filter.And(x => x.SampleDate.Date <= req.SampleEndDate);
            if (!string.IsNullOrEmpty(req.PlanType)) filter = filter.And(x => x.PlanType == req.PlanType);
            if (!string.IsNullOrEmpty(req.District)) filter = filter.And(x => x.District == req.District);
            if (!string.IsNullOrEmpty(req.ProvideUnit)) filter = filter.And(x => x.ProvideUnit == req.ProvideUnit);
            if (!string.IsNullOrEmpty(req.CropType)) filter = filter.And(x => x.CropType == req.CropType);
            if (!string.IsNullOrEmpty(req.CropName)) filter = filter.And(x => x.CropName.Contains(req.CropName));
            if (req.Result.HasValue)
            {
                if (req.Result == 1) filter = filter.And(x => x.Result == true);
                if (req.Result == 2) filter = filter.And(x => x.Result == false);
                if (req.Result == 3) filter = filter.And(x => !x.Result.HasValue);
            }
            if (req.HandingSituation > 0) filter = filter.And(x => x.HandingSituation == req.HandingSituation);
            if (req.ClosingSituation.HasValue) filter = filter.And(x => x.ClosingSituation == req.ClosingSituation);
            if (!string.IsNullOrEmpty(req.SampleName)) filter = filter.And(x => x.SampleName.Contains(req.SampleName));
            if (!string.IsNullOrEmpty(req.SampleContextName)) filter = filter.And(x => x.SampleContext.Contains(req.SampleContextName));
            if (req.SampleContextUseWay.Any())
            {
                foreach (var item in req.SampleContextUseWay)
                {
                    filter = filter.And(x => x.SampleContext.Contains(item));
                }
            }
            if (!string.IsNullOrEmpty(req.SampleNum)) filter = filter.And(x => x.SampleNum.Contains(req.SampleNum));
            if (!string.IsNullOrEmpty(req.ProviderCode)) filter = filter.And(x => x.ProviderCode.Contains(req.ProviderCode));
            if (req.IsTransfer.HasValue) filter = filter.And(x => x.IsTransfer == req.IsTransfer);
            if (!string.IsNullOrEmpty(req.SampleSource)) filter = filter.And(x => x.SampleSource == req.SampleSource);

            var _res = Service_pesticideResidueTesting.GetList(filter);

            if (req.Pages < 1) req.Pages = 1;
            var _response = new PesticideResidueModel()
            {
                prQueryReq = req,
                prModel = _res.OrderByDescending(x => x.CreatedAt).Skip((req.Pages - 1) * 10).Take(10).ToList(),
                TotalPage = (int)Math.Ceiling(((decimal)_res.Count / (decimal)10)),
                Pages = req.Pages,
            };

            return View(_response);
        }

        // 農藥殘留檢驗資料-List
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Inspection(PesticideResidueQueryReq req, string act = "")
        {
            Expression<Func<pesticideResidueTesting, bool>> filter = x => true;

            if (req.SampleStartDate.HasValue) filter = filter.And(filter.And(x => x.SampleDate.Date >= req.SampleStartDate));
            if (req.SampleStartDate.HasValue) filter = filter.And(x => x.SampleDate.Date <= req.SampleEndDate);
            if (!string.IsNullOrEmpty(req.PlanType)) filter = filter.And(x => x.PlanType == req.PlanType);
            if (!string.IsNullOrEmpty(req.District)) filter = filter.And(x => x.District == req.District);
            if (!string.IsNullOrEmpty(req.ProvideUnit)) filter = filter.And(x => x.ProvideUnit == req.ProvideUnit);
            if (!string.IsNullOrEmpty(req.CropType)) filter = filter.And(x => x.CropType == req.CropType);
            if (!string.IsNullOrEmpty(req.CropName)) filter = filter.And(x => (x.CropName??"").Contains(req.CropName));
            if (req.Result.HasValue)
            {
                if (req.Result == 1) filter = filter.And(x => x.Result == true);
                if (req.Result == 2) filter = filter.And(x => x.Result == false);
                if (req.Result == 3) filter = filter.And(x => !x.Result.HasValue);
            }
            if (req.HandingSituation > 0) filter = filter.And(x => x.HandingSituation == req.HandingSituation);
            if (req.ClosingSituation.HasValue) filter = filter.And(x => x.ClosingSituation == req.ClosingSituation);
            if (!string.IsNullOrEmpty(req.SampleName)) filter = filter.And(x => (x.SampleName??"").Contains(req.SampleName));
            if (!string.IsNullOrEmpty(req.SampleContextName)) filter = filter.And(x => (x.SampleContext??"").Contains(req.SampleContextName));
            if (req.SampleContextUseWay.Any())
            {
                foreach (var item in req.SampleContextUseWay)
                {
                    filter = filter.And(x => x.SampleContext.Contains(item));
                }
            }
            if (!string.IsNullOrEmpty(req.SampleNum)) filter = filter.And(x => (x.SampleNum??"").Contains(req.SampleNum));
            if (!string.IsNullOrEmpty(req.ProviderCode)) filter = filter.And(x => (x.ProviderCode??"").Contains(req.ProviderCode));
            if (req.IsTransfer.HasValue) filter = filter.And(x => x.IsTransfer == req.IsTransfer);
            if (!string.IsNullOrEmpty(req.SampleSource)) filter = filter.And(x => x.SampleSource == req.SampleSource);

            var _res = Service_pesticideResidueTesting.GetList(filter);

            if(req.Pages<1)req.Pages = 1;
            var _response = new PesticideResidueModel()
            {
                prQueryReq = req,
                prModel = _res.OrderByDescending(x => x.CreatedAt).Skip((req.Pages-1)*10).Take(10).ToList(),
                TotalPage = (int)Math.Ceiling(((decimal)_res.Count / (decimal)10)),
                Pages = req.Pages,
            };

            #region 匯出
            if (act == "export")
            {
                var _export = _res.Select(x => new
                {
                    檢體編號 = x.SampleNum,
                    採樣日期 = x.SampleDate.ToString("yyyy-MM-dd"),
                    行政區 = x.District,
                    樣品名稱 = x.SampleName,
                    作物名稱 = x.CropNameStr,
                    業者名稱 = x.ProviderName,
                    採樣來源 = _response.SampleSource.FirstOrDefault(o => o.Code == x.SampleSource).Name,
                    檢驗結果 = x.SampleResult ? (string.Join("\r\n",(JsonConvert.DeserializeObject<List<SampleContext>>(x.SampleContext) ?? new List<SampleContext>()).Select(o => $"{o.UseWay} {o.Name} {o.RemainQty}({o.AllowQty})"))) : "未檢出",
                    結果判定 = (!x.Result.HasValue ? "暫存" : (x.Result == true ? "合格" : "不合格")),
                    處理情形 = x.HandingSituation == 1 ? "查處中" : (x.HandingSituation == 2 ? "確認中" : (x.HandingSituation == 3 ? "結案" : (""))),
                    是否裁罰 = x.Penalty > 0 ? "是" : "否",
                    裁罰金額 = x.Penalty.HasValue ? (x.Penalty ?? 0).ToString() : "",
                    是否分期 = (!x.Staging.HasValue ? "" : (x.Staging == true ? "是" : "否")),
                    分期數 = x.Staging == true ? JsonConvert.DeserializeObject<List<StagingData>>(x.StagingData).Count.ToString() : "",
                    繳費期限 = x.PaymentDeadline.HasValue ? (x.PaymentDeadline ?? DateTime.Now).ToString("yyyy-MM-dd") : "",
                    繳費完成 = (!x.PaymentStatus.HasValue ? "" : (x.PaymentStatus == true ? "是" : "否")),
                    移送日期 = x.TransferDate.HasValue ? (x.TransferDate ?? DateTime.Now).ToString("yyyy-MM-dd") : "",
                    結案發文日期 = x.ClosingDate.HasValue ? (x.ClosingDate ?? DateTime.Now).ToString("yyyy-MM-dd") : "",
                    結案發文文號 = x.ClosingNumber,
                    結案說明 = x.ClosingInstructions,
                    轉單植醫輔導 = (x.TransferCounselingDate.HasValue ? (x.TransferCounselingDate ?? DateTime.Now).ToString("yyyy-MM-dd") : ""),
                }).OrderBy(x => x.採樣日期).ToList();

                var _output = ExcelHelper.ConvertToDataTable(_export);
                _output.Columns["業者名稱"].ColumnName = "生產者/業者名稱";

                return File(ExcelHelper.RenderDataTableToExcel(_output), "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", $"農藥殘留檢測紀錄_{DateTime.Now.ToString("yyyyMMddHHmmss")}.xlsx");
            }
            #endregion

            return View(_response);
        }

        // 農藥殘留檢驗資料-內頁
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult InspectionDelete(int ID = 0)
        {
            var _res = new PesticideResidueEditModel();
            var data = Service_pesticideResidueTesting.GetDetail(ID);
            if (data != null)
            {
                Service_pesticideResidueTesting.Delete(ID);
            }
            return RedirectToAction(nameof(Inspection));
        }
        // 農藥殘留檢驗資料-內頁
        public ActionResult InspectionDetail(int id = 0)
        {
            var _res = new PesticideResidueEditModel();
            var data = Service_pesticideResidueTesting.GetDetail(id);
            if (data != null)
            {
                _res.prSaveModels = new PesticideResidueSaveModel()
                {
                    ID = data.ID,
                    Year = data.Year,
                    PlanType = data.PlanType,
                    PlanComment = data.PlanComment,
                    SampleNum = data.SampleNum,
                    SampleDate = data.SampleDate,
                    City = data.City,
                    District = data.District,
                    SampleName = data.SampleName,
                    CropType = data.CropType,
                    CropName = data.CropName,
                    ProviderName = data.ProviderName,
                    ProviderCode = data.ProviderCode,
                    ProviderCity = data.ProviderCity,
                    ProviderDistrict = data.ProviderDistrict,
                    ProviderAddress = data.ProviderAddress,
                    ProviderPhone = data.ProviderPhone,
                    SampleSource = data.SampleSource,
                    SampleLocation = data.SampleLocation,
                    ProvideUnit = data.ProvideUnit,
                    SampleResult = data.SampleResult,
                    SampleContext = JsonConvert.DeserializeObject<List<SampleContext>>(data.SampleContext) ?? new List<SampleContext>(),
                    Result = data.Result,
                    HandingSituation = data.HandingSituation,
                    ClosingSituation = data.ClosingSituation,
                    Penalty = data.Penalty,
                    Staging = data.Staging,
                    PaymentDeadline = data.PaymentDeadline,
                    PaymentStatus = data.PaymentStatus,
                    StagingDatas = JsonConvert.DeserializeObject<List<StagingData>>(data.StagingData) ?? new List<StagingData>(),
                    TransferDate = data.TransferDate,
                    ClosingDate = data.ClosingDate,
                    ClosingNumber = data.ClosingNumber,
                    ClosingInstructions = data.ClosingInstructions,
                    IsTransfer = data.IsTransfer,
                    TransferCounselingDate = data.TransferCounselingDate,
                    AnalyzeDate = data.AnalyzeDate,
                    SampleID = data.SampleID,
                    FastSampleID = data.FastSampleID,
                    JointUnit = data.JointUnit,
                    ProviderUnit = data.ProviderUnit
                };
            }
            return View(_res);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult InspectionDetail(PesticideResidueSaveModel prSaveModel)
        {
            var _data = new pesticideResidueTesting
            {
                ID = prSaveModel.ID,
                Year = prSaveModel.Year,
                PlanType = prSaveModel.PlanType,
                PlanComment = prSaveModel.PlanComment,
                SampleNum = prSaveModel.SampleNum,
                SampleDate = prSaveModel.SampleDate,
                City = prSaveModel.City,
                District = prSaveModel.District,
                SampleName = prSaveModel.SampleName,
                CropType = prSaveModel.CropType,
                CropName = prSaveModel.CropName,
                ProviderName = prSaveModel.ProviderName,
                ProviderCode = prSaveModel.ProviderCode,
                ProviderCity = prSaveModel.ProviderCity,
                ProviderDistrict = prSaveModel.ProviderDistrict,
                ProviderAddress = prSaveModel.ProviderAddress,
                ProviderPhone = prSaveModel.ProviderPhone,
                SampleSource = prSaveModel.SampleSource,
                SampleLocation = prSaveModel.SampleLocation,
                ProvideUnit = prSaveModel.ProvideUnit,
                SampleResult = prSaveModel.SampleResult,
                SampleContext = JsonConvert.SerializeObject(prSaveModel.SampleContext),
                Result = prSaveModel.Result,
                HandingSituation = prSaveModel.HandingSituation,
                ClosingSituation = prSaveModel.ClosingSituation,
                Penalty = prSaveModel.Penalty,
                Staging = prSaveModel.Staging,
                PaymentDeadline = prSaveModel.PaymentDeadline,
                PaymentStatus = prSaveModel.PaymentStatus,
                StagingData = JsonConvert.SerializeObject(prSaveModel.StagingDatas.Where(x => x.PayAmt.HasValue)),
                TransferDate = prSaveModel.TransferDate,
                ClosingDate = prSaveModel.ClosingDate,
                ClosingNumber = prSaveModel.ClosingNumber,
                ClosingInstructions = prSaveModel.ClosingInstructions,
                IsTransfer = prSaveModel.IsTransfer,
                TransferCounselingDate = prSaveModel.TransferCounselingDate,
                AnalyzeDate = prSaveModel.AnalyzeDate,
                SampleID = prSaveModel.SampleID,
                FastSampleID = prSaveModel.FastSampleID,
                JointUnit = prSaveModel.JointUnit,
                ProviderUnit = prSaveModel.ProviderUnit
            };

            var _res = new BaseResult();
            if (Service_pesticideResidueTesting.GetDetail(_data.ID) != null && _data.ID > 0)
            {
                _res = Service_pesticideResidueTesting.Update(_data);
            }
            else
            {
                _res = Service_pesticideResidueTesting.Insert(_data);
            }
            if (!_res.result)
            {
                TempData["TempMsg"] = _res.Msg;
                TempData["TempResult"] = "error";
                prSaveModel.SampleContext = new List<SampleContext>();
                prSaveModel.StagingDatas = new List<StagingData>();
                return View(new PesticideResidueEditModel()
                {
                    prSaveModels = prSaveModel
                });
            }

            return RedirectToAction(nameof(Inspection));
        }
        // 農藥殘留檢驗資料-匯入舊資料
        public ActionResult ImportOldData()
        {
            return View(new BaseResult());
        }
        // 農藥殘留檢驗資料-匯入舊資料
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult ImportOldData(HttpPostedFileBase importFile)
        {
            var _res = new BaseResult();
            var errMsg = "";
            #region 匯入
            if (importFile != null)
            {
                var fileName = Path.GetFileName(importFile.FileName);
                var fileType = Path.GetExtension(fileName);

                var dt = new DataTable();
                if (fileType.ToLower() == ".xls") dt = ExcelHelper.RenderDataTableFromExcelXls(importFile.InputStream, 0, 0);
                else if (fileType.ToLower() == ".xlsx") dt = ExcelHelper.RenderDataTableFromExcelXlsx(importFile.InputStream, 0, 0);
                else
                {
                    errMsg = "僅接受副檔名為xls與xlsx之檔案";
                }
                if (string.IsNullOrEmpty(errMsg))
                {
                    #region 驗證欄位
                    var missRow = "";
                    var needRow = new List<string>() { "年度","計畫別","檢體編號", "採樣日期", "行政區", "樣品名稱", "作物類別", "提供單位", "作物名稱", "生產者/業者名稱", "採樣來源", "檢驗結果", "結果判定", "處理情形", "裁罰金額", "是否分期", "繳費期限", "繳費完成", "移送日期", "結案發文日期", "結案發文文號", "結案說明", "轉單植醫輔導" };
                    foreach (DataColumn dtc in dt.Columns)
                    {
                        if (needRow.Contains(dtc.ColumnName)) needRow.Remove(dtc.ColumnName);
                    }
                    if (needRow.Count > 0) errMsg = $"缺少必要欄位:{string.Join(",", needRow)}";
                    #endregion
                    if (string.IsNullOrEmpty(errMsg) && dt.Rows.Count > 0)
                    {
                        var errorCropList = new List<string>();
                        var PlantCategory = Service_cropType.GetList(x => x.Enable);
                        var PlanType = Service_defCode.GetList("PlanType");
                        var ProvideUnit = Service_defCode.GetList("ProvideUnit");
                        var SampleSource = Service_defCode.GetList("SampleSource");

                        for (int i = 0; i < dt.Rows.Count; i++)
                        {
                            var row = dt.Rows[i];
                            DateTime _date = DateTime.Now;
                            if (!DateTime.TryParse(row["採樣日期"].ToString(), out _date)) errMsg = "採樣日期格式不正確";
                            if (!PlantCategory.Any(x => x.Name == row["作物類別"].ToString()) && row["作物類別"].ToString() != "其他") errorCropList.Add(row["作物類別"].ToString());
                        }
                        if (errorCropList.Any())
                        {
                            if (!string.IsNullOrEmpty(errMsg)) errMsg += "\n對應不到的作物類別：" + string.Join(",", errorCropList);
                            else errMsg += "對應不到的作物類別：" + string.Join(",", errorCropList);
                        }

                        if (!string.IsNullOrEmpty(errMsg))
                        {
                            TempData["TempMsg"] = errMsg;
                            TempData["TempResult"] = "error";
                        }
                        if (string.IsNullOrEmpty(errMsg))
                        {
                            var importData = new List<pesticideResidueTesting>();
                            for (int i = 0; i < dt.Rows.Count; i++)
                            {
                                var row = dt.Rows[i];

                                var thisCropType = PlantCategory.FirstOrDefault(x => x.Name == row["作物類別"].ToString());
                                string _cropType = "其他", _planType = "", _sampleSource = "", _provideUnit = "";
                                if (thisCropType != null) _cropType = thisCropType.ID.ToString();
                                if (SampleSource.Any(x => x.Name == row["採樣來源"].ToString())) _sampleSource = SampleSource.FirstOrDefault(x => x.Name == row["採樣來源"].ToString()).Code;
                                if (PlanType.Any(x => x.Name == row["計畫別"].ToString())) _planType = PlanType.FirstOrDefault(x => x.Name.Contains(row["計畫別"].ToString())).Code;
                                if (ProvideUnit.Any(x => x.Name == row["提供單位"].ToString())) _provideUnit = ProvideUnit.FirstOrDefault(x => x.Name == row["提供單位"].ToString()).Code;

                                var newPRT = new pesticideResidueTesting()
                                {
                                    Year = row["年度"].ToString(),
                                    PlanType = _planType,
                                    SampleNum = row["檢體編號"].ToString(),
                                    SampleDate = DateTime.Parse(row["採樣日期"].ToString()),
                                    City = "高雄市",
                                    ProviderCity = "高雄市",
                                    ProvideUnit = _provideUnit,
                                    District = row["行政區"].ToString(),
                                    SampleName = row["樣品名稱"].ToString(),
                                    CropType = _cropType,
                                    CropName = row["作物名稱"].ToString(),
                                    ProviderName = row["生產者/業者名稱"].ToString(),
                                    SampleSource = _sampleSource,
                                    SampleResult = row["檢驗結果"].ToString() == "檢出",
                                    HandingSituation = 0,
                                    Penalty = row["裁罰金額"].ToString().ToInt32(),
                                    PaymentDeadline = row["繳費期限"].ToString().ToDateTime(),
                                    TransferDate = row["移送日期"].ToString().ToDateTime(),
                                    ClosingDate = row["結案發文日期"].ToString().ToDateTime(),
                                    ClosingNumber = row["結案發文文號"].ToString(),
                                    ClosingInstructions = row["結案說明"].ToString(),
                                    TransferCounselingDate = row["轉單植醫輔導"].ToString().ToDateTime(),
                                };
                                if (row["結果判定"].ToString() == "合格") newPRT.Result = true;
                                else if (row["結果判定"].ToString() == "不合格") newPRT.Result = false;

                                if (row["處理情形"].ToString() == "查處中") newPRT.HandingSituation = 1;
                                else if (row["處理情形"].ToString() == "確認中") newPRT.HandingSituation = 2;
                                else if (row["處理情形"].ToString() == "結案") newPRT.HandingSituation = 1;

                                if (row["是否分期"].ToString() == "是") newPRT.Staging = true;
                                else if (row["是否分期"].ToString() == "否") newPRT.Staging = false;
                                
                                if (row["繳費完成"].ToString() == "是") newPRT.PaymentStatus = true;
                                else if (row["繳費完成"].ToString() == "否") newPRT.PaymentStatus = false;

                                importData.Add(newPRT);
                            }
                            _res = Service_pesticideResidueTesting.Import(importData);
                        }
                    }
                }
            }
            #endregion
            return View(_res);
        }
    }
}