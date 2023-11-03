using ICCModule.Entity.Tables;
using ICCModule.Entity.Tables.Platform;
using ICCModule.EntityService.Service;
using ICCModule.Helper;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;

namespace InformationSystem.ViewModel
{
    #region 診斷/作物類別統計表
    public class DoctorCropStatisticModel
    {
        public List<cropType> CropTypes { get; set; } = Service_cropType.GetList();
        public DoctorCropStatisticQueryModel QueryReq { get; set; } = new DoctorCropStatisticQueryModel();
        public DataTable DataYear { get; set; } = new DataTable();
        public List<DoctorCropStatisticDataModel> DataList { get; set; } = new List<DoctorCropStatisticDataModel>();
    }
    public class DoctorCropStatisticQueryModel
    {
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public string CropType { get; set; } = "";
        public int QueryType { get; set; } = 1;
        public string ActionName { get; set; } = "";
    }
    public class DoctorCropStatisticDataModel
    {
        public string Title { get; set; } = "";
        public decimal Jan { get; set; } = 0;
        public decimal Feb { get; set; } = 0;
        public decimal Mar { get; set; } = 0;
        public decimal Apr { get; set; } = 0;
        public decimal May { get; set; } = 0;
        public decimal Jun { get; set; } = 0;
        public decimal Jul { get; set; } = 0;
        public decimal Aug { get; set; } = 0;
        public decimal Sep { get; set; } = 0;
        public decimal Oct { get; set; } = 0;
        public decimal Nov { get; set; } = 0;
        public decimal Dec { get; set; } = 0;
        public decimal Total { get; set; } = 0;
    }
    #endregion
    #region Inspection農藥殘留
    public class InspectionStatisticShowModel
    {
        public InspectionStatisticShowModel()
        {
            PlanTypeCode = Service_defCode.GetList("PlanType");
            Data = new List<InspectionStatisticModel>();
            QueryReq = new InspectionStatisticQueryModel();
        }
        /// <summary>
        /// 
        /// </summary>
        public InspectionStatisticQueryModel QueryReq { get; set; }
        /// <summary>
        /// 計畫別
        /// </summary>
        public List<defCode> PlanTypeCode { get; set; }
        public List<InspectionStatisticModel> Data { get; set; }
    }
    public class InspectionStatisticQueryModel
    {
        public InspectionStatisticQueryModel()
        {
            PlanType = new List<string>();
        }
        /// <summary>
        /// 
        /// </summary>
        public DateTime? StartDate { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public DateTime? EndDate { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public List<string> PlanType { get; set; }
    }
    public class InspectionStatisticModel
    {
        /// <summary>
        /// 採樣來源
        /// </summary>
        public string KeyName { get; set; }
        /// <summary>
        /// 採樣數
        /// </summary>
        public int TotalCount { get; set; }
        /// <summary>
        /// 合格數
        /// </summary>
        public int PassCount { get; set; }
        /// <summary>
        /// 未檢出數
        /// </summary>
        public int NoCheckCount { get; set; }
        /// <summary>
        /// 不合格數
        /// </summary>
        public int UnPassCount { get; set; }
        /// <summary>
        /// 超量核准
        /// </summary>
        public int OverApproved { get; set; }
        /// <summary>
        /// 未核准
        /// </summary>
        public int UnApproved { get; set; }
        /// <summary>
        /// 同時超量未核准
        /// </summary>
        public int SameOverUnApproved { get; set; }
        /// <summary>
        /// 禁用
        /// </summary>
        public int DisableCount { get; set; }

    }
    #endregion
    #region InvestCount調查次數
    public class InvestCountStatisticShowModel
    {
        public InvestCountStatisticShowModel()
        {
            Data = new List<InvestCountStatisticModel>();
        }
        public int QueryType { get; set; } = 1;
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public List<InvestCountStatisticModel> Data { get; set; }
    }
    public class InvestCountStatisticModel
    {
        public string Title { get; set; } = "";
        public int Jan { get; set; } = 0;
        public int Feb { get; set; } = 0;
        public int Mar { get; set; } = 0;
        public int Apr { get; set; } = 0;
        public int May { get; set; } = 0;
        public int Jun { get; set; } = 0;
        public int Jul { get; set; } = 0;
        public int Aug { get; set; } = 0;
        public int Sep { get; set; } = 0;
        public int Oct { get; set; } = 0;
        public int Nov { get; set; } = 0;
        public int Dec { get; set; } = 0;
        public int Total { get; set; } = 0;

    }
    #endregion
    #region InvestAnt琉璃蟻
    public class InvestAntStatisticShowModel
    {
        public InvestAntStatisticShowModel()
        {
            Data = new List<InvestAntStatisticModel>();
            Distict = new List<string>();
            DistictAll = DistrictHelper.KaohsiungDistrict();
        }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public List<string> Distict { get; set; }
        public List<string> DistictAll { get; set; }
        public List<InvestAntStatisticModel> Data { get; set; }
    }
    public class InvestAntStatisticModel
    {
        public string Title { get; set; } = "";
        public decimal Jan { get; set; } = 0;
        public decimal JanOther { get; set; } = 0;
        public decimal Feb { get; set; } = 0;
        public decimal FebOther { get; set; } = 0;
        public decimal Mar { get; set; } = 0;
        public decimal MarOther { get; set; } = 0;
        public decimal Apr { get; set; } = 0;
        public decimal AprOther { get; set; } = 0;
        public decimal May { get; set; } = 0;
        public decimal MayOther { get; set; } = 0;
        public decimal Jun { get; set; } = 0;
        public decimal JunOther { get; set; } = 0;
        public decimal Jul { get; set; } = 0;
        public decimal JulOther { get; set; } = 0;
        public decimal Aug { get; set; } = 0;
        public decimal AugOther { get; set; } = 0;
        public decimal Sep { get; set; } = 0;
        public decimal SepOther { get; set; } = 0;
        public decimal Oct { get; set; } = 0;
        public decimal OctOther { get; set; } = 0;
        public decimal Nov { get; set; } = 0;
        public decimal NovOther { get; set; } = 0;
        public decimal Dec { get; set; } = 0;
        public decimal DecOther { get; set; } = 0;

    }
    #endregion
}