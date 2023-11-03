using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ICCModule.ViewModel
{
    public class PesticideModel
    {
        /// <summary> 
        /// 識別ID
        /// </summary> 
        public string LicenseNum { get; set; }

        /// <summary>
        /// 業者名稱
        /// </summary> 
        public string VendorName { get; set; }

        /// <summary>
        /// 營業地址
        /// </summary> 
        public string VendorAddress { get; set; }

        /// <summary> 
        /// 狀態
        /// </summary> 
        public int Status { get; set; }

        /// <summary> 
        /// 聯繫電話
        /// </summary> 
        public string ContactPhone { get; set; }

        /// <summary> 
        /// 最近一次檢查日期
        /// </summary> 
        public DateTime? LastCheckDate { get; set; }

        /// <summary> 
        /// 友善資材站起日
        /// </summary> 
        public DateTime? FriendlyStartDate { get; set; }

        /// <summary>
        /// 友善資材站迄日
        /// </summary> 
        public DateTime? FriendlyEndDate { get; set; }

        /// <summary>
        /// 創建時間
        /// </summary> 
        public DateTime CreatedAt { get; set; }

        /// <summary>
        /// 更新時間
        /// </summary> 
        public DateTime? UpdatedAt { get; set; }

        /// <summary> 
        /// 檢查結果
        /// </summary> 
        public bool? Result { get; set; }

    }
    public class PesticideExportModel
    {
        /// <summary> 
        /// 識別ID
        /// </summary> 
        public string LicenseNum { get; set; }

        /// <summary>
        /// 業者名稱
        /// </summary> 
        public string VendorName { get; set; }

        /// <summary>
        /// 營業地址
        /// </summary> 
        public string VendorAddress { get; set; }

        /// <summary> 
        /// 狀態
        /// </summary> 
        public int Status { get; set; }

        /// <summary> 
        /// 檢查結果
        /// </summary> 
        public bool? Result { get; set; }
        
        /// <summary> 
        /// 檢查日期
        /// </summary> 
        public DateTime? Date { get; set; }
        
        /// <summary> 
        /// 檢查結果說明
        /// </summary> 
        public string Instructions { get; set; }

    }
}
