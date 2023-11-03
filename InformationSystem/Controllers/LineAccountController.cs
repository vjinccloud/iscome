using ICCModule.ActionFilters;
using ICCModule.EntityService.Service;
using ICCModule.Helper;
using InformationSystem.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace InformationSystem.Controllers
{
    /// <summary>
    /// 
    /// </summary>
    [InterceptorOfController]//系統操作Log
    public class LineAccountController : Controller
    {
        //LINE帳號綁定列表頁
        public ActionResult MemberInfo()
        {
            var req = new LineQueryModel();
            if(req.Page<1) req.Page = 1;
            var allData = Service_LineMessageLog.GetList(x => (!req.StartDate.HasValue || req.StartDate <= x.CreateDate) && (!req.EndDate.HasValue || req.EndDate >= x.CreateDate.Date));
            var data = from lm in allData.OrderByDescending(x => x.CreateDate).Skip((req.Page - 1) * 20).Take(20).ToList()
                       join member in Service_MemberInfo.GetList()
                       on (lm.MemberId ?? 0) equals member.ID into temp
                       from member in temp.DefaultIfEmpty()
                       select new LineMessageLogShowModel
                       {
                           MemberAccount = member?.LoginID ?? "",
                           Name = lm.Name,
                           UserMessage =lm.UserMessage,
                           CreateDate =lm.CreateDate,
                       };

            if (req.Action == "export")
            {
                var exportData = data.Select(x => new
                           {
                               會員帳號 = x.MemberAccount,
                               姓名 = x.Name,
                               使用者訊息 = x.UserMessage,
                               時間 = x.CreateDate,
                           });
                return File(ExcelHelper.RenderDataTableToExcel(ExcelHelper.ConvertToDataTable(exportData.OrderByDescending(x => x.時間).ToList())), "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", $"LineMessage紀錄_{DateTime.Now.ToString("yyyyMMddHHmm")}.xlsx");
            }

            var response = new LineModel()
            {
                QueryReq = req,
                Data = data.ToList(),
                TotalPage = (int)Math.Ceiling((decimal)allData.Count() / 20)
            };
            return View(response);
        }
        //LINE帳號綁定列表頁
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult MemberInfo(LineQueryModel req)
        {
            if(req.Page<1) req.Page = 1;
            var allData = Service_LineMessageLog.GetList(x => (!req.StartDate.HasValue || req.StartDate <= x.CreateDate) && (!req.EndDate.HasValue || req.EndDate >= x.CreateDate.Date));
            var data = from lm in allData.OrderByDescending(x => x.CreateDate).Skip((req.Page - 1) * 20).Take(20).ToList()
                       join member in Service_MemberInfo.GetList()
                       on (lm.MemberId ?? 0) equals member.ID into temp
                       from member in temp.DefaultIfEmpty()
                       select new LineMessageLogShowModel
                       {
                           MemberAccount = member?.LoginID ?? "",
                           Name = lm.Name,
                           UserMessage =lm.UserMessage,
                           CreateDate =lm.CreateDate,
                       };

            if (req.Action == "export")
            {
                var exportData = data.Select(x => new
                           {
                               會員帳號 = x.MemberAccount,
                               姓名 = x.Name,
                               使用者訊息 = x.UserMessage,
                               時間 = x.CreateDate,
                           });
                return File(ExcelHelper.RenderDataTableToExcel(ExcelHelper.ConvertToDataTable(exportData.OrderByDescending(x => x.時間).ToList())), "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", $"LineMessage紀錄_{DateTime.Now.ToString("yyyyMMddHHmm")}.xlsx");
            }

            var response = new LineModel()
            {
                QueryReq = req,
                Data = data.ToList(),
                TotalPage = (int)Math.Ceiling((decimal)allData.Count() / 20)
            };
            return View(response);
        }
    }
}