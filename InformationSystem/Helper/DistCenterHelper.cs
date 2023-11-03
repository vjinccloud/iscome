using System.Collections.Generic;
using System.Linq;

namespace InformationSystem.Helper
{

    /// <summary>
    /// 取得行政區中心點
    /// </summary>
    public static class DistCenterHelper
    {
        private static readonly Dictionary<string, DistCenterPoint> distMap;

        private const string SRS = "EPSG:4326";

        static DistCenterHelper()
        {
            distMap = new Dictionary<string, DistCenterPoint>
            {
                { "新興區", new DistCenterPoint("新興區", 22.62992906m, 120.3067337m, SRS) },
                { "前金區", new DistCenterPoint("前金區", 22.6269905m, 120.2944217m, SRS) },
                { "苓雅區", new DistCenterPoint("苓雅區", 22.62359448m, 120.3209103m, SRS) },
                { "鹽埕區", new DistCenterPoint("鹽埕區", 22.62424585m, 120.2842331m, SRS) },
                { "鼓山區", new DistCenterPoint("鼓山區", 22.65019525m, 120.274163m, SRS) },
                { "旗津區", new DistCenterPoint("旗津區", 22.58565583m, 120.2891539m, SRS) },
                { "前鎮區", new DistCenterPoint("前鎮區", 22.59269724m, 120.3146749m, SRS) },
                { "三民區", new DistCenterPoint("三民區", 22.64989883m, 120.3179187m, SRS) },
                { "楠梓區", new DistCenterPoint("楠梓區", 22.72109961m, 120.300758m, SRS) },
                { "小港區", new DistCenterPoint("小港區", 22.55140207m, 120.3592605m, SRS) },
                { "左營區", new DistCenterPoint("左營區", 22.68395699m, 120.2951588m, SRS) },
                { "仁武區", new DistCenterPoint("仁武區", 22.70120782m, 120.3605265m, SRS) },
                { "大社區", new DistCenterPoint("大社區", 22.73983479m, 120.3707994m, SRS) },
                { "岡山區", new DistCenterPoint("岡山區", 22.80505886m, 120.2978906m, SRS) },
                { "路竹區", new DistCenterPoint("路竹區", 22.85724171m, 120.2659871m, SRS) },
                { "阿蓮區", new DistCenterPoint("阿蓮區", 22.87022883m, 120.3210967m, SRS) },
                { "田寮區", new DistCenterPoint("田寮區", 22.86394307m, 120.3959842m, SRS) },
                { "燕巢區", new DistCenterPoint("燕巢區", 22.78769626m, 120.370799m, SRS) },
                { "橋頭區", new DistCenterPoint("橋頭區", 22.75252398m, 120.3006534m, SRS) },
                { "梓官區", new DistCenterPoint("梓官區", 22.748209m, 120.2593989m, SRS) },
                { "彌陀區", new DistCenterPoint("彌陀區", 22.77944528m, 120.2394571m, SRS) },
                { "永安區", new DistCenterPoint("永安區", 22.82224585m, 120.228051m, SRS) },
                { "湖內區", new DistCenterPoint("湖內區", 22.89324952m, 120.2259375m, SRS) },
                { "鳳山區", new DistCenterPoint("鳳山區", 22.61379251m, 120.3554359m, SRS) },
                { "大寮區", new DistCenterPoint("大寮區", 22.59283576m, 120.4111468m, SRS) },
                { "林園區", new DistCenterPoint("林園區", 22.50813743m, 120.399052m, SRS) },
                { "鳥松區", new DistCenterPoint("鳥松區", 22.66249302m, 120.3727783m, SRS) },
                { "大樹區", new DistCenterPoint("大樹區", 22.71100364m, 120.425407m, SRS) },
                { "旗山區", new DistCenterPoint("旗山區", 22.86497033m, 120.4754554m, SRS) },
                { "美濃區", new DistCenterPoint("美濃區", 22.90005529m, 120.5634635m, SRS) },
                { "六龜區", new DistCenterPoint("六龜區", 23.01195426m, 120.6585635m, SRS) },
                { "內門區", new DistCenterPoint("內門區", 22.95668817m, 120.4719272m, SRS) },
                { "杉林區", new DistCenterPoint("杉林區", 22.99694681m, 120.5621971m, SRS) },
                { "甲仙區", new DistCenterPoint("甲仙區", 23.11654995m, 120.6232895m, SRS) },
                { "桃源區", new DistCenterPoint("桃源區", 23.2249459m, 120.8523383m, SRS) },
                { "那瑪夏區", new DistCenterPoint("那瑪夏區", 23.275008m, 120.741944m, SRS) },
                { "茂林區", new DistCenterPoint("茂林區", 22.91993256m, 120.752384m, SRS) },
                { "茄萣區", new DistCenterPoint("茄萣區", 22.88241399m, 120.1980519m, SRS) },
                { "東沙群島", new DistCenterPoint("東沙群島", 20.705842m, 116.906984m, SRS) },
                { "南沙群島", new DistCenterPoint("茄萣區", 10.724232m, 115.812406m, SRS) },
            };
        }

        /// <summary>
        /// 取得行政區中心點
        /// </summary>
        /// <param name="distNames">行政區名稱清單</param>
        /// <returns></returns>
        public static List<DistCenterPoint> GetDistCenter(params string[] distNames)
        {
            var result = new List<DistCenterPoint>();
            foreach (var distName in distNames)
            {
                if (GetDistCenter(distName) is DistCenterPoint target)
                {
                    result.Add(target);
                }
            }

            return result;
        }

        /// <summary>
        /// 取得所有行政區資料
        /// </summary>
        /// <returns></returns>
        public static List<DistCenterPoint> GetAllDist() => distMap.Values.ToList();

        /// <summary>
        /// 取得行政區中心點
        /// </summary>
        /// <param name="distName">行政區名稱</param>
        /// <returns></returns>
        public static DistCenterPoint GetDistCenter(string distName)
            => distMap.TryGetValue(distName.Trim(), out var target) ? target : null;
    }


    /// <summary>
    /// 行政區的中心點位資料
    /// </summary>
    public class DistCenterPoint
    {
        public DistCenterPoint(string name, decimal lat, decimal lng, string srs)
        {
            Name = name;
            Lat = lat;
            Lng = lng;
            Srs = srs;
        }

        /// <summary>
        /// 名稱
        /// </summary>
        public string Name { get; }

        /// <summary>
        /// 緯度
        /// </summary>
        public decimal Lat { get; }

        /// <summary>
        /// 經度
        /// </summary>
        public decimal Lng { get; }

        /// <summary>
        /// 參考坐標系
        /// </summary>
        public string Srs { get; }
    }
}