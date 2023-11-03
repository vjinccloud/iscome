using ICCModule.Models;
using InformationSystem.Helper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace InformationSystem.ViewModel.TAP
{
    public class MapViewModel
    {
        public MapViewModel()
        {
            Districts = CommonDataHelper.GetDistricts("高雄市");
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
        /// 農產品經營業者
        /// </summary>
        public string VendorName { get; set; }
    }
}