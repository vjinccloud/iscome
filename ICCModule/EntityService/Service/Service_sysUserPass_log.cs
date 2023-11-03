using ICCModule.Entity.Tables;
using ICCModule.Repository;
using ICCModule.Repository.Class;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ICCModule.EntityService.Service
{
    public class Service_sysUserPass_log
    {
        /// <summary>
        /// 讀取清單
        /// </summary>
        /// <returns></returns>
        public static List<sysUserPass_log> GetList(string LoginID)
        {
            return RepositoryUtility.GetList<sysUserPass_log>(x => x.LoginID == LoginID);
        }

        /// <summary>
        /// 寫入單筆資料
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public static BaseResult Insert(sysUserPass_log data)
        {
            return RepositoryUtility.Insert<sysUserPass_log>(data);
        }

        /// <summary>
        /// 刪除
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public static BaseResult Delete(long id)
        {
            return RepositoryUtility.Delete<sysUserPass_log>(x => x.id == id);
        }
    }
}
