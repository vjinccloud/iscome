using ICCModule.EntityService.Service;
using ICCModule.Entity.Tables;
using ICCModule.Entity.Tables.Platform;
using ICCModule.Helper;
using IscomG2C.Utility;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Net;
using System.Net.Http;
using System.Text;
using Website.ViewModel.Member;
using System.IO;
using Website.Models;
using Website.Helper;
using System.Web.Security;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
using Website.ViewModel;
using ICCModule;

namespace Website.Controllers
{
    public class MemberController : Controller
    {
        /// <summary>
        /// 會員註冊
        /// </summary>
        /// <returns></returns>
        public ActionResult SignUp()
        {
            SignUpViewModel model = new SignUpViewModel();

            return View(model);
        }
        /// <summary>
        /// 會員註冊-送出
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult SignUp([Bind(Include = "Info,VerifyCode,Pwd,CheckPwd,FastInfo,IsFastSign")] SignUpViewModel model)
        {
            if (model.IsFastSign == true)
            {
                model.FastInfo.LoginID = model.FastInfo.Mobile;
                if (model.FastInfo == null || String.IsNullOrEmpty(model.FastInfo.LoginID) || String.IsNullOrEmpty(model.FastInfo.Name) || String.IsNullOrEmpty(model.FastInfo.Mobile))
                {
                    model.IsErr = true;
                    model.ErrMsg = "缺少必要資訊";
                    return View(model);
                }
                if (model.FastInfo.Pwd != model.FastInfo.CheckPwd)
                {
                    model.IsErr = true;
                    model.ErrMsg = "請輸入與第一次密碼相同之密碼";
                    return View(model);
                }
                memberInfo info = Service_MemberInfo.GetAccountDetail(model.FastInfo.LoginID);
                if (info != null)
                {
                    model.IsErr = true;
                    model.ErrMsg = "此帳號已被註冊(預設為手機)";
                    return View(model);
                }

                var checkCode = Service_PhoneRegistCode.GetList(x => x.PhoneNumber == model.FastInfo.Mobile && x.CheckCode == model.FastInfo.CheckCode);
                if (!checkCode.Any())
                {
                    model.IsErr = true;
                    model.ErrMsg = "驗證碼錯誤";
                    return View(model);
                }
                else if (checkCode.All(x => x.ExpireDate < DateTime.Now))
                {
                    model.IsErr = true;
                    model.ErrMsg = "驗證碼已過期，請重新發送";
                    return View(model);
                }
                
                if (Service_MemberInfo.GetList(x => x.VerifyMethod == "SMS" && x.Mobile == model.FastInfo.Mobile).Any())
                {
                    model.IsErr = true;
                    model.ErrMsg = "此驗證方式已被綁定";
                    return View(model);
                }
                if (PwdStrengHelper.StrongPassword(model.FastInfo.Pwd))
                {
                    model.IsErr = true;
                    model.ErrMsg = "密碼強度不符(長度8個字元以上，包含英文大小寫、數字，以及特殊字元)";
                    return View(model);
                }

                var pwd = Utility_Cryptography.Get_HashEncryption(model.FastInfo.Pwd);
                // 新增使用者
                var newMember = new memberInfo
                {
                    LoginID = model.FastInfo.LoginID,
                    Name = model.FastInfo.Name,
                    LoginPass = pwd,
                    OldPasswords = pwd,
                    PasswordExpiredAt = DateTime.Now.AddDays(90),
                    VerifyMethod = "SMS",
                    Mobile = model.FastInfo.Mobile,
                    City = model.FastInfo.City,
                    District = model.FastInfo.District,
                    Status = "Y",
                    ExpiredTimes = 0,
                    RegistFrom = "前台註冊",
                    FacebookId = model.FastInfo.FacebookId,
                    GoogleId = model.FastInfo.GoogleId,
                    LineLoginId = model.FastInfo.LineLoginId,
                    CreatedAt = DateTime.Now,
                    UpdatedAt = DateTime.Now,
                };
                var _regist = Service_MemberInfo.Insert(newMember);
            }
            else
            {
                if (model.Info == null || String.IsNullOrEmpty(model.Info.LoginID) || String.IsNullOrEmpty(model.Info.Name) || String.IsNullOrEmpty(model.Info.Mobile) || String.IsNullOrEmpty(model.Info.VerifyMethod))
                {
                    model.IsErr = true;
                    model.ErrMsg = "缺少必要資訊";
                    return View(model);
                }
                var vAccount = model.Info.VerifyMethod == "SMS" ? model.Info.Mobile : model.Info.Email;
                var vCode = Service_PhoneRegistCode.GetList(x => x.PhoneNumber == vAccount && x.ExpireDate >= DateTime.Now);
                if (vCode.All(x => x.CheckCode != model.VerifyCode))
                {
                    model.IsErr = true;
                    model.ErrMsg = "驗證碼錯誤";
                    return View(model);
                }
                if ((model.VerifyMethod == "SMS" && Service_MemberInfo.GetList(x => x.VerifyMethod == model.Info.VerifyMethod && x.Mobile == model.Info.Mobile).Any()) || (model.VerifyMethod == "Email" && Service_MemberInfo.GetList(x => x.VerifyMethod == model.Info.VerifyMethod && x.Email == model.Info.Email).Any()))
                {
                    model.IsErr = true;
                    model.ErrMsg = "此驗證方式已被綁定";
                    return View(model);
                }
                if (PwdStrengHelper.StrongPassword(model.Pwd))
                {
                    model.IsErr = true;
                    model.ErrMsg = "密碼強度不符(長度8個字元以上，包含英文大小寫、數字，以及特殊字元)";
                    return View(model);
                }

                model.Info.RegistFrom = "前台註冊";
                var pwd = Utility_Cryptography.Get_HashEncryption(model.Pwd);
                model.Info.LoginPass = pwd;
                model.Info.OldPasswords = pwd;
                model.Info.PasswordExpiredAt = DateTime.Now.AddDays(90);
                model.Info.Status = "Y";
                // 新增使用者
                Service_MemberInfo.Insert(model.Info);

            }
            //// 通知等待管理員審核
            //ViewData["Registered"] = true;
            return RedirectToAction(nameof(SignSuccess));
        }

