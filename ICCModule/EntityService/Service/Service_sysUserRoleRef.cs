using ICCModule.Entity.Tables;
using ICCModule.Repository;
using ICCModule.Repository.Class;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace ICCModule.EntityService.Service
{
    public class Service_sysUserRoleRef
    {
        /// <summary>
        /// 抓取單筆資料
        /// </summary>
        /// <param name="LoginID"></param>
        /// <returns></returns>
        public static sysUserRoleRef GetDetail(string LoginID, string RoleCode)
        {
            return RepositoryUtility.GetDetail<sysUserRoleRef>(x => x.LoginID == LoginID && x.RoleCode == RoleCode);
        }

        /// <summary>
        /// 讀取清單
        /// </summary>
        /// <returns></returns>
        public static List<sysUserRoleRef> GetList(string LoginID = null, string RoleCode = null)
        {
            Expression<Func<sysUserRoleRef, bool>> filter = PredicateBuilder.True<sysUserRoleRef>();
            if(!string.IsNullOrEmpty(LoginID))
                filter = filter.And(x => x.LoginID == LoginID);
            if (!string.IsNullOrEmpty(RoleCode))
                filter = filter.And(x => x.RoleCode == RoleCode);

            return RepositoryUtility.GetList<sysUserRoleRef>(filter);
        }

        /// <summary>
        /// 寫入單筆資料
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public static BaseResult Insert(sysUserRoleRef data)
        {
            //抓取角色名稱 帶入備註 
            string RoleName = Service_sysRole.GetDetail(data.RoleCode).RoleName ?? "";
            data.Remark = RoleName;
            return RepositoryUtility.Insert<sysUserRoleRef>(data);
        }

        /// <summary>
        /// 更新單筆資料
        /// </summary>
        public static BaseResult Update(sysUserRoleRef data)
        {
            string sErrMsg = "";

            BaseRepository Base = new BaseRepository();
            //查詢資料
            var model = Base
                .QueryData<sysUserRoleRef>(ref sErrMsg, x => x.LoginID == data.LoginID)
                .SingleOrDefault();
            if (model == null)
            {
                sErrMsg = "查無欲更新的資料";
                return new BaseResult(false, sErrMsg);
            }
            //只能更新角色
            model.RoleCode = data.RoleCode;
            //抓取角色名稱 帶入備註 
            string RoleName = Service_sysRole.GetDetail(data.RoleCode).RoleName ?? "";
            model.Remark = RoleName;
            //更新回資料庫
            bool rslt = Base.Update(ref sErrMsg);
            return new BaseResult(rslt, sErrMsg);
        }

        /// <summary>
        /// 刪除
        /// </summary>
        /// <param name="LoginID"></param>
        /// <returns></returns>
        public static BaseResult Delete(string LoginID)
        {
            return RepositoryUtility.Delete<sysUserRoleRef>(x => x.LoginID == LoginID);
        }
    }
}
