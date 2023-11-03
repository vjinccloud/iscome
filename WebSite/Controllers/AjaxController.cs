using System;
using ICCModule.EntityService.Service;
using System.Web.Mvc;
using Website.DataModel.PestMonitor;

namespace Website.Controllers
{
    public class AjaxController : Controller
    {

        public JsonResult PestMonitor(int year, int month)
        {
            var startDate = new DateTime(year, month, 1);
            var endDate = startDate.AddMonths(1);

            var rawData 
                = Service_VW_PestMonitorDetail.GetList(x => 
                    x.MeasureDate < endDate 
                    && x.MeasureDate >= startDate);

            var res = new PestMonitorJsonResult(rawData);

            return Json(res, JsonRequestBehavior.AllowGet);
        }
    }
}