using ICCModule;
using ICCModule.Entity.Tables;
using ICCModule.EntityService.Service;
using IscomG2C.Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Website.Models.RoleAuthority
{
    public class Model_List
    {
        public Model_List()
        {
            this.page = new PagerInfo();
            this.RolesList = new List<sysRole>();
            IniSet();
        }
        public PagerInfo page { get; set; }
        /// <summary>
        /// 群組列表
        /// </summary>
        public List<SelectListItem> RoleCodeList { get; set; }
        /// <summary>
        /// 狀態列表
        /// </summary>
        public List<SelectListItem> StateList { get; set; }
        /// <summary>
        /// 資料列
        /// </summary>
        public List<sysRole> RolesList { get; set; }

        public string RoleCode { get; set; }
        public string isActive { get; set; }

        public BaseResult Load()
        {
            try
            {
                this.RolesList = new List<sysRole>();
                this.RolesList = Service_sysRole.GetList(this.RoleCode, this.isActive, this.page);
            }
            catch (Exception e)
            {
                ErrorLog.SaveErrorLog(e.ToString());
                return new BaseResult(false, "讀取時發生錯誤");
            }

            return new BaseResult(true, "");

        }

        void IniSet()
        {
            var _List = Service_sysRole.GetList(this.RoleCode, this.isActive, this.page);
            //縮寫選單
            this.RoleCodeList = new List<SelectListItem>();
            this.RoleCodeList = _List.ConvertAll(x => { return new SelectListItem() { Value = x.RoleCode, Text = x.RoleName, Selected = false }; }).OrderBy(x => x.Value).ToList();
            this.RoleCodeList.Insert(0, new SelectListItem() { Value = "", Text = "請選擇" });
            //設定狀態下拉選單
            this.StateList = new List<SelectListItem>();
            this.StateList.Add(new SelectListItem { Text = "請選擇", Value = "" });
            this.StateList.Add(new SelectListItem { Text = "啟用", Value = "Y" });
            this.StateList.Add(new SelectListItem { Text = "停用", Value = "N" });
        }
    }
}