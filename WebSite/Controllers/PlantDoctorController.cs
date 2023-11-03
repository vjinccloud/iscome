using ICCModule;
using ICCModule.Entity.Tables.Platform;
using ICCModule.EntityService.Service;
using ICCModule.Helper;
using IscomG2C.Utility;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Website.Helper;
using Website.Models.Login;
using Website.ViewModel.PlantDoctor;

namespace Website.Controllers
{
    public class PlantDoctorController : Controller
    {
        //說明頁面
        public ActionResult Introduce()
        {
            return View();
        }

        // 說明頁面登入
        /// <summary>
        /// 登入頁
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Login(Model_Login_Index model)
        {
            BaseResult result = new BaseResult();
            //圖形驗證碼
            //檢查與Session值內的驗證碼是否符合?
            if (Check_VerifyCode(model.reCAPTCHA) == false)
            {
                result.result = false;
                result.Msg = "驗證碼資訊不符，請重新輸入";
                return Json(result);
            }
            //抓取偵錯模式標記
            bool isDebug = false;
            //登入驗證
            string msg = "";
            LoginState state = model.LoginCheck(isDebug, ref msg);
            //登入紀錄Log
            bool savelog = model.SaveLog(Request.UserHostAddress, state, msg);

            switch (state)
            {
                case LoginState.OK:
                    //抓取使用者資訊,放到Session中,避免後續一直抓資料庫
                    var usr = Service_MemberInfo.GetAccountDetail(model.LoginID);
                    //設定Session
                    //帳號
                    SessionHelper.Set("MemberID", usr.ID);
                    SessionHelper.Set("UserName", usr.Name);
                    result.result = true;
                    result.Msg = "登入成功";
                    break;
                //重設密碼功能之後再做
                //case LoginState.RESET:
                //    //設定Session
                //    Session["LoginID"] = model.LoginID;
                //    return RedirectToAction("index", "Home");
                case LoginState.NG:
                case LoginState.DISABLED:
                case LoginState.LOCK:
                default:
                    ModelState.Clear();
                    result.result = false;
                    result.Msg = "帳號或密碼不正確";
                    break;
            }
            return Json(result);
        }

        /// <summary>檢查與Session值內的驗證碼是否符合?
        ///
        /// </summary>
        /// <param name="m_sValidate"></param>
        /// <returns></returns>
        public bool Check_VerifyCode(string m_sValidate)
        {
            m_sValidate = m_sValidate ?? "";
            //抓取偵錯模式標記
            bool bDebug = AppSettingHelper.Get_Debug_Mode();

            //如果是測試模式 則略過驗證碼
            if (bDebug == true)
                return true;

            //如果不是測試模式 有輸入驗證碼 且 驗證碼符合
            if (Session["HomeValidate"] == null)
                return false;

            if (Session["HomeValidate"].ToString().ToUpper() != m_sValidate.ToUpper())
                return false;

            //通過檢查驗證碼檢查
            return true;

        }

