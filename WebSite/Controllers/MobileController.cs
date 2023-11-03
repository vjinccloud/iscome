using ICCModule.EntityService.Service;
using ICCModule.Entity.Tables;
using ICCModule.ViewModel;
using ICCModule.Entity.Tables.Platform;
using System;
using IscomG2C.Utility;
using Newtonsoft.Json;
using System.Web.Mvc;
using System.Text;
using Website.Models;
using Website.ViewModel;
using System.Security.Cryptography;

using System.Collections.Generic;
using System.Net;
using System.IO;
using Website.Controllers;
using ICCModule.Helper;
using System.Threading.Tasks;
using System.Net.Http;

namespace InformationSystem.Controllers
{
    /// <summary>
    /// 
    /// </summary>
    //[InterceptorOfController]//系統操作Log
    public class MobileController : Controller
    {


        [HttpPost]
        [AllowAnonymous]
        public void WebHook([System.Web.Http.FromBody] LineModel req)
        {
            try
            {
                LineReceive.ReceiveMsg(req);

            }
            catch (Exception ex)
            {
                ErrorLog.SaveErrorLog(ex, "LineError", JsonConvert.SerializeObject(req));
            }
        }


        /// <summary>
        /// 
        /// </summary>
        public ActionResult Login()
        {
            return View();
        }

        /// <summary>
        /// 
        /// </summary>
        [HttpPost]
        public async Task<ActionResult> Login(string userid , string account, string password , string VerifyCode, string LoginId, string LoginType)
        {
            string error = "";

            var loginType = new List<string>() { "1","2","3"};
            if (loginType.Contains(LoginType)) //快速登入
            {
                var member = Service_MemberInfo.GetAccountDetail(LoginId, Extention.ToInt32(LoginType));
                if (member != null)
                {
                    if (string.IsNullOrEmpty(member.Status)) error = "帳號尚未開通";
                    else if (member.Status == "N") error = "帳號已遭停用";
                    else account = member.LoginID;
                }
                else
                {
                    error = "未經綁定的帳號";
                }
            }
            else //一般登入
            {
                string SessionCode = HttpContext.Session["HomeValidate"].ToString();
                if (SessionCode != VerifyCode)
                {
                    error = "驗證碼錯誤;\n";
                }

                var usr = Service_MemberInfo.GetAccountDetail(account);
                if (usr == null)
                {
                    error += "找不到帳號;\n";
                }
                else
                {
                    bool match = false;
                    match = usr.CheckPassword(password);
                    //驗證失敗
                    if (match == false)
                    {
                        error += "帳密驗證失敗;\n";
                        //return LoginState.NG;
                    }
                    if (usr.LineUserId != "" && usr.LineUserId != null)
                    {
                        error += "此帳號已綁定;\n";
                    }
                }
            }
            

            if (error == "")
            {
                //產生隨機碼
                //var nonce = GenerateNonce();
                //var res = LineReceive.UpdateNonce(account, nonce);

                //直接更新db,導回指定頁
                LineReceive.UpdateMemberUserID(account, userid);
                LineApi.SetUserMenu(userid, true);
                LineApi.PushText(userid, "帳號綁定成功");


                return Json(new { error = error, url = $"{AppSettingHelper.GetAppSetting("LineOaUrl")}" }, "text/html");
                //return Json(new { error = error, url = $"https://access.line.me/dialog/bot/accountLink?linkToken={linkToken}&nonce={nonce}" }, "text/html");

                //return Redirect($"https://access.line.me/dialog/bot/accountLink?linkToken={linkToken}&nonce={nonce}");
                //返回連結給用戶
                //return View("Link",
                //    $"https://access.line.me/dialog/bot/accountLink?linkToken={linkToken}&nonce={nonce}");
            }
            return Json(new { error = error, url = ""}, "text/html");
        }



        private static RNGCryptoServiceProvider rng = new RNGCryptoServiceProvider();
        private string GenerateNonce()
        {
            var bytes = new Byte[16];
            rng.GetBytes(bytes);
            return Convert.ToBase64String(bytes).Replace("=", "").Replace("+","a");
        }

