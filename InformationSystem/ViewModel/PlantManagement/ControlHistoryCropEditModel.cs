using ICCModule.Entity.Tables;
using ICCModule.Entity.Tables.Platform;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace InformationSystem.ViewModel.Management
{
    public class ControlHistoryCropEditModel
    {
        public ControlHistoryCropEditModel()
        {
            ID = 0;
            ControlID = 0;
            Name = "";
            Sort = 0;
            Show = true;
            Type = true;
            Comment = "";
            Context = "";
            CollumHeads = new List<CollumHead>();
            ContentGroups = new List<ContentGroup>();
        }
        /// <summary> 流水號
        /// 
        /// </summary> 
        public int ID { get; set; }

        /// <summary> 防治曆ID
        /// 
        /// </summary> 
        public int ControlID { get; set; }

        /// <summary> 作物防治曆名稱
        /// 
        /// </summary> 
        public string Name { get; set; }

        /// <summary> 排序
        /// 
        /// </summary> 
        public int Sort { get; set; }

        /// <summary> 顯示
        /// 
        /// </summary> 
        public bool Show { get; set; }

        /// <summary> 類型
        /// 
        /// </summary> 
        public bool Type { get; set; }

        /// <summary> 備註
        /// 
        /// </summary> 
        public string Comment { get; set; }

        /// <summary> 防治內容
        /// 
        /// </summary> 
        public string Context { get; set; }

        /// <summary> 
        /// 天數
        /// </summary> 
        public int? DayCount { get; set; }

        /// <summary> 
        /// 說明 RichText
        /// </summary> 
        public string ActionName { get; set; }
        /// <summary> 
        /// 說明 RichText
        /// </summary> 
        public string ContentId { get; set; }
        /// <summary>
        /// 表頭
        /// </summary>
        public List<CollumHead> CollumHeads { get; set; }
        /// <summary>
        /// 內容
        /// </summary>
        public List<ContentGroup> ContentGroups { get; set; }
    }
    public class CollumHead
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
    public class ContentGroup
    {
        public ContentGroup()
        {
            ContentDatas = new List<ContentData>();
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
        public List<ContentData> ContentDatas { get; set; }
    }
    public class ContentData
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