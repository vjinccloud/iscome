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
    public class Service_crops
    {
        /// <summary>
        /// 讀取清單
        /// </summary>
        /// <returns></returns>
        public static List<crops> GetList(Expression<Func<crops, bool>> filter = null)
        {
            return RepositoryUtility.GetList<crops>(filter);
        }
        
        /// <summary>
        /// 讀取清單
        /// </summary>
        /// <returns></returns>
        public static List<crops> GetEnableList()
        {
            var cType = RepositoryUtility.GetList<cropType>(x => x.Enable).Select(x => x.ID);
            return RepositoryUtility.GetList<crops>(x => x.Enable && cType.Contains(x.TypeID));
        }

        /// <summary>
        /// 讀取單筆
        /// </summary>
        /// <returns></returns>
        public static crops GetDetail(int id)
        {
            return RepositoryUtility.GetDetail<crops>(x => x.ID == id);
        }

        /// <summary>
        /// 寫入單筆資料
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public static BaseResult Insert(crops data)
        {
            data.CreatedAt = DateTime.Now;
            data.UpdatedAt = DateTime.Now;
            return RepositoryUtility.Insert(data);
        }

        /// <summary>
        /// 更新
        /// </summary>
        /// <returns></returns>
        public static BaseResult Update(crops data)
        {
            string sErrMsg = "";

            BaseRepository Base = new BaseRepository();
            //查詢資料
            var model = Base.QueryData<crops>(ref sErrMsg, x => x.ID == data.ID).SingleOrDefault();
            if (model == null)
            {
                sErrMsg = "查無欲更新的資料";
                return new BaseResult(false, sErrMsg);
            }
            model.Name = data.Name;
            model.Sort = data.Sort;
            model.Enable = data.Enable;
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
            return RepositoryUtility.Delete<crops>(x => x.ID == ID);
        }
    }
}
