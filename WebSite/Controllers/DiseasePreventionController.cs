using ICCModule;
using ICCModule.Entity.Tables.Platform;
using ICCModule.EntityService.Service;
using ICCModule.Helper;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using Website.ViewModel;
using Website.ViewModel.DiseasePrevention;
using System.Web.Security.AntiXss;

namespace Website.Controllers
{
    public class DiseasePreventionController : Controller
    {
        //病蟲害防治-List
        public ActionResult PreventList()
        {
            var _res = Service_controlHistory.GetList(x => x.Enable).GroupBy(x => x.Type);
            return View(_res);
        }
        //病蟲害防治-內頁
        public ActionResult PreventInfo(int id = 0)
        {
            var sName = "PreventInfo" + id;
            var getSession = SessionHelper.Get(sName);

            var _res = Service_controlHistory.GetListAndAddclick(id, (string.IsNullOrEmpty(getSession)));
            if (string.IsNullOrEmpty(getSession))
            {
                SessionHelper.Set(sName, _res.ID);
            }
            var response = new ControlHistoryFrontModel
            {
                ID = _res.ID,
                Name = _res.Name,
                Type = _res.Type,
                Enable = _res.Enable,
                ClickCount = _res.ClickCount,
                Information = _res.Information,
                Explanation = _res.Explanation,
                UpdatedAt = _res.UpdatedAt,
                ControlHistoryCropFrontModels = new List<ControlHistoryCropFrontModel>(),
            };

            var _chc = Service_controlHistoryCrop.GetList(x => x.Show && x.ControlID == id).Select(x => new ControlHistoryCropFrontModel
            {
                ChcId = x.ID,
                Name = x.Name,
                Sort = x.Sort,
                Type = x.Type,
                Comment = x.Comment,
                DayCount = x.DayCount,
                CollumHeads = new List<FrontCollumHead>(),
                ContentGroups = new List<FrontContentGroup>(),
            }).ToList();

            foreach (var data in _chc)
            {
                var contentsData = Service_controlHistoryCropsContents.GetList(x => x.ControlHistoryCropId == data.ChcId);
                var cHeader = new List<FrontCollumHead>();
                var cGroup = new List<FrontContentGroup>();
                if (contentsData.Any())
                    if (data.Type)
                    {
                        for (var i = 0; i <= 12; i++)
                        {
                            if (i == 0) cHeader.Add(new FrontCollumHead { CollumName = "防治曆", Sort = i });
                            else cHeader.Add(new FrontCollumHead { CollumName = $"{i}月", Sort = i });
                        }
                        data.CollumHeads = cHeader;
                        data.ContentGroups = contentsData.GroupBy(x => x.ControlStage).Select(item => new FrontContentGroup()
                        {
                            ContentName = item.Key,
                            Sort = item.Key == "生長期" ? 1 : (item.Key == "生活史" ? 2 : (item.Key == "慣行栽植" ? 3 : 4)),
                            ContentDatas = item.Select(x => new FrontContentData
                            {
                                Id = x.Id,
                                Name = x.Name,
                                Width = (x.EndBlock.DateToDecimal() - x.StartBlock.DateToDecimal()) * 100,
                                Margin = (x.StartBlock.DateToDecimal() - x.StartBlock.ToDateTime()?.Month ?? 0) * 100,
                                Sort = x.StartBlock.ToDateTime()?.Month ?? 0,
                                ShowType = x.ShowType,
                                StartDate = x.StartBlock.ToDateTime().Value,
                                EndDate = x.EndBlock.ToDateTime().Value,
                            }).OrderBy(x => x.Sort).ToList()
                        }).ToList();

                        int s = 1;
                        var newContentGroup = new List<FrontContentGroup>();
                        foreach (var item in data.ContentGroups)
                        {
                            var j = 0;
                            var exceptId = new List<int>();
                            var oldContentData = new List<FrontContentData>();
                            foreach (var itemData in item.ContentDatas)
                            {
                                if (s != itemData.Sort) j = 0;
                                s = itemData.Sort;
                                var newData = new List<FrontContentData>();
                                if (oldContentData.Where(x => !exceptId.Contains(x.Id)).Any(x => x.Sort < s && (Math.Max(x.StartDate.Ticks, itemData.StartDate.Ticks) < Math.Min(x.EndDate.Ticks, itemData.EndDate.Ticks))))
                                {
                                    var addData = newContentGroup.FirstOrDefault(x => x.ContentDatas.All(o => !(Math.Max(o.StartDate.Ticks, itemData.StartDate.Ticks) < Math.Min(o.EndDate.Ticks, itemData.EndDate.Ticks))));
                                    if (addData != null)
                                    {
                                        addData.ContentDatas.Add(itemData);
                                    }
                                    else
                                    {
                                        newData.Add(itemData);
                                        newContentGroup.Add(new FrontContentGroup()
                                        {
                                            ContentName = item.ContentName,
                                            Sort = item.Sort,
                                            ContentDatas = newData
                                        });
                                    }
                                    exceptId.Add(itemData.Id);
                                }
                                else if (j > 0 && oldContentData.Where(x => !exceptId.Contains(x.Id)).Any(x => x.Sort == s && x.Id != itemData.Id && (Math.Max(x.StartDate.Ticks, itemData.StartDate.Ticks) < Math.Min(x.EndDate.Ticks, itemData.EndDate.Ticks))))
                                {
                                    var addData = newContentGroup.FirstOrDefault(x => x.ContentDatas.All(o => !(Math.Max(o.StartDate.Ticks, itemData.StartDate.Ticks) < Math.Min(o.EndDate.Ticks, itemData.EndDate.Ticks))));
                                    if (addData != null)
                                    {
                                        addData.ContentDatas.Add(itemData);
                                    }
                                    else
                                    {
                                        newData.Add(itemData);
                                        newContentGroup.Add(new FrontContentGroup()
                                        {
                                            ContentName = item.ContentName,
                                            Sort = item.Sort,
                                            ContentDatas = newData
                                        });
                                    }
                                    exceptId.Add(itemData.Id);
                                }
                                else
                                {
                                    oldContentData.Add(itemData);
                                }
                                j++;
                            }
                            item.ContentDatas = item.ContentDatas.Where(x => !exceptId.Contains(x.Id)).ToList();
                            s++;
                        }
                        data.ContentGroups.AddRange(newContentGroup);
                    }
                    else
                    {
                        var maxCount = data.DayCount ?? contentsData.Select(x => x.EndBlock.ToInt32()).Max();
                        var midCount = Math.Round((decimal)(maxCount / 10), 0, MidpointRounding.AwayFromZero);
                        for (var i = 10; i >= 0; i--)
                        {
                            if (i == 0) cHeader.Add(new FrontCollumHead { CollumName = "防治曆", Sort = i });
                            else cHeader.Add(new FrontCollumHead { CollumName = $"{maxCount - midCount * (10 - i)}", Sort = (int)(maxCount - midCount * (10 - i)) });
                        }
                        data.CollumHeads = cHeader;
                        var cArray = cHeader.Where(o => o.Sort != 0).Select(o => o.Sort).ToList();
                        data.ContentGroups = contentsData.GroupBy(x => x.ControlStage).Select(item => new FrontContentGroup()
                        {
                            ContentName = item.Key,
                            Sort = item.Key == "生長期" ? 1 : (item.Key == "生活史" ? 2 : (item.Key == "慣行栽植" ? 3 : 4)),
                            ContentDatas = item.Select(x => new FrontContentData
                            {
                                Id = x.Id,
                                Name = x.Name,
                                Width = (x.EndBlock.ToInt32().IntInterval(cArray) - x.StartBlock.ToInt32().IntInterval(cArray)) * 100,
                                Margin = (x.StartBlock.ToInt32().IntInterval(cArray) * 100) % 100,
                                Sort = (int)(x.StartBlock.ToInt32().IntInterval(cArray) / 1) + 1,
                                ShowType = x.ShowType,
                                StartInt = x.StartBlock.ToInt32(),
                                EndInt = x.EndBlock.ToInt32(),
                            }).OrderBy(x => x.Sort).ToList()
                        }).ToList();

                        int s = 1;
                        var newContentGroup = new List<FrontContentGroup>();
                        foreach (var item in data.ContentGroups)
                        {
                            var j = 0;
                            var exceptId = new List<int>();
                            var oldContentData = new List<FrontContentData>();
                            foreach (var itemData in item.ContentDatas)
                            {
                                if (s != itemData.Sort) j = 0;
                                s = itemData.Sort;
                                var newData = new List<FrontContentData>();
                                if (oldContentData.Any(x => x.Sort < s && (Math.Max(x.StartInt, itemData.StartInt) < Math.Min(x.EndInt, itemData.EndInt))))
                                {
                                    var addData = newContentGroup.FirstOrDefault(x => x.ContentDatas.All(o => !(Math.Max(o.StartInt, itemData.StartInt) < Math.Min(o.EndInt, itemData.EndInt))));
                                    if (addData != null)
                                    {
                                        addData.ContentDatas.Add(itemData);
                                    }
                                    else
                                    {
                                        newData.Add(itemData);
                                        newContentGroup.Add(new FrontContentGroup()
                                        {
                                            ContentName = item.ContentName,
                                            Sort = item.Sort,
                                            ContentDatas = newData
                                        });
                                    }
                                    exceptId.Add(itemData.Id);
                                }
                                else if (j > 0 && oldContentData.Any(x => x.Sort == s && x.Id != itemData.Id && (Math.Max(x.StartInt, itemData.StartInt) < Math.Min(x.EndInt, itemData.EndInt))))
                                {
                                    var addData = newContentGroup.FirstOrDefault(x => x.ContentDatas.All(o => !(Math.Max(o.StartInt, itemData.StartInt) < Math.Min(o.EndInt, itemData.EndInt))));
                                    if (addData != null)
                                    {
                                        addData.ContentDatas.Add(itemData);
                                    }
                                    else
                                    {
                                        newData.Add(itemData);
                                        newContentGroup.Add(new FrontContentGroup()
                                        {
                                            ContentName = item.ContentName,
                                            Sort = item.Sort,
                                            ContentDatas = newData
                                        });
                                    }
                                    exceptId.Add(itemData.Id);
                                }
                                else
                                {
                                    oldContentData.Add(itemData);
                                }
                                j++;
                            }
                            item.ContentDatas = item.ContentDatas.Where(x => !exceptId.Contains(x.Id)).ToList();
                            s++;
                        }
                        data.ContentGroups.AddRange(newContentGroup);
                    }
            }
            response.ControlHistoryCropFrontModels = _chc;

            return View(response);
        }

