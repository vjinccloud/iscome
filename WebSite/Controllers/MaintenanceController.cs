using System.Web.Mvc;

namespace Website.Controllers
{
    public class MaintenanceController : Controller
    {
        /// <summary>
        /// 維護中頁面
        /// </summary>
        /// <returns></returns>
        public ContentResult Index()
        {
            var maintenancePage = System.IO.File.ReadAllText(Server.MapPath(@"~\MaintenancePage.html"));
            return Content(maintenancePage, "text/html");
        }
    }
}