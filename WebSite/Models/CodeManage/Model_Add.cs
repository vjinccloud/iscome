using ICCModule;
using ICCModule.Entity.Tables;
using ICCModule.EntityService.Service;
using IscomG2C.Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Website.Models.CodeManage
{
    public class Model_Add
    {
        public Model_Add()
        {
            IniSet();
        }
        /// <summary>
        /// 代碼資料
        /// </summary>
        public defCode myCode { get; set; }
        /// <summary>
        /// 代碼類型
        /// </summary>
        public string Kind { get; set; }
        /// <summary>
        /// 狀態列表
        /// </summary>
        public List<SelectListItem> StateList { get; set; }

        /// <summary>
        /// 初始化設定
        /// </summary>
        void IniSet()
        {
            //設定狀態下拉選單
            this.StateList = new List<SelectListItem>();
            this.StateList.Add(new SelectListItem { Text = "啟用", Value = "N" });
            this.StateList.Add(new SelectListItem { Text = "停用", Value = "Y" });
        }
        //讀取
        public BaseResult Load (string Kind)
        {
            try
            {
                var _tmp = Service_defCode.GetKindDetail(Kind);
                if (_tmp != null)
                {
                    if (_tmp.AllowUpdate == "Y")
                    {
                        this.myCode = new defCode();
                        this.myCode.Kind = _tmp.Kind;
                        this.myCode.KindName = _tmp.KindName;
                        return new BaseResult(true, "");
                    }
                    return new BaseResult(false, "此代碼類型不允許新增!");
                }
                return new BaseResult(false, "載入失敗，請重新整理頁面!");
            }
            catch (Exception e)
            {
                ErrorLog.SaveErrorLog(e.ToString());
            }
            return new BaseResult(false, "讀取時發生錯誤");
        }
        public BaseResult SaveData()
        {
            try
            {
                this.myCode.CreateDate = DateTime.Now;
                this.myCode.UpdateDate = DateTime.Now;
                this.myCode.AllowUpdate = "Y";
                return Service_defCode.Insert(this.myCode);
            }
            catch (Exception e)
            {
                ErrorLog.SaveErrorLog(e.ToString());
            }
            return new BaseResult(false, "儲存時發生錯誤");
        }

    }
}