        //地圖查詢
        public ActionResult Map()
        {
            MapViewModel model = new MapViewModel();

            return View(model);
        }

        /// <summary>
        /// 取回全部販售業者，交由前台進行過濾
        /// </summary>
        /// <returns></returns>
        public ActionResult PesticideSellerList()
        {
            var dataList = Service_pesticideSeller.GetSellerList(x => true).Select(x => new pesticideSellerSelectModel
            {
                LicenseNum = x.LicenseNum,
                VendorName = x.VendorName,
                VendorAddress = x.VendorAddress,
                Location = x.Location,
                Status = x.Status,
                ContactPhone = x.ContactPhone,
                LastCheckDate = x.LastCheckDate,
                FriendlyStartDate = x.FriendlyStartDate,
                FriendlyEndDate = x.FriendlyEndDate,
                CreatedAt = x.CreatedAt,
                UpdatedAt = x.UpdatedAt ?? x.CreatedAt,
                Latitude = x.Latitude,
                Longitude = x.Longitude,
                StatusString = x.Status == 1 ? "營業中" : (x.Status == 2 ? "停業" : "歇業"),
                UpdateString = (x.UpdatedAt ?? x.CreatedAt).ToString("yyyy-MM-dd")
            }).ToList();
            return Json(dataList, JsonRequestBehavior.AllowGet);
        }

