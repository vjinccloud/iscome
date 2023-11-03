using ICCModule.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Website.Helper;

namespace Website.ViewModel.DiseasePrevention
{
    public class MapViewModel
    {
        public MapViewModel()
        {
            Districts = CommonDataHelper.GetDistricts("高雄市");
            Near = false;
            Friendly = false;
        }

        /// <summary>
        /// 行政區
        /// </summary>
        public List<District> Districts { get; set; }

        /// <summary>
        /// 選定行政區
        /// </summary>
        public string District { get; set; }

        /// <summary>
        /// 查詢我的附近
        /// </summary>
        public bool Near { get; set; }

        /// <summary>
        /// 關鍵字
        /// </summary>
        public string KeyWord { get; set; }

        /// <summary>
        /// 綠色友善資材站
        /// </summary>
        public bool Friendly { get; set; }
    }
}