using ICCModule;
using ICCModule.Entity.Tables;
using ICCModule.EntityService.Service;
using IscomG2C.Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace InformationSystem.Models.SystemLog
{
    public class Model_Operate
    {
        public Model_Operate()
        {
            //this.page = new PagerInfo();
            this.LogList = new List<sysAccess_log>();            
            IniSet();
        }
        //public PagerInfo page { get; set; }
        public string LoginID { get; set; } = "";
        public DateTime? qrybeg { get; set; }
        public DateTime? qryend { get; set; }
        public int TotalPage { get; set; }
        public int Page { get; set; }

        /// <summary>
        /// 帳號列表
        /// </summary>
        public List<SelectListItem> LoginIDList { get; set; }
        /// <summary>
        /// 資料列
        /// </summary>
        public List<sysAccess_log> LogList { get; set; }
        
        void IniSet()
        {
            var _List = Service_sysUserInfo.GetList();
            //設備縮寫選單
            this.LoginIDList = new List<SelectListItem>();
            this.LoginIDList = _List.ConvertAll(x => { return new SelectListItem() { Value = x.LoginID, Text = x.UserName, Selected = false }; }).OrderBy(x => x.Text).ToList();
            this.LoginIDList.Insert(0, new SelectListItem() { Value = "", Text = "請選擇" });           
        }
    }
}