        //儀錶板
        public ActionResult DashBoard()
        {
            var defaultDate = (new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1)).AddMonths(-5);
            var data = Service_doctorSchedule.GetList(x => x.ReserveDatetime >= defaultDate && !x.Status.Contains("Cancel"));
            #region 地圖
            var mapData = from ds in data
                          join ll in GetZipLngLat()
                          on ds.Zip equals ll.Zip
                          group new { ds, ll } by new { ds.Zip, ds.District, ll.Lat, ll.Lng } into gp
                          select new DashboardMap
                          {
                              DistrictName = gp.Key.District,
                              Lat = gp.Key.Lat,
                              Lng = gp.Key.Lng,
                              PopupWord = AntiXssEncoder.HtmlEncode($"高雄市{gp.Key.District}cla" + string.Join("cla", gp.GroupBy(x => x.ds.CropName).Select(x => x.Key + $" {x.Count()}件")), true),
                              TotalCount = gp.Count(),
                          };
            #endregion
            #region 折線圖
            var statisticData = data.GroupBy(x => new { Date = new DateTime(x.ReserveDatetime.Year, x.ReserveDatetime.Month, 1) }).Select(x => new
            {
                x.Key.Date,
                DataCount = x.Count()
            }).OrderBy(x => x.Date);
            #endregion
            #region 作物分類Top5
            var rankData = data.GroupBy(x => x.CropName).Select(x => new CropRank
            {
                Rank = 0,
                CropName = x.Key,
                DataCount = x.Count()
            }).OrderByDescending(x => x.DataCount).ToList();
            int lastCount = 0, lastRank = 0;
            foreach (var item in rankData)
            {
                if (lastCount == 0)
                {
                    lastCount = item.DataCount;
                    lastRank = 1;
                    item.Rank = lastRank;
                }
                else
                {
                    if (lastCount > item.DataCount)
                    {
                        lastCount = item.DataCount;
                        lastRank += 1;
                    }
                    item.Rank = lastRank;
                }
            }
            #endregion

