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
    public class Service_sysRole
    {

        /// <summary>
        /// 讀取清單
        /// </summary>
        public static List<sysRole> GetList(string RoleCode, string isActive, PagerInfo page = null)
        {
            Expression<Func<sysRole, bool>> filter = PredicateBuilder.True<sysRole>();
            if (!string.IsNullOrEmpty(RoleCode))
            {
                filter = filter.And(x => x.RoleCode == RoleCode);
            }
            if (!string.IsNullOrEmpty(isActive))
            {
                filter = filter.And(x => x.isActive == isActive);
            }

            if (page != null)
                return RepositoryUtility.GetList_Range<sysRole, string>(page, filter, a => a.RoleCode, true).ToList();
            else
                return RepositoryUtility.GetList<sysRole>(filter);
        }

        /// <summary>
        /// 抓取單筆資料
        /// </summary>
        public static sysRole GetDetail(string RoleCode)
        {
            return RepositoryUtility.GetDetail<sysRole>(x => x.RoleCode == RoleCode);
        }

        /// <summary>
        /// 寫入單筆資料
        /// </summary>
        public static BaseResult Insert(sysRole data, List<Service_sysRoleMenuRef.MenuChk> chklist)
        {
            int DataCnt = RepositoryUtility.GetCount<sysRole>(x => x.RoleName == data.RoleName);
            if (DataCnt > 0)
            {
                return new BaseResult(false, "此群組名稱已被使用，無法新增。");
            }

            int Code = 0;
            string RoleCode = "";
            sysRole maxRole = RepositoryUtility.GetList<sysRole>().OrderByDescending(x => x.RoleCode).First();
            if (int.TryParse(maxRole.RoleCode.Substring(1), out Code))
            {
                Code = Code + 1;
                RoleCode = "R" + Code.ToString("0#");
                #region 設定角色權限
                if (chklist != null)
                {
                    var rolemenuList = chklist.GroupBy(x => x.Menu_ID, x => x.chkvalue);
                    //設定權限
                    foreach (var item in rolemenuList)
                    {
                        sysRoleMenuRef rolemenu = new sysRoleMenuRef();
                        rolemenu.RoleCode = RoleCode;
                        rolemenu.Menu_ID = item.Key;
                        rolemenu.AllowList = "";
                        foreach (var auth in item)
                        {
                            rolemenu.AllowList += "|" + auth;
                        }
                        rolemenu.AllowList = rolemenu.AllowList.Length > 0 ? rolemenu.AllowList + "|" : "";
                        rolemenu.Remark = "";
                        RepositoryUtility.Insert<sysRoleMenuRef>(rolemenu);
                    }
                }
                #endregion
                //儲存角色
                data.RoleCode = RoleCode;
                return RepositoryUtility.Insert<sysRole>(data);
            }
            return new BaseResult(false, "新增異常，請重新建立!");
        }
        /// <summary>
        /// 寫入單筆資料
        /// </summary>
        public static BaseResult Update(sysRole data, List<Service_sysRoleMenuRef.MenuChk> chklist)
        {
            string sErrMsg = "";

            BaseRepository Base = new BaseRepository();
            //查詢資料
            var model = Base
                .QueryData<sysRole>(ref sErrMsg, x => x.RoleCode == data.RoleCode)
                .SingleOrDefault();
            if (model == null)
            {
                sErrMsg = "查無欲更新的資料";
                return new BaseResult(false, sErrMsg); ;
            }
            if (model.RoleName != data.RoleName)
            {
                int DataCnt = RepositoryUtility.GetCount<sysRole>(x => x.RoleName == data.RoleName);
                if (DataCnt > 0)
                {
                    return new BaseResult(false, "此群組名稱已被使用，修改失敗!");
                }
                model.RoleName = data.RoleName;
            }
            model.isActive = data.isActive;
            model.UpdateDate = data.UpdateDate;
            model.UpdateUser = data.UpdateUser;

            #region 設定角色權限
            if (chklist != null)
            {
                var rolemenuList = chklist.GroupBy(x => x.Menu_ID, x => x.chkvalue);
                //刪除舊權限
                Service_sysRoleMenuRef.Delete(data.RoleCode);
                //設定新權限
                foreach (var item in rolemenuList)
                {
                    sysRoleMenuRef rolemenu = new sysRoleMenuRef();
                    rolemenu.RoleCode = data.RoleCode;
                    rolemenu.Menu_ID = item.Key;
                    rolemenu.AllowList = "";
                    foreach (var auth in item)
                    {
                        rolemenu.AllowList += "|" + auth;
                    }
                    rolemenu.AllowList = rolemenu.AllowList.Length > 0 ? rolemenu.AllowList + "|" : "";
                    rolemenu.Remark = "";
                    RepositoryUtility.Insert<sysRoleMenuRef>(rolemenu);
                }
            }
            #endregion

            //更新回資料庫
            bool rslt = Base.Update(ref sErrMsg);
            return new BaseResult(rslt, sErrMsg);
        }

        /// <summary>
        /// 刪除
        /// </summary>
        public static BaseResult Delete(string RoleCode)
        {
            //刪除角色權限
            Service_sysRoleMenuRef.Delete(RoleCode);
            return RepositoryUtility.Delete<sysRole>(x => x.RoleCode == RoleCode);
        }
    }
}
