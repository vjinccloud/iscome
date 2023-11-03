using IscomG2C.Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Website.Controllers
{
    public class TESTController : Controller
    {
        // GET: TEST
        [AccessControlFilter]
        public ActionResult Index()
        {

            string admin = "admin";
            admin = Utility_Cryptography.Get_HashEncryption(admin);
            return View();
        }
    }

    public class TESTModel
    {
        public TESTModel()
        {
            this.chkList = new List<SelectListItem>();
        }
        //選單列
        public List<SelectListItem> chkList { get; set; }

        public string PostTargetStr { get; set; }
    }
}