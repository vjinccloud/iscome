using InformationSystem.Controllers;
using InformationSystem.Models.CodeManage;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.IO;
using ICCModule.ActionFilters;

namespace IMSystem.Controllers
{
    [InterceptorOfController]//系統操作Log
    public class CodeManageController : Controller
    {
        //URL = 子功能首頁 (List) //action = 動作存取權限控制
        [AccessControlFilter(followURL = "/CodeManage/List", action = ActionCode.read)]
        //查詢/清單
        public ActionResult List(string Kind = "")
        {
            Model_List PostData = new Model_List();
            //如果有帶入類型參數 (EX.從新增/編輯頁返回
            if (!string.IsNullOrEmpty(Kind))
                PostData.CodeKind = Kind;
            PostData.Load();
            return View(PostData);
        }
        [HttpPost]
        public ActionResult List(Model_List PostData)
        {
            PostData.Load();
            return View(PostData);
        }

        /// <summary>
        /// 確認此類別是否可編輯
        /// </summary>
        [HttpPost]
        public ActionResult CheckAdd(string Kind)
        {
            Model_Add PostData = new Model_Add();
            var rslt = PostData.Load(Kind);
            if (!rslt.result)
            {
                return Json(new { IsUpdate = rslt.result, Error = rslt.Msg });
            }
            return Json(new { IsUpdate = true, Error = "" });
        }

        //URL = 子功能首頁 (List) //action = 動作存取權限控制
        [AccessControlFilter(followURL = "/CodeManage/List", action = ActionCode.create)]
        // 新增
        public ActionResult Add(string Kind)
        {
            Model_Add PostData = new Model_Add();
            var rslt = PostData.Load(Kind);
            if (!rslt.result)
            {
                TempData["TempMsg"] = rslt.Msg;
                TempData["TempResult"] = "error";
                return RedirectToAction("List");
            }
            return View(PostData);
        }
        [HttpPost]
        public ActionResult Add(Model_Add PostData)
        {
            if (Session["UserName"] != null)
            {
                PostData.myCode.CreateUser = Session["UserName"].ToString();
                PostData.myCode.UpdateUser = Session["UserName"].ToString();
            }
            var rslt = PostData.SaveData();
            if (!rslt.result)
            {
                return Json(new { IsUpdate = rslt.result, Error = rslt.Msg });
            }
            return Json(new { IsUpdate = true, Error = "" });
        }

        //URL = 子功能首頁 (List) //action = 動作存取權限控制
        [AccessControlFilter(followURL = "/CodeManage/List", action = ActionCode.update)]
        // 編輯
        public ActionResult Edit(string Kind, string Code)
        {
            Model_Edit PostData = new Model_Edit();
            var rslt = PostData.Load(Kind, Code);
            if (!rslt.result)
            {
                TempData["TempMsg"] = rslt.Msg;
                TempData["TempResult"] = "error";
                return RedirectToAction("List");
            }
            return View(PostData);
        }
        [HttpPost]
        public ActionResult Edit(Model_Edit PostData)
        {
            if (Session["UserName"] != null)
            {
                PostData.myCode.UpdateUser = Session["UserName"].ToString();
            }
            var rslt = PostData.SaveData();
            if (!rslt.result)
            {
                return Json(new { IsUpdate = rslt.result, Error = rslt.Msg });
            }
            return Json(new { IsUpdate = true, Error = "" });
        }

        //URL = 子功能首頁 (List) //action = 動作存取權限控制
        [AccessControlFilter(followURL = "/CodeManage/List", action = ActionCode.delete)]
        //刪除
        [HttpPost]
        public ActionResult Delete(string Kind, string Code)
        {
            Model_Edit PostData = new Model_Edit();
            PostData.Load(Kind, Code);
            var rslt = PostData.DeleteData();
            if (!rslt.result)
            {
                //TempData["TempMsg"] = rslt.Msg;
                return Json(new { IsUpdate = rslt.result, Error = rslt.Msg });
            }
            //TempData["TempMsg"] = "刪除成功";
            return Json(new { IsUpdate = true, Error = "" });

        }
    }
}