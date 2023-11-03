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
    public class Service_PhoneRegistCode
    {
        /// <summary>
        /// 讀取清單
        /// </summary>
        /// <returns></returns>
        public static List<PhoneRegistCode> GetList(Expression<Func<PhoneRegistCode, bool>> filter = null)
        {
            return RepositoryUtility.GetList<PhoneRegistCode>(filter);
        }
        
        /// <summary>
        /// 讀取單筆
        /// </summary>
        /// <returns></returns>
        public static PhoneRegistCode GetDetail(int id)
        {
            return RepositoryUtility.GetDetail<PhoneRegistCode>(x => x.Id == id);
        }

        /// <summary>
        /// 寫入單筆資料
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public static BaseResult Insert(PhoneRegistCode data)
        {
            data.ExpireDate = DateTime.Now.AddMinutes(10);
            data.CreateDate = DateTime.Now;
            return RepositoryUtility.Insert(data);
        }
    }
}