        /// <summary>
        /// 輔導單位
        /// </summary>
        /// <returns></returns>
        public ActionResult CounselingUnit()
        {
            var LoginID = SessionHelper.Get_LoginID();
            //偵錯模式 或者 已經登入的情況下 才可使用
            //非偵錯模式 且 尚未登入的情況下 不可使用
            if (String.IsNullOrEmpty(LoginID))
            {
                TempData["error"] = "請先登入";
                return RedirectToAction(nameof(Introduce));
            }
            memberInfo member = Service_MemberInfo.GetDetail(Convert.ToInt64(LoginID));
            if (member == null)
            {
                TempData["error"] = "請重新登入";
                return RedirectToAction(nameof(Introduce));
            }

            if (member.BlacklistExpiredAt != null)
            {
                string Date = Utility_DateTime.ToFormat_inTaiwanYear(member.BlacklistExpiredAt);
                TempData["error"] = $"目前線上預約診斷服務停權中，預計停權至{Date}止";
                return RedirectToAction(nameof(Introduce));
            }



            return View(new CounselingUnitModel());
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult CounselingUnit(string OrgType, string OrgName, string OrgDistrict)
        {
            var LoginID = SessionHelper.Get_LoginID();
            //偵錯模式 或者 已經登入的情況下 才可使用
            //非偵錯模式 且 尚未登入的情況下 不可使用
            if (String.IsNullOrEmpty(LoginID))
            {
                TempData["error"] = "請先登入";
                return RedirectToAction(nameof(Introduce));
            }
            memberInfo member = Service_MemberInfo.GetDetail(Convert.ToInt64(LoginID));
            if (member == null)
            {
                TempData["error"] = "請重新登入";
                return RedirectToAction(nameof(Introduce));
            }

            if (member.BlacklistExpiredAt != null)
            {
                string Date = Utility_DateTime.ToFormat_inTaiwanYear(member.BlacklistExpiredAt);
                TempData["error"] = $"目前線上預約診斷服務停權中，預計停權至{Date}止";
                return RedirectToAction(nameof(Introduce));
            }

            return RedirectToAction(nameof(Calendar), new { ot = OrgType, on = OrgName, od = OrgDistrict });
        }

        //掛號行事曆
        public ActionResult Calendar(string ot = "", string on = "", string od = "")
        {
            var LoginID = SessionHelper.Get_LoginID();
            //偵錯模式 或者 已經登入的情況下 才可使用
            //非偵錯模式 且 尚未登入的情況下 不可使用
            if (String.IsNullOrEmpty(LoginID))
            {
                TempData["error"] = "請先登入";
                return RedirectToAction(nameof(Introduce));
            }
            memberInfo member = Service_MemberInfo.GetDetail(Convert.ToInt64(LoginID));
            if (member == null)
            {
                TempData["error"] = "請重新登入";
                return RedirectToAction(nameof(Introduce));
            }

            if (member.BlacklistExpiredAt != null)
            {
                string Date = Utility_DateTime.ToFormat_inTaiwanYear(member.BlacklistExpiredAt);
                TempData["error"] = $"目前線上預約診斷服務停權中，預計停權至{Date}止";
                return RedirectToAction(nameof(Introduce));
            }

            var Districts = CommonDataHelper.GetDistricts("高雄市");
            var CounselingUnitList = Service_defCode.GetList("PlantDoctorCounselingUnit");
            if (!Districts.Any(x => x.Zip == od) || !CounselingUnitList.Any(x => x.Code == ot))
            {
                TempData["error"] = $"資料錯誤，請重新填寫";
                return RedirectToAction(nameof(Introduce));
            }

            var _model = new CalendarModel
            {
                OrgType = ot,
                OrgDistrict = od,
                OrgName = on,
                DoctorList = Service_sysUserInfo.GetEnableDoctorList().Where(x => x.RoleID == "R05" || (x.District ?? "").Contains(od)).ToList()
            };

            return View(_model);
        }

        /// <summary>
        /// 取回指定月份的預約排班
        /// </summary>
        /// <param name="monthFirst"></param>
        /// <param name="monthLast"></param>
        /// <returns></returns>
        [HttpGet]
        public ActionResult GetSchedules(string monthFirst, string monthLast, string docId, string district)
        {
            DateTime StartDate = DateTime.Parse($"{monthFirst} 00:00:00");
            DateTime EndDate = DateTime.Parse($"{monthLast} 23:59:59");
            List<doctorDutyRecord> list = Service_doctor_DutyRecord.GetList(x => x.Date >= StartDate && x.Date <= EndDate && x.DoctorId == docId && x.IsDel == false).Where(x => (x.District == district || string.IsNullOrEmpty(x.District))).ToList();
            //List<doctorDutyRecord> filtered = list.Where(x => x.CreateUser != "system").ToList();
            List<doctorDutyRecord> filtered = list.ToList();

            return Json(filtered, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// 掛號單，會員選定的預約日期與時段
        /// </summary>
        /// <param name="Date"></param>
        /// <param name="Period"></param>
        /// <returns></returns>
        public ActionResult Ticket(string Date, string Period, string ot = "", string on = "", string od = "",string li = "")
        {
            var LoginID = SessionHelper.Get_LoginID();
            if (String.IsNullOrEmpty(LoginID))
            {
                TempData["error"] = "請先登入";
                return RedirectToAction("Index", "Home");
            }

            var Districts = CommonDataHelper.GetDistricts("高雄市");
            var CounselingUnitList = Service_defCode.GetList("PlantDoctorCounselingUnit");
            var DoctorList = Service_sysUserInfo.GetEnableDoctorList().Where(x => x.RoleID == "R05" || (x.District ?? "").Contains(od)).ToList();
            if (!Districts.Any(x => x.Zip == od) || !CounselingUnitList.Any(x => x.Code == ot)|| !DoctorList.Any(x => x.LoginID == li))
            {
                TempData["error"] = $"資料錯誤，請重新填寫";
                return RedirectToAction(nameof(Introduce));
            }

            memberInfo member = Service_MemberInfo.GetDetail(Convert.ToInt64(LoginID));

            InfoViewModel info = new InfoViewModel();
            info.ReserveDate = Date;
            info.ReserveTaiwanDate = Utility_DateTime.ToFormat_inTaiwanYear(DateTime.Parse(Date));
            info.ReservePeriod = Period;
            info.Schedule.Name = member.Name;
            info.Schedule.Sexy = member.Sexy;
            info.Schedule.District = member.District;

            info.Schedule.OrgType = ot;
            info.Schedule.OrgName = on;
            info.Schedule.OrgDistrict = od;
            info.Schedule.LoginID = li;

            if (!String.IsNullOrEmpty(member.District))
            {
                info.Schedule.Zip = CommonDataHelper.GetDistrictZip("高雄市", member.District);
            }
            info.Schedule.Mobile = member.Mobile;

            return View(info);
        }

        /// <summary>
        /// 查看預約單
        /// </summary>
        /// <param name="ID"></param>
        /// <returns></returns>
        public ActionResult GetTicket(int ID = 0)
        {
            var LoginID = SessionHelper.Get_LoginID();
            if (String.IsNullOrEmpty(LoginID))
            {
                TempData["error"] = "請先登入";
                return RedirectToAction("Index", "Home");
            }
            memberInfo member = Service_MemberInfo.GetDetail(Convert.ToInt64(LoginID));
            doctorSchedule schedule = Service_doctorSchedule.GetDetail(ID);
            if (schedule == null)
            {
                return RedirectToAction("Index", "Home");
            }

            InfoViewModel info = new InfoViewModel();
            info.ReserveDate = schedule.ReserveDatetimeStr;
            info.ReserveTaiwanDate = schedule.ReserveDateTaiwainStr;
            info.ReservePeriod = schedule.Period;
            info.Schedule = schedule;
            info.Readonly = true;
            return View(nameof(Ticket), info);
        }

        /// <summary>
        /// 前台會員預約植醫行程
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult ReserveSchedule(InfoViewModel model)
        {
            TempData["ID"] = 0;
            memberInfo member = Service_MemberInfo.GetDetail(Convert.ToInt64(SessionHelper.Get_LoginID()));
            if (member == null)
            {
                TempData["error"] = "尚未登入，請重新登入";
                return View(nameof(Ticket), model);
            }

            //圖形驗證碼
            //檢查與Session值內的驗證碼是否符合?
            if (Check_VerifyCode(model.VerifyCode) == false)
            {
                TempData["error"] = "驗證碼資訊不符，請重新輸入";
                return View(nameof(Ticket), model);
            }

            // 確認排班場次還有
            List<doctorDutyRecord> records = Service_doctor_DutyRecord.GetDesignDatePeriodList(DateTime.Parse(model.ReserveDate), model.ReservePeriod);
            List<doctorDutyRecord> filtered = records.Where(r => r.DoctorScheduleID == 0 && r.DoctorId == model.Schedule.LoginID).ToList();
            if (filtered.Count == 0)
            {
                TempData["error"] = "指定時段預約已滿，請更換時段";
                return View(nameof(Ticket), model);
            }
            model.Schedule.Origin = "member";
            doctorDutyRecord dutyRecord = filtered.First();
            model.Schedule.MemberInfoID = member.ID;
            model.Schedule.ReserveDatetimeStr = dutyRecord.DateStr;
            model.Schedule.ReserveTime = dutyRecord.Time;
            model.Schedule.District = CommonDataHelper.GetDistrictName("高雄市", model.Schedule.Zip);
            model.Schedule.Status = "Appointment";
            model.Schedule.Creator = member.Name;
            // 排班場次寫入預約診斷ID
            Service_doctorSchedule.Insert(model.Schedule);

            try
            {
                if (model.CropSymptomsFiles.Count > 0)
                {
                    List<string> CropSymptomsFiles = new List<string>();
                    string baseDirectory = $"/UploadedFiles/PlantDoctor/{model.Schedule.ID}/CropSymptomsFiles";
                    foreach (var file in model.CropSymptomsFiles)
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
                        CropSymptomsFiles.Add(url);
                        Service_FileManagement.Insert(new FileManagement()
                        {
                            TableName = "doctor_Schedule",
                            TableID = model.Schedule.ID.ToString(),
                            FileName = file.FileName,
                            FilePath = _path,
                            FileLength = file.ContentLength,
                            FileType = file.ContentType,
                            MD5 = FileLogic.CalculateMD5(file.InputStream),
                        });
                    }
                    model.Schedule.CropSymptoms = String.Join(",", CropSymptomsFiles);
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }

            try
            {
                if (model.RecentlyFertilizerFiles.Count > 0)
                {
                    List<string> RecentlyFertilizerFiles = new List<string>();
                    string baseDirectory = $"/UploadedFiles/PlantDoctor/{model.Schedule.ID}/RecentlyFertilizerFiles";
                    foreach (var file in model.RecentlyFertilizerFiles)
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
                        RecentlyFertilizerFiles.Add(url);
                        Service_FileManagement.Insert(new FileManagement()
                        {
                            TableName = "doctor_Schedule",
                            TableID = model.Schedule.ID.ToString(),
                            FileName = file.FileName,
                            FilePath = _path,
                            FileLength = file.ContentLength,
                            FileType = file.ContentType,
                            MD5 = FileLogic.CalculateMD5(file.InputStream),
                        });
                    }
                    model.Schedule.RecentlyFertilizer = String.Join(",", RecentlyFertilizerFiles);
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
            if (!String.IsNullOrEmpty(model.Schedule.CropSymptoms) || !String.IsNullOrEmpty(model.Schedule.RecentlyFertilizer))
            {
                Service_doctorSchedule.Update(model.Schedule);
            }

            dutyRecord.DoctorScheduleID = model.Schedule.ID;
            Service_doctor_DutyRecord.Update(dutyRecord);
            // 依照聯繫管道發送資訊，要確認驗證綁定
            List<string> ContactMethodArr = model.Schedule.ContactMethodArr;
            List<string> ResultAlert = new List<string>() { "預約成功" };
            IEnumerable<string> originValues = Request.Headers.GetValues("Origin");
            // 選擇手機且驗證通過
            if (ContactMethodArr.Contains("Mobile"))
            {
                Models.NotifyTemplate.SMS.PlantDoctorSuccessReplacementModel smsReplaceModel = new Models.NotifyTemplate.SMS.PlantDoctorSuccessReplacementModel();
                smsReplaceModel.Date = model.Schedule.ReserveDateTaiwainStr;
                smsReplaceModel.Weekday = Utility_DateTime.GetDayOfWeek(model.Schedule.ReserveDatetime);
                smsReplaceModel.Time = model.Schedule.ReserveTime;
                smsReplaceModel.CancelReserveUrl = String.Concat(originValues.First(), "/Member/Manage");
                SmsReadFile smsReadFile = new SmsReadFile();
                string body = smsReadFile.setReplacedSmsDataForPlantDoctorSuccess("TW_PlantDoctor_Succcess", "zh", smsReplaceModel);

                SmsReq req = new SmsReq
                {
                    Mobile = model.Schedule.Mobile,
                    Message = body
                };
                SmsHelper.SendSms(req);

            }
            // 選擇 信箱且驗證通過
            if (ContactMethodArr.Contains("Email"))
            {
                // 寄通知信
                Models.NotifyTemplate.Mail.PlantDoctorSuccessReplacementModel emailReplaceModel = new Models.NotifyTemplate.Mail.PlantDoctorSuccessReplacementModel();
                emailReplaceModel.NameRecipient = model.Schedule.Name;
                emailReplaceModel.Date = model.Schedule.ReserveDateTaiwainStr;
                emailReplaceModel.Weekday = Utility_DateTime.GetDayOfWeek(model.Schedule.ReserveDatetime);
                emailReplaceModel.Mobile = model.Schedule.Mobile;
                emailReplaceModel.ContactStr = String.Join(",", model.Schedule.ContactMethodArr.ToArray());
                emailReplaceModel.CancelReserveUrl = String.Concat(originValues.First(), "/Member/Manage");
                MailReadFile mailReadFile = new MailReadFile();
                string body = mailReadFile.setReplacedEmailDataForPlantDoctorSuccess("TW_PlantDoctor_Succcess", "zh", emailReplaceModel);
                MailHelper mailHelper = new MailHelper();
                string refErrMsg = "";
                mailHelper.SendMail(ref refErrMsg, member.Email, "高雄市政府農業局-植物醫師掛號預約成功通知信", body);
            }
            // TODO 處理 Line 預約
            if (ContactMethodArr.Contains("Line"))
            {

            }

            return RedirectToAction(nameof(Success), new { ID = model.Schedule.ID.ToString(), Error = String.Join(",", ResultAlert.ToArray()) });
        }

        //掛號單-預約成功
        public ActionResult Success(string ID, string Error)
        {
            var LoginID = SessionHelper.Get_LoginID();
            if (String.IsNullOrEmpty(LoginID))
            {
                TempData["error"] = "請先登入";
                return RedirectToAction("Index", "Home");
            }

            // 未送ID 導回首頁
            if (String.IsNullOrEmpty(ID))
            {
                return RedirectToAction("Index", "Home");
            }
            doctorSchedule model = Service_doctorSchedule.GetDetail(Convert.ToInt64(ID));
            // 不存在導回首頁
            if (model == null)
            {
                return RedirectToAction("Index", "Home");
            }
            memberInfo member = Service_MemberInfo.GetDetail(Convert.ToInt64(LoginID));
            // 不是自己的導回首頁
            if (model.MemberInfoID != member.ID)
            {
                return RedirectToAction("Index", "Home");
            }
            TempData["error"] = Error;
            return View(model);
        }

        /// <summary>
        /// 植醫輔導紀錄-List
        /// </summary>
        /// <returns></returns>
        public ActionResult RecordList()
        {
            if (Session["MemberID"] == null)
            {
                TempData["NeedLogin"] = true;
                return RedirectToAction("Index", "Home");
            }

            var mId = Convert.ToInt32(Session["MemberID"].ToString());
            var _res = Service_doctorSchedule.GetList(x => x.MemberInfoID == mId /*&& x.Status == "Close"*/).OrderByDescending(x => x.ReserveDatetime).ToList();

            return View(_res);
        }

        //植醫輔導紀錄-內容頁
        public ActionResult RecordInfo(int rid = 0)
        {
            if (Session["MemberID"] == null)
            {
                TempData["NeedLogin"] = true;
                return RedirectToAction("Index", "Home");
            }

            var mId = Convert.ToInt32(Session["MemberID"].ToString());
            var _res = Service_doctorSchedule.GetDetail(rid);
            if (_res == null || _res.MemberInfoID != mId)
            {
                return RedirectToAction(nameof(RecordList));
            }

            return View(_res);
        }

        //取消預約-預約診斷及前一日才可以取消，狀態變更為使用者取消預約
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult CancelReserve(string ID)
        {
            BaseResult result = new BaseResult();
            doctorSchedule schedule = Service_doctorSchedule.GetDetail(Convert.ToInt64(ID));
            if (schedule == null)
            {
                result.Msg = "系統找不到此紀錄";
                return Json(result);
            }


            if (DateTime.Compare(DateTime.Now.AddDays(1), schedule.ReserveDatetime) >= 0)
            {
                result.Msg = "無法取消預約";
                return Json(result);
            }

            schedule.Status = "UserCancel";
            result = Service_doctorSchedule.Update(schedule);
            result.Msg = result.result ? "取消完成" : "取消失敗";
            // TODO 需要釋放出排班?
            if (schedule.doctorDutyRecord != null)
            {
                schedule.doctorDutyRecord.DoctorScheduleID = 0;
                Service_doctor_DutyRecord.Update(schedule.doctorDutyRecord);
            }

            return Json(result);
        }

        //刪除掛號單-狀態為取消預約或者超時未看診，提供刪除紀錄(軟刪除)
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteSchedule(string ID)
        {
            BaseResult result = new BaseResult();
            doctorSchedule schedule = Service_doctorSchedule.GetDetail(Convert.ToInt64(ID));
            if (schedule == null)
            {
                result.Msg = "系統找不到此紀錄";
                return Json(result);
            }
            schedule.IsDel = true;
            result = Service_doctorSchedule.Update(schedule);
            result.Msg = result.result ? "刪除完成" : "刪除失敗";

            return Json(result);
        }
    }
}