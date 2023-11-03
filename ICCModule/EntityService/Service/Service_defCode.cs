using ICCModule.Entity.Tables;
using ICCModule.Repository;
using ICCModule.Repository.Class;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace ICCModule.EntityService.Service
{
    public class Service_defCode
    {
        /// <summary>
        /// 抓取單筆資料
        /// </summary>
        /// <param name="Code"></param>
        /// <returns></returns>
        public static defCode GetDetail(string Code)
        {
            return RepositoryUtility.GetDetail<defCode>(x => x.Code == Code);
        }

        public static defCode GetDetail(string Kind, string Code)
        {
            if (RepositoryUtility.GetDetail<defCode>(x => x.Code == Code && x.Kind == Kind) !=null)
            {
                return RepositoryUtility.GetDetail<defCode>(x => x.Code == Code && x.Kind == Kind);
            }
            else if (Code == "Other")
            {
                return new defCode
                {
                    Code = Code,
                    Kind = Kind,
                    Name = "其他"
                };
            }
            return RepositoryUtility.GetDetail<defCode>(x => x.Code == Code && x.Kind == Kind);
        }

        /// <summary>
        /// 讀取清單
        /// </summary>
        /// <returns></returns>
        public static List<defCode> GetList(string Kind = "", string NoUse = "N")
        {
            Expression<Func<defCode, bool>> filter = (x => true);
            if (!string.IsNullOrEmpty(Kind))
            {
                filter = filter.And(x => x.Kind == Kind);
            }
            if (!string.IsNullOrEmpty(NoUse))
            {
                filter = filter.And(x => x.NoUse == NoUse);
            }
            return RepositoryUtility.GetList<defCode>(filter).OrderBy(x => x.Sort).ToList();
        }

        /// <summary>
        /// 讀取清單
        /// </summary>
        /// <returns></returns>
        public static List<defCode> GetPageList(string Kind = "", PagerInfo page = null)
        {
            Expression<Func<defCode, bool>> filter = (x => true);
            if (!string.IsNullOrEmpty(Kind))
            {
                filter = filter.And(x => x.Kind == Kind);
            }

            if (page != null)
                return RepositoryUtility.GetList_Range<defCode, string>(page, filter, x => x.Code, true).ToList();
            else
                return RepositoryUtility.GetList<defCode>(filter);
        }

        public static List<SelectListItem> GetSelectList(string Kind = "", bool NeedFirst = false, string FirstVal = "", string FirstText = "請選擇")
        {
            List<defCode> tmpList = GetList(Kind);
            List<SelectListItem> ResultList = new List<SelectListItem>();
            if (NeedFirst)
            {
                ResultList.Add(new SelectListItem { Text = FirstText, Value = FirstVal });
            }
            foreach (var item in tmpList)
            {
                ResultList.Add(new SelectListItem { Text = item.Name, Value = item.Code });
            }
            return ResultList;
        }
        /// <summary>代碼類型下拉選單
        /// 
        /// </summary>
        public static List<SelectListItem> GetKindSelectList()
        {
            var tmpList = GetList().GroupBy(x => new { x.Kind, x.KindName }).ToList();
            List<SelectListItem> ResultList = new List<SelectListItem>();
            ResultList = tmpList.ConvertAll(x => { return new SelectListItem() { Text = x.Key.KindName, Value = x.Key.Kind }; });
            return ResultList;
        }
        public static defCode GetKindDetail(string Kind)
        {
            defCode ResultList = new defCode();
            ResultList = RepositoryUtility.GetList<defCode>(x => x.Kind == Kind).FirstOrDefault();
            return ResultList;
        }
        /// <summary>
        /// 寫入單筆資料
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public static BaseResult Insert(defCode data)
        {
            int DataCnt = RepositoryUtility.GetCount<defCode>(x => x.Kind == data.Kind && x.Code == data.Code);
            if (DataCnt > 0)
            {
                return new BaseResult(false, "此代碼編號已被使用，無法新增。");
            }
            data.Sort = 99; //棄用排序
            data.Remark = data.Remark ?? "";
            return RepositoryUtility.Insert<defCode>(data);
        }

        /// <summary>
        /// 更新單筆資料
        /// </summary>
        public static BaseResult Update(defCode data)
        {
            string sErrMsg = "";

            BaseRepository Base = new BaseRepository();
            //查詢資料
            var model = Base
                .QueryData<defCode>(ref sErrMsg, x => x.Kind == data.Kind && x.Code == data.Code && x.AllowUpdate == "Y")
                .SingleOrDefault();
            if (model == null)
            {
                sErrMsg = "查無欲更新的資料";
                return new BaseResult(false, sErrMsg); 
            }
            model.Name = data.Name;
            model.NoUse = data.NoUse;
            data.Remark = data.Remark ?? "";
            model.Remark = data.Remark;
            model.UpdateDate = data.UpdateDate;
            model.UpdateUser = data.UpdateUser;

            //更新回資料庫
            bool rslt = Base.Update(ref sErrMsg);
            return new BaseResult(rslt, sErrMsg);
        }

        /// <summary>
        /// 刪除
        /// </summary>
        public static BaseResult Delete(string Kind,string Code)
        {
            //確認是否有使用過
            bool isUse = false;
            switch (Kind)
            {
                //允許編輯的類型
                //case "defEquipment_Category":
                //    var _tempA = Service_defEquipment.GetList(new Service_defEquipment.QryParam() { Category = Code });
                //    isUse = _tempA.Count > 0;
                //    break;
                //case "defInstrument_Category":
                //    var _tempB = Service_defInstrument.GetList(new Service_defInstrument.QryParam() { Category = Code });
                //    isUse = _tempB.Count > 0;
                //    break;
                //case "insIoTSystem_State":
                //    var _tempC = Service_insIoTSystem.GetList(new Service_insIoTSystem.QryParam() { SystemState = Code });
                //    isUse = _tempC.Count > 0;
                //    break;
                //case "insIoTSystem_Type":
                //    var _tempD = Service_insIoTSystem.GetList(new Service_insIoTSystem.QryParam() { SystemType = Code });
                //    isUse = _tempD.Count > 0;
                //    break;
                case "PlantDoctorAm":
                case "PlantDoctorPm":
                    isUse = false;
                    break;
                default: //不允許刪除
                    isUse = true; 
                    break;
            }

            if (isUse)
            {
                return new BaseResult(false, "此代碼已被使用，不允許刪除!"); 
            }
            return RepositoryUtility.Delete<defCode>(x => x.Kind == Kind && x.Code == Code && x.AllowUpdate == "Y");
        }

        /// <summary>
        /// CODE & NAME
        /// </summary>
        public class defCategory
        {
            public defCategory(string _c = null, string _cname = null)
            {
                Category = _c;
                CategoryName = _cname;
            }
            public string Category { get; set; }
            public string CategoryName { get; set; }
        }
    }
}
