using ICCModule.ActionFilters;
using ICCModule.Entity.Tables.Platform;
using ICCModule.EntityService.Service;
using ICCModule.Helper;
using InformationSystem.ViewModel.Management;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Security.AntiXss;

namespace InformationSystem.Controllers
{
    [LoginCheckFilter]//掛載此Filter，如果沒登入，就導去登入頁
    [InterceptorOfController]//系統操作Log
    public class PlantManagerController : Controller
    {
        //作物類別資料
        public ActionResult Category()
        {
            var _res = Service_cropType.GetList(null).OrderBy(x => x.Sort).ToList();
            return View(_res);
        }
        //作物類別編輯
        public ActionResult CategoryEdit(int id = 0)
        {
            var _res = Service_cropType.GetDetail(id) ?? new cropType();
            return View(_res);
        }
        //作物類別編輯-儲存
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult CategoryEdit(cropType cropTypeModel)
        {
            if (cropTypeModel.ID > 0 && Service_cropType.GetDetail((int)cropTypeModel.ID) != null)
            {
                Service_cropType.Update(cropTypeModel);
            }
            else
            {
                Service_cropType.Insert(cropTypeModel);
            }

            return RedirectToAction(nameof(Category));
        }

        //作物內容
        public ActionResult PlantData(int tid)
        {
            if (Service_cropType.GetDetail(tid) == null) return RedirectToAction(nameof(Category));

            var _res = new PlantListViewModel()
            {
                crops = Service_crops.GetList(x => x.TypeID == tid).OrderBy(x => x.Sort).ToList(),
                TypeId = tid
            };
            return View(_res);
        }
        //作物內容編輯
        public ActionResult PlantEdit(int id = 0, int tid = 0)
        {
            var cTypes = Service_cropType.GetDetail(tid);
            if (cTypes == null) return RedirectToAction(nameof(Category));

            var _res = Service_crops.GetDetail(id) ?? new crops();
            _res.TypeID = cTypes.ID;
            return View(_res);
        }
        //作物內容編輯-儲存
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult PlantEdit(crops cropsModel)
        {
            if (cropsModel.ID > 0 && Service_crops.GetDetail((int)cropsModel.ID) != null)
            {
                Service_crops.Update(cropsModel);
            }
            else
            {
                Service_crops.Insert(cropsModel);
            }

            return RedirectToAction(nameof(PlantData), new { tid = cropsModel.TypeID });
        }

