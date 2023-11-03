using System;
using System.Collections.Generic;
using System.Data.Linq.Mapping;
using System.Linq;

namespace ICCModule.Entity.Tables.Platform
{
    [Serializable]
    [Table(Name = "pesticide_Sellers")]
    public class pesticideSeller
    {
        /// <summary> 識別ID
        /// 
        /// </summary> 
        [Column(IsPrimaryKey = true, IsDbGenerated = false)]
        public string LicenseNum { get; set; }

        /// <summary> 業者名稱
        /// 
        /// </summary> 
        [Column]
        public string VendorName { get; set; }

        /// <summary> 營業地址
        /// 
        /// </summary> 
        [Column]
        public string VendorAddress { get; set; }

        /// <summary> 經緯度
        /// 
        /// </summary> 
        [Column]
        public string Location { get; set; }

        /// <summary> 狀態
        /// 
        /// </summary> 
        [Column]
        public int Status { get; set; }

        /// <summary> 聯繫電話
        /// 
        /// </summary> 
        [Column]
        public string ContactPhone { get; set; }

        /// <summary> 最近一次檢查日期
        /// 
        /// </summary> 
        [Column]
        public DateTime? LastCheckDate { get; set; }

        /// <summary> 友善資材站起日
        /// 
        /// </summary> 
        [Column]
        public DateTime? FriendlyStartDate { get; set; }

        /// <summary> 友善資材站迄日
        /// 
        /// </summary> 
        [Column]
        public DateTime? FriendlyEndDate { get; set; }

        /// <summary> 創建時間
        /// 
        /// </summary> 
        [Column]
        public DateTime CreatedAt { get; set; }

        /// <summary> 更新時間
        /// 
        /// </summary> 
        [Column]
        public DateTime? UpdatedAt { get; set; }
        /// <summary> 
        /// 緯度
        /// </summary> 
        [Column(DbType = "decimal(18,8)")]
        public decimal? Latitude { get; set; }

        /// <summary> 
        /// 經度
        /// </summary> 
        [Column(DbType = "decimal(18,8)")]
        public decimal? Longitude { get; set; }

        /// <summary> 更新時間
        /// 
        /// </summary> 
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
        /// <summary> 更新時間
        /// 
        /// </summary> 
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
        /// <summary> 更新時間
        /// 
        /// </summary> 
        public List<string> Center
        {
            get
            {
                if (!string.IsNullOrEmpty(Location) && Location.Split(',').Length == 2)
                {
                    return Location.Split(',').OrderByDescending(x => x).ToList();
                }
                return new List<string>();
            }
        }
        /// <summary> 
        /// 綠色友善資材站
        /// </summary> 
        public bool Friendly
        {
            get
            {
                var _fr = false;
                if (FriendlyStartDate <= DateTime.Now && FriendlyEndDate >= DateTime.Today) _fr = true;
                return _fr;
            }
        }
    }
}
