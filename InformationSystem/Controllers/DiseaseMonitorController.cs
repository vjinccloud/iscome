using ICCModule.EntityService.Service;
using ICCModule.ViewModel;
using ICCModule.Entity.Tables.Platform;
using InformationSystem.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Net.Http;
using System.Net.Http.Headers;
using Newtonsoft.Json.Linq;
using System.Threading.Tasks;
using ICCModule.Helper;
using Newtonsoft.Json;
using System.Configuration;
using ICCModule;
using System.Data;
using System.IO;
using ICCModule.ActionFilters;

namespace InformationSystem.Controllers
{
    [LoginCheckFilter]//掛載此Filter，如果沒登入，就導去登入頁
    [InterceptorOfController]//系統操作Log
    public class DiseaseMonitorController : Controller
    {
        //監測數據上傳
        public ActionResult Upload()
        {
            return View(new BaseResult());
        }
        // 農藥殘留檢驗資料-匯入舊資料
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Upload(HttpPostedFileBase importFile)
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
                    if (!string.IsNullOrEmpty(errMsg))
                    {
                        TempData["TempMsg"] = errMsg;
                        TempData["TempResult"] = "error";
                        return View(new BaseResult());
                    }
                }
                if (string.IsNullOrEmpty(errMsg))
                {
                    #region 驗證欄位 & 整理欄位
                    var missRow = "";
                    var needRow = new List<string>() { "專案名稱", "年度", "調查起始日", "調查結束日", "調查對象類型", "行政區", "編號", "日期" };
                    foreach (DataColumn dtc in dt.Columns)
                    {
                        if (needRow.Contains(dtc.ColumnName)) needRow.Remove(dtc.ColumnName);
                    }
                    if (needRow.Count > 0) errMsg = $"缺少必要欄位:{string.Join(",", needRow)}";

                    if (!string.IsNullOrEmpty(errMsg))
                    {
                        TempData["TempMsg"] = errMsg;
                        TempData["TempResult"] = "error";
                        return View(new BaseResult());
                    }

                    foreach (DataRow dtr in dt.Rows)
                    {
                        if (string.IsNullOrEmpty(dtr["專案名稱"].ToString()) || string.IsNullOrEmpty(dtr["年度"].ToString()) || string.IsNullOrEmpty(dtr["調查起始日"].ToString()) ||
                            string.IsNullOrEmpty(dtr["調查結束日"].ToString()) || string.IsNullOrEmpty(dtr["調查對象類型"].ToString()) || string.IsNullOrEmpty(dtr["行政區"].ToString()) ||
                            string.IsNullOrEmpty(dtr["編號"].ToString()) || string.IsNullOrEmpty(dtr["日期"].ToString()))
                        {
                            errMsg = $"必填項目請勿空白";
                        }
                        if (!string.IsNullOrEmpty(errMsg))
                        {
                            TempData["TempMsg"] = errMsg;
                            TempData["TempResult"] = "error";
                            return View(new BaseResult());
                        }

                        if (!dtr["調查起始日"].ToString().MinToDateTime().HasValue || !dtr["調查結束日"].ToString().MinToDateTime().HasValue || !dtr["日期"].ToString().MinToDateTime().HasValue)
                        {
                            errMsg = $"調查起始日、調查結束日、日期格式不正確";
                        }
                        if (!string.IsNullOrEmpty(errMsg))
                        {
                            TempData["TempMsg"] = errMsg;
                            TempData["TempResult"] = "error";
                            return View(new BaseResult());
                        }
                    }

                    if (!dt.Columns.Contains("調查頻率")) dt.Columns.Add("調查頻率");
                    if (!dt.Columns.Contains("調查點位數量")) dt.Columns.Add("調查點位數量");
                    if (!dt.Columns.Contains("調查人員")) dt.Columns.Add("調查人員");
                    if (!dt.Columns.Contains("里別")) dt.Columns.Add("里別");
                    if (!dt.Columns.Contains("北緯")) dt.Columns.Add("北緯");
                    if (!dt.Columns.Contains("東經")) dt.Columns.Add("東經");
                    if (!dt.Columns.Contains("備註")) dt.Columns.Add("備註");
                    if (!dt.Columns.Contains("害蟲數")) dt.Columns.Add("害蟲數");
                    if (!dt.Columns.Contains("作物")) dt.Columns.Add("作物");
                    if (!dt.Columns.Contains("調查株數")) dt.Columns.Add("調查株數");
                    if (!dt.Columns.Contains("受害株數")) dt.Columns.Add("受害株數");
                    if (!dt.Columns.Contains("0級")) dt.Columns.Add("0級");
                    if (!dt.Columns.Contains("1級")) dt.Columns.Add("1級");
                    if (!dt.Columns.Contains("2級")) dt.Columns.Add("2級");
                    if (!dt.Columns.Contains("3級")) dt.Columns.Add("3級");
                    if (!dt.Columns.Contains("4級")) dt.Columns.Add("4級");
                    if (!dt.Columns.Contains("地點")) dt.Columns.Add("地點");
                    if (!dt.Columns.Contains("總蟻數")) dt.Columns.Add("總蟻數");
                    if (!dt.Columns.Contains("其他蟻種")) dt.Columns.Add("其他蟻種");
                    if (!dt.Columns.Contains("其他蟻種數")) dt.Columns.Add("其他蟻種數");
                    #endregion
                    if (string.IsNullOrEmpty(errMsg) && dt.Rows.Count > 0)
                    {
                        var cropTypes = Service_cropType.GetList(x => x.Enable);
                        var crops = Service_crops.GetList(x => x.Enable && cropTypes.Select(o => o.ID).Contains(x.TypeID));
                        var MonitoeFrequery = Service_defCode.GetList("MonitoeFrequery");
                        var SurveyType = Service_defCode.GetList("SurveyType");

                        var oldMp = Service_monitorProject.GetList(null);
                        var oldMpa = Service_monitorProjectArea.GetList(null);
                        var oldMap = Service_monitorAreaPoint.GetList(null);
                        var oldMapd = Service_monitorAreaPointDate.GetList(null);

                        if (string.IsNullOrEmpty(errMsg))
                        {
                            int importCount = 0, errorCount = 0;
                            for (int i = 0; i < dt.Rows.Count; i++)
                            {
                                var row = dt.Rows[i];

                                if (row["調查起始日"].ToString().MinToDateTime().HasValue && row["調查結束日"].ToString().MinToDateTime().HasValue && row["日期"].ToString().MinToDateTime().HasValue)
                                {
                                    #region 整理資料
                                    var _iptMonitorProject = new monitorProject()
                                    {
                                        Year = row["年度"].ToString().ToInt32(),
                                        Name = row["專案名稱"].ToString(),
                                        Frequency = MonitoeFrequery.FirstOrDefault(x => x.Name.Contains(row["調查頻率"].ToString()) || x.Sort == row["調查頻率"].ToString().ToInt32())?.Code ?? "",
                                        StartDate = row["調查起始日"].ToString().MinToDateTime().Value,
                                        EndDate = row["調查結束日"].ToString().MinToDateTime().Value,
                                        Show = true,
                                    };
                                    if(_iptMonitorProject.Year < 1911)
                                    {
                                        _iptMonitorProject.Year += 1911;
                                    }
                                    var _iptMonitorProjectArea = new monitorProjectArea()
                                    {
                                        Type = SurveyType.FirstOrDefault(x => x.Name.Contains(row["調查對象類型"].ToString()) || x.Sort == row["調查對象類型"].ToString().ToInt32())?.Code ?? "",
                                        Distist = row["行政區"].ToString(),
                                        Inspectors = row["調查人員"].ToString(),
                                        Points = row["調查點位數量"].ToString().ToInt32(),
                                    };
                                    var _iptMonitorAreaPoint = new monitorAreaPoint()
                                    {
                                        Village = row["里別"].ToString(),
                                        SerialNum = row["編號"].ToString(),
                                        NorthLatitude = row["北緯"].ToString().ToDecimal32(),
                                        EastLongitude = row["東經"].ToString().ToDecimal32(),
                                        CropType = crops.FirstOrDefault(x => x.Name == row["作物"].ToString())?.TypeID ?? 0,
                                        Crops = crops.FirstOrDefault(x => x.Name == row["作物"].ToString())?.ID ?? 0,
                                    };
                                    var _iptMonitorAreaPointDate = new monitorAreaPointDate()
                                    {
                                        Date = row["日期"].ToString().MinToDateTime().Value,
                                        Pests = row["害蟲數"].ToString().ToInt32(),
                                        Surveys = row["調查株數"].ToString().ToInt32(),
                                        Victims = row["受害株數"].ToString().ToInt32(),
                                        Adress = row["地點"].ToString(),
                                        OtherArts = row["其他蟻種"].ToString(),
                                        OtherArtsNum = row["其他蟻種數"].ToString().ToInt32(),
                                        Level_0 = row["0級"].ToString().ToDecimal32(),
                                        Level_1 = row["1級"].ToString().ToDecimal32(),
                                        Level_2 = row["2級"].ToString().ToDecimal32(),
                                        Level_3 = row["3級"].ToString().ToDecimal32(),
                                        Level_4 = row["4級"].ToString().ToDecimal32(),
                                        Comment = row["備註"].ToString(),
                                        Tempeature = 0,
                                        DailyRainfall = 0,

                                        CropGrowthPeriod = "",
                                        SurveyArea = 0,
                                        DiscoverFAW = false,
                                        DamageContent = "",
                                        MonitoringRecord = "",
                                    };
                                    #endregion
                                    #region api取溫度降雨量
                                    var districtNum = new List<TownSnData>()
                                    {
                                        new TownSnData {TOWN="前鎮區",TOWN_SN="328"},new TownSnData{ TOWN="旗山區",TOWN_SN="283"},new TownSnData{ TOWN="桃源區",TOWN_SN="353"},new TownSnData{ TOWN="那瑪夏區",TOWN_SN="232"},new TownSnData{ TOWN="六龜區",TOWN_SN="259"},new TownSnData{ TOWN="杉林區",TOWN_SN="269"},new TownSnData{ TOWN="茂林區",TOWN_SN="278"},
                                        new TownSnData{ TOWN="甲仙區",TOWN_SN="251"},new TownSnData{ TOWN="美濃區",TOWN_SN="282"},new TownSnData{ TOWN="大樹區",TOWN_SN="300"},new TownSnData{ TOWN="內門區",TOWN_SN="270"},new TownSnData{ TOWN="田寮區",TOWN_SN="284"},new TownSnData{ TOWN="燕巢區",TOWN_SN="296"},new TownSnData{ TOWN="鳳山區",TOWN_SN="321"},
                                        new TownSnData{ TOWN="小港區",TOWN_SN="330"},new TownSnData{ TOWN="新興區",TOWN_SN="324"},new TownSnData{ TOWN="旗津區",TOWN_SN="329"},new TownSnData{ TOWN="阿蓮區",TOWN_SN="287"},new TownSnData{ TOWN="永安區",TOWN_SN="293"},new TownSnData{ TOWN="茄萣區",TOWN_SN="286"},new TownSnData{ TOWN="湖內區",TOWN_SN="285"},
                                        new TownSnData{ TOWN="彌陀區",TOWN_SN="298"},new TownSnData{ TOWN="岡山區",TOWN_SN="294"},new TownSnData{ TOWN="楠梓區",TOWN_SN="305"},new TownSnData{ TOWN="仁武區",TOWN_SN="306"},new TownSnData{ TOWN="鼓山區",TOWN_SN="316"},new TownSnData{ TOWN="三民區",TOWN_SN="314"},new TownSnData{ TOWN="苓雅區",TOWN_SN="323"},
                                        new TownSnData{ TOWN="林園區",TOWN_SN="335"},new TownSnData{ TOWN="大寮區",TOWN_SN="320"},new TownSnData{ TOWN="旗山區",TOWN_SN="283"},new TownSnData{ TOWN="路竹區",TOWN_SN="288"},new TownSnData{ TOWN="橋頭區",TOWN_SN="299"},new TownSnData{ TOWN="大社區",TOWN_SN="304"},new TownSnData{ TOWN="六龜區",TOWN_SN="259"},
                                        new TownSnData{ TOWN="左營區",TOWN_SN="311"}
                                    };
                                    if (districtNum.FirstOrDefault(x => x.TOWN == _iptMonitorProjectArea.Distist) != null)
                                    {
                                        var dataWeatherList = new List<AutoWeatherStationType>();
                                        using (var client = new HttpClient())
                                        {
                                            client.DefaultRequestHeaders.Accept.Clear();
                                            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                                            var sDate = _iptMonitorAreaPointDate.Date;
                                            var eDate = _iptMonitorAreaPointDate.Date.AddDays(1);
                                            var _url = $"https://data.coa.gov.tw/api/v1/AutoWeatherStationType/?Start_time={sDate}&End_time={eDate}&CITY_SN=5&TOWN_SN={districtNum.FirstOrDefault(x => x.TOWN == _iptMonitorProjectArea.Distist)?.TOWN_SN}";

                                            var res = await client.GetAsync(new Uri(_url));
                                            var data = JsonConvert.DeserializeObject<AutoWeatherStationTypeResponse>(await res.Content.ReadAsStringAsync());

                                            if (data.Data.Any()) dataWeatherList = data.Data;
                                        }
                                        if (dataWeatherList.Where(x => x.TIME.HasValue).Any(x => x.TOWN == _iptMonitorProjectArea.Distist && x.TIME.Value.Date == _iptMonitorAreaPointDate.Date.Date))
                                        {
                                            var thisWeatger = dataWeatherList.Where(x => x.TIME.HasValue).FirstOrDefault(x => x.TOWN == _iptMonitorProjectArea.Distist && x.TIME.Value.Date == _iptMonitorAreaPointDate.Date);
                                            _iptMonitorAreaPointDate.Tempeature = thisWeatger.TEMP.ToDecimal32();
                                            _iptMonitorAreaPointDate.DailyRainfall = thisWeatger.H_24R.ToInt32();
                                        }
                                    }
                                    #endregion

                                    #region 新增修改資料
                                    int mpId = 0, mpaId = 0, mapId = 0, mapdId = 0;
                                    if (oldMp.Any(x => x.Year == _iptMonitorProject.Year && x.Name == _iptMonitorProject.Name && x.StartDate.ToShortTimeString() == _iptMonitorProject.StartDate.ToShortTimeString() && x.EndDate.ToShortTimeString() == _iptMonitorProject.EndDate.ToShortTimeString()))
                                    {
                                        mpId = oldMp.FirstOrDefault(x => x.Year == _iptMonitorProject.Year && x.Name == _iptMonitorProject.Name && x.StartDate.ToShortTimeString() == _iptMonitorProject.StartDate.ToShortTimeString() && x.EndDate.ToShortTimeString() == _iptMonitorProject.EndDate.ToShortTimeString()).ID;
                                    }
                                    else
                                    {
                                        Service_monitorProject.Insert(_iptMonitorProject);
                                        mpId = _iptMonitorProject.ID;
                                    }

                                    if (mpId > 0)
                                    {
                                        if (oldMpa.Any(x => x.ProjectID == mpId && x.Type == _iptMonitorProjectArea.Type && x.Distist == _iptMonitorProjectArea.Distist))
                                        {
                                            var updMpa = oldMpa.FirstOrDefault(x => x.ProjectID == mpId && x.Type == _iptMonitorProjectArea.Type && x.Distist == _iptMonitorProjectArea.Distist);
                                            updMpa.Inspectors = _iptMonitorProjectArea.Inspectors;
                                            updMpa.Points = _iptMonitorProjectArea.Points;
                                            Service_monitorProjectArea.Update(updMpa);
                                            mpaId = updMpa.ID;
                                        }
                                        else
                                        {
                                            _iptMonitorProjectArea.ProjectID = mpId;
                                            Service_monitorProjectArea.Insert(_iptMonitorProjectArea);
                                            mpaId = _iptMonitorProjectArea.ID;
                                        }
                                        if (mpaId > 0)
                                        {
                                            _iptMonitorAreaPoint.ProjectAreaID = mpaId;
                                            if (oldMap.Any(x => x.ProjectAreaID == mpaId && x.SerialNum == _iptMonitorAreaPoint.SerialNum))
                                            {
                                                var updMap = oldMap.FirstOrDefault(x => x.ProjectAreaID == mpaId && x.SerialNum == _iptMonitorAreaPoint.SerialNum);
                                                _iptMonitorAreaPoint.ID = updMap.ID;
                                                _iptMonitorAreaPointDate.AreaPointID = updMap.ID;
                                                if (oldMapd.Any(x => x.AreaPointID == updMap.ID))
                                                {
                                                    var updMapd = oldMapd.FirstOrDefault(x => x.AreaPointID == updMap.ID);

                                                    _iptMonitorAreaPointDate.ID = updMapd.ID;
                                                    _iptMonitorAreaPointDate.CropGrowthPeriod = updMapd.CropGrowthPeriod;
                                                    _iptMonitorAreaPointDate.SurveyArea = updMapd.SurveyArea;
                                                    _iptMonitorAreaPointDate.DiscoverFAW = updMapd.DiscoverFAW;
                                                    _iptMonitorAreaPointDate.DamageContent = updMapd.DamageContent;
                                                    _iptMonitorAreaPointDate.MonitoringRecord = updMapd.MonitoringRecord;
                                                }
                                            }

                                            Service_monitorAreaPoint.InsertOrUpdate(_iptMonitorAreaPoint, _iptMonitorAreaPointDate);
                                            importCount++;
                                        }
                                    }
                                    #endregion
                                }
                                else
                                {
                                    errorCount++;
                                }
                            }
                            return View(new BaseResult(true, $"匯入成功{importCount}筆，錯誤{errorCount}筆"));
                        }
                    }
                }
            }
            #endregion
            return View(new BaseResult());
        }
     
        
        
        //年度
        public ActionResult YearList()
        {
            var _res = from mp in Service_monitorProject.GetList(null)
                       join mpa in Service_monitorProjectArea.GetList(null)
                       on mp.ID equals mpa.ProjectID into temp
                       from mpa in temp.DefaultIfEmpty()
                       join map in Service_monitorAreaPoint.GetList(null)
                       on mpa?.ID equals map.ProjectAreaID into temp2
                       from map in temp2.DefaultIfEmpty()
                       join mapd in Service_monitorAreaPointDate.GetList(null)
                       on map?.ID equals mapd.AreaPointID into temp3
                       from mapd in temp3.DefaultIfEmpty()
                       group new { mp, mpa, map, mapd } by mp.Year into gp
                       select new MonitorYearViewModel
                       {
                           Years = gp.Key,
                           ProjectCount = gp.Select(x => x.mp.ID).Distinct().Count(),
                           DataCount = gp.Count(x => x.mapd != null)
                       };

            return View(_res.ToList());
        }
        //專案List
        public ActionResult ProjectList(int year)
        {
            var data = from mp in Service_monitorProject.GetList(x => x.Year == year).AsEnumerable()
                       join dc in Service_defCode.GetList("MonitoeFrequery")
                       on mp.Frequency equals dc.Code
                       join mpa in Service_monitorProjectArea.GetList(null)
                       on mp.ID equals mpa.ProjectID into temp
                       from mpa in temp.DefaultIfEmpty()
                       group new { mp, dc, mpa }
                       by new { mp, dc } into gp
                       select new MonitorProjectViewModel
                       {
                           ID = gp.Key.mp.ID,
                           Name = gp.Key.mp.Name,
                           District = string.Join("、", gp.Select(x => x.mpa?.Distist).Distinct()),
                           Frequency = gp.Key.dc.Name,
                           StartDate = gp.Key.mp.StartDate,
                           EndDate = gp.Key.mp.EndDate,
                           ShowDel = gp.All(x => x.mpa == null)
                       };

            var lid = (Session["LoginID"] ?? "").ToString();
            var usr = Service_sysUserInfo.GetDetail(lid);
            var sBtn = false;
            if (usr != null)
            {
                sBtn = usr.RoleID == "R02";
                //sBtn = usr.RoleID == "R02" || usr.RoleID == "R01";
            }
            var _res = new ProjectListShowModel()
            {
                Year = year,
                ShowBtn = sBtn,
                DataModel = data.ToList()
            };

            return View(_res);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult ProjectList(string ActionName, int Years, int MpId = 0)
        {
            var eName = "";
            switch (ActionName)
            {
                case "del":
                    if (Service_monitorProject.GetDetail(MpId) != null && !Service_monitorProjectArea.GetList(x => x.ProjectID == MpId).Any())
                    {
                        Service_monitorProject.Delete(MpId);
                    }
                    break;
                case "exportYear":
                    var exp2 = new DataSet();
                    var _res2 = Service_monitorProject.GetViewList(x => x.Year == Years).GroupBy(x => new { x.ProjectName });
                    if (_res2.Any())
                    {
                        foreach (var item in _res2)
                        {
                            exp2.Tables.Add(GetProjectExcel(item.Key.ProjectName, item.OrderBy(x => x.Distist).ThenBy(x => x.Date).ToList(), item.Key.ProjectName));
                            eName = $"{Years}年度彙整表_{DateTime.Now.ToString("yyyyMMddHHmmss")}";
                        };
                        return File(ExcelHelper.RenderDataTableToExcel(exp2), "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", $"{eName}.xlsx");
                    }
                    break;
                case "exportId":
                    var exp = new DataSet();
                    var _res = Service_monitorProject.GetViewList(x => x.Year == Years && x.ID == MpId).GroupBy(x => new { x.ProjectName, x.Date.Month });
                    if (_res.Any())
                    {
                        foreach (var item in _res)
                        {
                            exp.Tables.Add(GetProjectExcel(item.Key.ProjectName, item.OrderBy(x => x.Distist).ThenBy(x => x.Date).ToList(), $"{item.Key.Month}月"));
                            eName = $"{Years}年度{item.Key.ProjectName}_{DateTime.Now.ToString("yyyyMMddHHmmss")}";
                        };
                        return File(ExcelHelper.RenderDataTableToExcel(exp), "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", $"{eName}.xlsx");
                    }
                    break;
            }

            return RedirectToAction(nameof(ProjectList), new { year = Years });
        }

        #region 匯出相關
        public DataTable GetProjectExcel(string Key, List<MonitorProjectModel> req, string tName)
        {
            switch (Key)
            {
                case "小黃薊馬":
                case "瓜實蠅":
                case "東方果實蠅":
                    return ExcelHelper.ConvertToDataTable(req.Select(x => new
                    {
                        行政區 = x.Distist,
                        里別 = x.Village,
                        編號 = x.SerialNum,
                        北緯 = x.NorthLatitude,
                        東經 = x.EastLongitude,
                        日期 = x.Date.ToString("MM-dd"),
                        害蟲數 = x.Pests,
                        作物類別 = x.CropType,
                        作物 = x.Crops,
                        備註 = x.Comment,
                    }).ToList(), tName);
                case "秋行軍蟲田間巡查":
                    return ExcelHelper.ConvertToDataTable(req.Select(x => new
                    {
                        行政區 = x.Distist,
                        里別 = x.Village,
                        編號 = x.SerialNum,
                        北緯 = x.NorthLatitude,
                        東經 = x.EastLongitude,
                        日期 = x.Date.ToString("MM-dd"),

                        作物類別 = x.CropType,
                        作物 = x.Crops,
                        調查株數 = x.Surveys,
                        受害株數 = x.Victims,
                        受害率 = GerSurveyRate(x.Victims, x.Surveys),
                        備註 = x.Comment,
                    }).ToList(), tName);
                case "葉稻熱病":
                case "穗稻熱病":
                    return ExcelHelper.ConvertToDataTable(req.Select(x => new
                    {
                        行政區 = x.Distist,
                        里別 = x.Village,
                        編號 = x.SerialNum,
                        北緯 = x.NorthLatitude,
                        東經 = x.EastLongitude,
                        日期 = x.Date.ToString("MM-dd"),

                        零級 = x.Level_0,
                        一級 = x.Level_1,
                        二級 = x.Level_2,
                        三級 = x.Level_3,
                        四級 = x.Level_4,
                        嚴重程度 = GerLevelString(x.Level_1, x.Level_2, x.Level_3, x.Level_4),
                        比例 = GerLevelRate(x.Level_1 + x.Level_2 + x.Level_3 + x.Level_4, x.Level_0 + x.Level_1 + x.Level_2 + x.Level_3 + x.Level_4),
                        備註 = x.Comment,
                    }).ToList(), tName);

                case "琉璃蟻":
                    return ExcelHelper.ConvertToDataTable(req.Select(x => new
                    {
                        行政區 = x.Distist,
                        里別 = x.Village,
                        編號 = x.SerialNum,
                        北緯 = x.NorthLatitude,
                        東經 = x.EastLongitude,
                        日期 = x.Date.ToString("MM-dd"),

                        地點 = x.Adress,
                        總蟻數 = x.Pests + x.OtherArtsNum,
                        其他蟻種 = x.OtherArts,
                        其他蟻種數 = x.OtherArtsNum,
                        備註 = x.Comment,
                    }).ToList(), tName);
            }
            return new DataTable();
        }
        #region 計算
        public decimal GerSurveyRate(int v, int s)
        {
            if (s == 0) return 0;
            return Math.Round(((decimal)v / (decimal)s), 2, MidpointRounding.AwayFromZero);
        }

        public string GerLevelString(decimal v1, decimal v2, decimal v3, decimal v4)
        {
            string lString = "";
            if (v1 != 0 || v2 != 0 || v3 != 0 || v4 != 0)
            {
                if (v4 >= v1 && v4 >= v2 && v4 >= v3) { lString = "4級"; }
                else if (v3 >= v1 && v3 >= v2 && v3 > v4) { lString = "3級"; }
                else if (v2 >= v1 && v2 > v3 && v2 > v4) { lString = "2級"; }
                else { lString = "1級"; }
            }
            else
            {
                lString = "0級";
            }
            return lString;
        }

        public decimal GerLevelRate(decimal v, decimal s)
        {
            if (s == 0) return 0;
            return Math.Round(((decimal)v / (decimal)s), 2, MidpointRounding.AwayFromZero);
        }
        #endregion
        #endregion

        //設定專案-內頁(編輯專案)
        public ActionResult ProjectInfo(int id = 0)
        {
            var _res = new MonitorProjectsEditModel();
            var data = Service_monitorProject.GetDetail(id);
            if (data != null)
            {
                _res.DataModel = new MonitorProjectsSaveModel()
                {
                    ID = data.ID,
                    Year = data.Year,
                    Name = data.Name,
                    Frequency = data.Frequency,
                    StartDate = data.StartDate,
                    EndDate = data.EndDate,
                    Show = data.Show,
                    AreaData = Service_monitorProjectArea.GetList(x => x.ProjectID == data.ID).Select(x => new MonitorProjectAreasEditModel
                    {
                        ID = x.ID,
                        Type = x.Type,
                        Distist = x.Distist,
                        Inspectors = x.Inspectors,
                        Points = x.Points,
                    }).ToList(),
                };
            }

            return View(_res);
        }
        //設定專案-內頁(編輯專案)
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult ProjectInfo(MonitorProjectsSaveModel obj)
        {
            var monitorProject = new monitorProject()
            {
                ID = obj.ID,
                Year = obj.Year,
                Name = obj.Name,
                Frequency = obj.Frequency,
                StartDate = obj.StartDate,
                EndDate = obj.EndDate,
                Show = obj.Show,
            };
            if (obj.ID > 0)
            {
                Service_monitorProject.Update(monitorProject);
            }
            else
            {
                Service_monitorProject.Insert(monitorProject);
                obj.ID = monitorProject.ID;
            }
            if (obj.AreaData.Any())
            {
                var areaObj = obj.AreaData.Select(x => new monitorProjectArea()
                {
                    ID = x.ID,
                    ProjectID = obj.ID,
                    Type = x.Type,
                    Distist = x.Distist,
                    Inspectors = x.Inspectors,
                    Points = x.Points,
                }).ToList();

                var delArea = Service_monitorProjectArea.GetList(x => !areaObj.Select(o => o.ID).Contains(x.ID) && x.ProjectID == obj.ID);
                if (delArea.Any())
                {
                    Service_monitorProjectArea.DeleteMany(x => !areaObj.Select(o => o.ID).Contains(x.ID) && x.ProjectID == obj.ID);
                }

                foreach (var item in areaObj)
                {
                    var _res = Service_monitorProjectArea.InsertOrUpdate(item);
                    if (!_res.result)
                    {
                        TempData["TempMsg"] = _res.Msg;
                        TempData["TempResult"] = "error";
                        return View(new MonitorProjectsEditModel());
                    }
                }
            }

            return RedirectToAction(nameof(ProjectList), new { year = obj.Year });
        }
        //調查項目(調查對象編輯頁)
        public ActionResult InvestItem(int mpid)
        {
            var _res = new MonitorProjectsSaveModel();
            var data = Service_monitorProject.GetDetail(mpid);
            if (data != null)
            {
                var _areaData = from mpa in Service_monitorProjectArea.GetList(x => x.ProjectID == data.ID)
                                join dc in Service_defCode.GetList("SurveyType")
                                on mpa.Type equals dc.Code
                                join map in Service_monitorAreaPoint.GetList(null)
                                on mpa.ID equals map.ProjectAreaID into temp
                                from map in temp.DefaultIfEmpty()
                                join mapd in Service_monitorAreaPointDate.GetList(null)
                                on map?.ID equals mapd.AreaPointID into temp2
                                from mapd in temp2.DefaultIfEmpty()
                                group new { mpa, dc, mapd } by new { mpa, dc } into gp
                                select new MonitorProjectAreasEditModel
                                {
                                    ID = gp.Key.mpa.ID,
                                    Type = gp.Key.dc.Name,
                                    Distist = gp.Key.mpa.Distist,
                                    Inspectors = gp.Key.mpa.Inspectors,
                                    Points = gp.Key.mpa.Points,
                                    UpdateAt = gp.Max(x => x.mapd?.UpdatedAt) ?? (gp.Key.mpa.UpdatedAt ?? gp.Key.mpa.CreatedAt),
                                    ShowDel = gp.All(x => x.mapd == null)
                                };

                var mfCode = Service_defCode.GetList("MonitoeFrequery");
                _res = new MonitorProjectsSaveModel()
                {
                    ID = data.ID,
                    Year = data.Year,
                    Name = data.Name,
                    Frequency = mfCode.FirstOrDefault(x => x.Code == data.Frequency)?.Name,
                    StartDate = data.StartDate,
                    EndDate = data.EndDate,
                    Show = data.Show,
                    AreaData = _areaData.ToList(),
                };
            }
            else
            {
                return RedirectToAction(nameof(YearList));
            }

            return View(_res);
        }
        [HttpPost] //刪除方法為POST
        public ActionResult InvestItemDelete(int Id, int mpid)
        {
            if (Service_monitorProjectArea.GetDetail(Id) != null && !Service_monitorAreaPoint.GetList(x => x.ProjectAreaID == Id).Any())
            {
                Service_monitorProjectArea.Delete(Id);
            }
            return RedirectToAction(nameof(InvestItem), new { mpid = mpid });
        }

        //調查內容(調查對象內容)
        public ActionResult InvestInfo(int mpaid)
        {
            var _res = new InvestInfoShowModel();
            var getMPA = Service_monitorProjectArea.GetDetail(mpaid);
            if (getMPA != null)
            {
                var monitorProject = Service_monitorProject.GetDetail(getMPA.ProjectID);
                var surverMonths = new List<int>();
                var s = monitorProject.StartDate.Month;
                var e = monitorProject.EndDate.Month;
                while (s <= e)
                {
                    surverMonths.Add(s);
                    s++;
                }

                var sType = Service_defCode.GetList("SurveyType");
                _res.mpId = getMPA.ProjectID;
                _res.mpaId = mpaid;
                _res.Years = monitorProject.Year;
                _res.ProjectName = monitorProject.Name;
                _res.Type = sType.FirstOrDefault(x => x.Code == getMPA.Type)?.Name;
                _res.Distist = getMPA.Distist;
                if (monitorProject.Name == "葉稻熱病" || monitorProject.Name == "穗稻熱病")
                {
                    var severityData = (from map in Service_monitorAreaPoint.GetList(x => x.ProjectAreaID == mpaid).AsEnumerable()
                                        join mapd in Service_monitorAreaPointDate.GetList(null)
                                        on map.ID equals mapd.AreaPointID
                                        select new SeverityModel
                                        {
                                            Year = mapd.Date.Year,
                                            Month = mapd.Date.Month,
                                            NowWeek = mapd.Date.GetWeekNumInMonth(),
                                            Level_0 = mapd.Level_0,
                                            Level_1 = mapd.Level_1,
                                            Level_2 = mapd.Level_2,
                                            Level_3 = mapd.Level_3,
                                            Level_4 = mapd.Level_4,
                                            Severity = "",
                                            Scale = 0,
                                        }).GroupBy(x => new { x.Year, x.Month, x.NowWeek }).Select(x => new SeverityModel
                                        {
                                            Year = x.Key.Year,
                                            Month = x.Key.Month,
                                            NowWeek = x.Key.NowWeek,
                                            Level_0 = x.Sum(o => o.Level_0),
                                            Level_1 = x.Sum(o => o.Level_1),
                                            Level_2 = x.Sum(o => o.Level_2),
                                            Level_3 = x.Sum(o => o.Level_3),
                                            Level_4 = x.Sum(o => o.Level_4),
                                            Severity = GetTop(x.Sum(o => o.Level_1), x.Sum(o => o.Level_2), x.Sum(o => o.Level_3), x.Sum(o => o.Level_4)),
                                            Scale = GetAvg(x.Sum(o => o.Level_1 + o.Level_2 + o.Level_3 + o.Level_4), x.Sum(o => o.Level_0 + o.Level_1 + o.Level_2 + o.Level_3 + o.Level_4)),
                                        }).ToList();

                    if (surverMonths.Where(x => severityData.Select(o => o.Month).Contains(x)).Count() != surverMonths.Count)
                    {
                        foreach (var item in severityData)
                        {
                            item.ShowCopy = true;
                        }
                    }
                    _res.SeverityModels = severityData.ToList();
                }
                else if (monitorProject.Name.Contains("秋行軍蟲"))
                {
                    var bugData = (from map in Service_monitorAreaPoint.GetList(x => x.ProjectAreaID == mpaid).AsEnumerable()
                                   join mapd in Service_monitorAreaPointDate.GetList(null)
                                   on map.ID equals mapd.AreaPointID
                                   join cp in Service_crops.GetList(null)
                                   on map.Crops equals cp.ID into temp
                                   from cp in temp.DefaultIfEmpty()
                                   select new
                                   {
                                       Year = mapd.Date.Year,
                                       Month = mapd.Date.Month,
                                       TotalCount = mapd.Victims,
                                       SurveyCount = mapd.Surveys,
                                       Crops = cp?.Name,
                                   }).GroupBy(x => new { x.Year, x.Month }).Select(x => new BugTotalModel
                                   {
                                       Year = x.Key.Year,
                                       Month = x.Key.Month,
                                       TotalCount = x.Sum(o => o.TotalCount),
                                       Crops = string.Join("、", x.Where(o => !string.IsNullOrEmpty(o.Crops)).Select(o => o.Crops).Distinct()),
                                       VictimRate = GetRate(x.Sum(o => o.SurveyCount), x.Sum(o => o.TotalCount))
                                   }).ToList();

                    if (surverMonths.Where(x => bugData.Select(o => o.Month).Contains(x)).Count() != surverMonths.Count)
                    {
                        foreach (var item in bugData)
                        {
                            item.ShowCopy = true;
                        }
                    }

                    _res.BugTotalModels = bugData.ToList();
                }
                else if (monitorProject.Name.Contains("琉璃蟻"))
                {
                    var bugData = (from map in Service_monitorAreaPoint.GetList(x => x.ProjectAreaID == mpaid).AsEnumerable()
                                   join mapd in Service_monitorAreaPointDate.GetList(null)
                                   on map.ID equals mapd.AreaPointID
                                   select new
                                   {
                                       Year = mapd.Date.Year,
                                       Month = mapd.Date.Month,
                                       TotalCount = mapd.Pests,
                                       OtherCount = mapd.OtherArtsNum,
                                   }).GroupBy(x => new { x.Year, x.Month }).Select(x => new BugTotalModel
                                   {
                                       Year = x.Key.Year,
                                       Month = x.Key.Month,
                                       TotalCount = x.Sum(o => o.TotalCount),
                                       OtherCount = x.Sum(o => o.OtherCount),
                                       VictimRate = GetRate(x.Sum(o => o.TotalCount), x.Sum(o => o.OtherCount))
                                   }).ToList();

                    if (surverMonths.Where(x => bugData.Select(o => o.Month).Contains(x)).Count() != surverMonths.Count)
                    {
                        foreach (var item in bugData)
                        {
                            item.ShowCopy = true;
                        }
                    }

                    _res.BugTotalModels = bugData.ToList();
                }
                else
                {
                    var bugData = (from map in Service_monitorAreaPoint.GetList(x => x.ProjectAreaID == mpaid).AsEnumerable()
                                   join mapd in Service_monitorAreaPointDate.GetList(null)
                                   on map.ID equals mapd.AreaPointID
                                   join cp in Service_crops.GetList(null)
                                   on map.Crops equals cp.ID into temp
                                   from cp in temp.DefaultIfEmpty()
                                   select new BugTotalModel
                                   {
                                       Year = mapd.Date.Year,
                                       Month = mapd.Date.Month,
                                       TotalCount = mapd.Pests,
                                       Crops = cp?.Name,
                                   }).GroupBy(x => new { x.Year, x.Month }).Select(x => new BugTotalModel
                                   {
                                       Year = x.Key.Year,
                                       Month = x.Key.Month,
                                       TotalCount = x.Sum(o => o.TotalCount),
                                       Crops = string.Join("、", x.Where(o => !string.IsNullOrEmpty(o.Crops)).Select(o => o.Crops).Distinct()),
                                   }).ToList();

                    if (surverMonths.Where(x => bugData.Select(o => o.Month).Contains(x)).Count() != surverMonths.Count)
                    {
                        foreach (var item in bugData)
                        {
                            item.ShowCopy = true;
                        }
                    }

                    _res.BugTotalModels = bugData.ToList();
                }
            }
            else
            {
                return RedirectToAction(nameof(YearList));
            }
            return View(_res);
        }

        private decimal GetRate(decimal sCount, decimal vCount)
        {
            if (sCount > 0)
            {
                return Math.Round((((decimal)vCount / (decimal)sCount) * 100), 2, MidpointRounding.AwayFromZero);
            }
            return 0;
        }

        //登打紀錄
        public ActionResult Record(int mpaid, int year, int month = 0, int copymonth = 0)
        {
            var _response = new RecordShowModel();

            var getMPA = Service_monitorProjectArea.GetDetail(mpaid);
            if (getMPA != null)
            {
                var monitorProject = Service_monitorProject.GetDetail(getMPA.ProjectID);
                _response.mpaid = getMPA.ID;
                _response.Years = year;
                _response.Months = month;
                _response.ProjectName = monitorProject.Name;

                var _res = new List<MapdModel>();

                if (month > 0)
                {
                    _res = (from map in Service_monitorAreaPoint.GetList(x => x.ProjectAreaID == mpaid).AsEnumerable()
                            join mapd in Service_monitorAreaPointDate.GetList(x => x.Date.Month == month)
                            on map.ID equals mapd.AreaPointID
                            select new MapdModel
                            {
                                ID = map.ID,
                                Village = map.Village,
                                SerialNum = map.SerialNum,
                                NorthLatitude = map.NorthLatitude,
                                EastLongitude = map.EastLongitude,

                                CropType = map.CropType,
                                Crops = map.Crops,
                                Comment = map.Comment,
                                mapdID = mapd.ID,
                                Date = mapd.Date,

                                Pests = mapd.Pests,
                                Surveys = mapd.Surveys,
                                Victims = mapd.Victims,
                                CropGrowthPeriod = mapd.CropGrowthPeriod,
                                SurveyArea = mapd.SurveyArea,

                                DiscoverFAW = mapd.DiscoverFAW,
                                DamageContent = mapd.DamageContent,
                                Adress = mapd.Adress,
                                OtherArts = mapd.OtherArts,
                                OtherArtsNum = mapd.OtherArtsNum,

                                Tempeature = mapd.Tempeature,
                                Level_0 = mapd.Level_0,
                                Level_1 = mapd.Level_1,
                                Level_2 = mapd.Level_2,
                                Level_3 = mapd.Level_3,

                                Level_4 = mapd.Level_4,
                                mapdComment = mapd.Comment,
                                MonitoringRecord = mapd.MonitoringRecord,
                                DailyRainfall = mapd.DailyRainfall,
                            }).ToList();

                    foreach (var item in _res)
                    {
                        item.OldFiles = Service_FileManagement.GetList("monitorAreaPoints", item.ID.ToString());
                    }
                }
                else if (copymonth > 0)
                {
                    _res = (from map in Service_monitorAreaPoint.GetList(x => x.ProjectAreaID == mpaid).AsEnumerable()
                            join mapd in Service_monitorAreaPointDate.GetList(x => x.Date.Month == copymonth)
                            on map.ID equals mapd.AreaPointID
                            select new MapdModel
                            {
                                ID = 0,
                                Village = map.Village,
                                SerialNum = map.SerialNum,
                                NorthLatitude = map.NorthLatitude,
                                EastLongitude = map.EastLongitude,

                                CropType = map.CropType,
                                Crops = map.Crops,
                                Comment = map.Comment,
                                mapdID = mapd.ID,
                                Date = mapd.Date,

                                Pests = mapd.Pests,
                                Surveys = mapd.Surveys,
                                Victims = mapd.Victims,
                                CropGrowthPeriod = mapd.CropGrowthPeriod,
                                SurveyArea = mapd.SurveyArea,

                                DiscoverFAW = mapd.DiscoverFAW,
                                DamageContent = mapd.DamageContent,
                                Adress = mapd.Adress,
                                OtherArts = mapd.OtherArts,
                                OtherArtsNum = mapd.OtherArtsNum,

                                Tempeature = mapd.Tempeature,
                                Level_0 = mapd.Level_0,
                                Level_1 = mapd.Level_1,
                                Level_2 = mapd.Level_2,
                                Level_3 = mapd.Level_3,

                                Level_4 = mapd.Level_4,
                                mapdComment = mapd.Comment,
                                MonitoringRecord = mapd.MonitoringRecord,
                                DailyRainfall = mapd.DailyRainfall,
                            }).ToList();
                }

                _response.Datas = _res;
            }
            else
            {
                return RedirectToAction(nameof(YearList));
            }

            return View(_response);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Record(RecordShowModel obj)
        {
            if (obj.Datas.Any(x => x.Date < new DateTime(2000, 1, 1) || x.Date > new DateTime(2099, 1, 1)))
            {
                TempData["TempMsg"] = "請輸入正確日期格式";
                TempData["TempResult"] = "error";
                return View(obj);
            }
            if (obj.Datas.Any())
            {
                var _originData = (from map in Service_monitorAreaPoint.GetList(x => x.ProjectAreaID == obj.mpaid).AsEnumerable()
                                   join mapd in Service_monitorAreaPointDate.GetList(x => x.Date.Month == obj.Months)
                                   on map.ID equals mapd.AreaPointID
                                   select new
                                   {
                                       ID = map.ID,
                                       mapdID = mapd.ID,
                                   }).ToList();
                foreach (var item in _originData.Where(x => !obj.Datas.Select(o => o.ID).Contains(x.ID)).ToList())
                {
                    try
                    {
                        Service_monitorAreaPoint.Delete(item.ID);
                        Service_monitorAreaPointDate.Delete(item.mapdID);
                    }
                    catch (Exception ex)
                    {

                    }
                }
                var getMPA = Service_monitorProjectArea.GetDetail(obj.mpaid);
                var _data = obj.Datas.Select(x => new
                {
                    MapData = new monitorAreaPoint()
                    {
                        ID = x.ID,
                        ProjectAreaID = obj.mpaid,
                        Village = x.Village,
                        SerialNum = x.SerialNum,
                        NorthLatitude = x.NorthLatitude,
                        EastLongitude = x.EastLongitude,
                        CropType = x.CropType,
                        Crops = x.Crops,
                        Comment = "",
                    },
                    MapdData = new monitorAreaPointDate()
                    {
                        ID = x.mapdID,
                        AreaPointID = x.ID,
                        Date = x.Date,
                        Pests = x.Pests,
                        Surveys = x.Surveys,
                        Victims = x.Victims,
                        CropGrowthPeriod = x.CropGrowthPeriod,
                        SurveyArea = x.SurveyArea,
                        DiscoverFAW = x.DiscoverFAW,
                        DamageContent = x.DamageContent,
                        Adress = x.Adress,
                        OtherArts = x.OtherArts,
                        OtherArtsNum = x.OtherArtsNum,
                        Tempeature = x.Tempeature,
                        Level_0 = x.Level_0,
                        Level_1 = x.Level_1,
                        Level_2 = x.Level_2,
                        Level_3 = x.Level_3,
                        Level_4 = x.Level_4,
                        Comment = x.mapdComment,
                        MonitoringRecord = x.MonitoringRecord,
                        DailyRainfall = x.DailyRainfall,
                    },
                    x.EditFiles,
                    x.PictureFiles
                }).ToList();

                var dataWeatherList = Service_DailyWeather.GetList(x => obj.Datas.Min(o => o.Date) <= x.DataDate && obj.Datas.Max(o => o.Date) >= x.DataDate);
                foreach (var item in _data)
                {
                    if (dataWeatherList.Any(x => x.DistrictName == getMPA.Distist && x.DataDate.Date == item.MapdData.Date.Date))
                    {
                        var thisWeatger = dataWeatherList.FirstOrDefault(x => x.DistrictName == getMPA.Distist && x.DataDate.Date == item.MapdData.Date.Date);
                        item.MapdData.Tempeature = thisWeatger.Temp;
                        item.MapdData.DailyRainfall = (int)thisWeatger.Rainfall;
                    }
                }
                foreach (var item in _data)
                {
                    Service_monitorAreaPoint.InsertOrUpdate(item.MapData, item.MapdData);

                    try
                    {
                        var _oldFile = Service_FileManagement.GetList("monitorAreaPoints", item.MapData.ID.ToString());
                        if (_oldFile != null && _oldFile.Any())
                        {
                            foreach (var itemFile in _oldFile.Where(x => !item.EditFiles.Contains(x.ID)))
                            {
                                var thisFile = _oldFile.FirstOrDefault(x => x.ID == itemFile.ID);
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

                        if (item.PictureFiles != null && item.PictureFiles.Count > 0)
                        {
                            string baseDirectory = "/UploadedFiles/monitorAreaPoints";
                            foreach (var file in item.PictureFiles)
                            {
                                if (file == null)
                                {
                                    continue;
                                }
                                string _FileName = Path.GetFileName(file.FileName);
                                string _directory = Server.MapPath("~" + baseDirectory);
                                IscomG2C.Utility.FileHelper.CreateFolder(_directory);

                                string _path = Path.Combine(_directory, _FileName);
                                file.SaveAs(_path);
                                var url = String.Concat(baseDirectory, '/', _FileName);
                                Service_FileManagement.Insert(new FileManagement()
                                {
                                    TableName = "monitorAreaPoints",
                                    TableID = item.MapData.ID.ToString(),
                                    FileName = file.FileName,
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

                    }
                }
            }

            return RedirectToAction(nameof(InvestInfo), new { mpaid = obj.mpaid });
        }
        public string GetTop(decimal v1, decimal v2, decimal v3, decimal v4)
        {
            if (v1 != 0 || v2 != 0 || v3 != 0 || v4 != 0)
            {
                if (v4 >= v1 && v4 >= v2 && v4 >= v3) return "4級";
                else if (v3 >= v1 && v3 >= v2 && v3 > v4) return "3級";
                else if (v2 >= v1 && v2 > v3 && v2 > v4) return "2級";
                else return "1級";
            }
            else
            {
                return "0級";
            }
        }
        private decimal GetAvg(decimal vSum, decimal vTotal)
        {
            if (vTotal == 0) return 0;
            return vSum / vTotal;
        }
    }
}