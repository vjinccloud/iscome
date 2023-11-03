using ICCModule.EntityService.Service;
using ICCModule.Entity.Tables;
using ICCModule.Entity.Tables.Platform;
using Newtonsoft.Json;
using Website.ViewModel.Home;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using IscomG2C.Utility;
using Website.ViewModel;
using ICCModule.Models.PreventionInfo;
using System.Net.Http;
using System.Net.Http.Headers;
using ICCModule.Helper;
using Newtonsoft.Json.Linq;
using System.Threading.Tasks;
using System.IO;

namespace Website.Controllers
{

    //[LoginCheckFilter]//掛載此Filter，如果沒登入，就導去登入頁
    public class HomeController : Controller
    {
        //首頁
        public async Task<ActionResult> Index(bool isPopup = true)
        {
            List<PestNotice> pestNotices;
            JArray data = new JArray();
            var client = new HttpClient();
            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            try
            {
                if (String.IsNullOrEmpty(AppSettingHelper.GetAppSetting("BackendHostUrl")))
                {
                    throw new Exception("BackendHostUrl Not Set.");
                }
                string url = String.Concat(AppSettingHelper.GetAppSetting("BackendHostUrl"), "/Other/PreventionInfoDetail?last=7");
                var res = await client.GetAsync(new Uri(url));
                data = JArray.Parse(await res.Content.ReadAsStringAsync());
                pestNotices = data.ToObject<List<PestNotice>>();
            }
            catch (Exception e)
            {
                ErrorLog.SaveErrorLog(e.Message);
                pestNotices = new List<PestNotice>();
            }
            var _acc = "";
            HttpCookie cookie = Request.Cookies["MemberAcc"];
            if (cookie != null && cookie.Value != null)
            {
                _acc = cookie.Value;
            }

            IndexViewModel model = new IndexViewModel()
            {
                Tags = Service_Tag.GetList_Range(true),
                NewsType = Service_defCode.GetList("NewsType").OrderBy(x => x.Sort).ToList(),
                News = Service_New.GetList(x => !x.PopupShow && x.Show && x.StartDate <= DateTime.Now && (!x.EndDate.HasValue || x.EndDate >= DateTime.Today.Date)),
                Links = Service_Link.GetList(true),
                PestNotices = pestNotices,
                RememberAcc = _acc
            };

            var getPopupNews = Service_New.GetList(x => x.PopupShow && x.Show && x.StartDate <= DateTime.Now && (!x.EndDate.HasValue || x.EndDate >= DateTime.Today.Date)).OrderByDescending(x => x.StartDate).FirstOrDefault();
            if (getPopupNews != null&& isPopup) model.PopupNews = getPopupNews;

            var _memberId = Session["MemberID"];
            if (_memberId != null && Service_MemberInfo.GetDetail(Convert.ToInt64(_memberId)) != null)
            {
                model.MemberInfo = Service_MemberInfo.GetDetail(Convert.ToInt64(_memberId)).Name;
            }

            return View(model);
        }
        //首頁
        public ActionResult Logout()
        {
            Session.Remove("MemberID");
            return RedirectToAction(nameof(Index));
        }
        /// <summary>
        /// 登入頁
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Index(LoginReq req)
        {
            if (req.LoginType == 1 || req.LoginType == 3)
            {
                var member = Service_MemberInfo.GetAccountDetail(req.LoginId, req.LoginType);
                if (member != null)
                {
                    if (string.IsNullOrEmpty(member.Status))
                    {
                        TempData["TempMsg"] = "帳號尚未開通";
                        TempData["TempResult"] = "error";
                        return RedirectToAction("Index", new { isPopup = false});
                    }
                    else if (member.Status == "N")
                    {
                        TempData["TempMsg"] = "帳號已遭停用";
                        TempData["TempResult"] = "error";
                        return RedirectToAction("Index", new { isPopup = false });
                    }

                    Session["MemberID"] = member.ID;
                    Session["UserName"] = member.Name;
                    return RedirectToAction("Index", new { isPopup = false });
                }
                else
                {
                    TempData["TempMsg"] = "未經綁定的帳號";
                    TempData["TempResult"] = "error";
                    return RedirectToAction("Index", new { isPopup = false });
                }
            }
            else if (req.LoginType == 2)
            {
                var member = Service_MemberInfo.GetAccountDetail(req.LoginId,req.LoginType);
                if (member!=null)
                {
                    if (string.IsNullOrEmpty(member.Status))
                    {
                        TempData["TempMsg"] = "帳號尚未開通";
                        TempData["TempResult"] = "error";
                        return RedirectToAction("Index", new { isPopup = false });
                    }
                    else if (member.Status == "N")
                    {
                        TempData["TempMsg"] = "帳號已遭停用";
                        TempData["TempResult"] = "error";
                        return RedirectToAction("Index", new { isPopup = false });
                    }
                    Session["MemberID"] = member.ID;
                    Session["UserName"] = member.Name;
                    return RedirectToAction("Index", new { isPopup = false });
                }
                else
                {
                    TempData["TempMsg"] = "未經綁定的帳號";
                    TempData["TempResult"] = "error";
                    return RedirectToAction("Index", new { isPopup = false });
                }
            }
            else
            {
                var _memberId = Session["MemberID"];

                //圖形驗證碼
                //檢查與Session值內的驗證碼是否符合?
                if (Check_VerifyCode(req.VerifyCode) == false)
                {
                    TempData["TempMsg"] = "驗證碼資訊不符，請重新輸入";
                    TempData["TempResult"] = "error";
                    return RedirectToAction("Index", new { isPopup = false });
                }
                //登入驗證
                string msg = "";

                if (HttpContext.Session["Member_Fail_Time"] != null)
                {
                    var failTime = DateTime.Parse(HttpContext.Session["Member_Fail_Time"].ToString());
                    if (failTime >= DateTime.Now)
                    {
                        TempData["TempMsg"] = "錯誤已達三次，暫停登入至 " + failTime.ToString("MM-dd HH:mm");
                        TempData["TempResult"] = "error";
                        return RedirectToAction("Index", new { isPopup = false });
                    }
                }

                var getMember = Service_MemberInfo.GetAccountDetail(req.Account);
                if (getMember != null)
                {
                    if (Utility_Cryptography.Compare_HashCode(req.LoginPass, getMember.LoginPass))
                    {
                        if (string.IsNullOrEmpty(getMember.Status))
                        {
                            TempData["TempMsg"] = "帳號尚未開通";
                            TempData["TempResult"] = "error";
                            return RedirectToAction("Index", new { isPopup = false });
                        }
                        else if (getMember.Status == "N")
                        {
                            TempData["TempMsg"] = "帳號已遭停用";
                            TempData["TempResult"] = "error";
                            return RedirectToAction("Index", new { isPopup = false });
                        }

                        if (string.IsNullOrEmpty(getMember.OldPasswords))
                        {
                            Session["UptMemberID"] = getMember.ID;
                            return RedirectToAction("SetNewPwd", "Member");
                        }
                        else if (getMember.PasswordExpiredAt < DateTime.Now)
                        {
                            Session["UptMemberID"] = getMember.ID;
                            return RedirectToAction("SetNewPwd", "Member");
                        }
                        else
                        {
                            Session["MemberID"] = getMember.ID;
                            Session["UserName"] = getMember.Name;
                            return RedirectToAction("Index", new { isPopup = false });
                        }
                    }
                    else
                    {
                        TempData["TempMsg"] = "密碼錯誤";
                        TempData["TempResult"] = "error";
                        if (HttpContext.Session["Member_Fail"] == null)
                        {
                            HttpContext.Session["Member_Fail"] = "1";
                        }
                        else
                        {
                            var errorCount = int.Parse(HttpContext.Session["Member_Fail"].ToString()) + 1;
                            HttpContext.Session["Member_Fail"] = errorCount.ToString();
                            if (errorCount >= 3)
                            {
                                HttpContext.Session["Member_Fail_Time"] = DateTime.Now.AddMinutes(15);
                                TempData["TempMsg"] = "錯誤達三次，暫停登入15分鐘";
                            }
                        }
                        return RedirectToAction("Index", new { isPopup = false });
                    }
                }
                else
                {
                    TempData["TempMsg"] = "無此帳號";
                    TempData["TempResult"] = "error";
                    if (HttpContext.Session["Member_Fail"] == null)
                    {
                        HttpContext.Session["Member_Fail"] = "1";
                    }
                    else
                    {
                        var errorCount = int.Parse(HttpContext.Session["Member_Fail"].ToString()) + 1;
                        HttpContext.Session["Member_Fail"] = errorCount.ToString();
                        if (errorCount >= 3)
                        {
                            HttpContext.Session["Member_Fail_Time"] = DateTime.Now.AddMinutes(15);
                            TempData["TempMsg"] = "錯誤達三次，暫停登入15分鐘";
                        }
                    }
                    return RedirectToAction("Index", new { isPopup = false });
                }
            }
            
        }
        /// <summary>
        /// 搜尋
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        public string SearchKey(string keyword)
        {
            return JsonConvert.SerializeObject("/Home/Search?KeyName=" + AntiXss(keyword));
        }

