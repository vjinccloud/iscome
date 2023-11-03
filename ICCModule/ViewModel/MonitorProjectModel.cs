using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ICCModule.ViewModel
{
    public class MonitorProjectModel
    {
        /// <summary> 
        /// 專案名稱
        /// </summary> 
        public string ProjectName { get; set; }
        /// <summary> 
        /// 行政區
        /// </summary> 
        public string Distist { get; set; }


        /// <summary> 里別
        /// 
        /// </summary> 
        public string Village { get; set; }
        /// <summary> 編號
        /// 
        /// </summary> 
        public string SerialNum { get; set; }
        /// <summary> GPS-北緯
        /// 
        /// </summary> 
        public decimal NorthLatitude { get; set; }
        /// <summary> GPS-東經
        /// 
        /// </summary> 
        public decimal EastLongitude { get; set; }
        /// <summary> 
        /// 作物類別
        /// </summary> 
        public string CropType { get; set; }
        /// <summary> 作物(多筆)
        /// 
        /// </summary> 
        public string Crops { get; set; }


        /// <summary> 日期
        /// 
        /// </summary> 
        public DateTime Date { get; set; }

        /// <summary> 害蟲數
        /// 
        /// </summary> 
        public decimal Pests { get; set; }

        /// <summary> 調查株樹
        /// 
        /// </summary> 
        public int Surveys { get; set; }

        /// <summary> 受害株樹
        /// 
        /// </summary> 
        public int Victims { get; set; }

        /// <summary> 作物生長期
        /// 
        /// </summary> 
        public string CropGrowthPeriod { get; set; }

        /// <summary> 調查面積
        /// 
        /// </summary> 
        public decimal SurveyArea { get; set; }

        /// <summary> 發現FAW
        /// 
        /// </summary> 
        public bool DiscoverFAW { get; set; }

        /// <summary> 受害程度(概述)
        /// 
        /// </summary> 
        public string DamageContent { get; set; }

        /// <summary> 地址
        /// 
        /// </summary> 
        public string Adress { get; set; }

        /// <summary> 其他蟻種資訊
        /// 
        /// </summary> 
        public string OtherArts { get; set; }

        /// <summary> 其他蟻種數量
        /// 
        /// </summary> 
        public decimal OtherArtsNum { get; set; }

        /// <summary> 溫度
        /// 
        /// </summary> 
        public decimal Tempeature { get; set; }

        /// <summary> 0級發生面積(公頃)
        /// 
        /// </summary> 
        public decimal Level_0 { get; set; }

        /// <summary> 1級發生面積(公頃)
        /// 
        /// </summary> 
        public decimal Level_1 { get; set; }

        /// <summary> 2級發生面積(公頃)
        /// 
        /// </summary> 
        public decimal Level_2 { get; set; }

        /// <summary> 3級發生面積(公頃)
        /// 
        /// </summary> 
        public decimal Level_3 { get; set; }

        /// <summary> 4級發生面積(公頃)
        /// 
        /// </summary> 
        public decimal Level_4 { get; set; }

        /// <summary> 備註
        /// 
        /// </summary> 
        public string Comment { get; set; }
    }
    public class MonitorProjectSelectModel
    {
        /// <summary> 
        /// 行政區
        /// </summary> 
        public string Distist { get; set; }

        /// <summary> 里別
        /// 
        /// </summary> 
        public string Village { get; set; }
        /// <summary> 編號
        /// 
        /// </summary> 
        public string SerialNum { get; set; }
        /// <summary> 作物(多筆)
        /// 
        /// </summary> 
        public string Crops { get; set; }
        /// <summary> 害蟲數
        /// 
        /// </summary> 
        public decimal Pests { get; set; }

        /// <summary> 監測起
        /// 
        /// </summary> 
        public DateTime StartDate { get; set; }

        /// <summary> 監測迄
        /// 
        /// </summary> 
        public DateTime EndDate { get; set; }

    }
}
