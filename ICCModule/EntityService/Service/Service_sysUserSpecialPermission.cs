using ICCModule.Entity.Tables;
using ICCModule.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace ICCModule.EntityService.Service
{
    public class Service_sysUserSpecialPermission
    {

        /// <summary>
        /// 讀取清單
        /// </summary>
        public static List<sysUserSpecialPermission> GetList(Expression<Func<sysUserSpecialPermission, bool>> filter = null)
        {
            return RepositoryUtility.GetList<sysUserSpecialPermission>(filter);
        }

        /// <summary>
        /// 寫入單筆資料
        /// </summary>
        public static BaseResult Insert(sysUserSpecialPermission data)
        {
            var res = RepositoryUtility.Insert<sysUserSpecialPermission>(data);
            return res;
        }

        /// <summary>
        /// 刪除
        /// </summary>
        public static BaseResult Delete(string sysUserId, List<int> PermissionIds = null)
        {
            if (PermissionIds == null) PermissionIds = new List<int>();
            return RepositoryUtility.Delete<sysUserSpecialPermission>(x => x.sysUserId == sysUserId && !PermissionIds.Contains(x.PermissionId));
        }
    }
}