        public string AntiXss(string keyword) {
            keyword = keyword ?? "";
            keyword = keyword
                .Replace("<", "")
                .Replace(">", "")
                ;
            return keyword;
        }


        //平台簡介
        public ActionResult Introduce()
        {
            List<PlatformCommonView> commonViews = Service_PlatformCommonView.GetList("Introduce");
            PlatformCommonView model = commonViews.FirstOrDefault<PlatformCommonView>();
            // TODO: 增加點擊次數，需要透過 sessionID 判斷
            model.Clicks += 1;
            Service_PlatformCommonView.UpdateClicks((int)model.ID);

            return View(model);
        }

        //最新消息
        public ActionResult News(string KeyWord = "")
        {
            // 消息類型
            List<defCode> Types = Service_defCode.GetList("NewsType");
            // 取得問答，以 Type 為索引
            List<New> NewsList = Service_New.GetList("", true, KeyWord);

            Dictionary<string, List<New>> List = new Dictionary<string, List<New>>();

            foreach (defCode defCode in Types)
            {
                List<New> list = new List<New>();
                foreach (New m in NewsList)
                {
                    if (m.Code == defCode.Code)
                    {
                        list.Add(m);
                    }
                }
                List.Add(defCode.Code, list);
            }

            NewsViewModel model = new NewsViewModel()
            {
                Types = Types,
                List = List,
                KeyWord = KeyWord
            };

            return View(model);
        }

