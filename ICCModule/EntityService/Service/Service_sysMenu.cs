using ICCModule.Entity.Tables;
using ICCModule.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ICCModule.EntityService.Service
{
    public class Service_sysMenu
    {

        /// <summary>
        /// 讀取清單
        /// </summary>
        /// <returns></returns>
        public static List<sysMenu> GetList(int? lv = null)
        {
            return RepositoryUtility.GetList<sysMenu>(x => (lv == null) || x.Level == lv);
        }

        /// <summary>
        /// 寫入單筆資料
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public static BaseResult Insert(sysMenu data)
        {
            return RepositoryUtility.Insert<sysMenu>(data);
        }

        /// <summary>
        /// 刪除
        /// </summary>
        /// <param name="LoginID"></param>
        /// <returns></returns>
        public static BaseResult Delete(string MenuID)
        {
            return RepositoryUtility.Delete<sysMenu>(x => x.MenuID == MenuID);
        }
    }
}
