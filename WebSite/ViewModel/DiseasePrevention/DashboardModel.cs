using ICCModule.EntityService.Service;
using ICCModule.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Website.Helper;

namespace Website.ViewModel.DiseasePrevention
{
    public class DashboardModel
    {
        /// <summary>
        /// 行政區
        /// </summary>
        public List<District> Districts { get; set; } = CommonDataHelper.GetDistricts("高雄市");

        /// <summary>
        /// 搜尋條件
        /// </summary>
        public DashboardQueryModel QueryData { get; set; } = new DashboardQueryModel();
        /// <summary>
        /// 作物
        /// </summary>
        public List<DashboardCrop> CropList { get; set; } = Service_doctorSchedule.GetList(x => true).GroupBy(x => x.CropType).Select(x => new DashboardCrop { CropTypes = x.Key,CropNames = x.Select(o => o.CropName).Distinct().ToList()}).ToList();

        public List<int> DataCount { get; set; } = new List<int>();
        public List<string> Label { get; set; } = new List<string>();
        public List<CropRank> CropRanks { get; set; } = new List<CropRank>();
        public List<DashboardMap> DashboardMapData { get; set; } = new List<DashboardMap>();
        public string Center { get; set; }
        public bool IsSearch { get; set; } = false;

    }
    public class DashboardQueryModel
    {
        /// <summary>
        /// 起始時間
        /// </summary>
        public DateTime? StartDate { get; set; }
        
        /// <summary>
        /// 結束時間
        /// </summary>
        public DateTime? EndDate { get; set; }

        /// <summary>
        /// 行政區
        /// </summary>
        public string Zip { get; set; }
        /// <summary>
        /// 作物類別
        /// </summary>
        public string CropType { get; set; }
        /// <summary>
        /// 作物
        /// </summary>
        public string Crops { get; set; }
    }
    public class DashboardCrop
    {        
        /// <summary>
        /// 作物類別
        /// </summary>
        public string CropTypes { get; set; }
        /// <summary>
        /// 作物
        /// </summary>
        public List<string> CropNames { get; set; }
    }
    public class CropRank
    {
        public int Rank { get; set; }
        public string CropName { get; set; }
        public int DataCount { get; set; }
    }
    public class DashboardMap
    {
        public string DistrictName { get; set; }
        public string PopupWord { get; set; }
        public decimal Lng { get; set; }
        public decimal Lat { get; set; }
        public int TotalCount { get; set; }
    }
    public class ZipLngLat
    {
        public string Zip { get; set; }
        public decimal Lng { get; set; }
        public decimal Lat { get; set; }
    }
}