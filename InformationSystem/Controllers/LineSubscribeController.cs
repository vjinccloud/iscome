using ICCModule;
using ICCModule.ActionFilters;
using ICCModule.Entity.Tables.Platform;
using ICCModule.EntityService.Service;
using ICCModule.Helper;
using ICCModule.ViewModel;
using InformationSystem.ViewModel;
using IscomG2C.Utility;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Drawing;
using System.IO;
using System.Windows.Interop;

namespace InformationSystem.Controllers
{
    [LoginCheckFilter]//掛載此Filter，如果沒登入，就導去登入頁
    [InterceptorOfController]//系統操作Log
    public class LineSubscribeController : Controller
    {
        /// <summary>
        /// 推播統計
        /// </summary>
        /// <param name="req"></param>
        /// <returns></returns>
        public ActionResult SubscribeCount(LineBroadcastQueryModel req)
        {
            if (req.Page < 1) req.Page = 1;
            var allData = Service_LineBroadcast.GetList(x => (!req.StartDate.HasValue || req.StartDate <= x.CreateDate) && (!req.EndDate.HasValue || req.EndDate >= x.CreateDate.Date) && (string.IsNullOrEmpty(req.MessageType) || x.MessageType == req.MessageType));
            var data = from lb in allData.OrderByDescending(x => x.CreateDate).Skip((req.Page - 1) * 20).Take(20).ToList()
                       join type in Service_defCode.GetList("LineBroadcast")
                       on lb.MessageType equals type.Code into temp
                       from type in temp.DefaultIfEmpty()
                       select new LineBroadcastShowModel
                       {
                           MessageType = type?.Name,
                           MessageTarget = lb.MessageTarget == "all" ? "全部" : "特定對象",
                           TargetUserCount = 0,
                           Contents = lb.Contents,
                           CreateDate = lb.CreateDate,
                       };

            var sData = Service_LineBroadcast.GetList(x => true).GroupBy(x => new { x.CreateDate.Year, x.CreateDate.Month }).Select(x => new LineBroadcastStasticModel
            {
                Date = new DateTime(x.Key.Year, x.Key.Month, 1),
                Count = x.Count(),
            });

            var response = new LineBroadcastModel()
            {
                QueryReq = req,
                Data = data.ToList(),
                StasticData = sData.ToList(),
                TotalPage = (int)Math.Ceiling((decimal)allData.Count() / 20)
            };
            return View(response);
        }
        /// <summary>
        /// 新增推播
        /// </summary>
        /// <returns></returns>
        public ActionResult AddBroadcast()
        {
            var res = new AddBroadcastModel();
            return View(res);
        }
        /// <summary>
        /// 新增推播
        /// </summary>
        /// <param name="req"></param>
        /// <returns></returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult AddBroadcast(LineBroadcast req)
        {
            var res = new AddBroadcastModel()
            {
                Data = req
            };
            try
            {
                if (req.MessageTarget == "assign")
                {
                    var targets = Service_MemberInfo.GetList(x => x.LineMessageId != null).Where(x => !string.IsNullOrEmpty(x.LineMessageId.Trim()) && req.TargetUserList.Select(o => Extention.ToInt32(o)).Contains((int)x.ID)).Select(x => x.LineMessageId).ToList();
                    Service_LineBroadcast.SendMessage(new LinePushMessageModel()
                    {
                        to = targets,
                        messages = new List<SendMessage>() { new SendMessage() { type = "text", text = req.Contents }, },
                    });
                    req.TargetUser = string.Join(",", req.TargetUserList);
                }
                else
                {
                    var msg = new List<SendMessage>();
                    msg.Add(new SendMessage() { type = "text", text = req.Contents });
                    Service_LineBroadcast.SendBroadcast(msg);
                }
                var add = Service_LineBroadcast.Insert(req);
            }
            catch (Exception ex)
            {
                ErrorLog.SaveErrorLog(ex, "LineError");
                TempData["TempMsg"] = ex.Message;
                TempData["TempResult"] = "error";
                return View(res);
            }

            return RedirectToAction(nameof(SubscribeCount));
        }

