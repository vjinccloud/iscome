using ICCModule.Entity.Tables.Platform;
using ICCModule.EntityService.Service;
using IscomG2C.Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Security.AntiXss;
using Website.ViewModel;

namespace Website.Controllers
{
    public class EventController : Controller
    {
        //活動資訊
        public ActionResult AllEvent(EventQueryModel req)
        {
            var _res = Service_activityPlan.GetList(x => x.Show);

            if (req.StartDate.HasValue)
            {
                _res = _res.Where(x => x.StartDate >= req.StartDate).ToList();
            }
            if (req.EndDate.HasValue)
            {
                _res = _res.Where(x => x.StartDate.Date <= req.EndDate).ToList();
            }
            if (!string.IsNullOrEmpty(req.EventType))
            {
                _res = _res.Where(x => x.Type == req.EventType).ToList();
            }
            if (!string.IsNullOrEmpty(req.KeyWord))
            {
                _res = _res.Where(x => x.Name.Contains(req.KeyWord)).ToList();
            }
            if(req.PageIndex <1) req.PageIndex = 1;
            var data = new EventShowModel()
            {
                QueryData = req,
                PlanData = _res.Skip(10*(req.PageIndex-1)).Take(10).ToList(),
                TotalPage = (_res.Count() / 10) + 1,
                CalendarEvents = Service_activityPlan.GetList(x => x.Show).Select(x => new CalendarEvent
                {
                    title = x.Name,
                    url = AntiXssEncoder.HtmlEncode($"/Event/Info?id={x.ID}", true),
                    start = AntiXssEncoder.HtmlEncode(x.StartDate.ToString("yyyy-MM-dd"), true),
                    end = AntiXssEncoder.HtmlEncode(x.EndDate.AddDays(1).ToString("yyyy-MM-dd"), true)
                }).ToList()
            };
            return View(data);
        }
        //活動資訊-內容
        public ActionResult Info(int id = 0)
        {
            var _data = Service_activityPlan.GetDetail(id);
            if (_data == null) return RedirectToAction(nameof(AllEvent));

            _data.OpenTimes = Service_activityPlanOpenTIme.GetList(_data.ID);
            _data.Context = _data.Context;

            return View(_data);
        }
        //活動資訊-內容(無報名)
        public ActionResult NoRegisterInfo(int id = 0)
        {
            var _data = Service_activityPlan.GetDetail(id);
            if (_data == null) return RedirectToAction(nameof(AllEvent));

            _data.OpenTimes = Service_activityPlanOpenTIme.GetList(_data.ID);
            _data.Context = _data.Context;

            return View(_data);
        }
        //報名資訊
        public ActionResult Register(int id = 0)
        {
            var _data = Service_activityPlan.GetDetail(id);
            //if (_data == null) return RedirectToAction(nameof(AllEvent));
            //if (!_data.Open) return RedirectToAction(nameof(NoRegisterInfo), new { id = id });

            _data.OpenTimes = Service_activityPlanOpenTIme.GetList(_data.ID).Where(x => x.Nums == 0 || x.Nums > x.RegisteredNums).ToList();
            _data.Context =_data.Context;
            var memberData = new RegistDataModel();
            if (Session["MemberID"] != null)
            {
                var mId = Convert.ToInt32(Session["MemberID"].ToString());
                var member = Service_MemberInfo.GetDetail(mId);
                if (member != null)
                {
                    memberData = new RegistDataModel()
                    {
                        Name = member.Name,
                        Phone = member.Mobile,
                    };
                }
            }
            if (_data.Fields.MealsTypeNeed)
            {
                memberData.MealsType = true;
            }
            memberData.ActId = id;

            var _res = new RegistShowModel()
            {
                PlanData = _data,
                RegistData = memberData,
            };

            return View(_res);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Register(RegistDataModel req)
        {
            var _data = Service_activityPlan.GetDetail(req.ActId);
            if (_data == null) return RedirectToAction(nameof(AllEvent));
            if (!_data.Open) return RedirectToAction(nameof(NoRegisterInfo), new { id = req.ActId });

            _data.OpenTimes = Service_activityPlanOpenTIme.GetList(_data.ID).Where(x => x.Nums == 0 || x.Nums > x.RegisteredNums).ToList();

            var _res = new RegistShowModel()
            {
                PlanData = _data,
                RegistData = req,
            };
            var errMsg = "";
            if (!req.PlanOpenTimeID.Any())
            {
                errMsg = "請選擇欲報名之場次";
            }
            else if (req.PlanOpenTimeID.Any(x => !_data.OpenTimes.Select(o => o.ID).Contains(x)))
            {
                errMsg = "欲報名之場次已額滿，請重新選擇";
            }
            else if (Check_VerifyCode(req.ValidateCode) == false)
            {
                errMsg = "驗證碼資訊不符，請重新輸入";
            }

            if (!string.IsNullOrEmpty(errMsg))
            {
                TempData["TempMsg"] = errMsg;
                TempData["TempResult"] = "error";
                return View(_res);
            }
            else
            {
                int? mId = null;
                if (Session["MemberID"] != null)
                {
                    mId = Convert.ToInt32(Session["MemberID"].ToString());
                }
                var _planData = Service_activityPlan.GetDetail(req.ActId);
                var addRegist = req.PlanOpenTimeID.Select(x => new activityRegistraction
                {
                    MemberId = mId,
                    PlanOpenTimeID = x,
                    Name = req.Name,
                    IdentifiedID = req.IdentifiedID,
                    Phone = req.Phone,
                    ClassHours = _planData.ClassHours,
                    City = req.City,
                    PesticideManagementStaffID = req.PesticideManagementStaffID,
                    PesticideManagementStaffExpiryDate = req.PesticideManagementStaffExpiryDate,
                    ServiceUnit = req.ServiceUnit,
                    MealsType = req.MealsType,
                    Status = 1,
                    CreatedAt = DateTime.Now,
                    UpdatedAt = DateTime.Now,
                });
                foreach (var item in addRegist)
                {
                    Service_activityRegistration.Insert(item);
                }
                return RedirectToAction(nameof(AllEvent));
            }
        }



        /// <summary>登入頁面使用-產生隨機認證碼圖片 並且設定驗證碼在SESSION值中
        /// 
        /// </summary>
        public void GetValidateCodeImage()
        {
            string Code = Logic_ValidateCodeImage.GetValidateCode(4);
            //將驗證碼紀錄在Session中
            Session["EventValidate"] = Code;

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
            if (Session["EventValidate"] == null)
                return false;

            if (Session["EventValidate"].ToString().ToUpper() != m_sValidate.ToUpper())
                return false;

            //通過檢查驗證碼檢查
            return true;

        }


        //報名資訊
        public ActionResult SignIn(int id = 0)
        {
            var getActivityOpen = Service_activityPlanOpenTIme.GetDetail(id);
            if (getActivityOpen == null) return RedirectToAction(nameof(AllEvent));

            var getActivity = Service_activityPlan.GetDetail((int)getActivityOpen.ActivityPlanID);
            if (getActivity == null || getActivity?.Open != true || getActivity?.Show != true) return RedirectToAction(nameof(AllEvent));

            var res = new SignInModel
            {
                OpenId = id,
                ActivityName = $"{getActivity.Name}-{getActivityOpen.Name}",
                WrongSignDate = getActivityOpen.Date != DateTime.Now.Date
            };

            return View(res);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult SignIn(activitySign obj)
        {
            var getActivityOpen = Service_activityPlanOpenTIme.GetDetail(obj.ActivityOpenTimesId);
            if (getActivityOpen == null) return RedirectToAction(nameof(AllEvent));

            var getActivity = Service_activityPlan.GetDetail((int)getActivityOpen.ActivityPlanID);
            if (getActivity == null || getActivity?.Open != true || getActivity?.Show != true) return RedirectToAction(nameof(AllEvent));

            var res = new SignInModel();
            if (getActivityOpen.Date == DateTime.Now.Date)
            {
                res = new SignInModel
                {
                    OpenId = obj.ActivityOpenTimesId,
                    ActivityName = $"{getActivity.Name}-{getActivityOpen.Name}",
                    WrongSignDate = getActivityOpen.Date != DateTime.Now.Date,


                };
                var checkSign = Service_activitySign.GetList(x => x.ActivityOpenTimesId == obj.ActivityOpenTimesId && x.Name == obj.Name && x.Phone == obj.Phone && x.IdentityKey == obj.IdentityKey);
                if (checkSign.Any())
                {
                    res.SignSuccess = false;
                    res.ErrMsg = "您已報到";
                    res.Url = $"/Event/SignIn?id={obj.ActivityOpenTimesId}";
                }
                else
                {
                    var _res = Service_activitySign.Insert(obj);
                    res.SignSuccess = _res.result;
                    res.Url = $"/Event/Info?id={getActivity.ID}";
                }
            }
            else
            {
                res = new SignInModel
                {
                    OpenId = obj.ActivityOpenTimesId,
                    ActivityName = $"{getActivity.Name}-{getActivityOpen.Name}",
                    WrongSignDate = getActivityOpen.Date != DateTime.Now.Date
                };
            }

            return View(res);
        }
    }
}