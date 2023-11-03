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
    /// <summary>
    /// 作物
    /// </summary>
    public class Service_sysNews
    {
        /// <summary>
        /// 讀取清單
        /// </summary>
        /// <returns></returns>
        public static List<sysNews> GetList(Expression<Func<sysNews, bool>> filter = null)
        {
            return RepositoryUtility.GetList<sysNews>(filter);
        }
        
        /// <summary>
        /// 讀取單筆
        /// </summary>
        /// <returns></returns>
        public static sysNews GetDetail(int id)
        {
            return RepositoryUtility.GetDetail<sysNews>(x => x.Id == id);
        }

        /// <summary>
        /// 寫入單筆資料
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public static BaseResult Insert(sysNews data)
        {
            data.CreateDate = DateTime.Now;
            data.ModifyDate = DateTime.Now;
            return RepositoryUtility.Insert(data);
        }

        /// <summary>
        /// 更新
        /// </summary>
        /// <returns></returns>
        public static BaseResult Update(sysNews data)
        {
            string sErrMsg = "";

            BaseRepository Base = new BaseRepository();
            //查詢資料
            var model = Base.QueryData<sysNews>(ref sErrMsg, x => x.Id == data.Id).SingleOrDefault();
            if (model == null)
            {
                sErrMsg = "查無欲更新的資料";
                return new BaseResult(false, sErrMsg);
            }
            model.Title = data.Title;
            model.StartDate = data.StartDate;
            model.EndDate = data.EndDate;
            model.IsTop = data.IsTop;
            model.Contents = data.Contents;
            model.ModifyDate = DateTime.Now;
            model.ModifyUser = data.ModifyUser;

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
            return RepositoryUtility.Delete<sysNews>(x => x.Id == ID);
        }
    }
}
