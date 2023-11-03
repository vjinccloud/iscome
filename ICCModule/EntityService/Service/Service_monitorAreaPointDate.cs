﻿using ICCModule.Entity.Tables;
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
    public class Service_monitorAreaPointDate
    {
        /// <summary>
        /// 讀取清單
        /// </summary>
        /// <returns></returns>
        public static List<monitorAreaPointDate> GetList(Expression<Func<monitorAreaPointDate, bool>> filter = null)
        {
            return RepositoryUtility.GetList<monitorAreaPointDate>(filter);
        }

        /// <summary>
        /// 讀取單筆
        /// </summary>
        /// <returns></returns>
        public static monitorAreaPointDate GetDetail(int id)
        {
            return RepositoryUtility.GetDetail<monitorAreaPointDate>(x => x.ID == id);
        }

        /// <summary>
        /// 寫入單筆資料
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public static BaseResult Insert(monitorAreaPointDate data)
        {
            data.CreatedAt = DateTime.Now;
            data.UpdatedAt = DateTime.Now;
            return RepositoryUtility.Insert(data);
        }

        /// <summary>
        /// 更新
        /// </summary>
        /// <returns></returns>
        public static BaseResult Update(monitorAreaPointDate data)
        {
            string sErrMsg = "";

            BaseRepository Base = new BaseRepository();
            //查詢資料
            var model = Base.QueryData<monitorAreaPointDate>(ref sErrMsg, x => x.ID == data.ID).SingleOrDefault();
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
        /// 刪除
        /// </summary>
        /// <param name="LoginID"></param>
        /// <returns></returns>
        public static BaseResult Delete(int ID)
        {
            return RepositoryUtility.Delete<monitorAreaPointDate>(x => x.ID == ID);
        }
        /// <summary>
        /// 多筆刪除
        /// </summary>
        /// <param name="filter"></param>
        /// <returns></returns>
        public static bool DeleteMany(Expression<Func<monitorAreaPointDate, bool>> filter)
        {
            return RepositoryUtility.DeleteAll<monitorAreaPointDate>(filter);
        }
    }
}
