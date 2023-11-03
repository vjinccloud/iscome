using System.Web.Mvc;
using System.Web.Routing;
using ICCModule.Helper;

namespace InformationSystem.Helper
{
    public class MaintenanceActionFilter : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            string actionName = filterContext.ActionDescriptor.ActionName;
            string controllerName = filterContext.ActionDescriptor.ControllerDescriptor.ControllerName;

            var isMaintenancePage = (controllerName == "Maintenance" && actionName == "Index");
                
            if (AppSettingHelper.DownForMaintenance && !isMaintenancePage)
            {
                filterContext.Result = new RedirectToRouteResult("Maintenance",
                    new RouteValueDictionary(new { controller = "Maintenance", action = "Index" }));
            }
        }
    }
}