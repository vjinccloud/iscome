using ICCModule.Entity.Tables;
using ICCModule.Entity.Tables.Platform;
using ICCModule.EntityService.Service;
using ICCModule.Models;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;
using Website.Helper;

namespace Website.ViewModel.PlantDoctor
{
    public class CounselingUnitModel
    {
        public CounselingUnitModel()
        {
            Districts = CommonDataHelper.GetDistricts("高雄市");
            CounselingUnitList = Service_defCode.GetList("PlantDoctorCounselingUnit");
        }

        /// <summary>
        /// 行政區
        /// </summary>
        public List<District> Districts { get; set; }

        /// <summary>
        /// 耕作方式列表
        /// </summary>
        public List<defCode> CounselingUnitList { get; set; }
    }
}