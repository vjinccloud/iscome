using ICCModule;
using ICCModule.ActionFilters;
using ICCModule.Entity.Tables.Platform;
using ICCModule.EntityService.Service;
using ICCModule.Helper;
using ICCModule.Models.Map.Google;
using ICCModule.Models.Map.TGOS;
using InformationSystem.ViewModel;
using IscomG2C.Utility;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace InformationSystem.Controllers
{
    [LoginCheckFilter]//掛載此Filter，如果沒登入，就導去登入頁
    [InterceptorOfController]//系統操作Log
    public class PesticideSellerController : Controller
    {
        //農藥販售業者資料
        public ActionResult SellerList(PesticideSellerQueryModel param)
        {

            Expression<Func<pesticideSeller, bool>> filter = x => true;
            if (!string.IsNullOrEmpty(param.District)) filter = filter.And(x => x.VendorAddress.Contains(param.District));
            if (!string.IsNullOrEmpty(param.KeyWord)) filter = filter.And(x => x.VendorName.Contains(param.KeyWord) || x.ContactPhone.Contains(param.KeyWord));
            if (!string.IsNullOrEmpty(param.Status)) filter = filter.And(x => param.Status.Split(',').Select(o => Convert.ToInt32(o)).Contains(x.Status));

            var data = Service_pesticideSeller.GetList(filter, param.CheckResult, param.CheckStartDate, param.CheckEndDate, param.CheckData);
            param.PesticideModels = data.OrderByDescending(x => x.LastCheckDate).ThenByDescending(x => x.UpdatedAt ?? x.CreatedAt).Skip((param.Pages - 1) * 20).Take(20).ToList();
            param.TotalPage = (int)Math.Ceiling((decimal)data.Count() / 20);
            if (!string.IsNullOrEmpty(param.Status)) param.StatusList = param.Status.Split(',').Select(o => Convert.ToInt32(o)).ToList();

            return View(param);
        }

        //農藥販售業者資料
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult SellerList(PesticideSellerQueryReq param, HttpPostedFileBase importFile, string act = "")
        {
            var errMsg = "";
            #region 匯入
            if (importFile != null && act == "upload")
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
                    var needRow = new List<string>() { "執照號碼", "業者名稱", "聯繫電話", "營業地址", "營業狀態" };
                    foreach (DataColumn dtc in dt.Columns)
                    {
                        if (needRow.Contains(dtc.ColumnName)) needRow.Remove(dtc.ColumnName);
                    }
                    if (needRow.Count > 0) errMsg = $"缺少必要欄位:{string.Join(",", needRow)}";
                    #endregion
                    if (string.IsNullOrEmpty(errMsg) && dt.Rows.Count > 0)
                    {
                        var importData = new List<pesticideSeller>();
                        for (int i = 0; i < dt.Rows.Count; i++)
                        {
                            var row = dt.Rows[i];
                            var _status = 1;
                            if (row["營業狀態"].ToString().Contains("營業中")) _status = 1;
                            if (row["營業狀態"].ToString().Contains("停業")) _status = 2;
                            if (row["營業狀態"].ToString().Contains("歇業")) _status = 3;
                            importData.Add(new pesticideSeller()
                            {
                                LicenseNum = row["執照號碼"].ToString(),
                                VendorName = row["業者名稱"].ToString(),
                                ContactPhone = row["聯繫電話"].ToString(),
                                VendorAddress = row["營業地址"].ToString(),
                                Status = _status,
                            });
                        }
                        errMsg = Service_pesticideSeller.Import(importData).Msg;
                    }
                }
            }
            #endregion
            Expression<Func<pesticideSeller, bool>> filter = x => true;
            if (!string.IsNullOrEmpty(param.District)) filter = filter.And(x => x.VendorAddress.Contains(param.District));
            if (!string.IsNullOrEmpty(param.KeyWord)) filter = filter.And(x => x.VendorName.Contains(param.KeyWord) || x.ContactPhone.Contains(param.KeyWord));
            if (param.Status.Any()) filter = filter.And(x => param.Status.Contains(x.Status));

            var _res = new PesticideSellerQueryModel
            {
                District = param.District,
                KeyWord = param.KeyWord,
                Status = string.Join(",", param.Status),
                StatusList = param.Status,
                CheckResult = param.CheckResult,
                CheckStartDate = param.CheckStartDate,
                CheckEndDate = param.CheckEndDate,
                CheckData = param.CheckData,
            };

            var data = Service_pesticideSeller.GetList(filter, param.CheckResult, param.CheckStartDate, param.CheckEndDate, param.CheckData);
            _res.PesticideModels = data.OrderByDescending(x => x.LastCheckDate).ThenByDescending(x => x.CreatedAt).Take(20).ToList();
            _res.TotalPage = (int)Math.Ceiling((decimal)data.Count() / 20);

            #region 匯出
            if (act == "export")
            {
                var exportData = Service_pesticideSeller.GetExportList(filter, param.CheckResult, param.CheckStartDate, param.CheckEndDate, param.CheckData);
                var _export = exportData.Select(x => new
                {
                    執照號碼 = x.LicenseNum,
                    業者名稱 = x.VendorName,
                    營業地址 = x.VendorAddress,
                    營業狀態 = x.Status == 1 ? "營業中" : (x.Status == 2 ? "停業" : "歇業"),
                    檢查日期 = x.Date.ToDateString(),
                    檢查結果說明 = x.Instructions ?? "",
                    檢查結果 = !x.Result.HasValue ? "" : (x.Result == true ? "合格" : ("不合格")),
                }).OrderBy(x => x.業者名稱).ThenBy(x => x.檢查日期).ToList();

                //Response.Clear();
                //Response.ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
                //Response.AddHeader("content-disposition", $"attachment: filename=販售業者檢查紀錄_{DateTime.Now.ToString("yyyyMMddHHmmss")}.xlsx");
                //ExcelHelper.RenderDataTableToExcel(ExcelHelper.ConvertToDataTable(_export)).WriteTo(Response.OutputStream);
                //Response.End();
                return File(ExcelHelper.RenderDataTableToExcel(ExcelHelper.ConvertToDataTable(_export)), "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", $"販售業者檢查紀錄_{DateTime.Now.ToString("yyyyMMddHHmmss")}.xlsx");
            }
            #endregion

            if (!string.IsNullOrEmpty(errMsg))
            {
                TempData["TempMsg"] = errMsg;
                TempData["TempResult"] = "error";
            }

            return View(_res);
        }
        [HttpPost] //刪除方法為POST
        public ActionResult SellerDelete(string Id)
        {
            var _msg = "";
            if (Service_pesticideSeller.GetDetail(Id) != null)
            {
                var chc = Service_pesticideAudit.GetList(x => x.SellerID == Id);
                if (chc.Any())
                {
                    Service_pesticideAudit.DeleteMany(x => x.SellerID == Id);
                }
                _msg = Service_pesticideSeller.Delete(Id).Msg;
            }
            if (!string.IsNullOrEmpty(_msg))
            {

            }

            return RedirectToAction(nameof(SellerList));
        }

        //農藥販售業者資料-新增
        public ActionResult SellorAdd(string ln = "")
        {
            var data = Service_pesticideSeller.GetDetail(ln);
            var _response = new PesticideSellerModel();
            if (data != null)
            {
                _response.LicenseNum = data.LicenseNum;
                _response.VendorName = data.VendorName;
                _response.VendorAddress = data.VendorAddress;
                _response.Status = data.Status;
                _response.ContactPhone = data.ContactPhone;
                _response.LastCheckDate = data.LastCheckDate;
                _response.FriendlyStartDate = data.FriendlyStartDate;
                _response.FriendlyEndDate = data.FriendlyEndDate;
                _response.Location = data.Location;
                _response.ActionName = "Update";
            }
            return View(_response);
        }
        //農藥販售業者資料-新增
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult SellorAdd(PesticideSellerModel pesticideSellers)
        {
            var _sellor = new pesticideSeller()
            {
                LicenseNum = pesticideSellers.LicenseNum,
                VendorName = pesticideSellers.VendorName,
                VendorAddress = pesticideSellers.VendorAddress,
                Location = pesticideSellers.Location,
                Status = pesticideSellers.Status,
                ContactPhone = pesticideSellers.ContactPhone,
                LastCheckDate = pesticideSellers.LastCheckDate,
                FriendlyStartDate = pesticideSellers.FriendlyStartDate,
                FriendlyEndDate = pesticideSellers.FriendlyEndDate,
            };

            var oldData = Service_pesticideSeller.GetDetail(pesticideSellers.LicenseNum);
            if (oldData != null && pesticideSellers.ActionName == "Update")
            {
                Service_pesticideSeller.Update(_sellor);
            }
            else
            {
                if (oldData != null)
                {
                    TempData["TempMsg"] = "此執照號碼已存在";
                    TempData["TempResult"] = "error";
                    return View(pesticideSellers);
                }
                Service_pesticideSeller.Insert(_sellor);
            }

            return RedirectToAction(nameof(SellerList));
        }

        //農藥販售業者資料-檢查紀錄列表
        public ActionResult AuditorList(string ln = "")
        {
            var data = Service_pesticideSeller.GetDetail(ln);
            var _response = new PesticideAuditModel();
            if (data != null)
            {
                _response.LicenseNum = data.LicenseNum;
                _response.VendorName = data.VendorName;
                _response.PesticideAudits = Service_pesticideAudit.GetList(x => x.SellerID == ln);
            }
            //else return RedirectToAction(nameof(SellerList));
            return View(_response);
        }
        [HttpPost] //刪除方法為POST
        public ActionResult AuditorDelete(int Id, string ln)
        {
            if (Service_pesticideAudit.GetDetail(Id) != null)
            {
                Service_pesticideAudit.Delete(Id);
            }
            return RedirectToAction(nameof(AuditorList), new { ln = ln });
        }
        //農藥販售業者資料-檢查紀錄列表-新增
        public ActionResult AuditorList_Add(string ln = "", int Id = 0)
        {
            var data = Service_pesticideSeller.GetDetail(ln);
            if (data != null)
            {
                var _res = new PesticideAuditEditModel();
                _res.SellerID = ln;
                var _data = Service_pesticideAudit.GetDetail(Id);
                if (_data != null)
                {
                    _res.ID = _data.ID;
                    _res.SellerID = _data.SellerID;
                    _res.Date = _data.Date;
                    _res.Result = _data.Result;
                    _res.Instructions = _data.Instructions;
                    _res.OldFiles = Service_FileManagement.GetList("pesticideAudit", _data.ID.ToString());
                }
                return View(_res);
            }
            else return RedirectToAction(nameof(SellerList));

        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        //農藥販售業者資料-檢查紀錄列表-新增
        public ActionResult AuditorList_Add(PesticideAuditEditModel req, List<HttpPostedFileBase> NewFiles)
        {
            var data = new pesticideAudit()
            {
                ID = req.ID,
                SellerID = req.SellerID,
                Date = req.Date,
                Result = req.Result,
                Instructions = req.Instructions,
            };
            if (Service_pesticideAudit.GetDetail(req.ID) != null)
            {
                Service_pesticideAudit.Update(data);
            }
            else
            {
                Service_pesticideAudit.Insert(data);
            }

            var getSellor = Service_pesticideSeller.GetDetail(data.SellerID);
            if (getSellor != null && (getSellor.LastCheckDate ?? DateTime.MinValue) < data.Date)
            {
                getSellor.LastCheckDate = data.Date;
                Service_pesticideSeller.Update(getSellor);
            }

            try
            {
                var _oldFile = Service_FileManagement.GetList("pesticideAudit", data.ID.ToString());
                if (_oldFile.Any())
                {
                    foreach (var item in _oldFile.Where(x => !req.EditOldFiles.Contains(x.ID)))
                    {
                        var thisFile = _oldFile.FirstOrDefault(x => x.ID == item.ID);
                        if (thisFile == null)
                        {
                            continue;
                        }
                        else
                        {
                            string _path = Server.MapPath("~" + thisFile.FilePath);
                            if (System.IO.File.Exists(_path))
                            {
                                System.IO.File.Delete(_path);
                            }
                            Service_FileManagement.Delete((int)thisFile.ID);
                        }
                    }
                }
                if (NewFiles.Count > 0)
                {
                    var _msg = new BaseResult();
                    string baseDirectory = "/UploadedFiles/Pesticide";
                    foreach (var file in NewFiles)
                    {
                        if (file == null)
                        {
                            continue;
                        }
                        string _FileName = Path.GetFileNameWithoutExtension(file.FileName);
                        string _fileExtension = Path.GetExtension(file.FileName);
                        string _directory = Server.MapPath("~" + baseDirectory);
                        IscomG2C.Utility.FileHelper.CreateFolder(_directory);

                        string originFileName = _FileName;
                        string fullFileName = (_FileName + _fileExtension);
                        string _path = Path.Combine(_directory, fullFileName);
                        int i = 0;

                        DirectoryInfo dir = new DirectoryInfo(_directory);
                        FileInfo checkFile = dir.EnumerateFiles().FirstOrDefault(m => m.Name == fullFileName);

                        while (checkFile != null && checkFile.Exists)
                        {
                            _FileName = originFileName + "_" + i;
                            fullFileName = (_FileName + _fileExtension);
                            checkFile = dir.EnumerateFiles().FirstOrDefault(m => m.Name == fullFileName);

                            i++;
                        }
                        file.SaveAs(_path);
                        var url = String.Concat(baseDirectory, '/', fullFileName);
                        _msg = Service_FileManagement.Insert(new FileManagement()
                        {
                            TableName = "pesticideAudit",
                            TableID = req.ID.ToString(),
                            FileName = _FileName,
                            FilePath = url,
                            FileLength = file.ContentLength,
                            FileType = file.ContentType,
                            MD5 = IscomG2C.Utility.FileLogic.CalculateMD5(file.InputStream),
                        });
                    }
                }
            }
            catch (Exception e)
            {
                TempData["TempMsg"] = e.Message;
                TempData["TempResult"] = "error";
                return View(req);
            }
            return RedirectToAction(nameof(AuditorList), new { ln = req.SellerID });

        }
        //農藥販售業者資料-編輯
        public ActionResult AuditorEdit()
        {
            return View();
        }

        //更新農藥販賣業者地址經緯度
        public async Task<ActionResult> SyncLocation()
        {
            BaseResult result = new BaseResult();
            if (Session["SyncLocation"] != null && Session["SyncLocation"].ToString() == "sync")
            {
                result.result = true;
                result.Msg = "Job syncing";
                return Json(result, JsonRequestBehavior.AllowGet);
            }
            Session["SyncLocation"] = "sync";

            try
            {
                List<pesticideSeller> lackLocationSellers = Service_pesticideSeller.GetSellerList(x => true).Where(x => string.IsNullOrEmpty(x.Location) || !x.Latitude.HasValue || !x.Longitude.HasValue).ToList();
                //轉成 Dictionary
                Dictionary<string, string> AddressList = new Dictionary<string, string>();
                foreach (var s in lackLocationSellers)
                {
                    AddressList.Add(s.LicenseNum, s.VendorAddress);
                }
                List<string> updateFailItems = new List<string>();
                string Which_MAP = AppSettingHelper.GetAppSetting("Which_MAP");
                switch (Which_MAP)
                {
                    case "Google":

                        // 平行批次請求
                        var GoogleRequestResult = await new GoogleMapGeocodeHelper().GeocodeAddressInParallel(AddressList);

                        foreach (var keyPair in GoogleRequestResult)
                        {
                            // 確認取得經緯度後，寫回原資料
                            GeocodeApi geocodeApi = keyPair.Value;
                            if (geocodeApi.status == "OK" && geocodeApi.results.Count > 0)
                            {
                                Location location = geocodeApi.results[0].geometry.location;

                                var thisSeller = lackLocationSellers.FirstOrDefault(x => x.LicenseNum == keyPair.Key);
                                thisSeller.Location = $"{location.lat},{location.lng}";
                                thisSeller.Latitude = (decimal)location.lat;
                                thisSeller.Longitude = (decimal)location.lng;

                                BaseResult baseResult = Service_pesticideSeller.UpdateLocation(thisSeller);
                                if (!baseResult.result)
                                {
                                    updateFailItems.Add(keyPair.Key);
                                }
                            }
                        }

                        result.result = true;
                        break;
                    case "TGOS":
                        // 平行批次請求
                        foreach (var pSeller in lackLocationSellers)
                        {
                            var TGOSRequestResult = new TGOSHelper().QueryAddressNa(pSeller.VendorAddress);
                            // 確認取得經緯度後，寫回原資料
                            if (!string.IsNullOrEmpty(TGOSRequestResult) && TGOSRequestResult.Split(',').Count() == 2)
                            {
                                pSeller.Location = TGOSRequestResult;
                                var _coordinate = CoordinateTransHelper.TWD97_To_lonlat((TGOSRequestResult.Split(',')[0]).ToDouble32(), (TGOSRequestResult.Split(',')[1]).ToDouble32(), 2);
                                pSeller.Latitude = _coordinate.Split(',')[1].ToDecimal32();
                                pSeller.Longitude = _coordinate.Split(',')[0].ToDecimal32();

                                BaseResult baseResult = Service_pesticideSeller.UpdateLocation(pSeller);
                            }
                        }
                        result.result = true;
                        break;
                    default:
                        result.result = false;
                        result.Msg = "未設定正確 <Which_MAP> key";
                        Session["TAPSyncLocation"] = "over";
                        return Json(result, JsonRequestBehavior.AllowGet);
                }
                if (updateFailItems.Count > 0)
                {
                    result.Msg = "未正確更新經緯度計有:" + String.Join(",", updateFailItems.ToArray());
                }
            }
            catch (Exception e)
            {
                ErrorLog.SaveErrorLog(e.Message);
            }

            if (String.IsNullOrEmpty(result.Msg))
            {
                result.Msg = "更新成功";
            }

            Session["SyncLocation"] = "over";
            return Json(result, JsonRequestBehavior.AllowGet);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="addr"></param>
        /// <returns></returns>
        public ActionResult GetAddrLocation(string addr)
        {
            BaseResult result = new BaseResult()
            {
                result = false,
            };
            try
            {
                string Which_MAP = AppSettingHelper.GetAppSetting("Which_MAP");
                switch (Which_MAP)
                {
                    case "Google":

                        // 平行批次請求
                        var data = new GoogleMapGeocodeHelper().GeocodeAddressNa(addr);
                        if (data.status == "OK")
                        {
                            var addrPosition = data.results.FirstOrDefault(x => x.geometry.location_type == "ROOFTOP" || x.geometry.location_type == "RANGE_INTERPOLATED");
                            if (addrPosition != null)
                            {
                                result.result = true;
                                result.Msg = $"{addrPosition.geometry.location.lng},{addrPosition.geometry.location.lat}";
                            }
                            else
                            {
                                result.Msg = $"{addr}不精確地址";
                            }
                        }
                        break;
                    case "TGOS":
                        // 平行批次請求
                        result.Msg = new TGOSHelper().QueryAddressNa(addr);
                        result.result = true;
                        break;
                    default:
                        result.result = false;
                        result.Msg = "未設定正確 <Which_MAP> key";

                        return Json(result, JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception e)
            {
                ErrorLog.SaveErrorLog(e.Message);
            }

            return Json(result, JsonRequestBehavior.AllowGet);
        }
    }
}