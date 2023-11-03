using ICCModule;
using ICCModule.Entity.Tables;
using ICCModule.EntityService.Service;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace Website.Controllers
{
    /// <summary>
    /// 存取動作的代碼,
    /// 可自行擴充,請記得調整AccessControlFilter.Check_isAudth()方法對應的代碼
    /// </summary>
    public enum ActionCode
    {

        read,
        create,
        update,
        delete,
        import,
        export
    }
    public class AccessControlFilter : ActionFilterAttribute
    {
        /// <summary>
        /// 控制項動作代碼
        /// </summary>
        public ActionCode action { get; set; }

        /// <summary>
        /// 存取控制機制中,要跟隨的選單URL
        /// 每個功能,有多項操作項目,但資料庫只有記錄進入點清單模組的URL
        /// 例.若同一組功能，操作操作或修改功能時,要先指定由那一個清單模組進入，才能記錄其權限
        /// </summary>
        public string followURL { get; set; }

        public AccessControlFilter()
        {
            action = ActionCode.read;//預設通常是Read
            followURL = "";
        }

        /// <summary>
        /// Action執行的之前，判斷授權狀態 
        /// 會執行已下動作
        /// 1.從Session 取得目前使用者的ID 
        /// 2.取得目前操作功能的URL
        /// 3.從Session或資料庫抓當前使用者可存取的功能
        /// 4.分析是否有權限 若無則退去首頁 若有則通過
        /// 5.紀錄目前存取的功能大項目ID,方便維持選單開啟
        /// </summary>
        /// <param name="filterContext"></param>
        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            //1.取得目前使用者的ID
            string LoginID = SessionHelper.Get_LoginID();
            if (LoginID == "")
            {
                // 利用Default路由
                // 傳送使用者頁面回到登錄頁面
                filterContext.Result = new RedirectToRouteResult("Default",
                    new RouteValueDictionary(new { controller = "Login", action = "Index" }));
                return;
            }


            string url = PharseURL(filterContext);

            //3.分析是否有權限 
            bool isAuth = AccessControlFilter_Helper.Check_isAuth(LoginID, url, this.action);
            //分析是否有權限 沒有就回去首頁
            if (isAuth == false)
            {
                //紀錄log
                SaveLog(filterContext, "deny");
                //授權失敗的導頁動作
                ErrorRedirect(filterContext);
                return;
            }

            //4.通過的話,再記錄一下目前的大選單ID 維持開啟狀態
            //從Session或資料庫抓資料 先從Session抓 Session沒有 再從資料庫抓
            var list_menu = AccessControlFilter_Helper.GetMeunList_FromSessionOrDB(LoginID);

            //5.將目前開啟的大選單ID寫入Session 方便等一下維持開啟狀態
            //尋找目前的大選單ID是哪個
            var meunData = list_menu.Find(x => x.Path == url);
            string MenuParentID = "";
            if (meunData != null)
            {
                MenuParentID = meunData.MenuParentID;
            }
            //大選單ID寫入Session
            SessionHelper.Set("MenuID", MenuParentID);

            //紀錄授權通過log
            SaveLog(filterContext, this.action.ToString());

        }

        /// <summary>
        /// 解析本次功能主要的URL路徑
        /// </summary>
        /// <param name="filterContext"></param>
        /// <returns></returns>
        private string PharseURL(ActionExecutingContext filterContext)
        {

            //2.取得目前操作功能的URL
            string url = filterContext.HttpContext.Request.Path.ToString();
            //如果有指定 則替換成指定的路徑
            if (this.followURL != "")
            {
                url = this.followURL;
            }
            url = AccessControlFilter_Helper._PharseURL(url);
            return url;
        }

        /// <summary>
        /// 授權失敗的導頁動作
        /// </summary>
        /// <param name="filterContext"></param>
        private void ErrorRedirect(ActionExecutingContext filterContext)
        {
            //紀錄Log
            sysAccess_log log = new sysAccess_log();

            Service_sysAccess_log.Insert(log);

            filterContext.Result = new RedirectToRouteResult("Default",
                               new RouteValueDictionary(new { controller = "Home", action = "Index" }));

            filterContext.Controller.TempData["TempMsg"] = "您目前無此功能操作權限";
        }

        /// <summary>
        /// 紀錄系統Log
        /// </summary>
        /// <param name="filterContext"></param>
        /// <returns></returns>
        private BaseResult SaveLog(ActionExecutingContext filterContext, string action, string remark = "")
        {

            //紀錄Log
            sysAccess_log log = new sysAccess_log();
            log.Act = action;
            log.LoginID = SessionHelper.Get_LoginID();
            log.LoginIP = filterContext.RequestContext.HttpContext.Request.UserHostAddress;
            log.Path = PharseURL(filterContext);
            log.Remark = remark;
            return Service_sysAccess_log.Insert(log);
        }
    }

    public class AccessControlFilter_Helper
    {

        /// <summary>
        /// 根據MVC架構的Routing解析URL  
        /// EX: /TEST/Action/5567 => /TEST/Action
        ///     /TEST => /TEST/index
        /// </summary>
        /// <param name="url"></param>
        /// <returns></returns>
        public static string _PharseURL(string url)
        {
            //根據MVC架構的Routing解析URL  
            //EX: /TEST/Action/5567 => /TEST/Action
            //      /TEST => /TEST/index
            string controller = IscomG2C.Utility.Utility_String.GetSplit(url, "/", 0);
            string action = IscomG2C.Utility.Utility_String.GetSplit(url, "/", 1);
            if (action == "")
                action = "index";
            //重新拼接回去
            url = "/" + controller + "/" + action;
            //一律轉小寫,方便比對
            url = url.ToLower();
            return url;
        }

        /// <summary>
        /// 從Session或資料庫抓資料 先從Session抓 Session沒有 再從資料庫抓
        /// </summary>
        /// <param name="LoginID"></param>
        /// <returns></returns>
        public static List<VW_FuntionMenu_LV2> GetMeunList_FromSessionOrDB(string LoginID)
        {
            //取得授權資料 目前的架構下一定是第二層
            //先從Session抓
            List<VW_FuntionMenu_LV2> Authed_FuntionMenu = SessionHelper.Get_Authed_FuntionMenu();
            //Session沒有?
            if (Authed_FuntionMenu == null)
            {   //再從資料庫抓
                Authed_FuntionMenu = Service_VW_FuntionMenu_LV2.GetList(LoginID);
                foreach (var item in Authed_FuntionMenu)
                {
                    //一律轉小寫,方便比對
                    item.Path = item.Path.ToLower();
                }
            }
            //寫回去Session,方便之後使用 不用重抓資料庫
            SessionHelper.Set("Authed_FuntionMenu", Authed_FuntionMenu);
            return Authed_FuntionMenu;
        }

        /// <summary>
        /// 根據  檢查是否符合授權?
        /// </summary>
        /// <returns></returns>
        public static bool Check_isAuth(string LoginID, string UrlPath, ActionCode action)
        {
            //從Session或資料庫抓當前使用者可存取的功能 先從Session抓 Session沒有 再從資料庫抓
            List<VW_FuntionMenu_LV2> Authed_FuntionMenu = GetMeunList_FromSessionOrDB(LoginID);
            //分析是否有權限 
            bool isAuth = false;
            //分析是否有權限
            var data = Authed_FuntionMenu.Find(x => x.Path == UrlPath);
            if (data == null)
            {
                return false;
            }
            //根據 可授權的動作列 (格式:|C|R|U|D|I|E| ) 檢查是否符合授權? 
            isAuth = _Check_isAuthListStr_Match(action, data.AllowList);
            return isAuth;
        }

        /// <summary>
        /// 根據 可授權的動作列 (格式:|C|R|U|D|I|E| ) 檢查是否符合授權?
        /// </summary>
        /// <param name="action">當前的動作代碼</param>
        /// <param name="AuthListStr">可授權的動作列 (格式:|C|R|U|D|I|E| )</param>
        /// <returns></returns>
        public static bool _Check_isAuthListStr_Match(ActionCode action, string AuthListStr)
        {
            //分析是否有權限 
            bool isAuth = false;
            switch (action)
            {
                case ActionCode.read:
                    //這個無條件通過 不可能無法read
                    isAuth = true;
                    break;
                case ActionCode.create:
                    isAuth = AuthListStr.Contains("|C|");
                    break;
                case ActionCode.update:
                    isAuth = AuthListStr.Contains("|U|");
                    break;
                case ActionCode.delete:
                    isAuth = AuthListStr.Contains("|D|");
                    break;
                case ActionCode.import:
                    isAuth = AuthListStr.Contains("|I|");
                    break;
                case ActionCode.export:
                    isAuth = AuthListStr.Contains("|E|");
                    break;
                default:
                    break;
            }
            return isAuth;
        }

        /// <summary>
        /// 依據變數名稱 直接轉換過去
        /// </summary>
        /// <param name="action"></param>
        /// <returns></returns>
        public string ActionCode_ToString(ActionCode action)
        {
            return nameof(action);
        }

    }
}