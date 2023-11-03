using ICCModule.Entity.Tables;
using ICCModule.Entity.Tables.Platform;
using ICCModule.Repository;
using ICCModule.Repository.Class;
using ICCModule.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace ICCModule.EntityService.Service
{
    /// <summary>
    /// 監測專案調查對象
    /// </summary>
    public class Service_monitorProjectArea
    {
        /// <summary>
        /// 讀取清單
        /// </summary>
        /// <returns></returns>
        public static List<monitorProjectArea> GetList(Expression<Func<monitorProjectArea, bool>> filter = null)
        {
            return RepositoryUtility.GetList<monitorProjectArea>(filter);
        }

        /// <summary>
        /// 讀取單筆
        /// </summary>
        /// <returns></returns>
        public static monitorProjectArea GetDetail(int id)
        {
            return RepositoryUtility.GetDetail<monitorProjectArea>(x => x.ID == id);
        }

        /// <summary>
        /// 寫入單筆資料
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public static BaseResult Insert(monitorProjectArea data)
        {
            data.CreatedAt = DateTime.Now;
            data.UpdatedAt = DateTime.Now;
            return RepositoryUtility.Insert(data);
        }

        /// <summary>
        /// 更新
        /// </summary>
        /// <returns></returns>
        public static BaseResult Update(monitorProjectArea data)
        {
            string sErrMsg = "";

            BaseRepository Base = new BaseRepository();
            //查詢資料
            var model = Base.QueryData<monitorProjectArea>(ref sErrMsg, x => x.ID == data.ID).SingleOrDefault();
            if (model == null)
            {
                sErrMsg = "查無欲更新的資料";
                return new BaseResult(false, sErrMsg);
            }
            
            //更新內容

            model.UpdatedAt = DateTime.Now;

            //更新回資料庫
            bool rslt = Base.Update(ref sErrMsg);
            return new BaseResult(rslt, sErrMsg);
        }

        /// <summary>
        /// 寫入單筆資料
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public static BaseResult InsertOrUpdate(monitorProjectArea data)
        {
            string sErrMsg = "";
            BaseRepository Base = new BaseRepository();
            //查詢資料
            var model = Base.QueryData<monitorProjectArea>(ref sErrMsg, x => x.ID == data.ID).SingleOrDefault();
            if (model == null)
            {
                data.CreatedAt = DateTime.Now;
                data.UpdatedAt = DateTime.Now;
                return RepositoryUtility.Insert(data);
            }
            bool rslt = true;
            //更新內容
            if (model.Type != data.Type || model.Distist != data.Distist || model.Inspectors != data.Inspectors || model.Points != data.Points)
            {
                model.Type = data.Type;
                model.Distist = data.Distist;
                model.Inspectors = data.Inspectors;
                model.Points = data.Points;
                model.UpdatedAt = DateTime.Now;

                //更新回資料庫
                rslt = Base.Update(ref sErrMsg);
            }
           
            return new BaseResult(rslt, sErrMsg);
        }

        /// <summary>
        /// 刪除
        /// </summary>
        /// <param name="LoginID"></param>
        /// <returns></returns>
        public static BaseResult Delete(int ID)
        {
            return RepositoryUtility.Delete<monitorProjectArea>(x => x.ID == ID);
        }
        /// <summary>
        /// 多筆刪除
        /// </summary>
        /// <param name="filter"></param>
        /// <returns></returns>
        public static bool DeleteMany(Expression<Func<monitorProjectArea, bool>> filter)
        {
            return RepositoryUtility.DeleteAll<monitorProjectArea>(filter);
        }
    }
}
