using ICCModule;
using ICCModule.ActionFilters;
using ICCModule.Entity.Tables.Platform;
using ICCModule.EntityService.Service;
using ICCModule.Helper;
using InformationSystem.ViewModel;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Linq.Expressions;
using System.Web;
using System.Web.Mvc;
using InformationSystem.Models.Statistic.StatModel;
using ICCModule.Models;

namespace InformationSystem.Controllers
{
    [LoginCheckFilter]//掛載此Filter，如果沒登入，就導去登入頁
    [InterceptorOfController]//系統操作Log
    public class StatisticController : Controller
    {
        //診斷/作物類別統計
        public ActionResult Diagnose()
        {
            return View(new DoctorCropStatisticModel());
        }

        //診斷/作物類別統計
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Diagnose(DoctorCropStatisticQueryModel req)
        {
            var _res = new DoctorCropStatisticModel();

            _res.QueryReq = req;

            Expression<Func<doctorSchedule, bool>> filter = x => true;
            if (req.StartDate.HasValue) filter = filter.And(x => x.ReserveDatetime >= req.StartDate);
            if (req.EndDate.HasValue) filter = filter.And(x => x.ReserveDatetime.Date <= req.EndDate);
            if (!string.IsNullOrEmpty(req.CropType)) filter = x => x.CropType == req.CropType;
            var getUserId = SessionHelper.Get("LoginID");
            if (!string.IsNullOrEmpty(getUserId))
            {
                var thisUser = Service_sysUserInfo.GetDetail(getUserId);
                if (thisUser!=null)
                {
                    if (thisUser.RoleID == "R08")
                    {
                        filter = filter.And(x => x.LoginID == getUserId);
                    }
                }
            }
            filter = filter.And(x => x.Origin != "PlantDoctor_Return");

            var sameYear = (req.StartDate.HasValue && req.EndDate.HasValue && req.StartDate.Value.Year == req.EndDate.Value.Year);
            var data = new List<DoctorCropStatisticDataModel>();
            var exportName = "";

            if (req.QueryType == 1)
            {
                data = Service_doctorSchedule.GetList(filter).GroupBy(x => new { x.CropName, x.ReserveDatetime.Year }).Select(x => new DoctorCropStatisticDataModel
                {
                    Title = x.Key.CropName + (sameYear ? "" : $"({x.Key.Year})"),
                    Jan = x.Where(o => o.ReserveDatetime.Month == 1).Count(),
                    Feb = x.Where(o => o.ReserveDatetime.Month == 2).Count(),
                    Mar = x.Where(o => o.ReserveDatetime.Month == 3).Count(),
                    Apr = x.Where(o => o.ReserveDatetime.Month == 4).Count(),
                    May = x.Where(o => o.ReserveDatetime.Month == 5).Count(),
                    Jun = x.Where(o => o.ReserveDatetime.Month == 6).Count(),
                    Jul = x.Where(o => o.ReserveDatetime.Month == 7).Count(),
                    Aug = x.Where(o => o.ReserveDatetime.Month == 8).Count(),
                    Sep = x.Where(o => o.ReserveDatetime.Month == 9).Count(),
                    Oct = x.Where(o => o.ReserveDatetime.Month == 10).Count(),
                    Nov = x.Where(o => o.ReserveDatetime.Month == 11).Count(),
                    Dec = x.Where(o => o.ReserveDatetime.Month == 12).Count(),
                    Total = x.Count(),
                }).ToList();
                exportName = "診斷作物統計表";
            }
            else if (req.QueryType == 2)
            {
                data = Service_doctorSchedule.GetList(filter).GroupBy(x => new { x.CropName, x.ReserveDatetime.Year }).Select(x => new DoctorCropStatisticDataModel
                {
                    Title = x.Key.CropName + (sameYear ? "" : $"({x.Key.Year})"),
                    Jan = x.Where(o => o.ReserveDatetime.Month == 1).GroupBy(o => new { o.MemberInfoID, o.Name }).Sum(o => (decimal)o.Average(p => p.PlantingArea)),
                    Feb = x.Where(o => o.ReserveDatetime.Month == 2).GroupBy(o => new { o.MemberInfoID, o.Name }).Sum(o => (decimal)o.Average(p => p.PlantingArea)),
                    Mar = x.Where(o => o.ReserveDatetime.Month == 3).GroupBy(o => new { o.MemberInfoID, o.Name }).Sum(o => (decimal)o.Average(p => p.PlantingArea)),
                    Apr = x.Where(o => o.ReserveDatetime.Month == 4).GroupBy(o => new { o.MemberInfoID, o.Name }).Sum(o => (decimal)o.Average(p => p.PlantingArea)),
                    May = x.Where(o => o.ReserveDatetime.Month == 5).GroupBy(o => new { o.MemberInfoID, o.Name }).Sum(o => (decimal)o.Average(p => p.PlantingArea)),
                    Jun = x.Where(o => o.ReserveDatetime.Month == 6).GroupBy(o => new { o.MemberInfoID, o.Name }).Sum(o => (decimal)o.Average(p => p.PlantingArea)),
                    Jul = x.Where(o => o.ReserveDatetime.Month == 7).GroupBy(o => new { o.MemberInfoID, o.Name }).Sum(o => (decimal)o.Average(p => p.PlantingArea)),
                    Aug = x.Where(o => o.ReserveDatetime.Month == 8).GroupBy(o => new { o.MemberInfoID, o.Name }).Sum(o => (decimal)o.Average(p => p.PlantingArea)),
                    Sep = x.Where(o => o.ReserveDatetime.Month == 9).GroupBy(o => new { o.MemberInfoID, o.Name }).Sum(o => (decimal)o.Average(p => p.PlantingArea)),
                    Oct = x.Where(o => o.ReserveDatetime.Month == 10).GroupBy(o => new { o.MemberInfoID, o.Name }).Sum(o => (decimal)o.Average(p => p.PlantingArea)),
                    Nov = x.Where(o => o.ReserveDatetime.Month == 11).GroupBy(o => new { o.MemberInfoID, o.Name }).Sum(o => (decimal)o.Average(p => p.PlantingArea)),
                    Dec = x.Where(o => o.ReserveDatetime.Month == 12).GroupBy(o => new { o.MemberInfoID, o.Name }).Sum(o => (decimal)o.Average(p => p.PlantingArea)),
                    Total = x.Sum(o => (decimal)o.PlantingArea),
                }).ToList();
                exportName = "診斷作物面積統計表";
            }
            else if (req.QueryType == 3)
            {
                data = (from ds in Service_doctorSchedule.GetList(filter)
                    join dso in Service_defCode.GetList("PlantDoctorInquiry")
                        on ds.Inquiry equals dso.Code
                    group ds by new { dso, ds.ReserveDatetime.Year }
                    into gp
                    select new DoctorCropStatisticDataModel
                    {
                        Title = gp.Key.dso.Name + (sameYear ? "" : $"({gp.Key.Year})"),
                        Jan = gp.Where(o => o.ReserveDatetime.Month == 1).Count(),
                        Feb = gp.Where(o => o.ReserveDatetime.Month == 2).Count(),
                        Mar = gp.Where(o => o.ReserveDatetime.Month == 3).Count(),
                        Apr = gp.Where(o => o.ReserveDatetime.Month == 4).Count(),
                        May = gp.Where(o => o.ReserveDatetime.Month == 5).Count(),
                        Jun = gp.Where(o => o.ReserveDatetime.Month == 6).Count(),
                        Jul = gp.Where(o => o.ReserveDatetime.Month == 7).Count(),
                        Aug = gp.Where(o => o.ReserveDatetime.Month == 8).Count(),
                        Sep = gp.Where(o => o.ReserveDatetime.Month == 9).Count(),
                        Oct = gp.Where(o => o.ReserveDatetime.Month == 10).Count(),
                        Nov = gp.Where(o => o.ReserveDatetime.Month == 11).Count(),
                        Dec = gp.Where(o => o.ReserveDatetime.Month == 12).Count(),
                        Total = gp.Count(),
                    }).ToList();
                exportName = "問診方式次數統計表";
            }
            else if (req.QueryType == 4)
            {
                filter = filter.And(x => x.Status == "Close");
                var memberGroup = Service_doctorSchedule.GetList(filter).GroupBy(x =>
                    new { x.MemberInfoID, x.Name, x.ReserveDatetime.Month, x.ReserveDatetime.Year });
                var maxTimes = 0;
                if (memberGroup.Any())
                {
                    maxTimes = memberGroup.OrderByDescending(x => x.Count()).FirstOrDefault().Count();
                }

                foreach (var item in memberGroup.Select(x => x.Key.Year).Distinct().ToList())
                {
                    for (var i = 1; i <= maxTimes; i++)
                    {
                        data.Add(new DoctorCropStatisticDataModel
                        {
                            Title = $"問診{i}次" + (sameYear ? "" : $"({item})"),
                            Jan = memberGroup.Where(o => o.Key.Month == 1 && o.Key.Year == item && o.Count() == i).Count(),
                            Feb = memberGroup.Where(o => o.Key.Month == 2 && o.Key.Year == item && o.Count() == i).Count(),
                            Mar = memberGroup.Where(o => o.Key.Month == 3 && o.Key.Year == item && o.Count() == i).Count(),
                            Apr = memberGroup.Where(o => o.Key.Month == 4 && o.Key.Year == item && o.Count() == i).Count(),
                            May = memberGroup.Where(o => o.Key.Month == 5 && o.Key.Year == item && o.Count() == i).Count(),
                            Jun = memberGroup.Where(o => o.Key.Month == 6 && o.Key.Year == item && o.Count() == i).Count(),
                            Jul = memberGroup.Where(o => o.Key.Month == 7 && o.Key.Year == item && o.Count() == i).Count(),
                            Aug = memberGroup.Where(o => o.Key.Month == 8 && o.Key.Year == item && o.Count() == i).Count(),
                            Sep = memberGroup.Where(o => o.Key.Month == 9 && o.Key.Year == item && o.Count() == i).Count(),
                            Oct = memberGroup.Where(o => o.Key.Month == 10 && o.Key.Year == item && o.Count() == i).Count(),
                            Nov = memberGroup.Where(o => o.Key.Month == 11 && o.Key.Year == item && o.Count() == i).Count(),
                            Dec = memberGroup.Where(o => o.Key.Month == 12 && o.Key.Year == item && o.Count() == i).Count(),
                            Total = memberGroup.Where(o => o.Key.Year == item && o.Count() == i).Count(),
                        });
                    }
                }
                exportName = "問診次數統計表";
            }
            else if (req.QueryType == 5)
            {
                filter = filter.And(x => x.Status == "Close");
                var memberGroup = Service_doctorSchedule.GetList(filter).GroupBy(x => new { x.MemberInfoID, x.Name, x.ReserveDatetime.Year });
                var maxTimes = 0;
                if (memberGroup.Any())
                {
                    maxTimes = memberGroup.OrderByDescending(x => x.Count()).FirstOrDefault().Count();
                }
                DataTable table = new DataTable();
                table.Columns.Add("問診次數");
                foreach (var item in memberGroup.Select(x => x.Key.Year).Distinct().ToList())
                {
                    table.Columns.Add($"{item}年");
                }
                for (var i = 1; i <= maxTimes; i++)
                {
                    var newRow = table.NewRow();
                    newRow["問診次數"] = $"問診{i}次";
                    foreach (var item in memberGroup.Select(x => x.Key.Year).Distinct().ToList())
                    {
                        newRow[$"{item}年"] = memberGroup.Where(o => o.Key.Year == item && o.Count() == i).Count();
                    }
                    table.Rows.Add(newRow);
                }
                _res.DataYear = table;

                if (req.ActionName == "export")
                {
                    return File(ExcelHelper.RenderDataTableToExcel(table), "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", $"年度問診次數統計表_{DateTime.Now.ToString("yyyyMMddHHmmss")}.xlsx");
                }
            }

            if (req.ActionName == "export")
            {
                var _export = data.Select(x => new
                {
                    作物 = x.Title,
                    一月 = x.Jan,
                    二月 = x.Feb,
                    三月 = x.Mar,
                    四月 = x.Apr,
                    五月 = x.May,
                    六月 = x.Jun,
                    七月 = x.Jul,
                    八月 = x.Aug,
                    九月 = x.Sep,
                    十月 = x.Oct,
                    十一月 = x.Nov,
                    十二月 = x.Dec,
                    合計 = x.Total,
                }).ToList();

                return File(ExcelHelper.RenderDataTableToExcel(ExcelHelper.ConvertToDataTable(_export)),
                    "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                    $"{exportName}_{DateTime.Now.ToString("yyyyMMddHHmmss")}.xlsx");
            }

            _res.DataList = data;
            return View(_res);
        }

        public List<string> GetBackgroundColor(int c)
        {
            var _res = new List<string>();

            while (_res.Count < c)
            {
                var newColor = "#";
                for (int j = 0; j < 3; j++)
                {
                    Random rnd = new Random();
                    var colorInt = (rnd.Next(255) * (j + 1)) % 255;
                    newColor += colorInt.ToString("X");
                }
                if (!_res.Any(x => x.Contains(newColor))) _res.Add(newColor);
            }

            return _res;
        }
        //調查次數/點位
        public ActionResult InvestCount()
        {
            return View(new InvestCountStatisticShowModel());
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult InvestCount(DateTime? StartDate, DateTime? EndDate, string ActionName, int QueryType = 1)
        {
            Expression<Func<monitorAreaPointDate, bool>> filter = x => true;
            if (StartDate.HasValue) filter = filter.And(x => x.Date >= StartDate);
            if (EndDate.HasValue) filter = filter.And(x => x.Date.Date <= EndDate);

            var sameYear = (StartDate.HasValue && EndDate.HasValue && StartDate.Value.Year == EndDate.Value.Year);
            var _res = new List<InvestCountStatisticModel>();
            if (QueryType == 1)
            {
                _res = (from mapd in Service_monitorAreaPointDate.GetList(filter)
                        join map in Service_monitorAreaPoint.GetList(x => true)
                        on mapd.AreaPointID equals map.ID
                        join mpa in Service_monitorProjectArea.GetList(x => true)
                        on map.ProjectAreaID equals mpa.ID
                        join mp in Service_monitorProject.GetList(x => true)
                        on mpa.ProjectID equals mp.ID
                        select new
                        {
                            mp.Name,
                            mapd.Date
                        }).GroupBy(x => new { x.Name, x.Date.Year }).Select(x => new InvestCountStatisticModel
                        {
                            Title = x.Key.Name + (sameYear ? "" : $"({x.Key.Year})"),
                            Jan = x.Where(o => o.Date.Month == 1).Count(),
                            Feb = x.Where(o => o.Date.Month == 2).Count(),
                            Mar = x.Where(o => o.Date.Month == 3).Count(),
                            Apr = x.Where(o => o.Date.Month == 4).Count(),
                            May = x.Where(o => o.Date.Month == 5).Count(),
                            Jun = x.Where(o => o.Date.Month == 6).Count(),
                            Jul = x.Where(o => o.Date.Month == 7).Count(),
                            Aug = x.Where(o => o.Date.Month == 8).Count(),
                            Sep = x.Where(o => o.Date.Month == 9).Count(),
                            Oct = x.Where(o => o.Date.Month == 10).Count(),
                            Nov = x.Where(o => o.Date.Month == 11).Count(),
                            Dec = x.Where(o => o.Date.Month == 12).Count(),
                            Total = x.Count(),
                        }).ToList();
            }
            else
            {
                _res = (from mapd in Service_monitorAreaPointDate.GetList(filter)
                        join map in Service_monitorAreaPoint.GetList(x => true)
                        on mapd.AreaPointID equals map.ID
                        join mpa in Service_monitorProjectArea.GetList(x => true)
                        on map.ProjectAreaID equals mpa.ID
                        join mp in Service_monitorProject.GetList(x => true)
                        on mpa.ProjectID equals mp.ID
                        select new
                        {
                            mp.Name,
                            mapd.Date,
                            map.NorthLatitude,
                            map.EastLongitude
                        }).AsEnumerable().GroupBy(x => new { x.Name, x.Date.Year }).Select(x => new InvestCountStatisticModel
                        {
                            Title = x.Key.Name + (sameYear ? "" : $"({x.Key.Year})"),
                            Jan = x.Where(o => o.Date.Month == 1).GroupBy(o => new { NorthLatitude = o.NorthLatitude.ToString("0.###"), EastLongitude = o.EastLongitude.ToString("0.###") }).Count(),
                            Feb = x.Where(o => o.Date.Month == 2).GroupBy(o => new { NorthLatitude = o.NorthLatitude.ToString("0.###"), EastLongitude = o.EastLongitude.ToString("0.###") }).Count(),
                            Mar = x.Where(o => o.Date.Month == 3).GroupBy(o => new { NorthLatitude = o.NorthLatitude.ToString("0.###"), EastLongitude = o.EastLongitude.ToString("0.###") }).Count(),
                            Apr = x.Where(o => o.Date.Month == 4).GroupBy(o => new { NorthLatitude = o.NorthLatitude.ToString("0.###"), EastLongitude = o.EastLongitude.ToString("0.###") }).Count(),
                            May = x.Where(o => o.Date.Month == 5).GroupBy(o => new { NorthLatitude = o.NorthLatitude.ToString("0.###"), EastLongitude = o.EastLongitude.ToString("0.###") }).Count(),
                            Jun = x.Where(o => o.Date.Month == 6).GroupBy(o => new { NorthLatitude = o.NorthLatitude.ToString("0.###"), EastLongitude = o.EastLongitude.ToString("0.###") }).Count(),
                            Jul = x.Where(o => o.Date.Month == 7).GroupBy(o => new { NorthLatitude = o.NorthLatitude.ToString("0.###"), EastLongitude = o.EastLongitude.ToString("0.###") }).Count(),
                            Aug = x.Where(o => o.Date.Month == 8).GroupBy(o => new { NorthLatitude = o.NorthLatitude.ToString("0.###"), EastLongitude = o.EastLongitude.ToString("0.###") }).Count(),
                            Sep = x.Where(o => o.Date.Month == 9).GroupBy(o => new { NorthLatitude = o.NorthLatitude.ToString("0.###"), EastLongitude = o.EastLongitude.ToString("0.###") }).Count(),
                            Oct = x.Where(o => o.Date.Month == 10).GroupBy(o => new { NorthLatitude = o.NorthLatitude.ToString("0.###"), EastLongitude = o.EastLongitude.ToString("0.###") }).Count(),
                            Nov = x.Where(o => o.Date.Month == 11).GroupBy(o => new { NorthLatitude = o.NorthLatitude.ToString("0.###"), EastLongitude = o.EastLongitude.ToString("0.###") }).Count(),
                            Dec = x.Where(o => o.Date.Month == 12).GroupBy(o => new { NorthLatitude = o.NorthLatitude.ToString("0.###"), EastLongitude = o.EastLongitude.ToString("0.###") }).Count(),
                            Total = x.Count(),
                        }).ToList();
            }

            if (ActionName == "export")
            {
                var _export = _res.Select(x => new
                {
                    專案名稱 = x.Title,
                    一月 = x.Jan,
                    二月 = x.Feb,
                    三月 = x.Mar,
                    四月 = x.Apr,
                    五月 = x.May,
                    六月 = x.Jun,
                    七月 = x.Jul,
                    八月 = x.Aug,
                    九月 = x.Sep,
                    十月 = x.Oct,
                    十一月 = x.Nov,
                    十二月 = x.Dec,
                    總計 = x.Total,
                }).ToList();

                if (QueryType == 1) return File(ExcelHelper.RenderDataTableToExcel(ExcelHelper.ConvertToDataTable(_export)), "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", $"調查次數統計_{DateTime.Now.ToString("yyyyMMddHHmmss")}.xlsx");
                else return File(ExcelHelper.RenderDataTableToExcel(ExcelHelper.ConvertToDataTable(_export)), "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", $"調查點位數統計_{DateTime.Now.ToString("yyyyMMddHHmmss")}.xlsx");
            }

            return View(new InvestCountStatisticShowModel()
            {
                QueryType = QueryType,
                StartDate = StartDate,
                EndDate = EndDate,
                Data = _res
            });
        }
        //琉璃蟻蟲數
        public ActionResult InvestAnt()
        {
            return View(new InvestAntStatisticShowModel());
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult InvestAnt(DateTime? StartDate, DateTime? EndDate, List<string> Distict, string ActionName = "")
        {
            var _res = new InvestAntStatisticShowModel()
            {
                StartDate = StartDate,
                EndDate = EndDate,
                Distict = Distict ?? new List<string>(),
            };
            if (Distict == null || !Distict.Any())
            {
                return View(_res);
            }

            var sameYear = (StartDate.HasValue && EndDate.HasValue && StartDate.Value.Year == EndDate.Value.Year);
            var data = (from mpa in Service_monitorProjectArea.GetList(x => Distict.Contains(x.Distist))
                        join map in Service_monitorAreaPoint.GetList(null)
                        on mpa.ID equals map.ProjectAreaID
                        join mapd in Service_monitorAreaPointDate.GetList(x => (!StartDate.HasValue || StartDate <= x.Date) && (!EndDate.HasValue || x.Date <= EndDate))
                        on map.ID equals mapd.AreaPointID
                        group new { mapd } by new { mpa.Distist, mapd.Date.Year } into gp
                        select new InvestAntStatisticModel
                        {
                            Title = gp.Key.Distist + (sameYear ? "" : $"({gp.Key.Year})"),
                            Jan = gp.Where(x => x.mapd.Date.Month == 1).Sum(x => x.mapd.Pests),
                            JanOther = gp.Where(x => x.mapd.Date.Month == 1).Sum(x => x.mapd.OtherArtsNum),
                            Feb = gp.Where(x => x.mapd.Date.Month == 2).Sum(x => x.mapd.Pests),
                            FebOther = gp.Where(x => x.mapd.Date.Month == 2).Sum(x => x.mapd.OtherArtsNum),
                            Mar = gp.Where(x => x.mapd.Date.Month == 3).Sum(x => x.mapd.Pests),
                            MarOther = gp.Where(x => x.mapd.Date.Month == 3).Sum(x => x.mapd.OtherArtsNum),
                            Apr = gp.Where(x => x.mapd.Date.Month == 4).Sum(x => x.mapd.Pests),
                            AprOther = gp.Where(x => x.mapd.Date.Month == 4).Sum(x => x.mapd.OtherArtsNum),
                            May = gp.Where(x => x.mapd.Date.Month == 5).Sum(x => x.mapd.Pests),
                            MayOther = gp.Where(x => x.mapd.Date.Month == 5).Sum(x => x.mapd.OtherArtsNum),
                            Jun = gp.Where(x => x.mapd.Date.Month == 6).Sum(x => x.mapd.Pests),
                            JunOther = gp.Where(x => x.mapd.Date.Month == 6).Sum(x => x.mapd.OtherArtsNum),
                            Jul = gp.Where(x => x.mapd.Date.Month == 7).Sum(x => x.mapd.Pests),
                            JulOther = gp.Where(x => x.mapd.Date.Month == 7).Sum(x => x.mapd.OtherArtsNum),
                            Aug = gp.Where(x => x.mapd.Date.Month == 8).Sum(x => x.mapd.Pests),
                            AugOther = gp.Where(x => x.mapd.Date.Month == 8).Sum(x => x.mapd.OtherArtsNum),
                            Sep = gp.Where(x => x.mapd.Date.Month == 9).Sum(x => x.mapd.Pests),
                            SepOther = gp.Where(x => x.mapd.Date.Month == 9).Sum(x => x.mapd.OtherArtsNum),
                            Oct = gp.Where(x => x.mapd.Date.Month == 10).Sum(x => x.mapd.Pests),
                            OctOther = gp.Where(x => x.mapd.Date.Month == 10).Sum(x => x.mapd.OtherArtsNum),
                            Nov = gp.Where(x => x.mapd.Date.Month == 11).Sum(x => x.mapd.Pests),
                            NovOther = gp.Where(x => x.mapd.Date.Month == 11).Sum(x => x.mapd.OtherArtsNum),
                            Dec = gp.Where(x => x.mapd.Date.Month == 12).Sum(x => x.mapd.Pests),
                            DecOther = gp.Where(x => x.mapd.Date.Month == 12).Sum(x => x.mapd.OtherArtsNum),
                        }).ToList();

            data.Add(new InvestAntStatisticModel
            {
                Title = "總計",
                Jan = data.Sum(x => x.Jan),
                JanOther = data.Sum(x => x.JanOther),
                Feb = data.Sum(x => x.Feb),
                FebOther = data.Sum(x => x.FebOther),
                Mar = data.Sum(x => x.Mar),
                MarOther = data.Sum(x => x.MarOther),
                Apr = data.Sum(x => x.Apr),
                AprOther = data.Sum(x => x.AprOther),
                May = data.Sum(x => x.May),
                MayOther = data.Sum(x => x.MayOther),
                Jun = data.Sum(x => x.Jun),
                JunOther = data.Sum(x => x.JunOther),
                Jul = data.Sum(x => x.Jul),
                JulOther = data.Sum(x => x.JulOther),
                Aug = data.Sum(x => x.Aug),
                AugOther = data.Sum(x => x.AugOther),
                Sep = data.Sum(x => x.Sep),
                SepOther = data.Sum(x => x.SepOther),
                Oct = data.Sum(x => x.Oct),
                OctOther = data.Sum(x => x.OctOther),
                Nov = data.Sum(x => x.Nov),
                NovOther = data.Sum(x => x.NovOther),
                Dec = data.Sum(x => x.Dec),
                DecOther = data.Sum(x => x.DecOther),
            });

            if (ActionName == "export")
            {
                var _export = data.Select(x => new
                {
                    地區 = x.Title,
                    一月琉璃蟻 = x.Jan,
                    一月其他蟻種 = x.JanOther,
                    二月琉璃蟻 = x.Feb,
                    二月其他蟻種 = x.FebOther,
                    三月琉璃蟻 = x.Mar,
                    三月其他蟻種 = x.MarOther,
                    四月琉璃蟻 = x.Apr,
                    四月其他蟻種 = x.AprOther,
                    五月琉璃蟻 = x.May,
                    五月其他蟻種 = x.MayOther,
                    六月琉璃蟻 = x.Jun,
                    六月其他蟻種 = x.JunOther,
                    七月琉璃蟻 = x.Jul,
                    七月其他蟻種 = x.JulOther,
                    八月琉璃蟻 = x.Aug,
                    八月其他蟻種 = x.AugOther,
                    九月琉璃蟻 = x.Sep,
                    九月其他蟻種 = x.SepOther,
                    十月琉璃蟻 = x.Oct,
                    十月其他蟻種 = x.OctOther,
                    十一月琉璃蟻 = x.Nov,
                    十一月其他蟻種 = x.NovOther,
                    十二月琉璃蟻 = x.Dec,
                    十二月其他蟻種 = x.DecOther,
                }).ToList();

                return File(ExcelHelper.RenderDataTableToExcel(ExcelHelper.ConvertToDataTable(_export)), "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", $"琉璃蟻統計_{DateTime.Now.ToString("yyyyMMddHHmmss")}.xlsx");
            }

            _res.Data = data;

            return View(_res);
        }
        //檢驗次數統計
        public ActionResult Inspection(InspectionStatisticQueryModel req)
        {
            return View(new InspectionStatisticShowModel());
        }
        //檢驗次數統計
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Inspection(InspectionStatisticQueryModel req, string act)
        {
            Expression<Func<pesticideResidueTesting, bool>> filter = x => true;
            if (req.StartDate.HasValue) filter = filter.And(x => x.SampleDate.Date >= req.StartDate);
            if (req.EndDate.HasValue) filter = filter.And(x => x.SampleDate.Date <= req.EndDate);
            if (req.PlanType.Any()) filter = filter.And(x => req.PlanType.Contains(x.PlanType));

            var _res = Service_pesticideResidueTesting.GetList(filter).GroupBy(x => x.SampleSource).Select(x => new InspectionStatisticModel
            {
                KeyName = x.Key,
                TotalCount = x.Count(),
                PassCount = x.Where(o => o.Result == true).Count(),
                NoCheckCount = x.Where(o => o.SampleResult == false).Count(),
                UnPassCount = x.Where(o => o.Result == false).Count(),
                OverApproved = x.Where(o => (o.SampleContext ?? "").Contains("核准使用")).Count(),
                UnApproved = x.Where(o => (o.SampleContext ?? "").Contains("未核准")).Count(),
                SameOverUnApproved = x.Where(o => (o.SampleContext ?? "").Contains("未核准") && o.SampleContext.Contains("不符合容許量")).Count(),
                DisableCount = x.Where(o => (o.SampleContext ?? "").Contains("禁用")).Count(),
            }).ToList();

            var getSampleSource = Service_defCode.GetList("SampleSource");
            foreach (var item in _res)
            {
                if (getSampleSource.Any(x => x.Code == item.KeyName))
                {
                    item.KeyName = getSampleSource.FirstOrDefault(x => x.Code == item.KeyName).Name;
                }
            }

            var _response = new InspectionStatisticShowModel
            {
                QueryReq = req,
                Data = _res
            };

            #region 匯出

            if (act == "export")
            {
                var _export = _res.Select(x => new
                {
                    採樣來源 = x.KeyName,
                    採樣數 = x.TotalCount,
                    合格數 = x.PassCount,
                    未檢出數 = x.NoCheckCount,
                    不合格數 = x.UnPassCount,
                    超量核准 = x.OverApproved,
                    未核准 = x.UnApproved,
                    同時超量未核准 = x.SameOverUnApproved,
                    禁用 = x.DisableCount
                }).ToList();

                return File(ExcelHelper.RenderDataTableToExcel(ExcelHelper.ConvertToDataTable(_export)),
                    "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                    $"檢測次數統計_{DateTime.Now.ToString("yyyyMMddHHmmss")}.xlsx");
            }

            #endregion

            return View(_response);
        }
        
        
        

        /// <summary>
        /// 服務面積統計圖表
        /// </summary>
        /// <returns></returns>
        public ActionResult ServiceArea()
        {
            var model = new ServiceAreaStatisticModel();
            model.Load();
            
            return View(model);
        }
        
    }
}