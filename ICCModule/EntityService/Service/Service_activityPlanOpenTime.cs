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
    public class Service_activityPlanOpenTIme
    {
        /// <summary>
        /// 抓取單筆資料
        /// </summary>
        /// <param name="ID"></param>
        /// <returns></returns>
        public static activityPlanOpenTime GetDetail(int ID)
        {
            return RepositoryUtility.GetDetail<activityPlanOpenTime>(x => x.ID == ID);
        }

        /// <summary>讀取清單</summary>
        /// <param name="ActivityPlanID">對應活動ID</param>
        /// <returns>
        ///   <br />
        /// </returns>
        public static List<activityPlanOpenTime> GetList(long ActivityPlanID)
        {
            Expression<Func<activityPlanOpenTime, bool>> filter = (x => true);
            if (ActivityPlanID != 0)
            {
                filter = filter.And(x => x.ActivityPlanID == ActivityPlanID);
            }

            return RepositoryUtility.GetList<activityPlanOpenTime>(filter);
        }

        /// <summary>
        /// 讀取清單
        /// </summary>
        /// <param name="ActivityPlanID"></param>
        /// <returns></returns>
        public static List<activityPlanOpenTime> GetList(Expression<Func<activityPlanOpenTime, bool>> filter = null)
        {
            return RepositoryUtility.GetList<activityPlanOpenTime>(filter);
        }

        /// <summary>
        /// 寫入單筆資料
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public static BaseResult Insert(activityPlanOpenTime data)
        {
            return RepositoryUtility.Insert<activityPlanOpenTime>(data);
        }

        /// <summary>
        /// 更新單筆資料
        /// </summary>
        public static BaseResult Update(activityPlanOpenTime data)
        {
            string sErrMsg = "";

            BaseRepository Base = new BaseRepository();
            //查詢資料
            var model = Base
                .QueryData<activityPlanOpenTime>(ref sErrMsg, x => x.ID == data.ID)
                .SingleOrDefault();
            if (model == null)
            {
                sErrMsg = "查無欲更新的資料";
                return new BaseResult(false, sErrMsg);
            }
            model.Name = data.Name;
            model.DateStr = data.DateStr;
            model.TimeBetween = data.TimeBetween;
            model.Nums = data.Nums;
            // 編輯成果的出席人數
            if (data.Attendance != 0)
            {
                model.Attendance = data.Attendance;
            }
            model.SignInResult = data.SignInResult;
            model.Pictures = data.Pictures;
            
            model.UpdatedAt = DateTime.Now;
            

            //更新回資料庫
            bool rslt = Base.Update(ref sErrMsg);
            return new BaseResult(rslt, sErrMsg);
        }

        /// <summary>
        /// 刪除
        /// </summary>
        public static BaseResult Delete(int ID)
        {
            List<FileManagement> files = Service_FileManagement.GetList("activity_Plan_Open_Times", ID.ToString());
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

            return RepositoryUtility.Delete<activityPlanOpenTime>(x => x.ID == ID);
        }
    }
}
