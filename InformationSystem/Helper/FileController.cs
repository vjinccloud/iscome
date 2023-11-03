using ICCModule.CommonClass;
using IscomG2C.Utility;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace InformationSystem.Controllers
{
    public class FileController : Controller
    {
        [HttpPost]
        public ContentResult UploadImage(HttpPostedFileBase file)
        {
            FileResponse res = new FileResponse();
            try
            {
                if (file.ContentLength > 0)
                {
                    string _FileName = Path.GetFileName(file.FileName);
                    string _directory = Server.MapPath("~/UploadedFiles/ckfiles");

                    if (!new DirectoryInfo(_directory).Exists)
                    {
                        FileHelper.CreateFolder(_directory);
                    }

                    string _path = Path.Combine(_directory, _FileName);
                    file.SaveAs(_path);
                    res.url = String.Concat("/UploadedFiles/ckfiles/", _FileName);
                    res.result = "success";
                    res.msg = "";
                }
                return Content(JsonConvert.SerializeObject(res), "application/json");
            }
            catch (Exception e)
            {
                res.url = "";
                res.result = "fail";
                res.msg = e.Message;
                return Content(JsonConvert.SerializeObject(res), "application/json");
            }
        }

        [HttpPost]
        public ActionResult UploadFile(HttpPostedFileBase file)
        {
            try
            {
                if (file.ContentLength > 0)
                {
                    string _FileName = Path.GetFileName(file.FileName);
                    string _directory = Server.MapPath("~/UploadedFiles/ckfiles");
                    FileHelper.CreateFolder(_directory);

                    string _path = Path.Combine(_directory, _FileName);
                    file.SaveAs(_path);
                }
                return View();
            }
            catch
            {
                return View();
            }
        }
    }
}