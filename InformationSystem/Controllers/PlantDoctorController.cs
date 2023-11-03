using DocumentFormat.OpenXml.Drawing.Charts;
using ICCModule;
using ICCModule.ActionFilters;
using ICCModule.Entity.Tables;
using ICCModule.Entity.Tables.Platform;
using ICCModule.EntityService.Service;
using ICCModule.Helper;
using ICCModule.Models;
using ICCModule.Models.PlantDoctor;
using ICCModule.ViewModel;
using InformationSystem.Helper;
using InformationSystem.ViewModel.PlantDoctor;
using IscomG2C.Utility;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Drawing;
using System.Drawing.Imaging;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Web;
using System.Web.Mvc;
using System.Web.WebPages;

namespace InformationSystem.Controllers
{
    [LoginCheckFilter]//掛載此Filter，如果沒登入，就導去登入頁
    [InterceptorOfController]//系統操作Log
    public class PlantDoctorController : Controller
    {
        /// <summary>
        /// 植醫掛號-List
        /// </summary>
        /// <returns></returns>
        public ActionResult TicketList()
        {
            var data = Service_doctorSchedule.GetList(x => x.IsTransCase != true || !x.IsTransCase.HasValue);
            RecordListViewModel model = new RecordListViewModel()
            {
                doctorSchedules = data.OrderByDescending(x => x.CreatedAt).Take(20).ToList(),
                TotalPage = (data.Count() / 20) + 1
            };
            if (model.RoleCode == "R08")
            {
                model.doctorSchedules = model.doctorSchedules.Where(x => model.AllDoctors.Select(o => o.LoginID).Contains(x.LoginID)).ToList();
            }
            return View(model);
        }

        /// <summary>
        /// 搜尋植醫掛號列表
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult TicketList(RecordListViewModel model, List<string> DeleteIDs)
        {
            if (DeleteIDs != null && DeleteIDs.Count > 0)
            {
                foreach (string ID in DeleteIDs)
                {
                    Service_doctorSchedule.Delete(Convert.ToInt32(ID));
                }
            }

            #region 篩選
            Expression<Func<doctorSchedule, bool>> filter = x => x.IsTransCase != true || !x.IsTransCase.HasValue;

            switch (model.QueryDateType)
            {
                // 建立日期
                case "CreatedAt":
                    if (model.StartDate.HasValue) filter = filter.And(x => x.CreatedAt >= model.StartDate);
                    if (model.EndDate.HasValue) filter = filter.And(x => x.CreatedAt < model.EndDate.Value.AddDays(1).Date);
                    break;
                // 預約看診日期
                case "ReserveDatetime":
                    if (model.StartDate.HasValue) filter = filter.And(x => x.ReserveDatetime >= model.StartDate);
                    if (model.EndDate.HasValue) filter = filter.And(x => x.ReserveDatetime < model.EndDate.Value.AddDays(1).Date);
                    break;
                // 實際看診日期
                case "DateDiagnosis":
                    if (model.StartDate.HasValue) filter = filter.And(x => x.DateDiagnosis >= model.StartDate);
                    if (model.EndDate.HasValue) filter = filter.And(x => x.DateDiagnosis < model.EndDate.Value.AddDays(1).Date);
                    break;
            }

            if (!String.IsNullOrEmpty(model.DoctorId)) filter = filter.And(x => x.LoginID == model.DoctorId);
            // 行政區
            if (!String.IsNullOrEmpty(model.District)) filter = filter.And(x => x.Zip == model.District);
            // 作物類別
            if (!String.IsNullOrEmpty(model.CropType)) filter = filter.And(x => x.CropType == model.CropType);
            // 作物
            if (!String.IsNullOrEmpty(model.Crop)) filter = filter.And(x => x.CropName == model.Crop);
            // 狀態
            if (!String.IsNullOrEmpty(model.Status)) filter = filter.And(x => x.Status == model.Status);
            // 來源
            if (!String.IsNullOrEmpty(model.Origin)) filter = filter.And(x => x.Origin == model.Origin);
            // 害物種類
            if (!String.IsNullOrEmpty(model.PestType)) filter = filter.And(x => x.PestType.Contains(model.PestType));
            //後送診斷
            if (!String.IsNullOrEmpty(model.TransferDiagnosis)) filter = filter.And(x => x.TransferDiagnosis.Contains(model.TransferDiagnosis));
            if (!String.IsNullOrEmpty(model.KeyWord)) filter = filter.And(x => x.Name.Contains(model.KeyWord) || x.Context.Contains(model.KeyWord) || x.ResultDiagnosis.Contains(model.KeyWord) || x.Recommendation.Contains(model.KeyWord));

            #endregion
            var data = Service_doctorSchedule.GetList(filter);
            if (model.RoleCode == "R08")
            {
                data = data.Where(x => model.AllDoctors.Select(o => o.LoginID).Contains(x.LoginID)).ToList();
            }
            if (!string.IsNullOrEmpty(model.CaseNo))
                data = data.Where(x => x.CaseNo.Contains(model.CaseNo)).ToList();

            if (model.ActionName == "export")
            {
                var actRegister = data.AsEnumerable().Select(x => new
                {
                    來源 = Service_defCode.GetDetail("PlantDoctorRecordOrigin", x.Origin)?.Name ?? "",
                    預約日期 = x.ReserveDatetime.ToString("yyyy-MM-dd HH:mm"),
                    問診方式 = Service_defCode.GetDetail("PlantDoctorInquiry", x.Inquiry)?.Name ?? "",
                    藥檢不合格抽檢單位 = x.UnqualifiedUnit == "Other" ? ($"其他-{x.OtherUnit}") : (Service_defCode.GetDetail("DrugTestUnqualifiedSamplingUnit", x.UnqualifiedUnit)?.Name ?? ""),
                    姓名 = x.Name,
                    性別 = x.SexyStr,
                    行政區 = x.District,
                    作物類別 = x.CropType,
                    作物名稱 = x.CropName,
                    行動電話 = x.Mobile,
                    栽種面積 = x.PlantingArea,
                    耕作方式 = Service_defCode.GetDetail("FarmingMethod", x.FarmingMethod)?.Name ?? "",
                    聯繫管道 = x.ContactMethodStr,
                    發病位置 = string.Join(",", Service_defCode.GetList("OnsetLocation").Where(o => x.OnsetLocationArr.Contains(o.Code)).Select(o => o.Name)),
                    諮詢內容 = x.Context,
                    田區位置 = x.FramLocation ?? "",
                    //北緯 = ,
                    //東經 = ,
                    肥料or營養劑 = x.FertilizerNutrient,
                    殺菌劑 = x.Fungicide,
                    殺蟲蟎劑 = x.Insecticide,
                    殺草劑 = x.Herbicide,
                    其他資材 = x.OtherMedicines,
                    耕作歷史 = Service_defCode.GetDetail("FarmingHistory", x.FarmingHistory)?.Name ?? "",
                    前期作物 = x.ContinuousCrop,
                    害物種類 = string.Join(",", Service_defCode.GetList("PestType").Where(o => x.PestTypeArr.Contains(o.Code)).Select(o => o.Name)) + (!string.IsNullOrEmpty(x.OtherPest) ? $",{x.OtherPest}" : ""),
                    診斷結果 = x.ResultDiagnosis,
                    防治建議類別 = Service_defCode.GetDetail("PreventionRecommendations", x.RecommendationCategory)?.Name ?? "",
                    後送診斷 = Service_defCode.GetDetail("TransferDiagnosis", x.TransferDiagnosis)?.Name ?? "",
                    實際診斷日期 = x.DateDiagnosisStr,
                    植物醫師 = x.DoctorDiagnosis,
                    狀態 = Service_defCode.GetDetail("PlantDoctorRecordStatus", x.Status)?.Name ?? "",
                    植醫備註 = x.DoctorComment,
                }).ToList();
                var numList = new List<string> { "栽種面積" };

                return File(ExcelHelper.RenderDataTableToExcel(ExcelHelper.ConvertToDataTable(actRegister), numList), "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", $"植醫診斷紀錄_{DateTime.Now.ToString("yyyyMMddHHmm")}.xlsx");
            }

            model.doctorSchedules = data.OrderByDescending(x => x.CreatedAt).Skip((model.Page - 1) * 3).Take(3).ToList();
            model.TotalPage = (data.Count() / 3) + 1;

            return View(model);
        }

