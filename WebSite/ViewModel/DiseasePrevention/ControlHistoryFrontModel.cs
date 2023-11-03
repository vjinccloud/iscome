using ICCModule.Entity.Tables;
using ICCModule.Entity.Tables.Platform;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Website.ViewModel
{
    public class ControlHistoryFrontModel
    {
        /// <summary> 流水號
        /// 
        /// </summary> 
        public int ID { get; set; }

        /// <summary> 防治名稱
        /// 
        /// </summary> 
        public string Name { get; set; }

        /// <summary> 類型
        /// 
        /// </summary> 
        public string Type { get; set; }

        /// <summary> 
        /// 啟用狀態
        /// </summary> 
        public bool Enable { get; set; }

        /// <summary> 
        /// 點閱率
        /// </summary> 
        public int ClickCount { get; set; }

        /// <summary> 基本資料 RichText
        /// 
        /// </summary> 
        public string Information { get; set; }

        /// <summary> 說明 RichText
        /// 
        /// </summary> 
        public string Explanation { get; set; }

        /// <summary> 更新時間
        /// 
        /// </summary> 
        public DateTime? UpdatedAt { get; set; }

        /// <summary> 
        /// 防治曆
        /// </summary> 
        public List<ControlHistoryCropFrontModel> ControlHistoryCropFrontModels { get; set; }
    }

    public class ControlHistoryCropFrontModel
    {
        public ControlHistoryCropFrontModel()
        {
            Name = "";
            Sort = 0;
            Type = true;
            Comment = "";
            CollumHeads = new List<FrontCollumHead>();
            ContentGroups = new List<FrontContentGroup>();
        }

        /// <summary> 
        /// 流水號
        /// </summary> 
        public int ChcId { get; set; }
        
        /// <summary> 作物防治曆名稱
        /// 
        /// </summary> 
        public string Name { get; set; }

        /// <summary> 排序
        /// 
        /// </summary> 
        public int Sort { get; set; }

        /// <summary> 類型
        /// 
        /// </summary> 
        public bool Type { get; set; }

        /// <summary> 備註
        /// 
        /// </summary> 
        public string Comment { get; set; }

        /// <summary> 
        /// 天數
        /// </summary> 
        public int? DayCount { get; set; }

        /// <summary>
        /// 表頭
        /// </summary>
        public List<FrontCollumHead> CollumHeads { get; set; }
        /// <summary>
        /// 內容
        /// </summary>
        public List<FrontContentGroup> ContentGroups { get; set; }
    }
    public class FrontCollumHead
    {
        /// <summary>
        /// Key
        /// </summary>
        public string CollumName { get; set; }
        /// <summary>
        /// 內容
        /// </summary>
        public int Sort { get; set; }
    }
    public class FrontContentGroup
    {
        public FrontContentGroup()
        {
            ContentDatas = new List<FrontContentData>();
        }
        /// <summary>
        /// Key
        /// </summary>
        public string ContentName { get; set; }
        /// <summary>
        /// 排序
        /// </summary>
        public int Sort { get; set; }
        /// <summary>
        /// 內容
        /// </summary>
        public List<FrontContentData> ContentDatas { get; set; }
    }
    public class FrontContentData
    {
        /// <summary>
        /// 名稱
        /// </summary>
        public int Id { get; set; }
        /// <summary>
        /// 名稱
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// 寬度
        /// </summary>
        public decimal Width { get; set; }
        /// <summary>
        /// 左間格
        /// </summary>
        public decimal Margin { get; set; }
        /// <summary>
        /// 擺放格
        /// </summary>
        public int Sort { get; set; }
        /// <summary>
        /// 顯示方式
        /// </summary>
        public bool ShowType { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public int StartInt { get; set; }
        public int EndInt { get; set; }
    }
}