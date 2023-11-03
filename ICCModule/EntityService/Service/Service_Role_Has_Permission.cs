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
    public class Service_Role_Has_Permission
    {
        /// <summary>
        /// 抓取單筆資料
        /// </summary>
        /// <param name="PermissionId"></param>
        /// <param name="RoleCode"></param>
        /// <returns></returns>
        public static RoleHasPermission GetDetail(int PermissionId, string RoleCode)
        {
            return RepositoryUtility.GetDetail<RoleHasPermission>(x => (x.PermissionId == PermissionId) && (x.RoleCode == RoleCode));
        }

        /// <summary>
        /// 取得列表
        /// </summary>
        /// <param name="RoleCode"></param>
        /// <returns></returns>
        public static List<RoleHasPermission> GetList(string RoleCode)
        {
            Expression<Func<RoleHasPermission, bool>> filter = (x => true);
            if (!String.IsNullOrEmpty(RoleCode))
            {
                filter = filter.And(x => x.RoleCode == RoleCode);
            }

            return RepositoryUtility.GetList<RoleHasPermission>(filter);
        }

        /// <summary>
        /// 寫入單筆資料
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public static bool Insert(List<RoleHasPermission> list)
        {
            foreach (var item in list)
            {
                RepositoryUtility.Insert<RoleHasPermission>(item);
            }
            return true;
        }

        /// <summary>
        /// 刪除指定腳色編碼全部
        /// </summary>
        /// <param name="RoleCode">腳色編碼</param>
        /// <returns></returns>
        public static bool DeleteAll(string RoleCode)
        {
            string msg = String.Empty;

            return RepositoryUtility.DeleteAll<RoleHasPermission>(ref msg, x => x.RoleCode == RoleCode);
        }
    }
}
