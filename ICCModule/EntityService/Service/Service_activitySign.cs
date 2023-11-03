using ICCModule.Entity.Tables.Platform;
using ICCModule.Repository;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace ICCModule.EntityService.Service
{
    public class Service_activitySign
    {
        /// <summary>讀取清單</summary>
        /// <param name="Code"></param>
        /// <param name="Show">上、下架</param>
        /// <returns>
        ///   <br />
        /// </returns>
        public static List<activitySign> GetList(Expression<Func<activitySign, bool>> filter = null)
        {
            return RepositoryUtility.GetList<activitySign>(filter);
        }

        /// <summary>
        /// 寫入單筆資料
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public static BaseResult Insert(activitySign data)
        {
            data.SignInDate = DateTime.Now;
            return RepositoryUtility.Insert<activitySign>(data);
        }
        /// <summary>
        /// 刪除
        /// </summary>
        /// <param name="delIds"></param>
        /// <returns></returns>
        public static void DeleteAll(List<int> delIds)
        {
            foreach(var id in delIds)
            {
                RepositoryUtility.Delete<activitySign>(x => x.Id == id);
            }
        }
    }
}
