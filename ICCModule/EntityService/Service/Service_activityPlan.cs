using ICCModule.Entity.Tables.Platform;
using ICCModule.Repository;
using ICCModule.Repository.Class;
using IscomG2C.Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace ICCModule.EntityService.Service
{
    public class Service_activityPlan
    {
        /// <summary>
        /// 抓取單筆資料
        /// </summary>
        /// <param name="ID"></param>
        /// <returns></returns>
        public static activityPlan GetDetail(int ID)
        {
            return RepositoryUtility.GetDetail<activityPlan>(x => x.ID == ID);
        }

        /// <summary>讀取清單</summary>
        /// <param name="Code"></param>
        /// <param name="Show">上、下架</param>
        /// <returns>
        ///   <br />
        /// </returns>
        public static List<activityPlan> GetList(string StartDate, string EndDate, string Type, bool? Show = null)
        {
            Expression<Func<activityPlan, bool>> filter = (x => true);
            if (!string.IsNullOrEmpty(StartDate))
            {
                filter = filter.And(x => x.StartDate.Date >= Convert.ToDateTime(String.Concat(StartDate, " 00:00:00")));
            }
            if (!string.IsNullOrEmpty(EndDate))
            {
                filter = filter.And(x => x.StartDate.Date <= Convert.ToDateTime(String.Concat(EndDate, " 00:00:00")));
            }
            if (!string.IsNullOrEmpty(Type))
            {
                filter = filter.And(x => x.Type == Type);
            }
            if (Show.HasValue)
            {
                filter = filter.And(x => x.Show == Show);
            }

            return RepositoryUtility.GetList<activityPlan>(filter);
        }

        /// <summary>讀取清單</summary>
        /// <param name="Code"></param>
        /// <param name="Show">上、下架</param>
        /// <returns>
        ///   <br />
        /// </returns>
        public static List<activityPlan> GetList(Expression<Func<activityPlan, bool>> filter = null)
        {
            return RepositoryUtility.GetList<activityPlan>(filter);
        }

        /// <summary>
        /// 寫入單筆資料
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public static BaseResult Insert(activityPlan data)
        {
            return RepositoryUtility.Insert<activityPlan>(data);
        }

        /// <summary>
        /// 更新單筆資料
        /// </summary>
        public static BaseResult Update(activityPlan data)
        {
            string sErrMsg = "";

            BaseRepository Base = new BaseRepository();
            //查詢資料
            var model = Base
                .QueryData<activityPlan>(ref sErrMsg, x => x.ID == data.ID)
                .SingleOrDefault();
            if (model == null)
            {
                sErrMsg = "查無欲更新的資料";
                return new BaseResult(false, sErrMsg);
            }
            model.Type = data.Type;
            model.Show = data.Show;
            model.StartDateStr = data.StartDateStr;
            model.EndDateStr = data.EndDateStr;
            model.Name = data.Name;
            model.Address = data.Address;
            model.Undertaker = data.Undertaker;
            model.Phone = data.Phone;
            model.Open = data.Open;
            model.ClassHours = data.ClassHours;
            model.RegistrationStartDateStr = data.RegistrationStartDateStr;
            model.RegistrationEndDateStr = data.RegistrationEndDateStr;
            model.Fields = data.Fields;
            model.Context = data.Context;
            model.UpdatedAt = DateTime.Now;

            // 保存場次



            //更新回資料庫
            bool rslt = Base.Update(ref sErrMsg);
            return new BaseResult(rslt, sErrMsg);
        }

        /// <summary>
        /// 刪除
        /// </summary>
        public static BaseResult Delete(int ID)
        {
            List<FileManagement> files = Service_FileManagement.GetList("activity_Plans", ID.ToString());
            var fileLogic = new FileLogic();
            // 所有相關聯的檔案進行刪除
            foreach (var file in files)
            {
                // 需確認檔案是否存在
                if (fileLogic.IsFileExisted(file.FilePath))
                {
                    fileLogic.DeleteFile(file.FilePath);
                }
            }

            return RepositoryUtility.Delete<activityPlan>(x => x.ID == ID);
        }
    }
}
