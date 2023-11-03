using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ICCModule.Models
{
    /// <summary>
    /// 縣市行政區
    /// </summary>
    public class CityDistricts
    {
        public string Name { get; set; }

        public List<District> Districts { get; set; }
    }
}