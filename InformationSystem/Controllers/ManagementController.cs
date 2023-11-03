using ICCModule.Entity.Tables.Platform;
using ICCModule.Entity.Tables;
using ICCModule.EntityService.Service;
using ICCModule.Repository;
using IscomG2C.Utility;
using InformationSystem.ViewModel.Management;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Threading.Tasks;
using ICCModule.Helper;
using InformationSystem.Models;
using System.Web.Security;
using InformationSystem.Helper;
using ICCModule.ActionFilters;
using ICCModule;
using InformationSystem.Models.NotifyTemplate.SMS;
using System.Web.Security.AntiXss;
using System.Linq.Expressions;

namespace InformationSystem.Controllers
{
    [LoginCheckFilter]//掛載此Filter，如果沒登入，就導去登入頁
    [InterceptorOfController]//系統操作Log
    public class ManagementController : Controller
    {
        //簡介資料管理
        public ActionResult Introduce()
        {
            List<PlatformCommonView> commonViews = Service_PlatformCommonView.GetList("Introduce");
            PlatformCommonView model = commonViews.FirstOrDefault<PlatformCommonView>();
            if (model == null)
            {
                model = new PlatformCommonView();
            }

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Introduce(PlatformCommonView commonView)
        {
            commonView.Title = "簡介資料管理";
            commonView.LastEditorName = SessionHelper.Get_UserName();
            if (Convert.ToBoolean(commonView.ID))
            {
                Service_PlatformCommonView.Update(commonView);
            }
            else
            {
                commonView.GroupName = "Introduce";
                commonView.Show = true;
                commonView.UpdatedAt = DateTime.Now;
                Service_PlatformCommonView.Insert(commonView);
            }

            return RedirectToAction(nameof(Introduce));
        }

        //使用者條款管理
        public ActionResult UserTerms()
        {
            List<PlatformCommonView> commonViews = Service_PlatformCommonView.GetList("UserTerms");
            PlatformCommonView model = commonViews.FirstOrDefault<PlatformCommonView>();
            if (model == null)
            {
                model = new PlatformCommonView();
            }

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult UserTerms(PlatformCommonView commonView)
        {
            commonView.Title = "使用者條款管理";
            commonView.LastEditorName = SessionHelper.Get_UserName();
            if (Convert.ToBoolean(commonView.ID))
            {
                Service_PlatformCommonView.Update(commonView);
            }
            else
            {
                commonView.GroupName = "UserTerms";
                commonView.Show = true;
                commonView.UpdatedAt = DateTime.Now;
                Service_PlatformCommonView.Insert(commonView);
            }

            return RedirectToAction(nameof(UserTerms));
        }

        //隱私權政策管理
        public ActionResult PrivacyPolicy()
        {
            List<PlatformCommonView> commonViews = Service_PlatformCommonView.GetList("PrivacyPolicy");
            PlatformCommonView model = commonViews.FirstOrDefault<PlatformCommonView>();
            if (model == null)
            {
                model = new PlatformCommonView();
            }

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult PrivacyPolicy(PlatformCommonView commonView)
        {
            commonView.Title = "隱私權政策管理";
            commonView.LastEditorName = SessionHelper.Get_UserName();
            if (Convert.ToBoolean(commonView.ID))
            {
                Service_PlatformCommonView.Update(commonView);
            }
            else
            {
                commonView.GroupName = "PrivacyPolicy";
                commonView.Show = true;
                commonView.UpdatedAt = DateTime.Now;
                Service_PlatformCommonView.Insert(commonView);
            }

            return RedirectToAction(nameof(PrivacyPolicy));
        }

        //政府網站資料開放宣告管理
        public ActionResult GovOpenDoc()
        {
            List<PlatformCommonView> commonViews = Service_PlatformCommonView.GetList("GovOpenDoc");
            PlatformCommonView model = commonViews.FirstOrDefault<PlatformCommonView>();
            if (model == null)
            {
                model = new PlatformCommonView();
            }

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult GovOpenDoc(PlatformCommonView commonView)
        {
            commonView.Title = "政府網站資料開放宣告管理";
            commonView.LastEditorName = SessionHelper.Get_UserName();
            if (Convert.ToBoolean(commonView.ID))
            {
                Service_PlatformCommonView.Update(commonView);
            }
            else
            {
                commonView.GroupName = "GovOpenDoc";
                commonView.Show = true;
                commonView.UpdatedAt = DateTime.Now;
                Service_PlatformCommonView.Insert(commonView);
            }

            return RedirectToAction(nameof(Introduce));
        }

        //消息管理-列表
        public ActionResult NewsList(string Code = "", bool? Show = null, bool? PopupShow = null, int Page = 1, List<string> DeleteIDs = null)
        {
            // 刪除指定ID的消息
            if (DeleteIDs != null)
            {
                try
                {
                    DeleteIDs.ForEach(delegate (string id)
                    {
                        Service_New.Delete(int.Parse(id));
                    });
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.Message);
                }
            }

            var defCodes = Service_defCode.GetList("NewsType");
            defCodes.Sort();

            Expression<Func<New, bool>> filter = x => true;
            if (!string.IsNullOrEmpty(Code)) filter = filter.And(x => x.Code == Code);
            if (Show.HasValue) filter = filter.And(x => x.Show == Show);
            if (PopupShow.HasValue) filter = filter.And(x => x.PopupShow == PopupShow);

            var news = Service_New.GetList(filter);

            NewsListViewModel model = new NewsListViewModel()
            {
                Types = defCodes,
                News = news.OrderByDescending(x => x.CreatedAt).Skip((Page - 1) * 20).Take(20).ToList(),
                CurrentPage = 1,
                TotalPage = (int)Math.Ceiling((decimal)news.Count() / 20),
                PageCounts = 30,
                Page = Page,
                NewsFilters = new NewsFilter
                {
                    Show = Show,
                    PopupShow = PopupShow,
                    Code = Code,
                }
            };

            return View(model);
        }

        //消息管理-內容
        public ActionResult NewsInfo(string ID)
        {
            var defCodes = Service_defCode.GetList("NewsType");
            defCodes.Sort();
            var model = new NewsInfoViewModel()
            {
                Types = defCodes,
                New = new New(),
            };

            if (ID != null)
            {
                model.New = Service_New.GetDetail(int.Parse(ID));
            }
            else
            {
                model.New = new New();
                model.New.StartDate = null;
            }

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult NewsInfo(New NewModel, List<HttpPostedFileBase> Files)
        {

            NewModel.Kind = "NewsType";

            if (Convert.ToBoolean(NewModel.ID))
            {
                Service_New.Update(NewModel);
            }
            else
            {
                Service_New.Insert(NewModel);

                var checkMember = Service_MemberInfo.GetList(x => true).Where(x => !string.IsNullOrEmpty(x.Email) && x.Email.Contains("@") && !string.IsNullOrEmpty(x.SubscribeEpidemic));

                if (NewModel.StartDate < DateTime.Now && (!NewModel.EndDate.HasValue || NewModel.EndDate >= DateTime.Now.Date) && NewModel.Show)
                {
                    var rDate = NewModel.StartDate ?? NewModel.CreatedAt;
                    var rDateStr = (rDate.Year - 1911) + "/" + rDate.ToString("MM/dd");
                    foreach (var item in checkMember)
                    {
                        var hasSend = false;
                        var keyWord = item.SubscribeEpidemic.Split('|');
                        int i = 0;
                        while (!hasSend && keyWord.Count() > i)
                        {
                            var kw = keyWord[i];
                            if (!string.IsNullOrEmpty(kw) && NewModel.Title.Contains(kw))
                            {
                                var _htmlComtent = new System.Text.StringBuilder();
                                _htmlComtent.Append(@"<!DOCTYPE html>
                                          <html>
                                              <head>
                                                  <meta http-equiv='content-type' content='text/html; charset=utf-8' />
                                                  <meta charset='utf-8' />
                                              </head>
                                              <body>
                                                  #NameRecipient# 您好:
                                                  <br />
                                                  <div>您所訂閱的疫情訊息關鍵字【#Keyword#】，最新消息已有新上架資訊。</div>
                                                  <div>發布日期：#ReleaseDate#</div>
                                                  <div>最新消息主旨：#NewsTitle#</div>
                                                  <br />
                                                  <div><a href='#NewsUrl#' target='_blank'>檢視最新消息內容</a></div>
                                                  <br />
                                                  <div>***本郵件為系統自動發送，請勿直接回信***</div>
                                              </body>
                                          </html>");
                                _htmlComtent.Replace("#NameRecipient#", item.Name);
                                _htmlComtent.Replace("#Keyword#", kw);
                                _htmlComtent.Replace("#ReleaseDate#", rDateStr);
                                _htmlComtent.Replace("#NewsTitle#", NewModel.Title);
                                _htmlComtent.Replace("#NewsUrl#", AppSettingHelper.GetAppSetting("Front_HostName") + $"/Home/News_Detail?ID={NewModel.ID}");
                                MailHelper mailHelper = new MailHelper();
                                string refErrMsg = "";
                                var a = mailHelper.Send(ref refErrMsg, item.Email, $"高雄市政府農業局線上植物防疫平台 - 疫情訊息訂閱【{kw}】", _htmlComtent.ToString());

                                hasSend = true;
                            }
                            i++;
                        }
                    }
                }
            }

            try
            {
                var _oldFile = Service_FileManagement.GetList("News", NewModel.ID.ToString());
                if (_oldFile.Any())
                {
                    foreach (var item in _oldFile.Where(x => !NewModel.EditOldFiles.Contains(x.ID)))
                    {
                        var thisFile = _oldFile.FirstOrDefault(x => x.ID == item.ID);
                        if (thisFile == null)
                        {
                            continue;
                        }
                        else
                        {
                            string _path = thisFile.FilePath;
                            if (thisFile.FilePath.StartsWith("/UploadedFiles/News")) _path = Server.MapPath("~" + thisFile.FilePath);
                            if (System.IO.File.Exists(_path))
                            {
                                System.IO.File.Delete(_path);
                            }
                            Service_FileManagement.Delete((int)thisFile.ID);
                        }
                    }
                }

                if (Files.Count > 0)
                {
                    string baseDirectory = "/UploadedFiles/News";
                    foreach (var file in Files)
                    {
                        if (file == null)
                        {
                            continue;
                        }
                        string _FileName = Path.GetFileName(file.FileName);
                        string _directory = Server.MapPath("~" + baseDirectory);
                        FileHelper.CreateFolder(_directory);

                        string _path = Path.Combine(_directory, _FileName);
                        file.SaveAs(_path);
                        var url = String.Concat(baseDirectory, '/', _FileName);
                        Service_FileManagement.Insert(new FileManagement()
                        {
                            TableName = "News",
                            TableID = NewModel.ID.ToString(),
                            FileName = file.FileName,
                            FilePath = url,
                            FileLength = file.ContentLength,
                            FileType = file.ContentType,
                            MD5 = FileLogic.CalculateMD5(file.InputStream),
                        });
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }

            return RedirectToAction(nameof(NewsList));
        }

        //連結管理-列表
        public ActionResult LinkList(List<string> DeleteIDs = null)
        {
            // 刪除指定ID的消息
            if (DeleteIDs != null)
            {
                try
                {
                    DeleteIDs.ForEach(delegate (string id)
                    {
                        Service_Link.Delete(int.Parse(id));
                    });
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.Message);
                }
            }

            LinkListViewModel model = new LinkListViewModel()
            {
                Links = Service_Link.GetList(),
                CurrentPage = 1,
                TotalPage = 1,
                PageCounts = 30
            };

            return View(model);
        }

        //連結管理-內容
        public ActionResult LinkInfo(string ID)
        {
            Link model;
            if (ID != null)
            {
                model = Service_Link.GetDetail(int.Parse(ID));
            }
            else
            {
                model = new Link();
            }

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult LinkInfo(Link model, HttpPostedFileBase file = null)
        {
            if (file != null)
            {
                try
                {
                    var baseDirectory = "/UploadedFiles/Links";
                    string _FileName = Path.GetFileName(file.FileName);
                    string _directory = Server.MapPath("~" + baseDirectory);
                    FileHelper.CreateFolder(_directory);
                    string _path = Path.Combine(_directory, _FileName);
                    file.SaveAs(_path);
                    var url = String.Concat(baseDirectory, '/', _FileName);
                    Service_FileManagement.Insert(new FileManagement()
                    {
                        TableName = "Links",
                        TableID = model.ID.ToString(),
                        FileName = file.FileName,
                        FilePath = _path,
                        FileLength = file.ContentLength,
                        FileType = file.ContentType,
                        MD5 = FileLogic.CalculateMD5(file.InputStream),
                    });
                    model.ImagePath = url;
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.Message);
                }
            }
            if (Convert.ToBoolean(model.ID))
            {
                Service_Link.Update(model);
            }
            else
            {
                Service_Link.Insert(model);
            }

            return RedirectToAction(nameof(LinkList));
        }

        //問答管理-列表
        public ActionResult QAList(string Code = "", List<string> DeleteIDs = null)
        {
            // 刪除指定ID的消息
            if (DeleteIDs != null)
            {
                try
                {
                    DeleteIDs.ForEach(delegate (string id)
                    {
                        Service_QuestionManagement.Delete(int.Parse(id));
                    });
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.Message);
                }
            }

            QuestionManagementListViewModel model = new QuestionManagementListViewModel()
            {
                Types = Service_defCode.GetList("QuestionType"),
                List = Service_QuestionManagement.GetList(Code),
                CurrentPage = 1,
                TotalPage = 1,
                PageCounts = 30
            };

            return View(model);
        }
        //問答管理-內容
        public ActionResult QAInfo(string ID)
        {
            var model = new QuestionManagementInfoViewModel()
            {
                Types = Service_defCode.GetList("QuestionType"),
                QuestionManagement = new QuestionManagement(),
            };
            if (ID != null)
            {
                model.QuestionManagement = Service_QuestionManagement.GetDetail(int.Parse(ID));
            }

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult QAInfo(string Code, string ID, string Question, string Answer)
        {
            if (Convert.ToBoolean(int.Parse(ID)))
            {
                var QAModel = Service_QuestionManagement.GetDetail(int.Parse(ID));
                QAModel.Type = Code;
                QAModel.Question = Question;
                QAModel.Answer = Answer;
                Service_QuestionManagement.Update(QAModel);
            }
            else
            {
                QuestionManagement model = new QuestionManagement()
                {
                    Type = Code,
                    Question = Question,
                    Answer = Answer
                };
                Service_QuestionManagement.Insert(model);
            }
            return RedirectToAction(nameof(QAList));
        }

        /// <summary>
        /// 帳號管理-列表
        /// </summary>
        /// <param name="Status"></param>
        /// <param name="Identify"></param>
        /// <param name="Special"></param>
        /// <param name="act"></param>
        /// <param name="ChkAccount"></param>
        /// <param name="ChkAccountSource"></param>
        /// <param name="Type"></param>
        /// <returns></returns>
        public ActionResult MemberList(string Status, string Identify, string RegistFrom, string act, List<string> DeleteIDs = null)
        {
            // 刪除指定ID的消息
            if (DeleteIDs != null)
            {
                try
                {
                    DeleteIDs.ForEach(delegate (string TypeAccount)
                    {
                        string[] arr = TypeAccount.Split(',');
                        string UserType = arr[0];
                        string Account = arr[1];

                        if (UserType == "System")
                        {
                            Service_sysUserInfo.Delete(Account);
                        }
                        else if (UserType == "Member")
                        {
                            Service_MemberInfo.DeleteByAccount(Account);
                        }
                    });
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.Message);
                }
            }
            Expression<Func<memberInfo, bool>> filter = x => true;

            if (string.IsNullOrEmpty(Identify)) Identify = "";
            else filter = filter.And(x => x.Identify == Identify);
            if (string.IsNullOrEmpty(RegistFrom)) RegistFrom = "";
            else filter = filter.And(x => (x.RegistFrom ?? "前台註冊") == RegistFrom);
            if (string.IsNullOrEmpty(Status)) Status = "";
            else
            {
                if (Status == "1") filter = filter.And(x => x.Status == "Y");
                if (Status == "2") filter = filter.And(x => x.Status == "N");
                if (Status == "3") filter = filter.And(x => x.Status == null);
            }
            MemberListViewModel model = new MemberListViewModel()
            {
                Members = Service_MemberInfo.GetList(filter),
                Identify_Filter = Identify,
                Status_Filter = Status,
                CurrentPage = 1,
                TotalPage = 1,
                PageCounts = 10,
                RegistFrom = RegistFrom,
            };

            #region 匯出
            if (act == "export")
            {
                int i = 0;
                var _export = model.Members.Select(x => new
                {

                    項次 = (i += 1),
                    帳號 = x.LoginID,
                    姓名 = x.Name,
                    電子信箱 = x.Email,
                    角色別 = model.defCodes.FirstOrDefault(o => o.Code == x.Identify)?.Name ?? "",
                    狀態 = x.Status == "Y" ? "啟用" : "未啟用",
                }).ToList();

                var _output = ExcelHelper.ConvertToDataTable(_export);
                //_output.Columns["業者名稱"].ColumnName = "生產者/業者名稱";

                return File(ExcelHelper.RenderDataTableToExcel(_output), "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", $"使用者列表{DateTime.Now.ToString("yyyyMMddHHmmss")}.xlsx");
            }
            #endregion

            return View(model);
        }



        /// <summary>
        /// 前台會員帳號管理-編輯
        /// </summary>
        /// <param name="ID"></param>
        /// <returns></returns>
        public ActionResult MemberEdit(long ID)
        {
            memberInfo info = Service_MemberInfo.GetDetail(ID);
            // 代表尚未通過審核，啟用或禁用
            if (String.IsNullOrEmpty(info.Status))
            {
                return RedirectToAction(nameof(MemberCheck), new { Type = "Member", Account = info.LoginID });
            }

            MemberEditViewModel model = new MemberEditViewModel()
            {
                memberInfo = info,
            };
            if (model.memberInfo.Identify == null) model.memberInfo.Identify = "";
            if (model.memberInfo.District == null) model.memberInfo.District = "";

            return View(model);
        }

        //帳號管理-編輯
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult MemberEdit(memberInfo model, String ChkAccountSource)
        {
            model.UpdatedAt = DateTime.Now;
            if (string.IsNullOrEmpty(model.Mobile))
                model.Mobile = "";
            if (string.IsNullOrEmpty(model.Identify))
                model.Identify = "";

            Service_MemberInfo.Update(model);
            return RedirectToAction(nameof(MemberList));
        }

        //帳號管理-審核
        public ActionResult MemberCheck(string Type, string Account)
        {
            MemberCheckViewModel model = new MemberCheckViewModel();
            if (Type == "Member")
            {
                memberInfo info = Service_MemberInfo.GetAccountDetail(Account);
                model.LoginID = info.LoginID;
                model.Status = AntiXssEncoder.HtmlEncode(info.Status, true);
                model.Email = info.Email;
                model.Name = info.Name;
                model.RoleCode = AntiXssEncoder.HtmlEncode(info.RoleCode, true);
                model.City = info.City;
                model.Zip = CommonDataHelper.GetDistrictZip(info.City, info.District);
                model.Sexy = AntiXssEncoder.HtmlEncode(info.Sexy, true);
                model.YearOfBirth = AntiXssEncoder.HtmlEncode((info.Year != "---" ? info.Year : String.Empty), true);
                model.Mobile = AntiXssEncoder.HtmlEncode(info.Mobile, true);
                model.IdentifyID = info.Identify ?? "";
                // TODO FB、Google 註冊，需要加上
                model.Origin = "會員註冊"+ (!string.IsNullOrEmpty(info.FacebookId) ? "/Facebook" : "") + (!string.IsNullOrEmpty(info.GoogleId) ? "/Google" : "");
                model.IsFront = true;
            }
            else
            {
                sysUserInfo userInfo = Service_sysUserInfo.GetDetail(Account);
                model.LoginID = userInfo.LoginID;
                model.Status = userInfo.State == "N" ? "Y" : (userInfo.State == "D" ? "N" : "");
                model.Email = userInfo.EmailAddress;
                model.Name = userInfo.UserName;
                model.RoleCode = userInfo.RoleID;
                model.City = "高雄市";
                model.Zip = userInfo.District;
                model.Sexy = userInfo.Sexy;
                model.YearOfBirth = Utility_DateTime.ToFormat_inTaiwanYear(userInfo.YearOfBirth, "yyy");
                model.Mobile = userInfo.Mobile;
                model.IdentifyID = String.Empty;
                model.Origin = "後台管理員新增";
                model.IsFront = false;
            }

            return View(model);
        }

        /// <summary>
        /// 審核通過的話寄出密碼通知信
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult MemberCheck(MemberCheckViewModel model)
        {
            BaseResult result = new BaseResult();
            if (model.IsFront)
            {
                // 僅能變更狀態與角色
                memberInfo info = Service_MemberInfo.GetAccountDetail(model.LoginID);
                if (info == null)
                {
                    TempData["error"] = "帳號不存在";
                    return RedirectToAction(nameof(MemberList));
                }
                info.Status = model.Status;
                info.RoleCode = model.RoleCode;

                string allowedChars = "abcdefghijkmnopqrstuvwxyzABCDEFGHJKLMNOPQRSTUVWXYZ0123456789";
                int updLength = 8;//密碼長度
                string upd = "";
                Random rd = new Random();

                for (int i = 0; i < updLength; i++)
                {
                    //allowedChars -> 這個String ，隨機取得一個字，丟給chars[i]
                    upd += allowedChars[rd.Next(0, allowedChars.Length)];
                }

                info.LoginPass = Utility_Cryptography.Get_HashEncryption(upd);
                // 過期天數90天
                info.PasswordExpiredAt = DateTime.Now.AddDays(90);
                result = Service_MemberInfo.Update(info);
                if (!result.result)
                {
                    TempData["error"] = "更新出錯，請稍後再試";
                    return RedirectToAction(nameof(MemberList));
                }
                List<string> msgList = new List<string>();
                msgList.Add("審核完成");
                // 寄發簡訊
                if (info.VerifyMethod == "SMS" && !String.IsNullOrEmpty(info.Mobile))
                {
                    MemberInitPasswordReplaceModel passwordReplaceModel = new MemberInitPasswordReplaceModel();
                    passwordReplaceModel.Account = info.LoginID;
                    passwordReplaceModel.LoginPass = upd;
                    passwordReplaceModel.WebUrl = AppSettingHelper.GetAppSetting("Front_HostName");
                    SmsReadFile smsReadFile = new SmsReadFile();
                    string body = smsReadFile.setReplacedSmsDataForMemberInitPassword("TW_Member_Init_Password", "zh", passwordReplaceModel);

                    SmsReq req = new SmsReq
                    {
                        Mobile = info.Mobile,
                        Message = body
                    };
                    SmsHelper.SendSms(req);
                }
                else if (!String.IsNullOrEmpty(info.Email)) // 寄送信件
                {
                    // 寄出初始密碼信件
                    MemberEmailReplaceModel emailReplaceModel = new MemberEmailReplaceModel();
                    emailReplaceModel.NameRecipient = info.Name;
                    emailReplaceModel.Account = model.LoginID;
                    emailReplaceModel.LoginPass = upd;
                    emailReplaceModel.WebsiteUrl = AppSettingHelper.GetAppSetting("Front_HostName");
                    MailReadFile mailReadFile = new MailReadFile();
                    string body = mailReadFile.setReplacedEmailDataForMember("TW_Member", "zh", emailReplaceModel);
                    MailHelper mailHelper = new MailHelper();
                    string refErrMsg = "";
                    mailHelper.SendMail(ref refErrMsg, info.Email, "高雄市政府農業局植物防疫平台", body);
                }

                TempData["InfoMsg"] = String.Join(",", msgList.ToArray());
                return RedirectToAction(nameof(MemberList));
            }
            else
            {
                return RedirectToAction(nameof(MemberList));
            }
        }

        /// <summary>
        /// 帳號管理-新增
        /// </summary>
        /// <returns></returns>
        public ActionResult MemberAdd()
        {
            MemberAddViewModel model = new MemberAddViewModel();
            return View(model);
        }

        //帳號管理-新增
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult MemberAdd(sysUserInfo model)
        {
            model.LoginID = model.LoginID.Trim();
            model.AddDate = DateTime.Now;
            if (string.IsNullOrEmpty(model.Mobile))
                model.Mobile = "";
            if (string.IsNullOrEmpty(model.Identify))
                model.Identify = "";
            model.District = string.Join(",", model.SelectDistrict);

            var Password = Extention.GetPwdSecurity(Membership.GeneratePassword(8, 2));
            model.LoginPass = Utility_Cryptography.Get_HashEncryption(Extention.SecureStringToString(Password));
            Service_sysUserInfo.Insert(model);

            IEnumerable<string> originValues = Request.Headers.GetValues("Origin");
            // 寄通知信
            AdminEmailReplaceModel emailReplaceModel = new AdminEmailReplaceModel();
            emailReplaceModel.NameRecipient = model.UserName;
            emailReplaceModel.Account = model.LoginID;
            emailReplaceModel.LoginPass = Extention.SecureStringToString(Password);
            emailReplaceModel.WebsiteUrl = originValues.First();
            MailReadFile mailReadFile = new MailReadFile();
            string body = mailReadFile.setReplacedEmailDataForAdmin("TW_Admin", "zh", emailReplaceModel);
            MailHelper mailHelper = new MailHelper();
            string refErrMsg = "";
            mailHelper.SendMail(ref refErrMsg, model.EmailAddress, "植物醫師診斷系統暨作物病蟲害預警系統", body);

            return RedirectToAction(nameof(SysUserList));
        }


        /// <summary>
        /// 帳號管理-列表
        /// </summary>
        /// <param name="Status"></param>
        /// <param name="Identify"></param>
        /// <param name="Special"></param>
        /// <param name="act"></param>
        /// <param name="Type"></param>
        /// <returns></returns>
        public ActionResult SysUserList(string Status, string Identify, string Special, string act, List<string> DeleteIDs = null)
        {
            // 刪除指定ID的消息
            if (DeleteIDs != null)
            {
                try
                {
                    DeleteIDs.ForEach(delegate (string TypeAccount)
                    {
                        string[] arr = TypeAccount.Split(',');
                        string UserType = arr[0];
                        string Account = arr[1];

                        Service_sysUserInfo.Delete(Account);
                    });
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.Message);
                }
            }

            Expression<Func<sysUserInfo, bool>> filter = x => true;

            //權限
            if (string.IsNullOrEmpty(Identify)) Identify = "";
            else filter = filter.And(x => x.RoleID == Identify);

            //帳號狀態
            if (string.IsNullOrEmpty(Status)) Status = "";
            else filter = filter.And(x => (Status == "Y") == (x.State == "Y"));

            //特權帳號
            if (string.IsNullOrEmpty(Special)) Special = "";
            else filter = filter.And(x => (Special == "Y") == (x.Special == "Y"));

            SysUserModel model = new SysUserModel()
            {
                defCodes = Service_sysRole.GetList(String.Empty, String.Empty),
                SysUsers = Service_sysUserInfo.GetList(filter),
                Identify_Filter = Identify,
                Status_Filter = Status,
                Special_Filter = Special,
                CurrentPage = 1,
                TotalPage = 1,
                PageCounts = 10,
            };

            #region 匯出
            if (act == "export")
            {
                int i = 0;
                var _export = model.SysUsers.Select(x => new
                {
                    項次 = (i += 1),
                    帳號 = x.LoginID,
                    姓名 = x.UserName,
                    電子信箱 = x.EmailAddress,
                    角色別 = model.defCodes.FirstOrDefault(o => o.RoleCode == x.RoleID)?.RoleName ?? "",
                    特權帳號 = x.Special == "Y" ? "是" : "否",
                    狀態 = x.State == "Y" ? "啟用" : "未啟用",
                }).ToList();

                var _output = ExcelHelper.ConvertToDataTable(_export);
                return File(ExcelHelper.RenderDataTableToExcel(_output), "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", $"後台使用者列表{DateTime.Now.ToString("yyyyMMddHHmmss")}.xlsx");
            }
            #endregion

            return View(model);
        }

        [HttpGet]
        public ActionResult CheckAccount(string ChkAccount)
        {
            var _res = "";
            if (ChkAccount != "")
            {
                sysUserInfo sysUserInfo = Service_sysUserInfo.ChkAccount(ChkAccount);
                if (sysUserInfo != null) _res = "帳號已存在";
                else _res = "帳號可使用";
            }

            return Json(_res, JsonRequestBehavior.AllowGet);
        }


        //帳號管理-編輯
        public ActionResult SysUserEdit(String LoginID)
        {
            sysUserInfo userInfo = Service_sysUserInfo.GetDetail(LoginID);
            userInfo.SelectDistrict = (userInfo.District ?? "").Split(',').ToList();
            // 代表尚未通過審核，啟用或禁用
            if (userInfo.State != "Y" && userInfo.State != "N")
            {
                return RedirectToAction(nameof(MemberCheck), new { Type = "System", Account = userInfo.LoginID });
            }

            SysUserEditViewModel model = new SysUserEditViewModel()
            {
                sysUserInfo = userInfo,
                sysRoles = Service_sysRole.GetList(null, null, null),
            };
            if (model.sysUserInfo.Identify == null) model.sysUserInfo.Identify = "";
            //if (model.sysUserInfo.District == null) model.sysUserInfo.District = "";

            return View(model);
        }

        //系統帳號管理-編輯
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult SysUserEdit(sysUserInfo model, String ChkAccountSource)
        {
            model.District = string.Join(",", model.SelectDistrict);
            Service_sysUserInfo.Update(model);
            return RedirectToAction(nameof(SysUserList));
        }

        //群組與權限管理-列表
        public ActionResult GroupList()
        {
            var model = (from role in Service_sysRole.GetList("", "")
                         join sysUser in Service_sysUserInfo.GetList()
                         on role.RoleCode equals sysUser.RoleID into userTemp
                         from sysUser in userTemp.DefaultIfEmpty()
                         group sysUser by role into gp
                         select new RoleSelectModel
                         {
                             RoleID = gp.Key.RoleCode,
                             RoleName = gp.Key.RoleName,
                             RoleCount = gp.Count(x => x != null),
                         }).ToList();
            return View(model);
        }
        //群組與權限管理-內容
        public ActionResult GroupInfo(string RoleID)
        {
            if (String.IsNullOrEmpty(RoleID))
            {
                return RedirectToAction(nameof(GroupList));
            }

            List<RoleHasPermission> RoleHasPermissions = Service_Role_Has_Permission.GetList(RoleID);
            List<Permission> Permissions = null;
            // 前台農友會員
            if (RoleID == "R07")
            {
                Permissions = Service_Permission.GetFrontList();
            }
            else
            {
                Permissions = Service_Permission.GetBackList();
            }

            foreach (Permission m in Permissions)
            {
                m.Selected = RoleHasPermissions.Find(x => x.PermissionId == m.Id) != null;
            }

            GroupInfoViewModel model = new GroupInfoViewModel()
            {
                RoleCode = RoleID,
                Permissions = Permissions
            };

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult GroupInfo(string RoleCode, List<string> PermissionIds)
        {
            if (String.IsNullOrEmpty(RoleCode) || PermissionIds == null)
            {
                return RedirectToAction(nameof(GroupList));
            }
            // 清空原有的
            Service_Role_Has_Permission.DeleteAll(RoleCode);
            List<RoleHasPermission> list = new List<RoleHasPermission>();
            foreach (string id in PermissionIds)
            {
                RoleHasPermission hasPermission = new RoleHasPermission()
                {
                    PermissionId = Convert.ToInt32(id),
                    RoleCode = RoleCode
                };
                list.Add(hasPermission);
            }
            Service_Role_Has_Permission.Insert(list);

            return RedirectToAction(nameof(GroupList));
        }

        //群組與權限管理-內容
        public ActionResult UserSpecialGroupInfo(string id = "")
        {
            var sysUser = Service_sysUserInfo.GetDetail(id);
            if (sysUser == null)
            {
                return RedirectToAction(nameof(MemberList));
            }
            var rolePermission = Service_Role_Has_Permission.GetList(sysUser.RoleID).Select(x => x.PermissionId);
            var userPermissions = Service_sysUserSpecialPermission.GetList(x => x.sysUserId == id);
            List<Permission> Permissions = Service_Permission.GetBackList().Where(x => !rolePermission.Contains(x.Id)).ToList();

            foreach (Permission m in Permissions)
            {
                m.Selected = userPermissions.Any(x => x.PermissionId == m.Id);
            }

            userPermissionViewModel model = new userPermissionViewModel()
            {
                Id = id,
                Permissions = Permissions
            };

            return View(model);
        }

        /// <summary>
        /// 額外權限設定
        /// </summary>
        /// <param name="id"></param>
        /// <param name="PermissionIds"></param>
        /// <returns></returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult UserSpecialGroupInfo(string id, List<int> PermissionIds)
        {
            if (String.IsNullOrEmpty(id) || PermissionIds == null)
            {
                return RedirectToAction(nameof(GroupList));
            }
            var sysUser = Service_sysUserInfo.GetDetail(id);
            if (sysUser == null)
            {
                return RedirectToAction(nameof(SysUserList));
            }
            // 清空原有的
            Service_sysUserSpecialPermission.Delete(id);
            if (PermissionIds.Any())
            {
                foreach (var pid in PermissionIds)
                {
                    sysUserSpecialPermission newPermission = new sysUserSpecialPermission()
                    {
                        PermissionId = pid,
                        sysUserId = id
                    };
                    Service_sysUserSpecialPermission.Insert(newPermission);
                }

                sysUser.Special = "Y";
                Service_sysUserInfo.Update(sysUser);
            }

            return RedirectToAction(nameof(SysUserList));
        }


        //系統操作歷程LOG
        public ActionResult Log()
        {
            return View();
        }
        //常用表單下載-列表
        public ActionResult FormList(bool? Show = null, List<string> Codes = null, List<string> DeleteIDs = null)
        {
            // 刪除指定ID的消息
            if (DeleteIDs != null)
            {
                try
                {
                    DeleteIDs.ForEach(delegate (string id)
                    {
                        Service_BusinessForm.Delete(int.Parse(id));
                    });
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.Message);
                }
            }

            BusinessFormListViewModel model = new BusinessFormListViewModel()
            {
                Types = Service_sysRole.GetList("", ""),
                BusinessForms = Service_BusinessForm.GetList(Codes, Show),
                Show = Show,
                Codes = Codes ?? new List<string>(),
                CurrentPage = 1,
                TotalPage = 1,
                PageCounts = 30
            };

            return View(model);
        }
        //常用表單下載-新增、編輯
        public ActionResult FormInfo(string ID)
        {
            BusinessFormInfoViewModel model = new BusinessFormInfoViewModel()
            {
                Types = Service_sysRole.GetList("", ""),
                BusinessForm = new BusinessForm(),
            };

            if (ID != null)
            {
                model.BusinessForm = Service_BusinessForm.GetDetail(int.Parse(ID));
            }

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult FormInfo(BusinessForm model, HttpPostedFileBase file)
        {
            var baseDirectory = "/UploadedFiles/BusinessForms";
            string _FileName = Path.GetFileName(file.FileName);
            string _directory = Server.MapPath("~" + baseDirectory);
            FileHelper.CreateFolder(_directory);
            string _path = Path.Combine(_directory, _FileName);
            file.SaveAs(_path);
            var url = String.Concat(baseDirectory, '/', _FileName);
            model.FilePath = url;

            if (model.ID == 0)
            {
                Service_BusinessForm.Insert(model);
            }
            else
            {
                Service_BusinessForm.Update(model);
            }

            Service_FileManagement.Insert(new FileManagement()
            {
                TableName = "BusinessForms",
                TableID = model.ID.ToString(),
                FileName = file.FileName,
                FilePath = _path,
                FileLength = file.ContentLength,
                FileType = file.ContentType,
                MD5 = FileLogic.CalculateMD5(file.InputStream),
            });

            return RedirectToAction(nameof(FormList));
        }

        //個人資料管理
        public ActionResult UserInfo()
        {
            string LoginID = SessionHelper.Get_LoginID();
            // 取不到 session 的 LoginID 就導回登入頁
            if (String.IsNullOrEmpty(LoginID))
            {
                return RedirectToAction("Index", "Login");
            }
            UserInfoViewModel model = new UserInfoViewModel()
            {
                Info = Service_sysUserInfo.GetDetail(LoginID)
            };

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult UserInfo(string UserName, string EmailAddress, string Mobile, string Sexy, string YearOfBirthStr, string District)
        {
            string LoginID = SessionHelper.Get_LoginID();
            // 取不到 session 的 LoginID 就導回登入頁
            if (String.IsNullOrEmpty(LoginID))
            {
                return RedirectToAction("Index", "Login");
            }
            sysUserInfo userInfo = Service_sysUserInfo.GetDetail(LoginID);
            userInfo.UserName = UserName;
            userInfo.EmailAddress = EmailAddress;
            userInfo.Mobile = Mobile;
            userInfo.Sexy = Sexy;
            if (String.IsNullOrEmpty(YearOfBirthStr) || YearOfBirthStr == "---")
            {
                userInfo.YearOfBirthStr = null;
            }
            else
            {
                string date = DateTime.Now.ToString("MM-dd");
                string year = (Convert.ToInt32(YearOfBirthStr) + 1911).ToString();
                userInfo.YearOfBirthStr = year + '-' + date;
            }
            userInfo.District = District;
            var baseResult = Service_sysUserInfo.Update(userInfo);
            return Json(baseResult.result ? true : false);
        }

        /// <summary>
        /// 更新密碼
        /// </summary>
        /// <returns></returns>
        public ActionResult UserUpdatePwd()
        {
            UserUpdatePwdViewModel model = new UserUpdatePwdViewModel();
            return View(model);
        }

        /// <summary>
        /// 更新密碼
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult UserUpdatePwd(UserUpdatePwdViewModel model)
        {
            if (model.LoginPass != model.ConfirmLoginPass)
            {
                TempData["Error"] = true;
                TempData["Title"] = "新密碼與確認密碼不相符";
                return View(model);
            }
            if (!model.CheckStrength)
            {
                TempData["Error"] = true;
                TempData["Title"] = "新密碼強度不符合密碼規則";
                return View(model);
            }
            // 確認使用者密碼
            string LoginID = SessionHelper.Get_LoginID();
            // 取不到 session 的 LoginID 就導回登入頁
            if (String.IsNullOrEmpty(LoginID))
            {
                return RedirectToAction("Index", "Login");
            }
            sysUserInfo userInfo = Service_sysUserInfo.GetDetail(LoginID);
            // 舊密碼驗證不通過
            if (!Utility_Cryptography.Compare_HashCode(model.OldLoginPass, userInfo.LoginPass))
            {
                TempData["Error"] = true;
                TempData["Title"] = "驗證舊密碼錯誤";
                return View(model);
            }
            // 檢查密碼是否在近 3 次內重複
            bool duplicate = false;
            List<sysUserPass_log> _Logs = Service_sysUserPass_log.GetList(userInfo.LoginID);
            _Logs = _Logs.OrderByDescending(x => x.id).ToList();
            foreach (sysUserPass_log _Log in _Logs)
            {
                if (Utility_Cryptography.Compare_HashCode(model.LoginPass, _Log.LoginPW))
                {
                    duplicate = true;
                }
            }
            if (duplicate)
            {
                TempData["Error"] = true;
                TempData["Title"] = "新密碼與前3次使用密碼相同";
                return View(model);
            }

            userInfo.LoginPass = Utility_Cryptography.Get_HashEncryption(model.LoginPass);
            userInfo.LastChangePWDate = DateTime.Now;
            // 更新密碼
            Service_sysUserInfo.Update(userInfo);
            // 如果舊密碼紀錄已經有3次了，需要刪除最舊的
            if (_Logs.Count >= 3)
            {
                Service_sysUserPass_log.Delete(_Logs.Last().id);
            }

            // 新增一筆密碼紀錄
            Service_sysUserPass_log.Insert(new sysUserPass_log()
            {
                LoginID = userInfo.LoginID,
                LoginPW = userInfo.LoginPass,
                created_at = DateTime.Now
            });

            return RedirectToAction(nameof(UserInfo));
        }
        /// <summary>
        /// 更新密碼
        /// </summary>
        /// <returns></returns>
        public ActionResult UserNeedUpdatePwd()
        {
            UserUpdatePwdViewModel model = new UserUpdatePwdViewModel();
            return View(model);
        }

        /// <summary>
        /// 更新密碼
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult UserNeedUpdatePwd(UserUpdatePwdViewModel model)
        {
            if (model.LoginPass != model.ConfirmLoginPass)
            {
                TempData["Error"] = true;
                TempData["Title"] = "新密碼與確認密碼不相符";
                return View(model);
            }
            if (!model.CheckStrength)
            {
                TempData["Error"] = true;
                TempData["Title"] = "新密碼強度不符合密碼規則";
                return View(model);
            }
            // 確認使用者密碼
            string LoginID = SessionHelper.Get_LoginID();
            // 取不到 session 的 LoginID 就導回登入頁
            if (String.IsNullOrEmpty(LoginID))
            {
                return RedirectToAction("Index", "Login");
            }
            sysUserInfo userInfo = Service_sysUserInfo.GetDetail(LoginID);
            // 舊密碼驗證不通過
            if (!Utility_Cryptography.Compare_HashCode(model.OldLoginPass, userInfo.LoginPass))
            {
                TempData["Error"] = true;
                TempData["Title"] = "驗證舊密碼錯誤";
                return View(model);
            }
            // 檢查密碼是否在近 3 次內重複
            bool duplicate = false;
            List<sysUserPass_log> _Logs = Service_sysUserPass_log.GetList(userInfo.LoginID);
            _Logs = _Logs.OrderByDescending(x => x.id).ToList();
            foreach (sysUserPass_log _Log in _Logs)
            {
                if (Utility_Cryptography.Compare_HashCode(model.LoginPass, _Log.LoginPW))
                {
                    duplicate = true;
                }
            }
            if (duplicate)
            {
                TempData["Error"] = true;
                TempData["Title"] = "新密碼不可與前3次使用密碼相同";
                return View(model);
            }

            userInfo.LoginPass = Utility_Cryptography.Get_HashEncryption(model.LoginPass);
            userInfo.LastChangePWDate = DateTime.Now;
            // 更新密碼
            Service_sysUserInfo.Update(userInfo);
            // 如果舊密碼紀錄已經有3次了，需要刪除最舊的
            if (_Logs.Count >= 3)
            {
                Service_sysUserPass_log.Delete(_Logs.Last().id);
            }

            // 新增一筆密碼紀錄
            Service_sysUserPass_log.Insert(new sysUserPass_log()
            {
                LoginID = userInfo.LoginID,
                LoginPW = userInfo.LoginPass,
                created_at = DateTime.Now
            });

            TempData["Title"] = "變更成功，下次登入請使用新密碼";
            return RedirectToAction("Index","Home");
        }

        //消息管理-列表
        public ActionResult SysNewsList(DateTime? StartDate = null, DateTime? EndDate = null, string keyword = "", int Page = 1, string DeleteID = null)
        {
            // 刪除指定ID的消息
            if (DeleteID != null)
            {
                try
                {
                    var delId = Extention.ToInt32(DeleteID);
                    if (delId > 0)
                    {
                        Service_sysNews.Delete(delId);
                    };
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.Message);
                }
            }

            Expression<Func<sysNews, bool>> filter = x => true;
            if (!string.IsNullOrEmpty(keyword)) filter = filter.And(x => x.Title.Contains(keyword));
            if (StartDate.HasValue) filter = filter.And(x => x.StartDate >= StartDate || x.EndDate >= StartDate);
            if (EndDate.HasValue) filter = filter.And(x => x.StartDate <= EndDate || x.EndDate <= EndDate);

            var news = Service_sysNews.GetList(filter);

            SysNewsModel model = new SysNewsModel()
            {
                SysNews = news.OrderByDescending(x => x.IsTop ? 1 : 0).ThenByDescending(x => x.IsTop ? (x.ModifyDate ?? x.CreateDate) : x.StartDate).Skip((Page - 1) * 20).Take(20).ToList(),
                CurrentPage = 1,
                TotalPage = (int)Math.Ceiling((decimal)news.Count() / 20),
                PageCounts = 30,
                Page = Page,
                SysNewsFilters = new SysNewsFilter
                {
                    Keyword = keyword,
                    StartDate = StartDate,
                    EndDate = EndDate,
                }
            };

            return View(model);
        }

        /// <summary>
        /// 後臺公告
        /// </summary>
        /// <param name="ID"></param>
        /// <returns></returns>
        public ActionResult SysNewsInfo(string ID)
        {
            if (ID != null)
            {
                var sysNews = Service_sysNews.GetDetail(int.Parse(ID));
                return View(sysNews);
            }
            return View(new sysNews());
        }
        /// <summary>
        /// 後臺公告-送出
        /// </summary>
        /// <param name="sysNewModel"></param>
        /// <returns></returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult SysNewsInfo(sysNews sysNewModel)
        {
            if (sysNewModel.Id > 0)
            {
                Service_sysNews.Update(sysNewModel);
            }
            else
            {
                Service_sysNews.Insert(sysNewModel);
            }

            return RedirectToAction(nameof(SysNewsList));
        }

    }
}