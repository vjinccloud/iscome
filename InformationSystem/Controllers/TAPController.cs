using ICCModule;
using ICCModule.ActionFilters;
using ICCModule.Entity.Tables.Platform;
using ICCModule.EntityService.Service;
using ICCModule.Helper;
using ICCModule.Models.Map.Google;
using ICCModule.Models.Map.TGOS;
using ICCModule.ViewModel;
using InformationSystem.ViewModel.TAP;
using IscomG2C.Utility;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace InformationSystem.Controllers
{
    [LoginCheckFilter]//掛載此Filter，如果沒登入，就導去登入頁
    [InterceptorOfController]//系統操作Log
    public class TAPController : Controller
    {
        //產銷履歷產品-列表
        public ActionResult ProductList(resumeProductQueryModel param)
        {
            Expression<Func<resumeProduct, bool>> filter = null;
            if (!string.IsNullOrEmpty(param.KeyWord)) filter = x => x.VendorName.Contains(param.KeyWord) || x.OrganizationCode.Contains(param.KeyWord);

            var data = Service_resumeProduct.GetList(filter, new resumeProductReq
            {
                TagResult = param.TagResult,
                QuilityResult = param.QuilityResult,
                CheckStartDate = param.CheckStartDate,
                CheckEndDate = param.CheckEndDate,
                CheckData = param.CheckData,
            });
            param.resumeProductModels = data.OrderByDescending(x => x.LastCheckDate).ThenByDescending(x => x.CreatedAt).Skip((param.Pages - 1) * 20).Take(20).ToList();
            param.TotalPage = (int)Math.Ceiling((decimal)data.Count() / 20);

            return View(param);
        }


        //農藥販售業者資料
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult ProductList(resumeProductQueryModel param, HttpPostedFileBase importFile, string act = "")
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
                    var needRow = new List<string>() { "作物類別", "驗證品項", "農業品經營業者名稱", "農業品經營業者地址", "經營業者代碼", "農民姓名", "地段", "地號", "驗證面積", "驗證機構", "驗證到期日", "公開電話" };
                    foreach (DataColumn dtc in dt.Columns)
                    {
                        if (needRow.Contains(dtc.ColumnName)) needRow.Remove(dtc.ColumnName);
                    }
                    if (needRow.Count > 0) errMsg = $"缺少必要欄位:{string.Join(",", needRow)}";
                    #endregion
                    if (string.IsNullOrEmpty(errMsg) && dt.Rows.Count > 0)
                    {
                        for (int i = 0; i < dt.Rows.Count; i++)
                        {
                            var row = dt.Rows[i];
                            if (string.IsNullOrEmpty(row["地號"].ToString()) || string.IsNullOrEmpty(row["地段"].ToString()))
                            {
                                errMsg = "地段、地號為必填欄位";
                            }
                            var _date = Extention.MinToDateTime(row["驗證到期日"].ToString());
                            if (!_date.HasValue) errMsg = "驗證到期日格式不正確(可能是格式錯誤或有多餘欄位未清除)";
                        }
                        if (string.IsNullOrEmpty(errMsg))
                        {
                            var importData = new List<resumeProduct>();
                            for (int i = 0; i < dt.Rows.Count; i++)
                            {
                                var row = dt.Rows[i];

                                float _area = 0;
                                float? _areas = null;
                                if (float.TryParse(row["驗證面積"].ToString(), out _area)) _areas = _area;
                                var _date = Extention.MinToDateTime(row["驗證到期日"].ToString());
                                importData.Add(new resumeProduct()
                                {
                                    OrganizationCode = row["經營業者代碼"].ToString(),
                                    VendorName = row["農業品經營業者名稱"].ToString(),
                                    ProducerName = row["農民姓名"].ToString(),
                                    Address = row["農業品經營業者地址"].ToString(),
                                    Phone = row["公開電話"].ToString(),
                                    VerificationAgency = row["驗證機構"].ToString(),
                                    VerificationTypes = row["作物類別"].ToString(),
                                    VerificationItems = row["驗證品項"].ToString(),
                                    VerificationArea = _areas,
                                    Lot = row["地段"].ToString(),
                                    LandNum = row["地號"].ToString(),
                                    ExpirationDate = _date,
                                    Principal = "",
                                    ContactPerson = ""
                                });
                            }
                            errMsg = Service_resumeProduct.Import(importData).Msg;
                        }
                    }
                }
            }
            #endregion
            Expression<Func<resumeProduct, bool>> filter = null;
            if (!string.IsNullOrEmpty(param.KeyWord)) filter = x => x.VendorName.Contains(param.KeyWord) || x.OrganizationCode.Contains(param.KeyWord);

            var otherParam = new resumeProductReq
            {
                TagResult = param.TagResult,
                QuilityResult = param.QuilityResult,
                CheckStartDate = param.CheckStartDate,
                CheckEndDate = param.CheckEndDate,
                CheckData = param.CheckData,
            };
            var data = Service_resumeProduct.GetList(filter, otherParam);
            param.resumeProductModels = data.OrderByDescending(x => x.LastCheckDate).ThenByDescending(x => x.CreatedAt).Skip((param.Pages - 1) * 20).Take(20).ToList();
            param.TotalPage = (int)Math.Ceiling((decimal)data.Count() / 20);

            #region 匯出
            if (act == "export")
            {
                var exportData = Service_resumeProduct.GetExportList(filter, otherParam);
                var _export = exportData.Select(x => new
                {
                    組織代碼 = x.OrganizationCode,
                    業者名稱 = x.VendorName,
                    生產者名稱 = x.ProducerName,
                    驗證品項 = x.VerificationItems,
                    驗證面積 = !x.VerificationArea.HasValue ? "" : x.VerificationArea.Value.ToString(),
                    地段 = x.Lot,
                    地號 = x.LandNum,
                    驗證到期日 = !x.ExpirationDate.HasValue ? "" : x.ExpirationDate.Value.ToString("yyyy-MM-dd"),
                    檢查日期 = !x.Date.HasValue ? "" : x.Date.Value.ToString("yyyy-MM-dd"),
                    標示檢查結果 = !x.TagResult.HasValue ? "" : ((x.TagResult ?? true) ? "合格" : "不合格"),
                    品質檢查結果 = !x.QuilityResult.HasValue ? "" : ((x.QuilityResult ?? true) ? "合格" : "不合格"),
                    裁處說明 = x.ArbitrationInstructions
                }).OrderBy(x => x.業者名稱).ThenBy(x => x.檢查日期).ToList();

                return File(ExcelHelper.RenderDataTableToExcel(ExcelHelper.ConvertToDataTable(_export)), "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", $"產銷履歷檢查紀錄_{DateTime.Now.ToString("yyyyMMddHHmmss")}.xlsx");
            }
            #endregion

            if (!string.IsNullOrEmpty(errMsg))
            {
                TempData["TempMsg"] = errMsg;
                TempData["TempResult"] = "error";
            }

            return View(param);
        }

        //產銷履歷產品-內容
        public ActionResult ProductInfo(int id=0)
        {
            var _res = new resumeProductEditModel();
            var _data = Service_resumeProduct.GetDetail(id);
            if (_data != null)
            {
                _res = new resumeProductEditModel
                {
                    ID = _data.ID,
                    OrganizationCode = _data.OrganizationCode,
                    VendorName = _data.VendorName,
                    ProducerName = _data.ProducerName,
                    Principal = _data.Principal,
                    ContactPerson = _data.ContactPerson,
                    Address = _data.Address,
                    Phone = _data.Phone,
                    VerificationAgency = _data.VerificationAgency,
                    VerificationTypes = _data.VerificationTypes,
                    VerificationItems = _data.VerificationItems,
                    VerificationArea = _data.VerificationArea,
                    Lot = _data.Lot,
                    LandNum = _data.LandNum,
                    ExpirationDate = _data.ExpirationDate,
                    ActionName = ""
                };
            }
            return View(_res);
        }
        //產銷履歷產品-內容
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult ProductInfo(resumeProductEditModel req)
        {
            var data = new resumeProduct
            {
                ID = req.ID,
                OrganizationCode = req.OrganizationCode,
                VendorName = req.VendorName,
                ProducerName = req.ProducerName,
                Principal = req.Principal,
                ContactPerson = req.ContactPerson,
                Address = req.Address,
                Phone = req.Phone,
                VerificationAgency = req.VerificationAgency,
                VerificationTypes = req.VerificationTypes,
                VerificationItems = req.VerificationItems,
                VerificationArea = req.VerificationArea,
                Lot = req.Lot,
                LandNum = req.LandNum,
                ExpirationDate = req.ExpirationDate,
            };
            if (Service_resumeProduct.GetList(x => x.Lot == req.Lot && x.LandNum == req.LandNum && x.ID != req.ID).Any())
            {
                TempData["TempMsg"] = "此地段地號已存在";
                TempData["TempResult"] = "error";
                return View(req);
            }
            if (Service_resumeProduct.GetDetail(req.ID) != null && req.ID > 0)
            {
                Service_resumeProduct.Update(data);
            }
            else
            {
                Service_resumeProduct.Insert(data);
            }
            if (req.ActionName == "Add") return RedirectToAction(nameof(AuditorList), new { pid = data.ID });
            return RedirectToAction(nameof(ProductList));
        }

        //產銷履歷產品分布地圖
        public ActionResult Map()
        {
            MapViewModel model = new MapViewModel();
            return View(model);
        }

        /// <summary>
        /// 取回全部販售業者，交由前台進行過濾
        /// </summary>
        /// <returns></returns>
        public ActionResult ResumeProductList()
        {
            List<resumeProductModel> dataList = Service_resumeProduct.GetList(x => true).ToList().Where(x => !string.IsNullOrEmpty(x.Location)).GroupBy(x => new { x.Location,x.VendorName,x.OrganizationCode}).Select(x => new resumeProductModel
            {
                OrganizationCode=x.Key.OrganizationCode,
                VendorName =x.Key.VendorName,
                ProducerName =x.FirstOrDefault().ProducerName,
                Address = x.FirstOrDefault().Address,
                Location = x.Key.Location,
                VerificationAgency = x.FirstOrDefault().VerificationAgency,
                VerificationItems = x.FirstOrDefault().VerificationItems,
                CreatedAt = x.FirstOrDefault().CreatedAt,
                LastCheckDate = x.FirstOrDefault().LastCheckDate,
                TagResult = x.FirstOrDefault().TagResult,
                QuilityResult = x.FirstOrDefault().QuilityResult,
            }).ToList();
            return Json(dataList, JsonRequestBehavior.AllowGet);
        }

        //履歷產品檢查資料-列表
        public ActionResult AuditorList(int pid = 0)
        {
            var _res = new resumeProductCheckListModel();
            var data = Service_resumeProduct.GetDetail(pid);
            if (data != null)
            {
                _res.ProductId = pid;
                _res.OrganizationCode = data.OrganizationCode;
                _res.VendorName = data.VendorName;
                _res.resumeProductChecks = Service_resumeProductCheck.GetList(x => x.ProductID == pid);
            }
            else return RedirectToAction(nameof(ProductList));
            return View(_res);
        }
        [HttpPost] //刪除方法為POST
        public ActionResult AuditorDelete(int Id, int pid)
        {
            if (Service_resumeProductCheck.GetDetail(Id) != null)
            {
                Service_resumeProductCheck.Delete(Id);
            }
            return RedirectToAction(nameof(AuditorList), new { pid = pid });
        }
        //履歷產品檢查資料-內容
        public ActionResult AuditorInfo(int pid = 0, int Id = 0)
        {
            var data = Service_resumeProduct.GetDetail(pid);
            if (data != null)
            {
                var _res = new resumeProductCheckEditModel();
                _res.ProductID = pid;
                var _data = Service_resumeProductCheck.GetDetail(Id);
                if (_data != null)
                {
                    _res.ID = _data.ID;
                    _res.ProductID = _data.ProductID;
                    _res.Date = _data.Date;
                    _res.TagResult = _data.TagResult;
                    _res.QuilityResult = _data.QuilityResult;
                    _res.ArbitrationInstructions = _data.ArbitrationInstructions;
                    _res.OldFiles = Service_FileManagement.GetList("resumeProductCheck", _data.ID.ToString());
                }
                return View(_res);
            }
            else return RedirectToAction(nameof(ProductList));
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        //農藥販售業者資料-檢查紀錄列表-新增
        public ActionResult AuditorInfo(resumeProductCheckEditModel req, List<HttpPostedFileBase> NewFiles)
        {
            var data = new resumeProductCheck()
            {
                ID = req.ID,
                ProductID = req.ProductID,
                Date = req.Date,
                TagResult = req.TagResult,
                QuilityResult = req.QuilityResult,
                ArbitrationInstructions = req.ArbitrationInstructions,
            };
            if (Service_resumeProductCheck.GetDetail(req.ID) != null)
            {
                Service_resumeProductCheck.Update(data);
            }
            else
            {
                Service_resumeProductCheck.Insert(data);
            }

            var getProduct = Service_resumeProduct.GetDetail(data.ProductID);
            if (getProduct != null && (getProduct.LastCheckDate ?? DateTime.MinValue) < data.Date)
            {
                getProduct.LastCheckDate = data.Date;
                Service_resumeProduct.Update(getProduct);
            }

            try
            {
                var _oldFile = Service_FileManagement.GetList("resumeProductCheck", data.ID.ToString());
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
                    string baseDirectory = "/UploadedFiles/ResumeProduct";
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
                            TableName = "resumeProductCheck",
                            TableID = data.ID.ToString(),
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
            return RedirectToAction(nameof(AuditorList), new { pid = req.ProductID });

        }

        /// <summary>
        /// 更新履歷業者地址經緯度
        /// </summary>
        /// <returns></returns>
        public async Task<ActionResult> SyncLocation()
        {
            BaseResult result = new BaseResult();
            if (Session["TAPSyncLocation"] != null && Session["TAPSyncLocation"].ToString() == "sync")
            {
                result.result = true;
                result.Msg = "Job syncing";
                return Json(result, JsonRequestBehavior.AllowGet);
            }
            Session["TAPSyncLocation"] = "sync";

            try
            {
                List<resumeProductModel> lackLocationVendors = Service_resumeProduct.GetList(x => true && x.Location == null).ToList();
                // 轉成 Dictionary        
                Dictionary<string, string> AddressList = new Dictionary<string, string>();
                foreach (var s in lackLocationVendors)
                {
                    AddressList.Add(s.ID.ToString(), s.Address);
                }
                List<string> updateFailItems = new List<string>();
                string Which_MAP = AppSettingHelper.GetAppSetting("Which_MAP");
                switch (Which_MAP)
                {
                    case "Google":                        
                        // 平行批次請求
                        var RequestResult = await new GoogleMapGeocodeHelper().GeocodeAddressInParallel(AddressList);
                        
                        foreach (var keyPair in RequestResult)
                        {
                            // 確認取得經緯度後，寫回原資料
                            GeocodeApi geocodeApi = keyPair.Value;
                            if (geocodeApi.status == "OK" && geocodeApi.results.Count > 0)
                            {
                                Location location = geocodeApi.results[0].geometry.location;
                                BaseResult baseResult = Service_resumeProduct.UpdateLocation(keyPair.Key, $"{location.lat},{location.lng}");
                                if (!baseResult.result)
                                {
                                    updateFailItems.Add(keyPair.Key);
                                }
                            }
                        }
                        if (updateFailItems.Count > 0)
                        {
                            result.Msg = "未正確更新經緯度計有:" + String.Join(",", updateFailItems.ToArray());
                        }
                        result.result = true;
                        break;
                    case "TGOS":
                        // 平行批次請求
                        foreach (var oriAddr in lackLocationVendors.Where(x => !string.IsNullOrEmpty(x.Address)).Select(x => x.Address).Distinct().ToList())
                        {
                            try
                            {
                                var addr = oriAddr;
                                if (Extention.ToInt32(addr.Split('-')[0]) > 0) addr = addr.Substring(addr.Split('-')[0].Length + 1);
                                var TGOSRequestResult = new TGOSHelper().QueryAddressNa(addr);
                                // 確認取得經緯度後，寫回原資料
                                if (!string.IsNullOrEmpty(TGOSRequestResult))
                                {
                                    BaseResult baseResult = Service_resumeProduct.UpdateLocationAddr(oriAddr, TGOSRequestResult);
                                }
                            }
                            catch (Exception e)
                            {

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
            }
            catch (Exception e)
            {
                ErrorLog.SaveErrorLog(e.Message);
            }

            if (String.IsNullOrEmpty(result.Msg))
            {
                result.Msg = "更新成功";
            }

            Session["TAPSyncLocation"] = "over";
            return Json(result, JsonRequestBehavior.AllowGet);
        }
    }
}