        /// <summary>
        /// 二十四節氣列表
        /// </summary>
        /// <param name="req"></param>
        /// <returns></returns>
        public ActionResult SolarTermsList(SolarTermsQueryModel req)
        {
            if (req.Page < 1) req.Page = 1;
            var allData = Service_SolarTermsPush.GetList(x => true);
            var allYearList = allData.Where(x => x.ReleaseDate.HasValue).Select(x => x.ReleaseDate.Value.Year).Distinct().ToList();

            if (!string.IsNullOrEmpty(req.SolarTermsCode))
            {
                allData = allData.Where(x => x.SolarTermsCode == req.SolarTermsCode).ToList();
            }

            if (req.Years.HasValue)
            {
                allData = allData.Where(x => x.ReleaseDate.Value.Year == req.Years).ToList();
            }

            if (!string.IsNullOrEmpty(req.KeyWord))
            {
                allData = allData.Where(x => x.CropName.Contains(req.KeyWord) || x.PestDisease.Contains(req.KeyWord) || x.PushContents.Contains(req.KeyWord)).ToList();
            }

            var response = new SolarTermsListModel()
            {
                QueryReq = req,
                Data = allData.ToList(),
                YearData = allYearList,
                TotalPage = (int)Math.Ceiling((decimal)allData.Count() / 20)
            };
            return View(response);
        }
        /// <summary>
        /// 二十四節氣編輯
        /// </summary>
        /// <param name="Id"></param>
        /// <returns></returns>
        public ActionResult SolarTermsEdit(int Id = 0)
        {
            var _res = new SolarTermsEditView();
            var solarTermsData = Service_SolarTermsPush.GetDetail(Id);

            if (solarTermsData != null)
            {
                _res.Data = new SolarTermsEditModel()
                {
                    Id = solarTermsData.Id,
                    SolarTermsCode = solarTermsData.SolarTermsCode,
                    SolarTermsName = solarTermsData.SolarTermsName,
                    PushDate = solarTermsData.PushDate,
                    IsNeedPush = solarTermsData.IsNeedPush,

                    CropName = solarTermsData.CropName,
                    PestDisease = solarTermsData.PestDisease,
                    DataSubject = solarTermsData.DataSubject,
                    ReleaseDate = solarTermsData.ReleaseDate,
                    DataContents = solarTermsData.DataContents,

                    IsPushed = solarTermsData.IsPushed,
                    IsImport = solarTermsData.IsImport,
                };

                if (!string.IsNullOrEmpty(solarTermsData.PushContents))
                {
                    try
                    {
                        _res.Data.PushContents = JsonConvert.DeserializeObject<List<PushContentTab>>(solarTermsData.PushContents);
                        if(_res.Data.PushContents == null) _res.Data.PushContents = new List<PushContentTab>();
                    }
                    catch (Exception e)
                    {

                    }
                }
            }

            return View(_res);
        }
        /// <summary>
        /// 二十四節氣編輯
        /// </summary>
        /// <param name="Id"></param>
        /// <returns></returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult SolarTermsEdit(SolarTermsEditModel req)
        {
            var _res = new SolarTermsEditView()
            {
                Data = req,
            };

            var pushNow = req.PushDate == DateTime.Today.Date && DateTime.Now.Hour > 6 && !req.IsNeedPush;
            var getStName = Service_defCode.GetDetail("SolarTerms", req.SolarTermsCode);
            req.SolarTermsName = getStName?.Name;
            var sId = 0;
            var oldSolarTerms = Service_SolarTermsPush.GetDetail(req.Id);
            if (oldSolarTerms != null)
            {
                oldSolarTerms.SolarTermsCode = req.SolarTermsCode;
                oldSolarTerms.SolarTermsName = req.SolarTermsName;
                oldSolarTerms.PushDate = req.PushDate.Value;
                oldSolarTerms.IsNeedPush = !req.IsNeedPush;
                oldSolarTerms.CropName = req.CropName;
                oldSolarTerms.PestDisease = req.PestDisease;
                oldSolarTerms.PushContents = JsonConvert.SerializeObject(req.PushContents);
                oldSolarTerms.DataSubject = req.DataSubject;
                oldSolarTerms.ReleaseDate = req.ReleaseDate;
                oldSolarTerms.DataContents = req.DataContents;
                oldSolarTerms.IsPushed = pushNow;
                Service_SolarTermsPush.Update(oldSolarTerms);

                sId = oldSolarTerms.Id;
            }
            else
            {
                var newSolarTerm = new SolarTermsPush();
                newSolarTerm.SolarTermsCode = req.SolarTermsCode;
                newSolarTerm.SolarTermsName = req.SolarTermsName;
                newSolarTerm.PushDate = req.PushDate.Value;
                newSolarTerm.IsNeedPush = !req.IsNeedPush;
                newSolarTerm.CropName = req.CropName;
                newSolarTerm.PestDisease = req.PestDisease;
                newSolarTerm.PushContents = JsonConvert.SerializeObject(req.PushContents);
                newSolarTerm.DataSubject = req.DataSubject;
                newSolarTerm.ReleaseDate = req.ReleaseDate;
                newSolarTerm.DataContents = req.DataContents;
                newSolarTerm.IsPushed = pushNow;
                newSolarTerm.IsImport = false;

                Service_SolarTermsPush.Insert(newSolarTerm);
                sId = newSolarTerm.Id;
            }

            foreach (var item in req.PushContents)
            {
                if (!string.IsNullOrEmpty(item.picId))
                {
                    var getFile = Service_FileManagement.GetDetail(Extention.ToInt32(item.picId));
                    if (getFile!=null)
                    {
                        getFile.TableID = sId.ToString();
                        Service_FileManagement.UpdateId(getFile);
                    }
                }
            }

            var msg = new List<SendMessage>();
            try
            {
                if (pushNow)
                {
                    foreach (var item in req.PushContents)
                    {
                        if (item.tabId == 1) msg.Add(new SendMessage() { type = "text", text = item.content });
                        else if (item.tabId == 2) msg.Add(new SendMessage() { type = "image", originalContentUrl = $"{AppSettingHelper.GetAppSetting("Backend_HostName")}{item.path}", previewImageUrl = $"{AppSettingHelper.GetAppSetting("Backend_HostName")}{item.path}" });
                    }
                    Service_LineBroadcast.SendBroadcast(msg);
                    Service_SolarTermsPush.UpdatePushed(sId);
                }
            }
            catch (Exception e)
            {
                ErrorLog.SaveErrorLog(e + JsonConvert.SerializeObject(msg), "LineBroadCast");
            }

            return RedirectToAction(nameof(SolarTermsList));
        }
        [HttpPost]
        public ActionResult UploadPic()
        {
            var file = Request.Files[0];
            var _res = new object();
            var errMsg = "";
            var url = "";
            #region 匯入
            if (file != null)
            {
                try
                {
                    string baseDirectory = $"/UploadedFiles/LineSolarTerm";
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
                        string fixSaveName = $"{DateTime.Now.ToString("yyyyMMddHHmmss")}_" + Path.GetExtension(file.FileName);

                        //釋放掉圖檔         
                        image.Dispose();

                        string _path = Path.Combine(_directory, fixSaveName);
                        //將修改過的圖存於設定的位置
                        imageOutput.Save(_path);

                        // 釋放記憶體 (這行若寫在 imageOutput.Save() 之前，會造成修改結果無法存回原始圖檔，只能另存成一個新的圖檔)。
                        // 若要將修改結果，存成另一個新圖檔，就將此行移至 imageOutput.Save() 之前，並指派一個不同檔名給 fixSaveName 變數
                        imageOutput.Dispose();
                        #endregion
                        url = String.Concat(baseDirectory, '/', fixSaveName);

                        var newFile = new FileManagement()
                        {
                            TableName = "SolarTermsPush",
                            TableID = "",
                            FileName = fixSaveName,
                            FilePath = url,
                            FileLength = file.ContentLength,
                            FileType = file.ContentType,
                            MD5 = FileLogic.CalculateMD5(file.InputStream),
                        };
                        Service_FileManagement.Insert(newFile);

                        _res = new
                        {
                            result = true,
                            Msg = "成功",
                            Data = new
                            {
                                FilePath = url,
                                FileId = newFile.ID
                            }
                        };
                    }
                }
                catch (Exception e)
                {
                    _res = new
                    {
                        result = false,
                        Msg = e.Message
                    };
                }
            }
            #endregion
            else
            {
                _res = new
                {
                    result = false,
                    Msg = "無檔案",
                };
            }
            return Json(_res);
        }
        // 二十四節氣-匯入舊資料
        public ActionResult ImportSolarTerm()
        {
            return View(new BaseResult());
        }
        // 二十四節氣-匯入舊資料
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult ImportSolarTerm(HttpPostedFileBase importFile)
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
                    var needRow = new List<string>() { "發布日期", "主旨", "內容", "作物", "病蟲害", "節氣", "推播預警內容" };
                    foreach (DataColumn dtc in dt.Columns)
                    {
                        if (needRow.Contains(dtc.ColumnName)) needRow.Remove(dtc.ColumnName);
                    }
                    if (needRow.Count > 0) errMsg = $"缺少必要欄位:{string.Join(",", needRow)}";
                    #endregion
                    if (string.IsNullOrEmpty(errMsg) && dt.Rows.Count > 0)
                    {
                        var errorSolarTermList = new List<string>();
                        var solarTermsType = Service_defCode.GetList("SolarTerms");

                        for (int i = 0; i < dt.Rows.Count; i++)
                        {
                            var row = dt.Rows[i];
                            DateTime _date = DateTime.Now;
                            if (!DateTime.TryParse(row["發布日期"].ToString(), out _date)) errMsg = "發布日期格式不正確(請確認格式是否為文字)";
                            if (!solarTermsType.Any(x => x.Name == row["節氣"].ToString())) errorSolarTermList.Add(row["節氣"].ToString());
                        }
                        if (errorSolarTermList.Any())
                        {
                            if (!string.IsNullOrEmpty(errMsg)) errMsg += "\n對應不到的節氣：" + string.Join(",", errorSolarTermList);
                            else errMsg += "對應不到的節氣：" + string.Join(",", errorSolarTermList);
                        }
                        if (!string.IsNullOrEmpty(errMsg))
                        {
                            TempData["TempMsg"] = errMsg;
                            TempData["TempResult"] = "error";
                        }
                        if (string.IsNullOrEmpty(errMsg))
                        {
                            var importData = new List<SolarTermsPush>();
                            for (int i = 0; i < dt.Rows.Count; i++)
                            {
                                var row = dt.Rows[i];

                                var thisSolarTermsType = solarTermsType.FirstOrDefault(x => x.Name == row["節氣"].ToString());
                                var pushContents = new List<PushContentTab>()
                                {
                                    new PushContentTab()
                                    {
                                        id = 1,
                                        tabId= 1,
                                        content = row["推播預警內容"].ToString(),
                                        path = "",
                                        picId = ""
                                    }
                                };
                                var newPRT = new SolarTermsPush()
                                {
                                    SolarTermsCode = thisSolarTermsType.Code,
                                    SolarTermsName = thisSolarTermsType.Name,
                                    PushDate = DateTime.Today,
                                    IsNeedPush = false,
                                    CropName = row["作物"].ToString(),
                                    PestDisease = row["病蟲害"].ToString(),
                                    PushContents = JsonConvert.SerializeObject(pushContents),
                                    DataSubject = row["主旨"].ToString(),
                                    ReleaseDate = row["發布日期"].ToString().ToDateTime(),
                                    DataContents = row["內容"].ToString(),
                                    IsPushed = false,
                                    IsImport = true,
                                    CreateDate = DateTime.Now,
                                    UpdateDate = DateTime.Now,
                                };
                                importData.Add(newPRT);
                            }
                            _res = Service_SolarTermsPush.Import(importData);
                        }
                    }
                }
            }
            #endregion
            return View(_res);
        }
    }
}