        public ActionResult SignSuccess()
        {
            return View();
        }

        /// <summary>
        /// 註冊後等待驗證的畫面
        /// </summary>
        /// <param name="Account"></param>
        /// <param name="VerifyToken"></param>
        /// <returns></returns>
        public ActionResult SignUpVerfiy()
        {
            // 如果沒有提供驗證方式，代表透過網址直接進來，需要直接導回首頁
            if (String.IsNullOrEmpty(TempData["VerifyMethod"].ToString()))
            {
                TempData["NeedLogin"] = true;
                return RedirectToAction("Index", "Home");
            }

            SignUpVerifyViewModel model = new SignUpVerifyViewModel();
            if (TempData["VerifyMethod"].ToString() == "SMS")
            {
                // 再次提供給下一次的驗證用
                TempData["VerifyMethod"] = TempData["VerifyMethod"];
                model.Mobile = TempData["Mobile"].ToString();
            }
            else
            {
                model.Email = TempData["Email"].ToString();
            }

            return View(model);
        }

        /// <summary>
        /// 驗證帳戶
        /// </summary>
        /// <param name="SmsVerifyCode"></param>
        /// <param name="VerifyCode"></param>
        /// <returns></returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult SignUpVerfiy(SignUpVerifyViewModel model)
        {
            // 驗證簡訊驗證碼 與 頁面頁證碼
            if (TempData["VerifyMethod"].ToString() == "SMS")
            {
                bool pass = true;
                // 頁面驗證錯誤
                if (!Check_VerifyCode(model.VerifyCode))
                {
                    pass = false;
                    ViewData["VerifyCodeError"] = true;
                }
                // 簡訊驗證錯誤

                if (!pass)
                {
                    return View(model);
                }
            }

            return RedirectToAction("Index", "Home");
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

        /// <summary>
        /// 註冊頁面使用-產生隨機認證碼圖片 並且設定驗證碼在SESSION值中
        /// </summary>
        public void GetValidateCodeImage(string SessionName = "ValidateSignUp")
        {
            string Code = Logic_ValidateCodeImage.GetValidateCode(4);
            //將驗證碼紀錄在Session中
            Session[SessionName] = Code;

            //取得圖片物件
            var image = Logic_ValidateCodeImage.GetBinary_ValidateCodeImage(Code);
            /*輸出圖片*/
            Response.Clear();
            Response.ContentType = "image/jpeg";
            Response.Headers.Add("Strict-Transport-Security", "max-age=31536000; includeSubDomains");
            Response.BinaryWrite(image);
        }

        /// <summary>
        /// 檢查與Session值內的驗證碼是否符合?
        /// </summary>
        /// <param name="m_sValidate"></param>
        /// <returns></returns>
        bool Check_VerifyCode(string m_sValidate, string SessionName = "ValidateSignUp")
        {
            m_sValidate = m_sValidate ?? "";
            //抓取偵錯模式標記
            bool bDebug = AppSettingHelper.Get_Debug_Mode();

            //如果是測試模式 則略過驗證碼
            //if (bDebug == true)
            //    return true;

            //如果不是測試模式 有輸入驗證碼 且 驗證碼符合
            if (Session[SessionName] == null)
                return false;

            if (Session[SessionName].ToString().ToUpper() != m_sValidate.ToUpper())
                return false;

            //通過檢查驗證碼檢查
            return true;
        }
        /// <summary>
        /// 忘記密碼
        /// </summary>
        /// <returns></returns>
        public ActionResult Forgot()
        {
            return View();
        }
        /// <summary>
        /// 忘記密碼-送出
        /// </summary>
        /// <param name="req"></param>
        /// <returns></returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        //忘記密碼
        public ActionResult Forgot(ResetPwdModel req)
        {
            // 檢查驗證碼
            if (!Check_VerifyCode(req.ValidateCode, "ValidateForgot"))
            {
                TempData["TempMsg"] = $"驗證碼錯誤";
                TempData["TempResult"] = "error";
                return View(req);
            }

            var thisMember = new memberInfo();
            if (req.ValidateMethod == "SMS")
            {
                thisMember = Service_MemberInfo.GetList(x => x.VerifyMethod == req.ValidateMethod && x.Mobile == req.ValidateData).FirstOrDefault();
            }
            else if (req.ValidateMethod == "Email")
            {
                thisMember = Service_MemberInfo.GetList(x => x.VerifyMethod == req.ValidateMethod && x.Email == req.ValidateData).FirstOrDefault();
            }

            if (thisMember != null && thisMember.ID > 0)
            {
                string vCode = PwdStrengHelper.GetVeriCode(6);
                if (thisMember.VerifyMethod == "SMS" && !string.IsNullOrEmpty(thisMember.Mobile))
                {
                    var sendReq = new SmsReq()
                    {
                        Mobile = thisMember.Mobile,
                        Message = $"{thisMember.Name}您好，帳號{thisMember.LoginID}的驗證碼 {vCode}，請盡速修改密碼(線上植物防疫平台發送勿回)",
                    };
                    var smsResponse = SmsHelper.SendSms(sendReq);
                    if (smsResponse.IsSuccess)
                    {
                        thisMember.CheckCode = vCode;
                        Service_MemberInfo.UpdateCheckCode(thisMember);
                    }
                    else
                    {
                        TempData["TempMsg"] = $"驗證簡訊發送失敗，請聯絡系統管理員";
                        TempData["TempResult"] = "error";
                        return View();
                    }
                }
                else
                {
                    // 寄通知信
                    var _htmlComtent = new System.Text.StringBuilder();
                    _htmlComtent.Append(@"<!DOCTYPE html>
                                         <html>
                                         <head>
                                             <meta http-equiv='content-type' content='text/html; charset=utf-8' />
                                             <meta charset='utf-8' />
                                         </head>
                                         <body>
                                             #NameRecipient#，您好:
                                             <br />
                                             <div>您於 #ApplicationTime# 於「高雄市政府農業局線上植物防疫平台」使</div>
                                             <div>用email重設密碼功能，請輸入下方驗證碼，以利重新設定您的密碼!</div>
                                             <br />
                                             <div>驗證碼：#VeriCode#</div>

                                             <div>***本郵件為系統自動發送，請勿直接回信***</div>
                                         </body>
                                         </html>");
                    _htmlComtent.Replace("#NameRecipient#", thisMember.Name);
                    _htmlComtent.Replace("#ApplicationTime#", DateTime.Now.ToString("yyyy-MM-dd HH:mm"));
                    _htmlComtent.Replace("#VeriCode#", vCode);
                    MailHelper mailHelper = new MailHelper();
                    string refErrMsg = "";
                    if (mailHelper.SendMail(ref refErrMsg, thisMember.Email, "高雄市政府農業局線上植物防疫平台 - 忘記密碼驗證信件", _htmlComtent.ToString()))
                    {
                        thisMember.CheckCode = vCode;
                        Service_MemberInfo.UpdateCheckCode(thisMember);
                    }
                    else
                    {
                        TempData["TempMsg"] = $"驗證信件發送失敗：請聯絡系統管理員";
                        TempData["TempResult"] = "error";
                        return View();
                    }
                }
                Session["UptMemberID"] = thisMember.ID;
                return RedirectToAction(nameof(VerifyCode));
            }
            else
            {
                TempData["TempMsg"] = $"無與選擇之驗證方式相對應之帳號，請選擇與註冊時相同之驗證方式";
                TempData["TempResult"] = "error";
            }
            return View();
        }

        /// <summary>
        /// 個人資料管理，包含基本資料，預約紀錄，活動報名
        /// </summary>
        /// <returns></returns>
        public ActionResult Manage()
        {
            if (Session["MemberID"] == null)
            {
                TempData["NeedLogin"] = true;
                return RedirectToAction("Index", "Home");
            }

            var mId = Convert.ToInt32(Session["MemberID"].ToString());
            var _registPlan = Service_activityRegistration.GetList(x => x.MemberId == mId).Select(x => x.PlanOpenTimeID);
            var openPlan = Service_activityPlanOpenTIme.GetList(x => _registPlan.Contains(x.ID)).Select(x => x.ActivityPlanID);
            var plans = Service_activityPlan.GetList(x => openPlan.Contains(x.ID));
            var _member = Service_MemberInfo.GetDetail(mId);
            var _res = new MemberManageModel()
            {
                MemberData = new ManageMemberEditModel
                {
                    ID = _member.ID,
                    Account = _member.LoginID,
                    Name = _member.Name,
                    Email = _member.Email,
                    VerifyMethod = _member.VerifyMethod,
                    Mobile = _member.Mobile,
                    YearOfBirth = _member.YearOfBirth,
                    NewYear = _member.NewYear,
                    Sexy = _member.Sexy,
                    City = _member.City,
                    District = _member.District,
                    Identify = _member.Identify,
                    BlacklistExpiredAt = _member.BlacklistExpiredAt,
                    ExpiredTimes = _member.ExpiredTimes,
                    LineMessageId = _member.LineMessageId,
                    LineNonce = _member.LineNonce,
                    LineBindCode = _member.LineBindCode,
                    GoogleId = _member.GoogleId,
                    FacebookId = _member.FacebookId,
                    LineLoginId = _member.LineLoginId,
                    LineBindLimit = _member.LineBindLimit,
                },
                PlanList = plans,
                ScheduleList = Service_doctorSchedule.GetList(x => x.MemberInfoID == mId && /*x.Status != "Close" &&*/ x.IsDel == false).OrderByDescending(x => x.ReserveDatetime).ToList()
            };
            return View(_res);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Manage(memberInfo req, string ActionName)
        {
            if (Session["MemberID"] == null)
            {
                TempData["NeedLogin"] = true;
                return RedirectToAction("Index", "Home");
            }

            var mId = Convert.ToInt32(Session["MemberID"].ToString());
            if (mId != req.ID)
            {
                return RedirectToAction(nameof(Manage));
            }
            if (ActionName == "SetBind")
            {
                string allowedChars = "abcdefghijkmnopqrstuvwxyzABCDEFGHJKLMNOPQRSTUVWXYZ0123456789";
                int passwordLength = 8;//密碼長度
                string checkCode = "";
                Random rd = new Random();

                for (int i = 0; i < passwordLength; i++)
                {
                    //allowedChars -> 這個String ，隨機取得一個字，丟給chars[i]
                    checkCode += allowedChars[rd.Next(0, allowedChars.Length)];
                }

                Service_MemberInfo.SetLineBindCode(mId, checkCode);
            }
            else if (ActionName == "CancelBind")
            {
                Service_MemberInfo.BindLineMessage(mId, "");
            }
            else
            {
                Service_MemberInfo.UpdateClient(req);
            }
            return RedirectToAction(nameof(Manage));
        }

        [HttpPost] //刪除方法為POST
        [ValidateAntiForgeryToken]
        public ActionResult CancelPlan(int Id)
        {
            if (Session["MemberID"] == null)
            {
                TempData["NeedLogin"] = true;
                return RedirectToAction("Index", "Home");
            }
            var mid = Convert.ToInt32(Session["MemberID"].ToString());
            var getOpenTime = Service_activityPlanOpenTIme.GetList(Id).Select(x => x.ID);
            var getRegist = Service_activityRegistration.GetList(x => getOpenTime.Contains(x.PlanOpenTimeID) && x.MemberId == mid);
            if (getRegist.Any())
            {
                foreach (var item in getRegist)
                {
                    Service_activityRegistration.Delete(item.ID);
                }
            }

            return RedirectToAction(nameof(Manage));
        }

        //資材使用狀況-List
        public async Task<ActionResult> InventoryList()
        {
            if (Session["MemberID"] == null)
            {
                TempData["NeedLogin"] = true;
                return RedirectToAction("Index", "Home");
            }

            var mId = Convert.ToInt32(Session["MemberID"].ToString());
            var oldInventory = Service_memberInventory.GetList(x => x.MemberId == mId);

            #region 更新資料
            foreach (var item in oldInventory)
            {
                using (var client = new HttpClient())
                {
                    client.DefaultRequestHeaders.Accept.Clear();
                    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                    var filterString = new List<string>();
                    filterString.Add($"許可證號+like+{item.LicenseNumber}");
                    var _url = @"https://data.coa.gov.tw/Service/OpenData/FromM/PesticideData.aspx?$top=10&$skip=0&$filter=" + HttpUtility.UrlEncode(string.Join("and", filterString));

                    var res = await client.GetAsync(new Uri(_url));
                    JArray data = JArray.Parse(await res.Content.ReadAsStringAsync());

                    var getPesticideInfo = new List<PesticideInfoModel>();
                    foreach (var d in data)
                    {
                        JObject obj = JObject.Parse(d.ToString());
                        getPesticideInfo.Add(new PesticideInfoModel()
                        {
                            LicenseWord = obj["許可證字"].ToString(),
                            LicenseNumber = obj["許可證號"].ToString(),
                            ChineseName = obj["中文名稱"].ToString(),
                            PesticideCode = obj["農藥代號"].ToString(),
                            EnglishName = obj["英文名稱"].ToString(),
                            BrandName = obj["廠牌名稱"].ToString(),
                            ChemicalComposition = obj["化學成分"].ToString(),
                            Manufacturer = obj["國外原製造廠商"].ToString(),
                            DosageForm = obj["劑型"].ToString(),
                            Content = obj["含量"].ToString(),
                            ValidityPeriod = obj["有效期限"].ToString(),
                            TradeName = obj["廠商名稱"].ToString(),
                            FRACFungicideResistance = obj["FRAC殺菌劑抗藥性"].ToString(),
                            HRACHerbicideResistance = obj["HRAC除草劑抗藥性"].ToString(),
                            IRACInsecticideResistance = obj["IRAC殺蟲劑抗藥性"].ToString(),
                            PesticideApplicationRange = obj["農藥使用範圍"].ToString(),
                            LicenseMarking = obj["許可證標示"].ToString(),
                        });
                    }
                    var updData = getPesticideInfo.FirstOrDefault(x => x.LicenseNumber == item.LicenseNumber && x.LicenseWord == item.LicenseWord);
                    if (updData != null)
                    {
                        item.LicenseWord = updData.LicenseWord;
                        item.LicenseNumber = updData.LicenseNumber;
                        item.ChineseName = updData.ChineseName;
                        item.Contents = updData.Content;
                        item.DosageForm = updData.DosageForm;
                        item.BrandName = updData.BrandName;
                        item.TradeName = updData.TradeName;
                        item.ValidityPeriod = updData.ValidityPeriod;
                        item.ModifyDate = DateTime.Now;
                        Service_memberInventory.Update(item);
                    }
                }
            }
            #endregion

            var newInventory = Service_memberInventory.GetList(x => x.MemberId == mId);
            return View(newInventory);
        }
        [HttpPost] //刪除方法為POST
        [ValidateAntiForgeryToken]
        public ActionResult InventoryDelete(int Id)
        {
            if (Service_memberInventory.GetDetail(Id) != null)
            {
                Service_memberInventory.Delete(Id);
            }
            return RedirectToAction(nameof(InventoryList));
        }

        //資材使用狀況-農藥使用範圍
        public async Task<ActionResult> InventoryInfo(string id = "")
        {
            var _response = new PesticidesInventoryInfoShowModel()
            {
                PesticideInfos = new PesticideInfoModel(),
                UseScopes = new List<UseScope>(),
            };
            var getPesticideInfo = new List<PesticideInfoModel>();
            if (!string.IsNullOrWhiteSpace(id))
            {
                using (var client = new HttpClient())
                {
                    client.DefaultRequestHeaders.Accept.Clear();
                    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                    var _url = @"https://data.coa.gov.tw/Service/OpenData/FromM/PesticideData.aspx?$top=1&$filter=" + HttpUtility.UrlEncode("許可證號+like+") + id;

                    var res = await client.GetAsync(new Uri(_url));
                    JArray data = JArray.Parse(await res.Content.ReadAsStringAsync());

                    foreach (var d in data)
                    {
                        JObject obj = JObject.Parse(d.ToString());
                        getPesticideInfo.Add(new PesticideInfoModel()
                        {
                            LicenseWord = obj["許可證字"].ToString(),
                            LicenseNumber = obj["許可證號"].ToString(),
                            ChineseName = obj["中文名稱"].ToString(),
                            PesticideCode = obj["農藥代號"].ToString(),
                            EnglishName = obj["英文名稱"].ToString(),
                            BrandName = obj["廠牌名稱"].ToString(),
                            ChemicalComposition = obj["化學成分"].ToString(),
                            Manufacturer = obj["國外原製造廠商"].ToString(),
                            DosageForm = obj["劑型"].ToString(),
                            Content = obj["含量"].ToString(),
                            ValidityPeriod = obj["有效期限"].ToString(),
                            TradeName = obj["廠商名稱"].ToString(),
                            FRACFungicideResistance = obj["FRAC殺菌劑抗藥性"].ToString(),
                            HRACHerbicideResistance = obj["HRAC除草劑抗藥性"].ToString(),
                            IRACInsecticideResistance = obj["IRAC殺蟲劑抗藥性"].ToString(),
                            PesticideApplicationRange = obj["農藥使用範圍"].ToString(),
                            LicenseMarking = obj["許可證標示"].ToString(),
                        });
                    }
                }
                if (getPesticideInfo.Any())
                {
                    _response.PesticideInfos = getPesticideInfo.FirstOrDefault();
                    using (var client = new HttpClient())
                    {
                        client.DefaultRequestHeaders.Accept.Clear();
                        client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                        var _url = _response.PesticideInfos.PesticideApplicationRange;

                        var res = await client.GetAsync(new Uri(_url));
                        JArray data = JArray.Parse(await res.Content.ReadAsStringAsync());

                        foreach (var d in data)
                        {
                            JObject obj = JObject.Parse(d.ToString());
                            _response.UseScopes.Add(new UseScope()
                            {
                                CropName = obj["作物名稱"].ToString(),
                                PestName = obj["病蟲害名稱"].ToString(),
                                UseCount = obj["施用次數"].ToString(),
                                PerHectares = obj["每公頃使用用藥量"].ToString(),
                                DilutionMultiple = obj["稀釋倍數"].ToString(),
                                UseTime = obj["使用時期"].ToString(),
                                UseInterval = obj["施藥間隔"].ToString(),
                                SaveHarvest = obj["安全採收期"].ToString(),
                                Remark = obj["備註"].ToString(),
                                AttentionItem = obj["注意事項"].ToString(),
                            });
                        }
                    }
                }
                else
                {
                    return RedirectToAction(nameof(InventoryList));
                }
            }

            return View(_response);
        }

        /// <summary>
        /// 疫情訊息訂閱
        /// </summary>
        /// <returns></returns>
        public ActionResult Notify()
        {
            if (Session["MemberID"] == null)
            {
                TempData["NeedLogin"] = true;
                return RedirectToAction("Index", "Home");
            }

            memberInfo info = Service_MemberInfo.GetDetail(Convert.ToInt64(SessionHelper.Get_LoginID()));
            if (info == null)
            {
                TempData["NeedLogin"] = true;
                return RedirectToAction("Index", "Home");
            }
            var _res = new NotifyModel
            {
                ID = info.ID,
                SubscribeEpidemic = info.SubscribeEpidemic,
                SubscribeEpidemicList = (info.SubscribeEpidemic ?? "").Split('|').Where(x => !string.IsNullOrEmpty(x)).ToList()
            };

            return View(_res);
        }

        /// <summary>
        /// 更新疫情訊息訂閱關鍵字
        /// </summary>
        /// <param name="KeyWord"></param>
        /// <returns></returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Notify(string KeyWord)
        {
            if (Session["MemberID"] == null)
            {
                TempData["NeedLogin"] = true;
                return RedirectToAction("Index", "Home");
            }

            memberInfo info = Service_MemberInfo.GetDetail(Convert.ToInt64(SessionHelper.Get_LoginID()));
            if (info == null)
            {
                TempData["NeedLogin"] = true;
                return RedirectToAction("Index", "Home");
            }
            info.SubscribeEpidemic = KeyWord;
            BaseResult result = Service_MemberInfo.Update(info);
            ViewData["InfoMsg"] = result.result ? "更新成功" : "更新失敗";
            return RedirectToAction(nameof(Notify));
        }

        //簡訊驗證碼
        public ActionResult VerifyCode()
        {
            if (Session["UptMemberID"] == null)
            {
                TempData["NeedLogin"] = true;
                return RedirectToAction("Index", "Home");
            }
            var getMember = Service_MemberInfo.GetDetail(Extention.ToInt32(Session["UptMemberID"].ToString()));
            if (getMember == null)
            {
                TempData["NeedLogin"] = true;
                return RedirectToAction("Index", "Home");
            }
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult VerifyCode(string VerifyCode)
        {
            if (Session["UptMemberID"] == null)
            {
                TempData["NeedLogin"] = true;
                return RedirectToAction("Index", "Home");
            }
            var getMember = Service_MemberInfo.GetDetail(Extention.ToInt32(Session["UptMemberID"].ToString()));
            if (getMember == null)
            {
                TempData["NeedLogin"] = true;
                return RedirectToAction("Index", "Home");
            }
            else
            {
                if (getMember.CheckCode == VerifyCode)
                {
                    TempData["Reset"] = true;
                    return RedirectToAction(nameof(SetNewPwd));
                }
                else
                {
                    TempData["TempMsg"] = $"驗證碼錯誤";
                    TempData["TempResult"] = "error";
                }
            }
            return View();
        }

        /// <summary>
        /// 重設密碼
        /// </summary>
        /// <returns></returns>
        public ActionResult SetNewPwd()
        {
            if (Session["UptMemberID"] == null)
            {
                TempData["NeedLogin"] = true;
                return RedirectToAction("Index", "Home");
            }
            if (Service_MemberInfo.GetDetail(Extention.ToInt32(Session["UptMemberID"].ToString())) == null)
            {
                TempData["NeedLogin"] = true;
                return RedirectToAction("Index", "Home");
            }
            return View(Service_MemberInfo.GetDetail(Extention.ToInt32(Session["UptMemberID"].ToString())));
        }
        //重設密碼
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult SetNewPwd(int MemberId, string NewPwd, string ConfirnPwd)
        {
            var _member = Service_MemberInfo.GetDetail(MemberId);
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
                return RedirectToAction(nameof(SetNewPwd));
            }

            if (_member != null)
            {
                if (string.IsNullOrEmpty(_member.OldPasswords))
                {
                    var newPwd = Utility_Cryptography.Get_HashEncryption(NewPwd);
                    _member.LoginPass = newPwd;
                    _member.OldPasswords = newPwd;
                    _member.PasswordExpiredAt = DateTime.Now.AddDays(90);
                    // 選擇的驗證方式要寫入驗證時間
                    if (_member.VerifyMethod == "SMS")
                    {
                        _member.SMSVerifiedAt = DateTime.Now;
                    }
                    else
                    {
                        _member.EmailVerifiedAt = DateTime.Now;
                    }
                    Service_MemberInfo.UpdatePwd(_member);
                }
                else
                {
                    var oldPwd = _member.OldPasswords.Split(',').ToList();
                    foreach (var item in oldPwd)
                    {
                        if (Utility_Cryptography.Compare_HashCode(NewPwd, item))
                        {
                            TempData["TempMsg"] = "密碼不可與近三次內密碼相同";
                            TempData["TempResult"] = "error";
                            return RedirectToAction(nameof(SetNewPwd));
                        }
                    }
                    var newPwd = Utility_Cryptography.Get_HashEncryption(NewPwd);
                    oldPwd.Add(newPwd);
                    if (oldPwd.Count > 3)
                    {
                        oldPwd = oldPwd.Skip(oldPwd.Count - 3).Take(3).ToList();
                    }
                    _member.LoginPass = newPwd;
                    _member.OldPasswords = string.Join(",", oldPwd);
                    _member.PasswordExpiredAt = DateTime.Now.AddDays(90);
                    Service_MemberInfo.UpdatePwd(_member);
                }
                Session.Remove("UptMemberID");
                errMsg = "密碼已設定，請使用新密碼登入";
                return Content($"<script language='javascript' type='text/javascript'>alert('{errMsg}');window.location = '/Home/Index';</script>");
            }
            else
            {
                return RedirectToAction("Index", "Home");
            }
        }
        //重設密碼
        public ActionResult UpdatePwd()
        {
            if (Session["MemberID"] == null)
            {
                TempData["NeedLogin"] = true;
                return RedirectToAction("Index", "Home");
            }

            return View();
        }
        //重設密碼
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult UpdatePwd(string FirstPwd, string SecondPwd, string ThirdPwd)
        {
            if (Session["MemberID"] == null)
            {
                TempData["NeedLogin"] = true;
                return RedirectToAction("Index", "Home");
            }

            var _member = Service_MemberInfo.GetDetail(Extention.ToInt32(Session["MemberID"].ToString()));
            var errMsg = "";
            if (_member != null)
            {
                if (!Utility_Cryptography.Compare_HashCode(FirstPwd, _member.LoginPass))
                {
                    TempData["TempMsg"] = "原密碼輸入錯誤";
                    TempData["TempResult"] = "error";
                    return View();
                }
                else if (SecondPwd != ThirdPwd)
                {
                    errMsg = "兩次密碼輸入不相同";
                }
                else if (PwdStrengHelper.StrongPassword(SecondPwd))
                {
                    errMsg = "密碼強度不符(長度8個字元以上，包含英文大小寫、數字，以及特殊字元)";
                }

                if (!string.IsNullOrEmpty(errMsg))
                {
                    TempData["TempMsg"] = errMsg;
                    TempData["TempResult"] = "error";
                    return View();
                }

                var oldPwd = _member.OldPasswords.Split(',').ToList();
                foreach (var item in oldPwd)
                {
                    if (Utility_Cryptography.Compare_HashCode(SecondPwd, item))
                    {
                        TempData["TempMsg"] = "密碼不可與近三次內密碼相同";
                        TempData["TempResult"] = "error";
                        return View();
                    }
                }
                var newPwd = Utility_Cryptography.Get_HashEncryption(SecondPwd);
                oldPwd.Add(newPwd);
                if (oldPwd.Count > 3)
                {
                    oldPwd = oldPwd.Skip(oldPwd.Count - 3).Take(3).ToList();
                }
                _member.LoginPass = newPwd;
                _member.OldPasswords = string.Join(",", oldPwd);
                _member.PasswordExpiredAt = DateTime.Now.AddDays(90);
                Service_MemberInfo.UpdatePwd(_member);

                errMsg = "密碼已設定，請使用新密碼登入";
                return Content($"<script language='javascript' type='text/javascript'>alert('{errMsg}');window.location = '/Home/Index';</script>");
            }
            else
            {
                return RedirectToAction("Index", "Home");
            }
        }
        //使用者條款
        public ActionResult UserTerms()
        {
            List<PlatformCommonView> commonViews = Service_PlatformCommonView.GetList("UserTerms");
            PlatformCommonView model = commonViews.FirstOrDefault<PlatformCommonView>();
            // TODO: 增加點擊次數，需要透過 sessionID 判斷
            var sessionName = $"UserTerms";
            var newsViewSession = Session[sessionName];
            if (newsViewSession == null)
            {
                model.Clicks += 1;
                Service_PlatformCommonView.UpdateClick(model.ID);

                newsViewSession = model.ID;
                Session[sessionName] = model.ID;
            }

            return View(model);
        }
        //隱私權政策
        public ActionResult PrivacyPolicy()
        {
            List<PlatformCommonView> commonViews = Service_PlatformCommonView.GetList("PrivacyPolicy");
            PlatformCommonView model = commonViews.FirstOrDefault<PlatformCommonView>();
            // TODO: 增加點擊次數，需要透過 sessionID 判斷
            var sessionName = $"PrivacyPolicy";
            var newsViewSession = Session[sessionName];
            if (newsViewSession == null)
            {
                model.Clicks += 1;
                Service_PlatformCommonView.UpdateClick(model.ID);

                newsViewSession = model.ID;
                Session[sessionName] = model.ID;
            }

            return View(model);
        }
        //政府網站資料開放宣告
        public ActionResult GovOpenDoc()
        {
            List<PlatformCommonView> commonViews = Service_PlatformCommonView.GetList("GovOpenDoc");
            PlatformCommonView model = commonViews.FirstOrDefault<PlatformCommonView>();
            // TODO: 增加點擊次數，需要透過 sessionID 判斷

            var sessionName = $"GovOpenDoc";
            var newsViewSession = Session[sessionName];
            if (newsViewSession == null)
            {
                model.Clicks += 1;
                Service_PlatformCommonView.UpdateClick(model.ID);

                newsViewSession = model.ID;
                Session[sessionName] = model.ID;
            }

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public JsonResult SendCheckCode(string VerifyType, string VerifyAccount)
        {
            var res = new JsonResult();
            if ((VerifyType == "SMS" && Service_MemberInfo.GetList(x => x.VerifyMethod == VerifyType && x.Mobile == VerifyAccount).Any())
                || (VerifyType == "Email" && Service_MemberInfo.GetList(x => x.VerifyMethod == VerifyType && x.Email == VerifyAccount).Any()))
            {
                res.Data = false;
                return res;
            }
            var checkSend = Service_PhoneRegistCode.GetList(x => x.PhoneNumber == VerifyAccount && x.ExpireDate >= DateTime.Now.AddMilliseconds(2));
            if (checkSend.Any())
            {
                res.Data = true;
            }
            else
            {
                var phoneRegistCode = new PhoneRegistCode
                {
                    PhoneNumber = VerifyAccount,
                    CheckCode = PwdStrengHelper.GetVeriCode(6),
                    CreateDate = DateTime.Now,
                    ExpireDate = DateTime.Now.AddDays(10),
                };
                if (VerifyType == "SMS")
                {
                    SmsReq req = new SmsReq
                    {
                        Mobile = phoneRegistCode.PhoneNumber,
                        Message = $"您的高雄植醫來幫忙驗證碼是 {phoneRegistCode.CheckCode}(有效時間10分鐘)，請勿與他人分享。"
                    };
                    var sendRes = SmsHelper.SendSms(req);
                    res.Data = sendRes.IsSuccess;
                    if (sendRes.IsSuccess)
                    {
                        Service_PhoneRegistCode.Insert(phoneRegistCode);
                    }
                }
                else
                {
                    var _htmlComtent = new StringBuilder();
                    _htmlComtent.Append(@"<!DOCTYPE html>
                                          <html>
                                              <head>
                                                  <meta http-equiv='content-type' content='text/html; charset=utf-8' />
                                                  <meta charset='utf-8' />
                                              </head>
                                              <body>
                                                  您好:
                                                  <br />
                                                  <div>感謝您註冊高雄市政府農業局線上植物防疫平台，</div>
                                                  <div>您的驗證碼（請使用複製貼上）：#RegistCode#</div>
                                                  <br />
                                                  <div>***本郵件為系統自動發送，請勿直接回信***</div>
                                              </body>
                                          </html>");
                    _htmlComtent.Replace("#RegistCode#", phoneRegistCode.CheckCode);
                    MailHelper mailHelper = new MailHelper();
                    string refErrMsg = "";
                    var sendMail = mailHelper.Send(ref refErrMsg, phoneRegistCode.PhoneNumber, $"高雄市政府農業局線上植物防疫平台－會員註冊驗證碼通知信", _htmlComtent.ToString());
                    if (sendMail)
                    {
                        Service_PhoneRegistCode.Insert(phoneRegistCode);
                    }
                    res.Data = sendMail;
                }
            }

            return res;
        }
    }
}