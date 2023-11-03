using ICCModule.Entity.Tables.Platform;
using ICCModule.Repository;
using ICCModule.Repository.Class;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Linq.Mapping;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace ICCModule.EntityService.Service
{
    public class Service_doctor_DutyRecord
    {
        /// <summary>
        /// 抓取單筆資料
        /// </summary>
        /// <param name="ID"></param>
        /// <returns></returns>
        public static doctorDutyRecord GetDetail(string ID)
        {
            return RepositoryUtility.GetDetail<doctorDutyRecord>(x => x.ID == Convert.ToInt64(ID));

            //StringBuilder SQL_str = new StringBuilder();

            //SQL_str.Append("SELECT * FROM doctor_DutyRecords ");
            //SQL_str.Append("WHERE ");
            //SQL_str.Append($"ID = {Convert.ToInt64(ID)}");
            //SQL_str.Append(";");
            //string msg = String.Empty;
            //DataSet dataset = RepositoryUtility.EXEC_SQL_toDataSet(ref msg, SQL_str.ToString());

            //doctorDutyRecord model = new doctorDutyRecord();
            //PropertyInfo[] propertyInfos;
            //propertyInfos = typeof(doctorDutyRecord).GetProperties()
            //                .Where(p =>
            //                {
            //                    var attrib = (ColumnAttribute)p
            //                   .GetCustomAttributes(typeof(ColumnAttribute), false)
            //                   .SingleOrDefault();
            //                    return attrib != null;
            //                }).ToArray();

            //foreach (var p in propertyInfos)
            //{
            //    var value = dataset.Tables[0].Rows[0][p.Name];
            //    if (value == System.DBNull.Value)
            //    {
            //        if (p.PropertyType.Name == "String")
            //        {
            //            value = String.Empty;
            //        }
            //        else if (p.PropertyType.Name.Contains("Nullable"))
            //        {
            //            value = null;
            //        }
            //    }

            //    p.SetValue(model, value);
            //}

            //return model;
        }

        /// <summary>
        /// 抓取單筆資料
        /// </summary>
        /// <param name="ID"></param>
        /// <returns></returns>
        public static bool CheckExist(string CreateUserLoginID, DateTime Date, string Time)
        {
            Expression<Func<doctorDutyRecord, bool>> filter = (x => true);
            filter = filter.And(x => x.CreateUserLoginID == CreateUserLoginID && x.Date == Date && x.Time == Time);

            return RepositoryUtility.CheckExist<doctorDutyRecord>(filter);
        }

        /// <summary>
        /// 抓取單筆資料
        /// </summary>
        /// <param name="ID"></param>
        /// <returns></returns>
        public static bool CheckExist(Expression<Func<doctorDutyRecord, bool>> filter = null)
        {
            return RepositoryUtility.CheckExist<doctorDutyRecord>(filter);
        }

        /// <summary>
        /// 取回指定日期與時段之排班列表
        /// </summary>
        /// <param name="DesignDate"></param>
        /// <param name="Period"></param>
        /// <returns></returns>
        public static List<doctorDutyRecord> GetDesignDatePeriodList(DateTime DesignDate, string Period)
        {
            Expression<Func<doctorDutyRecord, bool>> filter = (x => true);
            filter = filter.And(x => x.Date == DesignDate && x.Period == Period);
            return RepositoryUtility.GetList<doctorDutyRecord>(filter);
        }

        /// <summary>
        /// 讀取清單
        /// </summary>
        /// <param name="StartDate"></param>
        /// <param name="EndDate"></param>
        /// <param name="IsDel"></param>
        /// <returns></returns>
        public static List<doctorDutyRecord> GetList(DateTime StartDate, DateTime EndDate, bool IsDel = false)
        {
            StringBuilder SQL_str = new StringBuilder();

            SQL_str.Append("SELECT * FROM doctor_DutyRecords ");
            SQL_str.Append("WHERE ");
            SQL_str.Append($"Date >= '{StartDate.ToString("yyyy-MM-dd HH:mm:ss")}'");
            SQL_str.Append(" AND ");
            SQL_str.Append($"Date <= '{EndDate.ToString("yyyy-MM-dd HH:mm:ss")}'");
            SQL_str.Append(" AND ");
            SQL_str.Append($" IsDel = {Convert.ToInt32(IsDel)}");
            SQL_str.Append(";");
            string msg = String.Empty;
            return RepositoryUtility.EXEC_SQL_toList<doctorDutyRecord>(ref msg, SQL_str.ToString());
        }

        /// <summary>
        /// 讀取清單
        /// </summary>
        /// <param name="StartDate"></param>
        /// <param name="EndDate"></param>
        /// <param name="IsDel"></param>
        /// <returns></returns>
        public static List<doctorDutyRecord> GetList(Expression<Func<doctorDutyRecord, bool>> filter = null)
        {
            return RepositoryUtility.GetList<doctorDutyRecord>(filter);
        }

        /// <summary>
        /// 寫入單筆資料
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public static BaseResult Insert(doctorDutyRecord data)
        {
            data.CreatedAt = DateTime.Now;

            return RepositoryUtility.Insert(data);
        }

        /// <summary>
        /// 更新
        /// </summary>
        /// <returns></returns>
        public static BaseResult Update(doctorDutyRecord data)
        {
            string sErrMsg = "";

            BaseRepository Base = new BaseRepository();
            //查詢資料
            var model = Base.QueryData<doctorDutyRecord>(ref sErrMsg, x => x.ID == data.ID).SingleOrDefault();
            if (model == null)
            {
                sErrMsg = "查無欲更新的資料";
                return new BaseResult(false, sErrMsg);
            }

            PropertyInfo[] propertyInfos;
            propertyInfos = typeof(doctorDutyRecord).GetProperties()
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
                        break;
                    case "UpdatedAt":
                        p.SetValue(model, DateTime.Now);
                        break;
                    default:
                        p.SetValue(model, p.GetValue(data));
                        break;
                }
            }

            //更新回資料庫
            bool rslt = Base.Update(ref sErrMsg);
            return new BaseResult(rslt, sErrMsg);
        }

        /// <summary>
        /// 刪除
        /// </summary>
        /// <param name="ID"></param>
        /// <returns></returns>
        public static BaseResult Delete(long ID)
        {
            BaseResult result = new BaseResult();

            try
            {
                result = RepositoryUtility.Delete<doctorDutyRecord>(x => x.ID == ID);
            }
            catch (Exception e)
            {
                result.result = false;
                result.Msg = e.Message;
            }

            return result;
        }

        public static IEnumerable<doctorDutyRecord> GetDoctorDutyList(string doctorId)
        {
            BaseRepository Base = new BaseRepository();
            var list = Base.QueryData<doctorDutyRecord>(x => x.DoctorId == doctorId).Select(a => a);
            return list;
        }

        public static doctorDutyRecord GetDetailByDatetime(string docId, string date, string time)
        {
            BaseRepository Base = new BaseRepository();
            DateTime dt = Convert.ToDateTime(date);

            //查詢資料
            var model = Base.QueryData<doctorDutyRecord>(x => x.DoctorId == docId && x.Date == dt && x.Time == time).SingleOrDefault();

            return model;
        }
    }
}
