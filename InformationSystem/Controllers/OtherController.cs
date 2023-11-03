using ICCModule.Models.PreventionInfo;
using InformationSystem.Helper;
using Newtonsoft.Json;
using System;
using System.Linq;
using System.Collections.Generic;
using System.Web.Mvc;

namespace InformationSystem.Controllers
{
    public class OtherController : Controller
    {
        public JsonResult PreventionInfoDetail(int last = 3)
        {
            List<PestNotice> notices = CommonDataHelper.PreventionInfoList();
            if (last != 0)
            {
                notices = notices.Skip(0).Take(last).ToList();
            }
            return Json(notices.ToArray(), JsonRequestBehavior.AllowGet);
        }


    }
}