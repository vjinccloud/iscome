using ICCModule.Entity.Tables;
using ICCModule.Entity.Tables.Platform;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace InformationSystem.ViewModel.TAP
{
    public class resumeProductCheckListModel
    {
        public resumeProductCheckListModel()
        {
            OrganizationCode = "";
            VendorName = "";
            resumeProductChecks = new List<resumeProductCheck>();
        }
        /// <summary>
        /// 產銷履歷序號
        /// </summary> 
        public int ProductId { get; set; }
        
        /// <summary>
        /// 組織代碼
        /// </summary> 
        public string OrganizationCode { get; set; }

        /// <summary>
        /// 業者名稱
        /// </summary> 
        public string VendorName { get; set; }
        
        /// <summary>
        /// 業者名稱
        /// </summary> 
        public List<resumeProductCheck> resumeProductChecks { get; set; }

    }
    public class resumeProductCheckEditModel
    {
        public resumeProductCheckEditModel()
        {
            ID = 0;
            ProductID = 0;
            Date = DateTime.Now;
            ArbitrationInstructions = "";
            OldFiles = new List<FileManagement>();
            EditOldFiles = new List<long>();
        }
        /// <summary> 
        /// 流水號
        /// </summary> 
        public int ID { get; set; }

        /// <summary> 
        /// 產品ID
        /// </summary> 
        public int ProductID { get; set; }

        /// <summary> 
        /// 檢查日期
        /// </summary> 
        public DateTime Date { get; set; }

        /// <summary> 
        /// 標示檢查結果
        /// </summary> 
        public bool? TagResult { get; set; }

        /// <summary>
        /// 品質檢查結果
        /// </summary> 
        public bool? QuilityResult { get; set; }

        /// <summary> 
        /// 裁處說明
        /// </summary> 
        public string ArbitrationInstructions { get; set; }
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