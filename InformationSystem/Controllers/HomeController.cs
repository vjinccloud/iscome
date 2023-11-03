using ICCModule.ActionFilters;
using ICCModule.EntityService.Service;
using InformationSystem.ViewModel.Home;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace InformationSystem.Controllers
{

    [LoginCheckFilter]//掛載此Filter，如果沒登入，就導去登入頁
    [InterceptorOfController]//系統操作Log
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            DateTime? _readDate = null;
            if (Session["LoginID"] != null)
            {
                var getReadDate = Service_sysNewsRead.GetDetail(Session["LoginID"].ToString());
                if (getReadDate != null) _readDate = getReadDate.ReadDate;
            }
            var _docShedule = Service_doctorSchedule.GetList(x => x.ReserveDatetime.Date == DateTime.Today);
            IndexViewModel model = new IndexViewModel()
            {
                Forms = Service_BusinessForm.GetList(null, true),
                doctorSchedules = _docShedule,
                sysNewsList = Service_sysNews.GetList(x => x.StartDate <= DateTime.Today && x.EndDate >= DateTime.Today),
                sysNewsPage = 1,
                FormPage = 1,
                ReadDate = _readDate ?? DateTime.MinValue,
                ReserveDatetime = DateTime.Today
            };
            if(model.RoleCode == "R08")
            {
                model.doctorSchedules = model.doctorSchedules.Where(x => model.AllDoctors.Select(o=> o.LoginID).Contains(x.LoginID)).ToList();
            }
            var origins = Service_defCode.GetList("PlantDoctorRecordOrigin");
            foreach(var item in model.doctorSchedules)
            {
                item.OriginName = origins.FirstOrDefault(x => x.Code.ToLower() == item.Origin.ToLower())?.Name ?? "";
            }
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Index(IndexViewModel model)
        {
            DateTime? _readDate = null;
            if (Session["LoginID"] != null)
            {
                var getReadDate = Service_sysNewsRead.GetDetail(Session["LoginID"].ToString());
                if (getReadDate != null) _readDate = getReadDate.ReadDate;
            }
            var sysNews = Service_sysNews.GetList(x => x.StartDate <= DateTime.Today && x.EndDate >= DateTime.Today);
            if (model.QueryDateType == "Created")
            {
                if (model.StartDate.HasValue) sysNews = sysNews.Where(x => x.CreateDate >= model.StartDate).ToList();
                if (model.EndDate.HasValue) sysNews = sysNews.Where(x => x.CreateDate <= model.EndDate).ToList();
            }
            if (model.QueryDateType == "Start")
            {
                if (model.StartDate.HasValue) sysNews = sysNews.Where(x => x.StartDate >= model.StartDate).ToList();
                if (model.EndDate.HasValue) sysNews = sysNews.Where(x => x.StartDate <= model.EndDate).ToList();
            }
            if (!string.IsNullOrEmpty(model.KeyWord)) sysNews = sysNews.Where(x => x.Title.Contains(model.KeyWord)).ToList();

            var _docShedule = Service_doctorSchedule.GetList(x => x.ReserveDatetime.Date == DateTime.Today);
            if(model.ReserveDatetime.HasValue) _docShedule = Service_doctorSchedule.GetList(x => x.ReserveDatetime.Date == model.ReserveDatetime.Value.Date);
            if(!string.IsNullOrEmpty(model.District)) _docShedule = _docShedule.Where(x => x.Zip == model.District).ToList();
            if (!string.IsNullOrEmpty(model.DoctorId)) _docShedule = _docShedule.Where(x => x.LoginID == model.DoctorId).ToList();

            model.Forms = Service_BusinessForm.GetList(null, true);
            model.doctorSchedules = _docShedule;
            model.sysNewsList = sysNews;
            model.ReadDate = _readDate ?? DateTime.MinValue;

            if (model.RoleCode == "R08")
            {
                model.doctorSchedules = model.doctorSchedules.Where(x => model.AllDoctors.Select(o => o.LoginID).Contains(x.LoginID)).ToList();
            }
            var origins = Service_defCode.GetList("PlantDoctorRecordOrigin");
            foreach (var item in model.doctorSchedules)
            {
                item.OriginName = origins.FirstOrDefault(x => x.Code.ToLower() == item.Origin.ToLower())?.Name ?? "";
            }
            return View(model);
        }
        [HttpPost]
        public void SetSysNewsRead()
        {
            if (Session["LoginID"] != null)
            {
                Service_sysNewsRead.RecordDate(Session["LoginID"].ToString());
            }

        }
        /// <summary>
        /// 取得公告
        /// </summary>
        /// <param name="Id"></param>
        /// <returns></returns>
        public JsonResult SysNewsDetail(int Id)
        {
            var news = Service_sysNews.GetList(x => x.StartDate <= DateTime.Today && x.EndDate >= DateTime.Today && x.Id == Id).FirstOrDefault();
            if (news != null)
            {
                return Json(new
                {
                    Title = news.Title,
                    NewsDate = news.StartDate.ToString("yyyy/MM/dd"),
                    Contents = news.Contents
                }, JsonRequestBehavior.AllowGet);
            }
            else
            {
                return Json(new
                {
                    Title = "",
                    NewsDate = "",
                    Contents = ""
                }, JsonRequestBehavior.AllowGet);
            }
        }
    }
}