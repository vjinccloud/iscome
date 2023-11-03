using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Website.Models.VUC;

namespace Website.Controllers
{
    public class VUCController : Controller
    {
        /// <summary>
        /// 左側選單 因為效能的關係 目前設計成會暫存在Session 避免一直重抓資料庫
        /// </summary>
        /// <returns></returns>
        public ActionResult VUC_Menu()
        {
            //從Session抓 目前登入者 目前打開的選單
            string LoginID = SessionHelper.Get_LoginID();
            string MenuID = SessionHelper.Get_MenuID();
            //建構 讀取選單資料
            Model_VUC_FunctionMenu model = new Model_VUC_FunctionMenu();
            //如果Session沒有值
            if (Session["Model_VUC_FunctionMenu"] == null)
            {
                //重新抓一次
                model.LoadData(LoginID, MenuID);
            }
            else
            {   //抓Session就好 可以避免每次都重抓左側選單 影響效能
                model = (Model_VUC_FunctionMenu)Session["Model_VUC_FunctionMenu"];
                //抓到的還是沒有內容?
                if (model.funcList.Count == 0)
                {
                    //重新抓一次
                    model.LoadData(LoginID, MenuID);
                }
                //展開選單
                model.Set_ShowInfo(MenuID);
            }
            //完成之後 寫入Session
            Session["Model_VUC_FunctionMenu"] = model;
            return PartialView(model);
        }
    }
}