            var showData = new DashboardModel()
            {
                QueryData = new DashboardQueryModel()
                {
                    StartDate = defaultDate,
                },
                DataCount = statisticData.Select(x => x.DataCount).ToList(),
                Label = statisticData.Select(x => AntiXssEncoder.HtmlEncode(x.Date.ToString("yyyy-MM-dd"),true)).ToList(),
                CropRanks = rankData.Take(5).ToList(),
                DashboardMapData = mapData.ToList(),
                IsSearch = true
            };
            return View(showData);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult DashBoard(DashboardQueryModel req)
        {
            Expression<Func<doctorSchedule, bool>> filter = x => !x.Status.Contains("Cancel");
            if (req.StartDate.HasValue) filter = filter.And(x => x.ReserveDatetime >= req.StartDate);
            if (req.EndDate.HasValue) filter = filter.And(x => x.ReserveDatetime.Date <= req.EndDate);
            if (!string.IsNullOrEmpty(req.Zip)) filter = filter.And(x => x.Zip == req.Zip);
            if (!string.IsNullOrEmpty(req.CropType)) filter = filter.And(x => x.CropType == req.CropType);
            if (!string.IsNullOrEmpty(req.Crops)) filter = filter.And(x => x.CropName == req.Crops);
            var data = Service_doctorSchedule.GetList(filter);

            #region 地圖
            var mapData = from ds in data
                          join ll in GetZipLngLat()
                          on ds.Zip equals ll.Zip
                          group new { ds, ll } by new { ds.Zip, ds.District, ll.Lat, ll.Lng } into gp
                          select new DashboardMap
                          {
                              Lat = gp.Key.Lat,
                              Lng = gp.Key.Lng,
                              PopupWord = $"高雄市{gp.Key.District}<br/>" + string.Join("<br/>", gp.GroupBy(x => x.ds.CropName).Select(x => x.Key + $" {x.Count()}件")),
                          };
            var _center = "";
            if (!string.IsNullOrEmpty(req.Zip))
            {
                var district = GetZipLngLat().FirstOrDefault(x => x.Zip == req.Zip);
                if (district != null)
                {
                    _center = $"{district.Lat},{district.Lng}";
                }
            }
            #endregion
            #region 折線圖
            var statisticData = data.GroupBy(x => new { Date = new DateTime(x.ReserveDatetime.Year, x.ReserveDatetime.Month, 1) }).Select(x => new
            {
                x.Key.Date,
                DataCount = x.Count()
            }).OrderBy(x => x.Date);
            #endregion
            #region 作物分類Top5
            var rankData = data.GroupBy(x => x.CropName).Select(x => new CropRank
            {
                Rank = 0,
                CropName = x.Key,
                DataCount = x.Count()
            }).OrderByDescending(x => x.DataCount).ToList();
            int lastCount = 0, lastRank = 0;
            foreach (var item in rankData)
            {
                if (lastCount == 0)
                {
                    lastCount = item.DataCount;
                    lastRank = 1;
                    item.Rank = lastRank;
                }
                else
                {
                    if (lastCount > item.DataCount)
                    {
                        lastCount = item.DataCount;
                        lastRank += 1;
                    }
                    item.Rank = lastRank;
                }
            }
            #endregion

            var showData = new DashboardModel()
            {
                QueryData = req,
                DataCount = statisticData.Select(x => x.DataCount).ToList(),
                Label = statisticData.Select(x => x.Date.ToString("yyyy-MM-dd")).ToList(),
                CropRanks = rankData.Take(5).ToList(),
                DashboardMapData = mapData.ToList(),
                Center = _center,
                IsSearch = true
            };
            return View(showData);
        }
        public List<ZipLngLat> GetZipLngLat()
        {
            var data = new List<ZipLngLat>() {
                new ZipLngLat{Zip ="800",Lat= (decimal)22.62992906,Lng=(decimal)120.3067337 },
                new ZipLngLat{Zip ="801",Lat= (decimal) 22.6269905, Lng=(decimal)120.2944217},
                new ZipLngLat{Zip ="802",Lat= (decimal) 22.62359448,Lng=(decimal)120.3209103},
                new ZipLngLat{Zip ="803",Lat= (decimal) 22.62424585,Lng=(decimal)120.2842331},
                new ZipLngLat{Zip ="804",Lat= (decimal) 22.65019525,Lng=(decimal)120.274163},
                new ZipLngLat{Zip ="805",Lat= (decimal) 22.58565583,Lng=(decimal)120.2891539},
                new ZipLngLat{Zip ="806",Lat= (decimal) 22.59269724,Lng=(decimal)120.3146749},
                new ZipLngLat{Zip ="807",Lat= (decimal) 22.64989883,Lng=(decimal)120.3179187},
                new ZipLngLat{Zip ="811",Lat= (decimal) 22.72109961,Lng=(decimal)120.300758},
                new ZipLngLat{Zip ="812",Lat= (decimal) 22.55140207,Lng=(decimal)120.3592605},
                new ZipLngLat{Zip ="813",Lat= (decimal) 22.68395699,Lng=(decimal)120.2951588},
                new ZipLngLat{Zip ="814",Lat= (decimal) 22.70120782,Lng=(decimal)120.3605265},
                new ZipLngLat{Zip ="815",Lat= (decimal) 22.73983479,Lng=(decimal)120.3707994},
                new ZipLngLat{Zip ="820",Lat= (decimal) 22.80505886,Lng=(decimal)120.2978906},
                new ZipLngLat{Zip ="821",Lat= (decimal) 22.85724171,Lng=(decimal)120.2659871},
                new ZipLngLat{Zip ="822",Lat= (decimal) 22.87022883,Lng=(decimal)120.3210967},
                new ZipLngLat{Zip ="823",Lat= (decimal) 22.86394307,Lng=(decimal)120.3959842},
                new ZipLngLat{Zip ="824",Lat= (decimal) 22.78769626,Lng=(decimal)120.370799},
                new ZipLngLat{Zip ="825",Lat= (decimal) 22.75252398,Lng=(decimal)120.3006534},
                new ZipLngLat{Zip ="826",Lat= (decimal) 22.748209,  Lng=(decimal)120.2593989},
                new ZipLngLat{Zip ="827",Lat= (decimal) 22.77944528,Lng=(decimal)120.2394571},
                new ZipLngLat{Zip ="828",Lat= (decimal) 22.82224585,Lng=(decimal)120.228051},
                new ZipLngLat{Zip ="829",Lat= (decimal) 22.89324952,Lng=(decimal)120.2259375},
                new ZipLngLat{Zip ="830",Lat= (decimal) 22.61379251,Lng=(decimal)120.3554359},
                new ZipLngLat{Zip ="831",Lat= (decimal) 22.59283576,Lng=(decimal)120.4111468},
                new ZipLngLat{Zip ="832",Lat= (decimal) 22.50813743,Lng=(decimal)120.399052},
                new ZipLngLat{Zip ="833",Lat= (decimal) 22.66249302,Lng=(decimal)120.3727783},
                new ZipLngLat{Zip ="840",Lat= (decimal) 22.71100364,Lng=(decimal)120.425407},
                new ZipLngLat{Zip ="842",Lat= (decimal) 22.86497033,Lng=(decimal)120.4754554},
                new ZipLngLat{Zip ="843",Lat= (decimal) 22.90005529,Lng=(decimal)120.5634635},
                new ZipLngLat{Zip ="844",Lat= (decimal) 23.01195426,Lng=(decimal)120.6585635},
                new ZipLngLat{Zip ="845",Lat= (decimal) 22.95668817,Lng=(decimal)120.4719272},
                new ZipLngLat{Zip ="846",Lat= (decimal) 22.99694681,Lng=(decimal)120.5621971},
                new ZipLngLat{Zip ="847",Lat= (decimal) 23.11654995,Lng=(decimal)120.6232895},
                new ZipLngLat{Zip ="848",Lat= (decimal) 23.2249459, Lng=(decimal)120.8523383},
                new ZipLngLat{Zip ="849",Lat= (decimal) 23.275008,  Lng=(decimal)120.741944},
                new ZipLngLat{Zip ="851",Lat= (decimal) 22.91993256,Lng=(decimal)120.752384},
                new ZipLngLat{Zip ="852",Lat= (decimal) 22.88241399,Lng=(decimal)120.1980519},
            };
            return data;
        }
        //農藥資訊查詢
        public ActionResult PesticidesInfo()
        {
            var _response = new PesticidesInfoShowModel()
            {
                QueryModel = new PesticidesInfoQuery(),
                DataModel = new List<PesticideInfoModel>(),
            };

            return View(_response);
        }
        //農藥資訊查詢
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> PesticidesInfo(PesticidesInfoQuery req)
        {
            List<PesticideInfoModel> dataList = new List<PesticideInfoModel>();
            var _response = new PesticidesInfoShowModel();

            int? mId = null;
            if (Session["MemberID"] == null)
            {
                _response.IsLogin = false;
            }
            else
            {
                _response.IsLogin = true;
                mId = Convert.ToInt32(Session["MemberID"].ToString());
            }


            if (!string.IsNullOrEmpty(req.Permit) || !string.IsNullOrEmpty(req.ChineseName) || !string.IsNullOrEmpty(req.CompanyName))
            {
                using (var client = new HttpClient())
                {
                    client.DefaultRequestHeaders.Accept.Clear();
                    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                    var filterString = new List<string>();
                    if (!string.IsNullOrEmpty(req.Permit)) filterString.Add($"許可證號+like+{req.Permit}");
                    if (!string.IsNullOrEmpty(req.ChineseName)) filterString.Add($"中文名稱+like+{req.ChineseName}");
                    if (!string.IsNullOrEmpty(req.CompanyName)) filterString.Add($"廠商名稱+like+{req.CompanyName}");
                    var _url = @"https://data.coa.gov.tw/Service/OpenData/FromM/PesticideData.aspx?$top=30&$skip=0&$filter=" + HttpUtility.UrlEncode(string.Join("and", filterString));

                    var res = await client.GetAsync(new Uri(_url));
                    JArray data = JArray.Parse(await res.Content.ReadAsStringAsync());

                    var checkData = new List<memberInventory>();
                    if (_response.IsLogin && mId.HasValue)
                    {
                        checkData = Service_memberInventory.GetList(x => x.MemberId == (mId ?? 0));
                    }
                    foreach (var d in data)
                    {
                        JObject obj = JObject.Parse(d.ToString());
                        var newData = new PesticideInfoModel()
                        {
                            LicenseWord = obj["許可證字"].ToString(),
                            LicenseNumber = obj["許可證號"].ToString(),
                            ChineseName = obj["中文名稱"].ToString(),
                            PesticideCode = obj["農藥代號"].ToString(),
                            EnglishName = obj["英文名稱"].ToString(),
                            BrandName = obj["廠牌名稱"].ToString(),
                            ChemicalComposition = obj["化學成分"].ToString(),
                            Manufacturer = obj["國外原製造廠商"].ToString(),
                            DosageForm = obj["劑型"].ToString(),
                            Content = obj["含量"].ToString(),
                            ValidityPeriod = obj["有效期限"].ToString(),
                            TradeName = obj["廠商名稱"].ToString(),
                            FRACFungicideResistance = obj["FRAC殺菌劑抗藥性"].ToString(),
                            HRACHerbicideResistance = obj["HRAC除草劑抗藥性"].ToString(),
                            IRACInsecticideResistance = obj["IRAC殺蟲劑抗藥性"].ToString(),
                            PesticideApplicationRange = obj["農藥使用範圍"].ToString(),
                            LicenseMarking = obj["許可證標示"].ToString(),
                        };
                        if (checkData.Any(x => x.LicenseNumber == newData.LicenseNumber)) newData.IsAlready = true;
                        dataList.Add(newData);
                    }
                }
            }

            _response.QueryModel = req;
            _response.DataModel = dataList;

            return View(_response);
        }

