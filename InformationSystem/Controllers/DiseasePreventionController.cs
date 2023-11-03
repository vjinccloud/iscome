using ICCModule.EntityService.Service;
using ICCModule.Entity.Tables.Platform;
using System;
using System.Collections.Generic;
using System.Web.Mvc;
using ICCModule.ActionFilters;
using System.Linq;
using ICCModule.Helper;
using InformationSystem.ViewModel;

namespace InformationSystem.Controllers
{
    [LoginCheckFilter]//掛載此Filter，如果沒登入，就導去登入頁
    [InterceptorOfController]//系統操作Log
    public class DiseasePreventionController : Controller
    {
        //介接設定
        public ActionResult APIStatus()
        {
            List<InterfaceManagement> model = Service_InterfaceManagement.GetList();
            return View(model);
        }

        //標籤管理
        public ActionResult TagManage()
        {
            List<Tag> tags = Service_Tag.GetList().OrderByDescending(x => x.Searches).ToList();
            return View(tags);
        }

        [HttpPost]
        public ActionResult ChangeShow(string KeyName, bool Show)
        {
            Tag model = Service_Tag.GetDetail(KeyName);
            bool result = false;
            if (model != null)
            {
                try
                {
                    model.Show = Show;
                    Service_Tag.Update(model);
                    result = true;
                }
                catch (Exception e)
                {
                    result = false;
                }
            }
            return RedirectToAction(nameof(TagManage));
        }

        //訂閱訊息統計
        public ActionResult NotifyStat()
        {
            var listWord = new List<string>();
            var getWords = Service_MemberInfo.GetList(x => true).Where(x => !string.IsNullOrEmpty(x.SubscribeEpidemic));
            foreach (var item in getWords)
            {
                var thisWord = item.SubscribeEpidemic.Split('｜').ToList();
                listWord.AddRange(thisWord);
            }
            var data = listWord.GroupBy(x=> x).Select(x => new NotifyStatModel
            {
                KeyWord = x.Key,
                Counts = x.Count(),
            }).OrderByDescending(x => x.Counts).ToList();
            return View(data);
        }

        /// <summary>
        /// 匯出訂閱關鍵字excel
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult OutputExcel()
        {
            var listWord = new List<string>();
            var getWords = Service_MemberInfo.GetList(x => true).Where(x => !string.IsNullOrEmpty(x.SubscribeEpidemic));
            foreach (var item in getWords)
            {
                var thisWord = item.SubscribeEpidemic.Split('｜').ToList();
                listWord.AddRange(thisWord);
            }
            var data = listWord.GroupBy(x => x).Select(x => new NotifyStatExcelModel
            {
                排行順序 = 0,
                關鍵字 = x.Key,
                計次 = x.Count(),
            }).OrderByDescending(x => x.計次).ToList();

            int i = 1;
            foreach (var item in data)
            {
                item.排行順序 = i;
                i++;
            }
            return File(ExcelHelper.RenderDataTableToExcel(ExcelHelper.ConvertToDataTable(data)), "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", $"訂閱關鍵字.xlsx");
        }
    }
}