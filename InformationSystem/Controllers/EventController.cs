using ICCModule;
using ICCModule.ActionFilters;
using ICCModule.Entity.Tables.Platform;
using ICCModule.EntityService.Service;
using ICCModule.Helper;
using InformationSystem.ViewModel.Event;
using IscomG2C.Utility;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Web;
using System.Web.Mvc;

namespace InformationSystem.Controllers
{
    [LoginCheckFilter]//掛載此Filter，如果沒登入，就導去登入頁
    [InterceptorOfController]//系統操作Log
    public class EventController : Controller
    {
        //活動報名管理-列表
        public ActionResult EventList(DateTime? StartDate, DateTime? EndDate , bool? Show ,string Code = "",  List<string> DeleteIDs = null)
        {
            // 刪除指定ID的消息
            if (DeleteIDs != null && DeleteIDs.Any())
            {
                try
                {
                    DeleteIDs.ForEach(delegate (string id)
                    {
                        var checkOpen = Service_activityPlanOpenTIme.GetList(Extention.ToInt32(id));
                        foreach (var item in checkOpen)
                        {
                            var checkRegist = Service_activityRegistration.GetList(item.ID);
                            foreach (var item2 in checkRegist)
                            {
                                Service_activityRegistration.Delete(item2.ID);
                            }
                            Service_activityPlanOpenTIme.Delete((int)item.ID);
                        }
                        Service_activityPlan.Delete(int.Parse(id));
                    });
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.Message);
                }
            }

            Expression<Func<activityPlan, bool>> filter = x => true;
            if (StartDate.HasValue)
            {
                filter = filter.And(x => x.StartDate.Date >= StartDate);
            }
            if (EndDate.HasValue)
            {
                filter = filter.And(x => x.StartDate.Date <= EndDate);
            }
            if (!string.IsNullOrEmpty(Code))
            {
                filter = filter.And(x => x.Type == Code);
            }
            if (Show.HasValue)
            {
                filter = filter.And(x => x.Show == Show);
            }

            EventListViewModel model = new EventListViewModel()
            {
                Types = Service_defCode.GetList("ActivityType"),
                QueryReq = new EventQueryReq()
                {
                    StartDate = StartDate,
                    EndDate = EndDate,  
                    Show = Show,
                    Code = Code,
                },
                Plans = Service_activityPlan.GetList(filter),
                CurrentPage = 1,
                TotalPage = 1,
                PageCounts = 30
            };

            return View(model);
        }