        /// <summary>
        /// 轉案
        /// </summary>
        /// <param name="ScheduleId"></param>
        /// <param name="DoctorId"></param>
        /// <param name="District"></param>
        /// <param name="TransReason"></param>
        /// <param name="TransRemark"></param>
        /// <returns></returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult ScheduleTrans(string ScheduleId, string DoctorId, string District, string TransReason, string TransRemark)
        {
            JsonResponse result = new JsonResponse()
            {
                result = false,
                Msg = "Fail",
            };
            try
            {
                var _scheduleId = Extention.ToInt32(ScheduleId);

                var getSchedule = Service_doctorSchedule.GetDetail(_scheduleId);
                if (getSchedule != null)
                {
                    getSchedule.IsTransCase = true;
                    getSchedule.TransDocId = DoctorId;
                    getSchedule.TransDistrict = District;
                    getSchedule.TransReason = TransReason;
                    getSchedule.TransRemark = TransRemark;
                    getSchedule.TransDate = DateTime.Now;
                    getSchedule.TransUser = SessionHelper.Get("UserName");
                    getSchedule.Updater = SessionHelper.Get("UserName");

                    Service_doctorSchedule.UpdateTrans(getSchedule, "Trans");

                    var transPlanDoc = Service_sysUserInfo.GetDetail(DoctorId);
                    if (transPlanDoc != null)
                    {
                        var _htmlComtent = new System.Text.StringBuilder();
                        _htmlComtent.Append($@"<!DOCTYPE html>
                                          <html>
                                              <head>
                                                  <meta http-equiv='content-type' content='text/html; charset=utf-8' />
                                                  <meta charset='utf-8' />
                                              </head>
                                              <body>
                                                  {transPlanDoc.UserName} 植物醫師，您好：<br />
                                                  <br />
                                                  <div>案號：{getSchedule.CaseNo}</div>
                                                  <div>已轉案給您，請您點擊下方連結進行收案。</div>
                                                  <div><a href='{Request.Url.Scheme}://{Request.Url.Authority}/PlantDoctor/TransScheduleList' target='_blank'>植物醫師診斷系統暨作物病蟲害預警系統</a></div>
                                                  <br/>
                                                  <div>***本郵件為系統自動發送，請勿直接回信***</div>
                                              </body>
                                          </html>");
                        string refErrMsg = "";
                        MailHelper mailHelper = new MailHelper();
                        var sendMail = mailHelper.SendMail(ref refErrMsg, transPlanDoc.EmailAddress, $"植物醫師診斷系統暨作物病蟲害預警系統－植醫掛號轉案通知", _htmlComtent.ToString());
                    }

                    result.result = true;
                    result.Msg = "Success";
                }
            }
            catch (Exception e)
            {

            }

            return Json(result);
        }

        [HttpGet]
        public ActionResult GetDistrictDoctor(string Zip, bool filterAll = false)
        {
            var allDoctors = Service_sysUserInfo.GetEnableDoctorList();
            var getUserId = SessionHelper.Get("LoginID");
            if (!string.IsNullOrEmpty(getUserId) && allDoctors.Any(x => x.LoginID == getUserId && x.RoleID == "R08"))
            {
                allDoctors = allDoctors.Where(x => x.LoginID == getUserId).ToList();
            }
            if (!string.IsNullOrEmpty(Zip)) allDoctors = allDoctors.Where(x => x.RoleID != "R08" || (x.District ?? "").Contains(Zip)).ToList();
            else if (filterAll) allDoctors = allDoctors.Where(x => x.RoleID == "R05").ToList();

            return Json(allDoctors, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// 匯出Word
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult ExportWord(int id)
        {
            var thisRecord = Service_doctorSchedule.GetDetail(id);
            if (thisRecord != null)
            {
                var checkFirst = !Service_doctorSchedule.GetList(x => x.MemberInfoID == thisRecord.MemberInfoID && x.CropName == thisRecord.CropName && x.CropType == thisRecord.CropType && x.ReserveDatetime < thisRecord.ReserveDatetime && x.ID != thisRecord.ID).Any();
                Dictionary<string, string> dct = new Dictionary<string, string>();
                var ds = new DataSet();

                //■□
                dct.Add("@CaseNo", $"{(thisRecord.ReserveDatetime.Year - 1911).ToString() + thisRecord.ReserveDatetime.ToString("MMdd") }-{thisRecord.CropName}-{thisRecord.District}-{thisRecord.MonthIndex}");
                dct.Add("@ResverveDate", thisRecord.ReserveDatetime.ToString("yyyy年MM月dd日"));
                dct.Add("@DateDiagnosis", (thisRecord.DateDiagnosis.HasValue ? thisRecord.DateDiagnosis.Value.ToString("yyyy年MM月dd日") : ""));

                dct.Add("@FirstTime", $"{(checkFirst ? "■" : "□")}");
                dct.Add("@FirstStartHour", $"{(checkFirst ? thisRecord.ReserveDatetime.ToString("HH") : "  ")}");
                dct.Add("@FirstStartMinute", $"{(checkFirst ? thisRecord.ReserveDatetime.ToString("mm") : "  ")}");
                dct.Add("@FirstEndHour", $"{(checkFirst ? thisRecord.ReserveDatetime.AddMinutes(30).ToString("HH") : "  ")}");
                dct.Add("@FirstEndMinute", $"{(checkFirst ? thisRecord.ReserveDatetime.AddMinutes(30).ToString("mm") : "  ")}");

                dct.Add("@ManyTimes", $"{(!checkFirst ? "■" : "□")}");
                dct.Add("@ManyStartHour", $"{(!checkFirst ? thisRecord.ReserveDatetime.ToString("HH") : "  ")}");
                dct.Add("@ManyStartMinute", $"{(!checkFirst ? thisRecord.ReserveDatetime.ToString("mm") : "  ")}");
                dct.Add("@ManyEndHour", $"{(!checkFirst ? thisRecord.ReserveDatetime.AddMinutes(30).ToString("HH") : "  ")}");
                dct.Add("@ManyEndMinute", $"{(!checkFirst ? thisRecord.ReserveDatetime.AddMinutes(30).ToString("mm") : "  ")}");

                var allInquiry = Service_defCode.GetList("PlantDoctorInquiry").OrderBy(x => x.Sort).ToList();
                var inquiryString = "";
                var i = 0;
                foreach (var item in allInquiry)
                {
                    inquiryString += $"{(item.Code == thisRecord.Inquiry ? "■" : "□")}{item.Name}{((i % 2 == 1) ? "\r\n" : "")}";
                    i++;
                }
                inquiryString += $"{(allInquiry.All(x => x.Code != thisRecord.Inquiry) ? "■" : "□")}其他";
                dct.Add("@Inquiry", inquiryString); //25

                dct.Add("@Name", thisRecord.Name);
                dct.Add("@Mobile", thisRecord.Mobile);
                dct.Add("@Crop", $"{thisRecord.CropType}-{thisRecord.CropName}");
                dct.Add("@District", thisRecord.District);
                dct.Add("@PlantingArea", thisRecord.PlantingArea.ToString());
                dct.Add("@FramLocation", thisRecord.FramLocation);

                dct.Add("@FertilizerNutrient", thisRecord.FertilizerNutrient);
                dct.Add("@Fungicide", thisRecord.Fungicide);
                dct.Add("@Insecticide", thisRecord.Insecticide);
                dct.Add("@Herbicide", thisRecord.Herbicide);
                dct.Add("@OtherMedicines", thisRecord.OtherMedicines);

                var allFarmingMethod = Service_defCode.GetList("FarmingMethod").OrderBy(x => x.Sort).ToList();
                var allFarmingHistory = Service_defCode.GetList("FarmingHistory").OrderBy(x => x.Sort).ToList();
                var allOnsetLocation = Service_defCode.GetList("OnsetLocation").OrderBy(x => x.Sort).ToList();
                var allPestType = Service_defCode.GetList("PestType").OrderBy(x => x.Sort).ToList();
                var allRecommendation = Service_defCode.GetList("PreventionRecommendations").OrderBy(x => x.Sort).ToList();
                var strRecommendation = "";
                if (allRecommendation.Any(x => x.Code == thisRecord.RecommendationCategory)) strRecommendation = allRecommendation.FirstOrDefault(x => x.Code == thisRecord.RecommendationCategory).Name;
                else strRecommendation = "其他";

                dct.Add("@FarmingMethod", string.Join("  ", allFarmingMethod.Select(x => $"{(thisRecord.FarmingMethod == x.Code ? "■" : "□")}{x.Name}")));
                dct.Add("@FarmingHistory", string.Join("  ", allFarmingHistory.Select(x => $"{(thisRecord.FarmingHistory == x.Code ? "■" : "□")}{x.Name}{((!string.IsNullOrEmpty(thisRecord.ContinuousCrop) && x.Code == "Rotation") ? $"，前一期作物：{thisRecord.ContinuousCrop}" : "")}")));//ContinuousCrop
                dct.Add("@OnsetLocation", string.Join("  ", allOnsetLocation.Select(x => $"{(thisRecord.OnsetLocationArr.Contains(x.Code) ? "■" : "□")}{x.Name}")));
                dct.Add("@Context", thisRecord.Context);
                dct.Add("@PestType", string.Join("  ", allPestType.Select(x => $"{(thisRecord.PestTypeArr.Contains(x.Code) ? "■" : "□")}{x.Name}")));
                dct.Add("@ResultDiagnosis", thisRecord.ResultDiagnosis);
                dct.Add("@Recommendation", strRecommendation + (!string.IsNullOrEmpty(thisRecord.Recommendation) ? "-" + thisRecord.Recommendation : ""));

                var allFiles = Service_FileManagement.GetList("doctor_Schedule", thisRecord.ID.ToString());

                if ((!string.IsNullOrEmpty(thisRecord.CropSymptoms) && (thisRecord.CropSymptoms ?? "").Split(',').Any()))
                {
                    if ((!string.IsNullOrEmpty(thisRecord.CropSymptoms) && thisRecord.CropSymptoms.Split(',').Any()))
                    {
                        var dataPic = thisRecord.CropSymptoms.Split(',').ToList();
                        var rContents = new List<PlantDoctorWord>();
                        var z = 1;
                        foreach (var item in dataPic)
                        {
                            var getPic = allFiles.FirstOrDefault(x => item.Contains(x.FileName));
                            if (getPic != null && System.IO.File.Exists(getPic.FilePath))
                            {
                                rContents.Add(new PlantDoctorWord()
                                {
                                    PicNum = z,
                                    PicUrl = getPic.FilePath,
                                });
                                z++;
                            }
                        }
                        ds.Tables.Add(ExcelHelper.ConvertToDataTable(rContents, "@CropPic"));
                    }
                    else ds.Tables.Add(ExcelHelper.ConvertToDataTable(new List<PlantDoctorWord>(), "@CropPic"));
                }
                else
                {
                    ds.Tables.Add(ExcelHelper.ConvertToDataTable(new List<PlantDoctorWord>(), "@CropPic"));
                }
                if ((!string.IsNullOrEmpty(thisRecord.RecentlyFertilizer) && (thisRecord.RecentlyFertilizer ?? "").Split(',').Any()))
                {
                    if ((!string.IsNullOrEmpty(thisRecord.RecentlyFertilizer) && thisRecord.RecentlyFertilizer.Split(',').Any()))
                    {
                        var dataPic = thisRecord.RecentlyFertilizer.Split(',').ToList();
                        var rContents = new List<PlantDoctorWord>();
                        var z = 1;
                        foreach (var item in dataPic)
                        {
                            var getPic = allFiles.FirstOrDefault(x => item.Contains(x.FileName));
                            if (getPic != null && System.IO.File.Exists(getPic.FilePath))
                            {
                                rContents.Add(new PlantDoctorWord()
                                {
                                    PicNum = z,
                                    PicUrl = getPic.FilePath,
                                });
                                z++;
                            }
                        }
                        ds.Tables.Add(ExcelHelper.ConvertToDataTable(rContents, "@RecentlyFertilizer"));
                    }
                    else ds.Tables.Add(ExcelHelper.ConvertToDataTable(new List<PlantDoctorWord>(), "@RecentlyFertilizer"));
                }
                else
                {
                    ds.Tables.Add(ExcelHelper.ConvertToDataTable(new List<PlantDoctorWord>(), "@RecentlyFertilizer"));
                }
                var param = DocHelper.SetWord($"{(thisRecord.ReserveDatetime.Year - 1911).ToString() + thisRecord.ReserveDatetime.ToString("MMdd") }_{thisRecord.CropName}_{thisRecord.District}_{thisRecord.MonthIndex}.docx", dct, ds);

                return Json(param);
            }

            return null;
        }


        /// <summary>
        /// 待收轉案
        /// </summary>
        /// <returns></returns>
        public ActionResult TransScheduleList()
        {
            TransSchedukeViewModel model = new TransSchedukeViewModel();

            var data = Service_doctorSchedule.GetList(x => x.IsTransCase == true);
            var originDefCode = Service_defCode.GetList("PlantDoctorRecordOrigin");
            var allDoc = Service_sysUserInfo.GetEnableDoctorList();

            if (model.RoleCode == "R08") data = data.Where(x => x.TransDocId == model.LoginID).ToList();
            model.doctorSchedule = data.OrderBy(x => x.TransDate).Take(20).Select(x => new TransSchedukeSelectModel
            {
                ID = x.ID,
                CaseNo = x.CaseNo,
                TransDate = (x.TransDate ?? DateTime.Now).ToString("yyyy-MM-dd HH:mm"),
                TransDistrict = CommonDataHelper.GetDistrictName("高雄市", x.TransDistrict),
                TransDocId = x.TransDocId,
                TransDocName = allDoc.FirstOrDefault(o => o.LoginID == x.TransDocId)?.UserName,
                Name = x.Name,
                CropName = x.CropName,
                Origin = originDefCode.FirstOrDefault(o => o.Code.ToLower() == x.Origin.ToLower())?.Name,
            }).ToList();
            return View(model);
        }

        /// <summary>
        /// 待收轉案-搜尋
        /// </summary>
        /// <param name="StartDate"></param>
        /// <param name="EndDate"></param>
        /// <param name="KeyWord"></param>
        /// <returns></returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult TransScheduleList(DateTime? StartDate, DateTime? EndDate, string KeyWord)
        {
            #region 篩選
            Expression<Func<doctorSchedule, bool>> filter = x => x.IsTransCase == true;

            if (StartDate.HasValue) filter = filter.And(x => x.TransDate >= StartDate);
            if (EndDate.HasValue) filter = filter.And(x => x.TransDate < EndDate.Value.AddDays(1).Date);

            #endregion
            var data = Service_doctorSchedule.GetList(filter);
            if (!string.IsNullOrEmpty(KeyWord)) data = data.Where(x => x.CaseNo.Contains(KeyWord)).ToList();

            var originDefCode = Service_defCode.GetList("PlantDoctorRecordOrigin");
            var allDoc = Service_sysUserInfo.GetEnableDoctorList();

            TransSchedukeViewModel model = new TransSchedukeViewModel()
            {
                StartDate = StartDate,
                EndDate = EndDate,
                KeyWord = KeyWord,
                doctorSchedule = data.OrderBy(x => x.TransDate).Take(20).Select(x => new TransSchedukeSelectModel
                {
                    ID = x.ID,
                    CaseNo = x.CaseNo,
                    TransDate = (x.TransDate ?? DateTime.Now).ToString("yyyy-MM-dd HH:mm"),
                    TransDistrict = CommonDataHelper.GetDistrictName("高雄市", x.TransDistrict),
                    TransDocId = x.TransDocId,
                    TransDocName = allDoc.FirstOrDefault(o => o.LoginID == x.TransDocId)?.UserName,
                    Name = x.Name,
                    CropName = x.CropName,
                    Origin = originDefCode.FirstOrDefault(o => o.Code.ToLower() == x.Origin.ToLower())?.Name,
                }).ToList(),
            };
            if (model.RoleCode == "R08")
            {
                model.doctorSchedule = model.doctorSchedule.Where(x => x.TransDocId == model.LoginID).ToList();
            }
            return View(model);
        }

        /// <summary>
        /// 收案
        /// </summary>
        /// <param name="ScheduleId"></param>
        /// <returns></returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult ReceiveSchedule(string ScheduleId)
        {
            JsonResponse result = new JsonResponse()
            {
                result = false,
                Msg = "Fail",
            };
            try
            {
                var _scheduleId = Extention.ToInt32(ScheduleId);

                var getSchedule = Service_doctorSchedule.GetDetail(_scheduleId);
                if (getSchedule != null)
                {
                    getSchedule.LoginID = getSchedule.TransDocId;
                    getSchedule.OrgDistrict = getSchedule.TransDistrict;
                    getSchedule.IsTransCase = false;
                    getSchedule.TransDocId = "";
                    getSchedule.TransDistrict = "";
                    getSchedule.AcceptCaseDate = DateTime.Now;
                    getSchedule.AcceptCaseUser = SessionHelper.Get("UserName");
                    getSchedule.Updater = SessionHelper.Get("UserName");

                    Service_doctorSchedule.UpdateTrans(getSchedule, "Receive");

                    result.result = true;
                    result.Msg = "Success";
                }
            }
            catch (Exception e)
            {

            }

            return Json(result);
        }

        /// <summary>
        /// 植醫掛號-內頁
        /// </summary>
        /// <returns></returns>
        public ActionResult ViewTrans(string ID)
        {
            doctorSchedule schedule = Service_doctorSchedule.GetDetail(Convert.ToInt64(ID));
            if (schedule == null)
            {
                return RedirectToAction(nameof(TicketList));
            }

            var am = Service_defCode.GetList("PlantDoctorAm");
            var pm = Service_defCode.GetList("PlantDoctorPm");
            var ReserveTimeList = new List<string>();
            foreach (defCode d in am)
            {
                ReserveTimeList.Add(d.Code);
            }
            foreach (defCode d in pm)
            {
                ReserveTimeList.Add(d.Code);
            }
            var oldFiles = Service_FileManagement.GetList("doctor_Schedule", ID.ToString());

            InfoListViewModel model = new InfoListViewModel()
            {
                Schedule = schedule,
                ReserveTimeList = ReserveTimeList,
                MapApiKey = ConfigurationManager.AppSettings["MapApiKey"],
                OldCropSymptomsFiles = oldFiles.Where(x => x.FilePath.Contains("CropSymptomsFiles")).ToList(),
                OldRecentlyFertilizerFiles = oldFiles.Where(x => x.FilePath.Contains("RecentlyFertilizerFiles")).ToList(),
            };

            var allDoctors = Service_sysUserInfo.GetEnableDoctorList();
            var getUserId = SessionHelper.Get("LoginID");
            if (!string.IsNullOrEmpty(getUserId) && allDoctors.Any(x => x.LoginID == getUserId && x.RoleID == "R08"))
            {
                allDoctors = allDoctors.Where(x => x.LoginID == getUserId).ToList();

                var thisDoctor = allDoctors.Where(x => x.LoginID == getUserId).FirstOrDefault();
                var selectDistrict = (thisDoctor.District ?? "").Split(',').ToList();
                model.Districts = model.Districts.Where(x => selectDistrict.Contains(x.Zip)).ToList();
            }
            else if (!string.IsNullOrEmpty(model.Schedule.OrgDistrict)) allDoctors = allDoctors.Where(x => x.RoleID != "R08" || (x.District ?? "").Contains(model.Schedule.OrgDistrict)).ToList();
            else allDoctors = allDoctors.Where(x => x.RoleID == "R05").ToList();

            model.DoctorList = allDoctors;
            return View(model);
        }

        /// <summary>
        /// 提供植醫首頁變更紀錄狀態
        /// </summary>
        /// <param name="ID"></param>
        /// <param name="Status"></param>
        /// <returns></returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult ChangeTicketStatus(int ID, string Status)
        {
            doctorSchedule schedule = Service_doctorSchedule.GetDetail(ID);
            BaseResult baseResult = new BaseResult();
            if (schedule == null)
            {
                baseResult.result = false;
                baseResult.Msg = "找不到紀錄";
                return Json(baseResult);
            }

            List<string> AllowStatus = new List<string> { "Processing", "ToBeTracked" };

            if (!AllowStatus.Contains(Status))
            {
                baseResult.result = false;
                baseResult.Msg = "非允許狀態值";
                return Json(baseResult);
            }
            schedule.Status = Status;
            schedule.Updater = SessionHelper.Get("UserName");
            schedule.UpdatedAt = DateTime.Now;

            baseResult = Service_doctorSchedule.Update(schedule);

            return Json(baseResult);
        }

        /// <summary>
        /// 植醫掛號-內頁
        /// </summary>
        /// <returns></returns>
        public ActionResult TicketInfo(string ID)
        {
            doctorSchedule schedule = Service_doctorSchedule.GetDetail(Convert.ToInt64(ID));
            if (schedule == null)
            {
                return RedirectToAction(nameof(TicketList));
            }

            var am = Service_defCode.GetList("PlantDoctorAm");
            var pm = Service_defCode.GetList("PlantDoctorPm");
            var ReserveTimeList = new List<string>();
            foreach (defCode d in am)
            {
                ReserveTimeList.Add(d.Code);
            }
            foreach (defCode d in pm)
            {
                ReserveTimeList.Add(d.Code);
            }
            var oldFiles = Service_FileManagement.GetList("doctor_Schedule", ID.ToString());

            InfoListViewModel model = new InfoListViewModel()
            {
                Schedule = schedule,
                ReserveTimeList = ReserveTimeList,
                MapApiKey = ConfigurationManager.AppSettings["MapApiKey"],
                OldCropSymptomsFiles = oldFiles.Where(x => x.FilePath.Contains("CropSymptomsFiles")).ToList(),
                OldRecentlyFertilizerFiles = oldFiles.Where(x => x.FilePath.Contains("RecentlyFertilizerFiles")).ToList(),
            };

            var allDoctors = Service_sysUserInfo.GetEnableDoctorList();
            var getUserId = SessionHelper.Get("LoginID");
            if (!string.IsNullOrEmpty(getUserId) && allDoctors.Any(x => x.LoginID == getUserId && x.RoleID == "R08"))
            {
                allDoctors = allDoctors.Where(x => x.LoginID == getUserId).ToList();

                var thisDoctor = allDoctors.Where(x => x.LoginID == getUserId).FirstOrDefault();
                var selectDistrict = (thisDoctor.District ?? "").Split(',').ToList();
                model.Districts = model.Districts.Where(x => selectDistrict.Contains(x.Zip)).ToList();
            }
            else if (!string.IsNullOrEmpty(model.Schedule.OrgDistrict)) allDoctors = allDoctors.Where(x => x.RoleID != "R08" || (x.District ?? "").Contains(model.Schedule.OrgDistrict)).ToList();
            else allDoctors = allDoctors.Where(x => x.RoleID == "R05").ToList();

            model.DoctorList = allDoctors;
            return View(model);
        }
        
        /// <summary>
        /// 新增或編輯
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult TicketInfo(InfoListViewModel model)
        {
            if (Convert.ToBoolean(model.Schedule.ID))
            {
                model.Schedule.Updater = SessionHelper.Get("UserName");
                Service_doctorSchedule.Update(model.Schedule);
                // 設定超時未看診，需要增加前台會員預期次數
                if (model.Schedule.Status == "Overtime" && Convert.ToBoolean(model.Schedule.MemberInfoID))
                {
                    memberInfo member = Service_MemberInfo.GetDetail(model.Schedule.MemberInfoID);
                    // 增加逾期次數
                    if (member != null)
                    {
                        // 設定可逾期上限
                        int ExpiredTimesLimit = Convert.ToInt16(ConfigurationManager.AppSettings["PlantDoctor_Reserve_Expired_Times_Limit"]);

                        member.ExpiredTimes++;
                        if (member.ExpiredTimes >= ExpiredTimesLimit && member.BlacklistExpiredAt != null)
                        {
                            // 停權天數
                            int SuspensionDays = Convert.ToInt16(ConfigurationManager.AppSettings["Suspension_Days"]);
                            member.BlacklistExpiredAt = DateTime.Now.AddDays(SuspensionDays);
                        }
                        // 更新前台會員資訊
                        Service_MemberInfo.Update(member);
                    }
                }

                //return RedirectToAction(nameof(TicketList));
            }
            else
            {
                // 以下為新增

                model.Schedule.Creator = SessionHelper.Get("UserName");
                model.Schedule.District = CommonDataHelper.GetDistrictName("高雄市", model.Schedule.Zip);

                Service_doctorSchedule.Insert(model.Schedule);

                // 前台新增會有值醫排班開放名額，其餘均要新增排班紀錄
                if (model.Schedule.Origin != "Member")
                {
                    List<RecordUpdate> recordUpdates = new List<RecordUpdate>();
                    recordUpdates.Add(new RecordUpdate
                    {
                        UpdatedAt = DateTime.Now,
                        UpdatedLoginID = "system",
                        UpdatedUser = "system",
                        Reason = $"掛號預約"
                    });

                    doctorDutyRecord dutyRecord = new doctorDutyRecord
                    {
                        DateStr = model.Schedule.ReserveDatetimeStr,
                        Time = model.Schedule.ReserveTime,
                        Period = Convert.ToInt32(model.Schedule.ReserveTime.Substring(0, 2)) > 12 ? "pm" : "am",
                        Status = 0,
                        IsDel = false,
                        CreateUser = "system",
                        CreateUserLoginID = SessionHelper.Get_LoginID(),
                        DoctorScheduleID = model.Schedule.ID,
                        UpdateDetail = recordUpdates,
                        DoctorId = "peilin26"
                    };

                    Service_doctor_DutyRecord.Insert(dutyRecord);
                }
            }

            #region 上傳檔案
            
            var _oldFile = Service_FileManagement.GetList("doctor_Schedule", model.Schedule.ID.ToString());
            if (_oldFile.Any())
            {
                foreach (var item in _oldFile.Where(x => !model.EditOldCropSymptomsFiles.Contains(x.ID) && !model.EditOldRecentlyFertilizerFiles.Contains(x.ID)))
                {
                    var thisFile = _oldFile.FirstOrDefault(x => x.ID == item.ID);
                    if (thisFile == null)
                    {
                        continue;
                    }
                    else
                    {
                        string _path = thisFile.FilePath;
                        if (System.IO.File.Exists(_path))
                        {
                            System.IO.File.Delete(_path);
                        }
                        Service_FileManagement.Delete((int)thisFile.ID);
                    }
                }
            }
            var remainFiles = Service_FileManagement.GetList("doctor_Schedule", model.Schedule.ID.ToString());
            try
            {
                if (model.CropSymptomsFiles.Any(x => x != null))
                {
                    List<string> CropSymptomsFiles = new List<string>();
                    if (remainFiles.Any(x => x.FilePath.Contains("CropSymptomsFiles"))) CropSymptomsFiles.AddRange(remainFiles.Where(x => x.FilePath.Contains("CropSymptomsFiles")).Select(x => $"/UploadedFiles/PlantDoctor/{model.Schedule.ID}/CropSymptomsFiles/{x.FileName}"));
                    string baseDirectory = $"/UploadedFiles/PlantDoctor/{model.Schedule.ID}/CropSymptomsFiles";
                    foreach (var file in model.CropSymptomsFiles)
                    {
                        if (file == null)
                        {
                            continue;
                        }
                        string _directory = Server.MapPath("~" + baseDirectory);
                        FileHelper.CreateFolder(_directory);

                        if (file.ContentType.Contains("image"))
                        {
                            #region
                            Image image = new Bitmap(file.InputStream);
                            #region 處理圖片尺寸
                            //第一種縮圖用   
                            int fixWidth = 0;
                            int fixHeight = 0;

                            //宣告一個圖片尺寸的設定值
                            int maxPx = 1000;

                            //如果圖片的寬高大於最大值，就往下執行
                            if (image.Width > maxPx || image.Height > maxPx)
                            {
                                //圖片的寬大於圖片的高
                                if (image.Width >= image.Height)
                                {
                                    //設定修改後的圖寬
                                    fixWidth = maxPx;

                                    //設定修改後的圖高
                                    fixHeight = Convert.ToInt32((Convert.ToDouble(fixWidth) / Convert.ToDouble(image.Width)) * Convert.ToDouble(image.Height));
                                }
                                else
                                {
                                    //設定修改後的圖高
                                    fixHeight = maxPx;

                                    //設定修改後的圖寬
                                    fixWidth = Convert.ToInt32((Convert.ToDouble(fixHeight) / Convert.ToDouble(image.Height)) * Convert.ToDouble(image.Width));
                                }
                            }
                            else  //圖片沒有超過設定值，不執行縮圖                     
                            {
                                fixHeight = image.Height;
                                fixWidth = image.Width;
                            }
                            #endregion

                            //輸出一個新圖(就是修改過的圖)
                            Bitmap imageOutput = new Bitmap(image, fixWidth, fixHeight);
                            foreach (var p in image.PropertyItems)
                            {
                                if (p.Id == 0x112)
                                {
                                    var rft = p.Value[0] == 6 ? RotateFlipType.Rotate90FlipNone
                                            : p.Value[0] == 3 ? RotateFlipType.Rotate180FlipNone
                                            : p.Value[0] == 8 ? RotateFlipType.Rotate270FlipNone
                                            : p.Value[0] == 1 ? RotateFlipType.RotateNoneFlipNone
                                            : RotateFlipType.RotateNoneFlipNone;
                                    p.Value[0] = 0;
                                    imageOutput.SetPropertyItem(p);
                                    imageOutput.RotateFlip(rft);
                                }
                            }

                            //副檔名不應該這樣給，但因為此範例沒有讀取檔案的部份所以demo就直接給啦
                            string fixSaveName = $"{DateTime.Now.ToString("yyyyMMddHHmmss")}_" + file.FileName;

                            //釋放掉圖檔         
                            image.Dispose();

                            string _path = Path.Combine(_directory, fixSaveName);
                            //將修改過的圖存於設定的位置
                            imageOutput.Save(_path);

                            // 釋放記憶體 (這行若寫在 imageOutput.Save() 之前，會造成修改結果無法存回原始圖檔，只能另存成一個新的圖檔)。
                            // 若要將修改結果，存成另一個新圖檔，就將此行移至 imageOutput.Save() 之前，並指派一個不同檔名給 fixSaveName 變數
                            imageOutput.Dispose();
                            #endregion
                            var url = String.Concat(baseDirectory, '/', fixSaveName);
                            CropSymptomsFiles.Add(url);
                            Service_FileManagement.Insert(new FileManagement()
                            {
                                TableName = "doctor_Schedule",
                                TableID = model.Schedule.ID.ToString(),
                                FileName = fixSaveName,
                                FilePath = _path,
                                FileLength = file.ContentLength,
                                FileType = file.ContentType,
                                MD5 = FileLogic.CalculateMD5(file.InputStream),
                            });
                        }
                        else
                        {
                            string _FileName = Path.GetFileName(file.FileName);

                            string _path = Path.Combine(_directory, _FileName);
                            file.SaveAs(_path);
                            var url = String.Concat(baseDirectory, '/', _FileName);
                            CropSymptomsFiles.Add(url);
                            Service_FileManagement.Insert(new FileManagement()
                            {
                                TableName = "doctor_Schedule",
                                TableID = model.Schedule.ID.ToString(),
                                FileName = file.FileName,
                                FilePath = _path,
                                FileLength = file.ContentLength,
                                FileType = file.ContentType,
                                MD5 = FileLogic.CalculateMD5(file.InputStream),
                            });
                        }

                    }
                    model.Schedule.CropSymptoms = String.Join(",", CropSymptomsFiles);
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
            try
            {
                if (model.RecentlyFertilizerFiles.Any(x => x != null))
                {
                    List<string> RecentlyFertilizerFiles = new List<string>();
                    if (remainFiles.Any(x => x.FilePath.Contains("RecentlyFertilizerFiles"))) RecentlyFertilizerFiles.AddRange(remainFiles.Where(x => x.FilePath.Contains("RecentlyFertilizerFiles")).Select(x => $"/UploadedFiles/PlantDoctor/{model.Schedule.ID}/RecentlyFertilizerFiles/{x.FileName}"));
                    string baseDirectory = $"/UploadedFiles/PlantDoctor/{model.Schedule.ID}/RecentlyFertilizerFiles";
                    foreach (var file in model.RecentlyFertilizerFiles)
                    {
                        if (file == null)
                        {
                            continue;
                        }
                        string _directory = Server.MapPath("~" + baseDirectory);
                        FileHelper.CreateFolder(_directory);

                        if (file.ContentType.Contains("image"))
                        {
                            #region
                            Image image = new Bitmap(file.InputStream);
                            #region 處理圖片尺寸
                            //第一種縮圖用   
                            int fixWidth = 0;
                            int fixHeight = 0;

                            //宣告一個圖片尺寸的設定值
                            int maxPx = 1000;

                            //如果圖片的寬高大於最大值，就往下執行
                            if (image.Width > maxPx || image.Height > maxPx)
                            {
                                //圖片的寬大於圖片的高
                                if (image.Width >= image.Height)
                                {
                                    //設定修改後的圖寬
                                    fixWidth = maxPx;

                                    //設定修改後的圖高
                                    fixHeight = Convert.ToInt32((Convert.ToDouble(fixWidth) / Convert.ToDouble(image.Width)) * Convert.ToDouble(image.Height));
                                }
                                else
                                {
                                    //設定修改後的圖高
                                    fixHeight = maxPx;

                                    //設定修改後的圖寬
                                    fixWidth = Convert.ToInt32((Convert.ToDouble(fixHeight) / Convert.ToDouble(image.Height)) * Convert.ToDouble(image.Width));
                                }
                            }
                            else  //圖片沒有超過設定值，不執行縮圖                     
                            {
                                fixHeight = image.Height;
                                fixWidth = image.Width;
                            }
                            #endregion

                            //輸出一個新圖(就是修改過的圖)
                            Bitmap imageOutput = new Bitmap(image, fixWidth, fixHeight);
                            foreach (var p in image.PropertyItems)
                            {
                                if (p.Id == 0x112)
                                {
                                    var rft = p.Value[0] == 6 ? RotateFlipType.Rotate90FlipNone
                                            : p.Value[0] == 3 ? RotateFlipType.Rotate180FlipNone
                                            : p.Value[0] == 8 ? RotateFlipType.Rotate270FlipNone
                                            : p.Value[0] == 1 ? RotateFlipType.RotateNoneFlipNone
                                            : RotateFlipType.RotateNoneFlipNone;
                                    p.Value[0] = 0;
                                    imageOutput.SetPropertyItem(p);
                                    imageOutput.RotateFlip(rft);
                                }
                            }

                            //副檔名不應該這樣給，但因為此範例沒有讀取檔案的部份所以demo就直接給啦
                            string fixSaveName = $"{DateTime.Now.ToString("yyyyMMddHHmmss")}_" + file.FileName;

                            //釋放掉圖檔         
                            image.Dispose();

                            string _path = Path.Combine(_directory, fixSaveName);
                            //將修改過的圖存於設定的位置
                            imageOutput.Save(_path);

                            // 釋放記憶體 (這行若寫在 imageOutput.Save() 之前，會造成修改結果無法存回原始圖檔，只能另存成一個新的圖檔)。
                            // 若要將修改結果，存成另一個新圖檔，就將此行移至 imageOutput.Save() 之前，並指派一個不同檔名給 fixSaveName 變數
                            imageOutput.Dispose();
                            #endregion

                            var url = String.Concat(baseDirectory, '/', fixSaveName);
                            RecentlyFertilizerFiles.Add(url);
                            Service_FileManagement.Insert(new FileManagement()
                            {
                                TableName = "doctor_Schedule",
                                TableID = model.Schedule.ID.ToString(),
                                FileName = fixSaveName,
                                FilePath = _path,
                                FileLength = file.ContentLength,
                                FileType = file.ContentType,
                                MD5 = FileLogic.CalculateMD5(file.InputStream),
                            });
                        }
                        else
                        {
                            string _FileName = Path.GetFileName(file.FileName);

                            string _path = Path.Combine(_directory, _FileName);
                            file.SaveAs(_path);
                            var url = String.Concat(baseDirectory, '/', _FileName);
                            RecentlyFertilizerFiles.Add(url);
                            Service_FileManagement.Insert(new FileManagement()
                            {
                                TableName = "doctor_Schedule",
                                TableID = model.Schedule.ID.ToString(),
                                FileName = file.FileName,
                                FilePath = _path,
                                FileLength = file.ContentLength,
                                FileType = file.ContentType,
                                MD5 = FileLogic.CalculateMD5(file.InputStream),
                            });
                        }
                    }
                    model.Schedule.RecentlyFertilizer = String.Join(",", RecentlyFertilizerFiles);
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
            #endregion
            if (!String.IsNullOrEmpty(model.Schedule.CropSymptoms) || !String.IsNullOrEmpty(model.Schedule.RecentlyFertilizer))
            {
                Service_doctorSchedule.Update(model.Schedule);
            }

            return RedirectToAction(nameof(TicketList));
        }

        /// <summary>
        /// 取出當月排班
        /// </summary>
        /// <returns></returns>
        public ActionResult Schedule()
        {
            DateTime date = DateTime.Now;
            var firstDayOfMonth = new DateTime(date.Year, date.Month, 1, 0, 0, 0);
            var lastDayOfMonth = firstDayOfMonth.AddMonths(1).AddDays(-1).AddHours(23).AddMinutes(59).AddSeconds(59);

            ScheduleViewModel model = new ScheduleViewModel
            {
                Events = GetScheduleEvents(firstDayOfMonth, lastDayOfMonth)
            };

            return View(model);
        }

        /// <summary>
        /// 取出當月排班
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Schedule(string District = "", string DoctorId = "")
        {
            DateTime date = DateTime.Now;
            var firstDayOfMonth = new DateTime(date.Year, date.Month, 1, 0, 0, 0);
            var lastDayOfMonth = firstDayOfMonth.AddMonths(1).AddDays(-1).AddHours(23).AddMinutes(59).AddSeconds(59);

            ScheduleViewModel model = new ScheduleViewModel
            {
                Events = GetScheduleEvents(firstDayOfMonth, lastDayOfMonth, DoctorId, District),
                DoctorId = DoctorId,
                District = District,
            };

            return View(model);
        }

        public List<ScheduleEvent> GetScheduleEvents(DateTime firstDayOfMonth, DateTime lastDayOfMonth, string DoctorId = "", string District = "")
        {
            List<doctorDutyRecord> list = Service_doctor_DutyRecord.GetList(x => x.Date >= firstDayOfMonth && x.Date <= lastDayOfMonth);

            var getUserId = SessionHelper.Get("LoginID");
            var _sysUser = Service_sysUserInfo.GetList(x => x.LoginID == getUserId).FirstOrDefault();
            if (_sysUser != null && _sysUser?.RoleID == "R08")
            {
                var selectDistrict = (_sysUser.District ?? "").Split(',').ToList();
                list = list.Where(x => x.DoctorId == _sysUser.LoginID && (selectDistrict.Contains(x.District) || string.IsNullOrEmpty(x.District))).ToList();
            }

            if (!string.IsNullOrEmpty(DoctorId)) list = list.Where(x => x.DoctorId == DoctorId).ToList();
            if (!string.IsNullOrEmpty(District)) list = list.Where(x => District == x.District || string.IsNullOrEmpty(x.District)).ToList();

            var allSysUser = Service_sysUserInfo.GetList(x => true);
            var allKaoDistricts = CommonDataHelper.GetDistricts("高雄市");

            List<ScheduleEvent> events = new List<ScheduleEvent>();
            foreach (doctorDutyRecord d in list)
            {
                var thisDoc = allSysUser.FirstOrDefault(x => x.LoginID == d.DoctorId);
                var thisDistrict = allKaoDistricts.FirstOrDefault(x => x.Zip == (d.District ?? ""));
                string appendStr = String.Empty;
                // 行事曆要顯示的 css class
                string className = String.Empty;

                //如果醫師排班有資料
                if (d.doctorSchedule != null)
                {
                    thisDoc = allSysUser.FirstOrDefault(x => x.LoginID == d.doctorSchedule.LoginID);
                    appendStr = $"-{thisDoc?.UserName ?? ""}-{d.doctorSchedule.CropName}-{d.doctorSchedule.District}-{d.doctorSchedule.MonthIndex}";

                    d.DistrictName = d.doctorSchedule.District;
                    d.DoctorName = thisDoc?.UserName ?? "";

                    switch (d.doctorSchedule.Origin)
                    {
                        case "Member":
                            className = "Member-Event";
                            break;
                        case "PlantDoctor":
                            className = "PlantDoctor-Event";
                            break;
                        case "DrugTest":
                            className = "DrugTest-Event";
                            break;
                        default:
                            break;
                    }
                }
                else
                {
                    d.DistrictName = thisDistrict?.Name ?? "高雄市全區";
                    d.DoctorName = thisDoc?.UserName ?? "";

                    appendStr = $"{thisDoc?.UserName ?? ""}-{thisDistrict?.Name ?? "高雄市全區"}-預約名額1";
                    className = "Reservation-Event";
                }
                events.Add(new ScheduleEvent
                {
                    id = "event_" + d.ID.ToString(),
                    start = d.Date.ToString("yyyy-MM-dd"),
                    end = null,
                    title = d.Time + appendStr,
                    classNames = className,
                    extendedProps = d
                });
            }
            return events;
        }

        [HttpGet]
        public ActionResult GetSchedule(string YearMonth, string District, string DoctorId)
        {
            DateTime date = DateTime.Parse(YearMonth);
            var firstDayOfMonth = new DateTime(date.Year, date.Month, 1, 0, 0, 0);
            var lastDayOfMonth = firstDayOfMonth.AddMonths(1).AddDays(-1).AddHours(23).AddMinutes(59).AddSeconds(59);

            return Json(GetScheduleEvents(firstDayOfMonth, lastDayOfMonth, DoctorId, District), JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public ActionResult GetDoctorDistrict(string DoctorId)
        {
            var _doctors = Service_sysUserInfo.GetDetail(DoctorId);
            if (_doctors != null)
            {
                var allDistrict = CommonDataHelper.GetDistricts("高雄市");
                if (_doctors.RoleID == "R08")
                {
                    var selectDistrict = (_doctors.District ?? "").Split(',').ToList();
                    var _res = allDistrict.Where(x => selectDistrict.Contains(x.Zip)).Select(x => new District
                    {
                        Zip = x.Zip,
                        Name = x.Name
                    }).OrderBy(x => x.Zip).ToList();
                    return Json(_res, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    var _res = allDistrict.Select(x => new District
                    {
                        Zip = x.Zip,
                        Name = x.Name
                    }).ToList();
                    _res.Add(new District { Name = "高雄市全區", Zip = "" });
                    _res = _res.OrderBy(x => x.Zip).ToList();
                    return Json(_res, JsonRequestBehavior.AllowGet);
                }
            }

            return Json(new List<District>(), JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// 植醫新增或藥檢轉單需要檢查 預約時間是否已有安排
        /// </summary>
        /// <param name="Date"></param>
        /// <param name="Time"></param>
        /// <returns></returns>
        [HttpGet]
        public ActionResult CheckDutyRecordNoCreate(string Date, string Time)
        {
            BaseResult result = new BaseResult();
            // 存在為 false 需要提醒更換時段
            result.result = !Service_doctor_DutyRecord.CheckExist(SessionHelper.Get_LoginID(), DateTime.Parse(Date), Time);
            result.Msg = result.result ? "該時段可預約" : "該時段已預約";

            return Json(result, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// 更新預設名額與時間
        /// </summary>
        /// <param name="Period"></param>
        /// <param name="Times"></param>
        /// <returns></returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult UpdateScheduleNumTime(string Period, List<string> Times)
        {
            // 取出時段
            List<defCode> list = Service_defCode.GetList(Period);
            foreach (defCode d in list)
            {
                Service_defCode.Delete(d.Kind, d.Code);
            }

            BaseResult result = new BaseResult();

            try
            {
                int i = 1;
                foreach (string t in Times)
                {
                    if (!String.IsNullOrEmpty(t))
                    {
                        Service_defCode.Insert(new defCode
                        {
                            Kind = Period,
                            KindName = (Period == "PlantDoctorAm" ? "植醫上午場" : "植醫下午場"),
                            Code = t,
                            Name = t,
                            NoUse = "N",
                            Sort = i,
                            Remark = "",
                            AllowUpdate = "Y",
                            CreateDate = DateTime.Now,
                            CreateUser = SessionHelper.Get("UserName"),
                            UpdateDate = DateTime.Now,
                            UpdateUser = SessionHelper.Get("UserName")
                        });
                        i++;
                    }
                }
                result.result = true;
                result.Msg = "Success";
            }
            catch (Exception e)
            {
                result.result = false;
                result.Msg = "Fail";
            }

            return Json(result);
        }

        /// <summary>
        /// 新增排班
        /// </summary>
        /// <param name="StartDate"></param>
        /// <param name="EndDate"></param>
        /// <param name="Weeks"></param>
        /// <param name="Periods"></param>
        /// <param name="OpenNum"></param>
        /// <returns></returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult AddScheduleNumTime(string DoctorId, string Districts, string StartDate, string EndDate, List<string> Weeks, List<string> Periods, int OpenNum)
        {

            // 取出起訖所有日期
            List<DateTime> betweenTwoDate = new List<DateTime>(AllDatesBetween(DateTime.Parse(StartDate), DateTime.Parse(EndDate)));
            List<defCode> amList = Service_defCode.GetList("PlantDoctorAm");
            List<defCode> pmList = Service_defCode.GetList("PlantDoctorPm");

            List<string> exists = new List<string>();

            JsonResponse result = new JsonResponse();

            try
            {
                bool isSendMail = false;
                foreach (DateTime date in betweenTwoDate)
                {
                    string weekday = date.ToString("ddd", new CultureInfo("en-US"));
                    // 星期需要符合
                    if (Weeks.Contains(weekday))
                    {
                        // 上午下午均要建立空預約
                        foreach (string period in Periods)
                        {
                            for (int i = 0; i < OpenNum; i++)
                            {
                                string Time = String.Empty;
                                if (period == "am")
                                {
                                    if ((i + 1) <= amList.Count)
                                    {
                                        Time = amList.ElementAt(i).Code;
                                    }
                                }
                                else
                                {
                                    if ((i + 1) <= pmList.Count)
                                    {
                                        Time = pmList.ElementAt(i).Code;
                                    }
                                }

                                if (!String.IsNullOrEmpty(Time))
                                {
                                    // 紀錄存在的，提醒已經新增過
                                    if (Service_doctor_DutyRecord.CheckExist(x => x.DoctorId == DoctorId && x.Date == date && x.Time == Time))
                                    {
                                        exists.Add(String.Format("{0:yyyy-MM-dd}", date) + " " + Time);
                                    }
                                    else
                                    {
                                        Service_doctor_DutyRecord.Insert(new doctorDutyRecord
                                        {
                                            CreateUserLoginID = SessionHelper.Get_LoginID(),
                                            Date = date,
                                            Time = Time,
                                            Period = period,
                                            Status = 0,
                                            IsDel = false,
                                            DoctorScheduleID = 0,
                                            CreateUser = SessionHelper.Get_UserName(),
                                            DoctorId = DoctorId,
                                            District = Districts
                                        });
                                        isSendMail = true;
                                    }
                                }
                            }
                        }
                    }
                }

                #region 寄信通知
                if (SessionHelper.Get("LoginID") != DoctorId && isSendMail)
                {
                    var setScheduleDoc = Service_sysUserInfo.GetDetail(DoctorId);
                    if (setScheduleDoc != null)
                    {
                        var weekList = new List<string>();
                        if (Weeks.Contains("Mon")) weekList.Add("星期一");
                        if (Weeks.Contains("Tue")) weekList.Add("星期二");
                        if (Weeks.Contains("Wed")) weekList.Add("星期三");
                        if (Weeks.Contains("Thu")) weekList.Add("星期四");
                        if (Weeks.Contains("Fri")) weekList.Add("星期五");
                        if (Weeks.Contains("Sat")) weekList.Add("星期六");
                        if (Weeks.Contains("Sun")) weekList.Add("星期日");

                        var apList = new List<string>();
                        if (Periods.Contains("am")) apList.Add("上午場");
                        if (Periods.Contains("pm")) apList.Add("下午場");

                        var dateString = betweenTwoDate.Min().ToString("yyyy 月 MM 月 dd 日");
                        if (betweenTwoDate.Count > 1) dateString += betweenTwoDate.Max().ToString("yyyy 月 MM 月 dd 日");

                        var _htmlComtent = new System.Text.StringBuilder();
                        _htmlComtent.Append($@"<!DOCTYPE html>
                                          <html>
                                              <head>
                                                  <meta http-equiv='content-type' content='text/html; charset=utf-8' />
                                                  <meta charset='utf-8' />
                                              </head>
                                              <body>
                                                  {setScheduleDoc.UserName} 植物醫師，您好：<br />
                                                  <br />
                                                  <div>排班日期：{dateString}</div>
                                                  <div>設定星期：{string.Join("、", weekList)}</div>
                                                  <div>設定時段：{string.Join("、", apList)}</div>
                                                  <div>行政區：{(string.IsNullOrEmpty(Districts) ? "高雄市全區" : CommonDataHelper.GetDistrictName("高雄市", Districts))}</div>
                                                  <br/>
                                                  <div>已新增您的排班，詳情請點擊下方連結登入查看。</div>
                                                  <div><a href='{Request.Url.Scheme}://{Request.Url.Authority}/PlantDoctor/Schedule' target='_blank'>植物醫師診斷系統暨作物病蟲害預警系統</a></div>
                                                  <br/>
                                                  <div>***本郵件為系統自動發送，請勿直接回信***</div>
                                              </body>
                                          </html>");
                        string refErrMsg = "";
                        MailHelper mailHelper = new MailHelper();
                        var sendMail = mailHelper.SendMail(ref refErrMsg, setScheduleDoc.EmailAddress, $"植物醫師診斷系統暨作物病蟲害預警系統－新增排班通知信", _htmlComtent.ToString());
                    }
                }
                #endregion
                result.result = true;
                result.Msg = "Success";
                result.Data = JsonConvert.SerializeObject(exists);
            }
            catch (Exception e)
            {
                result.result = false;
                result.Msg = "Fail";
            }


            return Json(result);
        }

        static IEnumerable<DateTime> AllDatesBetween(DateTime start, DateTime end)
        {
            for (var day = start.Date; day <= end; day = day.AddDays(1))
                yield return day;
        }

        /// <summary>
        /// 刪除排班紀錄
        /// </summary>
        /// <param name="ID"></param>
        /// <param name="DelReason"></param>
        /// <returns></returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteSchedule(string ID, string DelReason)
        {
            BaseResult result = new BaseResult();
            doctorDutyRecord record = Service_doctor_DutyRecord.GetDetail(ID);
            if (record == null)
            {
                result.result = false;
                result.Msg = "紀錄不存在";
                return Json(result);
            }

            // 代表沒有被預約的紀錄，直接刪除
            if (record.DoctorScheduleID == 0)
            {
                SendDelScheduleMail(record.DoctorId, record.Date, record.Time);
                result = Service_doctor_DutyRecord.Delete(record.ID);
                result.Msg = "刪除成功";
                return Json(result);
            }

            // 預約紀錄進行刪除
            doctorSchedule schedule = record.doctorSchedule;
            if (schedule != null)
            {
                schedule.IsDel = true;
                // 前台會員新增，狀態變更為 植醫取消預約
                if (schedule.Origin == "Member")
                {
                    schedule.Status = "PlantDoctorCancel";
                }
                result = Service_doctorSchedule.Update(schedule);
                if (result.result == false)
                {
                    result.Msg = "刪除預約記錄未正確，請稍後再試";
                    return Json(result);
                }
                // 發送簡訊
                string period = Convert.ToInt32(schedule.ReserveTime.Substring(0, 2)) > 12 ? "下午" : "上午";

                SmsReq req = new SmsReq
                {
                    Mobile = schedule.Mobile,
                    Message = $"高雄市農業局植物防疫平台 - 植醫掛號取消預約通知：預約日期 {schedule.ReserveDatetimeStr} {period}{schedule.ReserveTime}，因故取消該次預約，不便之處，敬請見諒。"
                };
                SmsHelper.SendSms(req);
            }
            // 更新預約記錄為刪除
            List<RecordUpdate> recordUpdates = record.UpdateDetail;
            if (recordUpdates == null)
            {
                recordUpdates = new List<RecordUpdate>();
            }
            recordUpdates.Add(new RecordUpdate
            {
                UpdatedAt = DateTime.Now,
                UpdatedLoginID = SessionHelper.Get_LoginID(),
                UpdatedUser = SessionHelper.Get_UserName(),
                Reason = $"刪除預約，原因說明: {DelReason}"
            });
            record.Status = (int)DutyRecordStatus.Deleted;
            record.IsDel = true;
            record.UpdateDetail = recordUpdates;
            result = Service_doctor_DutyRecord.Update(record);

            SendDelScheduleMail(record.DoctorId, record.Date, record.Time);
            return Json(result);
        }

        /// <summary>
        /// 刪除全行政區排班
        /// </summary>
        /// <param name="Date"></param>
        /// <param name="Time"></param>
        /// <param name="Period"></param>
        /// <returns></returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteAllSchedule(DateTime DelDate, string Time, string Period)
        {
            BaseResult result = new BaseResult();
            try
            {
                var allRecord = Service_doctor_DutyRecord.GetList(x => x.Date == DelDate && x.Time == Time && x.Period == Period && x.DoctorScheduleID == 0);
                foreach (var record in allRecord)
                {
                    result = Service_doctor_DutyRecord.Delete(record.ID);
                }
                foreach (var doctorId in allRecord.Select(x => x.DoctorId).Distinct().ToList())
                {
                    SendDelScheduleMail(doctorId, DelDate, Time);
                }
                result.result = true;
                result.Msg = "刪除成功";
            }
            catch (Exception ex)
            {
                result.result = false;
                result.Msg = ex.Message;
            }
            return Json(result);
        }

        public void SendDelScheduleMail(string DoctorId, DateTime DelDate, string Time)
        {
            if (SessionHelper.Get("LoginID") != DoctorId)
            {
                var setScheduleDoc = Service_sysUserInfo.GetDetail(DoctorId);
                if (setScheduleDoc != null)
                {
                    var weekStr = "";
                    if ((int)DelDate.DayOfWeek == 1) weekStr = "星期一";
                    if ((int)DelDate.DayOfWeek == 2) weekStr = "星期二";
                    if ((int)DelDate.DayOfWeek == 3) weekStr = "星期三";
                    if ((int)DelDate.DayOfWeek == 4) weekStr = "星期四";
                    if ((int)DelDate.DayOfWeek == 5) weekStr = "星期五";
                    if ((int)DelDate.DayOfWeek == 6) weekStr = "星期六";
                    if ((int)DelDate.DayOfWeek == 0) weekStr = "星期日";

                    var dateString = DelDate.ToString("yyyy 月 MM 月 dd 日");
                    var _htmlComtent = new System.Text.StringBuilder();
                    _htmlComtent.Append($@"<!DOCTYPE html>
                                          <html>
                                              <head>
                                                  <meta http-equiv='content-type' content='text/html; charset=utf-8' />
                                                  <meta charset='utf-8' />
                                              </head>
                                              <body>
                                                  {setScheduleDoc.UserName} 植物醫師，您好：<br />
                                                  <br />
                                                  <div>排班日期：{dateString}</div>
                                                  <div>設定星期：{weekStr}</div>
                                                  <div>設定時段：{Time}</div>
                                                  <br/>
                                                  <div>已刪除您的排班，詳情請點擊下方連結登入查看。</div>
                                                  <div><a href='{Request.Url.Scheme}://{Request.Url.Authority}/PlantDoctor/Schedule' target='_blank'>植物醫師診斷系統暨作物病蟲害預警系統</a></div>
                                                  <br/>
                                                  <div>***本郵件為系統自動發送，請勿直接回信***</div>
                                              </body>
                                          </html>");
                    string refErrMsg = "";
                    MailHelper mailHelper = new MailHelper();
                    var sendMail = mailHelper.SendMail(ref refErrMsg, setScheduleDoc.EmailAddress, $"植物醫師診斷系統暨作物病蟲害預警系統－刪除排班通知信", _htmlComtent.ToString());
                }
            }
        }

        /// <summary>
        /// 變更排班紀錄，有關聯診斷紀錄才可以變更排班
        /// </summary>
        /// <param name="ID"></param>
        /// <param name="ChangeDate"></param>
        /// <param name="Time"></param>
        /// <returns></returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult ChangeSchedule(string ID, string ChangeDate, string Time)
        {
            BaseResult result = new BaseResult();
            doctorDutyRecord dutyRecord = Service_doctor_DutyRecord.GetDetail(ID);

            doctorSchedule schedule = dutyRecord.doctorSchedule;
            if (schedule != null)
            {
                schedule.ReserveDatetimeStr = ChangeDate;
                schedule.ReserveTime = Time;
                result = Service_doctorSchedule.Update(schedule);
                if (result.result == false)
                {
                    result.Msg = "變更預約記錄未正確，請稍後再試";
                    return Json(result);
                }
                string period = Convert.ToInt32(schedule.ReserveTime.Substring(0, 2)) > 12 ? "下午" : "上午";
                // 發送簡訊
                SmsReq req = new SmsReq
                {
                    Mobile = schedule.Mobile,
                    Message = $"高雄市農業局植物防疫平台 - 植醫掛號變更預約通知：預約日期 {schedule.ReserveDatetimeStr} {period}{schedule.ReserveTime}，更新預約日期時段，不便之處，敬請見諒。"
                };
                SmsHelper.SendSms(req);
            }

            dutyRecord.Date = DateTime.Parse(ChangeDate);
            dutyRecord.Time = Time;
            dutyRecord.Status = (int)DutyRecordStatus.Changed;
            // 更新預約紀錄為變更日期
            List<RecordUpdate> recordUpdates = dutyRecord.UpdateDetail;
            if (recordUpdates == null)
            {
                recordUpdates = new List<RecordUpdate>();
            }
            recordUpdates.Add(new RecordUpdate
            {
                UpdatedAt = DateTime.Now,
                UpdatedLoginID = SessionHelper.Get_LoginID(),
                UpdatedUser = SessionHelper.Get_UserName(),
                Reason = $"變更預約日期至{Utility_DateTime.ToFormat_inTaiwanYear(DateTime.Parse(ChangeDate), "yyy/MM/dd")}"
            });
            dutyRecord.UpdateDetail = recordUpdates;
            result = Service_doctor_DutyRecord.Update(dutyRecord);

            return Json(result);
        }

        /// <summary>
        /// 前台新增案件
        /// </summary>
        /// <param name="Date">預約日期</param>
        /// <param name="Time">預約時段</param>
        /// <returns></returns>
        public ActionResult FrontAdd(string Date, string Time)
        {
            InfoListViewModel model = new InfoListViewModel();
            model.Schedule.Origin = "Member";
            model.Schedule.ReserveDatetimeStr = Date;
            List<string> ReserveTimeList = new List<string>();
            ReserveTimeList.Add(Time);
            model.ReserveTimeList = ReserveTimeList;
            model.MapApiKey = ConfigurationManager.AppSettings["MapApiKey"];

            return View("TicketInfo", model);
        }

        /// <summary>
        /// 後台新增案件
        /// </summary>
        /// <returns></returns>
        public ActionResult BackAdd(string cID = "", bool revisit = false)
        {
            InfoListViewModel model = new InfoListViewModel();
            model.Schedule.Origin = "PlantDoctor";
            var am = Service_defCode.GetList("PlantDoctorAm");
            var pm = Service_defCode.GetList("PlantDoctorPm");
            var ReserveTimeList = new List<string>();
            foreach (defCode d in am)
            {
                ReserveTimeList.Add(d.Code);
            }
            foreach (defCode d in pm)
            {
                ReserveTimeList.Add(d.Code);
            }
            model.ReserveTimeList = ReserveTimeList;
            model.MapApiKey = ConfigurationManager.AppSettings["MapApiKey"];

            var allDoctors = Service_sysUserInfo.GetEnableDoctorList();
            var getUserId = SessionHelper.Get("LoginID");
            if (!string.IsNullOrEmpty(getUserId) && allDoctors.Any(x => x.LoginID == getUserId && x.RoleID == "R08"))
            {
                allDoctors = allDoctors.Where(x => x.LoginID == getUserId).ToList();

                var thisDoctor = allDoctors.Where(x => x.LoginID == getUserId).FirstOrDefault();
                var selectDistrict = (thisDoctor.District ?? "").Split(',').ToList();
                model.Districts = model.Districts.Where(x => selectDistrict.Contains(x.Zip)).ToList();
            }
            else if (!string.IsNullOrEmpty(model.Schedule.OrgDistrict)) allDoctors = allDoctors.Where(x => x.RoleID != "R08" || (x.District ?? "").Contains(model.Schedule.OrgDistrict)).ToList();
            else allDoctors = allDoctors.Where(x => x.RoleID == "R05").ToList();

            model.DoctorList = allDoctors;

            if (!string.IsNullOrEmpty(cID))
            {
                doctorSchedule schedule = Service_doctorSchedule.GetDetail(Convert.ToInt64(cID));
                if (schedule != null)
                {
                    var copySchedule = new doctorSchedule
                    {
                        Origin = "PlantDoctor",
                        OrgType = schedule.OrgType,
                        OrgDistrict = schedule.OrgDistrict,
                        OtherUnit = schedule.OtherUnit,
                        OrgName = schedule.OrgName,
                        LoginID = schedule.LoginID,
                        Inquiry = schedule.Inquiry,
                        UnqualifiedUnit = schedule.UnqualifiedUnit,
                        Name = schedule.Name,
                        MemberInfoID = schedule.MemberInfoID,
                        Sexy = schedule.Sexy,
                        District = schedule.District,
                        Zip = schedule.Zip,
                        CropType = schedule.CropType,
                        CropName = schedule.CropName,
                        Mobile = schedule.Mobile,
                        PlantingArea = schedule.PlantingArea,
                        FarmingMethod = schedule.FarmingMethod,
                        ContactMethod = schedule.ContactMethod,
                        OnsetLocation = schedule.OnsetLocation,
                        ParentID = (revisit && !cID.IsEmpty()) ? long.Parse(cID) : (long?) null,
                    };
                    model.Schedule = copySchedule;
                }
            }

            return View("TicketInfo", model);
        }
    }
}