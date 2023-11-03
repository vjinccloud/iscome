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
    public class Model_Edit
    {
        public Model_Edit()
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

        public BaseResult Load(string Kind, string Code)
        {
            try
            {
                this.myCode = Service_defCode.GetDetail(Kind,Code);
                if (this.myCode != null)
                {
                    //如果不允許編輯 設定提醒
                    if (this.myCode.AllowUpdate == "Y")
                    {
                        return new BaseResult(true, "");
                    }
                    return new BaseResult(true, "此代碼類型不允許更改!");
                }
                return new BaseResult(false, "載入失敗!");
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
                this.myCode.UpdateDate = DateTime.Now;
                return Service_defCode.Update(this.myCode);
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
                return Service_defCode.Delete(this.myCode.Kind, this.myCode.Code);
            }
            catch (Exception e)
            {
                ErrorLog.SaveErrorLog(e.ToString());
            }
            return new BaseResult(false, "刪除時發生錯誤");
        }

    }
}