        [HttpPost] //刪除方法為POST
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> InventoryAdd(string LicenseWord, string LicenseNumber)
        {
            if (Session["MemberID"] == null)
            {
                return RedirectToAction("Index", "Home");
            }
            var mId = Convert.ToInt32(Session["MemberID"].ToString());
            var checkData = Service_memberInventory.GetList(x => x.MemberId == mId);
            if (checkData.Any(x => x.LicenseNumber == LicenseNumber))
            {
                return RedirectToAction("InventoryList", "Member");
            }
            List<PesticideInfoModel> dataList = new List<PesticideInfoModel>();
            using (var client = new HttpClient())
            {
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                var filterString = new List<string>();
                filterString.Add($"許可證號+like+{LicenseNumber}");
                var _url = @"https://data.coa.gov.tw/Service/OpenData/FromM/PesticideData.aspx?$top=30&$skip=0&$filter=" + HttpUtility.UrlEncode(string.Join("and", filterString));

                var res = await client.GetAsync(new Uri(_url));
                JArray data = JArray.Parse(await res.Content.ReadAsStringAsync());

                foreach (var d in data)
                {
                    JObject obj = JObject.Parse(d.ToString());
                    var newData = new PesticideInfoModel()
                    {
                        LicenseWord = obj["許可證字"].ToString(),
                        LicenseNumber = obj["許可證號"].ToString(),
                        ChineseName = obj["中文名稱"].ToString(),
                        PesticideCode = obj["農藥代號"].ToString(),
                        EnglishName = obj["英文名稱"].ToString(),
                        BrandName = obj["廠牌名稱"].ToString(),
                        ChemicalComposition = obj["化學成分"].ToString(),
                        Manufacturer = obj["國外原製造廠商"].ToString(),
                        DosageForm = obj["劑型"].ToString(),
                        Content = obj["含量"].ToString(),
                        ValidityPeriod = obj["有效期限"].ToString(),
                        TradeName = obj["廠商名稱"].ToString(),
                        FRACFungicideResistance = obj["FRAC殺菌劑抗藥性"].ToString(),
                        HRACHerbicideResistance = obj["HRAC除草劑抗藥性"].ToString(),
                        IRACInsecticideResistance = obj["IRAC殺蟲劑抗藥性"].ToString(),
                        PesticideApplicationRange = obj["農藥使用範圍"].ToString(),
                        LicenseMarking = obj["許可證標示"].ToString(),
                    };
                    dataList.Add(newData);
                }
            }
            var thisData = dataList.FirstOrDefault(x => x.LicenseNumber == LicenseNumber && x.LicenseWord == LicenseWord);
            if (thisData != null)
            {
                var newInventory = new memberInventory();
                newInventory.MemberId = mId;
                newInventory.LicenseWord = thisData.LicenseWord;
                newInventory.LicenseNumber = thisData.LicenseNumber;
                newInventory.ChineseName = thisData.ChineseName;
                newInventory.Contents = thisData.Content;

                newInventory.DosageForm = thisData.DosageForm;
                newInventory.BrandName = thisData.BrandName;
                newInventory.TradeName = thisData.TradeName;
                newInventory.ValidityPeriod = thisData.ValidityPeriod;
                newInventory.CreateDate = DateTime.Now;
                newInventory.ModifyDate = DateTime.Now;
                Service_memberInventory.Insert(newInventory);
            }

            return RedirectToAction("InventoryList", "Member");
        }
        //農藥使用範圍
        public async Task<ActionResult> PesticidesInventoryInfo(string id = "")
        {
            var _response = new PesticidesInventoryInfoShowModel()
            {
                PesticideInfos = new PesticideInfoModel(),
                UseScopes = new List<UseScope>(),
            };
            var getPesticideInfo = new List<PesticideInfoModel>();
            if (!string.IsNullOrWhiteSpace(id))
            {
                using (var client = new HttpClient())
                {
                    client.DefaultRequestHeaders.Accept.Clear();
                    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                    var _url = @"https://data.coa.gov.tw/Service/OpenData/FromM/PesticideData.aspx?$top=1&$filter=" + HttpUtility.UrlEncode("許可證號+like+") + id;

                    var res = await client.GetAsync(new Uri(_url));
                    JArray data = JArray.Parse(await res.Content.ReadAsStringAsync());

                    foreach (var d in data)
                    {
                        JObject obj = JObject.Parse(d.ToString());
                        getPesticideInfo.Add(new PesticideInfoModel()
                        {
                            LicenseWord = obj["許可證字"].ToString(),
                            LicenseNumber = obj["許可證號"].ToString(),
                            ChineseName = obj["中文名稱"].ToString(),
                            PesticideCode = obj["農藥代號"].ToString(),
                            EnglishName = obj["英文名稱"].ToString(),
                            BrandName = obj["廠牌名稱"].ToString(),
                            ChemicalComposition = obj["化學成分"].ToString(),
                            Manufacturer = obj["國外原製造廠商"].ToString(),
                            DosageForm = obj["劑型"].ToString(),
                            Content = obj["含量"].ToString(),
                            ValidityPeriod = obj["有效期限"].ToString(),
                            TradeName = obj["廠商名稱"].ToString(),
                            FRACFungicideResistance = obj["FRAC殺菌劑抗藥性"].ToString(),
                            HRACHerbicideResistance = obj["HRAC除草劑抗藥性"].ToString(),
                            IRACInsecticideResistance = obj["IRAC殺蟲劑抗藥性"].ToString(),
                            PesticideApplicationRange = obj["農藥使用範圍"].ToString(),
                            LicenseMarking = obj["許可證標示"].ToString(),
                        });
                    }
                }
                if (getPesticideInfo.Any())
                {
                    _response.PesticideInfos = getPesticideInfo.FirstOrDefault();
                    using (var client = new HttpClient())
                    {
                        client.DefaultRequestHeaders.Accept.Clear();
                        client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                        var _url = _response.PesticideInfos.PesticideApplicationRange;

                        var res = await client.GetAsync(new Uri(_url));
                        JArray data = JArray.Parse(await res.Content.ReadAsStringAsync());

                        foreach (var d in data)
                        {
                            JObject obj = JObject.Parse(d.ToString());
                            _response.UseScopes.Add(new UseScope()
                            {
                                CropName = obj["作物名稱"].ToString(),
                                PestName = obj["病蟲害名稱"].ToString(),
                                UseCount = obj["施用次數"].ToString(),
                                PerHectares = obj["每公頃使用用藥量"].ToString(),
                                DilutionMultiple = obj["稀釋倍數"].ToString(),
                                UseTime = obj["使用時期"].ToString(),
                                UseInterval = obj["施藥間隔"].ToString(),
                                SaveHarvest = obj["安全採收期"].ToString(),
                                Remark = obj["備註"].ToString(),
                                AttentionItem = obj["注意事項"].ToString(),
                            });
                        }
                    }
                }
                else
                {
                    return RedirectToAction(nameof(PesticidesInfo));
                }
            }

            return View(_response);
        }
    }
}