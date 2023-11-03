using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Configuration;
using System.Net.Mail;
using ICCModule.Helper;

namespace ICCModule.Helper
{
    /// <summary>
    /// 行政區
    /// </summary>
    public static partial class DistrictHelper
    {
        /// <summary>
        /// 高雄行政區
        /// </summary>
        /// <returns></returns>
        public static List<string> KaohsiungDistrict()
        {
            var _res = new List<string>();
            _res.Add("楠梓區"); _res.Add("左營區"); _res.Add("鼓山區"); _res.Add("三民區");
            _res.Add("鹽埕區"); _res.Add("前金區"); _res.Add("新興區"); _res.Add("苓雅區");
            _res.Add("前鎮區"); _res.Add("旗津區"); _res.Add("小港區"); _res.Add("鳳山區");
            _res.Add("大寮區"); _res.Add("鳥松區"); _res.Add("林園區"); _res.Add("仁武區");
            _res.Add("大樹區"); _res.Add("大社區"); _res.Add("岡山區"); _res.Add("路竹區");
            _res.Add("橋頭區"); _res.Add("梓官區"); _res.Add("彌陀區"); _res.Add("永安區");
            _res.Add("燕巢區"); _res.Add("田寮區"); _res.Add("阿蓮區"); _res.Add("茄萣區");
            _res.Add("湖內區"); _res.Add("旗山區"); _res.Add("美濃區"); _res.Add("內門區");
            _res.Add("杉林區"); _res.Add("甲仙區"); _res.Add("六龜區"); _res.Add("茂林區");
            _res.Add("桃源區"); _res.Add("那瑪夏區");
            return _res;
        }
    }
}
