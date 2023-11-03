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
    /// 每日天氣
    /// </summary>
    public class Service_DailyWeather
    {
        /// <summary>
        /// 讀取清單
        /// </summary>
        /// <returns></returns>
        public static List<DailyWeather> GetList(Expression<Func<DailyWeather, bool>> filter = null)
        {
            return RepositoryUtility.GetList<DailyWeather>(filter);
        }
        
        /// <summary>
        /// 讀取單筆
        /// </summary>
        /// <returns></returns>
        public static DailyWeather GetDetail(int id)
        {
            return RepositoryUtility.GetDetail<DailyWeather>(x => x.Id == id);
        }
        
        /// <summary>
        /// 讀取單筆
        /// </summary>
        /// <returns></returns>
        public static DailyWeather GetDetail(string ct,string dt)
        {
            return RepositoryUtility.GetDetail<DailyWeather>(x => x.CityName == ct && x.DistrictName == dt);
        }

        /// <summary>
        /// 寫入單筆資料
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public static BaseResult Insert(DailyWeather data)
        {
            return RepositoryUtility.Insert(data);
        }
    }
}
