using ICCModule.Entity.Tables;
using ICCModule.Entity.Tables.Platform;
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
    public class Service_controlHistoryCropsContents
    {
        /// <summary>
        /// 讀取清單
        /// </summary>
        /// <returns></returns>
        public static List<controlHistoryCropsContents> GetList(Expression<Func<controlHistoryCropsContents, bool>> filter = null)
        {
            return RepositoryUtility.GetList<controlHistoryCropsContents>(filter);
        }

        /// <summary>
        /// 讀取單筆
        /// </summary>
        /// <returns></returns>
        public static controlHistoryCropsContents GetDetail(int id)
        {
            return RepositoryUtility.GetDetail<controlHistoryCropsContents>(x => x.Id == id);
        }

        /// <summary>
        /// 寫入單筆資料
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public static BaseResult Insert(controlHistoryCropsContents data)
        {
            data.CreatedAt = DateTime.Now;
            data.UpdatedAt = DateTime.Now;

            return RepositoryUtility.Insert(data);
        }

        /// <summary>
        /// 更新
        /// </summary>
        /// <returns></returns>
        public static BaseResult Update(controlHistoryCropsContents data)
        {
            string sErrMsg = "";

            BaseRepository Base = new BaseRepository();
            //查詢資料
            var model = Base.QueryData<controlHistoryCropsContents>(ref sErrMsg, x => x.Id == data.Id).SingleOrDefault();
            if (model == null)
            {
                sErrMsg = "查無欲更新的資料";
                return new BaseResult(false, sErrMsg);
            }
            model.ControlStage = data.ControlStage;
            model.Name = data.Name;
            model.ShowType = data.ShowType;
            model.StartBlock = data.StartBlock;
            model.EndBlock = data.EndBlock;
            model.UpdatedAt = DateTime.Now;

            //更新回資料庫
            bool rslt = Base.Update(ref sErrMsg);
            return new BaseResult(rslt, sErrMsg);
        }

        /// <summary>
        /// 刪除
        /// </summary>
        /// <param name="LoginID"></param>
        /// <returns></returns>
        public static BaseResult Delete(int ID)
        {
            return RepositoryUtility.Delete<controlHistoryCropsContents>(x => x.Id == ID);
        }
        /// <summary>
        /// 多筆刪除
        /// </summary>
        /// <param name="filter"></param>
        /// <returns></returns>
        public static bool DeleteMany(Expression<Func<controlHistoryCropsContents, bool>> filter)
        {
            return RepositoryUtility.DeleteAll<controlHistoryCropsContents>(filter);
        }
    }
}
