using ICCModule.ActionFilters;
using ICCModule.Entity.Tables;
using ICCModule.EntityService.Service;
using ICCModule.Helper;
using InformationSystem.Controllers;
using InformationSystem.Helper;
using InformationSystem.Models;
using InformationSystem.Models.Login;
using InformationSystem.ViewModel.Management;
using IscomG2C.Utility;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;

namespace InformationSystem.Controllers
{
    [InterceptorOfController]
    public class LoginController : Controller
    {
        /// <summary>
        /// 登入頁
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public ActionResult Index()
        {
            Model_Login_Index model = new Model_Login_Index();
            return View(model);
        }
        /// <summary>
        /// 登入頁
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Index(Model_Login_Index model)
        {
            if (HttpContext.Session["SysUser_Fail_Time"] != null)
            {
                var failTime = DateTime.Parse(HttpContext.Session["SysUser_Fail_Time"].ToString());
                if (failTime >= DateTime.Now)
                {
                    TempData["TempMsg"] = "錯誤已達三次，暫停登入至 " + failTime.ToString("MM-dd HH:mm");
                    TempData["TempResult"] = "error";
                    return RedirectToAction("Index");
                }
            }
            //圖形驗證碼
            //檢查與Session值內的驗證碼是否符合?
            if (Check_VerifyCode(model.reCAPTCHA) == false)
            {
                TempData["TempMsg"] = "驗證碼資訊不符，請重新輸入";
                TempData["TempResult"] = "error";
                return View(model);
            }
            //抓取偵錯模式標記
            bool isDebug = AppSettingHelper.Get_Debug_Mode();
            //登入驗證
            string msg = "";
            LoginState state = model.LoginCheck(isDebug, ref msg);
            //登入紀錄Log
            bool savelog = model.SaveLog(Request.UserHostAddress, state, msg);
            //如果錯誤則回到首頁
            switch (state)
            {
                case LoginState.OK:
                    //抓取使用者資訊,放到Session中,避免後續一直抓資料庫
                    var usr = Service_sysUserInfo.GetDetail(model.LoginID);
                    //設定Session
                    //帳號
                    Session["LoginID"] = usr.LoginID;
                    SessionHelper.Set("LoginID", usr.LoginID);
                    SessionHelper.Set("UserName", usr.UserName);
                    SessionHelper.Set("UserRoleCode", usr.RoleID);

                    var rolePermission = from p in Service_Permission.GetBackList()
                                         join rp in Service_Role_Has_Permission.GetList(usr.RoleID)
                                         on p.Id equals rp.PermissionId
                                         select p;
                    var specialPermission = from p in Service_Permission.GetBackList()
                                            join rp in Service_sysUserSpecialPermission.GetList(x => x.sysUserId == usr.LoginID)
                                            on p.Id equals rp.PermissionId
                                            select p;
                    rolePermission = rolePermission.Union(specialPermission);
                    SessionHelper.Set("LoginRole", JsonConvert.SerializeObject(rolePermission));

                    // 初次登入需要變更密碼
                    //if (usr.LastChangePWDate == null || usr.LastChangePWDate< DateTime.Now.AddMonths(-3))
                    //{
                    //    return RedirectToAction("UserNeedUpdatePwd", "Management");
                    //}
                    return RedirectToAction("SelectRole", "Login");
                case LoginState.RESET:
                    //設定Session
                    Session["LoginID"] = model.LoginID;
                    return RedirectToAction("index", "Home");
                case LoginState.NG:
                case LoginState.DISABLED:
                case LoginState.LOCK:
                default:
                    //回去首頁
                    TempData["TempMsg"] = msg;//顯示錯誤訊息
                    TempData["TempResult"] = "error";

                    if (HttpContext.Session["SysUser_Fail"] == null)
                    {
                        HttpContext.Session["SysUser_Fail"] = "1";
                    }
                    else
                    {
                        var errorCount = int.Parse(HttpContext.Session["SysUser_Fail"].ToString()) + 1;
                        HttpContext.Session["SysUser_Fail"] = errorCount.ToString();
                        if (errorCount >= 3)
                        {
                            HttpContext.Session["SysUser_Fail_Time"] = DateTime.Now.AddMinutes(15);
                            TempData["TempMsg"] = "錯誤達三次，暫停登入15分鐘";
                        }
                    }

                    //清掉方便重新輸入
                    model.reCAPTCHA = "";
                    ModelState.Clear();
                    return View(model);
                    //return RedirectToAction("index", "Login");
            }
        }

        /// <summary>
        /// 權限選擇
        /// </summary>
        /// <returns></returns>
        public ActionResult SelectRole()
        {
            // TODO
            List<string> pages = new List<string>();
            pages.Add("B_Disease_Monitor_Record");


            pages.Add("B_Home_Plant_Doctor_Ticket_List");
            pages.Add("B_Plant_Doctor_Ticket_List");
            pages.Add("B_Plant_Doctor_Schedule");
            pages.Add("B_Plant_Doctor_Schedule");

            SelectRoleViewModel model = new SelectRoleViewModel(SessionHelper.Get("UserRoleCode"));

            return View(model);
        }

        /// <summary>
        /// 登出頁
        /// </summary>
        /// <returns></returns>
        public ActionResult Logout()
        {
            Session.Clear();
            return RedirectToAction("Index");
        }

