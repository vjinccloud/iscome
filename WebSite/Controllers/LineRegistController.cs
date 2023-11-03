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
using Website.ViewModel.Member;
using Website.Models;

namespace Website.Controllers
{
    /// <summary>
    /// LineOA導入註冊
    /// </summary>
    public class LineRegistController : Controller
    {
        /// <summary>
        /// 一般註冊
        /// </summary>
        /// <returns></returns>
        public ActionResult NormalRegist()
        {
            LineSignUpViewModel model = new LineSignUpViewModel();

            return View(model);
        }
        /// <summary>
        /// 會員註冊-送出
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult NormalRegist([Bind(Include = "Info")] LineSignUpViewModel model , string userid)
        {
            if (model.Info == null || String.IsNullOrEmpty(model.Info.LoginID) || String.IsNullOrEmpty(model.Info.Name) || String.IsNullOrEmpty(model.Info.Mobile))
            {
                model.IsErr = true;
                model.ErrMsg = "缺少必要資訊";
                return View(model);
            }

            var checkCode = Service_PhoneRegistCode.GetList(x => x.PhoneNumber == model.Info.Mobile && x.CheckCode == model.Info.CheckCode);
            if (checkCode.All(x => x.ExpireDate < DateTime.Now))
            {
                model.IsErr = true;
                model.ErrMsg = "驗證碼已過期，請重新發送";
                return View(model);
            }
            else if (!checkCode.Any())
            {
                model.IsErr = true;
                model.ErrMsg = "驗證碼錯誤";
                return View(model);
            }
            if (Service_MemberInfo.GetList(x => x.VerifyMethod == "SMS" && x.Mobile == model.Info.Mobile).Any())
            {
                model.IsErr = true;
                model.ErrMsg = "此驗證方式已被綁定";
                return View(model);
            }
            if (PwdStrengHelper.StrongPassword(model.Info.Pwd))
            {
                model.IsErr = true;
                model.ErrMsg = "密碼強度不符(長度8個字元以上，包含英文大小寫、數字，以及特殊字元)";
                return View(model);
            }

            var pwd = Utility_Cryptography.Get_HashEncryption(model.Info.Pwd);
            // 新增使用者
            var newMember = new memberInfo
            {
                LoginID = model.Info.LoginID,
                Name = model.Info.Name,
                LoginPass = pwd,
                OldPasswords = pwd,
                PasswordExpiredAt = DateTime.Now.AddDays(90),
                VerifyMethod = "SMS",
                Mobile = model.Info.Mobile,
                City = model.Info.City,
                District = model.Info.District,
                Identify = model.Info.Identify,
                Status = "Y",
                ExpiredTimes = 0,
                RegistFrom = "Line註冊",
                CreatedAt = DateTime.Now,
                UpdatedAt = DateTime.Now,
            };
            var _regist = Service_MemberInfo.Insert(newMember);

            if (_regist.result)
            {
                LineApi.PushText(userid, "恭喜您完成會員註冊!\n請再點選 會員註冊/綁定 才能完成帳號綁定喔!");

                return RedirectToAction(nameof(NormalRegistComplete), new { ac = newMember.LoginID });
            }
            model.IsErr = true;
            model.ErrMsg = _regist.Msg;
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public JsonResult CheckAccountAvailable(string Account)
        {
            memberInfo info = Service_MemberInfo.GetAccountDetail(Account);

            var res = new JsonResult();
            res.Data = info == null ? true : false;
            return res;
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public JsonResult SendCheckCode(string PhoneNum)
        {
            var res = new JsonResult();
            var checkSend = Service_PhoneRegistCode.GetList(x => x.PhoneNumber == PhoneNum && x.ExpireDate >= DateTime.Now);
            if (checkSend.Any())
            {
                res.Data = true;
            }
            var phoneRegistCode = new PhoneRegistCode
            {
                PhoneNumber = PhoneNum,
                CheckCode = PwdStrengHelper.GetVeriCode(6),
                CreateDate = DateTime.Now,
                ExpireDate = DateTime.Now.AddDays(10),
            };
            SmsReq req = new SmsReq
            {
                Mobile = phoneRegistCode.PhoneNumber,
                Message = $"您的高雄植醫來幫忙驗證碼是 {phoneRegistCode.CheckCode}(有效時間10分鐘)，請勿與他人分享。"
            };
            var sendRes = SmsHelper.SendSms(req);
            if (sendRes.IsSuccess)
            {
                Service_PhoneRegistCode.Insert(phoneRegistCode);
            }

            res.Data = sendRes.IsSuccess;
            return res;
        }
        /// <summary>
        /// 一般註冊-註冊完成
        /// </summary>
        /// <param name="Acc"></param>
        /// <returns></returns>
        public ActionResult NormalRegistComplete(string ac = "")
        {
            var getAcc = Service_MemberInfo.GetAccountDetail(ac);
            if (getAcc != null)
            {
                return View(getAcc);
            }
            return RedirectToAction("Index", "Home");
        }

        /// <summary>
        /// 快速註冊
        /// </summary>
        /// <returns></returns>
        public ActionResult FastRegist(int loginType = 0, string loginKey = "")
        {
            LineSignUpViewModel model = new LineSignUpViewModel();
            var checkRegist = Service_MemberInfo.GetAccountDetail(loginKey, loginType);
            if (checkRegist != null)
            {
                model.IsErr = true;
                model.ErrMsg = "此社群帳號已綁定";
                model.IsClose = true;
            }

            if ((loginType != 1 && loginType != 2 && loginType != 3) || string.IsNullOrEmpty(loginKey))
            {
                return RedirectToAction("Index", "Home");
            }

            switch (loginType)
            {
                case 1: model.Info.GoogleId = loginKey; break;
                case 2: model.Info.FacebookId = loginKey; break;
                case 3: model.Info.LineId = loginKey; break;
            }
            return View(model);
        }
        /// <summary>
        /// 快速註冊-送出
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult FastRegist([Bind(Include = "Info")] LineSignUpViewModel model)
        {
            model.Info.LoginID = model.Info.Mobile;
            if (model.Info == null || String.IsNullOrEmpty(model.Info.LoginID) || String.IsNullOrEmpty(model.Info.Name) || String.IsNullOrEmpty(model.Info.Mobile))
            {
                model.IsErr = true;
                model.ErrMsg = "缺少必要資訊";
                return View(model);
            }

            memberInfo info = Service_MemberInfo.GetAccountDetail(model.Info.LoginID);
            if (info != null)
            {
                model.IsErr = true;
                model.ErrMsg = "此帳號已被註冊(預設為手機)";
                return View(model);
            }

            var checkCode = Service_PhoneRegistCode.GetList(x => x.PhoneNumber == model.Info.Mobile && x.CheckCode == model.Info.CheckCode);
            if (checkCode.All(x => x.ExpireDate < DateTime.Now))
            {
                model.IsErr = true;
                model.ErrMsg = "驗證碼已過期，請重新發送";
                return View(model);
            }
            else if (!checkCode.Any())
            {
                model.IsErr = true;
                model.ErrMsg = "驗證碼錯誤";
                return View(model);
            }
            if (Service_MemberInfo.GetList(x => x.VerifyMethod == "SMS" && x.Mobile == model.Info.Mobile).Any())
            {
                model.IsErr = true;
                model.ErrMsg = "此驗證方式已被綁定";
                return View(model);
            }
            if (PwdStrengHelper.StrongPassword(model.Info.Pwd))
            {
                model.IsErr = true;
                model.ErrMsg = "密碼強度不符(長度8個字元以上，包含英文大小寫、數字，以及特殊字元)";
                return View(model);
            }

            var pwd = Utility_Cryptography.Get_HashEncryption(model.Info.Pwd);
            // 新增使用者
            var newMember = new memberInfo
            {
                LoginID = model.Info.LoginID,
                Name = model.Info.Name,
                LoginPass = pwd,
                OldPasswords = pwd,
                PasswordExpiredAt = DateTime.Now.AddDays(90),
                VerifyMethod = "SMS",
                Mobile = model.Info.Mobile,
                City = model.Info.City,
                District = model.Info.District,
                Identify = model.Info.Identify,
                Status = "Y",
                ExpiredTimes = 0,
                RegistFrom = "Line註冊",
                CreatedAt = DateTime.Now,
                UpdatedAt = DateTime.Now,
                FacebookId = model.Info.FacebookId,
                GoogleId = model.Info.GoogleId,
                LineLoginId = model.Info.LineId,
            };
            var _regist = Service_MemberInfo.Insert(newMember);

            if (_regist.result)
            {
                return RedirectToAction(nameof(FastRegistComplete), new { ac = newMember.LoginID });
            }
            model.IsErr = true;
            model.ErrMsg = _regist.Msg;
            return View(model);
        }
        /// <summary>
        /// 快速註冊-註冊完成
        /// </summary>
        /// <param name="Acc"></param>
        /// <returns></returns>
        public ActionResult FastRegistComplete(string ac = "")
        {
            var getAcc = Service_MemberInfo.GetAccountDetail(ac);
            if (getAcc != null)
            {
                return View();
            }
            return RedirectToAction("Index", "Home");
        }

        public ActionResult LineCallBack(string code = "", string userid = "", string state = "")
        {
            var url = "https://api.line.me/oauth2/v2.1/token";
            var request = HttpWebRequest.Create(url) as HttpWebRequest;
            string result = "";
            string profileResult = "";
            var d = new RequestLineProfile();
            if (request != null)
            {
                try
                {
                    request.Method = "POST";    // 方法
                    request.KeepAlive = true; //是否保持連線
                    request.ContentType = "application/x-www-form-urlencoded";

                    var gType = "authorization_code";
                    var rUri = $"{Request.Url.Scheme}://{Request.Url.Authority}/LineRegist/LineCallBack" + (!string.IsNullOrEmpty(userid) ? $"?userid={userid}" : "");
                    var cID = AppSettingHelper.GetAppSetting("ChannelId");
                    var cSecret = AppSettingHelper.GetAppSetting("ChannelSecret");

                    var param = $"grant_type={gType}&code={code}&redirect_uri={rUri}&client_id={cID}&client_secret={cSecret}";
                    var bs = Encoding.UTF8.GetBytes(param);

                    using (var reqStream = request.GetRequestStream())
                    {
                        reqStream.Write(bs, 0, bs.Length);
                    }
                    using (var response = request.GetResponse())
                    {
                        var sr = new StreamReader(response.GetResponseStream());
                        result = sr.ReadToEnd();
                        sr.Close();
                    }
                    //Response.Write(result);
                    var data = (JsonConvert.DeserializeObject<RequestLineAtJson>(result));

                    #region
                    string targetUrl = "https://api.line.me/v2/profile";

                    HttpWebRequest request2 = HttpWebRequest.Create(targetUrl) as HttpWebRequest;
                    request2.Method = "GET";
                    request2.ContentType = "application/x-www-form-urlencoded";
                    request2.Headers.Set("Authorization", "Bearer " + data.access_token);

                    // 取得回應資料
                    using (HttpWebResponse response = request2.GetResponse() as HttpWebResponse)
                    {
                        using (StreamReader sr = new StreamReader(response.GetResponseStream()))
                        {
                            profileResult = sr.ReadToEnd();
                        }
                    }
                    d = JsonConvert.DeserializeObject<RequestLineProfile>(profileResult);

                    #region 會員專區綁定
                    if (state == "bfmml")
                    {
                        if (d != null && !string.IsNullOrEmpty(d?.userId))
                        {
                            if (Session["MemberID"] == null)
                            {
                                TempData["NeedLogin"] = true;
                                return RedirectToAction("Index", "Home");
                            }
                            var mId = Convert.ToInt32(Session["MemberID"].ToString());
                            var _member = Service_MemberInfo.GetDetail(mId);
                            if (_member != null) Service_MemberInfo.SetFastBind(mId, 3, d.userId);
                        }
                        else return RedirectToAction("Index", "Home");
                    }
                    #endregion
                    #region
                    if (state == "lfhin")
                    {
                        if (d != null && !string.IsNullOrEmpty(d?.userId))
                        {
                            return RedirectToAction("Index", "Home", new { loginType = 3, loginKey = d.userId });
                        }
                        else return RedirectToAction("Index", "Home");
                    }
                    #endregion

                    if (state == "qwera") return RedirectToAction("Login", "Mobile", new { loginType = 3, loginKey = d?.userId, userid = userid });
                    else if (state == "bfmml") return RedirectToAction("Manage", "Member");
                    else if (state == "frfws") return RedirectToAction("SignUp", "Member", new { loginKey = d?.userId});
                    else return RedirectToAction(nameof(FastRegist), new { loginType = 3, loginKey = d?.userId });
                    #endregion
                }
                catch (Exception ex)
                {
                    ErrorLog.SaveErrorLog($"{ex.Message}&{result}&{profileResult}");
                }
            }
            return RedirectToAction("Index", "Home");
        }
    }

    public class RequestLineAtJson
    {
        public string access_token { get; set; }

        public string expires_in { get; set; }

        public string id_token { get; set; }

        public string refresh_token { get; set; }

        public string scope { get; set; }

        public string token_type { get; set; }
    }

    public class RequestLineProfile
    {
        public string userId { get; set; }

        public string displayName { get; set; }

        public string pictureUrl { get; set; }
    }
}