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
    public class Service_activityRegistration
    {
        /// <summary>
        /// 抓取單筆資料
        /// </summary>
        /// <param name="ID"></param>
        /// <returns></returns>
        public static activityRegistraction GetDetail(int ID)
        {
            return RepositoryUtility.GetDetail<activityRegistraction>(x => x.ID == ID);
        }

        /// <summary>讀取清單</summary>
        /// <param name="Code"></param>
        /// <param name="Show">上、下架</param>
        /// <returns>
        ///   <br />
        /// </returns>
        public static List<activityRegistraction> GetList(long PlanOpenTimeID)
        {
            Expression<Func<activityRegistraction, bool>> filter = (x => true);
            if (PlanOpenTimeID != 0)
            {
                filter = filter.And(x => x.PlanOpenTimeID == PlanOpenTimeID);
            }

            return RepositoryUtility.GetList<activityRegistraction>(filter);
        }
        /// <summary>
        /// 讀取清單
        /// </summary>
        /// <param name="PlanOpenTimeID"></param>
        /// <returns></returns>
        public static List<activityRegistraction> GetList(Expression<Func<activityRegistraction, bool>> filter = null)
        {
            return RepositoryUtility.GetList<activityRegistraction>(filter);
        }

        public static bool CheckAllRegist(long PlanID)
        {
            var _res = false;
            var planData = RepositoryUtility.GetList<activityPlanOpenTime>(x => x.ActivityPlanID == PlanID);
            _res = planData.Any(x => x.Nums == 0);
            if (planData.Any() && planData.All(x => x.Nums != 0))
            {
                var ids = planData.Select(x => x.ID).ToList();
                var registData = RepositoryUtility.GetList<activityRegistraction>(x => ids.Contains(x.ID));
                _res = planData.Any(x => x.Nums > registData.Count(o => o.PlanOpenTimeID == x.ID));
            }

            return _res;
        }

        public static BaseResult CheckRegist(int ID)
        {
            string sErrMsg = "";

            BaseRepository Base = new BaseRepository();
            //查詢資料
            var model = Base.QueryData<activityRegistraction>(ref sErrMsg, x => x.ID == ID).SingleOrDefault();
            if (model == null)
            {
                sErrMsg = "查無欲更新的資料";
                return new BaseResult(false, sErrMsg);
            }
            if(model.Status == 1) model.Status = 0;
            else model.Status = 1;

            //更新回資料庫
            bool rslt = Base.Update(ref sErrMsg);
            return new BaseResult(rslt, sErrMsg);
        }

        /// <summary>
        /// 寫入單筆資料
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public static BaseResult Insert(activityRegistraction data)
        {
            var result = RepositoryUtility.Insert<activityRegistraction>(data);
            if (result.result)
            {
                string sErrMsg = "";
                BaseRepository Base = new BaseRepository();
                var model = Base.QueryData<activityPlanOpenTime>(ref sErrMsg, x => x.ID == data.PlanOpenTimeID).SingleOrDefault();
                if (model != null)
                {
                    model.RegisteredNums = model.RegisteredNums + 1;
                    bool rslt = Base.Update(ref sErrMsg);
                }
            }
            return result;
        }

        /// <summary>
        /// 更新單筆資料
        /// </summary>
        public static BaseResult Update(activityRegistraction data)
        {
            string sErrMsg = "";

            BaseRepository Base = new BaseRepository();
            //查詢資料
            var model = Base
                .QueryData<activityRegistraction>(ref sErrMsg, x => x.ID == data.ID)
                .SingleOrDefault();
            if (model == null)
            {
                sErrMsg = "查無欲更新的資料";
                return new BaseResult(false, sErrMsg);
            }
            model.Name = data.Name;
            model.IdentifiedID = data.IdentifiedID;
            model.Phone = data.Phone;
            model.ClassHours = data.ClassHours;
            model.Status = data.Status;
            model.UpdatedAt = DateTime.Now;

            //更新回資料庫
            bool rslt = Base.Update(ref sErrMsg);
            return new BaseResult(rslt, sErrMsg);
        }

        /// <summary>
        /// 刪除
        /// </summary>
        public static BaseResult Delete(long ID)
        {
            List<FileManagement> files = Service_FileManagement.GetList("activity_Registrations", ID.ToString());
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

            return RepositoryUtility.Delete<activityRegistraction>(x => x.ID == ID);
        }
    }
}
