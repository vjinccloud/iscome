using ICCModule.Entity.Tables;
using ICCModule.Entity.Tables.Platform;
using ICCModule.EntityService.Service;
using ICCModule.Helper;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace InformationSystem.ViewModel
{
    public class MonitorProjectsEditModel
    {
        /// <summary>
        /// 行政區
        /// </summary>
        public List<string> Districts { get; set; } = DistrictHelper.KaohsiungDistrict();
        /// <summary>
        /// 監測頻率
        /// </summary>
        public List<defCode> MonitoeFrequery { get; set; } = Service_defCode.GetList("MonitoeFrequery");
        /// <summary>
        /// 調查對象
        /// </summary>
        public List<defCode> SurveyType { get; set; } = Service_defCode.GetList("SurveyType");

        public MonitorProjectsSaveModel DataModel { get; set; } = new MonitorProjectsSaveModel();
    }
    public class MonitorProjectsSaveModel
    {
        /// <summary> 
        /// 流水號
        /// </summary> 
        public int ID { get; set; }

        /// <summary> 
        /// 專案年度
        /// </summary> 
        public int Year { get; set; } = DateTime.Now.Year;

        /// <summary> 
        /// 專案名稱
        /// </summary> 
        public string Name { get; set; }

        /// <summary>
        /// 監測頻率，對應 defCode 表
        /// </summary> 
        public string Frequency { get; set; }

        /// <summary>
        /// 監測起
        /// </summary> 
        public DateTime StartDate { get; set; } = DateTime.Now;

        /// <summary> 
        /// 監測迄
        /// </summary> 
        public DateTime EndDate { get; set; } = DateTime.Now;

        /// <summary> 
        /// 顯示狀態
        /// </summary> 
        public bool Show { get; set; } = true;
        public List<MonitorProjectAreasEditModel> AreaData { get; set; } = new List<MonitorProjectAreasEditModel>();
    }
    public class MonitorProjectAreasEditModel
    {
        /// <summary> 
        /// 流水號
        /// </summary> 
        public int ID { get; set; }

        /// <summary>
        /// 調查對象類型
        /// </summary> 
        public string Type { get; set; }

        /// <summary>
        /// 行政區
        /// </summary> 
        public string Distist { get; set; }

        /// <summary>
        /// 調查人員
        /// </summary> 
        public string Inspectors { get; set; }

        /// <summary> 
        /// 調查點位數量
        /// </summary> 
        public int Points { get; set; } = 0;
        public DateTime? UpdateAt { get; set; }
        public bool ShowDel { get; set; }
    }
    public class ProjectListShowModel
    {
        public int Year { get; set; }
        public bool ShowBtn { get; set; }
        public List<MonitorProjectViewModel> DataModel { get; set; }
    }
    public class MonitorProjectViewModel
    {
        public int ID { get; set; }
        public string Name { get; set; }
        public string District { get; set; }
        public string Frequency { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public bool ShowDel { get; set; }
    }
    public class MonitorYearViewModel
    {
        public int Years { get; set; }
        public int ProjectCount { get; set; }
        public int DataCount { get; set; }
    }
    #region InvestInfo Model
    public class InvestInfoShowModel
    {
        public InvestInfoShowModel()
        {
            BugTotalModels = new List<BugTotalModel>();
            SeverityModels = new List<SeverityModel>();
        }
        public int mpId { get; set; }
        public int mpaId { get; set; }
        public int Years { get; set; }
        public string ProjectName { get; set; }
        public string Type { get; set; }
        public string Distist { get; set; }
        public List<BugTotalModel> BugTotalModels { get; set; }
        public List<SeverityModel> SeverityModels { get; set; }
    }
    public class BugTotalModel
    {
        /// <summary>
        /// 年分
        /// </summary>
        public int Year { get; set; }
        /// <summary>
        /// 月份
        /// </summary>
        public int Month { get; set; }
        /// <summary>
        /// 蟲數加總
        /// </summary>
        public decimal TotalCount { get; set; }
        /// <summary>
        /// 作物
        /// </summary>
        public string Crops { get; set; }
        /// <summary>
        /// 比例
        /// </summary>
        public decimal VictimRate { get; set; }
        /// <summary>
        /// 其他數量
        /// </summary>
        public decimal OtherCount { get; set; }
        /// <summary>
        /// 顯示複製按鈕
        /// </summary>
        public bool ShowCopy { get; set; } = false;
    }
    public class SeverityModel
    {
        /// <summary>
        /// 年分
        /// </summary>
        public int Year { get; set; }
        /// <summary>
        /// 月份
        /// </summary>
        public int Month { get; set; }
        /// <summary>
        /// 第幾周
        /// </summary>
        public int NowWeek { get; set; }
        /// <summary>
        /// 0級發生面積
        /// </summary>
        public decimal Level_0 { get; set; }
        /// <summary>
        /// 1級發生面積
        /// </summary>
        public decimal Level_1 { get; set; }
        /// <summary>
        /// 2級發生面積
        /// </summary>
        public decimal Level_2 { get; set; }
        /// <summary>
        /// 3級發生面積
        /// </summary>
        public decimal Level_3 { get; set; }
        /// <summary>
        /// 4級發生面積
        /// </summary>
        public decimal Level_4 { get; set; }
        /// <summary>
        /// 嚴重程度
        /// </summary>
        public string Severity { get; set; }
        /// <summary>
        /// 比例
        /// </summary>
        public decimal Scale { get; set; }
        /// <summary>
        /// 顯示複製按鈕
        /// </summary>
        public bool ShowCopy { get; set; }
    }
    #endregion
    #region
    public class RecordShowModel
    {
        public int mpaid { get; set; }
        public int Years { get; set; }
        public int Months { get; set; }
        public string ProjectName { get; set; }
        public List<cropType> cropTypes { get; set; } = Service_cropType.GetEnableList();
        public List<crops> cropDatas { get; set; } = Service_crops.GetEnableList();
        public List<MapdModel> Datas { get; set; }
    }
    public class MapdModel
    {
        public MapdModel()
        {
            Village = "";
            SerialNum = "";
            NorthLatitude = 0;
            EastLongitude = 0;
            CropType = 0;
            Crops = 0;
            Comment = "";
            Pests = 0;
            Surveys = 0;
            Victims = 0;
            CropGrowthPeriod = "";
            SurveyArea = 0;
            DiscoverFAW = false;
            DamageContent = "";
            Adress = "";
            OtherArts = "";
            OtherArtsNum = 0;
            Tempeature = 0;
            Level_0 = 0;
            Level_1 = 0;
            Level_2 = 0;
            Level_3 = 0;
            Level_4 = 0;
            mapdComment = "";
            MonitoringRecord = "";
            DailyRainfall = 0;
        }
        /// <summary>
        /// 流水號
        /// </summary>
        public int ID { get; set; }
        /// <summary>
        /// 里別
        /// </summary>
        public string Village { get; set; }
        /// <summary>
        /// 編號
        /// </summary>
        public string SerialNum { get; set; }
        /// <summary>
        /// GPS-北緯
        /// </summary>
        public decimal NorthLatitude { get; set; }
        /// <summary>
        /// GPS-東經
        /// </summary>
        public decimal EastLongitude { get; set; }
        /// <summary> 
        /// 作物類別
        /// </summary> 
        public int CropType { get; set; }

        /// <summary>
        /// 作物
        /// </summary>
        public int Crops { get; set; }
        /// <summary>
        /// 備註
        /// </summary>
        public string Comment { get; set; }

        /// <summary>
        /// 流水號
        /// </summary> 
        public int mapdID { get; set; }

        /// <summary>
        /// 日期
        /// </summary> 
        public DateTime Date { get; set; }

        /// <summary>
        /// 害蟲數
        /// </summary> 
        public decimal Pests { get; set; }

        /// <summary>
        /// 調查株樹
        /// </summary> 
        public int Surveys { get; set; }

        /// <summary>
        /// 受害株樹
        /// </summary> 
        public int Victims { get; set; }

        /// <summary> 
        /// 作物生長期
        /// </summary> 
        public string CropGrowthPeriod { get; set; }

        /// <summary> 
        /// 調查面積
        /// </summary> 
        public decimal SurveyArea { get; set; }

        /// <summary>
        /// 發現FAW
        /// </summary> 
        public bool DiscoverFAW { get; set; }

        /// <summary> 
        /// 受害程度(概述)
        /// </summary> 
        public string DamageContent { get; set; }

        /// <summary> 
        /// 地址
        /// </summary> 
        public string Adress { get; set; }

        /// <summary> 
        /// 其他蟻種資訊
        /// </summary> 
        public string OtherArts { get; set; }

        /// <summary> 
        /// 其他蟻種數量
        /// </summary> 
        public decimal OtherArtsNum { get; set; }

        /// <summary> 
        /// 溫度
        /// </summary> 
        public decimal Tempeature { get; set; }

        /// <summary>
        /// 0級發生面積(公頃)
        /// </summary> 
        public decimal Level_0 { get; set; }

        /// <summary>
        /// 1級發生面積(公頃)
        /// </summary> 
        public decimal Level_1 { get; set; }

        /// <summary>
        /// 2級發生面積(公頃)
        /// </summary> 
        public decimal Level_2 { get; set; }

        /// <summary>
        /// 3級發生面積(公頃)
        /// </summary> 
        public decimal Level_3 { get; set; }

        /// <summary> 
        /// 4級發生面積(公頃)
        /// </summary> 
        public decimal Level_4 { get; set; }

        /// <summary>
        /// 備註
        /// </summary> 
        public string mapdComment { get; set; }

        /// <summary>
        /// 監測情形紀錄
        /// </summary> 
        public string MonitoringRecord { get; set; }

        /// <summary>
        /// 日累積雨量
        /// </summary> 
        public int DailyRainfall { get; set; }
        public List<FileManagement> OldFiles { get; set; } = new List<FileManagement>();
        public List<long> EditFiles { get; set; } = new List<long>();
        public List<HttpPostedFileBase> PictureFiles { get; set; } = new List<HttpPostedFileBase>();
    }
    #endregion


    public class AutoWeatherStationTypeResponse
    {
        public string RS { get; set; }
        public List<AutoWeatherStationType> Data { get; set; }
        public bool Next { get; set; }
    }

    public class AutoWeatherStationType
    {
        public string Station_name { get; set; }
        public string Station_ID { get; set; }
        public DateTime? TIME { get; set; }
        public string TEMP { get; set; }
        public string H_24R { get; set; }
        public string TOWN { get; set; }
    }
    public class TownSnData
    {
        public string TOWN { get; set; }
        public string TOWN_SN { get; set; }
    }

}