using ICCModule.Entity.Tables.Platform;
using ICCModule.EntityService.Service;
using ICCModule.Helper;
using IscomG2C.Utility;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Web;
using System.Web.Mvc;
using Website.Models;
using Website.ViewModel.Liff;

namespace Website.Controllers
{
    /// <summary>
    /// LineOA導入註冊
    /// </summary>
    public class LiffController : Controller
    {
        public ActionResult index(string redirectUri = "")
        {
            LiffViewModel data = new LiffViewModel();
            data.redirectUri = redirectUri;
            return View(data);
        }
    }
}