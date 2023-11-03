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
    public class Service_controlHistoryCrop
    {
        /// <summary>
        /// 讀取清單
        /// </summary>
        /// <returns></returns>
        public static List<controlHistoryCrop> GetList(Expression<Func<controlHistoryCrop, bool>> filter = null)
        {
            return RepositoryUtility.GetList<controlHistoryCrop>(filter);
        }

        /// <summary>
        /// 讀取單筆
        /// </summary>
        /// <returns></returns>
        public static controlHistoryCrop GetDetail(int id)
        {
            return RepositoryUtility.GetDetail<controlHistoryCrop>(x => x.ID == id);
        }

        /// <summary>
        /// 寫入單筆資料
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public static BaseResult Insert(controlHistoryCrop data)
        {
            data.CreatedAt = DateTime.Now;
            data.UpdatedAt = DateTime.Now;

            return RepositoryUtility.Insert(data);
        }

        /// <summary>
        /// 更新
        /// </summary>
        /// <returns></returns>
        public static BaseResult Update(controlHistoryCrop data)
        {
            string sErrMsg = "";

            BaseRepository Base = new BaseRepository();
            //查詢資料
            var model = Base.QueryData<controlHistoryCrop>(ref sErrMsg, x => x.ID == data.ID).SingleOrDefault();
            if (model == null)
            {
                sErrMsg = "查無欲更新的資料";
                return new BaseResult(false, sErrMsg);
            }
            model.Name = data.Name;
            model.Sort = data.Sort;
            model.Show = data.Show;
            model.Type = data.Type;
            model.DayCount = data.DayCount;
            model.Comment = data.Comment;
            model.Context = data.Context;
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
            return RepositoryUtility.Delete<controlHistoryCrop>(x => x.ID == ID);
        }
        /// <summary>
        /// 多筆刪除
        /// </summary>
        /// <param name="filter"></param>
        /// <returns></returns>
        public static bool DeleteMany(Expression<Func<controlHistoryCrop, bool>> filter)
        {
            return RepositoryUtility.DeleteAll<controlHistoryCrop>(filter);
        }
    }
}