        //最新消息-內頁
        public ActionResult News_Detail(String ID)
        {
            if (string.IsNullOrEmpty(ID))
            {
                return RedirectToAction(nameof(News));
            }
            var sessionName = $"NewsView{ID}";
            var newsViewSession = Session[sessionName];
            if (newsViewSession == null)
            {
                Service_New.UpdateClicks(int.Parse(ID));
                newsViewSession = ID;
                Session[sessionName] = ID;
            }
            New model = Service_New.GetDetail(int.Parse(ID));
            return View(model);
        }

        //相關連結
        public ActionResult Links()
        {
            List<Link> model = Service_Link.GetList(true);

            return View(model);
        }

        //全站搜尋
        public ActionResult Search(String KeyName = "")
        {
            if (String.IsNullOrEmpty(KeyName))
            {
                SearchViewModel model2 = new SearchViewModel()
                {
                    VW_Searchs = new List<VW_Search>(),
                    KeyName = KeyName,
                };
                return View(model2);
            }

            SearchViewModel model = new SearchViewModel()
            {
                VW_Searchs = Service_VW_Search.GetList(KeyName),
                KeyName = KeyName,
            };

            return View(model);
        }

        //網站導覽
        public ActionResult Sitemaps()
        {
            return View();
        }

        //常見問答
        public ActionResult QA()
        {
            // 問題類型
            List<defCode> Types = Service_defCode.GetList("QuestionType");
            // 取得問答，以 Type 為索引
            List<QuestionManagement> Questions = Service_QuestionManagement.GetList();

            Dictionary<string, List<QuestionManagement>> List = new Dictionary<string, List<QuestionManagement>>();

            foreach (defCode defCode in Types)
            {
                List<QuestionManagement> list = new List<QuestionManagement>();
                foreach (QuestionManagement qa in Questions)
                {
                    if (qa.Type == defCode.Code)
                    {
                        list.Add(qa);
                    }
                }
                List.Add(defCode.Code, list);
            }

            QuestionManagementViewModel model = new QuestionManagementViewModel()
            {
                Types = Types,
                List = List,
            };

            return View(model);
        }

