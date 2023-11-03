using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Website.Models.ExternalAPI
{
    /// <summary>
    /// 農委會資料開放平台-植物疫情預警
    /// </summary>
    public class PestNotice
    {
        /// <summary>
        /// 主旨
        /// </summary>
        /// <value>
        /// The title.
        /// </value>
        public string Title { get; set; }

        /// <summary>
        /// 疫情內容
        /// </summary>
        /// <value>
        /// The content.
        /// </value>
        public string Content { get; set; }

        /// <summary>
        /// 建議用藥
        /// </summary>
        /// <value>
        /// The suggested medication.
        /// </value>
        public string SuggestedMedication { get; set; }

        /// <summary>
        /// 發布縣市
        /// </summary>
        /// <value>
        /// The provider ciry.
        /// </value>
        public string ProvideCiry { get; set; }

        /// <summary>
        /// 植物品項
        /// </summary>
        /// <value>
        /// The type of the plant.
        /// </value>
        public string PlantType { get; set; }

        /// <summary>
        /// 發布日期
        /// </summary>
        /// <value>
        /// The provide date.
        /// </value>
        public string CreatedAt { get; set; }

        /// <summary>
        /// 發布單位
        /// </summary>
        /// <value>
        /// The provide unit.
        /// </value>
        public string ProvideUnit { get; set; }
    }

    public static class PestNoticeConfig
    {
        /// <summary>
        /// 農委會資料開放平台-植物疫情預警
        /// </summary>
        public static string PestNoticeDataUrl = "https://data.coa.gov.tw/Service/OpenData/FromM/PestNoticeData.aspx";
    }
}