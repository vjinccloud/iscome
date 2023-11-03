using ICCModule.Entity.Tables;
using ICCModule.Entity.Tables.Platform;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Website.ViewModel
{
    public class PesticidesInfoShowModel
    {
        /// <summary> 
        /// 查詢條件
        /// </summary> 
        public PesticidesInfoQuery QueryModel { get; set; }
        public List<PesticideInfoModel> DataModel { get; set; }
        public bool IsLogin { get; set; }

    }
    public class PesticidesInfoQuery
    {
        /// <summary> 
        /// 許可證號
        /// </summary> 
        public string Permit { get; set; }

        /// <summary> 
        /// 農藥中文名稱
        /// </summary> 
        public string ChineseName { get; set; }

        /// <summary> 
        /// 廠商名稱
        /// </summary> 
        public string CompanyName { get; set; }
    }

    public class PesticideInfoModel
    {
        /// <summary>
        /// 許可證字
        /// </summary>
        /// <value></value>
        public string LicenseWord { get; set; }
        /// <summary>
        /// 許可證號
        /// </summary>
        /// <value></value>
        public string LicenseNumber { get; set; }
        /// <summary>
        /// 中文名稱
        /// </summary>
        /// <value></value>
        public string ChineseName { get; set; }
        /// <summary>
        /// 農藥代號
        /// </summary>
        /// <value></value>
        public string PesticideCode { get; set; }
        /// <summary>
        /// 英文名稱
        /// </summary>
        /// <value></value>
        public string EnglishName { get; set; }
        /// <summary>
        /// 廠牌名稱
        /// </summary>
        /// <value></value>
        public string BrandName { get; set; }
        /// <summary>
        /// 化學成分
        /// </summary>
        /// <value></value>
        public string ChemicalComposition { get; set; }
        /// <summary>
        /// 國外原製造廠商
        /// </summary>
        /// <value></value>
        public string Manufacturer { get; set; }
        /// <summary>
        /// 劑型
        /// </summary>
        /// <value></value>
        public string DosageForm { get; set; }
        /// <summary>
        /// 含量
        /// </summary>
        /// <value></value>
        public string Content { get; set; }
        /// <summary>
        /// 有效期限
        /// </summary>
        /// <value></value>
        public string ValidityPeriod { get; set; }
        /// <summary>
        /// 廠商名稱
        /// </summary>
        /// <value></value>
        public string TradeName { get; set; }
        /// <summary>
        /// FRAC殺菌劑抗藥性
        /// </summary>
        /// <value></value>
        public string FRACFungicideResistance { get; set; }
        /// <summary>
        /// HRAC除草劑抗藥性
        /// </summary>
        /// <value></value>
        public string HRACHerbicideResistance { get; set; }
        /// <summary>
        /// IRAC殺蟲劑抗藥性
        /// </summary>
        /// <value></value>
        public string IRACInsecticideResistance { get; set; }
        /// <summary>
        /// 農藥使用範圍
        /// </summary>
        /// <value></value>
        public string PesticideApplicationRange { get; set; }
        /// <summary>
        /// 許可證標示
        /// </summary>
        /// <value></value>
        public string LicenseMarking { get; set; }
        public bool IsAlready { get; set; } = false;
    }
    public class PesticidesInventoryInfoShowModel
    {
        public PesticideInfoModel PesticideInfos { get; set; }
        public List<UseScope> UseScopes { get; set; }
    }

    public class UseScope
    {
        /// <summary>
        /// 作物名稱
        /// </summary>
        public string CropName { get; set; }
        /// <summary>
        /// 病蟲害名稱
        /// </summary>
        public string PestName { get; set; }
        /// <summary>
        /// 施用次數
        /// </summary>
        public string UseCount { get; set; }
        /// <summary>
        /// 每公頃使用用藥量
        /// </summary>
        public string PerHectares { get; set; }
        /// <summary>
        /// 稀釋倍數
        /// </summary>
        public string DilutionMultiple  { get; set; }
        /// <summary>
        /// 使用時期
        /// </summary>
        public string UseTime { get; set; }
        /// <summary>
        /// 施藥間隔
        /// </summary>
        public string UseInterval { get; set; }
        /// <summary>
        /// 安全採收期
        /// </summary>
        public string SaveHarvest { get; set; }
        /// <summary>
        /// 備註
        /// </summary>
        public string Remark { get; set; }
        /// <summary>
        /// 注意事項
        /// </summary>
        public string AttentionItem { get; set; }
    }

}