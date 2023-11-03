using ICCModule.Entity.Tables;
using ICCModule.Entity.Tables.Platform;
using ICCModule.EntityService.Service;
using ICCModule.Helper;
using ICCModule.ViewModel;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace InformationSystem.ViewModel
{
    public class PesticideResidueModel
    {
        public PesticideResidueModel()
        {
            Districts = DistrictHelper.KaohsiungDistrict();
            prQueryReq = new PesticideResidueQueryReq();
            PlanType = Service_defCode.GetList("PlanType");
            ProvideUnit = Service_defCode.GetList("ProvideUnit");
            SampleSource = Service_defCode.GetList("SampleSource");
            PlantCategory = Service_cropType.GetList(x => x.Enable);
            CropName = Service_crops.GetEnableList();
            prModel = new List<pesticideResidueTesting>();
        }
        /// <summary>
        /// 搜尋條件
        /// </summary>
        public PesticideResidueQueryReq prQueryReq { get; set; }
        /// <summary>
        /// 行政區
        /// </summary>
        public List<string> Districts { get; set; }
        /// <summary>
        /// 計畫別
        /// </summary>
        public List<defCode> PlanType { get; set; }
        /// <summary>
        /// 提供單位
        /// </summary>
        public List<defCode> ProvideUnit { get; set; }
        /// <summary>
        /// 採樣來源
        /// </summary>
        public List<defCode> SampleSource { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public List<cropType> PlantCategory { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public List<crops> CropName { get; set; }
        public List<pesticideResidueTesting> prModel { get; set; }
        /// <summary> 
        /// 總頁數
        /// </summary> 
        public int TotalPage { get; set; } = 1;
        /// <summary> 
        /// 頁數
        /// </summary> 
        public int Pages { get; set; } = 1;
    }
    public class PesticideResidueQueryReq
    {
        public PesticideResidueQueryReq()
        {
            HandingSituation = 0;
            SampleContextUseWay = new List<string>();
        }
        /// <summary>
        /// 採樣日期-起
        /// </summary>
        public DateTime? SampleStartDate { get; set; }
        /// <summary>
        /// 採樣日期-迄
        /// </summary>
        public DateTime? SampleEndDate { get; set; }
        /// <summary>
        /// 計畫別
        /// </summary>
        public string PlanType { get; set; }
        /// <summary>
        /// 行政區
        /// </summary>
        public string District { get; set; }
        /// <summary>
        /// 提供單位
        /// </summary>
        public string ProvideUnit { get; set; }
        /// <summary>
        /// 作物類別
        /// </summary>
        public string CropType { get; set; }
        /// <summary>
        /// 作物名稱
        /// </summary>
        public string CropName { get; set; }
        /// <summary>
        /// 結果判定
        /// </summary>
        public int? Result { get; set; }
        /// <summary>
        /// 處理情形
        /// </summary>
        public int HandingSituation { get; set; }
        /// <summary>
        /// 結果處理情形
        /// </summary>
        public int? ClosingSituation { get; set; }
        /// <summary>
        /// 樣品名稱
        /// </summary>
        public string SampleName { get; set; }
        /// <summary>
        /// 農藥名稱
        /// </summary>
        public string SampleContextName { get; set; }
        /// <summary>
        /// 使用方法
        /// </summary>
        public List<string> SampleContextUseWay { get; set; }
        /// <summary>
        /// 檢體編號
        /// </summary>
        public string SampleNum { get; set; }
        /// <summary>
        /// 供應代號
        /// </summary>
        public string ProviderCode { get; set; }
        /// <summary>
        /// 轉單植醫輔導
        /// </summary>
        public bool? IsTransfer { get; set; }
        /// <summary>
        /// 採樣來源
        /// </summary>
        public string SampleSource { get; set; }
        /// <summary> 
        /// 頁數
        /// </summary> 
        public int Pages { get; set; } = 1;
    }
    public class PesticideResidueEditModel
    {
        public PesticideResidueEditModel()
        {
            Districts = DistrictHelper.KaohsiungDistrict();
            prSaveModels = new PesticideResidueSaveModel();
            PlanType = Service_defCode.GetList("PlanType");
            ProvideUnit = Service_defCode.GetList("ProvideUnit");
            SampleSource = Service_defCode.GetList("SampleSource");
            PlantCategory = Service_cropType.GetList(x => x.Enable);
            PlantCrop = Service_crops.GetEnableList();
        }
        public PesticideResidueSaveModel prSaveModels { get; set; }
        public List<string> Districts { get; set; }
        public List<defCode> PlanType { get; set; }
        public List<defCode> ProvideUnit { get; set; }
        public List<defCode> SampleSource { get; set; }
        public List<cropType> PlantCategory { get; set; }
        public List<crops> PlantCrop { get; set; }
    }
    public class PesticideResidueSaveModel
    {
        public PesticideResidueSaveModel()
        {
            ID = 0;
            Year = (DateTime.Now.Year - 1911).ToString();
            PlanType = "";
            PlanComment = "";
            SampleNum = "";
            SampleDate = DateTime.Now;
            District = "";
            SampleName = "";
            CropName = "";
            ProviderName = "";
            ProviderCode = "";
            ProviderDistrict = "";
            ProviderAddress = "";
            ProviderPhone = "";
            SampleSource = "";
            SampleLocation = "";
            ProvideUnit = "";
            SampleResult = false;
            SampleContext = new List<SampleContext>();
            StagingDatas = new List<StagingData>();
            HandingSituation = 0;
            ClosingNumber = "";
            ClosingInstructions = "";

        }
        /// <summary>
        /// 流水號
        /// </summary> 
        public int ID { get; set; }

        /// <summary>
        /// 年度
        /// </summary> 
        public string Year { get; set; }

        /// <summary> 
        /// 計畫別
        /// </summary> 
        public string PlanType { get; set; }

        /// <summary> 
        /// 計畫別-其他-備註
        /// </summary> 
        public string PlanComment { get; set; }

        /// <summary> 
        /// 檢體編號
        /// </summary> 
        public string SampleNum { get; set; }

        /// <summary>
        /// 採樣日期
        /// </summary> 
        public DateTime SampleDate { get; set; }

        /// <summary>
        /// 縣市
        /// </summary> 
        public string City { get; set; }

        /// <summary>
        /// 行政區
        /// </summary> 
        public string District { get; set; }

        /// <summary>
        /// 樣品名稱
        /// </summary> 
        public string SampleName { get; set; }

        /// <summary>
        /// 作物類別
        /// </summary> 
        public string CropType { get; set; }

        /// <summary>
        /// 作物名稱
        /// </summary> 
        public string CropName { get; set; }

        /// <summary> 
        /// 生產者/業者名稱
        /// </summary> 
        public string ProviderName { get; set; }

        /// <summary>
        /// 供應代號
        /// </summary> 
        public string ProviderCode { get; set; }

        /// <summary>
        /// 生產者/業者縣市
        /// </summary> 
        public string ProviderCity { get; set; }

        /// <summary>
        /// 生產者/業者行政區
        /// </summary> 
        public string ProviderDistrict { get; set; }

        /// <summary>
        /// 生產者/業者地址
        /// </summary> 
        public string ProviderAddress { get; set; }

        /// <summary>
        /// 連絡電話
        /// </summary> 
        public string ProviderPhone { get; set; }

        /// <summary> 
        /// 採樣來源，對應到 defCode 表
        /// </summary> 
        public string SampleSource { get; set; }

        /// <summary> 
        /// 採樣地點
        /// </summary> 
        public string SampleLocation { get; set; }

        /// <summary>
        /// 提供單位，對應到 defCode 表
        /// </summary> 
        public string ProvideUnit { get; set; }

        /// <summary> 
        /// 檢驗結果
        /// </summary> 
        public bool SampleResult { get; set; }

        /// <summary> 
        /// 檢驗內容
        /// </summary> 
        public List<SampleContext> SampleContext { get; set; }

        /// <summary>
        /// 結果判定
        /// </summary> 
        public bool? Result { get; set; }

        /// <summary>
        /// 處理情形
        /// </summary> 
        public int HandingSituation { get; set; }

        /// <summary>
        /// 結案處理情形
        /// </summary> 
        public int? ClosingSituation { get; set; }

        /// <summary>
        /// 罰款金額
        /// </summary> 
        public int? Penalty { get; set; }

        /// <summary> 
        /// 分期
        /// </summary> 
        public bool? Staging { get; set; }

        /// <summary>
        /// 繳費期限
        /// </summary> 
        public DateTime? PaymentDeadline { get; set; }

        /// <summary> 
        /// 繳費狀態
        /// </summary> 
        public bool? PaymentStatus { get; set; }

        /// <summary> 
        /// 分期資料
        /// </summary> 
        public List<StagingData> StagingDatas { get; set; }

        /// <summary>
        /// 強制移送日期
        /// </summary> 
        public DateTime? TransferDate { get; set; }

        /// <summary> 
        /// 結案發文日期
        /// </summary> 
        public DateTime? ClosingDate { get; set; }

        /// <summary> 
        /// 結案發文文號
        /// </summary> 
        public string ClosingNumber { get; set; }

        /// <summary> 
        /// 結案說明
        /// </summary> 
        public string ClosingInstructions { get; set; }

        /// <summary> 
        /// 是否轉單
        /// </summary> 
        public bool? IsTransfer { get; set; }
        /// <summary> 
        /// 轉單日期
        /// </summary> 
        public DateTime? TransferCounselingDate { get; set; }

        /// <summary>
        /// 採樣編號
        /// </summary>
        public string SampleID { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string FastSampleID { get; set; }
        /// <summary>
        /// 會同單位
        /// </summary>
        public string JointUnit { get; set; }

        /// <summary> 
        /// 分析日期
        /// </summary> 
        public DateTime? AnalyzeDate { get; set; }
        /// <summary>
        /// 供應名稱
        /// </summary>
        public string ProviderUnit { get; set; }
    }

    public class SampleContext
    {
        /// <summary>
        /// 農藥名稱
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// 農藥英文名稱
        /// </summary>
        public string EnName { get; set; }
        /// <summary>
        /// 殘留量
        /// </summary>
        public decimal? RemainQty { get; set; }
        /// <summary>
        /// 容許量
        /// </summary>
        public decimal? AllowQty { get; set; }
        /// <summary>
        /// 使用方法
        /// </summary>
        public string UseWay { get; set; }
    }
    public class StagingData
    {
        /// <summary>
        /// 期數
        /// </summary>
        public int StagingCount { get; set; }
        /// <summary>
        /// 繳交金額
        /// </summary>
        public int? PayAmt { get; set; }
        /// <summary>
        /// 繳費期限
        /// </summary>
        public DateTime? PayLimitDate { get; set; }
        /// <summary>
        /// 是否完成繳費
        /// </summary>
        public bool? IsPay { get; set; }
    }
}