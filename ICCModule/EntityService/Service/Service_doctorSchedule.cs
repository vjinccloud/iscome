using ICCModule.Entity.Tables.Platform;
using ICCModule.Repository;
using ICCModule.Repository.Class;
using System;
using System.Collections.Generic;
using System.Data.Linq.Mapping;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace ICCModule.EntityService.Service
{
    /// <summary>
    /// 植醫排程
    /// </summary>
    public class Service_doctorSchedule
    {
        /// <summary>
        /// 抓取單筆資料
        /// </summary>
        /// <param name="ID"></param>
        /// <returns></returns>
        public static doctorSchedule GetDetail(long ID)
        {
            return RepositoryUtility.GetDetail<doctorSchedule>(x => x.ID == ID);
        }

        /// <summary>
        /// 讀取清單
        /// </summary>
        /// <returns></returns>
        public static List<doctorSchedule> GetList(Expression<Func<doctorSchedule, bool>> filter = null)
        {
            return RepositoryUtility.GetList<doctorSchedule>(filter);
        }

        /// <summary>
        /// 讀取清單
        /// </summary>
        /// <param name="QueryDateType"></param>
        /// <param name="StartDate"></param>
        /// <param name="EndDate"></param>
        /// <returns></returns>
        public static List<doctorSchedule> GetList(string QueryDateType, DateTime? StartDate, DateTime? EndDate)
        {
            StringBuilder SQL_str = new StringBuilder();

            SQL_str.Append("SELECT * FROM doctor_schedule ");
            bool hasWhere = false;
            // 查詢日期
            if (!String.IsNullOrEmpty(QueryDateType))
            {
                if (!hasWhere)
                {
                    SQL_str.Append("WHERE ");
                }

                switch (QueryDateType)
                {
                    // 建立日期
                    case "CreatedAt":
                    // 預約看診日期
                    case "ReserveDatetime":
                    // 實際看診日期
                    case "DateDiagnosis":
                        if (StartDate != null)
                        {
                            SQL_str.Append($"{QueryDateType} >= '{String.Format("{0: yyyy-MM-dd 00:00:00}", StartDate)}'");
                        }
                        if (EndDate != null)
                        {
                            if (StartDate != null)
                            {
                                SQL_str.Append(" AND ");
                            }

                            SQL_str.Append($"{QueryDateType} <= '{String.Format("{0: yyyy-MM-dd 23:59:59}", EndDate)}'");
                        }
                        break;
                }
            }

            SQL_str.Append(";");
            string msg = String.Empty;
            return RepositoryUtility.EXEC_SQL_toList<doctorSchedule>(ref msg, SQL_str.ToString());
        }

        /// <summary>
        /// 寫入單筆資料
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public static BaseResult Insert(doctorSchedule data)
        {
            var firstDayOfMonth = new DateTime(data.ReserveDatetime.Year, data.ReserveDatetime.Month, 1, 0, 0, 0);
            var lastDayOfMonth = firstDayOfMonth.AddMonths(1).AddDays(-1).AddHours(23).AddMinutes(59).AddSeconds(59);
            List<doctorSchedule> list = GetList("CreatedAt", firstDayOfMonth, lastDayOfMonth);
            int counts = list == null ? 0 : list.Count;
            if (string.IsNullOrEmpty(data.LoginID) || data.LoginID == "null") data.LoginID = null;
            data.CreatedAt = DateTime.Now;
            data.MonthIndex = counts + 1;

            return RepositoryUtility.Insert(data);
        }

        /// <summary>
        /// 更新
        /// </summary>
        /// <returns></returns>
        public static BaseResult Update(doctorSchedule data)
        {
            string sErrMsg = "";

            BaseRepository Base = new BaseRepository();
            //查詢資料
            var model = Base.QueryData<doctorSchedule>(ref sErrMsg, x => x.ID == data.ID).SingleOrDefault();
            if (model == null)
            {
                sErrMsg = "查無欲更新的資料";
                return new BaseResult(false, sErrMsg);
            }
            if (string.IsNullOrEmpty(data.CropSymptoms) && !string.IsNullOrEmpty(model.CropSymptoms)) data.CropSymptoms = model.CropSymptoms;
            if (string.IsNullOrEmpty(data.RecentlyFertilizer) && !string.IsNullOrEmpty(model.RecentlyFertilizer)) data.RecentlyFertilizer = model.RecentlyFertilizer;

            model.UpdatedAt = DateTime.Now;
            model.Status = data.Status;
            model.Updater = data.Updater;

            PropertyInfo[] propertyInfos;
            propertyInfos = typeof(doctorSchedule).GetProperties()
                            .Where(p =>
                            {
                                var attrib = (ColumnAttribute)p
                               .GetCustomAttributes(typeof(ColumnAttribute), false)
                               .SingleOrDefault();
                                return attrib != null;
                            }).ToArray();

            foreach (var p in propertyInfos)
            {
                switch (p.Name)
                {
                    case "ID":
                    case "CreatedAt":
                    case "IsDel":
                    case "Creator":
                    case "MonthIndex":
                    case "MemberInfoID":
                    case "IsTransCase":
                    case "TransDocId":
                    case "TransDistrict":
                    case "TransReason":
                    case "TransRemark":
                    case "TransDate":
                    case "TransUser":
                    case "AcceptCaseDate":
                    case "AcceptCaseUser":
                        break;
                    case "UpdatedAt":
                        p.SetValue(model, DateTime.Now);
                        break;
                    default:
                        p.SetValue(model, p.GetValue(data));
                        break;
                }
            }
            if (string.IsNullOrEmpty(data.LoginID) || data.LoginID == "null") model.LoginID = null;

            //更新回資料庫
            bool rslt = Base.Update(ref sErrMsg);
            return new BaseResult(rslt, sErrMsg);
        }

        /// <summary>
        /// 更新
        /// </summary>
        /// <returns></returns>
        public static BaseResult UpdateTrans(doctorSchedule data, string ActionName)
        {
            string sErrMsg = "";

            BaseRepository Base = new BaseRepository();
            //查詢資料
            var model = Base.QueryData<doctorSchedule>(ref sErrMsg, x => x.ID == data.ID).SingleOrDefault();
            if (model == null)
            {
                sErrMsg = "查無欲更新的資料";
                return new BaseResult(false, sErrMsg);
            }

            model.UpdatedAt = DateTime.Now;
            model.Updater = data.Updater;

            if (ActionName == "Receive")
            {
                model.LoginID = data.LoginID;
                model.OrgDistrict = data.OrgDistrict;
                model.IsTransCase = data.IsTransCase;
                model.TransDocId = data.TransDocId;
                model.TransDistrict = data.TransDistrict;
                model.AcceptCaseDate = data.AcceptCaseDate;
                model.AcceptCaseUser = data.AcceptCaseUser;
            }
            else if (ActionName == "Trans")
            {
                model.IsTransCase = data.IsTransCase;
                model.TransDocId = data.TransDocId;
                model.TransDistrict = data.TransDistrict;
                model.TransReason = data.TransReason;
                model.TransRemark = data.TransRemark;
                model.TransDate = data.TransDate;
                model.TransUser = data.TransUser;
            }

            //更新回資料庫
            bool rslt = Base.Update(ref sErrMsg);
            return new BaseResult(rslt, sErrMsg);
        }

        /// <summary>
        /// 刪除
        /// </summary>
        /// <param name="LoginID"></param>
        /// <returns></returns>
        public static BaseResult Delete(long ID)
        {
            return RepositoryUtility.Delete<doctorSchedule>(x => x.ID == ID);
        }
        /// <summary>
        /// line 電子處方箋
        /// </summary>
        public static IQueryable<doctorSchedule> GetLineReserve(string userId)
        {
            BaseRepository Base = new BaseRepository();
            var mem = Base.QueryData<memberInfo>(x => x.LineUserId == userId).SingleOrDefault();
           if(mem == null)
            {
                return null;
            }

            IQueryable<doctorSchedule> list = Base.QueryData<doctorSchedule>(x => x.MemberInfoID == mem.ID && x.IsDel == false).OrderByDescending(a=>a.CreatedAt)  ;
            if(list.Count() > 0)
            {
                return list;
            }
            else
            {
                return null;
            }
            
        }
        public static void CancelReserve(int id)
        {
            string sErrMsg = "";
            BaseRepository Base = new BaseRepository();
            doctorSchedule model = Base.QueryData<doctorSchedule>(x => x.ID == id).SingleOrDefault();
            //model.IsDel = true;
            model.Status = "UserCancel";

            //更新回資料庫
            bool rslt = Base.Update(ref sErrMsg);

            Base = new BaseRepository();
            doctorDutyRecord model2 = Base.QueryData<doctorDutyRecord>(x => x.DoctorScheduleID == id).SingleOrDefault();
            model2.DoctorScheduleID = 0;

            //更新回資料庫
            rslt = Base.Update(ref sErrMsg);


            //return new BaseResult(rslt, sErrMsg);
        }
    }
}
