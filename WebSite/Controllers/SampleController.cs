using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Website.Models.Sample;

namespace Website.Controllers
{

    [LoginCheckFilter]//掛載此Filter，如果沒登入，就導去登入頁
    public class SampleController : Controller
    {
        //存取控制,如果這個功能是從左側選單進來,就不用特別設定參數
        [AccessControlFilter]
        public ActionResult AccessControl()
        {
            return View();
        }

        //存取控制,設定要跟隨的主功能路徑,以及要做的動作
        [AccessControlFilter(followURL = "/Sample/AccessControl", action = ActionCode.create)]
        public ActionResult AccessControl_Create()
        {
            return View();
        }
        //存取控制,自動依據路徑判定
        [AccessControlFilter(followURL = "/Sample/AccessControl", action = ActionCode.update)]
        public ActionResult AccessControl_Update()
        {
            return View();
        }
        //存取控制,自動依據路徑判定
        [AccessControlFilter(followURL = "/Sample/AccessControl", action = ActionCode.delete)]
        public ActionResult AccessControl_Delete()
        {
            return View();
        }

        /// <summary>
        /// 網頁元件範例-換頁
        /// </summary>
        /// <returns></returns>
        //[AccessControlFilter]
        [HttpGet]
        public ActionResult Pager()
        {
            Model_Sample_Pager model = new Model_Sample_Pager();
            model.Load();
            return View(model);
        }

        /// <summary>
        /// 網頁元件範例-換頁 POST
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public ActionResult Pager(Model_Sample_Pager model)
        {
            //清掉快取
            ModelState.Clear();
            //重新抓取資料
            model.Load();
            return View(model);
        }
    }
}