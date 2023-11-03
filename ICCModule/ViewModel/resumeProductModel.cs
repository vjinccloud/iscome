using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ICCModule.ViewModel
{
    public class resumeProductModel
    {
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
        /// 地址
        /// </summary> 
        public string Address { get; set; }

        /// <summary>
        /// 經緯度
        /// </summary>
        public string Location { get; set; }

        /// <summary>
        /// 驗證機構
        /// </summary> 
        public string VerificationAgency { get; set; }

        /// <summary> 
        /// 驗證品項
        /// </summary> 
        public string VerificationItems { get; set; }

        /// <summary> 
        /// 創建日
        /// </summary> 
        public DateTime CreatedAt { get; set; }
        /// <summary> 
        /// 最近檢查日
        /// </summary> 
        public DateTime? LastCheckDate { get; set; }
        /// <summary> 
        /// 標示檢查結果
        /// </summary> 
        public bool? TagResult { get; set; }

        /// <summary> 
        /// 品質檢查結果
        /// </summary> 
        public bool? QuilityResult { get; set; }
        public List<string> Center {
            get
            {
                if (!string.IsNullOrEmpty(Location) && Location.Split(',').Length == 2)
                {
                    return Location.Split(',').OrderByDescending(x => x).ToList();
                }
                return new List<string>();
            }
        }
        public string TGOSX
        {
            get
            {
                if (!string.IsNullOrEmpty(Location) && Location.Split(',').Length == 2)
                {
                    return Location.Split(',')[0];
                }
                return "";
            }
        }
        public string TGOSY
        {
            get
            {
                if (!string.IsNullOrEmpty(Location) && Location.Split(',').Length == 2)
                {
                    return Location.Split(',')[1];
                }
                return "";
            }
        }
    }

    public class resumeProductReq
    {

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

    }
    public class resumeProductExportModel
    {
        /// <summary>
        /// 組織代碼
        /// </summary> 
        public string OrganizationCode { get; set; }

        /// <summary> 
        /// 業者名稱
        /// </summary> 
        public string VendorName { get; set; }

        /// <summary>
        /// 生產者名稱
        /// </summary> 
        public string ProducerName { get; set; }

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
        /// 檢查日期
        /// </summary> 
        public DateTime? Date { get; set; }

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

    }
}