        public ActionResult TimeOut()
        {
            TempData["TempMsg"] = "連線逾時，請重新登入";//顯示錯誤訊息
            TempData["TempResult"] = "error";
            Session.Clear();
            return RedirectToAction("Index");
        }

        /// <summary>登入頁面使用-產生隨機認證碼圖片 並且設定驗證碼在SESSION值中
        /// 
        /// </summary>
        public void GetValidateCodeImage()
        {
            string Code = Logic_ValidateCodeImage.GetValidateCode(4);
            //將驗證碼紀錄在Session中
            Session["Validate"] = Code;

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
            //抓取偵錯模式標記
            bool bDebug = AppSettingHelper.Get_Debug_Mode();

            //如果是測試模式 則略過驗證碼
            if (bDebug == true)
                return true;

            //如果不是測試模式 有輸入驗證碼 且 驗證碼符合
            if (Session["Validate"] == null)
                return false;

            if (Session["Validate"].ToString().ToUpper() != m_sValidate.ToUpper())
                return false;

            //通過檢查驗證碼檢查
            return true;

        }

        /// <summary>
        /// 忘記密碼
        /// </summary>
        /// <returns></returns>
        public ActionResult ForgotPassword()
        {
            return View(new ForgotPasswordViewModel());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult ForgotPassword(ForgotPasswordViewModel model)
        {
            if (!Check_VerifyCode(model.VerifyCode))
            {
                TempData["Error"] = true;
                TempData["Title"] = "驗證碼錯誤";
                return View(model);
            }

            sysUserInfo userInfo = Service_sysUserInfo.GetDetailByEmail(model.Email);
            if (userInfo == null)
            {
                // 不揭露使用者不存在
                TempData["TempMsg"] = "重設密碼信件已寄出";
                return RedirectToAction("Index");
            }
            string Token = Guid.NewGuid().ToString();
            Service_sysUserInfo.ForgetPassword(userInfo.LoginID, Token);
            IEnumerable<string> originValues = Request.Headers.GetValues("Origin");
            // 寄通知信
            ForgetEmailReplaceModel emailReplaceModel = new ForgetEmailReplaceModel();
            emailReplaceModel.NameRecipient = userInfo.UserName;
            emailReplaceModel.ApplicationTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
            emailReplaceModel.ResetPasswordUrl = String.Concat(originValues.First(), "/Login/ResetPassword?Token=", Token);
            MailReadFile mailReadFile = new MailReadFile();
            string body = mailReadFile.setReplacedEmailDataForForgot("TW_AdminForget", "zh", emailReplaceModel);
            MailHelper mailHelper = new MailHelper();
            string refErrMsg = "";
            mailHelper.SendMail(ref refErrMsg, userInfo.EmailAddress, "植物醫師診斷系統暨作物病蟲害預警系統-忘記密碼申請信", body);
            TempData["TempMsg"] = "重設密碼信件已寄出";
            return RedirectToAction("Index");
        }

        /// <summary>
        /// 忘記密碼，重設密碼
        /// </summary>
        /// <param name="Token"></param>
        /// <returns></returns>
        public ActionResult ResetPassword(string Token)
        {
            if (String.IsNullOrEmpty(Token))
            {
                return RedirectToAction("Index");
            }

            // 確認 Token 有效
            sysUserInfo userInfo = Service_sysUserInfo.GetDetailByToken(Token);
            if (userInfo == null)
            {
                TempData["TempMsg"] = "無效驗證";
                return RedirectToAction("Index");
            }
            
            DateTime now = DateTime.Now;
            TimeSpan ts = now.Subtract((DateTime)userInfo.EditDate);
            double minutes = ts.TotalMinutes;
            // 30分鐘內未使用需要重新申請
            if (userInfo.EditDate.AddMinutes(30) < DateTime.Now)
            {
                TempData["TempMsg"] = "驗證已過期，請重新申請";
                return RedirectToAction("Index");
            }

            return View(userInfo);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult ResetPassword(string LoginID, string NewPwd, string ConfirnPwd)
        {
            var _member = Service_sysUserInfo.GetDetail(LoginID);
            var errMsg = "";
            if (NewPwd != ConfirnPwd)
            {
                errMsg = "兩次密碼輸入不相同";
            }
            else if (PwdStrengHelper.StrongPassword(NewPwd))
            {
                errMsg = "密碼強度不符(長度8個字元以上，包含英文大小寫、數字，以及特殊字元)";
            }

            if (!string.IsNullOrEmpty(errMsg))
            {
                TempData["TempMsg"] = errMsg;
                TempData["TempResult"] = "error";
                return View(_member);
            }

            if (_member != null)
            {
                var newPwd = Utility_Cryptography.Get_HashEncryption(NewPwd);
                List<sysUserPass_log> userPass_Logs = Service_sysUserPass_log.GetList(LoginID);
                foreach (var userPass_log in userPass_Logs)
                {
                    if (Utility_Cryptography.Compare_HashCode(NewPwd, userPass_log.LoginPW))
                    {
                        TempData["TempMsg"] = "密碼不可與近三次內密碼相同";
                        TempData["TempResult"] = "error";
                        return View(_member);
                    }
                }
                Service_sysUserInfo.ResetPassword(LoginID, newPwd);
                errMsg = "密碼已設定，請使用新密碼登入";
                return Content($"<script language='javascript' type='text/javascript'>alert('{errMsg}');window.location = '/';</script>");
            }
            else
            {
                return RedirectToAction("Index");
            }
        }
    }
}