        //活動報名管理-新增、編輯
        public ActionResult EventInfo(int ID = 0)
        {
            var model = new EventInfoViewModel()
            {
                Types = Service_defCode.GetList("ActivityType"),
                Plan = Service_activityPlan.GetDetail(ID) ?? new activityPlan(),
            };
            if (model.Plan != null && model.Plan.Open)
            {
                model.OpenTimes = Service_activityPlanOpenTIme.GetList(x => x.ActivityPlanID == ID);
                model.OldFiles = Service_FileManagement.GetList("activity_Plans", ID.ToString());
            }

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult EventInfo(EventInfoViewModel infoViewModel)
        {
            infoViewModel.Plan.Fields = infoViewModel.PlanField;

            if (Convert.ToBoolean(infoViewModel.Plan.ID))
            {
                Service_activityPlan.Update(infoViewModel.Plan);
                // 更新場次
                foreach (var openTime in infoViewModel.OpenTimes)
                {
                    openTime.Date = openTime.DateStr.ToDateTime().Value;
                    if (openTime.ID > 0) Service_activityPlanOpenTIme.Update(openTime);
                    else
                    {
                        openTime.ActivityPlanID = infoViewModel.Plan.ID;
                        openTime.CreatedAt = DateTime.Now;
                        Service_activityPlanOpenTIme.Insert(openTime);
                    }
                }
            }
            else
            {

                Service_activityPlan.Insert(infoViewModel.Plan);
                // 寫入場次
                foreach (var openTime in infoViewModel.OpenTimes)
                {
                    openTime.Date = openTime.DateStr.ToDateTime().Value;
                    openTime.ActivityPlanID = infoViewModel.Plan.ID;
                    openTime.CreatedAt = DateTime.Now;
                    Service_activityPlanOpenTIme.Insert(openTime);
                }

            }

            try
            {
                var _oldFile = Service_FileManagement.GetList("activity_Plans", infoViewModel.Plan.ID.ToString());
                if (_oldFile.Any())
                {
                    foreach (var item in _oldFile.Where(x => !infoViewModel.EditOldFiles.Contains(x.ID)))
                    {
                        var thisFile = _oldFile.FirstOrDefault(x => x.ID == item.ID);
                        if (thisFile == null)
                        {
                            continue;
                        }
                        else
                        {
                            string _path = Server.MapPath("~" + thisFile.FilePath);
                            if (System.IO.File.Exists(_path))
                            {
                                System.IO.File.Delete(_path);
                            }
                            Service_FileManagement.Delete((int)thisFile.ID);
                        }
                    }
                }

                if (infoViewModel.Files.Count > 0)
                {
                    string baseDirectory = "/UploadedFiles/Activity_Plans";
                    foreach (var file in infoViewModel.Files)
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
                            TableName = "activity_Plans",
                            TableID = infoViewModel.Plan.ID.ToString(),
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

            return RedirectToAction(nameof(EventList));
        }


        //報名資料
        public ActionResult RegisterList(int ID = 0)
        {
            var planData = Service_activityPlan.GetDetail(ID);
            if (planData == null)
            {
                return RedirectToAction(nameof(EventList));
            }

            List<activityPlanOpenTime> model = Service_activityPlanOpenTIme.GetList(ID);

            return View(model);
        }

        //報名資料-查看名單
        public ActionResult RegisterInfo(int ID)
        {
            if (Service_activityPlanOpenTIme.GetDetail(ID) == null)
            {
                return RedirectToAction(nameof(EventList));
            }

            List<activityRegistraction> model = Service_activityRegistration.GetList(ID);

            return View(model);
        }

        /// <summary>
        /// 報名表
        /// </summary>
        /// <param name="Id"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult OutputExcel(int Id = 0)
        {
            var data = Service_activityPlanOpenTIme.GetDetail(Id);
            if (data == null)
            {
                return RedirectToAction(nameof(EventList));
            }

            var actRegister = Service_activityRegistration.GetList(Id).Select(x => new RegistSignModel
            {
                序號 = "",
                姓名 = x.Name,
                身分證字號 = x.IdentifiedID,
                認證時數 = x.ClassHours.ToString("0.0#"),
            }).ToList();
            var i = 1;
            foreach (var item in actRegister)
            {
                item.序號 = i.ToString();
                i++;
            };
            return File(ExcelHelper.RenderDataTableToExcel(ExcelHelper.ConvertToDataTable(actRegister)), "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", $"{data.Name}報名表.xlsx");

            return RedirectToAction(nameof(RegisterInfo), new { ID = Id });
        }
        [HttpPost] //刪除方法為POST
        public ActionResult ChangeCheck(int Id = 0, int PlanOpenTimeID = 0)
        {
            var actRegister = Service_activityRegistration.GetDetail(Id);
            if (actRegister != null)
            {
                Service_activityRegistration.CheckRegist(Id);
            }

            return RedirectToAction(nameof(RegisterInfo), new { ID = PlanOpenTimeID });
        }
        //成果資料
        public ActionResult Achievement(int ID = 0)
        {
            var planData = Service_activityPlan.GetDetail(ID);
            if (planData == null)
            {
                return RedirectToAction(nameof(EventList));
            }

            List<activityPlanOpenTime> model = Service_activityPlanOpenTIme.GetList(ID);

            return View(model);
        }

        /// <summary>
        /// 匯出簽到表
        /// </summary>
        /// <param name="Id"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult OutputSignExcel(int Id = 0)
        {
            var data = Service_activityPlanOpenTIme.GetDetail(Id);
            if (data == null)
            {
                return RedirectToAction(nameof(EventList));
            }

            var actRegister = Service_activitySign.GetList(x => x.ActivityOpenTimesId == Id).Select(x => new SignInExportModel
            {
                序號 = "",
                姓名 = x.Name,
                身分證字號 = x.IdentityKey,
                手機號碼 = x.Phone,
            }).ToList();
            var i = 1;
            foreach (var item in actRegister)
            {
                item.序號 = i.ToString();
                i++;
            };
            return File(ExcelHelper.RenderDataTableToExcel(ExcelHelper.ConvertToDataTable(actRegister)), "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", $"{data.Name}簽到表.xlsx");
        }


        //成果資料-編輯成果
        public ActionResult AchievementEdit(int ID = 0)
        {
            activityPlanOpenTime model = Service_activityPlanOpenTIme.GetDetail(ID);
            if (model == null)
            {
                return RedirectToAction(nameof(EventList));
            }

            var data = new AchievementModel() 
            {
                Data = model,
                SignInResultFiles = Service_FileManagement.GetList("actPlanOpenTimes_srf", ID.ToString()),
                PicturesFiles = Service_FileManagement.GetList("actPlanOpenTimes_pic", ID.ToString()),
            };

            return View(data);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult AchievementEdit(int ID, int Attendance, List<HttpPostedFileBase> SignInResultFiles = null, List<HttpPostedFileBase> PictureFiles = null, List<long> EditSignFiles = null, List<long> EditPicFiles = null)
        {
            activityPlanOpenTime model = Service_activityPlanOpenTIme.GetDetail(ID);
            if (model == null)
            {
                return RedirectToAction(nameof(EventList));
            }

            model.Attendance = Attendance;
            try
            {
                if(EditSignFiles!=null && EditSignFiles.Any())
                {
                    var _oldFile = Service_FileManagement.GetList("actPlanOpenTimes_srf", model.ID.ToString());
                    if (_oldFile.Any())
                    {
                        foreach (var item in _oldFile.Where(x => !EditSignFiles.Contains(x.ID)))
                        {
                            var thisFile = _oldFile.FirstOrDefault(x => x.ID == item.ID);
                            if (thisFile == null)
                            {
                                continue;
                            }
                            else
                            {
                                string _path = Server.MapPath("~" + thisFile.FilePath);
                                if (System.IO.File.Exists(_path))
                                {
                                    System.IO.File.Delete(_path);
                                }
                                Service_FileManagement.Delete((int)thisFile.ID);
                            }
                        }
                    }
                }
                if (SignInResultFiles.Count > 0)
                {
                    string baseDirectory = "/UploadedFiles/Activity_Plan_Open_Times/SignInResult";
                    foreach (var file in SignInResultFiles)
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
                            TableName = "actPlanOpenTimes_srf",
                            TableID = model.ID.ToString(),
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
            try
            {
                if (EditPicFiles != null && EditPicFiles.Any())
                {
                    var _oldFile = Service_FileManagement.GetList("actPlanOpenTimes_pic", model.ID.ToString());
                    if (_oldFile.Any())
                    {
                        foreach (var item in _oldFile.Where(x => !EditPicFiles.Contains(x.ID)))
                        {
                            var thisFile = _oldFile.FirstOrDefault(x => x.ID == item.ID);
                            if (thisFile == null)
                            {
                                continue;
                            }
                            else
                            {
                                string _path = Server.MapPath("~" + thisFile.FilePath);
                                if (System.IO.File.Exists(_path))
                                {
                                    System.IO.File.Delete(_path);
                                }
                                Service_FileManagement.Delete((int)thisFile.ID);
                            }
                        }
                    }
                }
                if (PictureFiles.Count > 0)
                {
                    string baseDirectory = "/UploadedFiles/Activity_Plan_Open_Times/Pictures";
                    foreach (var file in PictureFiles)
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
                            TableName = "actPlanOpenTimes_pic",
                            TableID = model.ID.ToString(),
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
            var srFiles = Service_FileManagement.GetList("actPlanOpenTimes_srf", ID.ToString());
            var picFiles = Service_FileManagement.GetList("actPlanOpenTimes_pic", ID.ToString());
            model.SignInResult = String.Join(",", srFiles.Select(x => x.FilePath));
            model.Pictures = String.Join(",", picFiles.Select(x => x.FilePath));
            Service_activityPlanOpenTIme.Update(model);

            return RedirectToAction(nameof(Achievement), new { ID = model.ActivityPlanID });
        }

        public ActionResult SignQrcode(int ID = 0)
        {
            var checkOpenTime = Service_activityPlanOpenTIme.GetDetail(ID);
            if (checkOpenTime == null) RedirectToAction(nameof(EventList));

            var _qrcode = new OpenTimeQrcode
            {
                imgSrc = $"https://chart.googleapis.com/chart?chs=500x500&cht=qr&chl={AppSettingHelper.GetAppSetting("Front_HostName")}/Event/SignIn?id={ID}&chld=L|4"
            };
            return View(_qrcode);
        }
    }
}