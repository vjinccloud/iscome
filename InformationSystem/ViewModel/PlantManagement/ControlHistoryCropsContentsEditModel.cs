using ICCModule.Entity.Tables;
using ICCModule.Entity.Tables.Platform;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace InformationSystem.ViewModel.Management
{
    public class ControlHistoryCropsContentsEditModel
    {
        public ControlHistoryCropsContentsEditModel()
        {
            Id = 0;
            ControlHistoryCropId = 0;
            ControlStage = "";
            Name = "";
            ShowType = true;
            StartBlock = "";
            EndBlock = "";
        }
        /// <summary>
        /// 流水號
        /// </summary>
        public int Id { get; set; }

        /// <summary> 
        /// 防治曆作物序號
        /// </summary> 
        public int ControlHistoryCropId { get; set; }

        /// <summary> 
        /// 防治階段
        /// </summary> 
        public string ControlStage { get; set; }

        /// <summary> 
        /// 名稱
        /// </summary> 
        public string Name { get; set; }

        /// <summary> 
        /// 顯示方式
        /// </summary> 
        public bool ShowType { get; set; }

        /// <summary> 
        /// 區間-起
        /// </summary> 
        public string StartBlock { get; set; }

        /// <summary> 
        /// 區間-迄
        /// </summary> 
        public string EndBlock { get; set; }

        /// <summary> 
        /// 日月曆類型
        /// </summary> 
        public bool TypeId { get; set; }
        /// <summary> 
        /// 天數
        /// </summary> 
        public int? DayCount { get; set; }
        /// <summary> 
        /// 防治曆序號
        /// </summary> 
        public int ControlId { get; set; }
        /// <summary> 
        /// 
        /// </summary> 
        public string ActName { get; set; }

    }
}