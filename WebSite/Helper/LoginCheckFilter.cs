using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using IscomG2C.Utility;

namespace Website.Controllers
{
    /// <summary>MVC Filter 用來檢查使用者是否有登入 如果沒登入 就導去登入頁
    /// 1.注意此Filter 不會進行權限檢查 只用來協助導去登入頁
    /// 2.AJAX之類 不需要登入的功能  不要掛此Filter 避免系統誤判
    /// </summary>
    public class LoginCheckFilter : ActionFilterAttribute
    {
        public LoginCheckFilter()
        {
        }

        // Action執行的之前，判斷授權狀態
        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            // 設定節點資訊

            try
            {
                if (filterContext.HttpContext.Session["LoginID"] == null)
                {
                    //導入登入頁面
                    RedirectLoginPage(filterContext);
                }

                string m_iUserID = Convert.ToString(filterContext.HttpContext.Session["LoginID"]);

                if (m_iUserID == "")
                {
                    //導入登入頁面
                    RedirectLoginPage(filterContext);
                }
            }
            catch (Exception ex)
            {
                ErrorLog.SaveErrorLog(ex);
               //只留紀錄 但是不防止 避免因為一直重複導頁導致掛掉
            }
            //通過檢查
            return;
        }

        /// <summary>導入登入頁面
        /// 
        /// </summary>
        /// <param name="filterContext"></param>
        private void RedirectLoginPage(ActionExecutingContext filterContext)
        {
            filterContext.Controller.TempData["TempMsg"] = "您尚未登入，請重新登入（可能是連線逾時導致）";
            filterContext.Result = new RedirectToRouteResult("Default",
                new RouteValueDictionary(new { controller = "Login", action = "Index" }));
        }
    }
}