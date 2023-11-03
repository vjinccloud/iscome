using ICCModule.Entity.Tables;
using ICCModule.Entity.Tables.Platform;
using ICCModule.Helper;
using ICCModule.ViewModel;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace InformationSystem.ViewModel
{
    public class PesticideAuditModel
    {
        public PesticideAuditModel()
        {
            LicenseNum = "";
            VendorName = "";
            PesticideAudits = new List<pesticideAudit>();
        }
        /// <summary> 
        /// 識別ID
        /// </summary> 
        public string LicenseNum { get; set; }
        /// <summary> 
        /// 業者名稱
        /// </summary> 
        public string VendorName { get; set; }
        /// <summary> 
        /// 檢查資料
        /// </summary> 
        public List<pesticideAudit> PesticideAudits { get; set; }
    }
    public class PesticideAuditEditModel
    {
        public PesticideAuditEditModel()
        {
            Date = DateTime.Today;
            Result = false;
            OldFiles = new List<FileManagement>();
            EditOldFiles = new List<long>();
        }
        /// <summary> 
        /// 流水號
        /// </summary> 
        public int ID { get; set; }

        /// <summary> 
        /// 農藥業者識別ID
        /// </summary> 
        public string SellerID { get; set; }

        /// <summary> 
        /// 檢查日期
        /// </summary> 
        public DateTime Date { get; set; }

        /// <summary> 
        /// 檢查結果
        /// </summary> 
        public bool Result { get; set; }

        /// <summary> 
        /// 說明
        /// </summary> 
        public string Instructions { get; set; }
        /// <summary> 
        /// 舊檔案
        /// </summary> 
        public List<FileManagement> OldFiles { get; set; }
        /// <summary> 
        /// 編輯舊檔案
        /// </summary> 
        public List<long> EditOldFiles { get; set; }
    }
}