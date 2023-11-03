using ICCModule;
using ICCModule.Entity.Tables;
using ICCModule.EntityService.Service;
using IscomG2C.Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace InformationSystem.Models.CodeManage
{
    public class Model_List
    {
        public Model_List()
        {
            this.page = new PagerInfo();
            this.CodeList = new List<defCode>();
            IniSet();
        }
        public PagerInfo page { get; set; }
        public string CodeKind { get; set; }
        public string Code { get; set; }
        public string AllowUpdate { get; set; }
        /// <summary>
        /// 代碼類型
        /// </summary>
        public List<SelectListItem> CodeKindList { get; set; }
        /// <summary>
        /// 資料列
        /// </summary>
        public List<defCode> CodeList { get; set; }
        /// <summary>
        /// 查詢
        /// </summary>
        public BaseResult Load()
        {
            try
            {
                this.CodeList = new List<defCode>();
                this.CodeList = Service_defCode.GetPageList(this.CodeKind, this.page);
                this.AllowUpdate = "Y";
                if (this.CodeList.Count > 0)
                    this.AllowUpdate = this.CodeList.FirstOrDefault().AllowUpdate;
            }
            catch (Exception e)
            {
                ErrorLog.SaveErrorLog(e.ToString());
                return new BaseResult(false, "讀取時發生錯誤");
            }

            return new BaseResult(true, "");

        }

        /// <summary>
        /// 初始化設定
        /// </summary>
        void IniSet()
        {
            //設定類別下拉選單 預設選擇第一種類型
            this.CodeKindList = new List<SelectListItem>();
            this.CodeKindList = Service_defCode.GetKindSelectList();
            this.CodeKind = this.CodeKindList.First().Value;
        }
    }
}