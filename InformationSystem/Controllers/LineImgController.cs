using ICCModule.ActionFilters;
using ICCModule.EntityService.Service;
using ICCModule.Entity.Tables.Platform;
using InformationSystem.ViewModel.Home;
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
    [LoginCheckFilter]//掛載此Filter，如果沒登入，就導去登入頁
    [InterceptorOfController]//系統操作Log
    public class LineImgController : Controller
    {
      
        /// <summary>
        /// Line上傳圖片
        /// </summary>
        /// <returns></returns>
        public ActionResult Index()
        {
            List<FileManagement> list = Service_FileManagement
                                                    .GetList("line_doctor_img", null)
                                                    .OrderBy(a=>a.TableID)
                                                    .ToList<FileManagement>();
            ViewBag.list = list;
            return View();
        }

        /// <summary>
        /// 更新經緯度
        /// </summary>
        /// <returns></returns>
        public JsonResult SetLocation()
        {
            var list = Service_pesticideSeller.GetSellerList();
            foreach(pesticideSeller item in list)
            {
                string gps = item.Location;
                string[] arr = gps.Split(',');
                double x = Convert.ToDouble(arr[0]);
                double y = Convert.ToDouble(arr[1]);

                if(x >200 && y >200)
                {

                }
            }
            return Json(new { error = ""});
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="list"></param>
        /// <returns></returns>
        public JsonResult SaveImg(string[] list)
        {
            int index = 0;
            foreach(string f in list)
            {               
                if(f == "")
                {
                    Service_FileManagement.DeleteOldFiles("line_doctor_img", (index + 1).ToString());
                }
                else
                {
                    var dblist = Service_FileManagement.GetList("line_doctor_img", (index + 1).ToString());
                    if (dblist.Count > 0)
                    {
                        FileManagement file = new FileManagement();
                        file = dblist.Take(1).SingleOrDefault();
                        file.FileName  = "line_img" + (index + 1) + ".jpg";
                        file.FilePath = Server.MapPath("~/ErrorLog/line_img" + (index + 1) + ".jpg");
                        Service_FileManagement.Update(file);
                    }
                    else
                    {
                        FileManagement file = new FileManagement();
                        file.TableName = "line_doctor_img";
                        file.TableID = (index + 1).ToString();
                        file.FileName = "line_img" + (index + 1) + ".jpg";
                        file.FilePath = Server.MapPath("~/ErrorLog/line_img" + (index + 1) + ".jpg");

                        //FileStream fs = new FileStream(檔名, FileMode.Open, FileAccess.Read);
                        file.FileType = "image/jpeg";
                        file.UploadTime = DateTime.Now;
                        file.FileLength = 999999999;
                        Service_FileManagement.Insert(file);
                    }
                    
                }
                
                index++;
            }
            return Json(new { error = "" });
        }

        /// <summary>
        /// 上傳圖片
        /// </summary>
        /// <param name="file"></param>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpPost]
        public JsonResult Upload(HttpPostedFileBase file, int id)
        {
            string result = "";
            if (file != null && file.ContentLength > 0)
            {
                string[] arr = file.FileName.Split('.');

                string filePath = Server.MapPath("~/ErrorLog/line_img" + id + "."+ arr[arr.Length-1]);
                //儲存圖片至Server
                file.SaveAs(filePath);
                result = "/ErrorLog/line_img" + id + "." + arr[arr.Length - 1];
            }
            return Json(new { file = result });
        }

    }
}