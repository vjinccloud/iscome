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
    public class Service_sysRoleMenuRef
    {

        /// <summary>
        /// 讀取清單
        /// </summary>
        public static List<sysRoleMenuRef> GetList(string RoleCode = null, string Menu_ID = null, PagerInfo page = null)
        {
            Expression<Func<sysRoleMenuRef, bool>> filter = PredicateBuilder.True<sysRoleMenuRef>();
            if (!string.IsNullOrEmpty(RoleCode))
            {
                filter = filter.And(x => x.RoleCode == RoleCode);
            }
            if (!string.IsNullOrEmpty(Menu_ID))
            {
                filter = filter.And(x => x.Menu_ID == Menu_ID);
            }

            if (page != null)
                return RepositoryUtility.GetList_Range<sysRoleMenuRef, string>(page, filter, a => a.RoleCode, true).ToList();
            else
                return RepositoryUtility.GetList<sysRoleMenuRef>(filter);
        }

        /// <summary>
        /// 寫入單筆資料
        /// </summary>
        public static BaseResult Insert(sysRoleMenuRef data)
        {
            return RepositoryUtility.Insert<sysRoleMenuRef>(data);
        }

        /// <summary>
        /// 刪除
        /// </summary>
        public static BaseResult Delete(string RoleCode, string Menu_ID = null)
        {
            if (string.IsNullOrEmpty(Menu_ID))
            {
                string sErrMsg = "";
                bool rs = RepositoryUtility.DeleteAll<sysRoleMenuRef>(ref sErrMsg, x => x.RoleCode == RoleCode);
                return new BaseResult(rs, sErrMsg);
            }
            else
                return RepositoryUtility.Delete<sysRoleMenuRef>(x => x.RoleCode == RoleCode &&
                    (string.IsNullOrEmpty(Menu_ID) || x.Menu_ID == Menu_ID));
        }

        /// <summary>
        /// 勾選清單
        /// </summary>
        public class MenuChk
        {
            public MenuChk() { }
            public string RoleCode { get; set; }
            public string Menu_ID { get; set; }
            public string chkvalue { get; set; }

        }
    }
}
