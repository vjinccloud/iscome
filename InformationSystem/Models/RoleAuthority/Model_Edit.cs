using ICCModule;
using ICCModule.Entity.Tables;
using ICCModule.EntityService.Service;
using IscomG2C.Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace InformationSystem.Models.RoleAuthority
{
    public class Model_Edit
    {
        public Model_Edit(string RoleCode)
        {
            this.Role = Service_sysRole.GetDetail(RoleCode);
            this.checkMenuList = new List<Service_sysRoleMenuRef.MenuChk>();
            this.myMenuList = new List<sysRoleMenuRef>();
            IniSet();
        }
        /// <summary>
        /// 資料
        /// </summary>
        public sysRole Role { get; set; }
        /// <summary>
        /// 群組名稱
        /// </summary>
        public string RoleName { get; set; }
        /// <summary>
        /// 啟用
        /// </summary>
        public string isActive { get; set; }
        /// <summary>
        /// 狀態列表
        /// </summary>
        public List<SelectListItem> StateList { get; set; }
        /// <summary>
        /// 主功能列表
        /// </summary>
        public List<sysMenu> MainMenuList { get; set; }
        /// <summary>
        /// 子功能列表
        /// </summary>
        public List<sysMenu> SubMenuList { get; set; }
        /// <summary>
        /// 勾選的Menu列表
        /// </summary>
        public List<Service_sysRoleMenuRef.MenuChk> checkMenuList { get; set; }
        /// <summary>
        /// 讀取Menu設定列表
        /// </summary>
        public List<sysRoleMenuRef> myMenuList { get; set; }

        /// <summary>
        /// 初始化設定
        /// </summary>
        void IniSet()
        {
            //設定狀態下拉選單
            this.StateList = new List<SelectListItem>();
            this.StateList.Add(new SelectListItem { Text = "啟用", Value = "Y" });
            this.StateList.Add(new SelectListItem { Text = "停用", Value = "N" });

            List<sysMenu> MenuList = Service_sysMenu.GetList(null);
            this.MainMenuList = new List<sysMenu>();
            this.MainMenuList = MenuList.Where(x => x.Level == 1).ToList();

            this.SubMenuList = new List<sysMenu>();
            this.SubMenuList = MenuList.Where(x => x.Level == 2).ToList();
        }

        public BaseResult Load(string RoleCode)
        {
            try
            {
                this.myMenuList = Service_sysRoleMenuRef.GetList(RoleCode, null);
                this.checkMenuList = this.myMenuList.ConvertAll(x => { return new Service_sysRoleMenuRef.MenuChk() { RoleCode = x.RoleCode, Menu_ID = x.Menu_ID, chkvalue = x.AllowList }; }).OrderBy(x => x.Menu_ID).ToList();
                if (this.Role != null)
                    return new BaseResult(true, "");
            }
            catch (Exception e)
            {
                ErrorLog.SaveErrorLog(e.ToString());
            }
            return new BaseResult(false, "讀取時發生錯誤");
        }
        public BaseResult SaveData(List<Service_sysRoleMenuRef.MenuChk> chklist)
        {
            try
            {
                this.Role.UpdateDate = DateTime.Now;
                this.Role.RoleName = this.RoleName;
                this.Role.isActive = this.isActive;
                return Service_sysRole.Update(this.Role, chklist);
            }
            catch (Exception e)
            {
                ErrorLog.SaveErrorLog(e.ToString());
            }
            return new BaseResult(false, "儲存時發生錯誤");
        }


        public BaseResult DeleteData()
        {
            try
            {
                return Service_sysRole.Delete(this.Role.RoleCode);
            }
            catch (Exception e)
            {
                ErrorLog.SaveErrorLog(e.ToString());
            }
            return new BaseResult(false, "刪除時發生錯誤");
        }

    }
}