        /// <summary>
        /// 職醫日常
        /// </summary>
        public ActionResult DoctorDailyJob() {
            return View();
        }

        /// <summary>登入頁面使用-產生隨機認證碼圖片 並且設定驗證碼在SESSION值中
        /// 
        /// </summary>
        public void GetValidateCodeImage()
        {
            string Code = Logic_ValidateCodeImage.GetValidateCode(4);
            //將驗證碼紀錄在Session中
            Session["HomeValidate"] = Code;

            //取得圖片物件
            var image = Logic_ValidateCodeImage.GetBinary_ValidateCodeImage(Code);
            /*輸出圖片*/
            Response.Clear();
            Response.ContentType = "image/jpeg";
            Response.Headers.Add("Strict-Transport-Security", "max-age=31536000; includeSubDomains");
            Response.BinaryWrite(image);
        }

        /// <summary>檢查與Session值內的驗證碼是否符合?
        /// 
        /// </summary>
        /// <param name="m_sValidate"></param>
        /// <returns></returns>
        bool Check_VerifyCode(string m_sValidate)
        {
            m_sValidate = m_sValidate ?? "";

            //如果不是測試模式 有輸入驗證碼 且 驗證碼符合
            if (Session["HomeValidate"] == null)
                return false;

            if (Session["HomeValidate"].ToString().ToUpper() != m_sValidate.ToUpper())
                return false;

            //通過檢查驗證碼檢查
            return true;
        }
        /// <summary>
        /// 取得計數
        /// </summary>
        /// <returns></returns>
        public ActionResult GetViewCount()
        {
            var path = Server.MapPath("~/Count.txt");

            int line = 0;
            try
            {
                StreamReader sr = new StreamReader(path);
                try
                {
                    line = Extention.ToInt32((sr.ReadLine() ?? ""));
                    sr.Close();
                }
                catch (Exception ex)
                {
                    sr.Close();
                    Console.WriteLine("Exception: " + ex.Message);
                }

                var setCount = Session["SetCount"];
                var setCountDate = Session["SetCountDate"];

                DateTime _date = DateTime.Now.AddDays(-1);
                if (setCountDate != null) DateTime.TryParse(setCountDate.ToString(), out _date);

                if (setCount == null || _date < DateTime.Now.AddMinutes(-30))
                {
                    StreamWriter sw = new StreamWriter(path);
                    try
                    {
                        line = line + 1;
                        Session["SetCount"] = line;
                        Session["SetCountDate"] = DateTime.Now;
                        sw.Write(line);
                        sw.Close();
                    }
                    catch (Exception ex)
                    {
                        sw.Close();
                        Console.WriteLine("Exception: " + ex.Message);
                    }
                }
            }
            catch (Exception ex)
            {

            }
            return Json(line.ToString("0000"), JsonRequestBehavior.AllowGet);
        }
    }

}