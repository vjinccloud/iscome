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
    public class PesticideSellerModel
    {
        public PesticideSellerModel()
        {
            LicenseNum = "";
            VendorName = "";
            VendorAddress = "";
            Status = 1;
            ContactPhone = "";
            ActionName = "Add";
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
        /// 營業地址
        /// </summary> 
        public string VendorAddress { get; set; }

        /// <summary>
        /// 經緯度
        /// </summary> 
        public string Location { get; set; }

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
        /// 動作名稱
        /// </summary> 
        public string ActionName { get; set; }
    }
    public class PesticideSellerQueryModel
    {
        public PesticideSellerQueryModel()
        {
            District = "";
            KeyWord = "";
            Status = "";
            StatusList = new List<int>();
            Districts = DistrictHelper.KaohsiungDistrict();
            PesticideModels = new List<PesticideModel>();
        }
        /// <summary> 
        /// 行政區
        /// </summary> 
        public string District { get; set; }

        /// <summary> 
        /// 關鍵字
        /// </summary> 
        public string KeyWord { get; set; }

        /// <summary> 
        /// 營業狀態
        /// </summary> 
        public string Status { get; set; }
        
        /// <summary> 
        /// 營業狀態
        /// </summary> 
        public List<int> StatusList { get; set; }

        /// <summary> 
        /// 檢查結果
        /// </summary> 
        public int? CheckResult { get; set; }

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
        public List<PesticideModel> PesticideModels { get; set; }
        /// <summary> 
        /// 高雄行政區
        /// </summary> 
        public List<string> Districts { get; set; }
        /// <summary> 
        /// 總頁數
        /// </summary> 
        public int TotalPage { get; set; } = 1;
        /// <summary> 
        /// 頁數
        /// </summary> 
        public int Pages { get; set; } = 1;
    }
    public class PesticideSellerQueryReq
    {
        public PesticideSellerQueryReq()
        {
            District = "";
            KeyWord = "";
            Status = new List<int>();
        }
        /// <summary> 
        /// 行政區
        /// </summary> 
        public string District { get; set; }

        /// <summary> 
        /// 關鍵字
        /// </summary> 
        public string KeyWord { get; set; }

        /// <summary> 
        /// 營業狀態
        /// </summary> 
        public List<int> Status { get; set; }

        /// <summary> 
        /// 檢查結果
        /// </summary> 
        public int? CheckResult { get; set; }

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
    }
}