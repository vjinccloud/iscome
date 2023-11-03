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
    /// 作物
    /// </summary>
    public class Service_pesticideAudit
    {
        /// <summary>
        /// 讀取清單
        /// </summary>
        /// <returns></returns>
        public static List<pesticideAudit> GetList(Expression<Func<pesticideAudit, bool>> filter = null)
        {
            return RepositoryUtility.GetList<pesticideAudit>(filter);
        }

        /// <summary>
        /// 讀取單筆
        /// </summary>
        /// <returns></returns>
        public static pesticideAudit GetDetail(int id)
        {
            return RepositoryUtility.GetDetail<pesticideAudit>(x => x.ID == id);
        }

        /// <summary>
        /// 寫入單筆資料
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public static BaseResult Insert(pesticideAudit data)
        {
            data.CreatedAt = DateTime.Now;
            data.UpdatedAt = DateTime.Now;
            return RepositoryUtility.Insert(data);
        }

        /// <summary>
        /// 更新
        /// </summary>
        /// <returns></returns>
        public static BaseResult Update(pesticideAudit data)
        {
            string sErrMsg = "";

            BaseRepository Base = new BaseRepository();
            //查詢資料
            var model = Base.QueryData<pesticideAudit>(ref sErrMsg, x => x.ID == data.ID).SingleOrDefault();
            if (model == null)
            {
                sErrMsg = "查無欲更新的資料";
                return new BaseResult(false, sErrMsg);
            }
            model.Date = data.Date;
            model.Result = data.Result;
            model.Instructions = data.Instructions;
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
            return RepositoryUtility.Delete<pesticideAudit>(x => x.ID == ID);
        }
        /// <summary>
        /// 多筆刪除
        /// </summary>
        /// <param name="filter"></param>
        /// <returns></returns>
        public static bool DeleteMany(Expression<Func<pesticideAudit, bool>> filter)
        {
            return RepositoryUtility.DeleteAll<pesticideAudit>(filter);
        }
    }
}
