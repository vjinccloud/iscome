using ICCModule.Entity.Tables;
using ICCModule.EntityService.Service;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace ICCModule.ActionFilters
{
    /// <summary>
    /// Controller 攔截器擴增屬性
    /// </summary>
    [AttributeUsage(AttributeTargets.All, Inherited = true, AllowMultiple = true)]
    public sealed class InterceptorOfControllerAttribute : ActionFilterAttribute
    {
        /// <summary>
        /// 執行 Action 觸發事件
        /// </summary>
        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            try
            {
                if (filterContext.HttpContext.Session["LoginID"] != null)
                {
                    // 參數資訊
                    string parametersInfo = JsonConvert.SerializeObject(filterContext.ActionParameters, new JsonSerializerSettings()
                    {
                        ContractResolver = new ReadablePropertiesOnlyResolver()
                    });

                    // 運行中的 Controller & Action 資訊
                    string controllerName = filterContext.Controller.GetType().Name;
                    string actionName = filterContext.ActionDescriptor.ActionName;
                    string ipAddress = HttpContext.Current.Request.UserHostAddress;
                    string _loginId = filterContext.HttpContext.Session["LoginID"].ToString();
                    string _path = controllerName.Replace("Controller", "") + "/" + actionName;
                    var _param = "";
                    if (!filterContext.ActionParameters.Any(x => x.Key == "model")) _param = string.Join("&", filterContext.ActionParameters.Select(x => x.Key + "=" + x.Value));

                    // 訊息內容
                    string message = string.Format(
                        "{0}.{1}() => {2}",
                        controllerName,
                        actionName,
                        string.IsNullOrEmpty(parametersInfo) ? "(void)" : parametersInfo
                    );
                    if (parametersInfo.ToLower().Contains("password") || parametersInfo.ToLower().Contains("pwd"))
                        message = string.Format("{0}.{1}() => {2}", controllerName, actionName, "(void)");

                    var sysLog = new sysAccess_log()
                    {
                        LoginID = _loginId,
                        Path = _path + (string.IsNullOrEmpty(_param) ? "" : "?" + _param),
                        Act = actionName,
                        LoginIP = ipAddress.Trim(),
                        Remark = "",
                        CDate = DateTime.Now,
                    };
                    Service_sysAccess_log.Insert(sysLog);
                }
            }
            catch (Exception ex)
            {

            }
        }
    }
}