        //防治曆-List
        public ActionResult CalendarList()
        {
            var _res = Service_controlHistory.GetList(null);
            return View(_res);
        }
        [HttpPost] //刪除方法為POST
        public ActionResult CalendarDelete(int Id)
        {
            if (Service_controlHistory.GetDetail(Id) != null)
            {
                var chc = Service_controlHistoryCrop.GetList(x => x.ControlID == Id);
                if (chc.Any())
                {
                    var chcc = Service_controlHistoryCropsContents.GetList(x => chc.Select(o => o.ID).Contains(x.ControlHistoryCropId));
                    if (chcc.Any())
                    {
                        var _rt1 = Service_controlHistoryCropsContents.DeleteMany(x => chc.Select(o => o.ID).Contains(x.ControlHistoryCropId));
                    }
                    var _rt2 = Service_controlHistoryCrop.DeleteMany(x => x.ControlID == Id);
                }
                Service_controlHistory.Delete(Id);
            }
            return RedirectToAction(nameof(CalendarList));
        }
        //防治曆-內頁
        public ActionResult CalendarInfo(int Id = 0)
        {
            var _response = new CalendarEditModel();
            var cHistory = Service_controlHistory.GetDetail(Id);
            if (cHistory != null)
            {
                _response = new CalendarEditModel
                {
                    ID = cHistory.ID,
                    Name = cHistory.Name,
                    Type = cHistory.Type,
                    Enable = cHistory.Enable,
                    Information = cHistory.Information,
                    Explanation = cHistory.Explanation,
                    ControlHistoryCrops = Service_controlHistoryCrop.GetList(x => x.ControlID == cHistory.ID)
                };
            }
            return View(_response);
        }
        //作物內容編輯-儲存
        [HttpPost]
        [ValidateAntiForgeryToken]
        [ValidateInput(false)]
        public ActionResult CalendarInfo(CalendarEditModel calendarModel)
        {
            var oldData = Service_controlHistory.GetDetail((int)calendarModel.ID);
            if (calendarModel.ID > 0 && oldData != null)
            {
                oldData.Name = calendarModel.Name;
                oldData.Type = calendarModel.Type;
                oldData.Enable = calendarModel.Enable;
                oldData.Information = calendarModel.Information;
                oldData.Explanation = calendarModel.Explanation;
                Service_controlHistory.Update(oldData);
            }
            else
            {
                var newData = new controlHistory();
                newData.Name = calendarModel.Name;
                newData.Type = calendarModel.Type;
                newData.Enable = calendarModel.Enable;
                newData.ClickCount = 0;
                newData.Information = calendarModel.Information;
                newData.Explanation = calendarModel.Explanation;
                Service_controlHistory.Insert(newData);
                calendarModel.ID = Service_controlHistory.GetList(null).OrderByDescending(x => x.CreatedAt).FirstOrDefault().ID;
            }
            if (calendarModel.ActionName == "Add")
                return RedirectToAction(nameof(CalendarAdd), new { cid = calendarModel.ID });
            else if (calendarModel.ActionName == "Update")
                return RedirectToAction(nameof(CalendarAdd), new { cid = calendarModel.ID, id = calendarModel.ControlHistoryCropId });

            return RedirectToAction(nameof(CalendarList));
        }
        //防治曆-新增
        public ActionResult CalendarAdd(int cid = 0, int id = 0)
        {
            var getControlHistory = Service_controlHistory.GetDetail((int)cid);
            if (getControlHistory == null) return RedirectToAction(nameof(CalendarList));

            var data = Service_controlHistoryCrop.GetDetail(id);
            var _res = new ControlHistoryCropEditModel();
            _res.ControlID = cid;

            if (data != null)
            {
                _res = new ControlHistoryCropEditModel()
                {
                    ID = data.ID,
                    ControlID = data.ControlID,
                    Name = data.Name,
                    Sort = data.Sort,
                    Show = data.Show,
                    Type = data.Type,
                    Comment = data.Comment,
                    Context = data.Context,
                    DayCount = data.DayCount,
                };

                var contentsData = Service_controlHistoryCropsContents.GetList(x => x.ControlHistoryCropId == _res.ID);
                var cHeader = new List<CollumHead>();
                var cGroup = new List<ContentGroup>();
                if (contentsData.Any())
                    if (_res.Type)
                    {
                        for (var i = 0; i <= 12; i++)
                        {
                            if (i == 0) cHeader.Add(new CollumHead { CollumName = "防治曆", Sort = i });
                            else cHeader.Add(new CollumHead { CollumName = $"{i}月", Sort = i });
                        }
                        _res.CollumHeads = cHeader;
                        _res.ContentGroups = contentsData.GroupBy(x => x.ControlStage).Select(item => new ContentGroup()
                        {
                            ContentName = item.Key,
                            Sort = item.Key == "生長期" ? 1 : (item.Key == "生活史" ? 2 : (item.Key == "慣行栽植" ? 3 : 4)),
                            ContentDatas = item.Select(x => new ContentData
                            {
                                Id = x.Id,
                                Name = x.Name,
                                Width = (x.EndBlock.DateToDecimal() - x.StartBlock.DateToDecimal()) * 100,
                                Margin = (x.StartBlock.DateToDecimal() - x.StartBlock.ToDateTime()?.Month ?? 0) * 100,
                                Sort = x.StartBlock.ToDateTime()?.Month ?? 0,
                                ShowType = x.ShowType,
                                StartDate = x.StartBlock.ToDateTime().Value,
                                EndDate = x.EndBlock.ToDateTime().Value,
                            }).OrderBy(x => x.Sort).ToList()
                        }).ToList();

                        int s = 1;
                        var newContentGroup = new List<ContentGroup>();
                        foreach (var item in _res.ContentGroups)
                        {
                            var j = 0;
                            var exceptId = new List<int>();
                            var oldContentData = new List<ContentData>();
                            foreach (var itemData in item.ContentDatas)
                            {
                                if (s != itemData.Sort) j = 0;
                                s = itemData.Sort;
                                var newData = new List<ContentData>();
                                if (oldContentData.Where(x => !exceptId.Contains(x.Id)).Any(x => x.Sort < s && (Math.Max(x.StartDate.Ticks, itemData.StartDate.Ticks) < Math.Min(x.EndDate.Ticks, itemData.EndDate.Ticks))))
                                {
                                    var addData = newContentGroup.FirstOrDefault(x => x.ContentDatas.All(o => !(Math.Max(o.StartDate.Ticks, itemData.StartDate.Ticks) < Math.Min(o.EndDate.Ticks, itemData.EndDate.Ticks))));
                                    if (addData != null)
                                    {
                                        addData.ContentDatas.Add(itemData);
                                    }
                                    else
                                    {
                                        newData.Add(itemData);
                                        newContentGroup.Add(new ContentGroup()
                                        {
                                            ContentName = item.ContentName,
                                            Sort = item.Sort,
                                            ContentDatas = newData
                                        });
                                    }
                                    exceptId.Add(itemData.Id);
                                }
                                else if (j > 0 && oldContentData.Where(x => !exceptId.Contains(x.Id)).Any(x => x.Sort == s && x.Id != itemData.Id && (Math.Max(x.StartDate.Ticks, itemData.StartDate.Ticks) < Math.Min(x.EndDate.Ticks, itemData.EndDate.Ticks))))
                                {
                                    var addData = newContentGroup.FirstOrDefault(x => x.ContentDatas.All(o => !(Math.Max(o.StartDate.Ticks, itemData.StartDate.Ticks) < Math.Min(o.EndDate.Ticks, itemData.EndDate.Ticks))));
                                    if (addData != null)
                                    {
                                        addData.ContentDatas.Add(itemData);
                                    }
                                    else
                                    {
                                        newData.Add(itemData);
                                        newContentGroup.Add(new ContentGroup()
                                        {
                                            ContentName = item.ContentName,
                                            Sort = item.Sort,
                                            ContentDatas = newData
                                        });
                                    }
                                    exceptId.Add(itemData.Id);
                                }
                                else
                                {
                                    oldContentData.Add(itemData);
                                }
                                j++;
                            }
                            item.ContentDatas = item.ContentDatas.Where(x => !exceptId.Contains(x.Id)).ToList();
                            s++;
                        }
                        _res.ContentGroups.AddRange(newContentGroup);
                    }
                    else
                    {
                        var maxCount = _res.DayCount ?? contentsData.Select(x => x.EndBlock.ToInt32()).Max();
                        var midCount = Math.Round((decimal)(maxCount / 10), 0, MidpointRounding.AwayFromZero);
                        for (var i = 10; i >= 0; i--)
                        {
                            if (i == 0) cHeader.Add(new CollumHead { CollumName = "防治曆", Sort = i });
                            else cHeader.Add(new CollumHead { CollumName = $"{maxCount - midCount * (10 - i)}", Sort = (int)(maxCount - midCount * (10 - i)) });
                        }
                        _res.CollumHeads = cHeader;
                        var cArray = cHeader.Where(o => o.Sort != 0).Select(o => o.Sort).ToList();
                        _res.ContentGroups = contentsData.GroupBy(x => x.ControlStage).Select(item => new ContentGroup()
                        {
                            ContentName = item.Key,
                            Sort = item.Key == "生長期" ? 1 : (item.Key == "生活史" ? 2 : (item.Key == "慣行栽植" ? 3 : 4)),
                            ContentDatas = item.Select(x => new ContentData
                            {
                                Id = x.Id,
                                Name = x.Name,
                                Width = (x.EndBlock.ToInt32().IntInterval(cArray) - x.StartBlock.ToInt32().IntInterval(cArray)) * 100,
                                Margin = (x.StartBlock.ToInt32().IntInterval(cArray) * 100) % 100,
                                Sort = (int)(x.StartBlock.ToInt32().IntInterval(cArray) / 1) + 1,
                                ShowType = x.ShowType,
                                StartInt = x.StartBlock.ToInt32(),
                                EndInt = x.EndBlock.ToInt32(),
                            }).OrderBy(x => x.Sort).ToList()
                        }).ToList();

                        int s = 1;
                        var newContentGroup = new List<ContentGroup>();
                        foreach (var item in _res.ContentGroups)
                        {
                            var j = 0;
                            var exceptId = new List<int>();
                            var oldContentData = new List<ContentData>();
                            foreach (var itemData in item.ContentDatas)
                            {
                                if (s != itemData.Sort) j = 0;
                                s = itemData.Sort;
                                var newData = new List<ContentData>();
                                if (oldContentData.Any(x => x.Sort < s && (Math.Max(x.StartInt, itemData.StartInt) < Math.Min(x.EndInt, itemData.EndInt))))
                                {
                                    var addData = newContentGroup.FirstOrDefault(x => x.ContentDatas.All(o => !(Math.Max(o.StartInt, itemData.StartInt) < Math.Min(o.EndInt, itemData.EndInt))));
                                    if (addData != null)
                                    {
                                        addData.ContentDatas.Add(itemData);
                                    }
                                    else
                                    {
                                        newData.Add(itemData);
                                        newContentGroup.Add(new ContentGroup()
                                        {
                                            ContentName = item.ContentName,
                                            Sort = item.Sort,
                                            ContentDatas = newData
                                        });
                                    }
                                    exceptId.Add(itemData.Id);
                                }
                                else if (j > 0 && oldContentData.Any(x => x.Sort == s && x.Id != itemData.Id && (Math.Max(x.StartInt, itemData.StartInt) < Math.Min(x.EndInt, itemData.EndInt))))
                                {
                                    var addData = newContentGroup.FirstOrDefault(x => x.ContentDatas.All(o => !(Math.Max(o.StartInt, itemData.StartInt) < Math.Min(o.EndInt, itemData.EndInt))));
                                    if (addData != null)
                                    {
                                        addData.ContentDatas.Add(itemData);
                                    }
                                    else
                                    {
                                        newData.Add(itemData);
                                        newContentGroup.Add(new ContentGroup()
                                        {
                                            ContentName = item.ContentName,
                                            Sort = item.Sort,
                                            ContentDatas = newData
                                        });
                                    }
                                    exceptId.Add(itemData.Id);
                                }
                                else
                                {
                                    oldContentData.Add(itemData);
                                }
                                j++;
                            }
                            item.ContentDatas = item.ContentDatas.Where(x => !exceptId.Contains(x.Id)).ToList();
                            s++;
                        }
                        _res.ContentGroups.AddRange(newContentGroup);
                    }
            }
            return View(_res);
        }
        //作物內容編輯-儲存
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult CalendarAdd(ControlHistoryCropEditModel controlHistoryCropEditModel)
        {
            var oldData = Service_controlHistoryCrop.GetDetail((int)controlHistoryCropEditModel.ID);
            if (controlHistoryCropEditModel.ID > 0 && oldData != null)
            {
                oldData.Name = controlHistoryCropEditModel.Name;
                oldData.Sort = controlHistoryCropEditModel.Sort;
                oldData.Show = controlHistoryCropEditModel.Show;
                oldData.Type = controlHistoryCropEditModel.Type;
                oldData.DayCount = controlHistoryCropEditModel.DayCount;
                oldData.Comment = controlHistoryCropEditModel.Comment;
                oldData.Context = controlHistoryCropEditModel.Context;
                Service_controlHistoryCrop.Update(oldData);
            }
            else
            {
                var newData = new controlHistoryCrop();
                newData.ControlID = controlHistoryCropEditModel.ControlID;
                newData.Name = controlHistoryCropEditModel.Name;
                newData.Sort = controlHistoryCropEditModel.Sort;
                newData.Show = controlHistoryCropEditModel.Show;
                newData.Type = controlHistoryCropEditModel.Type;
                newData.DayCount = controlHistoryCropEditModel.DayCount;
                newData.Comment = controlHistoryCropEditModel.Comment;
                newData.Context = controlHistoryCropEditModel.Context;
                Service_controlHistoryCrop.Insert(newData);

                controlHistoryCropEditModel.ID = Service_controlHistoryCrop.GetList(x => x.ControlID == controlHistoryCropEditModel.ControlID).OrderByDescending(x => x.CreatedAt).FirstOrDefault().ID;
            }
            if (controlHistoryCropEditModel.ActionName == "Add")
                return RedirectToAction(nameof(CropContentAdd), new { cid = controlHistoryCropEditModel.ControlID, chcid = controlHistoryCropEditModel.ID });
            else if (controlHistoryCropEditModel.ActionName == "Update")
                return RedirectToAction(nameof(CropContentAdd), new { cid = controlHistoryCropEditModel.ControlID, chcid = controlHistoryCropEditModel.ID, id = controlHistoryCropEditModel.ContentId });

            return RedirectToAction(nameof(CalendarAdd), new { cid = controlHistoryCropEditModel.ControlID, id = controlHistoryCropEditModel.ID });
        }
        //防治曆-新增
        public ActionResult CropContentAdd(int cid = 0, int chcid = 0, int id = 0)
        {
            var getControlHistoryCrop = Service_controlHistoryCrop.GetDetail((int)chcid);
            //if (getControlHistoryCrop == null) return RedirectToAction(nameof(CalendarList));

            var data = Service_controlHistoryCropsContents.GetDetail(id);
            var _res = new ControlHistoryCropsContentsEditModel();
            _res.ControlHistoryCropId = chcid;
            _res.ControlId = cid;
            _res.TypeId = getControlHistoryCrop.Type;
            _res.DayCount = getControlHistoryCrop.DayCount;

            if (data != null)
            {
                _res = new ControlHistoryCropsContentsEditModel()
                {
                    Id = data.Id,
                    ControlHistoryCropId = data.ControlHistoryCropId,
                    ControlStage = data.ControlStage,
                    Name = data.Name,
                    ShowType = data.ShowType,
                    StartBlock = data.StartBlock,
                    EndBlock = data.EndBlock,
                    TypeId = getControlHistoryCrop.Type,
                    DayCount = getControlHistoryCrop.DayCount,
                    ControlId = getControlHistoryCrop.ControlID,
                };
            }

            return View(_res);
        }
        //作物內容編輯-儲存
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult CropContentAdd(ControlHistoryCropsContentsEditModel controlHistoryCropEditModel)
        {
            var oldData = Service_controlHistoryCropsContents.GetDetail((int)controlHistoryCropEditModel.Id);
            if (controlHistoryCropEditModel.ActName == "del")
            {
                Service_controlHistoryCropsContents.Delete(oldData.Id);
            }
            else
            {
                if (controlHistoryCropEditModel.Id > 0 && oldData != null)
                {
                    oldData.Id = controlHistoryCropEditModel.Id;
                    oldData.ControlHistoryCropId = controlHistoryCropEditModel.ControlHistoryCropId;
                    oldData.ControlStage = controlHistoryCropEditModel.ControlStage;
                    oldData.Name = controlHistoryCropEditModel.Name;
                    oldData.ShowType = controlHistoryCropEditModel.ShowType;
                    oldData.StartBlock = controlHistoryCropEditModel.StartBlock;
                    oldData.EndBlock = controlHistoryCropEditModel.EndBlock;
                    Service_controlHistoryCropsContents.Update(oldData);
                }
                else
                {
                    var newData = new controlHistoryCropsContents()
                    {
                        Id = controlHistoryCropEditModel.Id,
                        ControlHistoryCropId = controlHistoryCropEditModel.ControlHistoryCropId,
                        ControlStage = controlHistoryCropEditModel.ControlStage,
                        Name = controlHistoryCropEditModel.Name,
                        ShowType = controlHistoryCropEditModel.ShowType,
                        StartBlock = controlHistoryCropEditModel.StartBlock,
                        EndBlock = controlHistoryCropEditModel.EndBlock,
                    };
                    Service_controlHistoryCropsContents.Insert(newData);
                    controlHistoryCropEditModel.Id = Service_controlHistoryCropsContents.GetList(x => x.ControlHistoryCropId == controlHistoryCropEditModel.ControlHistoryCropId).OrderByDescending(x => x.CreatedAt).FirstOrDefault().Id;
                }
            }
            return RedirectToAction(nameof(CalendarAdd), new { cid = controlHistoryCropEditModel.ControlId, id = controlHistoryCropEditModel.ControlHistoryCropId });
        }
        [HttpPost] //刪除方法為POST
        [ValidateAntiForgeryToken]
        public ActionResult ContentDelete(int Id)
        {
            var thisData = Service_controlHistoryCropsContents.GetDetail(Id);
            long cid = 0, id = 0;
            if (thisData != null)
            {
                var getControlHistoryCrop = Service_controlHistoryCrop.GetDetail((int)thisData.ControlHistoryCropId);
                if (getControlHistoryCrop != null)
                {
                    cid = getControlHistoryCrop.ControlID;
                    id = getControlHistoryCrop.ID;
                }
                Service_controlHistory.Delete(Id);
            }
            if (cid != 0 && id != 0) return RedirectToAction(nameof(CalendarAdd), new { cid = cid, id = id });
            return RedirectToAction(nameof(CalendarList));
        }


        public string GetCropDatas(int id)
        {
            var _res = Service_crops.GetEnableList().Where(x => x.TypeID == id).ToList();

            return JsonConvert.SerializeObject(_res);
        }
    }
}