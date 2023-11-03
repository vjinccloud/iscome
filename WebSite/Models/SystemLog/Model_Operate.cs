using ICCModule;
using ICCModule.Entity.Tables;
using ICCModule.EntityService.Service;
using IscomG2C.Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Website.Models.SystemLog
{
    public class Model_Operate
    {
        public Model_Operate()
        {
            this.page = new PagerInfo();
            this.LogList = new List<Service_sysAccess_log.VW_sysAccess_Log>();
            IniSet();
        }
        public PagerInfo page { get; set; }
        public string LoginID { get; set; }
        public DateTime? qrybeg { get; set; }
        public DateTime? qryend { get; set; }
        /// <summary>
        /// 帳號列表
        /// </summary>
        public List<SelectListItem> LoginIDList { get; set; }
        /// <summary>
        /// 資料列
        /// </summary>
        public List<Service_sysAccess_log.VW_sysAccess_Log> LogList { get; set; }
        

        public BaseResult Load()
        {
            try
            {
                this.LogList = new List<Service_sysAccess_log.VW_sysAccess_Log>();
                if (this.qrybeg != null && this.qryend != null)
                {
                    if(this.qrybeg > this.qryend)
                        return new BaseResult(false, "查詢起始日期不可大於結束日期!");
                }
                else if (this.qrybeg == null && this.qryend == null)
                {
                    //this.qrybeg = this.qrybeg ?? DateTime.Parse("1900-01-01");
                    //this.qryend = this.qryend ?? DateTime.Now;
                }
                else
                {
                    //this.qrybeg = this.qrybeg ?? DateTime.Parse("1900-01-01");
                    //this.qryend = this.qryend ?? DateTime.MaxValue;
                }
                this.LogList = Service_sysAccess_log.GetVWList(this.LoginID, this.qrybeg, this.qryend, this.page);
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
            var _List = Service_sysUserInfo.GetList();
            //設備縮寫選單
            this.LoginIDList = new List<SelectListItem>();
            this.LoginIDList = _List.ConvertAll(x => { return new SelectListItem() { Value = x.LoginID, Text = x.UserName, Selected = false }; }).OrderBy(x => x.Text).ToList();
            this.LoginIDList.Insert(0, new SelectListItem() { Value = "", Text = "請選擇" });           
        }
    }
}