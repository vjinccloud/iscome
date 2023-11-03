using ICCModule.Entity.Tables;
using ICCModule.Entity.Tables.Platform;
using ICCModule.ViewModel;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace InformationSystem.ViewModel.TAP
{
    public class resumeProductQueryModel
    {
        public resumeProductQueryModel()
        {
            KeyWord = "";
            TagResult = 0;
            QuilityResult = 0;
            resumeProductModels = new List<resumeProductModel>();
        }
        /// <summary> 
        /// 關鍵字
        /// </summary> 
        public string KeyWord { get; set; }

        /// <summary> 
        /// 標示檢查結果
        /// </summary> 
        public int TagResult { get; set; }

        /// <summary> 
        /// 品質檢查結果
        /// </summary> 
        public int QuilityResult { get; set; }

        /// <summary> 
        /// 檢查日期-起
        /// </summary> 
        public DateTime? CheckStartDate { get; set; }

        /// <summary> 
        /// 檢查日期-迄
        /// </summary> 
        public DateTime? CheckEndDate { get; set; }

        /// <summary> 
        /// 是否稽核紀錄
        /// </summary> 
        public bool? CheckData { get; set; }
        /// <summary> 
        /// 資料
        /// </summary> 
        public List<resumeProductModel> resumeProductModels { get; set; }
        /// <summary> 
        /// 總頁數
        /// </summary> 
        public int TotalPage { get; set; } = 1;
        /// <summary> 
        /// 頁數
        /// </summary> 
        public int Pages { get; set; } = 1;
    }
    public class resumeProductEditModel
    {
        public resumeProductEditModel()
        {
            ID = 0;
            OrganizationCode = "";
            VendorName = "";
            ProducerName = "";
            Principal = "";
            ContactPerson = "";
            Address = "";
            Phone = "";
            VerificationAgency = "";
            VerificationTypes = "";
            VerificationItems = "";
            Lot = "";
            LandNum = "";
            ActionName = "";
        }
        /// <summary> 
        /// 流水號
        /// </summary> 
        public int ID { get; set; }

        /// <summary>
        /// 組織代碼
        /// </summary> 
        public string OrganizationCode { get; set; }

        /// <summary> 業者名稱
        /// 
        /// </summary> 
        public string VendorName { get; set; }

        /// <summary>
        /// 生產者名稱
        /// </summary> 
        public string ProducerName { get; set; }

        /// <summary>
        /// 負責人
        /// </summary> 
        public string Principal { get; set; }

        /// <summary> 
        /// 聯絡人
        /// </summary> 
        public string ContactPerson { get; set; }

        /// <summary>
        /// 地址
        /// </summary> 
        public string Address { get; set; }

        /// <summary>
        /// 公開電話
        /// </summary> 
        public string Phone { get; set; }

        /// <summary>
        /// 驗證機構
        /// </summary> 
        public string VerificationAgency { get; set; }

        /// <summary> 
        /// 驗證類型
        /// </summary> 
        public string VerificationTypes { get; set; }

        /// <summary> 
        /// 驗證品項
        /// </summary> 
        public string VerificationItems { get; set; }

        /// <summary> 
        /// 驗證面積
        /// </summary> 
        public double? VerificationArea { get; set; }

        /// <summary> 
        /// 地段
        /// </summary> 
        public string Lot { get; set; }

        /// <summary> 
        /// 地號
        /// </summary> 
        public string LandNum { get; set; }

        /// <summary> 
        /// 驗證到期日
        /// </summary> 
        public DateTime? ExpirationDate { get; set; }
        /// <summary> 
        /// 動作名稱
        /// </summary> 
        public string ActionName { get; set; }
    }
}