        /// <summary>
        /// 電子處方箋
        /// </summary>
        public ActionResult Reserve(string userId)
        {
            var list = Service_doctorSchedule.GetLineReserve(userId);

            List<string> doctorList = new List<string>();
            
            if(list != null)
            {
                foreach (var item in list)
                {
                    sysUserInfo doctor = Service_sysUserInfo.GetDetail(item.LoginID);
                    doctorList.Add(doctor.UserName);
                }
            }
           

            ViewBag.list = list;
            ViewBag.doctorList = doctorList;
            return View();
        }

        /// <summary>
        /// 電子處方箋明細
        /// </summary>
        public ActionResult ReserveDetail(string userId, int id)
        {
            doctorSchedule model = Service_doctorSchedule.GetDetail(id);

            sysUserInfo doctor = Service_sysUserInfo.GetDetail(model.LoginID);


            List<FileManagement>  flist = Service_FileManagement.GetList("doctor_Schedule_line", model.ID.ToString());

            ViewBag.doctor = doctor.UserName;
            ViewBag.model = model;
            ViewBag.flist = flist;
            return View();
        }


        /// <summary>
        /// 
        /// </summary>
        public ActionResult LoginSuccess()
        {

            return View();
        }

        /// <summary>
        /// 
        /// </summary>
        public ActionResult Register()
        {

            return View();
        }

        [HttpPost]
        public JsonResult checkAcc(string account)
        {
            string msg = "";
            memberInfo dbMem = Service_MemberInfo.ChkAccount(account);
            if (dbMem == null)
            {
                msg = "無人使用";
            }
            else
            {
                msg = "已有人使用此帳號";
            }
            return Json(new { msg = msg });
        }

        //取消預約
        public JsonResult CancelReserve(int id)
        {
            string msg = "";
            Service_doctorSchedule.CancelReserve(id);
            return Json(new { msg = msg });
        }
        /// <summary>
        /// 儲存註冊
        /// </summary>
        [HttpPost]
        public JsonResult Register( MemberRegisterModel model )
        {
            string error = "";
            string id = "";
            try
            {
                memberInfo dbMem = Service_MemberInfo.ChkAccount(model.Account);

                if(dbMem == null)
                {
                    memberInfo mem = new memberInfo();
                    mem.Name = model.Name;
                    mem.Mobile = model.Mobile;
                    mem.LoginID = model.Account;
                    mem.LoginPass = Utility_Cryptography.Get_HashEncryption(model.Password);
                    mem.District = model.District;
                    mem.City = model.City;
                    mem.Identify = model.Identify;
                    mem.Status = "Y";

                    mem.UpdatedAt = DateTime.Now;
                    mem.CreatedAt = DateTime.Now;

                    Service_MemberInfo.Insert(mem);

                    id = mem.LoginID;
                }
                else
                {
                    error = "有重複帳號";
                }


              
            }
            catch(Exception ex)
            {
                error = ex.ToString();
            }

           
            return Json(new { id = id, error = error });
        }

        /// <summary>
        /// 
        /// </summary>
        public ActionResult RegisterSuccess(string id)
        {

            ViewBag.id = id;
            return View();
        }

        /// <summary>
        /// 
        /// </summary>
        public ActionResult RegisterFast()
        {

            return View();
        }

        /// <summary>
        /// 
        /// </summary>
        public ActionResult Description()
        {
            return View();
        }

        /// <summary>
        /// 
        /// </summary>
        public void GetValidateCodeImage()
        {
            string Code = Logic_ValidateCodeImage.GetValidateCode(4);
            //將驗證碼紀錄在Session中
            HttpContext.Session["HomeValidate"] = Code;


            //取得圖片物件
            var image = Logic_ValidateCodeImage.GetBinary_ValidateCodeImage(Code);
            /*輸出圖片*/
            HttpContext.Response.Clear();
            HttpContext.Response.ContentType = "image/jpeg";
            HttpContext.Response.Headers.Add("Strict-Transport-Security", "max-age=31536000; includeSubDomains");
            HttpContext.Response.BinaryWrite(image);
        }

    }
}