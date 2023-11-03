using InformationSystem.Controllers;
using InformationSystem.Models.SystemLog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.IO;
using ICCModule.EntityService.Service;

namespace IMSystem.Controllers
{
    [LoginCheckFilter]
    public class SystemLogController : Controller
    {
        //URL = 子功能首頁 (List) //action = 動作存取權限控制
        [AccessControlFilter(followURL = "/SystemLog/OperateRecord", action = ActionCode.read)]
        //系統操作紀錄查詢/清單
        public ActionResult OperateRecord()
        {
            var mid = "";
            if (Session["LoginID"] != null)
            {
                mid = Session["LoginID"].ToString();
            }
            var logList = Service_sysAccess_log.GetVWList(null, null);
            Model_Operate PostData = new Model_Operate()
            {
                LoginID = mid,
                LogList = logList.Take(30).ToList(),
                Page = 1,
                TotalPage = (logList.Count / 30) +1
            };
            return View(PostData);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult OperateRecord(Model_Operate PostData)
        {
            var mid = "";
            if (Session["LoginID"] != null)
            {
                mid = Session["LoginID"].ToString();
            }
            var logList = Service_sysAccess_log.GetVWList(PostData.qrybeg, PostData.qryend);
            PostData.LoginID = mid;
            PostData.LogList = logList.Skip((PostData.Page-1)*30).Take(30).ToList();
            PostData.TotalPage = (logList.Count / 30) + 1;
            return View(PostData);
        }

        //URL = 子功能首頁 (List) //action = 動作存取權限控制
        [AccessControlFilter(followURL = "/SystemLog/LoginRecord", action = ActionCode.read)]
        // 系統登入紀錄查詢/清單
        public ActionResult LoginRecord()
        {
            Model_Login PostData = new Model_Login();
            return View(PostData);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult LoginRecord(Model_Login PostData)
        {
            PostData.LogList = Service_sysLogin_log.GetVWList(PostData.LoginID, PostData.qrybeg, PostData.qryend, PostData.page);
            return View(PostData);
        }

    }
}