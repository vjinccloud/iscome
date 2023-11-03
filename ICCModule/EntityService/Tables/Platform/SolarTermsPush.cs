using ICCModule.EntityService.Service;
using IscomG2C.Utility;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Data.Linq.Mapping;
using System.Web.Mvc;

namespace ICCModule.Entity.Tables.Platform
{
    [Serializable]
    [Table(Name = "SolarTermsPush")]
    public class SolarTermsPush
    {
        /// <summary> 流水號
        ///
        /// </summary>
        [Column(IsPrimaryKey = true, IsDbGenerated = true, AutoSync = AutoSync.OnInsert)]
        public int Id { get; set; }

        /// <summary> 
        /// 節氣代碼，對應 defCode 的 Code
        /// </summary>
        [Column]
        [Required]
        public string SolarTermsCode { get; set; }

        /// <summary> 
        ///節氣名稱，對應 defCode 的 名稱
        /// </summary>
        [Column]
        [Required]
        public string SolarTermsName { get; set; }

        /// <summary> 
        /// 推播時間
        /// </summary>
        [Column]
        [Required]
        public DateTime PushDate { get; set; }

        /// <summary> 
        /// 是否進行推播
        /// </summary>
        [Column]
        [Required]
        public bool IsNeedPush { get; set; }

        /// <summary> 
        /// 作物名稱(逗號區分)
        /// </summary>
        [Column]
        public string CropName { get; set; }

        /// <summary> 
        /// 病蟲害(逗號區分)
        /// </summary>
        [Column]
        public string PestDisease { get; set; }

        /// <summary> 
        /// 推播內容
        /// </summary>
        [Column]
        public string PushContents { get; set; }

        /// <summary> 
        /// 資料主旨
        /// </summary>
        [Column]
        public string DataSubject { get; set; }

        /// <summary> 
        /// 發布日期
        /// </summary>
        [Column]
        public DateTime? ReleaseDate { get; set; }

        /// <summary> 內容
        ///
        /// </summary>
        [Column]
        [AllowHtml]
        [Required]
        public string DataContents { get; set; }

        /// <summary>
        /// 是否已推播
        /// </summary>
        [Column]
        [Required]
        public bool IsPushed { get; set; }

        /// <summary>
        /// 是否為匯入
        /// </summary>
        [Column]
        [Required]
        public bool IsImport { get; set; }

        /// <summary>
        /// 創建時間
        /// </summary>
        [Column]
        public DateTime CreateDate { get; set; }


        /// <summary>
        /// 更新時間
        /// </summary>
        [Column]
        public DateTime? UpdateDate { get; set; }
    }
}
