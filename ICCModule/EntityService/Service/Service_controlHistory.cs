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
    public class Service_controlHistory
    {
        /// <summary>
        /// 讀取清單
        /// </summary>
        /// <returns></returns>
        public static List<controlHistory> GetList(Expression<Func<controlHistory, bool>> filter = null)
        {
            return RepositoryUtility.GetList<controlHistory>(filter);
        }

        /// <summary>
        /// 讀取單筆
        /// </summary>
        /// <returns></returns>
        public static controlHistory GetDetail(int id)
        {
            return RepositoryUtility.GetDetail<controlHistory>(x => x.ID == id);
        }

        /// <summary>
        /// 寫入單筆資料
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public static BaseResult Insert(controlHistory data)
        {
            data.CreatedAt = DateTime.Now;
            data.UpdatedAt = DateTime.Now;

            return RepositoryUtility.Insert(data);
        }

        /// <summary>
        /// 更新
        /// </summary>
        /// <returns></returns>
        public static BaseResult Update(controlHistory data)
        {
            string sErrMsg = "";

            BaseRepository Base = new BaseRepository();
            //查詢資料
            var model = Base.QueryData<controlHistory>(ref sErrMsg, x => x.ID == data.ID).SingleOrDefault();
            if (model == null)
            {
                sErrMsg = "查無欲更新的資料";
                return new BaseResult(false, sErrMsg);
            }
            model.Name = data.Name;
            model.Type = data.Type;
            model.Enable = data.Enable;
            model.Information = data.Information;
            model.Explanation = data.Explanation;
            model.UpdatedAt = DateTime.Now;

            //更新回資料庫
            bool rslt = Base.Update(ref sErrMsg);
            return new BaseResult(rslt, sErrMsg);
        }

        /// <summary>
        /// 更新
        /// </summary>
        /// <returns></returns>
        public static controlHistory GetListAndAddclick(int id, bool updCount)
        {
            if (updCount)
            {
                string sErrMsg = "";
                BaseRepository Base = new BaseRepository();
                //查詢資料
                var model = Base.QueryData<controlHistory>(ref sErrMsg, x => x.ID == id).SingleOrDefault();
                if (model == null)
                {
                    sErrMsg = "查無欲更新的資料";
                }

                model.ClickCount = model.ClickCount + 1;

                //更新回資料庫
                bool rslt = Base.Update(ref sErrMsg);
            }

            return RepositoryUtility.GetDetail<controlHistory>(x => x.ID == id);
        }

        /// <summary>
        /// 刪除
        /// </summary>
        /// <param name="LoginID"></param>
        /// <returns></returns>
        public static BaseResult Delete(int ID)
        {
            return RepositoryUtility.Delete<controlHistory>(x => x.ID == ID);
        }
    }
}
