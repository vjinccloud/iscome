using ICCModule.EntityService.Service;
using ICCModule.Helper;
using System;
using System.Data.Linq.Mapping;

namespace ICCModule.Entity.Tables.Platform
{
    [Serializable]
    [Table(Name = "pesticide_Residue_Testing")]
    public class pesticideResidueTesting
    {
        /// <summary> 流水號
        /// 
        /// </summary> 
        [Column(IsPrimaryKey = true, IsDbGenerated = true, AutoSync = AutoSync.OnInsert)]
        public int ID { get; set; }

        /// <summary> 年度
        /// 
        /// </summary> 
        [Column]
        public string Year { get; set; }

        /// <summary> 計畫別
        /// 
        /// </summary> 
        [Column]
        public string PlanType { get; set; }

        /// <summary> 計畫別-其他-備註
        /// 
        /// </summary> 
        [Column]
        public string PlanComment { get; set; }

        /// <summary> 檢體編號
        /// 
        /// </summary> 
        [Column]
        public string SampleNum { get; set; }

        /// <summary> 採樣日期
        /// 
        /// </summary> 
        [Column]
        public DateTime SampleDate { get; set; }

        /// <summary> 縣市
        /// 
        /// </summary> 
        [Column]
        public string City { get; set; }

        /// <summary> 行政區
        /// 
        /// </summary> 
        [Column]
        public string District { get; set; }

        /// <summary> 樣品名稱
        /// 
        /// </summary> 
        [Column]
        public string SampleName { get; set; }

        /// <summary>
        /// 作物類別
        /// </summary> 
        [Column]
        public string CropType { get; set; }
        public string CropTypeStr { 
            get
            {
                var _cType = Service_cropType.GetDetail(CropType.ToInt32());
                if (_cType!=null)
                {
                    return _cType.Name;
                }
                return "";
            }
        }

        /// <summary> 作物名稱
        /// 
        /// </summary> 
        [Column]
        public string CropName { get; set; }
        public string CropNameStr
        {
            get
            {
                var _cType = Service_crops.GetDetail(CropName.ToInt32());
                if (_cType != null)
                {
                    return _cType.Name;
                }
                return "";
            }
        }

        /// <summary> 生產者/業者名稱
        /// 
        /// </summary> 
        [Column]
        public string ProviderName { get; set; }

        /// <summary> 供應代號
        /// 
        /// </summary> 
        [Column]
        public string ProviderCode { get; set; }

        /// <summary>
        /// 生產者/業者縣市
        /// </summary> 
        [Column]
        public string ProviderCity { get; set; }

        /// <summary>
        /// 生產者/業者行政區
        /// </summary> 
        [Column]
        public string ProviderDistrict { get; set; }

        /// <summary> 生產者/業者地址
        /// 
        /// </summary> 
        [Column]
        public string ProviderAddress { get; set; }

        /// <summary> 連絡電話
        /// 
        /// </summary> 
        [Column]
        public string ProviderPhone { get; set; }

        /// <summary> 採樣來源，對應到 defCode 表
        /// 
        /// </summary> 
        [Column]
        public string SampleSource { get; set; }

        /// <summary> 採樣地點
        /// 
        /// </summary> 
        [Column]
        public string SampleLocation { get; set; }

        /// <summary> 提供單位，對應到 defCode 表
        /// 
        /// </summary> 
        [Column]
        public string ProvideUnit { get; set; }

        /// <summary> 
        /// 檢驗結果
        /// </summary> 
        [Column]
        public bool SampleResult { get; set; }

        /// <summary> 檢驗內容
        /// 
        /// </summary> 
        [Column]
        public string SampleContext { get; set; }

        /// <summary> 結果判定
        /// 
        /// </summary> 
        [Column]
        public bool? Result { get; set; }

        /// <summary> 處理情形
        /// 
        /// </summary> 
        [Column]
        public int HandingSituation { get; set; }

        /// <summary> 結案處理情形
        /// 
        /// </summary> 
        [Column]
        public int? ClosingSituation { get; set; }

        /// <summary> 罰款金額
        /// 
        /// </summary> 
        [Column]
        public int? Penalty { get; set; }

        /// <summary> 分期
        /// 
        /// </summary> 
        [Column]
        public bool? Staging { get; set; }

        /// <summary> 繳費期限
        /// 
        /// </summary> 
        [Column]
        public DateTime? PaymentDeadline { get; set; }

        /// <summary> 繳費狀態
        /// 
        /// </summary> 
        [Column]
        public bool? PaymentStatus { get; set; }

        /// <summary> 
        /// 分期資料
        /// </summary> 
        [Column]
        public string StagingData { get; set; }

        /// <summary> 強制移送日期
        /// 
        /// </summary> 
        [Column]
        public DateTime? TransferDate { get; set; }

        /// <summary> 結案發文日期
        /// 
        /// </summary> 
        [Column]
        public DateTime? ClosingDate { get; set; }

        /// <summary> 結案發文文號
        /// 
        /// </summary> 
        [Column]
        public string ClosingNumber { get; set; }

        /// <summary> 結案說明
        /// 
        /// </summary> 
        [Column]
        public string ClosingInstructions { get; set; }

        /// <summary> 是否轉單
        /// 
        /// </summary> 
        [Column]
        public bool? IsTransfer { get; set; }
        
        /// <summary> 轉單日期
        /// 
        /// </summary> 
        [Column]
        public DateTime? TransferCounselingDate { get; set; }


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
        /// 採樣編號
        /// </summary>
        [Column]
        public string SampleID { get; set; }
        /// <summary>
        /// 
        /// </summary>
        [Column]
        public string FastSampleID { get; set; }
        /// <summary>
        /// 會同單位
        /// </summary>
        [Column]
        public string JointUnit { get; set; }

        /// <summary> 
        /// 分析日期
        /// </summary> 
        [Column]
        public DateTime? AnalyzeDate { get; set; }

        /// <summary> 
        /// 供應名稱
        /// </summary> 
        [Column]
        public string ProviderUnit { get; set; }
    }
}
