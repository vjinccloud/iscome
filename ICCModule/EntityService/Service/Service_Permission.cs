using ICCModule.Entity.Tables.Platform;
using ICCModule.Repository;
using ICCModule.Repository.Class;
using IscomG2C.Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace ICCModule.EntityService.Service
{
    public class Service_Permission
    {
        /// <summary>
        /// 抓取單筆資料
        /// </summary>
        /// <param name="Id"></param>
        /// <returns></returns>
        public static Permission GetDetail(int Id)
        {
            return RepositoryUtility.GetDetail<Permission>(x => x.Id == Id);
        }

        /// <summary>讀取清單</summary>
        /// <returns>
        ///   <br />
        /// </returns>
        public static List<Permission> GetList()
        {
            Expression<Func<Permission, bool>> filter = (x => true);

            return RepositoryUtility.GetList<Permission>(filter);
        }

        /// <summary>
        /// 取出前台權限列表
        /// </summary>
        /// <returns></returns>
        public static List<Permission> GetFrontList()
        {
            Expression<Func<Permission, bool>> filter = (x => true);
            filter = filter.And(x => x.Code.StartsWith("F_"));

            return RepositoryUtility.GetList<Permission>(filter);
        }

        /// <summary>
        /// 取出後台權限列表
        /// </summary>
        /// <returns></returns>
        public static List<Permission> GetBackList()
        {
            Expression<Func<Permission, bool>> filter = (x => true);
            filter = filter.And(x => x.Code.StartsWith("B_"));

            return RepositoryUtility.GetList<Permission>(filter);
        }
    }
}
