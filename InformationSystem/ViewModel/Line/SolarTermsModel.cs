using ICCModule.Entity.Tables;
using ICCModule.Entity.Tables.Platform;
using ICCModule.EntityService.Service;
using ICCModule.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.UI.WebControls;
using Microsoft.Ajax.Utilities;

namespace InformationSystem.ViewModel
{
    /// <summary>
    /// 
    /// </summary>
    public class SolarTermsListModel
    {
        /// <summary>
        /// 查詢內容
        /// </summary>
        public SolarTermsQueryModel QueryReq { get; set; } = new SolarTermsQueryModel();
        /// <summary>
        /// 資料
        /// </summary>
        public List<SolarTermsPush> Data { get; set; } = new List<SolarTermsPush>();
        /// <summary>
        /// 資料
        /// </summary>
        public List<int> YearData { get; set; } = new List<int>();
        /// <summary>
        /// 節氣資料
        /// </summary>
        public List<defCode> SolarTermsType { get; set; } = Service_defCode.GetList("SolarTerms");
        /// <summary>
        /// 資料頁數
        /// </summary>
        public int TotalPage { get; set; }
    }
    /// <summary>
    /// 查詢
    /// </summary>
    public class SolarTermsQueryModel
    {
        /// <summary>
        /// 節氣
        /// </summary>
        public string SolarTermsCode { get; set; }
        /// <summary>
        /// 年度
        /// </summary>
        public int? Years { get; set; }
        /// <summary>
        /// 關鍵字
        /// </summary>
        public string KeyWord { get; set; }
        /// <summary>
        /// 頁數
        /// </summary>
        public int Page { get; set; } = 1;
    }

    public class SolarTermsEditView
    {
        /// <summary>
        /// 節氣資料
        /// </summary>
        public List<defCode> SolarTermsType { get; set; } = Service_defCode.GetList("SolarTerms");
        public SolarTermsEditModel Data { get; set; } = new SolarTermsEditModel();

    }
    public class SolarTermsEditModel
    {
        /// <summary>
        /// 流水號
        /// </summary>
        public int Id { get; set; }

        /// <summary> 
        /// 節氣代碼，對應 defCode 的 Code
        /// </summary>
        public string SolarTermsCode { get; set; }

        /// <summary> 
        ///節氣名稱，對應 defCode 的 名稱
        /// </summary>
        public string SolarTermsName { get; set; }

        /// <summary> 
        /// 推播時間
        /// </summary>
        public DateTime? PushDate { get; set; }

        /// <summary> 
        /// 是否進行推播
        /// </summary>
        public bool IsNeedPush { get; set; }

        /// <summary> 
        /// 作物名稱(逗號區分)
        /// </summary>
        public string CropName { get; set; }

        /// <summary> 
        /// 病蟲害(逗號區分)
        /// </summary>
        public string PestDisease { get; set; }

        /// <summary> 
        /// 推播內容
        /// </summary>
        public List<PushContentTab> PushContents { get; set; } = new List<PushContentTab>();

        /// <summary> 
        /// 資料主旨
        /// </summary>
        public string DataSubject { get; set; }

        /// <summary> 
        /// 發布日期
        /// </summary>
        public DateTime? ReleaseDate { get; set; }

        /// <summary> 
        /// 內容
        /// </summary>
        public string DataContents { get; set; }

        /// <summary>
        /// 是否已推播
        /// </summary>
        public bool IsPushed { get; set; }

        /// <summary>
        /// 是否為匯入
        /// </summary>
        public bool IsImport { get; set; }

    }
    /// <summary>
    /// 推播內容
    /// </summary>
    public class PushContentTab
    {
        /// <summary>
        /// 類型(文字/圖片)
        /// </summary>
        public int id { get; set; }

        /// <summary>
        /// 內容
        /// </summary>
        public int tabId { get; set; }
        /// <summary>
        /// 內容
        /// </summary>
        public string content { get; set; }
        /// <summary>
        /// 內容
        /// </summary>
        public string picId { get; set; }
        /// <summary>
        /// 內容
        /// </summary>
        public string path { get; set; }
    }

}